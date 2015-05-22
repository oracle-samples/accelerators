<?php

/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC WSS + EBS Case Management Accelerator
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (February 2015)
 *  EBS release: 12.1.3
 *  reference: 140626-000078
 *  date: Fri May 15 13:41:40 PDT 2015

 *  revision: rnw-15-2-fixes-release-01
 *  SHA1: $Id: 2ac1cc36825fda8fd5f78fc5368882f2e7d8d0f4 $
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
     * Hook function to check permission before loading the page
     * Handle cases like:
     * - SR does not exist
     * - user dosen't have the right to access the SR
     */
    function checkBeforeLoadingPageHook() {
        if ($this->extServerType === 'EBS') {
            $this->checkBeforeLoadingPageHookForEbs();
        } else if ($this->extServerType === 'MOCK') {
            $this->checkBeforeLoadingPageHookForMock();
        } else {
            $this->log->error('Invalid external server type', __METHOD__);
        }
    }

    /**
     * CheckBeforeLoadingPageHook for EBS
     */
    private function checkBeforeLoadingPageHookForEbs() {
        $url = $_SERVER['REQUEST_URI'];
        if (Text::beginsWith($url, '/app/account/questions/detail/i_id')) {
            // check if the i_id in URL is valid
            $incidentID = Url::getParameter('i_id');
            if (!$incidentID || !is_numeric($incidentID)) {
                $this->log->error("Invalid i_id#{$incidentID}", __METHOD__, array(null, $this->contact));
                Url::redirectToErrorPage(9);
            }
        } else if (Text::beginsWith($url, '/app/account/questions/detail/sr_id') === true) {
            // check if the sr_id in URL is valid
            $srID = Url::getParameter('sr_id');
            if (!$srID || !is_numeric($srID)) {
                $this->log->error("Invalid sr_id#{$srID}", __METHOD__, array(null, $this->contact));
                Url::redirectToErrorPage(10);
            }

            // check if the contact party id and org id have been set
            if (!$this->CI->utility->validateEbsContactID($this->contact)) {
                $this->log->error("contact_party_id and/or contact_org_id not provided", __METHOD__, array(null, $this->contact));
                Url::redirectToErrorPage(12);
            }

            // get the SR by sr_id
            $getSRResult = $this->CI->model('custom/EbsServiceRequest')->getSRDetailByID($srID);
            if ($getSRResult->error) {
                $this->log->error("Unable to get SR#{$srID}", __METHOD__, array(null, $this->contact));
                Url::redirectToErrorPage(4);
            }

            // check if the current user is the owner of the SR, if not, redirect to the permission deny error page
            $srDetail = $getSRResult->result;
            $contactPartyID = ($this->contact !== null) ? $this->contact->CustomFields->Accelerator->ebs_contact_party_id : null;
            if ((string) $contactPartyID !== (string) $srDetail['CONTACT_PARTY_ID']) {
                $this->log->error('Permission Denied', __METHOD__, array(null, $this->contact), "ContactPartyID#{$contactPartyID} dosen't match SR.contactId #{$srDetail['CONTACT_PARTY_ID']}");
                Url::redirectToErrorPage(4);
            }

            // redirect to the corresponding Incident detail page if the SR has already associated with an Incident in RN
            $incidentID = $srDetail['EXTATTRIBUTE15'];
            if ($incidentID) {
                $this->log->debug("Redirect to incident#{$incidentID} page", __METHOD__, array(null, $this->contact));
                header("Location: /app/account/questions/detail/i_id/{$incidentID}" . Url::sessionParameter());
                exit;
            }
        }
    }

    /**
     * Hook function after update an incident
     * @param array &$hookData Hook data which contains the updated incident info
     */
    function updateSRAfterUpdateIncidentHook(array &$hookData) {
        if ($this->extServerType === 'EBS') {
            $this->updateSRAfterUpdateIncidentHookForEbs($hookData);
        } else if ($this->extServerType === 'MOCK') {
            $this->updateSrAfterUpdateIncidentHookForMock($hookData);
        } else {
            $this->log->error('Invalid external server type', __METHOD__);
        }
    }

    /**
     * UpdateSrAfterUpdateIncidentHook function for EBS.
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
