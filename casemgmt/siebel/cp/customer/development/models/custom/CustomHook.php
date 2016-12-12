<?php

/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 150520-000047
 *  date: Mon Nov 30 20:14:20 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: 5ee367c8f5954b32e6b5503c908a7b244439f8ee $
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
     * Hook function to check permission before loading the page.
     * Handle cases such as:
     * - SR not exist
     * - user dosn't have the right to access the SR
     */
    function checkBeforeLoadingPageHook() {
        if (Text::beginsWith($_SERVER['REQUEST_URI'], '/app/error/')) {
            return;
        }
        if ($this->extServerType === 'SIEBEL') {
            $this->checkBeforeLoadingPageHookForSiebel();
        } else {
            $this->log->error('Invalid external server type', __METHOD__);
        }
    }

    /**
     * CheckBeforeLoadingPageHook for Siebel
     */
    private function checkBeforeLoadingPageHookForSiebel() {
        $url = $_SERVER['REQUEST_URI'];

        if (Text::beginsWith($url, '/app/account/questions/detail/i_id')) {
            // check if i_id in URL is valid
            $incidentID = Url::getParameter('i_id');
            if (!$incidentID || !is_numeric($incidentID)) {
                $this->log->error('Invalid i_id#{$incidentID}', __METHOD__, array(null, $this->contact));
                Url::redirectToErrorPage(9);
            }

            // check if the linked SR has been closed
            if ($incident = RNCPHP\Incident::fetch(intval($incidentID))) {
                if ($incident->StatusWithType->Status->ID === 2) { // solved
                    return;
                }
                if ($srID = $incident->CustomFields->Accelerator->siebel_sr_id) {
                    $srDetail = $this->checkServiceRequest($srID);

                    // if the status is closed, redirect to the read only page
                    if ($srDetail['STATUS'] === 'Closed' && Url::getParameter('readonly') !== "1") {
                        $this->log->debug("Redirect to read-only page", __METHOD__, array(null, $this->contact));
                        header("Location: /app/account/questions/detail/i_id/$incidentID/readonly/1" . Url::sessionParameter());
                        exit;
                    }
                }
            }
            return;
        }

        // sr detail page
        if (Text::beginsWith($url, '/app/account/questions/detail/sr_id') === true) {
            $srID = Url::getParameter('sr_id');

            // check SR
            $srDetail = $this->checkServiceRequest($srID);

            // check if SR has already been associated with an Incident in RN.
            // if so, redirect to the corresponding Incident detail page
            $integrationID = $srDetail['INTEGRATIONID'];
            $integrationID = explode(',', $integrationID);
            $incidentID = $integrationID[0];
            if ($incidentID) {
                $this->log->debug('Redirect to incident#{$incidentID} page', __METHOD__, array(null, $this->contact));
                header("Location: /app/account/questions/detail/i_id/{$incidentID}" . Url::sessionParameter());
                exit;
            }

            // if the status is closed, redirect to the read only page
            if ($srDetail['STATUS'] === 'Closed' && Url::getParameter('readonly') !== "1") {
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
        if (!$srID) {
            $this->log->error('Invalid sr_id#{$srID}', __METHOD__, array(null, $this->contact));
            Url::redirectToErrorPage(10);
        }

        // check if contact party id and org id have been set
        if (!$this->CI->utility->validateSiebelContactID($this->contact)) {
            $this->log->error('contact_party_id and/or contact_org_id not provided', __METHOD__, array(null, $this->contact));
            Url::redirectToErrorPage(12);
        }

        // get SR by sr_id
        $getSRResult = $this->CI->model('custom/SiebelServiceRequest')->getSRDetailByID($srID);
        if ($getSRResult->error) {
            $this->log->error('Unable to get SR#{$srID}', __METHOD__, array(null, $this->contact));
            Url::redirectToErrorPage(11);
        }

        // check if the current user is the owner of the SR, if not, redirect to permission deny page
        $srDetail = $getSRResult->result;
        $contactPartyID = ($this->contact !== null) ? $this->contact->CustomFields->Accelerator->siebel_contact_party_id : null;
        if ($contactPartyID !== $srDetail['CONTACTID']) {
            $this->log->error('Permission Denied', __METHOD__, array(null, $this->contact), "ContactPartyID#{$contactPartyID} doesn't match SR.contactId #{$srDetail['CONTACT_PARTY_ID']}");
            Url::redirectToErrorPage(4);
        }

        return $srDetail;
    }

    /**
     * Hook function after update an incident
     * @param array &$hookData Hook data
     */
    function updateSRAfterUpdateIncidentHook(array &$hookData) {
        if ($this->extServerType === 'SIEBEL') {
            $this->updateSRAfterUpdateIncidentHookForSiebel($hookData);
        } else {
            $this->log->error('Invalid external server type', __METHOD__);
        }
    }

    /**
     * UpdateSrAfterUpdateIncidentHook function for Siebel
     * @param array &$hookData Hook data
     * @return null
     */
    private function updateSRAfterUpdateIncidentHookForSiebel(array &$hookData) {
        // fetch incident by ID
        $incidentID = $hookData['data']->ID;
        if (!$incident = RNCPHP\Incident::fetch(intval($incidentID))) {
            $this->log->error("Unable to get Incident#{$incidentID}", __METHOD__, array(null, $this->contact));
            return;
        }

        // check if the Incident has been associated with a SR in Siebel
        if (!$srID = $incident->CustomFields->Accelerator->siebel_sr_id) {
            $this->log->error("No SR associate with Incident#{$incidentID}", __METHOD__, array($incident, $this->contact));
            return;
        }

        // create a new web-inbound action for the SR in Siebel
        $this->CI->model('custom/SiebelServiceRequest')->createWebInboundAction($srID, $incident);
    }

    /**
     * PostIncidentCreateHook for Siebel handles two differenct cases based on if the sr_id is empty
     * 1. if sr_id is empty, the hook is for incident created through the 'Ask a New Question' page
     * 2. otherwise, the hoos is incident created based on a legacy SR
     * @param array &$hookData HookData
     */
    function postIncidentCreateHook(array &$hookData) {
        $incidentID = $hookData['data']->ID;
        if (!$incident = RNCPHP\Incident::fetch(intval($incidentID))) {
            $this->log->error("Unable to get Incident#{$incidentID}", __METHOD__, array(null, $this->contact));
        } else {
            $this->log->debug("Incident #{$incidentID} has been created in CP", __METHOD__, array($incident, $this->contact));
        }

        // check if the incident has been associated with a SR
        if ($srID = $incident->CustomFields->Accelerator->siebel_sr_id) {
            $this->createIncidentToLinkWithSR($srID, $incident);
        } else {
            $this->createSRFromCPToLinkWithIncident($incident);
        }
    }

    /**
     * After Incident has been created from "Ask a New Question" page, 
     * create Service Request and a web-inbound action in Siebel, 
     * then save the sr_id in the incident
     * @param RNCPHP\Incident $incident Incident just created
     * @return null
     */
    private function createSRFromCPToLinkWithIncident(RNCPHP\Incident $incident) {
        if (!$this->CI->utility->validateSiebelContactID($this->contact)) {
            $this->log->error("contact_party_id and/or contact_org_id not provided", __METHOD__, array($incident, $this->contact));
            return;
        }

        // validate Serial Number if needed
        $serialNumberValidationResult = $this->CI->model('custom/SiebelServiceRequest')->validateSerialNumber(
                $incident->CustomFields->Accelerator->siebel_serial_number, $incident->Product->ID, $incident);

        // create new Service Request in Siebel to associate with the incident just created
        if (!$srID = $this->CI->model('custom/SiebelServiceRequest')->createSR($incident, $serialNumberValidationResult->result->ifPropagateSerialNumberAndProduct, $serialNumberValidationResult->result->asset)) {
            $this->log->error('Service Request creation failed', __METHOD__, array($incident, $this->contact));
            return;
        }

        // create new Web-Inbound action in Siebel 
        if (!$noteID = $this->CI->model('custom/SiebelServiceRequest')->createWebInboundAction($srID, $incident)) {
            $this->log->error('Web-Inbound action creation failed', __METHOD__, array($incident, $this->contact));
            return;
        }

        // getSR to retrieve the SR Number
        $getSRResult = $this->CI->model('custom/SiebelServiceRequest')->getSRDetailByID($srID);
        if (!$getSRResult->error) {
            $srNum = $getSRResult->result['SRNUMBER'];
        }

        // update the siebel_sr_id and siebel_sr_num field of the Inicdent
        $incident->CustomFields->Accelerator->siebel_sr_id = $srID;
        $incident->CustomFields->Accelerator->siebel_sr_num = $srNum;
        $incident->save(RNCPHP\RNObject::SuppressAll);
        $this->log->debug("Update Incident#{$incident->ID}.siebel_sr_id to {$srID}, sr_num to {$srNum}", __METHOD__, array($incident, $this->contact));
    }

    /**
     * Create Web-Inbound action and update the Service Request after create an incident based on the exiting Service Request
     * @param string $srID The corresponsing service request id of Incident
     * @param RNCPHP\Incident $incident Incident just created
     * @return null
     */
    private function createIncidentToLinkWithSR($srID, RNCPHP\Incident $incident) {
        // add web-inbound action for SR in Siebel
        if (!$noteID = $this->CI->model('custom/SiebelServiceRequest')->createWebInboundAction($srID, $incident)) {
            return;
        }

        // update the SR to save the IncidentID and IncidentRef fields
        $this->CI->model('custom/SiebelServiceRequest')->updateSR($srID, $incident);
    }

}
