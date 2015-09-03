<!--
/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.5 (May 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 141216-000121
 *  date: Wed Sep  2 23:14:33 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: 5cd4b5ea6a4cebb541f90ba5ea38b1da8206f0ed $
 * *********************************************************************************************
 *  File: list.php
 * ****************************************************************************************** */
-->

<rn:meta title="#rn:msg:SUPPORT_HISTORY_LBL#" template="standard.php" clickstream="incident_list" login_required="true" force_https="true" />
<rn:container report_id="101032">
    <div id="rn_PageTitle" class="rn_QuestionList">
        <div id="rn_SearchControls">
            <h1 class="rn_ScreenReaderOnly">#rn:msg:SEARCH_CMD#</h1>
            <form onsubmit="return false;">
                <div class="rn_SearchInput">
                    <rn:widget path="search/AdvancedSearchDialog"/>
                    <rn:widget path="search/KeywordText" label_text="#rn:msg:SEARCH_YOUR_SUPPORT_HISTORY_CMD#" initial_focus="true"/>
                </div>
                <rn:widget path="search/SearchButton"/>
            </form>
            <rn:widget path="search/DisplaySearchFilters"/>
        </div>
    </div>
    <div id="rn_PageContent" class="rn_QuestionList">
        <div class="rn_Padding">
            <h2>My Incident</h2>
            <rn:widget path="reports/ResultInfo"/>
            <rn:widget path="reports/Grid" label_caption="<span class='rn_ScreenReaderOnly'>#rn:msg:SEARCH_YOUR_SUPPORT_HISTORY_CMD#</span>"/>
            <rn:widget path="reports/Paginator"/>
        </div>

        <div class="rn_Padding">
            <h2>Service Request List</h2>
            <rn:widget path="custom/SiebelServiceRequest/SrListGrid" max_row='25'
                       display_attrs = 'SrNum,IncidentRef,CreatedTime,Subject,Status'/>
        </div>
    </div>
</rn:container>
