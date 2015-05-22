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
 *  date: Fri May 15 13:41:39 PDT 2015

 *  revision: rnw-15-2-fixes-release-01
 *  SHA1: $Id: fb8530c849618f7327af0a69049a32103c59052c $
 * *********************************************************************************************
 *  File: hooks.php
 * ****************************************************************************************** */

/*
  | -------------------------------------------------------------------------
  | Hooks
  | -------------------------------------------------------------------------
  | This file lets you define hooks to extend Customer Portal functionality. Hooks allow
  | you to specify custom code that you wish to execute before and after many
  | important events that occur within Customer Portal. This custom code can modify data,
  | perform custom validation, and return customized error messages to display to your users.
  |
  | Hooks are defined by specifying the location where you wish the hook to run as the array index
  | and setting that index to an array of 3 items, class, function, and filepath. The 'class' index
  | is the case-sensitive name of the custom model you wish to use. The 'function' index is the name
  | of the function within the 'class' you wish to call. Finally, the 'filepath' is the location to
  | your model, which will automatically be prefixed by models/custom/. The 'filepath' index only
  | needs a value if your model is contained within a subfolder
  |
  |-----------------
  | Hook Locations
  |-----------------
  |
  |     pre_allow_contact          - Called before allowing a contact to access content.
  |     pre_login                  - Called immediately before user becomes logged in
  |     post_login                 - Called immediately after user has been logged in
  |     pre_logout                 - Called immediately before user logs out
  |     post_logout                - Called immediately after user logs out
  |     pre_contact_create         - Called before Customer Portal validation and contact is created
  |     post_contact_create        - Called immediately after contact has been created
  |     pre_contact_update         - Called before Customer Portal validation and contact is updated
  |     post_contact_update        - Called immediately after contact is updated
  |     pre_incident_create        - Called before Customer Portal validation and incident is created
  |     pre_incident_create_save   - Called before save is called on the Incident Connect object. Returning a string will prevent the save function from being called and will set the $incident object to the 'incident' key in the hook data.
  |     post_incident_create       - Called immediately after incident has been created
  |     pre_register_smart_assistant_resolution - Called before the KnowledgeFoundation\Knowledge::RegisterSmartAssistantResolution is called. Returning a string will prevent the KnowledgeFoundation\Knowledge::RegisterSmartAssistantResolution function from being called.
  |     pre_incident_update        - Called before Customer Portal validation and incident is updated
  |     post_incident_update       - Called immediately after incident is updated
  |     pre_siebel_incident_submit - Called before Customer Portal submits a Service Request to a configured Siebel instance
  |     post_siebel_incident_error - Called after Customer Portal submits a Service Request to a configured Siebel instance if an error occurs
  |     pre_asset_create	         - Called before Customer Portal validation and asset is created
  |     post_asset_create          - Called immediately after asset has been created
  |     pre_asset_update           - Called before Customer Portal validation and asset is updated
  |     post_asset_update          - Called immediately after asset is updated
  |     pre_feedback_submit        - Called before both site and answer feedback
  |     post_feedback_submit       - Called after both site and answer feedback is submitted
  |     pre_login_redirect         - Called before user is redirected to a new page because they are not logged in
  |     pre_pta_decode             - Called before PTA string is decoded and converted to pairdata
  |     pre_pta_convert            - Called after PTA string has been decoded and converted into key/value pairs
  |     pre_page_render            - Called before page content is sent to the browser
  |     pre_report_get             - Called before a report is retrieved
  |     pre_report_get_data        - Called before submitting the report and allows for modification of the query parameters.
  |     post_report_get_data       - Called after the report data has been retrieved and allows for modification of the report data.
  |     pre_page_set_selection     - Called before the user is redirected to a specific page set
  |     pre_attachment_upload      - Called before an attachment is uploaded to the server. Return a string error message to prevent the file from being uploaded.
  |     pre_attachment_download    - Called before an attachment is downloaded from the server. Set `preventBrowserDisplay` to true to prevent the attachment from being rendered in the client's browser.
  |
  |
  | Please refer to the documentation for further information
  |
  |------------------
  |Examples
  |------------------
  |
  | Example 1 - Call the sendFeedback function immediately after an incident is created
  |             using the Immediateincidentfeedback_model
  |             (located at /models/custom/immediateincidentfeedback_model.php).
  |
  | $rnHooks['post_incident_create'] = array(
  |        'class' => 'Immediateincidentfeedback_model',
  |        'function' => 'sendFeedback',
  |        'filepath' => ''
  |    );

  |=========================================================================================================

  | Example 2 - Call the copyLogin function immediately before a contact is created using
  |             the Customcontact_model (located at /models/custom/contact/customcontact_model.php)
  |
  | $rnHooks['pre_contact_create'] = array(
  |        'class' => 'Customcontact_model',
  |        'function' => 'copyLogin',
  |        'filepath' => 'contact'
  |    );
  |=========================================================================================================

  | Example 3 - First call the customValidation function from the Myfeedback_model
  |             (located at /models/custom/feedback/myfeedback_model.php) then call
  |             the sendFeedback function from Immediateincidentfeedback_model (located at
  |             /models/custom/immediateincidentfeedback_model.php). The first function will be called
  |             before the feedback is submitted. The second will be called after.
  |
  | $rnHooks['pre_feedback_submit'][] = array(
  |        'class' => 'Myfeedback_model',
  |        'function' => 'customValidation',
  |        'filepath' => 'feedback'
  |    );
  | $rnHooks['post_feedback_submit'][] = array(
  |        'class' => 'Immediateincidentfeedback_model',
  |        'function' => 'sendFeedback',
  |        'filepath' => ''
  |    );
  |=========================================================================================================
 */


/**
 * Called immediately after incident is updated
 * 
 */
$rnHooks['post_incident_update'][] = array(
    'class' => 'CustomHook',
    'function' => 'updateSRAfterUpdateIncidentHook',
    'filepath' => ''
);

/**
 * Called before page content is sent to the browser
 */
$rnHooks['pre_page_render'][] = array(
    'class' => 'CustomHook',
    'function' => 'checkBeforeLoadingPageHook',
    'filepath' => ''
);


$rnHooks['post_incident_create'][] = array(
    'class' => 'CustomHook',
    'function' => 'postIncidentCreateHook',
    'filepath' => ''
);

/**
 * Called after a contact is created
 */
//$rnHooks['post_contact_create'][] = array(
//    'class' => 'ContactHook',
//    'function' => 'contactCreated',
//    'filepath' => ''
//);
//
//$rnHooks['post_login'][] = array(
//    'class' => 'ContactHook',
//    'function' => 'firstLogin',
//    'filepath' => ''
//);
//
///**
//* Called immediately before user becomes logged in
//* Please read the instructions in Beehive at
//* Service Cloud Accelerator Program/Public Documents/Work Products/saml.sso/sso.saml.pdf
//* for using this hook 
//*/
//$rnHooks['pre_login'][] = array(
//    'class' => 'Saml_contact',
//    'function' => 'contactLogin',
//    'filepath' => ''
//);
