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
 *  SHA1: $Id: 786babe7309e20202446c973e6586d4d9fbd58e5 $
 * *********************************************************************************************
 *  File: CTIConnect.php
 * ****************************************************************************************** */
 
namespace Custom\Models;

use RightNow\Connect\v1_3 as Connect;

class CTIConnect extends \RightNow\Models\Base
{
    function __construct()
    {
        parent::__construct();
    }

    /**
     * Searches the DB for a particular contact based on the raw phone number
     * and returns a contact
     * @param $phoneNumber
     * @return false|Connect\Contact
     */
    function findContactByPhoneNo($phoneNumber)
    {
        $contact = false;
        $phoneNumber = str_replace("+", "", $phoneNumber);
        $roql_result_set = Connect\ROQL::query("SELECT ID,Name.First,Name.Last FROM Contact C where Phones.PhoneList.RawNumber='" . $phoneNumber . "' LIMIT 1");
        
        while ($roql_result = $roql_result_set->next()) {
            if ($row = $roql_result->next()) {
                $contact = Connect\Contact::fetch($row[ID]);
            }
        }
       
        return $contact;
    }

    /**
     * Creates a contact for the particular phone number
     * @param $phoneNumber
     * @return null|Connect\Contact
     */
    function createDummyContact($phoneNumber)
    {
        $newContact = null;
        try {
            $newContact = new Connect\Contact();
            $newContact->Name = new Connect\PersonName;
            $newContact->Name->First = 'Unknown';
            $newContact->Name->Last = 'Caller';
            $newContact->Phones = new Connect\PhoneArray();
            $i = 0;
            $newContact->Phones[$i] = new Connect\Phone();
            $newContact->Phones[$i]->PhoneType = new Connect\NamedIDOptList();
            $newContact->Phones[$i]->PhoneType->LookupName = 'Mobile Phone';
            $newContact->Phones[$i]->Number = "$phoneNumber";
            $newContact->save();
        } catch (Connect\ConnectAPIError $err) {
            echo $err->getMessage();
        }
        return $newContact;
    }

    function attachVoicemail( $url, $duration, $cid )
    {
        $incident = new Connect\Incident();

        try
        {
            $contact = Connect\Contact::fetch( $cid );
            $incident->Subject = "Voicemail from '" . $contact->Name->First . " " . $contact->Name->Last . "'";
            $incident->Threads = new Connect\ThreadArray();
            $incident->Threads[0] = new Connect\Thread();
            $incident->Threads[0]->EntryType = new Connect\NamedIDOptList();
            $incident->Threads[0]->EntryType->ID = 3;
            $incident->Threads[0]->Text = "Voicemail with duration: " . $duration . " seconds.\nRecording URL: ". $url . ".mp3";
            $incident->PrimaryContact = $contact;
            $incident->save();

            return $incident->ID;

        }
        catch( Connect\ConnectAPIError $e )
        {
            var_dump( $e );
            return false; 
        }
    }

    function attachTranscription( $text, $phone )
    {
        $incident = false;
        $contact = $this->getContactData( $phone );
        $roql_result_set = Connect\ROQL::query("SELECT ID FROM Incident WHERE PrimaryContact.Contact = " . $contact["id"] . " AND StatusWithType.StatusType.ID != 2 order by CreatedTime desc")->next();
        
        $count = $roql_result_set->count();

        if( $count > 0 )
        {
            $row = $roql_result_set->next();
            $cid = $row["ID"];
            $incident = Connect\Incident::fetch( $cid );
        }
        
        if( $incident == false )
        {
            return false;
        }
        
        try
        {
            $tText = "Voicemail Transcription: " . $text;
            if( $text == false )
            {
                $tText = "Voicemail Transcription failed!";
            }
            $incident->Threads = new Connect\ThreadArray();
            $tcount = count( $incident->Threads );
            $incident->Threads[$tcount] = new Connect\Thread();
            $incident->Threads[$tcount]->EntryType = new Connect\NamedIDOptList();
            $incident->Threads[$tcount]->EntryType->ID = 3;
            $incident->Threads[$tcount]->Text = $tText;
            $incident->save();

            return $incident->ID;

        }
        catch( Connect\ConnectAPIError $e )
        {
            return false;
        }
    }


    function logInboundMessage( $from, $message )
    {
        $log = new Connect\Accelerator\MessageLog();
        $contact = $this->getContactData( $from );
        $log->Contact = $contact["id"];
        $log->Message = $message;
        // Check if there are open incidents for this contact
        $roql_result_set = Connect\ROQL::query("SELECT ID FROM Incident WHERE PrimaryContact.Contact = " . $contact["id"] . " AND StatusWithType.StatusType.ID != 2 order by CreatedTime desc")->next();

        
        $count = $roql_result_set->count();
        $incident = false;

        if( $count == 1 )
        {
            $row = $roql_result_set->next();
            $incident = Connect\Incident::fetch( $row["ID"] );

        }
        else if( $count == 0)
        {
            $incident = new Connect\Incident();
            $incident->PrimaryContact = Connect\Contact::fetch( $contact["id"] );
            $incident->Subject = "Incoming SMS from " . $contact["name"];
            $incident->Threads = new Connect\ThreadArray();
        }
        
        if( $incident != false )
        {
            $tcount = count( $incident->Threads );

            $incident->Threads[$tcount] = new Connect\Thread();
            $incident->Threads[$tcount]->EntryType = new Connect\NamedIDOptList();
            $incident->Threads[$tcount]->EntryType->ID = 3;
            $incident->Threads[$tcount]->Text = "Incoming SMS from $from: " . $message;

            $incident->save();
            $log->Incident = $incident->ID;
        }

        $log->Phone = $from;
        $log->Direction = "inbound";
        $log->save();
    }

