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
 *  date: Monday Oct 30 13:4:53 UTC 2017
 *  revision: rnw-17-11-fixes-releases
 * 
 *  SHA1: $Id: 5bf85e120af6da408fc1d74092d02e7ee90be659 $
 * *********************************************************************************************
 *  File: twilio_rest_helper.php
 * ****************************************************************************************** */

use RightNow\Connect\Crypto\v1_3 as Crypto;
use RightNow\Connect\v1_3 as Connect;
use RightNow\Utils\Config;

require_once __DIR__ . "/../libraries/CTI/CTIConfig.php";

class TwilioHelper
{
    static function transferCall( $callsid, $base, $worker, $attributes, $lookupParent = false )
    {

        // Lookup the parent call (for transferring an outbound call)
        if( $lookupParent == true )
        {
            $url = $url = "https://api.twilio.com/2010-04-01/Accounts/" . twilio_account_sid . "/Calls.json?ParentCallSid=" . $callsid;
            $result = self::httpGet( $url );
            if( $result )
            {
                $result = json_decode( $result, true );
                if( array_key_exists("calls", $result) && count( $result["calls"]) > 0 )
                {
                    $callsid = $result["calls"][0]["sid"];
                    $attributes["callSid"] = $callsid;
                }
            }
        }

        $url = "https://api.twilio.com/2010-04-01/Accounts/" . twilio_account_sid . "/Calls/" . $callsid;

        $params = [];

        $attribs = ["contact","incident","data", "callSid" ];
        foreach( $attribs as $idx => $key )
        {
            if( array_key_exists( $key, $attributes ) )
            {
                $params[$key] = $attributes[$key];
            }
        }
        $params["w"] = $worker;
        $uri = $base . "/x/" . base64_encode_urlsafe( json_encode( $params ) );

        self::httpPost( $url, [
            "Url"   => $uri,
            "Method"=> "POST"
        ]);
        
    }

    static function dropCall( $queueSid, $callSid, $base )
    {
        $url = "https://api.twilio.com/2010-04-01/Accounts/" . twilio_account_sid . "/Queues/" . $queueSid . "/Members/" . $callSid . ".json";
        //$url = "https://api.twilio.com/2010-04-01/Accounts/" . twilio_account_sid . "/Calls/" . $callSid;
        $uri = $base . "/dropCall";
        $response = self::httpPost( $url, [
            "Url"   => $uri,
            "Method"=> "POST"
        ]);
    }

    static function reEnqueCall( $attributes )
    {
        $worker = $attributes["w"];
        unset( $attributes["w"]);
        $attributes["agent_name"] = $worker;

        $xml_template = '<Enqueue workflowSid="' . \CTIConfig::get("workflow")["sid"] . '"><Task>' . json_encode( $attributes ) . '</Task></Enqueue>';

        return $xml_template;
    }

    static function findWorker( $name )
    {
        $workers = self::getWorkers( [
            "FriendlyName" => $name 
        ]);

        if( is_array( $workers ) && count( $workers > 0 ))
            return $workers[0];
        else
            return false;
    }

    static function findAvailableWorkers( )
    {
        $workers = self::getWorkers( [
            "Available" => 1 
        ]);

        $result = array();
        if( $workers == false )
        {
            return false;
        }
        foreach( $workers as $idx => $worker )
        {
            $name = $worker["friendly_name"];
            $name = str_replace( "agent_", '', $name );
            $result[] = $name;
        }

        return $result;
    }

    static function addWorker( $name, $activity = "Offline" )
    {
        $workspace = \CTIConfig::get("workspace")["sid"];

        $worker = self::findWorker( $name );

        if( $worker === false )
        {
            $activitySid = \CTIConfig::get("activities")[$activity];

            self::httpPost( "https://taskrouter.twilio.com/v1/Workspaces/$workspace/Workers", [
                "FriendlyName" => $name,
                "ActivitySid"  => $activitySid,
                "Attributes"   => json_encode(array("contact_uri" => "client:" . $name, "agent_name" => $name ))
            ] );
        }

    }

