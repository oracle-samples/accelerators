/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: Mobile Agent App Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.8 (August 2016)
 *  MAF release: 2.3
 *  reference: 151217-000185
 *  date: Tue Aug 23 16:35:59 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: 33ef476e72d3f183edf319c65ac4b44d2898bdd0 $
 * *********************************************************************************************
 *  File: OrganizationSearchFilters.java
 * *********************************************************************************************/

package organizations;

public class OrganizationSearchFilters {
    private String _name;
    private Integer _industryId;
    private String _industry;
    private String _login;
    
    public OrganizationSearchFilters() {
        super();
    }
    
    public OrganizationSearchFilters(OrganizationSearchFilters filters) {
        super();
        this._name = filters._name;
        this._industryId = filters._industryId;
        this._industry = filters._industry;
        this._login = filters._login;
    }

    public void setName(String _name) {
        this._name = _name;
    }

    public String getName() {
        return _name;
    }

    public void setIndustryId(Integer _industryId) {
        this._industryId = _industryId;
    }

    public Integer getIndustryId() {
        return _industryId;
    }

    public void setIndustry(String _industry) {
        this._industry = _industry;
    }

    public String getIndustry() {
        return _industry;
    }

    public void setLogin(String _login) {
        this._login = _login;
    }

    public String getLogin() {
        return _login;
    }
}