    function logOutboundMessage( $to, $message, $incident = false )
    {
        $log = new Connect\Accelerator\MessageLog();
        $contact = $this->getContactData( $to );
        $log->Contact = $contact["id"];
        $log->Message = $message;

        if( $incident != false )
        {
            try 
            {
                $incident = Connect\Incident::fetch( $incident );
                $log->Incident = $incident->ID;
            }
            catch( Connect\ConnectAPIError $e )
            {
                // If the incident passed is an invalid incident, we just ignore it. 
            }
        }
        
        $log->Phone = $to;
        $log->Direction = "outbound";
        $log->save();
    }
    
    function setChatOfferState( $callSid )
    {
        $log = new Connect\Accelerator\ChatOfferState();
        $log->CallSid = $callSid;
        $log->QueueSid = $_POST["QueueSid"];
        if( isset( $_POST["From"] ) )
        {
            $log->Phone = $_POST["From"];
        }
        $log->save();

        return $log->ID;
    }

    function findChatOfferState( $id )
    {
        $log = NULL;
        try
        {
            $log = Connect\Accelerator\ChatOfferState::fetch( $id );
        }
        catch( Connect\ConnectAPIError $e )
        {}
        return $log;
    }

    function logCallAction( $accId, $action )
    {
        $log = new Connect\Accelerator\CallLog();

        $log->CallAction = $action;
        $log->AccountID = $accId;

        $log->save();

    }

    function getContactData( $phone, $create = true )
    {
        $contact = self::findContactByPhoneNo( $phone );

        if( $contact == false )
        {
            if( $create )
            {
                $contact = self::createDummyContact( $phone );
            }
            else
            {
                return false;
            }
        }

        $email = null;
        if( count($contact->Emails) >0 )
        {
            $email = $contact->Emails[0]->Address;
        }
        // Compute gravathar URL
        $url = "https://www.gravatar.com/avatar/";
        $url .= md5( $email ?: "example@example.com" );
        $url .= "?d=mm"; 

        $contact_data = array(
            "id"            => $contact->ID,
            "name"          => $contact->Name->First . " " . $contact->Name->Last,
            "firstName"     => $contact->Name->First,
            "lastName"      => $contact->Name->Last,
            "phone"         => $phone,
            "email"         => $email,
            "dp"            => $url
        );

        return $contact_data;
    }

    function createIncidentWithProductId( $contactId, $productId )
    {
        $incident = new Connect\Incident();

        try
        {
            $contact = Connect\Contact::fetch( $contactId );
            $incident->Subject = "Incoming Call from '" . $contact->Name->First . " " . $contact->Name->Last . "'";
            $incident->Product = Connect\ServiceProduct::fetch( $productId );
            $incident->PrimaryContact = $contact;
            $incident->save();

            return $incident->ID;

        }
        catch( Connect\ConnectAPIError $e )
        {
            return false;
        }
    }

    
    function getAccountDetails( $ids )
    {
        $result = array();

        foreach( $ids as $idx => $id )
        {
            if( $id == '' || !is_numeric( $id ) )
            {
                continue;
            }
            try
            {
                
                $account = Connect\Account::fetch( $id );
                $emails = $account->Emails;
                $email = "";
                if( count($emails) > 0 )
                {
                    $email = $emails[0]->Address;
                }
                // Compute gravathar URL
                $url = "https://www.gravatar.com/avatar/";
                $url .= md5( $email ?: "example@example.com" );
                $url .= "?d=mm"; 

                $account_data = array(
                    "id"   => $account->ID,
                    "name" => $account->DisplayName,
                    "email" => $email,
                    "dp" => $url
                );
                $result[] = $account_data;
            }
            catch( Connect\ConnectAPIError $e )
            {
                continue;
            }
        }

        return $result;
    }
    
    function findOpenIncident( $from )
    {
        $contact = $this->getContactData( $from );

        $roql_result_set = Connect\ROQL::query("SELECT ID FROM Incident WHERE PrimaryContact.Contact = " . $contact["id"] . " AND StatusWithType.StatusType.ID != 2 order by CreatedTime desc")->next();

        $count = $roql_result_set->count();

        $incident = false;
        if( $count == 1 )
        {
            $row = $roql_result_set->next();
            $incident = Connect\Incident::fetch( $row["ID"] );
        }

        if( $incident != false )
        {
            return [
                "id" => $incident->ID,
                "ref"=> $incident->LookupName
            ];
        }
        return $count;
    }


}