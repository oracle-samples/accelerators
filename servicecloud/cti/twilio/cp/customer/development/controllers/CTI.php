<?php

/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set 
 *  published by Oracle under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: Telephony and SMS Accelerator for Twilio
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17D (November 2017)
 *  reference: 161212-000059
 *  date: Monday Oct 30 13:8:16 UTC 2017
 *  revision: rnw-17-11-fixes-releases
 * 
 *  SHA1: $Id: 9a56003f4193af8fc60055b979d34a4d2c996578 $
 * *********************************************************************************************
 *  File: CTI.php
 * ****************************************************************************************** */

namespace Custom\Controllers;

use RightNow\Connect\v1_3 as Connect;
use RightNow\Utils\Config;
use RightNow\Models;

require_once __DIR__ . "/../libraries/CTI/CTIConfig.php";
require_once __DIR__ . "/../libraries/CTI/IVRConfig.php";

class CTI extends \RightNow\Controllers\Base
{
    private static $connect;
    private static $base;
    function __construct()
    {
        parent::__construct();
        \CTIConfig::readConfig();
        self::$base = "https://" . $_SERVER['HTTP_HOST'] . "/cc/CTI/";
        load_curl();
        
        
        $this->load->helper('twilio_rest');
        self::$connect = $this->model("custom/CTIConnect") ;
    }

    /**
     * This function helps us to automatically authenticate calls from 
     * BUI or CX console. This helps us to prevent abuse and unauthorized
     * URL access. 
     *
     * @param: NIL
     * @return: boolean
     */
    private function isValidAgent()
    {
        if( $this->_getAgentSessionId() === false )
        {
            return false;
        }
        if( $this->account === false )
        {
            return false;
        }

        return true;
    }

    /**
     * This function generates a worker name - a Twilio identifier corresponds to 
     * agents in OSvC
     *
     * @param: NIL
     * @return: string
     */
    private function getWorkerName( $id = false )
    {
        if( $id === false )
        {
            $id = $this->account->acct_id;
        }
        return "agent_" . $id;
    }

    /**
     * This function generates some connon routes and return it with the login call. 
     * It greatly hepls to change certain routes from the server side without causing any client 
     * code change.
     *
     * @param: NIL
     * @return: array
     */
    private function getAccessRoutes()
    {
        return [
            "login"         => self::$base . "login",
            "setActivity"   => self::$base . "setActivity"
            
        ];
    }

    /**
     * This function validates the CTI access for a particular account using the session_id
     * passed to the script as a post parameter
     *
     * @param: NIL
     * @return: boolean (May exit the program if there is no proper access)
     */
    private function validateCTIAccess()
    {
        $message = "<h1>Forbidden</h1>";

        if( IS_DEVELOPMENT )
        {
            return true;
        }
        if( $this->isValidAgent() )
        {
            $account = Connect\Account::fetch( $this->account->acct_id );

            if( $account->CustomFields->Accelerator->bui_cti_access == true )
            {
               return true;
            }
            else
            {
                $message = "<h1>CTI access denied for the agent! Please contact your administrator to get access.</h1>";
            }
        }

        header("HTTP/1.0 403 Forbidden");
        echo $message;
        exit();
    }

    /**
     * This function helps to validate the SMS access for a particular agent using the 
     * session_id parameter passed to the script as a post parameter
     *
     * @param: NIL
     * @return: boolean (May exit the program if there is no proper access)
     */
    private function validateSMSAccess()
    {
        $message = "<h1>Forbidden</h1>";
        if( $this->isValidAgent() )
        {
            $account = Connect\Account::fetch( $this->account->acct_id );

            if( $account->CustomFields->Accelerator->bui_cti_access == true )
            {
               return true;
            }
            else
            {
                $message = "<h1>CTI access denied for the agent! Please contact your administrator to get access.</h1>";
            }
        }

        header("HTTP/1.0 403 Forbidden");
        echo $message;
        exit();
    }

