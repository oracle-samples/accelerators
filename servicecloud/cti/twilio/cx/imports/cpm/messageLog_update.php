<?php
/*
 * CPMObjectEventHandler: MessageLog_Update
 * Package: Accelerator
 * Objects: Accelerator$MessageLog
 * Actions: Update
 * Version: 1.3
 */

/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set 
 *  published by Oracle under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: Telephony and SMS Accelerator for Twilio
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17D (November 2017)
 *  reference: 161212-000059
 *  date: Monday Oct 30 13:4:52 UTC 2017
 *  revision: rnw-17-11-fixes-releases
 * 
 *  SHA1: $Id: f02c31dd98b6538626a235877a18e3016072a9d2 $
 * *********************************************************************************************
 *  File: messageLog_update.php
 * ****************************************************************************************** */


// This object procedure binds to v1_3 of the Connect PHP API
use \RightNow\Connect\v1_3 as RNCPHP;
$ip_dbreq = true;


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

class MessageLog_Update implements RNCPM\ObjectEventHandler
{
    
    public static function apply($run_mode, $action, $obj, $n_cycles)
    {
        require_once(get_cfg_var("doc_root") . "/ConnectPHP/Connect_init.php");
        if ($obj->Incident->ID != null && $obj->Phone != null && $obj->Direction != null) {
            $Incident = RNCPHP\Incident::fetch($obj->Incident->ID);
            $f_count  = count($Incident->Threads);
            if ($f_count == 0) {
                $Incident->Threads = new RNCPHP\ThreadArray();
            }
            $Incident->Threads[$f_count]                = new RNCPHP\Thread();
            $Incident->Threads[$f_count]->EntryType     = new RNCPHP\NamedIDOptList();
            $Incident->Threads[$f_count]->EntryType->ID = 3; // Used the ID here. See the Thread object for definition
            $messageFlow                                = "Incoming SMS from";
            if ($obj->Direction == "outbound") {
                $messageFlow = "Outgoing SMS to";
            }
            $Incident->Threads[$f_count]->Text = $messageFlow . " " . $obj->Phone . " : " . $obj->Message;
            $Incident->save(RNCPHP\RNObject::SuppressAll);
        }
    } // apply()
    
}


/*
The Test Harness
*/


class MessageLog_Update_TestHarness implements RNCPM\ObjectEventHandler_TestHarness
{
    
    static $tmpMessageLog = NULL;
    static $tmpIncident = NULL;
    static $oldThreadsCount = 0;
    public static function setup()
    {
        static::$tmpMessageLog = new RNCPHP\Accelerator\MessageLog();
        return;
    }
    
    public static function fetchObject($action, $object_type)
    {
        static::$tmpIncident              = RNCPHP\Incident::first("ID > 0");
        static::$oldThreadsCount          = count(static::$tmpIncident->Threads);
        static::$tmpMessageLog->Incident  = static::$tmpIncident;
        static::$tmpMessageLog->Message   = "Sample Message for CPM Test";
        static::$tmpMessageLog->Phone     = "123456789";
        static::$tmpMessageLog->Direction = "outbound";
        return static::$tmpMessageLog;
    }
    
    public static function validate($action, $object)
    {
        static::$tmpMessageLog->save();
        return (static::$oldThreadsCount + 1 == count(static::$tmpIncident->Threads));
        
    }
    
    public static function cleanup()
    {
        // Destroy every object invented
        // by this test.
        // Not necessary since in test
        // mode and nothing is committed,
        // but good practice if only to
        // document the side effects of
        // this test.
        static::$tmpIncident = NULL;
        static::$tmpMessageLog->destroy();
        static::$tmpMessageLog = NULL;
        return;
    }
}