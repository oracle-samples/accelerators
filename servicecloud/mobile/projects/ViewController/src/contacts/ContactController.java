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

package contacts;

import java.util.ArrayList;

import java.util.HashMap;

import javax.el.ValueExpression;

import lov.ListOfValue;

import lov.ListOfValueController;

import oracle.adfmf.bindings.dbf.AmxIteratorBinding;
import oracle.adfmf.framework.api.AdfmfContainerUtilities;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;

import oracle.adfmf.json.JSONArray;
import oracle.adfmf.json.JSONException;

import oracle.adfmf.json.JSONObject;

import rest_adapter.RestAdapter;

import util.Attribute;
import util.Email;
import util.EmailType;
import util.OsvcResource;
import util.Phone;
import util.PhoneType;
import util.Util;

/**
 * A controller for contacts.
 */
public class ContactController {
    private Contact contact;
    private Contact cachedContact;
    private ListOfValue[] states = null;
    private Contact serverSavedContact;

    public ContactController() {
        super();
    }

    public Contact getContact() {

        if(this.cachedContact != null){
            System.out.println("returning cached Contact");
            this.contact = new Contact();
            this.contact = this.cachedContact;
            
            Object id = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.organizationId}");
            Object name = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.organizationName}");
            if(id != null){
                Integer i = Integer.parseInt((String)id);
                if (i != 0) {
                    contact.setOrgId( i );
                    contact.setOrgName((String)name);
                }
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.organizationId}", null);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.organizationName}", null);
            }

            this.cachedContact = null;
            return this.contact;
        }

        Integer id;
        Object ido = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.contactId}");
        if (ido instanceof String) {
            id = Integer.valueOf((String) ido);
        }
        else {
            id = (Integer) ido;
        }

        if (null != this.contact
            && null != id
            && id == this.contact.getId()) {
            return this.contact;
        }

        this.contact  = new Contact();
        if(id != null && !(0 == id)){
            this.contact = loadContact(id.toString());
        }
        return this.contact;
    }

    public void setContact(Contact aContact) {
        this.contact = aContact;
    }

    /**
     * Gets contact details from the service cloud.
     *
     * @param id - ID of the contact
     * @return a contact object
     */
    public Contact loadContact(String id) {
        Contact contact1 = new Contact();
        this.serverSavedContact  = new Contact();
        Object obj = null;
        try {
            obj = Util.loadObject("contacts.Contact", "Contacts", id, ContactAttributes.detail);
            if (obj!=null) {
                contact1 = (Contact)obj;
                loadPhones(id, contact1);
                loadEmails(id, contact1);
            } else {
                System.out.println("Problem in loading contact id " + id);
            }
        }
        catch (Exception e) {
            System.out.println("Exception loading" + e);
        }
        this.contact = contact1;
        return contact1;
    }

    private void loadPhones(String id, Contact contact){
        String QUERY_URL_CONNECTION = (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_URL_CONNECTION}");    
    
        try{
            System.out.println("load phone");
            String response = RestAdapter.doGET(QUERY_URL_CONNECTION, "queryResults/?query=%20select%20phones.Number,phones.PhoneType.id%20from%20Contacts%20where%20id="+id);
            System.out.println("response: " + response);

            if (response != null) {
                JSONObject jsonObj = new JSONObject(response);
                JSONArray items = jsonObj.getJSONArray("items");
                if(items == null || items.length() == 0)
                    return;
                JSONObject item = items.getJSONObject(0);
                JSONArray rows = item.getJSONArray("rows");
                if(rows == null || rows.length() == 0)
                    return;
                System.out.println("rows:"+rows.length());
                for(int i=0; i<rows.length();i++){
                    JSONArray fields = (JSONArray)rows.get(i);
                    if(fields.getString(1).equals(PhoneType.OFFICE.getString())){
                        String op = fields.getString(0);
                        contact.setOfficePhone(op);
                        serverSavedContact.setOfficePhone(op);
                    }
                    else if(fields.getString(1).equals(PhoneType.MOBILE.getString())){
                        String mp = fields.getString(0);
                        contact.setMobilePhone(mp);
                        serverSavedContact.setMobilePhone(mp);
                    }
                    else if(fields.getString(1).equals(PhoneType.HOME.getString())){
                        String hp = fields.getString(0);
                        contact.setHomePhone(hp);
                        serverSavedContact.setHomePhone(hp);
                    }
                }
            }
        }catch (Exception e) {
                e.printStackTrace();
        }

    }
    
    private void loadEmails(String id, Contact contact){
        String QUERY_URL_CONNECTION = (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_URL_CONNECTION}");    
    
        try{
            System.out.println("load emails");
            String response = RestAdapter.doGET(QUERY_URL_CONNECTION, "queryResults/?query=%20select%20emails.Address,emails.AddressType.id%20from%20Contacts%20where%20id="+id);
            System.out.println("response: " + response);

            if (response != null) {
                JSONObject jsonObj = new JSONObject(response);
                JSONArray items = jsonObj.getJSONArray("items");
                if(items == null || items.length() == 0)
                    return;
                JSONObject item = items.getJSONObject(0);
                JSONArray rows = item.getJSONArray("rows");
                if(rows == null || rows.length() == 0)
                    return;
                System.out.println("rows:"+rows.length());
                for(int i=0; i<rows.length();i++){
                    JSONArray fields = (JSONArray)rows.get(i);
                    System.out.println("0: " + fields.getString(0));
                    System.out.println("1: " + fields.getString(1));
                    if(fields.getString(1).equals(EmailType.PRIMARY.getString())){
                        String em = fields.getString(0);
                        contact.setEmail(em);
                        serverSavedContact.setEmail(em);
                    }
                    else if(fields.getString(1).equals(EmailType.ALT1.getString())){
                        String aem = fields.getString(0);
                        contact.setAlternateEmail(aem);
                        serverSavedContact.setAlternateEmail(aem);
                    }
                }
            }
        }catch (Exception e) {
                e.printStackTrace();
        }

    }
    
    public Contact newContact() {
        System.out.println("newContact called");
        if (this.cachedContact != null) {
            System.out.println("cached Contact");
            contact = cachedContact;
            
            Object id = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.organizationId}");
            Object name = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.organizationName}");
            if(id != null){
                Integer i = Integer.parseInt((String)id);
                if (i != 0) {
                    contact.setOrgId( i );
                    contact.setOrgName((String)name);
                }
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.organizationId}", null);
                AdfmfJavaUtilities.setELValue("#{pageFlowScope.organizationName}", null);
            }

            this.cachedContact = null;
            return this.contact;
        } else {
            Contact aContact = new Contact();
            return aContact;
        }
    }

    /**
     * Creates a new contact in OSvC.
     * @param aContact
     */
    public void submitNewContact(Contact aContact) {
        ArrayList<OsvcResource> res = new ArrayList<OsvcResource>(10);
        res.add(aContact);

        // validation
        ArrayList<String> invalidFields = new ArrayList<String>(20);
        if(Util.isNullOrWhitespace(aContact.getFirstName())) invalidFields.add("First Name cannot be empty");
        if(Util.isNullOrWhitespace(aContact.getLastName())) invalidFields.add("Last Name cannot be empty");

        String hp, op, mp, em, aem;
        hp = aContact.getHomePhone();
        mp = aContact.getMobilePhone();
        op = aContact.getOfficePhone();
        em = aContact.getEmail();
        aem = aContact.getAlternateEmail();

        if (!Util.isNullOrEmpty(hp) && !Util.isValidPhone(hp)) {
            hp = null;
            invalidFields.add("Home phone is not valid");
        }

        if (!Util.isNullOrEmpty(mp) && !Util.isValidPhone(mp)) {
            mp = null;
            invalidFields.add("Mobile phone is not valid");
        }

        if (!Util.isNullOrEmpty(op) && !Util.isValidPhone(op)) {
            op = null;
            invalidFields.add("Office phone is not valid");
        }
        if (!Util.isNullOrEmpty(em) && !Util.isValidEmail(em)) {
            em = null;
            invalidFields.add("Email is not valid");
        }
        if (!Util.isNullOrEmpty(aem) && !Util.isValidEmail(aem)) {
            aem = null;
            invalidFields.add("Alternate email is not valid");
        }

        if(null == aContact.getOrgId() || 0 == aContact.getOrgId()) invalidFields.add("Organization cannot be empty");
        if (invalidFields.size() > 0) {
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(
                AdfmfJavaUtilities.getFeatureId(),
                "navigator.notification.alert",
                new Object[] { Util.join(invalidFields, ", ", null), null, "Alert", "OK" });
            return;
        }

        if (checkEmail(0, em, aem)){
            return;
        }

        if(!Util.isNullOrWhitespace(hp)) res.add(new Phone(hp, "Home Phone"));
        if(!Util.isNullOrWhitespace(mp)) res.add(new Phone(mp, "Mobile Phone"));
        if(!Util.isNullOrWhitespace(op)) res.add(new Phone(op, "Office Phone"));
        if(!Util.isNullOrWhitespace(em)) res.add(new Email(em, "Email - Primary"));
        if(!Util.isNullOrWhitespace(aem)) res.add(new Email(aem, "Alternate Email 1"));

        // create contact
        try {
            Util.createObjects(res);
        }
        catch (Exception e) {
            e.printStackTrace();
        }
        AdfmfContainerUtilities.invokeContainerJavaScriptFunction(
            AdfmfJavaUtilities.getFeatureId(),
            "adf.mf.api.amx.doNavigation",
            new Object[] {"__back"});
    }

    public void cancelWithConfirmation() {
        String cancelMsg = "Are you sure you want to leave this page without saving?";
        AdfmfContainerUtilities.invokeContainerJavaScriptFunction(
            AdfmfJavaUtilities.getFeatureId(),
            "discardConfirmation",
            new Object[] {cancelMsg, ""});
    }
    
    public void cacheEditedContact(Contact currentNewObject) {
        System.out.println("cacheEditedContact called");
        this.cachedContact = currentNewObject;
    }


    /**
     * Updates an existing OSvC contact.
     * @param aContact
     * @throws JSONException
     */
    public void updateContact(Contact aContact) throws JSONException {

        // validation
        ArrayList<String> invalidFields = new ArrayList<String>(20);
        if(Util.isNullOrWhitespace(aContact.getFirstName())) invalidFields.add("First Name cannot be empty");
        if(Util.isNullOrWhitespace(aContact.getLastName())) invalidFields.add("Last Name cannot be empty");
        if(aContact.getOrgId() == null
            || aContact.getOrgId().intValue() == 0) invalidFields.add("Organization cannot be empty");


        String hp, op, mp, em, aem;
        hp = aContact.getHomePhone();
        mp = aContact.getMobilePhone();
        op = aContact.getOfficePhone();
        em = aContact.getEmail();
        aem = aContact.getAlternateEmail();

        if (!Util.isNullOrEmpty(hp) && !Util.isValidPhone(hp)) {
            hp = null;
            invalidFields.add("Home phone is not valid");
        }

        if (!Util.isNullOrEmpty(mp) && !Util.isValidPhone(mp)) {
            mp = null;
            invalidFields.add("Mobile phone is not valid");
        }

        if (!Util.isNullOrEmpty(op) && !Util.isValidPhone(op)) {
            op = null;
            invalidFields.add("Office phone is not valid");
        }
        if (!Util.isNullOrEmpty(em) && !Util.isValidEmail(em)) {
            em = null;
            invalidFields.add("Email is not valid");
        }
        if (!Util.isNullOrEmpty(aem) && !Util.isValidEmail(aem)) {
            aem = null;
            invalidFields.add("Alternate email is not valid");
        }

        if (invalidFields.size() > 0) {
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(
                AdfmfJavaUtilities.getFeatureId(),
                "navigator.notification.alert",
                new Object[] { Util.join(invalidFields, ", ", null), null, "Alert", "OK" });
            return;
        }

        if (checkEmail(aContact.getId(), em, aem)){
            return;
        }
        // create payload
        JSONObject payload = new JSONObject();

        // add name
        JSONObject name = new JSONObject();
        name.put("first", aContact.getFirstName());
        name.put("last", aContact.getLastName());
        payload.put("name", name);

        JSONObject org = new JSONObject();
        org.put("id", aContact.getOrgId());
        payload.put("organization", org);

        if (Util.isNullOrWhitespace(aContact.getTitle())) {
            payload.put("title", JSONObject.NULL);
        }
        else {
            payload.put("title", aContact.getTitle());
        }

        // add address
        JSONObject address = new JSONObject();

        if (Util.isNullOrWhitespace(aContact.getStreet())) {
            address.put("street", JSONObject.NULL);
        }
        else {
            address.put("street", aContact.getStreet());
        }

        if (Util.isNullOrWhitespace(aContact.getCity())) {
            address.put("city", JSONObject.NULL);
        }
        else {
            address.put("city", aContact.getCity());
        }

        if (Util.isNullOrWhitespace(aContact.getZip())) {
            address.put("postalCode", JSONObject.NULL);
        }
        else {
            address.put("postalCode", aContact.getZip());
        }

        if (null == aContact.getStateId() || 0 == aContact.getStateId().intValue()) {
            address.put("stateOrProvince", JSONObject.NULL);
        }
        else {
            JSONObject stateOrProvince = new JSONObject();
            stateOrProvince.put("id", aContact.getStateId());
            address.put("stateOrProvince", stateOrProvince);
        }

        if (null == aContact.getCountryId() || 0 == aContact.getCountryId().intValue()) {
            address.put("country", JSONObject.NULL);
        }
        else {
            JSONObject country = new JSONObject();
            country.put("id", aContact.getCountryId());
            address.put("country", country);
        }

        payload.put("address", address);

        //Update
        RestAdapter.Status suc;
        suc =
            Util.updateObject(payload, Contact.RESOURCE_NAME + "/" + Integer.toString(aContact.getId()));

        boolean success = false;
        if ("200".equals(suc.getStatus())){
            success = true;
        }
        else {
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(
                AdfmfJavaUtilities.getFeatureId(),
                "navigator.notification.alert",
                new Object[] { "Contact could not be updated. Please contact your administrator.",
                               null, "Alert", "OK" });
            return;
        }

        String savedHp = this.serverSavedContact.getHomePhone();
        String savedMp = this.serverSavedContact.getMobilePhone();
        String savedOp = this.serverSavedContact.getOfficePhone();
        String savedEm = this.serverSavedContact.getEmail();
        String savedAem = this.serverSavedContact.getAlternateEmail();

        // update phones and emails
        updatePhonesEmails(new Phone(hp, "Home Phone"), PhoneType.HOME.getString(), aContact, hp, savedHp);
        updatePhonesEmails(new Phone(mp, "Mobile Phone"), PhoneType.MOBILE.getString(), aContact, mp, savedMp);
        updatePhonesEmails(new Phone(op, "Office Phone"), PhoneType.OFFICE.getString(), aContact, op, savedOp);
        updatePhonesEmails(new Email(em, "Email - Primary"), EmailType.PRIMARY.getString(), aContact, em, savedEm);
        updatePhonesEmails(new Email(aem, "Alternate Email 1"), EmailType.ALT1.getString(), aContact, aem, savedAem);

        if (success) {
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities
              .getFeatureId(),
                "adf.mf.api.amx.doNavigation",
                new Object[] {"__back"});
        }
    }

    private void updatePhonesEmails(OsvcResource p, String typeId, Contact aContact, String newString, String savedString){
        if(!Util.isNullOrEmpty(newString) && !Util.isNullOrEmpty(savedString)){
            if(!newString.equals(savedString)){
                Util.updateObject(p
                    ,Contact.RESOURCE_NAME + "/" + aContact.getId() + "/" + p.getResourceName()
                    ,new ArrayList<Attribute>(p.getUpdateAttributes()));
            }
        }else if (!Util.isNullOrEmpty(newString) && Util.isNullOrEmpty(savedString)){
            Util.createObject(p
                ,Contact.RESOURCE_NAME + "/" + aContact.getId() + "/" + p.getResourceName()
                ,new ArrayList<Attribute>(p.getUpdateAttributes()));
        }else if (Util.isNullOrEmpty(newString) && !Util.isNullOrEmpty(savedString)){
            Util.deleteObject(Contact.RESOURCE_NAME + "/" + aContact.getId() + "/" + p.getResourceName() + "/" + typeId);
        }
    }

    public void resetContact() {
        this.contact = null;
        this.cachedContact= null;
        AdfmfJavaUtilities.setELValue("#{pageFlowScope.contactId}", null);
        ValueExpression iter = AdfmfJavaUtilities.getValueExpression("#{bindings.contactIterator}", Object.class);
        AmxIteratorBinding iteratorBinding = (AmxIteratorBinding)iter.getValue(AdfmfJavaUtilities.getELContext());
        iteratorBinding.getIterator().refresh();
    }

    public ListOfValue[] getStates() {
            this.states = ListOfValueController.getLov("/contacts/address/stateOrProvince");
            return this.states;
    }

    private boolean checkEmail(int id, String email, String altEmail){
        boolean isDuplicated = false;
        String msg="Unable to save the contact. ";

        //Check if two emails are same
        if(!Util.isNullOrEmpty(email) && !Util.isNullOrEmpty(altEmail) && email.equalsIgnoreCase(altEmail)){
            msg += "Duplicate email addresses are not allowed.";

            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(
                AdfmfJavaUtilities.getFeatureId(),
                "navigator.notification.alert",
                new Object[] { msg, null, "Alert", "OK" });
            return true;
        }
        
        //Prepare the where clause
        String whereClause = "";
        boolean needOR = false;

        if(!Util.isNullOrEmpty(email)){
            whereClause = "emails.Address=%27" + email + "%27";
            needOR = true;
        }

        if(!Util.isNullOrEmpty(altEmail)){
            if(needOR){
                whereClause += "%20OR%20";
            }
            whereClause += "emails.Address=%27" + altEmail + "%27";
        }

        if(whereClause == ""){
            return isDuplicated;
        }else if(id != 0){
            whereClause = "(" + whereClause + ")%20AND%20id!=" + id;
        }

        String QUERY_URL_CONNECTION = (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_URL_CONNECTION}");

        try{
            String response = RestAdapter.doGET(QUERY_URL_CONNECTION, "queryResults/?query=%20select%20emails.Address%20from%20Contacts%20where%20" + whereClause);

            if (response != null) {
                JSONObject jsonObj = new JSONObject(response);
                JSONArray items = jsonObj.getJSONArray("items");
                if(items == null || items.length() == 0)
                    return isDuplicated;
                JSONObject item = items.getJSONObject(0);
                Integer count = item.getInt("count");
                if(count > 0){
                    JSONArray rows = item.getJSONArray("rows");
                    if(rows == null || rows.length() == 0)
                        return isDuplicated;
                    isDuplicated = true;
                    for(int i=0; i<rows.length();i++){
                        JSONArray fields = (JSONArray)rows.get(i);
                        msg = msg + "Email address " + fields.getString(0) + " already exists. ";
                    }
                    AdfmfContainerUtilities.invokeContainerJavaScriptFunction(
                        AdfmfJavaUtilities.getFeatureId(),
                        "navigator.notification.alert",
                        new Object[] { msg, null, "Alert", "OK" });

                }
            }
        }catch (Exception e) {
                e.printStackTrace();
        }
        return isDuplicated;
    }
}