    /**
     * This function validates the agent and and return the status if CTI is enabled
     * for the agent.
     *
     * @url: cu/CTI/isCTIEnabled
     * @param: session_id
     * @return: bool. True if CTI is enabled for the user
     */
    public function isCTIEnabled()
    {
        $result = [
            "enabled" => false
        ];

        if( $this->isValidAgent() )
        {
            $account = Connect\Account::fetch( $this->account->acct_id );

            if( $account->CustomFields->Accelerator->bui_cti_access == true )
            {
                $result = [
                    "enabled" => true
                ];
            }
        }

        echo json_encode( $result, JSON_PRETTY_PRINT );
    }

    /**
     * This function validates the agent and generate appropriate Twilio tokens 
     * for the agent to connect to Twilio for CTI
     *
     * @url: cu/CTI/login
     * @param: session_id
     * @return: Twilio device capability token, Twilio worker token, Twilio ICE server details
     */
    public function login()
    {
        $result = [];

        if( $this->isValidAgent() )
        {
            $account = Connect\Account::fetch( $this->account->acct_id );

            if( $account->CustomFields->Accelerator->bui_cti_access == true )
            {
               $name = $this->getWorkerName();
        
                $worker_token = \TwilioHelper::getWorkerCapabilityToken( $name, 600 );
                if( $worker_token === false )
                {
                    \TwilioHelper::addWorker( $name );
                    $worker_token = \TwilioHelper::getWorkerCapabilityToken( $name, 600 );
                }
                
                $result = [
                    "success" => true,
                    "device" => \TwilioHelper::getDeviceCapabilityToken( $name, true ),
                    "worker" => $worker_token,
                    "ICE"    => \TwilioHelper::getICEServers(),

                    "config" => [
                        "number"    => twilio_phone_number,
                        "routes"    => $this->getAccessRoutes()
                    ]
                ];
            }
            else
            {
                $result = [
                    "success" => false,
                    "message" => "CTI access denied for the agent! Please contact your administrator to get access."
                ];
            }
        }
        else
        {
            $result = [
                "success" => false,
                "message" => "Access denied."
            ];
        }

        echo json_encode( $result, JSON_PRETTY_PRINT );
    }

    /**
     * This function generates the requested token
     *
     * @param: tokens - String tokens to be generated ( client | worker )
     * @return: JSON token string
     */
    function renewTokens()
    {
        $this->validateCTIAccess();
        
        $name = $this->getWorkerName();
        $tokens = $this->input->post("tokens");

        if( $tokens === false )
        {
            header("HTTP/1.0 400 Invalid Request");
            exit();
        }

        $tokens = explode( ",", $tokens );

        $result = [];
        foreach( $tokens as $idx=>$token )
        {
            $token = trim( $token );
            switch( $token )
            {
                case 'device': 
                    $result["device"] = \TwilioHelper::getDeviceCapabilityToken( $name, true );
                    break;
                case 'worker':
                    $worker_token = \TwilioHelper::getWorkerCapabilityToken( $name );
                    if( $worker_token )
                    {
                        $result["worker"] = $worker_token;
                    }
            }
        }

        echo json_encode( $result );
    }
 
    /**
     * This function allows to set the activity of a Twilio worker (agent)
     * 
     * @url: cu/CTI/setActivity
     * @param: session_id, activity
     * @return: Nothing
     */
    function setActivity()
    {
        $this->validateCTIAccess();

        if( !( $activity = $this->input->post('activity') ) )
        {
            return false;
        }
        else
        {
            $name = $this->getWorkerName();
            \TwilioHelper::setWorkerActivity( $name, $activity );
        }
    }

    /**
     * This function returns the connected available agents using the TwilioTaskRouter
     *
     * @url: /cc/CTI/getConnectedAgents
     * @param: Nothing
     * @return: JSON list of users
     */
    function getConnectedAgents() 
    {
        $this->validateCTIAccess();

        $ids =  \TwilioHelper::findAvailableWorkers();
        if( $ids == false )
        {
            echo json_encode( [] );
            return;
        }
        $accounts_data = self::$connect->getAccountDetails( $ids );

   
        foreach( $accounts_data as $idx=>$value )
        {
            $accounts_data[ $idx ]["worker"] = $this->getWorkerName( $value["id"] );
        }

        echo json_encode( $accounts_data );
    }