    static function setWorkerActivity( $name, $activity )
    {
        $workspace = \CTIConfig::get("workspace")["sid"];
        
        $activities = \CTIConfig::get("activities");
        if( !array_key_exists( $activity, $activities ))
        {
            return false;
        }

        $activitySid = $activities[$activity];

        $worker = self::findWorker( $name );

        if( $worker === false )
        {
            return false;
        }

        self::httpPost( "https://taskrouter.twilio.com/v1/Workspaces/$workspace/Workers/" . $worker["sid"], [
                "ActivitySid"  => $activitySid
            ] );

        return true;
    }

    static function getWorkerCapabilityToken( $name, $exp = 3600 )
    {
        $workspace = \CTIConfig::get("workspace")["sid"];
        $worker = TwilioHelper::findWorker( $name );

        if( $worker === false )
        {
            return false;
        }
        $wsid = $worker["sid"];

        $headers = [ "type" => "JWT", "alg" => "HS256" ];

        $payload_json = <<<EOF
{
	"version": "v1",
	"friendly_name": "{WORKER_SID}",
	"iss": "{ACCOUNT_SID}",
	"exp": {EXPIRATION},
	"account_sid": "{ACCOUNT_SID}",
	"channel": "{WORKER_SID}",
	"workspace_sid": "{WORKSPACE_SID}",
	"worker_sid": "{WORKER_SID}",
	"policies": [{
		"url": "https:\/\/event-bridge.twilio.com\/v1\/wschannels\/{ACCOUNT_SID}\/{WORKER_SID}",
		"method": "GET",
		"allow": true
	},
	{
		"url": "https:\/\/event-bridge.twilio.com\/v1\/wschannels\/{ACCOUNT_SID}\/{WORKER_SID}",
		"method": "POST",
		"allow": true
	},
	{
		"url": "https:\/\/taskrouter.twilio.com\/v1\/Workspaces\/{WORKSPACE_SID}\/Workers\/{WORKER_SID}",
		"method": "GET",
		"allow": true
	},
	{
		"url": "https:\/\/taskrouter.twilio.com\/v1\/Workspaces\/{WORKSPACE_SID}\/Activities",
		"method": "GET",
		"allow": true
	},
	{
		"url": "https:\/\/taskrouter.twilio.com\/v1\/Workspaces\/{WORKSPACE_SID}\/Tasks\/**",
		"method": "GET",
		"allow": true
	},
	{
		"url": "https:\/\/taskrouter.twilio.com\/v1\/Workspaces\/{WORKSPACE_SID}\/Workers\/{WORKER_SID}\/Reservations\/**",
		"method": "GET",
		"allow": true
	},
	{
		"url": "https:\/\/taskrouter.twilio.com\/v1\/Workspaces\/{WORKSPACE_SID}\/Workers\/{WORKER_SID}",
		"method": "POST",
		"allow": true,
		"query_filter": {
			
		},
		"post_filter": {
			"ActivitySid": {
				"required": true
			}
		}
	},
	{
		"url": "https:\/\/taskrouter.twilio.com\/v1\/Workspaces\/{WORKSPACE_SID}\/Tasks\/**",
		"method": "POST",
		"allow": true,
		"query_filter": {
			
		},
		"post_filter": {
			
		}
	},
	{
		"url": "https:\/\/taskrouter.twilio.com\/v1\/Workspaces\/{WORKSPACE_SID}\/Workers\/{WORKER_SID}\/Reservations\/**",
		"method": "POST",
		"allow": true,
		"query_filter": {
			
		},
		"post_filter": {
			
		}
	}]
}
EOF;

        $payload = str_replace( "{WORKER_SID}", $wsid, $payload_json );
        $payload = str_replace( "{ACCOUNT_SID}", twilio_account_sid, $payload );
        $payload = str_replace( "{WORKSPACE_SID}", $workspace, $payload );
        $payload = str_replace( "{EXPIRATION}", "" . time() + $exp, $payload );
        

        $headers_encoded = base64_encode_urlsafe( json_encode( $headers ) );
        $payload_encoded = base64_encode_urlsafe( $payload );

        $jws_payload = $headers_encoded . "." . $payload_encoded;
        $jws = hash_hmac( "sha256", $jws_payload, twilio_auth_token, true );
        $jws_encoded = base64_encode_urlsafe( $jws );

        $token = $jws_payload . "." . $jws_encoded;
        return $token;
        
    }

