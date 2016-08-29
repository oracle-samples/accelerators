/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: Mobile Agent App Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.8 (August 2016)
 *  MAF release: 2.3
 *  reference: 151217-000185
 *  date: Tue Aug 23 16:35:58 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: 83880f31c32328928fda808409a666aeee03057d $
 * *********************************************************************************************
 *  File: IncidentAttachmentsController.java
 * *********************************************************************************************/

package incidents;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

import javax.el.ValueExpression;

import oracle.adfmf.amx.event.RangeChangeEvent;
import oracle.adfmf.amx.event.RangeChangeListener;
import oracle.adfmf.bindings.dbf.AmxIteratorBinding;
import oracle.adfmf.bindings.iterator.BasicIterator;
import oracle.adfmf.framework.api.AdfmfContainerUtilities;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;

import oracle.adfmf.java.beans.PropertyChangeListener;
import oracle.adfmf.java.beans.PropertyChangeSupport;
import oracle.adfmf.java.beans.ProviderChangeSupport;
import oracle.adfmf.java.beans.ProviderChangeListener;


import rest_adapter.RestAdapter;

import oracle.adfmf.json.JSONException;
import oracle.adfmf.json.JSONObject;

import report.ReportController;

import report.ReportItem;

public class IncidentAttachmentsController implements RangeChangeListener {
    public static final String QUERY_URL_CONNECTION =
        (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_URL_CONNECTION}");
    private transient PropertyChangeSupport propertyChangeSupport = new PropertyChangeSupport(this);
    private transient ProviderChangeSupport providerChangeSupport = new ProviderChangeSupport(this);

    private List<Attachment> _attchList = null; // working dynamic list
    private Attachment[] _attachment;  // static returned array.

    private String _incidentId = "0";
    
    private ReportController _reportCtrl = null;
    private String _reportName;
    private String _dataControl;
    private String _filterName;

    public IncidentAttachmentsController() {
        super();
    }

    public void addPropertyChangeListener(PropertyChangeListener l) {
        propertyChangeSupport.addPropertyChangeListener(l);
    }

    public void removePropertyChangeListener(PropertyChangeListener l) {
        propertyChangeSupport.removePropertyChangeListener(l);
    }
    
    public void addProviderChangeListener(ProviderChangeListener l) {
        providerChangeSupport.addProviderChangeListener(l);
    }
    
    public void removeProviderChangeListener(ProviderChangeListener l) {
        providerChangeSupport.removeProviderChangeListener(l);
    }
    
    /**
     * This is for init.  Needed for refresh.
     *
     * You can have filterName + filterValue but this function always should get the Incident ID as the value.
     * @param filternName = eg "incidentId"
     * @param filterValue - Incident ID
     * @param reportName = should be the report name used for att list, e.g. AcceleratorIncidentAttachmentList
     * @param dataControlName - should simply be this Controller's name, just providing for future extensions
     */
    public void initAttachmentsList(String filterName, String filterValue) {
        System.out.println("initAttachmentsList called; setting Incident ID/key:" +filterValue);
        this._incidentId  = filterValue;
        this._filterName = filterName;
        // forcing reload of the list; null internal List, empty external array + fire event:
        this._attchList = null;
        setAttachment(new Attachment[0]);
        
        this._reportName = "AcceleratorIncidentAttachmentList";
        this._dataControl = "IncidentAttachmentsController";
        
        _reportCtrl = new ReportController();
        _reportCtrl.initReport(_reportName, _dataControl, filterName, filterValue);
    }

    /**
     *
     */
    public void deleteAttachmentWithPopup(String incidentId, String attId) {
        System.out.println("deleteAttachmentWithPopup(String attId "+attId+ " and incId "+incidentId);
        
        String deleteMsg = "Are you sure you want to delete?";
        AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                                                "yesNoForDeletion", 
                                                                new Object[] {deleteMsg, "", incidentId, attId });
        