    /** 
     * This function search OSvC for a phone number, and return the first contact with this 
     * particular phone number.
     * 
     * @param: session_id, phone number
     * @return: JSON contact data
     */
    function searchPhone()
    {
        $this->validateCTIAccess();
        
        $result = false;
        $phone = $this->input->post("phone");
        // TODO - sanitize phone number
        if( $phone === false )
        {
            header("HTTP/1.0 400 Invalid Request");
        }

        $contact_data = self::$connect->getContactData( $phone, false );

        if( $contact_data === false )
        {
            $result = [
                "success" => false,
                "contact" => [
                    "name" => "Unknown Caller",
                    "firstName" => "Unknown",
                    "lastName" => "Caller",
                    "phone"=> $phone,
                    "email"=> "",
                    "dp"   => "https://www.gravatar.com/avatar/" . md5( 'example@example.com' ) . "?d=mm"
                ]
            ];
        }
        else
        {
            $result = [
                "success" => true,
                "contact" => $contact_data
            ];
        }
        print json_encode( $result );
    }
    /**
     * This function allows to transfer a call to another logged in agent.
     * 
     * @param: session_id, callsid, attributes (JSON string)
     * @return: Nothing 
     */
    function transferCall()
    {
        $this->validateCTIAccess();

        $worker = $this->input->post("worker");
        $attributes = json_decode( $this->input->post("attributes"), true );
        $lookupParentCall = $this->input->post("lookup");

        if( $lookupParentCall && strtolower( $lookupParentCall ) == "true" )
        {
            $lookupParentCall = true;
        }
        else
        {
            $lookupParentCall = false;
        }
        
        if( !array_key_exists( "callSid", $attributes ))
        {
            header("HTTP/1.0 400 Invalid Request");
            echo "<h1>Invalid Request</h1>"; exit();
        }
        
        $callsid = $attributes["callSid"];

        \TwilioHelper::transferCall( $callsid, self::$base . 'reEnqueue', $worker, $attributes, $lookupParentCall );
    }

    /**
     * This function provides an interface for Twilio to re-enqueue the call - to transfer the call to an agent.
     *
     * @param: Twilio Standard post parameters, attributes (as part of the URL - param x)
     * @return: TwiML - for enqueueing the call
     */
    function reEnqueue()
    {
        if( !IS_DEVELOPMENT )
        {
            if( false == \TwilioHelper::isValidTwilioRequest()  )
            {
                header("HTTP/1.0 403 Forbidden");
                echo "<h1>Forbidden</h1>"; exit();
            }
        }

        $param = \RightNow\Utils\Url::getParameter('x');
        $attributes = json_decode( base64_decode_urlsafe( html_entity_decode( $param ) ), true );
        $xml_template = \TwilioHelper::reEnqueCall( $attributes );

        $xml_template = '<?xml version="1.0" encoding="UTF-8"?>
<Response>
' . $xml_template . '  
</Response>
';
        $this->outputXML( $xml_template );  

    }

    /**
     * Interface for Twilio to connect for Incoming and Outgoing calls. 
     * 
     * @param: Standard Twilio post parameters
     * @return: TwiML
     */
    function twilioVoiceHandle()
    {
        if( !IS_DEVELOPMENT )
        {
            if( false == \TwilioHelper::isValidTwilioRequest()  )
            {
                header("HTTP/1.0 403 Forbidden");
                echo "<h1>Forbidden</h1>"; exit();
            }
        }

        $direction = $this->input->post("Direction");
        $from = $this->input->post("From");

        if( substr( $from, 0, 13 ) === "client:agent_" )
        {
            $direction = "outbound";
        }

        if( $direction == "inbound" )
        {
            if( "client:Anonymous" == $_POST["From"])
            {
                $_POST["From"] = "+911234567890";
            }
            
            $this->handleIVR( true );
        }
        else if( $direction == "outbound" ) 
        {
            $this->handleOutgoing();
        }
    }
    
