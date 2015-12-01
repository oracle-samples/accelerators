<!--
/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 150520-000047
 *  date: Mon Nov 30 19:59:29 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: aa7c81a8a04f1943be52d97bc747103485f6a65a $
 * *********************************************************************************************
 *  File: profile.php
 * ****************************************************************************************** */
-->

<rn:meta title="#rn:msg:ACCOUNT_SETTINGS_LBL#" template="standard.php" login_required="true" force_https="true" />

<div class="rn_Hero">
    <div class="rn_Container">
        <h1>#rn:msg:ACCOUNT_SETTINGS_LBL#</h1>
    </div>
</div>

<div class="rn_PageContent rn_Profile rn_Container">
    <rn:condition flashdata_value_for="info">
        <div class="rn_MessageBox rn_InfoMessage">
            #rn:flashdata:info#
        </div>
    </rn:condition>

    <rn:condition url_parameter_check="msg != null">
        <div class="rn_MessageBox rn_InfoMessage">#rn:url_param_value:msg#</div>
    </rn:condition>

    <form id="rn_CreateAccount" onsubmit="return false;">
        <div id="rn_ErrorLocation"></div>
        <h2>#rn:msg:ACCT_HDG#</h2>

        <rn:condition external_login_used="true">
            <rn:container read_only="true">
        </rn:condition>

        <fieldset>
            <legend>#rn:msg:ACCT_HDG#</legend>
            <rn:widget path="input/FormInput" name="Contact.Emails.PRIMARY.Address" required="true" validate_on_blur="true" initial_focus="true" label_input="#rn:msg:EMAIL_ADDR_LBL#"/>
            <rn:widget path="input/FormInput" name="Contact.Login" required="true" validate_on_blur="true" label_input="#rn:msg:USERNAME_LBL#" hint="#rn:msg:TH_PRIVATE_S_LOG_IN_SITE_JUST_WANT_MSG#"/>
            <rn:condition external_login_used="false">
                <rn:condition config_check="EU_CUST_PASSWD_ENABLED == true">
                    <br>
                    <a href="/app/#rn:config:CP_CHANGE_PASSWORD_URL##rn:session#">#rn:msg:CHANGE_YOUR_PASSWORD_CMD#</a>
                </rn:condition>
            </rn:condition>
        </fieldset>

        <h2>#rn:msg:CONTACT_INFO_LBL#</h2>
        <fieldset>
            <legend>#rn:msg:CONTACT_INFO_LBL#</legend>
            <rn:condition config_check="intl_nameorder == 1">
                <rn:widget path="input/FormInput" name="Contact.Name.Last" label_input="#rn:msg:LAST_NAME_LBL#" required="true"/>
                <rn:widget path="input/FormInput" name="Contact.Name.First" label_input="#rn:msg:FIRST_NAME_LBL#" required="true"/>
                <rn:condition_else/>
                <rn:widget path="input/FormInput" name="Contact.Name.First" label_input="#rn:msg:FIRST_NAME_LBL#" required="true"/>
                <rn:widget path="input/FormInput" name="Contact.Name.Last" label_input="#rn:msg:LAST_NAME_LBL#" required="true"/>
            </rn:condition>
            <rn:condition language_in="ja-JP,ko-KR,zh-CN,zh-HK,zh-TW">
                <rn:widget path="input/FormInput" name="Contact.Address.PostalCode" label_input="#rn:msg:POSTAL_CODE_LBL#" />
                <rn:widget path="input/FormInput" name="Contact.Address.Country" label_input="#rn:msg:COUNTRY_LBL#"/>
                <rn:widget path="input/FormInput" name="Contact.Address.StateOrProvince" label_input="#rn:msg:STATE_PROV_LBL#"/>
                <rn:widget path="input/FormInput" name="Contact.Address.City" label_input="#rn:msg:CITY_LBL#"/>
                <!-- This widget is for Street BUT HAS A VERIFY BUTTON AND 
                        GETS THE REST OF THE ADDRESS FROM THE OTHER WIDGETS ABOVE -->
                <rn:widget path="custom/SiebelServiceRequest/PostalValidate" name="Contact.Address.Street" label_input="#rn:msg:STREET_LBL#" />
                <rn:condition_else />
                <rn:widget path="input/FormInput" name="Contact.Address.Street" label_input="#rn:msg:STREET_LBL#"/>
                <rn:widget path="input/FormInput" name="Contact.Address.City" label_input="#rn:msg:CITY_LBL#"/>
                <rn:widget path="input/FormInput" name="Contact.Address.Country" label_input="#rn:msg:COUNTRY_LBL#"/>
                <rn:widget path="input/FormInput" name="Contact.Address.StateOrProvince" label_input="#rn:msg:STATE_PROV_LBL#"/>
                <!-- This widget is for Street BUT HAS A VERIFY BUTTON AND 
                           GETS THE REST OF THE ADDRESS FROM THE OTHER WIDGETS ABOVE -->
                <rn:widget path="custom/SiebelServiceRequest/PostalValidate"
                           name="Contact.Address.PostalCode" 
                           label_input="#rn:msg:POSTAL_CODE_LBL#" />
            </rn:condition>
            <rn:widget path="input/FormInput" name="Contact.Phones.HOME.Number" label_input="#rn:msg:HOME_PHONE_LBL#"/>
            <rn:widget path="input/FormInput" name="Contact.CustomFields.Accelerator.siebel_contact_party_id" label_input="Siebel Contact Party ID"/>
            <rn:widget path="input/FormInput" name="Contact.CustomFields.Accelerator.siebel_contact_org_id" label_input="Siebel Contact Org ID"/>
        </fieldset>

        <rn:condition external_login_used="true">
            </rn:container>
        </rn:condition>

        <rn:condition is_social_user="false" is_active_social_user="true">
            <h2>#rn:msg:PUBLIC_PROFILE_LBL#</h2>
            <fieldset>
                <legend>#rn:msg:PUBLIC_PROFILE_LBL#</legend>

                <rn:condition is_social_user="true">
                    <a class="rn_AvatarLink" href="/app/#rn:config:CP_PUBLIC_PROFILE_URL#/user/#rn:profile:socialUserID##rn:session#" title="#rn:msg:VIEW_YOUR_PUBLIC_PROFILE_LBL#">
                        <rn:widget path="user/AvatarDisplay" user_id="#rn:profile:socialUserID#">
                    </a>
                    <br>
                </rn:condition>
                <a href="/app/account/profile_picture#rn:session#">#rn:msg:CHANGE_YOUR_PROFILE_PICTURE_LBL#</a>
                <br/><br/>
                <rn:widget path="input/DisplayNameInput" always_required="false"/>
            </fieldset>
        </rn:condition>
        <br/><br/>

        <h2>#rn:msg:CONTACT_INFO_LBL# - Shipping Address</h2>
        <fieldset>
            <!-- Demonstrating the Custom Widget Postal Validate -- for Custom Fields!!! -->
            <rn:widget path="input/FormInput" name="Contact.CustomFields.Accelerator.Shipping_Street" label_input="#rn:msg:STREET_LBL#"/>
            <rn:widget path="input/FormInput" name="Contact.CustomFields.Accelerator.Shipping_City" label_input="#rn:msg:CITY_LBL#"/>
            <rn:widget path="input/FormInput" name="Contact.CustomFields.Accelerator.Shipping_Country" label_input="#rn:msg:COUNTRY_LBL#"/>
            <rn:widget path="input/FormInput" name="Contact.CustomFields.Accelerator.Shipping_State" label_input="#rn:msg:STATE_PROV_LBL#"/>
            <rn:widget path="custom/SiebelServiceRequest/PostalValidate"
                       name="Contact.CustomFields.Accelerator.Shipping_PostalCode" 
                       label_input="#rn:msg:POSTAL_CODE_LBL#" 
                       label_button="Verify Shipping Address"
                       verification_required="true" 
                       field_street="Contact.CustomFields.Accelerator.Shipping_Street"
                       field_city="Contact.CustomFields.Accelerator.Shipping_City"
                       field_state="Contact.CustomFields.Accelerator.Shipping_State"
                       field_country="Contact.CustomFields.Accelerator.Shipping_Country"
                       field_zip="Contact.CustomFields.Accelerator.Shipping_PostalCode"/>
        </fieldset>

        <rn:condition external_login_used="false" is_social_user="false" is_active_social_user="true">
            <rn:widget path="input/FormSubmit" label_button="#rn:msg:SAVE_CHANGE_CMD#" on_success_url="none" label_on_success_banner="#rn:msg:PROFILE_UPDATED_SUCCESSFULLY_LBL#" error_location="rn_ErrorLocation"/>
        </rn:condition>
    </form>
</div>