    static function getICEServers()
    {
        $url = "https://api.twilio.com/2010-04-01/Accounts/" . twilio_account_sid . "/Tokens.json";
        
        $tokens = self::httpPost( $url, array() );
        $tokens = json_decode( $tokens, true );

        $tokens = $tokens["ice_servers"];
        return $tokens;
    }
    
    static function getDeviceCapabilityToken( $name, $outgoing = false , $expire = 3600 )
    {
        $headers = [ "type" => "JWT", "alg" => "HS256" ];
        $scope = "scope:client:incoming?clientName=$name";

        if( $outgoing == true )
        {
            $scope .= ' scope:client:outgoing?appSid=' . twilio_app_sid . "&appParams=&clientName=$name";
        }

        $payload = [
            "scope" => $scope,
            "iss" => twilio_account_sid,
            "exp" => time() + $expire
        ];

        $headers_encoded = base64_encode_urlsafe( json_encode( $headers ) );
        $payload_encoded = base64_encode_urlsafe( json_encode( $payload ) );

        $jws_payload = $headers_encoded . '.' . $payload_encoded;
        $jws = hash_hmac( "sha256", $jws_payload, twilio_auth_token, true );
        $jws_encoded = base64_encode_urlsafe( $jws );

        $token = $jws_payload . '.' . $jws_encoded;
        return $token;

    }

    static function getWorkerStatistics( $worker )
    {
        $wsid = self::findWorker( $worker )['sid'];


    }
   
    static function sendSMS( $number, $message )
    {
        $url = "https://api.twilio.com/2010-04-01/Accounts/" . twilio_account_sid . "/Messages.json";

        self::httpPost( $url, [
            "To"    => $number,
            "Body"  => $message,
            "From"  => twilio_phone_number
        ]);

        // TODO - error handling
    }

