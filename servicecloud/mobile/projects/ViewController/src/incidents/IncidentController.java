/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: Mobile Agent App Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.8 (August 2016)
 *  MAF release: 2.3
 *  reference: 151217-000185
 *  date: Tue Aug 23 16:35:58 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: 602fc25eec144f29783a65c331a781bc14130ca7 $
 * *********************************************************************************************
 *  File: IncidentController.java
 * *********************************************************************************************/

package incidents;

import javax.el.ValueExpression;

import lov.ListOfValue;
import lov.ListOfValueController;

import oracle.adfmf.bindings.dbf.AmxIteratorBinding;
import oracle.adfmf.framework.api.AdfmfContainerUtilities;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;

import oracle.adfmf.json.JSONArray;
import oracle.adfmf.json.JSONObject;

import rest_adapter.RestAdapter;

import util.Util;

public class IncidentController {
    private Incident incident;
    private Incident cachedIncident;
    private ListOfValue[] statuses = null;
    private ListOfValue[] severities = null;
    
    public IncidentController() {
        super();
    }
    
    public Incident getIncident(){
        // set the pageFlowScope.ScannerPage for barcode scanner js check
        AdfmfJavaUtilities.setELValue("#{pageFlowScope.ScannerPage}", "AssetQuickSearch");
        if(cachedIncident != null){
            System.out.println("cachedInc");
            incident = new Incident();
            incident = cachedIncident;
            
            Object productId = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.productId}");
            Object productName = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.productName}");
            if(productId != null){
                incident.setProductId(Integer.parseInt((String)productId));
                incident.setProduct((String)productName);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.productId}", null);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.productName}", null);
            }
            Object assetId = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.assetId}");
            Object assetSerial = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.assetSerial}");
            if(assetId != null){
                incident.setAssetId(Integer.parseInt((String)assetId));
                incident.setAssetSerialNumber((String)assetSerial);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.assetId}", null);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.assetSerial}", null);
            }
            Object categoryId = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.categoryId}");
            Object categoryName = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.categoryName}");
            if(categoryId != null){
                incident.setCategoryId(Integer.parseInt((String)categoryId));
                incident.setCategory((String)categoryName);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.categoryId}", null);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.categoryName}", null);
            }
            Object contactId = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.contactId}");
            Object contactName = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.contactName}");
            if(contactId != null){
                System.out.println("getIncident got contactId "+contactId );
                incident.setContactId(Integer.parseInt((String)contactId));
                incident.setContact((String)contactName);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.contactId}", null);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.contactName}", null);
            }
            Object accountId = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.accountId}");
            Object accountName = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.accountName}");
            if(accountId != null){
                incident.setAssignedId(Integer.parseInt((String)accountId));
                incident.setAssigned((String)accountName);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.accountId}", null);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.accountName}", null);
            }
                        
            cachedIncident = null;
            //initLocation();
            return incident; 
        }

        String id;
        id = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.incidentId}").toString();

        incident = new Incident();
        if(id != null && !id.isEmpty()){
            System.out.println("loadIncident id: " + id);
            Object obj = Util.loadObject("incidents.Incident", "Incidents", id, IncidentAttributes.incidentDetailsAttrs);
            if(obj != null){
                incident = (Incident) obj;
            }
        }
        //initLocation();
        return incident;
    }
    
    public void setIncident(Incident incident) {
        this.incident = incident;
    }
    
    public void cacheEditedIncident(Incident incident) {
        System.out.println("cachingInc");
        this.cachedIncident = incident;
    }
    
    public void updateIncident(Incident incident){
        System.out.println("Required Fields Validation on update incident..");
        
        //Required Fields Validation
        String invalidFields = "";
        if(incident.getSubject() == null 
            || incident.getSubject().replaceAll("\\s","").isEmpty()){
            
            invalidFields = "Subject";
        }
        
        if(incident.getStatusId() == null
            || incident.getStatusId().intValue() == 0){
            
            if(invalidFields != "")
                invalidFields += ", Status";
            else 
                invalidFields += "Status";
               
        }
        
        if(incident.getContactId() == null
            || incident.getContactId().intValue() == 0){
            
            if(invalidFields != "")
                invalidFields += ", Contact";
            else 
                invalidFields += "Contact";
               
        }
        
        if(invalidFields != ""){
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                                                "navigator.notification.alert",
                                                                      new Object[] {"The "+ invalidFields +" cannot be empty!", null, "Alert", "OK"});
            
            return;
        }
        
        //Update Organization when updating Incident's Primary Contact
        if(incident.getContactId() != null)
            incident.setOrganizationId(getOrgIdByContactId(incident.getContactId()));
        
        boolean success = Util.updateObject(incident, "incidents", Integer.toString(incident.getId()),
                             IncidentAttributes.incidentEditAttrs);
        if(success){
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                                                "adf.mf.api.amx.doNavigation",
                                                                new Object[] {"__back"});
        }

    }
    
    public void cancelCreateEditIncident(){
        String cancelMsg = "Are you sure you want to leave this page without saving?";
        AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                                                "discardConfirmation", 
                                                                new Object[] {cancelMsg, ""});
        
    }
    
    private Integer getOrgIdByContactId(Integer contactId){
        Integer orgId = null;
        try {
            String queryString = "/?query=%20select%20Organization.ID%20from%20Contacts%20where%20id=" +contactId;
            
            String QUERY_URL_CONNECTION = (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_URL_CONNECTION}");
            String QUERY_GET_URI = (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_RESULTS_URI}");

            String response = RestAdapter.doGET(QUERY_URL_CONNECTION, QUERY_GET_URI + queryString);
            System.out.println("response: " + response);
            
            if (response != null) {               
                JSONObject jsonObj = new JSONObject(response);
                JSONArray items = jsonObj.getJSONArray("items");
                if(items == null || items.length() == 0)
                    return null;
                JSONObject item = items.getJSONObject(0);
                JSONArray rows = item.getJSONArray("rows");
                if(rows == null || rows.length() == 0)
                    return null;
                JSONArray fields = (JSONArray)rows.get(0);
                if(!fields.isNull(0)){
                    orgId = fields.getInt(0);
                }
            }
        }catch (Exception e) {
            e.printStackTrace();
        }
        return orgId;
    }

    public Incident newIncident() {
        // set the pageFlowScope.ScannerPage for barcode scanner js check
        AdfmfJavaUtilities.setELValue("#{pageFlowScope.ScannerPage}", "AssetQuickSearch");
        if(cachedIncident != null){
            System.out.println("cachedInc");
            incident = new Incident();
            incident = cachedIncident;
            
            Object assetId = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.assetId}");
            Object assetSerial = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.assetSerial}");
            if(assetId != null){
                incident.setAssetId(Integer.parseInt((String)assetId));
                incident.setAssetSerialNumber((String)assetSerial);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.assetId}", null);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.assetSerial}", null);
            }
            Object productId = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.productId}");
            Object productName = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.productName}");
            if(productId != null){
                incident.setProductId(Integer.parseInt((String)productId));
                incident.setProduct((String)productName);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.productId}", null);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.productName}", null);
            }
            Object categoryId = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.categoryId}");
            Object categoryName = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.categoryName}");
            if(categoryId != null){
                incident.setCategoryId(Integer.parseInt((String)categoryId));
                incident.setCategory((String)categoryName);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.categoryId}", null);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.categoryName}", null);
            }
            Object contactId = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.contactId}");
            Object contactName = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.contactName}");
            if(contactId != null){
                incident.setContactId(Integer.parseInt((String)contactId));
                incident.setContact((String)contactName);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.contactId}", null);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.contactName}", null);
            }
            Object accountId = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.accountId}");
            Object accountName = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.accountName}");
            if(accountId != null){
                System.out.println("newIncident got accountId "+accountId );
                incident.setAssignedId(Integer.parseInt((String)accountId));
                incident.setAssigned((String)accountName);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.accountId}", null);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.accountName}", null);
            }
            
            cachedIncident = null;
            return incident; 
        }
        Incident incident = new Incident();
        incident.setStatusId(1);
        
        return incident;
    }

    public void submitNewIncident(Incident incident) {
        //Required Fields Validation
        String invalidFields = "";
        if(incident.getSubject() == null 
            || incident.getSubject().replaceAll("\\s","").isEmpty()){
            
            invalidFields = "Subject";
        }
        
        if(incident.getStatusId() == null
            || incident.getStatusId().intValue() == 0){
            
            if(invalidFields != "")
                invalidFields += ", Status";
            else 
                invalidFields += "Status";
               
        }
        
        if(incident.getContactId() == null
            || incident.getContactId().intValue() == 0){
            
            if(invalidFields != "")
                invalidFields += ", Contact";
            else 
                invalidFields += "Contact";
               
        }
        
        if(invalidFields != ""){
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                                                "navigator.notification.alert",
                                                                      new Object[] {"The "+ invalidFields +" cannot be empty!", null, "Alert", "OK"});
            
            return;
        }
        
        incident.setOrganizationId(getOrgIdByContactId(incident.getContactId()));
        // create incident
       String res = Util.createObject(incident, "incidents", IncidentAttributes.incidentEditAttrs);

        AdfmfContainerUtilities.invokeContainerJavaScriptFunction(
            AdfmfJavaUtilities.getFeatureId(),
            "adf.mf.api.amx.doNavigation",
            new Object[] {"__back"});
    }
    
    public void resetIncident() {
        this.cachedIncident = null;
        AdfmfJavaUtilities.setELValue("#{pageFlowScope.incidentId}", null);
        ValueExpression iter = AdfmfJavaUtilities.getValueExpression("#{bindings.incidentIterator}", Object.class);
        AmxIteratorBinding iteratorBinding = (AmxIteratorBinding)iter.getValue(AdfmfJavaUtilities.getELContext());
        iteratorBinding.getIterator().refresh();
    }
    
    public ListOfValue[] getStatuses() {
            this.statuses = ListOfValueController.getLov("/incidents/statusWithType/status");
            return statuses;
            
        }
    
    public void setStatuses(ListOfValue[] statuses) {
        this.statuses = statuses;
    }
    
    public ListOfValue[] getSeverities() {
            this.severities = ListOfValueController.getLov("/incidents/severity");
            return severities;
            
        }
    
    public void setSeverities(ListOfValue[] severities) {
        this.severities = severities;
    }

}
