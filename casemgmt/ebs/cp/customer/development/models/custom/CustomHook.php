<?php

/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:35 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 7e049a1db27e3093f19f6ceecb50e41d90d51141 $
 * *********************************************************************************************
 *  File: CustomHook.php
 * ****************************************************************************************** */

namespace Custom\Models;

use RightNow\Connect\v1_2 as RNCPHP,
    \RightNow\Utils\Url as Url,
    \RightNow\Utils\Text as Text;

class CustomHook extends \RightNow\Models\Base {

    private $log;
    private $contact;
    private $extServerType;

    function __construct() {
        parent::__construct();

        $this->CI->load->library('Utility');

        $this->log = $this->CI->utility->getLogWrapper();
        $this->contact = $this->CI->utility->getLoginContact();
        $this->extServerType = $this->CI->model('custom/ExtIntegrationConfigVerb')->getExtServerType();
    }

    /**
     * CheckBeforeLoadingPageHook
     */
    function checkBeforeLoadingPageHook() {
        if (Text::beginsWith($_SERVER['REQUEST_URI'], '/app/error/')) {
            return;
        }

        if ($this->extServerType === 'EBS') {
            $this->checkBeforeLoadingPageHookForEbs();
        } else {
            $this->log->error('Invalid external server type', __METHOD__);
        }
    }

    /**
     * Execute the following check before loading page
     * - SR doesn't exist
     * - user doesn't have the right to access the SR
     * - SR has been closed
     */
    private function checkBeforeLoadingPageHookForEbs() {
        $url = $_SERVER['REQUEST_URI'];

        // incident detail page
        if (Text::beginsWith($url, '/app/account/questions/detail/i_id')) {
            // check if the i_id is valid
            $incidentID = Url::getParameter('i_id');
            if (!$incidentID || !is_numeric($incidentID)) {
                $this->log->error("Invalid i_id#{$incidentID}", __METHOD__, array(null, $this->contact));
                Url::redirectToErrorPage(9);
            }

            // check if the linked SR has been closed
            if ($incident = RNCPHP\Incident::fetch(intval($incidentID))) {
                if ($incident->StatusWithType->Status->ID === 2) { // solved
                    return;
                }
                if ($srID = $incident->CustomFields->Accelerator->ebs_sr_id) {
                    $srDetail = $this->checkServiceRequest($srID);

                    // if the status is closed, redirect to the read only page
                    if ($srDetail['INCIDENT_STATUS'] === 'Closed' && Url::getParameter('readonly') !== "1") {
                        $this->log->debug("Redirect to read-only page", __METHOD__, array(null, $this->contact));
                        header("Location: /app/account/questions/detail/i_id/$incidentID/readonly/1" . Url::sessionParameter());
                        exit;
                    }
                }
            }
            return;
        }

        // sr detail page
        if (Text::beginsWith($url, '/app/account/questions/detail/sr_id')) {
            $srID = Url::getParameter('sr_id');

            // check SR
            $srDetail = $this->checkServiceRequest($srID);

            // redirect to the incident detail page if the SR has already associated with an incident
            $incidentID = $srDetail['EXTATTRIBUTE15'];
            if ($incidentID) {
                $this->log->debug("Redirect to incident#{$incidentID} page", __METHOD__, array(null, $this->contact));
                header("Location: /app/account/questions/detail/i_id/{$incidentID}" . Url::sessionParameter());
                exit;
            }

            // if the status is closed, redirect to the read only page
            if ($srDetail['INCIDENT_STATUS'] === 'Closed' && Url::getParameter('readonly') !== "1") {
                $this->log->debug("Redirect to read-only page", __METHOD__, array(null, $this->contact));
                header("Location: /app/account/questions/detail/sr_id/$srID/readonly/1" . Url::sessionParameter());
                exit;
            }
        }
    }

    /**
     * fetch SR and check if the current user is the owner of the SR
     * @param int $srID Serivce Request ID
     * @return array|null Service Request detail
     */
    private function checkServiceRequest($srID) {
        if (!$srID || !is_numeric($srID)) {
            $this->log->error("Invalid sr_id#{$srID}", __METHOD__, array(null, $this->contact));
            Url::redirectToErrorPage(10);
        }

        // check if the contact party id and org id have been set
        if (!$this->CI->utility->validateEbsContactID($this->contact)) {
            $this->log->error("contact_party_id and/or contact_org_id not provided", __METHOD__, array(null, $this->contact));
            Url::redirectToErrorPage(12);
        }

        // get SR by sr_id
        $getSRResult = $this->CI->model('custom/EbsServiceRequest')->getSRDetailByID($srID);
        if ($getSRResult->error) {
            $this->log->error("Unable to get SR#{$srID}", __METHOD__, array(null, $this->contact));
            Url::redirectToErrorPage(11);
        }

        // check if the current user is the owner of the SR, if not, redirect to the permission deny error page
        $srDetail = $getSRResult->result;
        $contactPartyID = ($this->contact !== null) ? $this->contact->CustomFields->Accelerator->ebs_contact_party_id : null;
        if ((string) $contactPartyID !== (string) $srDetail['CONTACT_PARTY_ID']) {
            $this->log->error('Permission Denied', __METHOD__, array(null, $this->contact), "ContactPartyID#{$contactPartyID} dosen't match SR.contactId #{$srDetail['CONTACT_PARTY_ID']}");
            Url::redirectToErrorPage(4);
        }

        return $srDetail;
    }

    /**
     * Hook function after update an incident
     * @param array &$hookData Hook data
     */
    function updateSRAfterUpdateIncidentHook(array &$hookData) {
        if ($this->extServerType === 'EBS') {
            $this->updateSRAfterUpdateIncidentHookForEbs($hookData);
        } else {
            $this->log->error('Invalid external server type', __METHOD__);
        }
    }