    static function isValidTwilioRequest()
    {
        if( !isset( $_SERVER["HTTP_X_TWILIO_SIGNATURE"] ) )
        {
            return false;
        }

        $signature = $_SERVER["HTTP_X_TWILIO_SIGNATURE"];

        $url = $_SERVER["SCRIPT_URI"];
        if( $_SERVER["RNT_SSL"] == "yes" )
            $url = str_replace("http:","https:",$url);

        ksort( $_POST );

        foreach( $_POST as $key => $value )
        {
            $url .= $key . $value;
        }

        $hash = base64_encode( hash_hmac( "sha1", $url, twilio_auth_token, true ) );

        if( $signature === $hash )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static function processIVR( $state, $base = NULL, $data = [] )
    {
        $flow = \IVRConfig::get("flow");
        $voice = \IVRConfig::get("voice");

        if( !array_key_exists( $state, $flow ) )
        {
            throw new Exception("State does not exist in the flow configuration");
        }

        $xml_template = "";
        $action = "";

        foreach( $flow[$state] as $key => $value )
        {
            switch( $key )
            {
                case 'dialogs': 
                    $xml_template .= self::wrapDialogs( $value, $voice );
                    break;
                case 'enqueue':
                    $base = str_replace( '/handleIVR/x/', '', $base );
                    $xml_template .= self::processEnqueue( $value, $data, $voice, $base );
                    break;
                case 'gather':
                    $action = self::makeActionURL( $base, $state, $data );
                    $xml_template .= self::processGather( $value, $action, $voice );
                    break;
                case 'redirect':
                    $action = self::makeActionURL( $base, $value , $data );
                    $xml_template .= self::processRedirect( $action );
                    break;
                case 'record':
                    $action = self::makeActionURL( $base, $state , $data );
                    $xml_template .= self::processRecord( $value, $action, $state, $data );
            }
        }
        return $xml_template;
    }
    static function wrapDialogs( $dialogs, $voice = "alice" )
    {
        $xml_template = "";
        foreach( $dialogs as $idx => $dialog )
        {
            $text = self::getMessage( $dialog );
            if( $text != false ) 
            {
                $dialog = $text;
            }

            $xml_template .= '<Say voice="' . $voice . '">' . $dialog . '</Say>';
        }

        return $xml_template;
    }

    static function getMessage( $key )
    {
        $text = "";
        try
        {
            $message = Connect\MessageBase::fetch( $key );
            $text = $message->Value;
        }
        catch( \Exception $e )
        {
            return false;
        }
        return $text;
    }

    private static function processGather( $gather, $action, $voice = "alice" )
    {
        $xml_template = "<Gather";

        if( array_key_exists("digits", $gather))
        {
            $xml_template .= ' numDigits="' . $gather["digits"] . '"';
        }
        
        if( array_key_exists("timeout", $gather))
        {
            $xml_template .= ' timeout="' . $gather["timeout"] . '"';
        }

        $xml_template .= ' action="' . $action . '" method="POST"';

        $xml_template .= '>';

        if( array_key_exists("dialogs", $gather) )
        {
            $xml_template .= self::wrapDialogs( $gather["dialogs"], $voice);
        }

        $xml_template .= "\n</Gather>\n";

        if( array_key_exists( "repeat", $gather ))
        {
            $xml_template .= self::wrapDialogs( [$gather["repeat"]], $voice);
        }

        return $xml_template;
    }

    private static function processEnqueue( $enqueue, $attributes, $voice, $base )
    {
        $CI = get_instance();

        $CI->load->model('custom/CTIConnect');
        $contact_data = $CI->CTIConnect->getContactData( $attributes["i"] );
        
        $attributes["contact"] = $contact_data;
        if( isset( $_POST["CallSid"] ) )
        {
            $attributes["callSid"] = $_POST["CallSid"];
        }

        $wait_string = "";
        $wait_music_disabled = \CTIConfig::get("CTI_CHAT_DISABLED", "true");
        if( !$wait_music_disabled && \RightNow\Utils\Chat::isChatAvailable() == true && Config::getConfig(MOD_CHAT_ENABLED) == true )
        {
            $wait_url = $base . "/waitMusic";
            if( isset( $_POST["CallSid"] ) )
            {
                $wait_url .= "/csid/" . $_POST["CallSid"];
            }
            $wait_string = ' waitUrl="' . $wait_url . '"';
        }

        $xml_template = self::wrapDialogs( $enqueue["dialogs"], $voice );
        $xml_template .= '<Enqueue workflowSid="' . \CTIConfig::get("workflow")["sid"] . '"' . $wait_string . '><Task>' . json_encode( $attributes ) . '</Task></Enqueue>';

        return $xml_template;       

    }

    private static function processRecord( $record, $base, $state, $data )
    {
        $action = $base . "/y/record";
        $xml_template = '<Record trim="trim-silence" action="' . $action . '" ';
        if( array_key_exists( "beep", $record ) && $record["beep"] == true )
        {
            $xml_template .= 'playBeep="true" ';
        }

        if( array_key_exists( "transcribe", $record ) && $record["transcribe"] == true )
        {
            $t_callback = $base . "/y/transcribe";
            $xml_template .= 'transcribe="true" transcribeCallback="' . $t_callback . '" ';
        }

        if( array_key_exists( "timeout", $record ) )
        {
            $xml_template .= 'maxLength="' . $record["timeout"] . '" ';
        }

        $xml_template .= '></Record>';

        return $xml_template;
    }

    private static function makeActionURL( $base, $state, $data )
    {
        $segment = [
            "s" => $state,
            "d" => $data
        ];

        $part = base64_encode_urlsafe( json_encode( $segment ) );

        return $base . $part;
    }

    private static function processRedirect( $action )
    {
        $xml_template = '<Redirect>' . $action . '</Redirect>';
        return $xml_template;
    }

    public static function getWorkers( $data )
    {
        $workspace = \CTIConfig::get("workspace");
        $url = "https://taskrouter.twilio.com/v1/Workspaces/" . $workspace["sid"] . "/Workers";

        $url .= "?" . http_build_query( $data );

        $results = self::httpGet( $url, $data );
        if( $results != false )
        {
            $results = json_decode( $results, true );
            $results = $results["workers"];
        }

        if( $results == NULL )
            $results = false;
        
        return $results;

    }

    static function proxy( $url )
    {
        $response = self::httpGet( $url );
        return $response;
    }

    private static function httpGet( $url, $headers = array() )
    {
        $curl = curl_init();
        // Set some options - we are passing in a useragent too here
        curl_setopt_array($curl, array(
            CURLOPT_RETURNTRANSFER => 1,
            CURLOPT_URL => $url,
            CURLOPT_SSL_VERIFYHOST => 0,
            CURLOPT_SSL_VERIFYPEER => 0,
            CURLOPT_HTTPHEADER => $headers,
            CURLOPT_USERPWD => twilio_account_sid . ":" . twilio_auth_token
        ));
        // Send the request & save response to $resp
        $resp = curl_exec($curl);
        if(!curl_exec($curl)){
            return false;
        }
        // Close request to clear up some resources
        curl_close($curl);
        return $resp;
    }

    private static function httpPost( $url,$data, $headers = array() )
    {
        // Get cURL resource
        $curl = curl_init();
        // Set some options - we are passing in a useragent too here
        curl_setopt_array($curl, array(
            CURLOPT_RETURNTRANSFER => 1,
            CURLOPT_URL => $url,
            CURLOPT_POST => 1,
            CURLOPT_SSL_VERIFYHOST => 0,
            CURLOPT_SSL_VERIFYPEER => 0,
            CURLOPT_POSTFIELDS => $data,
            CURLOPT_HTTPHEADER => $headers,
            CURLOPT_USERPWD => twilio_account_sid . ":" . twilio_auth_token
        ));
        // Send the request & save response to $resp
        $resp = curl_exec($curl);
        
        // Close request to clear up some resources
        curl_close($curl);

        return $resp;
    }

    public static function urlsafeB64Encode($string) {
        return str_replace('=', '', strtr(base64_encode($string), '+/', '-_'));
    }
}

// Other helper functions required

if( !function_exists( "sha256" ) )
{
    function sha256( $txt )
    {
        return do_hash( Crypto\MessageDigest::ALGORITHM_SHA256, $txt );
    }
}

if( !function_exists( "md5" ) )
{
    function md5( $txt )
    {
        return do_hash( Crypto\MessageDigest::ALGORITHM_MD5, $txt );
    }
}

function do_hash( $mode,  $txt )
{

    $cipher = new Crypto\MessageDigest();
    $cipher->Algorithm->ID = $mode;
    $cipher->Text = $txt;
    $cipher->Encoding->ID = 1;

    $cipher->hash();
    return  bin2hex($cipher->HashText);

}

function base64_encode_urlsafe($string) 
{
    return str_replace('=', '', strtr(base64_encode($string), '+/', '-_'));
}

function base64_decode_urlsafe($string) 
{
    $padlen = 4 - strlen($string) % 4;
    $string .= str_repeat('=', $padlen);
    return base64_decode(strtr($string, '-_', '+/'));
}

/* 
Defining the HMAC hash method as 'hash' functions are not available in OSvC standard PhP. 
In OSvC, hash functions are exposed through MessageDigest (v1.3+) class
*/

if( !function_exists( "hash_hmac" ) )
{
    function hash_hmac($algo, $data, $key, $raw_output = false)
    {
        $algo = strtolower($algo);
        $pack = "H".strlen($algo("test"));
        $size = 64;
        $opad = str_repeat(chr(0x5C), $size);
        $ipad = str_repeat(chr(0x36), $size);

        if (strlen($key) > $size) {
            $key = str_pad(pack($pack, $algo($key)), $size, chr(0x00));
        } else {
            $key = str_pad($key, $size, chr(0x00));
        }

        for ($i = 0; $i < strlen($key) - 1; $i++) {
            $opad[$i] = $opad[$i] ^ $key[$i];
            $ipad[$i] = $ipad[$i] ^ $key[$i];
        }

        $output = $algo($opad.pack($pack, $algo($ipad.$data)));

        return ($raw_output) ? pack($pack, $output) : $output;
    }
}
