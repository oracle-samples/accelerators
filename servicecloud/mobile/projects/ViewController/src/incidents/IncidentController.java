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

package incidents;

import java.text.SimpleDateFormat;

import java.util.HashMap;

import javax.el.ValueExpression;

import lov.ListOfValue;
import lov.ListOfValueController;

import oracle.adf.model.datacontrols.device.DeviceManagerFactory;
import oracle.adf.model.datacontrols.device.Location;

import oracle.adfmf.bindings.dbf.AmxIteratorBinding;
import oracle.adfmf.framework.api.AdfmfContainerUtilities;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;

import oracle.adfmf.json.JSONArray;
import oracle.adfmf.json.JSONException;
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
            initLocation();
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
        initLocation();
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
        AdfmfJavaUtilities.setELValue("#{pageFlowScope.saveLocation}", false);
        isLocationAvailable();
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
       
        // add gps location
       try {
           addGeoLocation(res);
        }
        catch (Exception e) {
            if (null != e) {
                System.out.println("Failed to update location " + e.getMessage());
            }
        }

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

    private Location _location;

    /**
     * Find the current geo location.
     * @return true if it finds a location
     */
    public boolean isLocationAvailable() {
        boolean result = false;
        try {
//            String deviceOS = DeviceManagerFactory.getDeviceManager().getOs();
//            String deviceName = DeviceManagerFactory.getDeviceManager().getName();
            // due to MAF defect, geolocation does not work on real android devices or iOS devices
            // so, we only enable the position detecting in iOS Simulator
//            if (deviceOS.contains("iOS") && deviceName.contains("Simulator")) {
                this._location = DeviceManagerFactory.getDeviceManager().getCurrentPosition(1000, 5000, true);
                result = true;
                System.out.println("Current location is " + this._location.getLatitude() + "," + this._location.getLongitude());
//            } else {
//                result = false;
//            }
        }
        catch (Exception e) {
            result = false;
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.saveLocation}", false);
            System.out.println("Failed to obtain a geolocation " + e.getMessage());
        }
        return result;
    }

    public boolean initLocation() {
        if (!this.isLocationAvailable()) return false;
        if (null != this.incident) {
            this.incident.setGpsLatitudeCurrent(this._location.getLatitude());
            this.incident.setGpsLongitudeCurrent(this._location.getLongitude());
        }
        if (null != this.cachedIncident) {
            this.cachedIncident.setGpsLatitudeCurrent(this._location.getLatitude());
            this.cachedIncident.setGpsLongitudeCurrent(this._location.getLongitude());
        }
        return true;
    }

    private void addGeoLocation(String res) {
        boolean sl = false;
        try {
            Object o = AdfmfJavaUtilities.getELValue("#{pageFlowScope.saveLocation}");
            if (null != o && o instanceof Boolean) {
                sl = ((Boolean)o).booleanValue();
            }
        }
        catch (Exception e) {
            System.out.println("Failed to read the property saveLocation " + e.getMessage());
            return;
        }

        if (!sl) return;

        JSONObject prjo;
        Integer id;
        try {
            prjo = new JSONObject(res);
            id = (Integer) prjo.get("id");
        } catch (JSONException e) {
            System.out.println("Cannot parse incident id");
            return;
        }
        if (null == id || JSONObject.NULL == id || 0 == id) return;

        // Get the location
        Location loc = null;

        loc = null == loc ? this._location : null;

        if (null == loc) {
            return;
        }
        double latd = loc.getLatitude();
        double lgtd = loc.getLongitude();

        // Create payload
        JSONObject payload = new JSONObject();
        JSONObject customFields = new JSONObject();
        JSONObject Mobile = new JSONObject();
        try {
            payload.put("customFields", customFields);
            customFields.put("Mobile", Mobile);
            Mobile.put("gps_latitude", 0 == latd ? JSONObject.NULL : String.valueOf(latd));
            Mobile.put("gps_longitude", 0 == lgtd ? JSONObject.NULL : String.valueOf(lgtd));
        } catch (JSONException e) {
            e.printStackTrace();
        }

        // Update
        RestAdapter.Status suc;
        suc = Util.updateObject(payload, "incidents/" + Integer.toString(id));
        boolean success = false;
        if ("200".equals(suc.getStatus())){
            success = true;
        }
        else {
            success = false;
        }
        if (success) {
            System.out.println("Added location to incident " + id + " location " + Mobile.toString());
        }
    }

    public void removeGeoLocation() {

        // Create payload
        JSONObject payload = new JSONObject();
        JSONObject customFields = new JSONObject();
        JSONObject Mobile = new JSONObject();
        try {
            payload.put("customFields", customFields);
            customFields.put("Mobile", Mobile);
            Mobile.put("gps_latitude", JSONObject.NULL);
            Mobile.put("gps_longitude", JSONObject.NULL);
        } catch (JSONException e) {
            e.printStackTrace();
        }

        // Remove location
        RestAdapter.Status suc;
        suc = Util.updateObject(payload, "incidents/" + Integer.toString(this.incident.getId()));
        boolean success = false;
        if ("200".equals(suc.getStatus())){
            success = true;
        }
        else {
            success = false;
        }
        if (success) {
            System.out.println("Removed location from incident " + Integer.toString(this.incident.getId()));
        }
    }

    public void addCurrentGeoLocation() {
        // Get the location
        Location loc = null;

        loc = null == loc ? this._location : null;

        if (null == loc) {
            return;
        }
        double latd = loc.getLatitude();
        double lgtd = loc.getLongitude();

        // Create payload
        JSONObject payload = new JSONObject();
        JSONObject customFields = new JSONObject();
        JSONObject Mobile = new JSONObject();
        try {
            payload.put("customFields", customFields);
            customFields.put("Mobile", Mobile);
            Mobile.put("gps_latitude", 0 == latd ? JSONObject.NULL : String.valueOf(latd));
            Mobile.put("gps_longitude", 0 == lgtd ? JSONObject.NULL : String.valueOf(lgtd));
        } catch (JSONException e) {
            e.printStackTrace();
        }

        // Add location
        RestAdapter.Status suc;
        suc = Util.updateObject(payload, "incidents/" + Integer.toString(this.incident.getId()));
        boolean success = false;
        if ("200".equals(suc.getStatus())){
            success = true;
        }
        else {
            success = false;
        }
        if (success) {
            System.out.println("Added location to incident " + Integer.toString(this.incident.getId()));
        }
    }

}