    /**
     * UpdateSrAfterUpdateIncidentHook function for EBS
     * @param array &$hookData Hook data
     * @return null
     */
    private function updateSRAfterUpdateIncidentHookForEbs(array &$hookData) {
        // fetch the incident
        $incidentID = $hookData['data']->ID;
        if (!$incident = RNCPHP\Incident::fetch(intval($incidentID))) {
            $this->log->error("Unable to get Incident#{$incidentID}", __METHOD__, array(null, $this->contact));
            return;
        }

        // check if the incident has associated with a SR
        if (!$srID = $incident->CustomFields->Accelerator->ebs_sr_id) {
            $this->log->error("No Service Request associated with Incident#{$incidentID}", __METHOD__, array($incident, $this->contact));
            return;
        }

        // create a new Note for the SR in EBS
        $this->CI->model('custom/EbsServiceRequest')->createNote($srID, $incident);
    }

    /**
     * PostIncidentCreateHook for EBS handles two differenct cases based on if the sr_id is empty
     * 1. if sr_id is empty, the hook function is for Incident created through the 'Ask a New Question' page
     * 2. otherwise, the hook function is for Incident created based on a legacy SR
     * @param array &$hookData HookData which contains the incident
     */
    function postIncidentCreateHook(array &$hookData) {
        // fetch incident by ID
        $incidentID = $hookData['data']->ID;
        if (!$incident = RNCPHP\Incident::fetch(intval($incidentID))) {
            $this->log->error("Unable to get Incident#{$incidentID}", __METHOD__, array(null, $this->contact));
        } else {
            $this->log->debug("Incident#{$incidentID} has been created in CP", __METHOD__, array($incident, $this->contact));
        }

        // add additinal field to the newly created incident
        $this->addAdditionalFieldsToIncident($incident);

        // talk with EBS server
        if (!$srID = $incident->CustomFields->Accelerator->ebs_sr_id) {
            $this->createSRToLinkWithIncident($incident);
        } else {
            $this->createNoteAndUpdateSR($srID, $incident);
        }
    }

    /**
     * After Incident has been created from the "Ask a New Question" page,
     * create a corresponding Service Request(and Note) in EBS,
     * then save the SR ID in the Incident.
     * @param RNCPHP\Incident $incident Incident just created
     * @return null
     */
    private function createSRToLinkWithIncident(RNCPHP\Incident $incident) {
        // check if contact party id and org id have been set
        if (!$this->CI->utility->validateEbsContactID($this->contact)) {
            $this->log->error("contact_party_id and/or contact_org_id not provided", __METHOD__, array($incident, $this->contact));
            return;
        }

        // validate the serial number
        $serialNumberValidationResult = $this->CI->model('custom/EbsServiceRequest')->validateSerialNumber(
                $incident->CustomFields->Accelerator->ebs_serial_number, $incident->Product->ID, $incident);

        // create new Service Request in EBS to associate with the Incident just created
        list($srID, $srNum) = $this->CI->model('custom/EbsServiceRequest')->createSR($incident, $serialNumberValidationResult->result->ebsItem);
        if (!$srNum || !$srID) {
            $this->log->error('Service Request creation failed', __METHOD__, array($incident, $this->contact));
            return;
        }

        // create new Note in EBS according the question of the Incident
        if (!$this->CI->model('custom/EbsServiceRequest')->createNote($srID, $incident)) {
            $this->log->error('Service Request Note creation failed', __METHOD__, array($incident, $this->contact));
            return;
        }

        // update the ebs_sr_id and ebs_sr_num fields of the Inicdent
        $incident->CustomFields->Accelerator->ebs_sr_id = intval($srID);
        $incident->CustomFields->Accelerator->ebs_sr_num = $srNum;
        $incident->save(RNCPHP\RNObject::SuppressAll);
        $this->log->debug("Update Incident#{$incident->ID}.EBS_SR_ID to {$srID}, EBS_SR_NUM to {$srNum}", __METHOD__, array($incident, $this->contact));
    }

    /**
     * Add additional fields which are unable to be added by the standard input widget to the Incident 
     * @param RNCPHP\Incident $incident RightNow Incident object
     */
    private function addAdditionalFieldsToIncident(RNCPHP\Incident $incident) {
        // set the 'ebs_sr_owner_id'
        $incident->CustomFields->Accelerator->ebs_sr_owner_id = $this->CI->model('custom/ExtIntegrationConfigVerb')->getEbsDefaultSROwnerID();
        $incident->save(RNCPHP\RNObject::SuppressAll);
    }

    /**
     * Create Note and update the Service Request after create an Incident based on a legacy Service Request
     * @param int $srID Service Request ID
     * @param RNCPHP\Incident $incident RightNow Incident object
     * @return null
     */
    private function createNoteAndUpdateSR($srID, RNCPHP\Incident $incident) {
        // create Note for SR 
        if (!$this->CI->model('custom/EbsServiceRequest')->createNote($srID, $incident)) {
            return;
        }

        // update the SR to set the IncidentID and IncidentRef fields
        $sessionKey = 'sr_' . $srID;
        $srDetail = $this->CI->session->getSessionData($sessionKey);
        if (!$srDetail) {
            $getSRResult = $this->CI->model('custom/EbsServiceRequest')->getSRDetailByID($srID);
            if ($getSRResult->error) {
                $this->log->error("Unable to get SR#{$srID}", __METHOD__, array(null, $this->contact));
                return null;
            }
        }
        $this->CI->model('custom/EbsServiceRequest')->updateSR($srID, $srDetail, $incident);
    }

}