        return;
    }

    /**
     * no popup, danga, will delete!
     * Public func called from JavaScipt, class might be null.
     * @param attId
     */
    public void deleteAttachment(String incId, String attId) {
        // member var 0 
        System.out.println("deleteAttachment called for att id: "+attId + " . Member var _incidentId:"+_incidentId + " in "+this);

        this._incidentId = incId; // needzzz..
        // danger oth can all
        if (attId==null || "".equals(attId)) {
            System.out.println("deleteAttachment NOT deleting, empty Object ID!");
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                    "navigator.notification.alert",
                                    new Object[] {"Error in the Attachment Id!", null, "Deletion Alert", "OK"});
            return;
        }

        String restResource = "/incidents";
        String restRequestURI = restResource + "/" +_incidentId + "/fileAttachments/"+attId;
        try{
            System.out.println("uri/deleteAttachment -> " + restRequestURI);

            String response = RestAdapter.doDELETE(IncidentAttachmentsController.QUERY_URL_CONNECTION, restRequestURI);
            System.out.println("response/deleteAttachment JSONlen: " + (response==null?"null":response.length())); // del OK
                        
            /*
            try {
                
                _attchList = removeIdFromAttachments(_attchList, attId);
                //making new array
                Attachment[] newAttArr = new Attachment[ _attchList.size() ];
                providerChangeSupport.fireProviderDelete("attachment", attId);
                
                // Setting. Refresh done by flow. I think.
                this.setAttachment( _attchList.toArray( newAttArr ) );

                // additional try :
                // setShowRefreshNeeded("true");

                // AdfmfJavaUtilities.flushDataChangeEvent(); // nah
            } catch (Exception e) {
                System.out.println("Exception when firing on List in DELETEAttach: "+e);
                e.printStackTrace();
            }
            */
        }catch (Exception e) {
            System.out.println("Better luck next time from DELETEAttach: "+e);
            e.printStackTrace();
        }
        
        // since using report, we need to re-create and re-init its control before doing the refresh below; would be empty eitherway.
        AdfmfJavaUtilities.setELValue("#{applicationScope.myAttachmentsRefresh}", incId);
        
        //Refresh Attachment List after deleting an attachment
        ValueExpression iter = AdfmfJavaUtilities.getValueExpression("#{bindings.attachmentIterator}", Object.class);
        AmxIteratorBinding iteratorBinding = (AmxIteratorBinding)iter.getValue(AdfmfJavaUtilities.getELContext());
        iteratorBinding.getIterator().refresh();
    }

    /**
     * this downloads an attachment  and sends back z data
     *
     * @param incidentId
     * @param attachmentId
     */
    public String getAttachmentData(String incidentId, String attachmentId) {
        System.out.println("getAttachmentData  called for incidentId:"+incidentId+" attachmentID:"+attachmentId
                           +" . Member var _incidentId:"+_incidentId);

        if (attachmentId == null || incidentId == null)
            return "";
        this._incidentId = incidentId;  //  no member, first call. 

        String base64data = "c2FtcGxlIGNvbnRlbnQgZm9yIGZpbGUgYXR0YWNobWVudA==";//sample content for file attachment
        String restResource = "/incidents";
        String restResourceURI = restResource + "/" +_incidentId + "/fileAttachments/"+attachmentId+"/data";

        try {
            System.out.println("uri/getAttachmentData(base64) -> " + restResourceURI);

            String response = RestAdapter.doGET(IncidentAttachmentsController.QUERY_URL_CONNECTION, restResourceURI);
            System.out.println("response/getAttachmentData(base64) JSONlen: " + (response==null?"null":response.length()));

            if (response != null) {
                JSONObject attAttribs = new JSONObject(response);

                try {
                    base64data = attAttribs.getString("data");
                } catch (JSONException j) {
                    System.out.println("JSONException in attrs in loadAttach: "+ j);
                }
            }

        }catch (Exception e) {
            System.out.println("Better luck next time from getAttachmentData: ");
            e.printStackTrace();
        }
        return base64data;
    }


    /**
     *sets the record given in the dataProvider highlited value dot .getId()
     * to the expression in the elExpression
     * @param highlightedRecord
     * @param elExpression
     */
    public void setSelectedRecordById(ReportItem highlightedRecord, String elExpression) {
        elExpression = "#{" + elExpression + "}";
        if(highlightedRecord != null) {
            System.out.println("setSelectedRecordById, highlightedRecord value is:"+highlightedRecord.getId());
            AdfmfJavaUtilities.setELValue(elExpression, highlightedRecord.getId());
        } else {
            System.out.println("setSelectedRecordById, highlightedRecord  is:"+highlightedRecord);
        }
    }

    /**
     * this func is used to get the item , to get a list, for a list of Attachments.
     * Also assigns to the internal _attachment array.
     *
     * @return a list
     */
    public Attachment[] getAttachment() {
        System.out.println("getAttachment in "+this);  // single-word DBG MSG FTW

        Object obj = AdfmfJavaUtilities.getELValue("#{applicationScope.myAttachmentsRefresh}");
        if ( (obj instanceof Boolean && ((Boolean)obj)) || (obj instanceof String && obj.equals("true")) )  {
            AdfmfJavaUtilities.setELValue("#{applicationScope.myAttachmentsRefresh}", false);
            _reportCtrl.initReport(_reportName, _dataControl, _filterName, _incidentId);
        }

        this._attchList = new ArrayList<Attachment>();
        //loadData - later versions this is a report fetch
        loadListOfAttachments();
        _attachment = _attchList.toArray(new Attachment[_attchList.size()]);
        
        if (_attachment.length == 0) {
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.isNoDataFoundIncidentAttachmentsController}", true);
        } else {
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.isNoDataFoundIncidentAttachmentsController}", false);
        }
        
        return _attachment;
    }

    /**
     *
     * @param _attachment
     */
    public void setAttachment(Attachment[] _attachment) {
        Attachment[] oldAttachment = this._attachment;
        this._attachment = _attachment;
        propertyChangeSupport.firePropertyChange("attachment", oldAttachment, _attachment);
    }

    /**
     * this - is private method.
     */
    private void loadListOfAttachments() {
        try{
            // gets report, has responses.  Dates in report are conv. already/
            ReportItem[] items = _reportCtrl.getReport();
            System.out.println("response/loadListOfAttachments array: " + (items==null?"null":items.length)  + " items");
            
            if (items != null) {
                for (int i = 0; i < items.length ; i++) {
                    ReportItem item = items[i];
                    // att ID
                    String id = item.getId();
                    String attURL = "/incidents/"+_incidentId+"/fileAttachments/"+ id;
                    Attachment att = new Attachment(attURL);
                    
                    att.setId(id);
                    att.setSize(item.getAttr1());
                    att.setFileName(item.getAttr2());
                    att.setCreatedTime(item.getAttr3());
                    System.out.println("Attachment id "+att.getId() +  " url "+attURL + " name "+att.getFileName());
                    
                    if (att.getId()!=null)    // for some reason TB Q returns null fields ?
                        _attchList.add(att);
                }
                
                System.out.println("response/loadListOfAttachments array done.");

            }

        }catch (Exception e) {
            System.out.println("Better luck next time from IncAttContr: ");
            e.printStackTrace();
        }
    }

    /* *
     * removes an attachment by ID
     * @param attachmentsList
     * @param idValue
     * @return new list
     * /
    private List<Attachment> removeIdFromAttachments(List<Attachment> attachmentsList, String idValue) {
        Iterator<Attachment> iter = attachmentsList.iterator();
        while (iter.hasNext()) {
            Attachment att = iter.next();
            if (att.getId().equalsIgnoreCase(idValue)) {
                iter.remove();
                System.out.println("Removed from att list->id: " + idValue);
            }
        }
        return attachmentsList;
    }
    */

    @Override
    public void rangeChange(RangeChangeEvent rangeChangeEvent) {
        System.out.println("rangeChange IncidentAttachmentsController");

        if (_reportCtrl != null)
            _reportCtrl.rangeChange(rangeChangeEvent);
        else 
            System.out.println("rangeChange IncidentAttachmentsController reportCtrl is null!");
    }
}
