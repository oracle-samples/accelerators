<!--
/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.5 (May 2015)
 *  EBS release: 12.1.3
 *  reference: 150202-000157
 *  date: Wed Sep  2 23:11:31 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: 989530fa1270c78cd21d640a27d64eef5249e687 $
 * *********************************************************************************************
 *  File: ask.php
 * ****************************************************************************************** */
-->

<rn:meta title="#rn:msg:ASK_QUESTION_HDG#" template="standard.php" clickstream="incident_create"/>

<div id="rn_PageTitle" class="rn_AskQuestion">
    <h1>#rn:msg:SUBMIT_QUESTION_OUR_SUPPORT_TEAM_CMD#</h1>
</div>
<div id="rn_PageContent" class="rn_AskQuestion">
    <div class="rn_Padding">
        <form id="rn_QuestionSubmit" method="post" action="/ci/ajaxRequest/sendForm">
            <div id="rn_ErrorLocation"></div>

            <rn:condition logged_in="true">
                <rn:widget path="input/FormInput" name="Incident.Subject" required="true" initial_focus="true" label_input="#rn:msg:SUBJECT_LBL#"/>
                <rn:widget path="input/FormInput" name="Incident.CustomFields.Accelerator.ebs_sr_request_type" required="true" label_input="Request Type"/>
                <rn:widget path="input/ProductCategoryInput" name="Incident.Product"/>
                <rn:widget path="custom/EbsServiceRequest/SerialNumberInput" name="Incident.CustomFields.Accelerator.ebs_serial_number" label_input="Serial Number" maximum_length="30"/>
                <rn:widget path="input/FormInput" name="Incident.Threads" required="true" label_input="#rn:msg:QUESTION_LBL#" maximum_length="1000"/>
                <rn:widget path="input/FormSubmit"  on_success_url="/app/ask_confirm" error_location="rn_ErrorLocation" timeout="180000"/>
                <rn:condition answers_viewed="2" searches_done="1">
                    <rn:condition_else/>
                    <rn:widget path="input/SmartAssistantDialog"/>
                </rn:condition>
            </rn:condition>
        </form>
    </div>
</div>
