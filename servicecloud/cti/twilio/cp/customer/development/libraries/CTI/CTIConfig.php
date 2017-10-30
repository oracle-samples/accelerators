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
 *  SHA1: $Id: 9f238946a412606daca490bf3810bd4e0ba68554 $
 * *********************************************************************************************
 *  File: CTIConfig.php
 * ****************************************************************************************** */

use \RightNow\Connect\v1_3 as Connect;

class CTIConfig
{
    static $config = NULL;

    static function readConfig()
    {
        if( self::$config !== NULL )
        {
            return true;
        }

        try
        {
            $cfg = Connect\Configuration::fetch( "CUSTOM_CFG_Accel_Ext_Integrations" );
            $json = $cfg->Value;
            $json = json_decode( $json, true );
            
            if( json_last_error() != JSON_ERROR_NONE )
            {
                print( "Invalid configuration!!!" );die;
            }

            // grab CTI config
            if( !array_key_exists( "cti", $json ) )
            {
                print( "CTI configuration is not set properly");die;
            }

            if( !array_key_exists( "twilio", $json["cti"] ) )
            {
                print( "CTI Twilio configuration is not set properly");die;
            }

            self::$config = $json["cti"]["twilio"];

            define( 'twilio_account_sid', self::$config["account_sid"] );
            define( 'twilio_auth_token', self::$config["auth_token"] );
            define( 'twilio_app_sid', self::$config["app_sid"] );
            define( 'twilio_phone_number', self::$config["phone_number"] );
            
            return true;
        }
        catch( Exception $e )
        {
            print("Unable to fetch configurations!!!");die;
        }
        return false;
    }
    
    static function get($key, $default = NULL)
    {
        if( !self::readConfig() )
        {
            return false;
        }

        if( self::$config === NULL || !array_key_exists($key, self::$config ) )
        {
            return $default;
        }

        return self::$config[$key];
    }

    static function getTwilioCredentials()
    {
        return array(
            "account_sid" => twilio_account_sid,
            "auth_token"  => twilio_auth_token
        );
    }

}
