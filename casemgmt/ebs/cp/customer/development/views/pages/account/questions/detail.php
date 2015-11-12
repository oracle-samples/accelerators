<!--
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
 *  date: Thu Nov 12 00:52:36 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 8b680d7579963118a4240d72f6c4cbcf611bc4f4 $
 * *********************************************************************************************
 *  File: detail.php
 * ****************************************************************************************** */
-->

<rn:meta title="#rn:php:\RightNow\Libraries\SEO::getDynamicTitle('incident', \RightNow\Utils\Url::getParameter('i_id'))#" template="standard.php" login_required="true" clickstream="incident_view" force_https="true"/>

<div class="rn_Hero">
    <div class="rn_Container">
        <h1><rn:field name="Incident.Subject" highlight="true"/></h1>
    </div>
</div>

<div class="rn_PageContent rn_RecordDetail rn_IncidentDetail rn_Container">

    <rn:condition logged_in="true">
        <!-- Display the incident detail page if the current URL is /question/detail/i_id/{id} -->
        <rn:condition url_parameter_check="i_id != null">

            <!-- Incident Detail (standard widget) -->
            <h2 class="rn_HeadingBar">Incident Detail</h2>
            <div id="rn_AdditionalInfo">
                <rn:widget path="output/DataDisplay" name="Incident.CustomFields.Accelerator.ebs_sr_id" label="SR ID"/>
                <rn:widget path="output/DataDisplay" name="Incident.CustomFields.Accelerator.ebs_sr_num" label="SR NUM"/>
                <rn:widget path="output/DataDisplay" name="Incident.ReferenceNumber" label="Reference #"/>
                <rn:widget path="output/DataDisplay" name="Incident.Subject" label="#rn:msg:SUBJECT_LBL#" />
                <rn:widget path="output/DataDisplay" name="Incident.CreatedTime" label="#rn:msg:CREATED_LBL#" />
                <rn:widget path="output/DataDisplay" name="Incident.StatusWithType.Status" label="#rn:msg:STATUS_LBL#"/>
                <rn:widget path="output/DataDisplay" name="Incident.CustomFields.Accelerator.ebs_serial_number" label="Serial Number"/>
                <rn:widget path="output/ProductCategoryDisplay" name="Incident.Product" label="#rn:msg:PRODUCT_LBL#"/>
            </div>
            <br>

            <!-- Incident update form (standard widget)  -->
            <rn:condition incident_reopen_deadline_hours="0">
                <rn:condition url_parameter_check="readonly == 1">
                    <h2 class="rn_HeadingBar">#rn:msg:INC_REOPNED_UPD_FURTHER_ASST_PLS_MSG#</h2>
                    <rn:condition_else>
                        <h2 class="rn_HeadingBar">#rn:msg:UPDATE_THIS_QUESTION_CMD#</h2>
                        <div id="rn_ErrorLocation"></div>
                        <form id="rn_UpdateQuestion" onsubmit="return false;">
                            <rn:widget path="input/FormInput" name="Incident.Threads" label_input="#rn:msg:ADD_ADDTL_INFORMATION_QUESTION_CMD#" initial_focus="true" required="true" maximum_length="1000"/>
                            <rn:widget path="input/FormSubmit" on_success_url="/app/account/questions/list" error_location="rn_ErrorLocation"/>
                        </form>
                </rn:condition>
                <rn:condition_else/>
                <h2 class="rn_HeadingBar">#rn:msg:INC_REOPNED_UPD_FURTHER_ASST_PLS_MSG#</h2>
            </rn:condition>
            <br>

            <!-- Service Request interaction history (custom widget) -->
            <h2 class="rn_HeadingBar">#rn:msg:COMMUNICATION_HISTORY_LBL#</h2>
            <div id="rn_QuestionThread">
                <rn:widget path="custom/EbsServiceRequest/SrInteractionDisplay" maxrows="20"/>
            </div>
            <br>

            <rn:condition_else/>

            <!-- Display the Service Request Detail page if the current URL is /question/detail/sr_id/{id} -->
            <rn:condition url_parameter_check="sr_id != null">

                <!-- Service Request Detail (custom widget) -->
                <div class="rn_SrDetailDisplayContainer">
                    <h2 class="rn_HeadingBar">Service Request Detail</h2>
                    <div id="rn_AdditionalInfo">
                        <rn:widget path="custom/EbsServiceRequest/SrFieldDisplay" name="SR.SrId" label="SR ID" />
                        <rn:widget path="custom/EbsServiceRequest/SrFieldDisplay" name="SR.SrNum" label="SR NUM" />
                        <rn:widget path="custom/EbsServiceRequest/SrFieldDisplay" name="SR.IncidentRef" label="Reference #" />
                        <rn:widget path="custom/EbsServiceRequest/SrFieldDisplay" name="SR.Subject" label="Subject" />
                        <rn:widget path="custom/EbsServiceRequest/SrFieldDisplay" name="SR.RequestDate" label="Created"  />
                        <rn:widget path="custom/EbsServiceRequest/SrFieldDisplay" name="SR.Status" label="Status" />
                        <rn:widget path="custom/EbsServiceRequest/SrFieldDisplay" name="SR.SerialNumber" label="Serial Number" />
                        <rn:widget path="custom/EbsServiceRequest/SrFieldDisplay" name="SR.Product" label="Product" />
                        <rn:widget path="custom/EbsServiceRequest/GetSrDetail" />
                    </div>
                </div>
                <br>

                <!-- Service Request update form (custom widget) -->
                <rn:condition url_parameter_check="readonly == 1">
                    <h2 class="rn_HeadingBar">This service request cannot be updated. If you need further assistance, please submit a new question.</h2>
                    <rn:condition_else>
                        <h2 class="rn_HeadingBar">#rn:msg:UPDATE_THIS_QUESTION_CMD#</h2>
                        <div id="rn_ErrorLocation"></div>

                        <form id="rn_QuestionSubmit" method="post" action="/cc/ServiceRequestController/sendFormToCreateIncidentToLinkWithSR">
                            <div id="rn_ErrorLocation"></div>
                            <rn:widget path="input/FormInput" name="Incident.Threads" required="true" label_input="#rn:msg:ADD_ADDTL_INFORMATION_QUESTION_CMD#" maximum_length="1000"/>
                            <div class="rn_Hidden">
                                <rn:widget path="input/FormInput" name="Incident.CustomFields.Accelerator.ebs_sr_id" label_input="ic_text" default_value="#rn:url_param_value:sr_id#"/>
                            </div>

                            <rn:widget path="input/FormSubmit"  on_success_url="/app/account/questions/list" error_location="rn_ErrorLocation" timeout="180000"/>
                            <rn:condition answers_viewed="2" searches_done="1">
                                <rn:condition_else/>
                                <rn:widget path="input/SmartAssistantDialog"/>
                            </rn:condition>
                        </form>
                </rn:condition>
                <br>

                <!-- Service Request Communication History (custom widget) -->
                <h2 class="rn_HeadingBar">#rn:msg:COMMUNICATION_HISTORY_LBL#</h2>
                <div id="rn_QuestionThread">
                    <rn:widget path="custom/EbsServiceRequest/SrInteractionDisplay" maxrows="20"/>
                </div>

            </rn:condition> <!-- end of rn:condition url_parameter_check="sr_id != null -->
        </rn:condition> <!-- end of rn:condition url_parameter_check="i_id != null" -->
    </rn:condition> <!-- end of rn:condition logged_in="true" -->

    <div class="rn_DetailTools">
        <rn:widget path="utils/PrintPageLink" />
    </div>
</div>
