<?
/*
 * CPMObjectEventHandler: apns_update_incident
 * Package: RN
 * Objects: Incident
 * Actions: Update
 * Version: 1.3
 */

/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published 
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 
 *  included in the original distribution. 
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved. 
  ***********************************************************************************************
 *  Accelerator Package: OSVC Mobile Application Accelerator 
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html 
 *  OSvC release: 16.11 (November 2016) 
 *  date: Mon Dec 12 02:05:30 PDT 2016 
 *  revision: rnw-16-11

 *  SHA1: $Id$
 * *********************************************************************************************
 *  File: This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.

 * *********************************************************************************************/

// This object procedure binds to v1_3 of the Connect PHP API
use\RightNow\Connect\v1_3 as RNCPHP;


// This object procedure binds to the v1 interface of the process
// designer
use \RightNow\CPM\v1 as RNCPM;

/**
 * An Object Event Handler must provide two classes:
 * - One with the same name as the CPMObjectEventHandler tag
 * above that implements the ObjectEventHandler interface.
 * - And one of the same name with a "_TestHarness" suffix
 * that implements the ObjectEventHandler_TestHarness interface.
 *
 * Each method must have an implementation.
 */

//define("MCS_SERVER_URL", "");
//define("ORACLE_MOBILE_BACKEND_ID", "");
//define("USERNAME", "");
//define("PASSWORD", "");

class apns_update_incident implements RNCPM\ObjectEventHandler
{

    static $MCS_SERVER_URL = "";
    static $ORACLE_MOBILE_BACKEND_ID = "";
    static $USERNAME = "";
    static $PASSWORD = "";

    public static function apply($run_mode, $action, $obj, $n_cycles)
    {


        $assignee_id = $obj->AssignedTo->Account->ID;
        $acct_id     = $obj->CreatedByAccount->ID;
        $id         = $obj->ID;
        $sub         = $obj->Subject;
        $ref         = $obj->ReferenceNumber;
        $acct         = RNCPHP\Account::fetch($assignee_id);
        $fetchedToken = $acct->CustomFields->Mobile->apns_token;

        if (!empty($fetchedToken)) {
            print 'start pushing notification ...';

            self::ParseConfiguration();
            print 'MCS_SERVER_URL: ' . static::$MCS_SERVER_URL;
            print 'ORACLE_MOBILE_BACKEND_ID: ' . static::$ORACLE_MOBILE_BACKEND_ID;
            print 'USERNAME: ' . static::$USERNAME;
            print 'PASSWORD: ' . static::$PASSWORD;
            
            if(empty(static::$MCS_SERVER_URL) 
                || empty(static::$ORACLE_MOBILE_BACKEND_ID) 
                || empty(static::$USERNAME) 
                || empty(static::$PASSWORD))
                return;

            $registation_ids = array(
                $fetchedToken
                );

            // Set POST variables
            $url = static::$MCS_SERVER_URL.'/mobile/system/notifications/notifications';
            $data = array(
               'alert'=> "Incident " . $ref . " is updated.", 
               'feature'=> "Incidents" , 'id'=> $id           
               );

            $fields = array(
                'notificationTokens' => $registation_ids,
                'payload' => array(
                  'services' => array(
                     'apns' => array(
                        'aps'=>$data
                        )
                     )
                  )
                );

            $headers = array(
              'Accept: application/json',
              'Content-Type: application/json; charset=UTF-8',
              'Oracle-Mobile-Backend-ID:' . static::$ORACLE_MOBILE_BACKEND_ID
              );


            load_curl();
            $request = curl_init();

            // Set the url, number of POST vars, POST data
            curl_setopt($request, CURLOPT_URL, $url);
            curl_setopt($request, CURLOPT_POST, true);
            curl_setopt($request, CURLOPT_HTTPHEADER, $headers);
            curl_setopt($request, CURLOPT_RETURNTRANSFER, true);
            curl_setopt($request, CURLOPT_USERPWD, static::$USERNAME. ":" . static::$PASSWORD);

            // Disabling SSL Certificate support temporarly
            curl_setopt($request, CURLOPT_SSL_VERIFYPEER, false);

            curl_setopt($request, CURLOPT_POSTFIELDS, json_encode($fields));

            // Execute post
            $result = curl_exec($request);
            print($reuslt);

            if ($result === FALSE)
            {
                die('Curl failed: ' . curl_error($request));
            }

            // Close connection
            curl_close($request);
            echo $result;

        } else {
            print "NO TOKEN FOUND";
        }
        return;
    }

    public static function ParseConfiguration()
    {
        $cfg = RNCPHP\Configuration::fetch(CUSTOM_CFG_Accel_Mobile);
        $cfgVal = $cfg->Value;
        $cfgArray = json_decode($cfgVal, true);

        $curentHost = $_SERVER["HTTP_HOST"];

        foreach($cfgArray["hosts"] as $cfgHost){
            $host = parse_url($cfgHost["rnt_host"], PHP_URL_HOST);
            if($host === $curentHost){
                $cfgPushNotification = $cfgHost["push_notification"];
            }
        }

        static::$MCS_SERVER_URL = $cfgPushNotification["mcs"]["basedURL"];
        static::$ORACLE_MOBILE_BACKEND_ID = $cfgPushNotification["mcs"]["mobileBackendId"];
        static::$USERNAME = $cfgPushNotification["mcs"]["username"];
        static::$PASSWORD = $cfgPushNotification["mcs"]["password"];

    }

    // apply()

}
// class apns_update_incident


/*
The Test Harness
*/
class apns_update_incident_TestHarness implements RNCPM\ObjectEventHandler_TestHarness
{

    static $inc_invented = NULL;
    public static function setup()
    {

        // For this test, fetch incident as expected
        $inc = RNCPHP\Incident::fetch(1);
        $subject      = $inc->Subject;
        $inc->Subject = $subject . "xxx";

        static::$inc_invented = $inc;
        return;
    }

    public static function fetchObject($action, $object_type)
    {
        if (static::$inc_invented)
            return (static::$inc_invented);

    }

    public static function validate($action, $object)
    {
        $pass = false;
        assert($object == "Incident");

        $pass = true;
        echo "Incident test passed.\r\n";

        return ($pass);
    }

    public static function cleanup()
    {
        // Destroy every object invented by this test.
        // Not necessary since in test mode and nothing is committed,
        // but good practice if only to document the side effects of
        // this test.
        //        static::$inc_invented->destroy().
        //        static::$inc_invented = NULL;
        return;
    }

}