    function logCallAction()
    {
        $this->validateCTIAccess();

        $action = $this->input->post("action");

        if( $action == false )
        {
            header("HTTP/1.0 400 Invalid Request");
            echo "<h1>Invalid Request</h1>"; exit();
        } 

        try
        {
            self::$connect->logCallAction( $this->account->acct_id, $action );
        }
        catch( Exception $e )
        {
            echo json_encode( array(
                "success" => false
            )); return false;
        }
        echo json_encode( array(
            "success" => true
        ));
    }    

    function handleIVR( $internal = false )
    {
        if( !$internal )
        {
            if( !IS_DEVELOPMENT )
            {
                if( false == \TwilioHelper::isValidTwilioRequest()  )
                {
                    header("HTTP/1.0 403 Forbidden");
                    echo "<h1>Forbidden</h1>"; exit();
                }
            }
        }

        $config = \IVRConfig::get("flow");
        $xml_template = "";

        $param = \RightNow\Utils\Url::getParameter('x');

        if( $param === false || $param == '' )
        {
            $param = [
                "s" => 0,
                "d" => []
            ];
            if( array_key_exists( "From", $_POST ) )
            {
                $param["d"]["i"] = $_POST["From"];
            }
        }
        else
        {
            $param = json_decode( base64_decode_urlsafe( html_entity_decode( $param ) ), true );
        }

        //Special param to handle recording and transcribing
        $special = \RightNow\Utils\Url::getParameter('y');
        if( $special != false )
        {
            switch( $special )
            {
                case 'record':
                    $this->handleRecording( $param );
                    break;
                case 'transcribe':
                    $this->handleTranscribe( $param );
                    $xml_template = '<?xml version="1.0" encoding="UTF-8"?>
<Response></Response>
';
                    $this->outputXML( $xml_template );  
                    return;
            }
            // If there is any other cases, just fall through...
        }

        // Process the state
        $state_config = []; 
        if( $param['s'] >= 0 )
            $state_config = $config[ $param['s'] ];

        $next_state = $param['s']; // + 1;

        if( array_key_exists("preProcess", $state_config ) )
        {
            $func = $state_config["preProcess"];
            $result = $func( self::$connect );

            if( $result )
            {
                $param["d"] = array_merge( $param["d"], $result["data"] );
                $state_config = array_merge( $result["config"], $state_config );
                $config[$next_state] = $state_config;
                \IVRConfig::$config["flow"][$next_state] = $state_config;
            }
        }

        if( array_key_exists( "gather", $state_config ) && array_key_exists( "next", $state_config["gather"] ) )
        {
            $next = $state_config["gather"][ "next" ];
            if( is_array( $next ) )
            {
                if( array_key_exists( "Digits", $_POST ) && array_key_exists( $_POST["Digits"], $next ) )
                {
                    $next_state = $next[ $_POST["Digits"] ];
                }
            }
            else
            {
                $next_state = $next;
            }
        }

        if( array_key_exists( "record", $state_config ) && array_key_exists( "next", $state_config["record"] ) )
        {
            $next_state = $state_config["record"][ "next" ];
        }

        if( array_key_exists("process", $state_config ) )
        {
            $func = $state_config["process"];
            $result = $func( $param, $_POST );
            if ( $result === false ) 
            {
                unset( $_POST["Digits"] );
                $next_state = $param["s"];
                $voice = \CTIConfig::get("voice", "woman");
                if( array_key_exists( "errorDialogs", $state_config ) )
                {
                    $xml_template .= \TwilioHelper::wrapDialogs( $state_config["errorDialogs"], $voice) ;
                }
            }

            if( $result )
            {
                $param["d"] = array_merge( $param["d"], $result );
            }
        }

        $action = self::$base . "handleIVR/x/";
        $xml_template .= \TwilioHelper::processIVR( $next_state, $action, $param["d"] );
        $xml_template = '<?xml version="1.0" encoding="UTF-8"?>
<Response>
' . $xml_template . '  
</Response>
';
        $this->outputXML( $xml_template );        
    }

