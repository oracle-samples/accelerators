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


import java.util.ArrayList;

import java.util.HashMap;

import javax.el.ValueExpression;

import lov.ListOfValue;

import lov.ListOfValueController;

import oracle.adfmf.bindings.dbf.AmxIteratorBinding;
import oracle.adfmf.framework.api.AdfmfContainerUtilities;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;

import oracle.adfmf.json.JSONException;
import oracle.adfmf.json.JSONObject;

import rest_adapter.RestAdapter;

import util.Util;

/**
 * A controller for organizations.
 */
public class OrganizationController {
    private Organization cachedOrganization;
    private ListOfValue[] industry = null;
    private Organization organization;

    public OrganizationController() {
        super();
        System.out.println("Some Org Constructor" );
    }

    public Organization loadOrganization(String id){
        Organization org = new Organization();
        System.out.println("Some Org id: " + id);
        Object obj = null;
        try {
            // the so-called "className" argument is really packageName.className
            obj = Util.loadObject("organizations.Organization", "Organizations", id, OrganizationAttributes.organizationAttrs);

            if (obj!=null) {
                org = (Organization)obj;
            } else {
                System.out.println("Some Org id LOADED Error NULL: " + id);
            }

            System.out.println("Some Complex Lookup Org id: " + id);
            obj = Util.loadObject("organizations.Organization", "Organizations", id, OrganizationAttributes.organizationMultiAttrs);

            if (obj!=null) {
                Organization org2 = (Organization)obj;
                org.setStreet( org2.getStreet() );
                org.setCity( org2.getCity() );
                org.setState( org2.getState() );
                org.setPostalCode( org2.getPostalCode() );
                org.setCountry( org2.getCountry() );
                org.setCountryId(org2.getCountryId());
                org.setStateId(org2.getStateId());
            } else {
                System.out.println("Some Org CX id LOADED Error NULL: " + id);
            }
        } catch (Exception e) {
            System.out.println("Exception loading.. "+e);
        }
        this.organization = org;
        return org;
    }

    public void cacheEditedOrganization(Organization organization) {
        System.out.println("caching Organization");
        this.cachedOrganization = organization;
    }

    public void updateOrganization(Organization org) throws JSONException {
        //Required Fields Validation
        ArrayList<String> invf = new ArrayList<String>(20);
        if(Util.isNullOrWhitespace(org.getName())) invf.add("Name cannot be empty");

        boolean mas, sas, css;
        mas = org.getMaStateBool();
        sas = org.getSaStateBool();
        css = org.getCssStateBool();

        if (!(mas || sas || css)) {
            invf.add("You must select at least one State");
        }

        if (invf.size() > 0) {
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(
                AdfmfJavaUtilities.getFeatureId(),
                "navigator.notification.alert",
                new Object[] { Util.join(invf, ", ", null), null, "Alert", "OK" });
            return;
        }

        // create payload
        JSONObject payload = new JSONObject();
        payload.put("name", org.getName());
        // add state
        JSONObject state = new JSONObject();
        state.put("marketing", mas);
        state.put("sales", sas);
        state.put("service", css);
        payload.put("cRMModules", state);
        // add industry
        if (null == org.getIndustryId() || 0 == org.getIndustryId().intValue()) {
            payload.put("industry", JSONObject.NULL);
        }
        else {
            JSONObject industry = new JSONObject();
            industry.put("id", org.getIndustryId().intValue());
            payload.put("industry", industry);
        }

        //Update Organization
        RestAdapter.Status suc;
        suc =
            Util.updateObject(payload, Organization.RESOURCE_NAME + "/" + Integer.toString(org.getId()));

        if ("200".equals(suc.getStatus())){
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                                                "adf.mf.api.amx.doNavigation",
                                                                new Object[] {"__back"});
        }
        else {
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(
                AdfmfJavaUtilities.getFeatureId(),
                "navigator.notification.alert",
                new Object[] { "Organization could not be updated. Please contact your administrator.",
                               null, "Alert", "OK" });
            return;
        }
    }

    public void cancelCreateEditOrganization(){
        String cancelMsg = "Are you sure you want to leave this page without saving?";
        AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                                                "discardConfirmation",
                                                                new Object[] {cancelMsg, ""});
    }

    public ListOfValue[] getIndustry() {
            this.industry = ListOfValueController.getLov("/organizations/industry");
            return this.industry;
    }

    public Organization getOrganization() {
        if(this.cachedOrganization != null){
            System.out.println("returning cached Organization");
            this.organization = this.cachedOrganization;
            this.cachedOrganization = null;
            return this.organization;
        }

        if (null != this.organization) return this.organization;

        String id = (String) AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.organizationId}");
        this.organization = new Organization();
        System.out.println("load Organization id: " + id);
        if(id != null && !id.isEmpty())
        {
            Organization obj = loadOrganization(id);
            if(null != obj){
                this.organization = obj;
            }
        }
        return this.organization;
    }

    public void setOrganization(Organization organization) {
        this.organization = organization;
    }

}
