<!--
/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:36 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: ae9c61763d8f5f7f09eb9be6d4a304086ecb9fd0 $
 * *********************************************************************************************
 *  File: list.php
 * ****************************************************************************************** */
-->

<rn:meta title="#rn:msg:SUPPORT_HISTORY_LBL#" template="standard.php" clickstream="incident_list" login_required="true" force_https="true" />
<rn:container report_id="196">
    <div class="rn_Hero">
        <div class="rn_HeroInner">
            <div class="rn_SearchControls">
                <h1 class="rn_ScreenReaderOnly">#rn:msg:SEARCH_CMD#</h1>
                <form onsubmit="return false;" class="translucent">
                    <div class="rn_SearchInput">
                        <rn:widget path="search/KeywordText" label_text="#rn:msg:SEARCH_YOUR_SUPPORT_HISTORY_CMD#" label_placeholder="#rn:msg:SEARCH_YOUR_SUPPORT_HISTORY_CMD#" initial_focus="true"/>
                    </div>
                    <rn:widget path="search/SearchButton"/>
                </form>
                <div class="rn_SearchFilters">
                    <rn:widget path="search/ProductCategorySearchFilter" />
                    <rn:widget path="search/ProductCategorySearchFilter" filter_type="Category"/>
                </div>
            </div>
        </div>
    </div>
    <div class="rn_PageContent rn_Container">
        <h2 class="rn_ScreenReaderOnly">#rn:msg:SEARCH_RESULTS_CMD#</h2>
        <h2>#rn:msg:SEARCH_RESULTS_CMD#</h2>
        <rn:widget path="reports/ResultInfo"/>
        <rn:widget path="reports/Grid" label_caption="<span class='rn_ScreenReaderOnly'>#rn:msg:SEARCH_YOUR_SUPPORT_HISTORY_CMD#</span>"/>
        <rn:widget path="reports/Paginator"/>
    </div>
    <div class="rn_PageContent rn_Container">
        <h2>EBS Service Request List</h2>
        <rn:widget path="custom/EbsServiceRequest/SrListGrid" max_row='25' display_attrs = 'SrNum,IncidentRef,CreatedTime,Subject,Status'/>
    </div>
</rn:container>