    private function handleRecording( $param )
    {
        $recordingURL = $this->input->post("RecordingUrl");
        $duration = $this->input->post("RecordingDuration");
        $status = $this->input->post("RecordingStatus");

        if( $recordingURL === false )
        {
            return false; 
        }

        $voicemailset = isset($_COOKIE["voicemail"]);
        
        if( $voicemailset == false )
        {
            $phone = $param["d"]["i"];

            $contact_data = self::$connect->getContactData( $phone );
            $id = self::$connect->attachVoicemail( $recordingURL, $duration, $contact_data["id"] );
            setcookie( "voicemail", "$id" );
        }
    }

    private function handleTranscribe( $param )
    {
        $status = $this->input->post("TranscriptionStatus");
        $text = $this->input->post("TranscriptionText");

        if( $status == "failed" )
        {
            $text = false;
        }
        else if( $status != "completed" )
        {
            return;
        }
        
        $phone = $param["d"]["i"];
        self::$connect->attachTranscription( $text, $phone );
    }

    private function handleOutgoing()
    {
        $number = $this->input->post("To");
        $voice = \CTIConfig::get("voice", "woman");

        if( $number === false || $number == '' )
        {
            $this->outputXML( '<?xml version="1.0" encoding="UTF-8"?><Response><Say voice="woman">Unable to connect the call. Please check the number you have dialed!</Say></Response>' );
            return false;
        }

        $xml_template = '<?xml version="1.0" encoding="UTF-8"?>
<Response>
  <Say voice="' . $voice . '">Please wait while we connect the call.</Say>
  <Dial callerId="' . twilio_phone_number . '">' . $number . '</Dial>   
</Response>
';

        $this->outputXML( $xml_template );
        return true;
    }

    function twilioMessageHandle()
    {
        if( !IS_DEVELOPMENT )
        {
            if( false == \TwilioHelper::isValidTwilioRequest()  )
            {
                header("HTTP/1.0 403 Forbidden");
                echo "<h1>Forbidden</h1>"; exit();
            }
        }

        $from = $this->input->post("From");
        $message = $this->input->post("Body");

        self::$connect->logInboundMessage( $from, $message );

        $xml_template = '<?xml version="1.0" encoding="UTF-8"?>
<Response>
  <Message>Thank you for your SMS. We will get back to you shortly!</Message>   
</Response>
';

        $this->outputXML( $xml_template );
        return true;

    }

    function twilioProxy()
    {
        $this->validateCTIAccess();
        $url = $this->input->post("uri");

        if( $url === false )
        {
            header("HTTP/1.0 400 Invalid Request");
            echo "<h1>Invalid Request</h1>"; exit();
        }

        $substitutions = [
            "{ACCOUNT_SID}" => twilio_account_sid,
            "{WORKSPACE_SID}" => \CTIConfig::get("workspace")["sid"]
        ];

        foreach( $substitutions as $key => $value )
        {
            $url = str_replace( $key, $value, $url );
        }
        echo \TwilioHelper::proxy( $url );
    }

    function sendSMS()
    {
        $this->validateSMSAccess();

        // TODO: Number validation        
        $number = $this->input->post("number");
        $message = $this->input->post("message");
        $incident = $this->input->post("incident");
        
        \TwilioHelper::sendSMS( $number, $message );
        self::$connect->logOutboundMessage( $number, $message, $incident );

        echo json_encode(["status"=>"Success"]);
    }

