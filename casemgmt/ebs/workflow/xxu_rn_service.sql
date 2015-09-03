--
-- Creating the temporary logging table
-- If there is an Exception here, ignore it. 
CREATE TABLE xx_temp (msg VARCHAR2 (4000));


--
-- Declaring the package
CREATE OR REPLACE PACKAGE xxu_rn_service
AS
  -- Constants
  -- Change this constant to the hostname you intend to allow for updates propagation.
  C_ALLOW_HOST    CONSTANT  VARCHAR2(1024) := 'CHANGEME';
  C_EVENT_NAME    CONSTANT  VARCHAR2(256) := 'oracle.apps.rn.callUpdateStatusRnSystem';
  
  FUNCTION raise_webservice (
    p_subscription_guid   IN     RAW,
    p_event               IN OUT wf_event_t
  ) RETURN VARCHAR2;
  
  FUNCTION propagate_note (
    p_subscription_guid   IN     RAW,
    p_event               IN OUT wf_event_t
  ) RETURN VARCHAR2;

END xxu_rn_service;
/
	
--
-- Package Body
CREATE OR REPLACE PACKAGE BODY xxu_rn_service
AS
   FUNCTION raise_webservice (
	  p_subscription_guid   IN     RAW,
	  p_event               IN OUT wf_event_t
   )
	  RETURN VARCHAR2
   IS
    
    -- Local Variables
	  l_plist           wf_parameter_list_t := p_event.getparameterlist ();
    l_timestamp       VARCHAR2(256);
    
    l_request_number  VARCHAR2(256);
    l_username        VARCHAR2(100);
    l_sr_owner_name   CS_SERVICE_REQUEST_PUB_V.OWNER_NAME%TYPE;
    l_sr_status       CS_SERVICE_REQUEST_PUB_V.STATUS%TYPE;  -- vc2(90)
    l_sr_status_id    CS_INCIDENTS_ALL_B.INCIDENT_STATUS_ID%TYPE;  -- N(15)
    l_rninc_host      CS_INCIDENTS_ALL_B.EXTERNAL_ATTRIBUTE_13%TYPE;
    l_rninc_refno     CS_INCIDENTS_ALL_B.EXTERNAL_ATTRIBUTE_14%TYPE;
    l_rninc_id        CS_INCIDENTS_ALL_B.EXTERNAL_ATTRIBUTE_15%TYPE; -- VC2(150)

    l_rninc_status_id   NUMBER;
    l_rninc_statusxml   VARCHAR2(256) := '';
	  l_ws_parameters   wf_parameter_list_t;
	  l_ws_event_data		CLOB; -- gets sent by manual raise event.
    l_note_text       VARCHAR2(256) := NULL;
    l_extra_log       VARCHAR2(450) := '';
	BEGIN
    l_timestamp:=  fnd_date.date_to_canonical (SYSDATE);
    
    -- get the Req Number
    l_request_number := wf_event.getvalueforparameter (
						  'REQUEST_NUMBER',
						  l_plist
					   );
    l_username := wf_event.getvalueforparameter (
						  'INITIATOR_ROLE',
						  l_plist
					   );

    -- Skipping the user used on Status Change
    IF l_username = 'EBUSINESS' THEN
  	  RETURN 'SUCCESS';
    END IF;
             
    -- A separate block for the select/join exception
    -- Matching by internal SR_V.SERVICE_REQUEST_ID.
    BEGIN
      select SR_V.STATUS, INC.INCIDENT_STATUS_ID, SR_V.OWNER_NAME,
        lower(INC.EXTERNAL_ATTRIBUTE_13),      
        INC.EXTERNAL_ATTRIBUTE_14, INC.EXTERNAL_ATTRIBUTE_15
      into l_sr_status, l_sr_status_id, l_sr_owner_name,
        l_rninc_host, l_rninc_refno, l_rninc_id
      from CS_SERVICE_REQUEST_PUB_V SR_V, CS_INCIDENTS_ALL_B INC
      where SR_V.SERVICE_REQUEST_NUMBER = l_request_number
        AND INC.INCIDENT_ID = SR_V.SERVICE_REQUEST_ID;
    EXCEPTION
      WHEN NO_DATA_FOUND THEN
        l_rninc_id := null;
    END;

    -- Skipping if the host is different (host filtering).
    -- doesn't allow any SRs with empty Hostname 
    IF l_rninc_host = '' OR l_rninc_host IS NULL OR l_rninc_host != C_ALLOW_HOST
    THEN
      BEGIN
        INSERT INTO xx_temp (msg)
          VALUES (
               'Timestamp: '
            || l_timestamp
            || ' | NOT Propagating: '
            || p_event.geteventname ()
            || ' | Event Key: '
            || p_event.geteventkey ()
            || ' | REQUEST_NUMBER: '
            || l_request_number
            || ' | RN_HOST: '
            || l_rninc_host
        );
      EXCEPTION
        WHEN OTHERS THEN
          l_rninc_status_id := 0; -- DO NOTHING
      END;
  	  RETURN 'SUCCESS';
    END IF;
    
    -- Logging inside the Custom subscription code
    BEGIN
      IF p_event.geteventname () LIKE '%reassigned' THEN
        -- adding stuff to log
        l_extra_log := ' | OWNER: ' || l_sr_owner_name;
        l_note_text := 'EBS Service Request '||l_request_number          
                ||' assigned to: ' ||l_sr_owner_name
                ||' on ' || l_timestamp;
      END IF;
      
      INSERT INTO xx_temp (msg)
        VALUES (
					   'Timestamp: '
					|| l_timestamp
					|| ' | Propagating: '
					|| p_event.geteventname ()
					|| ' | Event Key: '
					|| p_event.geteventkey ()
					|| ' | REQUEST_NUMBER: '
					|| l_request_number
					|| ' | RNINC REF No: '
          || l_rninc_refno
					|| ' | RNINC_ID: '
					|| l_rninc_id
					|| ' | STATUS: '
					|| l_sr_status
          || ' | RN_HOST: '
          || l_rninc_host
          || l_extra_log
      );        
    EXCEPTION
      WHEN OTHERS THEN
        l_rninc_status_id := 0; -- DO NOTHING
    END;
    
    -- deciding the RN status code:
    -- send Solved on 'Closed'; nothing on 'Reopened'; Unresolved on everything else
    -- INC.INCIDENT_STATUS_ID : 1 - Open, 2 - Closed
    -- 6 - Engineer On-Site, 7 - Resolution In Progress
    -- 102 - Reopened 103 - Working, Escalated: 160
    IF l_sr_status_id = 2  THEN
      l_rninc_status_id := 2;
    ELSIF l_sr_status_id = 102  THEN 
      l_rninc_status_id := 0; -- Reopened will send a different Status:Updated.
    ELSE
      l_rninc_status_id := 1;
    END IF;

    -- this block decides that we're doing 'Status Update', 
    -- otherwise it is 'Owner update'.
    IF l_note_text IS NULL THEN
      l_note_text := 'Status change for EBS SR number '||l_request_number
              ||' on ' || l_timestamp
              ||', status now is: ' ||l_sr_status;
    
      IF l_rninc_status_id <> 0 THEN
        l_rninc_statusxml := 
          '<ns4:StatusWithType>
            <ns4:Status><ns1:ID id="'||l_rninc_status_id||'"/></ns4:Status>
          </ns4:StatusWithType>';
      ELSE
        -- Sending a named status type 'Updated':
        l_rninc_statusxml := 
          '<ns4:StatusWithType>
            <ns4:Status><ns1:Name>Updated</ns1:Name></ns4:Status>
          </ns4:StatusWithType>';
      END IF;
    END IF;
            
		l_ws_parameters :=
			wf_parameter_list_t (
				wf_parameter_t ('WFBES_INPUT_request_header', 
					'<ns7:ClientInfoHeader xmlns:ns7="urn:messages.ws.rightnow.com/v1_2"  soapenv:mustUnderstand="0" xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"> 
					<ns7:AppID>PL/SQL Update</ns7:AppID>
					</ns7:ClientInfoHeader>')
			);

		l_ws_event_data := '
<ns7:Update xmlns:ns7="urn:messages.ws.rightnow.com/v1_2">
      <ns7:RNObjects xmlns:ns4="urn:objects.ws.rightnow.com/v1_2"
          xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
          xsi:type="ns4:Incident"
          xmlns:ns1="urn:base.ws.rightnow.com/v1_2">
        <ns1:ID id="'||l_rninc_id||'" />'
        ||l_rninc_statusxml
        ||'<ns4:Threads>
          <ns4:ThreadList action="add">
            <ns4:EntryType>
              <ns1:Name>Note</ns1:Name>
            </ns4:EntryType>
            <ns4:Text>'||l_note_text||'</ns4:Text>
          </ns4:ThreadList>
        </ns4:Threads>        
      </ns7:RNObjects>
      <ns7:ProcessingOptions>
        <ns7:SuppressExternalEvents>false</ns7:SuppressExternalEvents>
        <ns7:SuppressRules>false</ns7:SuppressRules>
      </ns7:ProcessingOptions>
    </ns7:Update>';
    
		wf_event.raise (
			p_event_name   => C_EVENT_NAME, 
			p_event_key    => SYS_GUID (),
			p_event_data   => l_ws_event_data,
			p_parameters   => l_ws_parameters
			);
    COMMIT;

	  RETURN 'SUCCESS';
   EXCEPTION
	  WHEN OTHERS THEN
		 wf_core.context (
			'xxu_rn_service',
			'raise_webservice',
			p_event.geteventname (),
			p_subscription_guid
		 );
		 wf_event.seterrorinfo (p_event, 'ERROR');
		 RETURN 'ERROR';
   END raise_webservice;

-- ------------------------------------ --
-- Second function; to propagate notes. --
-- ------------------------------------ --
FUNCTION propagate_note(
    p_subscription_guid IN RAW,
    p_event             IN OUT wf_event_t )
  RETURN VARCHAR2
IS

  -- Local Variables
  l_plist           wf_parameter_list_t := p_event.getparameterlist ();
  l_timestamp       VARCHAR2(256);
  
  l_ebsinc_id       VARCHAR2(256);
  l_jtf_note_id     VARCHAR2(100);
  l_sr_note         JTF_NOTES_VL.NOTES%TYPE;
  l_sr_note_detail_clob JTF_NOTES_VL.NOTES_DETAIL%TYPE;
  l_sr_note_upd_by  JTF_NOTES_VL.LAST_UPDATED_BY%TYPE;
  l_sr_note_detail  VARCHAR2(32767);
  l_rninc_host      CS_INCIDENTS_ALL_B.EXTERNAL_ATTRIBUTE_13%TYPE;
  l_rninc_refno     CS_INCIDENTS_ALL_B.EXTERNAL_ATTRIBUTE_14%TYPE;
  l_rninc_id        CS_INCIDENTS_ALL_B.EXTERNAL_ATTRIBUTE_15%TYPE; -- VC2(150)

  l_ws_parameters   wf_parameter_list_t;
  l_ws_event_data		CLOB; -- gets sent by manual raise event.
BEGIN
  l_timestamp:= fnd_date.date_to_canonical (SYSDATE);
  
  -- get relevant data from the Event parameters:
  l_ebsinc_id := wf_event.getvalueforparameter (
            'SOURCE_OBJECT_ID',
            l_plist
           );
  l_jtf_note_id := wf_event.getvalueforparameter (
            'NOTE_ID',
            l_plist
           );
  
  -- Getting the note content.
  BEGIN
    select A.notes, A.notes_detail, A.LAST_UPDATED_BY,
      lower(INC.EXTERNAL_ATTRIBUTE_13),INC.EXTERNAL_ATTRIBUTE_14, INC.EXTERNAL_ATTRIBUTE_15
    into l_sr_note, l_sr_note_detail_clob, l_sr_note_upd_by,
      l_rninc_host, l_rninc_refno, l_rninc_id
    from jtf_notes_vl A, cs_incidents_All_b INC
    where 
      JTF_NOTE_ID = l_jtf_note_id AND     --incoming from event
      A.SOURCE_OBJECT_CODE='SR' AND 
      A.SOURCE_OBJECT_ID=l_ebsinc_id AND  --incoming
      A.SOURCE_OBJECT_ID = INC.INCIDENT_ID
      AND A.ENTERED_BY_NAME NOT LIKE '%(EBUSINESS,%';  -- skipping EBUSINESS changes.
    
    l_sr_note_detail := dbms_lob.substr(l_sr_note_detail_clob, 32000, 1);
  EXCEPTION
    WHEN NO_DATA_FOUND THEN
      l_rninc_id := null;
      l_rninc_refno := 'EEE-EEE';
      l_sr_note_detail  := '';
      l_sr_note_upd_by := 0;      
  END;
           
  -- Skipping the note if the user is EBUSINESS (created from CX) 
  -- OR if it is user '0', which is SYSADMIN (created from CP 'customer')
  -- Also, 
  -- Skipping if the host is different (host filtering).
  -- doesn't allow any SRs with empty Hostname.
  IF l_sr_note_upd_by = 0 OR
    l_rninc_host = '' OR l_rninc_host IS NULL OR l_rninc_host != C_ALLOW_HOST
  THEN
    BEGIN
      INSERT INTO xx_temp (msg)
        VALUES (
             'Timestamp: '
          || l_timestamp
          || ' | NOT Propagating: '
          || p_event.geteventname ()
          || ' | Event Key: '
          || p_event.geteventkey ()
          || ' | EBS REQUEST ID: '
          || l_ebsinc_id
          || ' | RNINC REF No: '
          || l_rninc_refno
          || ' | RN_HOST: '
          || l_rninc_host
      );
    EXCEPTION
      WHEN OTHERS THEN
        l_rninc_refno := 'EEE-EEN'; -- do nothing
    END;
    RETURN 'SUCCESS';
  END IF;
  
  -- Logging inside the Custom subscription code (if propagating the Note)
  BEGIN
    INSERT INTO xx_temp (msg)
      VALUES (
           'Timestamp: '
        || l_timestamp
        || ' | Propagating: '
        || p_event.geteventname ()
        || ' | Event Key: '
        || p_event.geteventkey ()
        || ' | EBS REQUEST ID: '
        || l_ebsinc_id
        || ' | NOTE_ID: '
        || l_jtf_note_id
        || ' | RNINC REF No: '
        || l_rninc_refno
        || ' | RNINC_ID: '
        || l_rninc_id
        || ' | RN_HOST: '
        || l_rninc_host
    );        
  EXCEPTION
    WHEN OTHERS THEN
      l_rninc_refno := 'EEE-EEL'; -- do nothing
  END;

  -- EBS saves either an empty detail field; or concatenated note+detail.
  IF l_sr_note_detail = '' OR l_sr_note_detail IS NULL THEN
    l_sr_note_detail := l_sr_note;  -- just using the note in the detail var.
  END IF;
  
  l_ws_parameters :=
    wf_parameter_list_t (
      wf_parameter_t ('WFBES_INPUT_request_header', 
        '<ns7:ClientInfoHeader xmlns:ns7="urn:messages.ws.rightnow.com/v1_2" soapenv:mustUnderstand="0" xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"> 
        <ns7:AppID>PL/SQL Update</ns7:AppID>
        </ns7:ClientInfoHeader>')
    );

  -- using the Note Detail variable(b/c above). 
  l_ws_event_data := 
'<ns7:Update xmlns:ns7="urn:messages.ws.rightnow.com/v1_2">
    <ns7:RNObjects xmlns:ns4="urn:objects.ws.rightnow.com/v1_2"
        xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
        xsi:type="ns4:Incident"
        xmlns:ns1="urn:base.ws.rightnow.com/v1_2">
      <ns1:ID id="'||l_rninc_id||'" />'
      ||'<ns4:Threads>
        <ns4:ThreadList action="add">
          <ns4:EntryType>
            <ns1:Name>Note</ns1:Name>
          </ns4:EntryType>
          <ns4:Text>'||l_sr_note_detail||'</ns4:Text>
        </ns4:ThreadList>
      </ns4:Threads>        
    </ns7:RNObjects>
    <ns7:ProcessingOptions>
      <ns7:SuppressExternalEvents>false</ns7:SuppressExternalEvents>
      <ns7:SuppressRules>false</ns7:SuppressRules>
    </ns7:ProcessingOptions>
  </ns7:Update>';
  
  wf_event.raise (
    p_event_name   => C_EVENT_NAME, 
    p_event_key    => SYS_GUID (),
    p_event_data   => l_ws_event_data,
    p_parameters   => l_ws_parameters
    );
  
  COMMIT;
  RETURN 'SUCCESS';
EXCEPTION
WHEN OTHERS THEN
  wf_core.context ( 'xxu_rn_service', 'propagate_note', p_event.geteventname (), p_subscription_guid );
  wf_event.seterrorinfo (p_event, 'ERROR');
  RETURN 'ERROR';
END propagate_note;

END xxu_rn_service;
/

