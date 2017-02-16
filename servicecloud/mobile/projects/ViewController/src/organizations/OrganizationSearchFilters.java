/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published 
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 
 *  included in the original distribution. 
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved. 
  ***********************************************************************************************
 *  Accelerator Package: OSVC Mobile Application Accelerator 
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html 
 *  OSvC release: 16.11 (November 2016) 
 *  date: Mon Dec 12 02:05:30 PDT 2016 
 *  revision: rnw-16-11

 *  SHA1: $Id$
 * *********************************************************************************************
 *  File: This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.

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