    function waitMusic()
    {
        // Timeout in seconds
        $timeout = intval(\CTIConfig::get("chat_offer_threshold", "20"));

        $qTime = $this->input->post("QueueTime");
        $voice = \CTIConfig::get("voice", "woman");

        if( $qTime >= $timeout )
        {
            // Store the Chat offer status
            $callSid = \RightNow\Utils\Url::getParameter('csid');
            $chat_url = self::$base . 'offerChat';

            if( $callSid != false )
            {
                $id = self::$connect->setChatOfferState( $callSid );
                $chat_url .= "/t/" . $id;
            }

            $message = \TwilioHelper::getMessage(CUSTOM_MSG_CTI_Timeout_Escalate_Dialog);
            $xml_template = '<?xml version="1.0" encoding="UTF-8"?>
<Response>
  <Say voice="' . $voice . '">' . $message . '</Say>
  <Redirect method="GET">http://com.twilio.sounds.music.s3.amazonaws.com/index.xml</Redirect>
</Response>
';
            $number = $this->input->post("From");
            $message = "Our voice agents are busy at this moment heping others. Please click the following URL to initiate a Chat with a chat agent. We are happy to help you!\r\n " . $chat_url;
            \TwilioHelper::sendSMS( $number, $message );
            $this->outputXML( $xml_template );
            return;

        }
        else
        {
            $chat_offer_threshold = \CTIConfig::get("chat_offer_threshold", "20");
            $xml_template = '<?xml version="1.0" encoding="UTF-8"?>
<Response>
  <Play>' . self::$base . '/waitMedia/t/' . $chat_offer_threshold . '</Play>   
</Response>
';
            $this->outputXML( $xml_template );
            return;
        }
    }

    function offerChat()
    {
        $id = \RightNow\Utils\Url::getParameter('t');
        $contact = NULL;

        if( $id != false )
        {
            $log = self::$connect->findChatOfferState( $id );
            if( $log != NULL )
            {
                $callSid = $log->CallSid;
                $queueSid = $log->QueueSid;
                
                if( $log->Phone != NULL )
                {
                    $contact = self::$connect->getContactData( $log->Phone, false );
                }
                try
                {
                    $log->destroy();
                }
                catch( \Exception $e )
                {}
                \TwilioHelper::dropCall( $queueSid, $callSid, self::$base );
            }
        }

        $chat_url = "https://" . $_SERVER['HTTP_HOST'] . '/app/chat/chat_landing/';
        if( $contact != NULL )
        {
            $chat_url .= "email/" . $contact['email'] . "/first_name/" . $contact["firstName"] . "/last_name/" . $contact["lastName"] . "/c_id/" . $contact["id"];
        }
        header("Location: " . $chat_url );
    }

    private function outputXML( $xml )
    {
        header("Content-type: text/xml;charset=UTF-8");
        print $xml;
    }

    function ringMedia()
    {
        header("Content-Transfer-Encoding: binary"); 
        header("Content-Type: audio/mpeg, audio/x-mpeg, audio/x-mpeg-3, audio/mpeg3");
        header('Content-Disposition: inline; filename="ring.mp3"');
        readfile( __DIR__ . "/../helpers/ring.mp3");
    }

    function waitMedia()
    {
        $time = \RightNow\Utils\Url::getParameter('t');

        if( $time == false )
        {
            $time = "30";
        }

        if( $time != "30" || $time != "60" )
        {
            $time = "30";
        }

        $file = "wait_music_" . $time . ".mp3";

        header("Content-Transfer-Encoding: binary"); 
        header("Content-Type: audio/mpeg");
        header('Content-Disposition: inline; filename="' . $file . '"');
        readfile( __DIR__ . "/../helpers/" . $file );
    }
    
    function dropCall()
    {
        $message = \TwilioHelper::getMessage(CUSTOM_MSG_CTI_DropCall);
        $voice = \CTIConfig::get("voice", "woman");
        $xml_template = '<?xml version="1.0" encoding="UTF-8"?>
<Response>
  <Say voice="' . $voice . '">' . $message . '</Say>
  <Leave />
</Response>
';
        $this->outputXML( $xml_template );
    }
}