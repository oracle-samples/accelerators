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
 *  date: Tue Aug 23 16:35:58 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: 513e9c3b6fad7fddc5091195772cb06067a53109 $
 * *********************************************************************************************
 *  File: Organization.java
 * *********************************************************************************************/

package organizations;

import java.util.Locale;

import util.Util;

public class Organization {

    public static final String RESOURCE_NAME = "organizations";

    private Integer _id;

    private String _name;
    private String _parent;
    private String _state;  //address
    private String _street; //address
    private String _city;
    private String _postalCode;
    private String _country;
    private String _addressOneLine;
    private String _sla;
    private String _stateOfSla;
    private Integer _industryId;
    private String _industry;
    private String _printableOrgState;
    private Integer _cssState = 0; // Says boolean in the doc but comes  as Integer
    private Integer _maState = 0;
    private Integer _saState = 0;
    private Boolean _cssStateBool;
    private Boolean _maStateBool;
    private Boolean _saStateBool;
    private Integer _countryId;
    private Integer _stateId;

    public void setCssState(Integer _cssState) {
        this._cssState = _cssState;
        this._cssStateBool = 1 == _cssState ? true : false;
    }

    /**
     * Service. 0 or 1
     * @return
     */
    public Integer getCssState() {
        return _cssState ;
    }

    public void setMaState(Integer _maState) {
        this._maState = _maState;
        this._maStateBool = 1 == _maState ? true : false;
    }

    /**
     * Outreach 1, or 0. 
     * @return
     */
    public Integer getMaState() {
        return _maState ;
    }

    public void setSaState(Integer _saState) {
        this._saState = _saState;
        this._saStateBool = 1 == _saState ? true : false;
    }

    /**
     * Opportunities 0 or 1
     * @return
     */
    public Integer getSaState() {
        return _saState ;
    }

    public void setCssStateBool(boolean _cssState) {
        System.out.println(_cssState);
        this._cssState = _cssState ? 1 : 0;
        this._cssStateBool = _cssState;
    }

    public boolean getCssStateBool() {
        return _cssState == 1 ? true : false;
    }

    public void setMaStateBool(boolean _maState) {
        System.out.println(_maState);
        this._maState = _maState ? 1 : 0;
        this._maStateBool = _maState;
    }

    public boolean getMaStateBool() {
        return _maState == 1 ? true : false;
    }

    public void setSaStateBool(boolean _saState) {
        System.out.println(_saState);
        this._saState = _saState ? 1 : 0;
        this._saStateBool = _saState;
    }

    public boolean getSaStateBool() {
        return _saState == 1 ? true : false;
    }

    /**
     *not sure if used.
     * @param _printableOrgState
     */
    public void setPrintableOrgState(String _printableOrgState) {
        this._printableOrgState = _printableOrgState;
    }

    /**
     *gets all other private states and concats them. 
     * @return
     */
    public String getPrintableOrgState() {
        // preparing a bit-checkable value, to match the
        // "state" Pseudo-column which is described in the Data Dictionary.
        // Done that way, for code legacy issues (bit checking) and cuz cool of course. 
        int pseudoState = getCssState() * 1 + getMaState() * 2 + getSaState() * 4;
            
        String str = "";
        String[] states = {"Service", "Outreach", "Opportunities" };
        for (int i = 0; i <= 2; i++) {
            if ((pseudoState & (1<<i)) > 0) {  // 0 or "true"
                if (!"".equals(str) && (i > 0))
                    str += ", ";
                                    
                // insert word
                str += states[i];
            }
        }        
        return str;
    }

    /**
     * possibly not used
     * @param _addressOneLine
     */
    public void setAddressOneLine(String _addressOneLine) {
        this._addressOneLine = _addressOneLine;
    }

    /**
     * Returns a combined address on one-line from using internal address variables. 
     * @return
     */
    public String getAddressOneLine() {
        _addressOneLine = Util.getSafeString(_street)
                        + ", " + Util.getSafeString(_city) + " " + Util.getSafeString(_state)
                        + ", " + Util.getSafeString(_postalCode) +  " " + Util.getSafeString(_country);
        
        _addressOneLine = _addressOneLine.trim();
        if (_addressOneLine.startsWith(","))
            _addressOneLine = _addressOneLine.substring(1);
      
        _addressOneLine = _addressOneLine.trim();
        if (_addressOneLine.endsWith(","))
            _addressOneLine = _addressOneLine.substring(0, _addressOneLine.length()-1);
        
        return _addressOneLine;
    }

    public void setPostalCode(String _postalCode) {
        this._postalCode = _postalCode;
    }

    public String getPostalCode() {
        return _postalCode;
    }

    public void setCountry(String _country) {
        this._country = _country;
    }

    public String getCountry() {
        return _country;
    }

    public void setCountryId(Integer _country) {
        this._countryId = _country;
    }

    public Integer getCountryId() {
        return _countryId;
    }

    public Organization() {
        super();
    }

    public void setStateOfSla(String _stateOfSla) {
//        System.out.println("setStateOfSla:"+_stateOfSla);
        this._stateOfSla = _stateOfSla;
    }

    public String getStateOfSla() {
//        System.out.println("getStateOfSla:"+_stateOfSla);
        return _stateOfSla;
    }

    public void setCity(String _city) {
        this._city = _city;
    }

    public String getCity() {
        return _city;
    }

    public void setId(Integer _id) {
        this._id = _id;
    }

    public Integer getId() {
        return _id;
    }

    public void setName(String _name) {
        this._name = _name;
    }

    public String getName() {
        return _name;
    }

    public void setParent(String _parent) {
        this._parent = _parent;
    }

    public String getParent() {
        return _parent;
    }

    public void setState(String _state) {
        this._state = _state;
    }

    public String getState() {
        return _state;
    }

    public void setStateId(Integer _state) {
        this._stateId = _state;
    }

    public Integer getStateId() {
        return _stateId;
    }

    public void setStreet(String _street) {
        this._street = _street;
    }

    public String getStreet() {
        return _street;
    }

    public void setSla(String _sla) {
        this._sla = _sla;
    }

    public String getSla() {
        String st = getStateOfSla();
        if (st!=null && st.toUpperCase(Locale.US).startsWith("DISABLE"))
            return "";
        else
            return _sla;
    }

    public void setIndustry(String _industry) {
        this._industry = _industry;
    }

    public String getIndustry() {
        return _industry;
    }

    public void setIndustryId(Integer _industryId) {
        this._industryId = _industryId;
    }

    public Integer getIndustryId() {
        return _industryId;
    }
}
