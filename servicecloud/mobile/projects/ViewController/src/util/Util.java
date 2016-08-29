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
 *  date: Tue Aug 23 16:36:01 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: 79f8c80805d650b37b53f5d578608f0eb001bc29 $
 * *********************************************************************************************
 *  File: Util.java
 * *********************************************************************************************/

package util;

import java.lang.reflect.Method;

import java.text.ParseException;
import java.text.SimpleDateFormat;

import java.time.DateTimeException;
import java.time.LocalDateTime;
import java.time.ZoneId;
import java.time.ZonedDateTime;
import java.time.format.DateTimeFormatter;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Date;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;

import java.util.Locale;
import java.util.NoSuchElementException;
import java.util.StringTokenizer;
import java.util.TimeZone;

import java.util.regex.Matcher;
import java.util.regex.Pattern;

import oracle.adf.model.datacontrols.device.DeviceManagerFactory;

import oracle.adfmf.framework.ApplicationInformation;
import oracle.adfmf.framework.api.AdfmfContainerUtilities;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;
import oracle.adfmf.json.JSONArray;
import oracle.adfmf.json.JSONException;
import oracle.adfmf.json.JSONObject;

import oracle.sql.DATE;

import rest_adapter.RestAdapter;

/**
 * This class contains utility methods.
 */
public class Util {
    /**
     * del date format to use to print as a PST
     * don't forget to set the timezone to PST after initializing with it
     */

    private static final String NOT_REACHABLE = "NotReachable";
    public final static String QUERY_URL_CONNECTION = (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_URL_CONNECTION}");
    private final static String QUERY_GET_URI = (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_RESULTS_URI}");
    
    public Util() {
        super();
    }

    public static boolean isNetworkReachable() {
        String networkStatus =
            (String) AdfmfJavaUtilities.evaluateELExpression("#{deviceScope.hardware.networkStatus}");

        if(networkStatus.equals(NOT_REACHABLE))
            return false;
        else
            return true;

    }
    
    public static Object loadObject(String className, String tableName, String id, List<Attribute> attrs){
        Object obj = null;
        String queryAttrs = "";
        String queryString = "";
        String[] attrNames = new String [attrs.size()];
        for(int i=0; i<attrs.size(); i++){
            queryAttrs += attrs.get(i).attrPath + ",";
            attrNames[i] = attrs.get(i).attrName;
        }
        if(queryAttrs == ""){
            return obj;
        }else{
            queryString = "/?query=%20select%20" + queryAttrs.substring(0, queryAttrs.length()-1) + "%20from%20" + tableName +"%20where%20id=" +id;
        }
        System.out.println("queryString: " + queryString);

        try {

            String response = RestAdapter.doGET(QUERY_URL_CONNECTION, QUERY_GET_URI + queryString);
            System.out.println("response: " + response);

            if (response != null) {
                JSONObject jsonObj = new JSONObject(response);
                JSONArray items = jsonObj.getJSONArray("items");
                if(items == null || items.length() == 0)
                    return obj;
                JSONObject item = items.getJSONObject(0);
                JSONArray rows = item.getJSONArray("rows");
                if(rows == null || rows.length() == 0)
                    return obj;
                JSONArray fields = (JSONArray)rows.get(0);
                int fieldCount = item.getJSONArray("columnNames").length();
                Class c = Class.forName(className);
                obj = c.newInstance();
                for(int j=0; j<fieldCount; j++){
                    Attribute currentAttr= attrs.get(j);
                    Method method;
                    switch(currentAttr.attrType)
                    {
                    case INT:
                    case MENU:
                        method = obj.getClass().getMethod("set" + currentAttr.attrName, Integer.class);
                        if(!fields.isNull(j)){
                            method.invoke(obj, fields.getInt(j));
                        }
                        break;
                    case STRING:
                        method = obj.getClass().getMethod("set" + currentAttr.attrName, String.class);
                        method.invoke(obj, fields.getString(j));
                        break;
                    case BOOLEAN:
                        method = obj.getClass().getMethod("set" + currentAttr.attrName, Boolean.class);
                        method.invoke(obj, fields.getBoolean(j));
                        break;
                    case DATETIME:
                        String dateStr = fields.getString(j);
                        if (dateStr != null) {  
                            String clientFormat = (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.configuration.client_datetime_format}");
                            DateTimeFormatter localFormat = DateTimeFormatter.ofPattern(clientFormat);
                            ZonedDateTime serverZonedTime = ZonedDateTime.parse(dateStr);
                            
                            TimeZone localTimeZone = TimeZone.getDefault();
                            String localTimeZoneString = localTimeZone.getID();
                            
                            String tzNoColon00 = null;
                            // Windows 10 format has :00 at the end, need to remove 
                            if (localTimeZoneString.contains(":")) {
                                tzNoColon00 = localTimeZoneString.split(":")[0];
                                localTimeZoneString = tzNoColon00;
                            }                         
                            ZoneId localZone = ZoneId.of(localTimeZoneString);                             
                            ZonedDateTime localZonedTime = serverZonedTime.withZoneSameInstant(localZone);
                            // for setting the "String" date attribute
                            String methodName = "set" + currentAttr.attrName + "String";
                            method = obj.getClass().getMethod(methodName, String.class);
                            method.invoke(obj, localZonedTime.format(localFormat));
                            // for setting the "Date" attribute, need to use old Date to match the Date type
                            SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ssX", Locale.ENGLISH);  
                            Date dateObj = dateFormat.parse(dateStr);
                            method = obj.getClass().getMethod("set" + currentAttr.attrName, Date.class);
                            method.invoke(obj, dateObj);
                        }
                        break;
                    case DATE:
                        String dateOnlyStr = fields.getString(j);
                        if (dateOnlyStr != null) { 
                            String clientDateOnlyFormat = (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.configuration.client_dateOnly_format}");
                            SimpleDateFormat clientDateFormat = new SimpleDateFormat(clientDateOnlyFormat);
                            SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd");  
                            Date dateObj = dateFormat.parse(dateOnlyStr);   
                            method = obj.getClass().getMethod("set" + currentAttr.attrName, Date.class);
                            method.invoke(obj, dateObj);
                            String methodName = "set" + currentAttr.attrName + "String";
                            method = obj.getClass().getMethod(methodName, String.class);
                            method.invoke(obj, clientDateFormat.format(dateObj));
                        }
                        break;
                    case LONG:
                        method = obj.getClass().getMethod("set" + currentAttr.attrName, Long.class);
                        method.invoke(obj, fields.getLong(j));
                        break;
                    case DECIMAL:
                        method = obj.getClass().getMethod("set" + currentAttr.attrName, Double.class);
                        method.invoke(obj, fields.getDouble(j));
                        break;
                    default:
                        method = obj.getClass().getMethod("set" + currentAttr.attrName, String.class);
                        method.invoke(obj, fields.getString(j));
                        break;
                    }
                }
            }
        }catch (Exception e) {
            e.printStackTrace();
        }
        return obj;
    }
    
    /**
     * More flexible function, you can specify the whole resource Path.  Just make sure you spec the ID in it.
     * @param updatedObj
     * @param resourceFullURI  incidents/6/attachments
     * @param attrs
     * @return RestAdapter.Status
     */
    public static RestAdapter.Status updateObject(Object updatedObj, String resourceFullURI, List<Attribute> attrs) {
        JSONObject updatePayload = Util.getFinalPayload(updatedObj, attrs);
        RestAdapter.Status statusObj;

        try {
            statusObj = RestAdapter.doPATCH(QUERY_URL_CONNECTION, resourceFullURI, updatePayload.toString());
            String betterMsg;
            try {
                JSONObject jo = new JSONObject(statusObj.getMessage());
                betterMsg = jo.getString("detail");
            } catch (JSONException e) {
                // nothing
                betterMsg =  "Server Error";
            }
            statusObj = new RestAdapter.Status(statusObj.getStatus(), betterMsg);
        } catch (Exception e) {
            e.printStackTrace();
            String errMsg;
            if (e.getCause()!=null)
                errMsg = e.getCause().getMessage();
            else
                errMsg = "Generic";
            statusObj = new RestAdapter.Status("400", errMsg);
        }
        return statusObj;
    }

    public static RestAdapter.Status updateObject(JSONObject updatePayload, String resourceFullURI) {
        RestAdapter.Status statusObj;

        try {
            statusObj = RestAdapter.doPATCH(QUERY_URL_CONNECTION, resourceFullURI, updatePayload.toString());
            String betterMsg;
            try {
                JSONObject jo = new JSONObject(statusObj.getMessage());
                betterMsg = jo.getString("detail");
            } catch (JSONException e) {
                // nothing
                betterMsg =  "Server Error";
            }
            statusObj = new RestAdapter.Status(statusObj.getStatus(), betterMsg);
        } catch (Exception e) {
            e.printStackTrace();
            String errMsg;
            if (e.getCause()!=null)
                errMsg = e.getCause().getMessage();
            else
                errMsg = "Generic";
            statusObj = new RestAdapter.Status("400", errMsg);
        }
        return statusObj;
    }

    /**
     * Update top lvl object, URL will be 'resourceName/id'
     *
     * @param updatedObj
     * @param resourceName
     * @param id
     * @param attrs
     * @return
     */
    public static boolean updateObject(Object updatedObj, String resourceName, String id, List<Attribute> attrs) {
        RestAdapter.Status statO = updateObject(updatedObj, resourceName + "/" + id, attrs);

        return statO!=null && "200".equals(statO.getStatus());
    }

    /**
     * Posts multiple new resources to OSvC using the REST API.
     * First object in the collection must be the primary resource such as contacts
     * The remaining objects in the collection must be the children of the primary resource
     * @param theResources is a collection of OsvcResource
     * @throws NoSuchElementException
     * @throws JSONException
     */
    public static void createObjects(Collection<OsvcResource> theResources)
        throws NoSuchElementException, JSONException {
        Iterator<OsvcResource> ri = theResources.iterator();
        OsvcResource pr = ri.next();
        String s = Util.createObject(
            pr,
            pr.getResourceName(),
            new ArrayList<Attribute>(pr.getCreateAttributes()));
        JSONObject prjo = new JSONObject(s);
        Integer id = (Integer)prjo.get("id");
        if (null == id || JSONObject.NULL == id || 0 == id) return;
        while (ri.hasNext()) {
            OsvcResource cr = ri.next();
            Util.createObject(
                cr,
                new StringBuffer(100)
                .append("/")
                .append(pr.getResourceName())
                .append("/")
                .append(id)
                .append("/")
                .append(cr.getResourceName())
                .toString(),
                new ArrayList<Attribute>(cr.getCreateAttributes()));
        }
    }

    public static String deleteObject(String resourceFullURI) {
        String result = "{}";
        try {
            String res = RestAdapter.doDELETE(QUERY_URL_CONNECTION, resourceFullURI);
            result = res;
        }
        catch (Exception e) {
            e.printStackTrace();
        }
        return result;
    }
        
    /**
     * Posts a new instance of the resource to OSvC using the REST API.
     * @param obj is a Java Bean that contains the object
     * @param resourceName is the name of the resource such as "incidents"
     * @param attrs is a list of attributes that will be sent to the server
     */
    public static String createObject(Object obj, String resourceName, List<Attribute> attrs) {
        JSONObject payload = Util.getFinalPayload(obj, attrs);
        String result = "{}";
        try {
            String res = RestAdapter.doPOST(QUERY_URL_CONNECTION, resourceName, payload.toString());
            result = res;
        }
        catch (Exception e) {
            e.printStackTrace();
        }
        return result;
    }

    private static JSONObject getFinalPayload(Object obj, List<Attribute> attrs) {
        JSONObject result = new JSONObject();
        try {
            for(int i=0; i<attrs.size(); i++) {
                Method method = obj.getClass().getMethod("get" + attrs.get(i).attrName);
                Object updatedVal = method.invoke(obj);
                Object updatedValStr = ConvertUpdatedVal(attrs.get(i).attrType, updatedVal);
                String path = attrs.get(i).attrPath;
                //Special case for menu type, if value is null
                if(attrs.get(i).attrType == AttrType.MENU && (updatedValStr == null || updatedValStr.equals(0))&& path.substring(path.length()-3).equals(".id")) {
                    path = path.substring(0, path.length()-3);
                    /*
                     * "statusWithType":{"status":null} has error (Asset.StatusWithType.Status):
                     * A problem setting a property to NULL was encountered: Not Allowed: Cannot be set to Nil/NULL; 
                     * Hard code this case so that existing other cases won't break
                     */
                    if (path.equals("statusWithType.status"))
                    {
                        path = "statusWithType"; 
                        updatedValStr = null;
                    }
                }
                System.out.println(path);
                
                String[] splitPath = path.split("\\.");
                result = getPayload(result, splitPath, 0, updatedValStr);
            }
            System.out.println(result);
        } catch(Exception e) {
            e.printStackTrace();
        }
        return result;
    }

    private static JSONObject getPayload(JSONObject currentMap, String[] splitPath, int currentLevel, Object val){
        JSONObject newMap = new JSONObject ();
        try{
            if(currentMap != null){
                newMap = currentMap;
            }
            if(currentLevel == splitPath.length-1){
                if(val == null){
                    val = JSONObject.NULL;
                }
                newMap.put(splitPath[currentLevel], val);
            }
            else if(currentMap != null && currentMap.has(splitPath[currentLevel])){
                newMap = currentMap;
                Object existedSubObj = currentMap.get(splitPath[currentLevel]);
                if(existedSubObj instanceof JSONObject){
                    JSONObject existedSubMap = (JSONObject) existedSubObj;
                    JSONObject newSubMap = getPayload(null, splitPath, currentLevel+1, val);
                    JSONArray mergedSubMap = new JSONArray();
                    mergedSubMap.put(existedSubMap);
                    mergedSubMap.put(newSubMap);
                    newMap.put(splitPath[currentLevel], mergedSubMap);
                }else if(existedSubObj instanceof JSONArray){
                    JSONArray existedSubMap = (JSONArray) existedSubObj;
                    JSONObject newSubMap = getPayload(null, splitPath, currentLevel+1, val);
                    JSONArray mergedSubMap = existedSubMap;
                    mergedSubMap.put(newSubMap);
                    newMap.put(splitPath[currentLevel], mergedSubMap);
                }else{
                    JSONObject subMap = getPayload(null, splitPath, currentLevel+1, val);
                    System.out.println(subMap);
                    newMap.put(splitPath[currentLevel], subMap);
                }

            }else{
                JSONObject subMap = getPayload(null, splitPath, currentLevel+1, val);
                newMap.put(splitPath[currentLevel], subMap);
            }
        }catch (Exception e) {
            e.printStackTrace();
        }

        return newMap;
    }

    private static Object ConvertUpdatedVal(AttrType attrType, Object val){
        Object convertedVal = null;
        if(val == null){
            return null;
        }
        switch(attrType)
        {
        case INT:
        case LONG:
        case DECIMAL:
        case MENU:
            convertedVal = val;
            break;
        case DATETIME:
            SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss'Z'");
            dateFormat.setTimeZone(TimeZone.getTimeZone("GMT"));  // you forgot dis  ^^^
            Date date = (Date)val;
            //
            System.out.println("Some call to convertUpdatedVal with date: "+date);
            convertedVal = dateFormat.format(date);
            break;
        case DATE:
            Date dateOnly = (Date)val;
            SimpleDateFormat dateOnlyFormat = new SimpleDateFormat("yyyy-MM-dd");
            convertedVal = dateOnlyFormat.format(dateOnly);
            break;
        default:
            convertedVal = String.valueOf(val);
            break;
        }
        return convertedVal;
    }

    /**
     * safe string
     * @param s
     * @return
     */
    public static String getSafeString(String s){
        return s==null?"":s;
    }

    /**
     *
     * @param attributeName e.g. "attr2"
     * @param value
     * @return
     */
    public static String attributeToDateString(String attributeName, String value) {
        if (value!=null) {
            // checking if it it's a date; change the Pattern if the Report Format changes.
            Pattern pattern = Pattern.compile("'([\\d :\\-]+)'");  // gettz
            Matcher matcher = pattern.matcher( value );
            if (matcher.find()) {
                value = matcher.group(1);
                // System.out.println("Got report "+attributeName+" date: "+value);
            } else {
                return value; // exit
            }

            /* REST 1.3 Report DateTime field in this format (and not in UTC):
             * "yyyy-MM-dd HH:mm:ss", eg: "2016-05-17 08:05:00"
             * The Server Time Zone is not specified (missing), so, get from Config verb 
             * 
             */
            
            DateTimeFormatter reportFormat = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss");
            String clientFormat = (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.configuration.client_datetime_format}");
            DateTimeFormatter localFormat = DateTimeFormatter.ofPattern(clientFormat);
            
            LocalDateTime serverTime = LocalDateTime.parse(value, reportFormat);
            String serverTimeZone = (String)AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.configuration.server_timezone}");
           
            try {
                ZoneId serverZone = ZoneId.of(serverTimeZone); 
                ZonedDateTime serverZonedTime = ZonedDateTime.of(serverTime, serverZone);

                TimeZone localTimeZone = TimeZone.getDefault();
                String localTimeZoneString = localTimeZone.getID();
                
                String tzNoColon00 = null;
                // Windows 10 format has :00 at the end, need to remove 
                if (localTimeZoneString.contains(":")) {
                    tzNoColon00 = localTimeZoneString.split(":")[0];
                    localTimeZoneString = tzNoColon00;
                }              
                ZoneId localZone = ZoneId.of(localTimeZoneString); 
                ZonedDateTime localZonedTime = serverZonedTime.withZoneSameInstant(localZone);
           
                value = localZonedTime.format(localFormat);
                System.out.println("serverZonedTime: " + serverZonedTime.format(reportFormat) + " " + serverZone);
                System.out.println("localZonedTime: " + value + " " + localZone);

            } catch (DateTimeException e) {
                System.out.println("DateTimeException while formatting date: "+e.getMessage());
                AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                            "navigator.notification.alert",
                            new Object[] {e.getMessage(), null, "Error", "OK"});
                
                throw e;
            }
        }
        return value;
    }

    /**
     * Checks whether the string is null or empty.
     *
     * @param aString
     * @return true if the string is null or empty
     */
    public static boolean isNullOrEmpty(String aString) {
        if (null == aString) return true;
        if (aString.isEmpty()) return true;
        return false;
    }

    public static String escapeQuotesInQueryStmt( String originalString){
        String editedString = "";
        boolean isEndWithSingleQuote = originalString.endsWith("'");
        if(originalString != null && !originalString.equals("")){
            StringTokenizer st = new StringTokenizer(originalString, "'");
            int count = st.countTokens();
            String stmtArray[] = new String[count];
            for(int i =0; i < count; i++){
                stmtArray[i] = st.nextToken();
                editedString +=  stmtArray[i] + "\'\'";
            }
            if(!isEndWithSingleQuote)
                editedString = editedString.substring(0, editedString.length()-2);
        }else{
            editedString = originalString;
        }
        return editedString;
    }

    /**
     * Checks whether the string is null or whitespace.
     *
     * @param aString
     * @return true if the string is null or whitespace
     */
    public static boolean isNullOrWhitespace(String aString) {
        if (null == aString) return true;
        if (aString.trim().isEmpty()) return true;
        return false;
    }

    /**
     * Returns a concatenated string.
     *
     * @param aList of Strings to be joined
     * @param aDelimiter is empty by default
     * @param aSuffix will be appended at the end of the string. It's empty by default
     * @return String
     */
    public static String join(Collection<String> aList, String aDelimiter, String aSuffix) {
        String dl = "";
        StringBuffer result = new StringBuffer(512);
        for (String s : aList) {
            result.append(dl).append(s);
            dl = Util.isNullOrEmpty(aDelimiter) ? "" : aDelimiter;
        }
        aSuffix = Util.isNullOrEmpty(aSuffix) ? "" : aSuffix;
        return result.append(aSuffix).toString();
    }

    private static Pattern VALID_PHONE_PATTERN = Pattern.compile("[0-9]+");
    public static boolean isValidPhone(String aPhone) {
        if (null == aPhone) return false;
        String result = aPhone.trim();
        result = result.replace("-", "");
        if (Util.isNullOrWhitespace(aPhone)) return false;
        Matcher m = Util.VALID_PHONE_PATTERN.matcher(result);
        return m.matches();
    }

    private static Pattern VALID_EMAIL_ADDRESS_PATTERN =
        Pattern.compile("^[A-Z0-9._]+@[A-Z0-9.]+\\.[A-Z]{2,7}$", Pattern.CASE_INSENSITIVE);
    public static boolean isValidEmail(String anEmail) {
        if (Util.isNullOrWhitespace(anEmail)) return false;
        Matcher m = Util.VALID_EMAIL_ADDRESS_PATTERN.matcher(anEmail);
        return m.matches();
    }
    
    public static void saveRegisterId(String token) {
        String tokenType = "gcm_token";
        String deviceOS = DeviceManagerFactory.getDeviceManager().getOs();
        String deviceName = DeviceManagerFactory.getDeviceManager().getName();
        // due to MAF defect, geolocation does not work on real android devices or iOS devices
        // so, we only enable the position detecting in iOS Simulator
        if (deviceOS.contains("iOS") && !deviceName.contains("Simulator")) {
            tokenType = "apns_token";
        }

        String userName = (String) AdfmfJavaUtilities.evaluateELExpression("#{securityContext.userName}");
        String QUERY_GET_URI =
            (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_RESULTS_URI}");

        String query =
            QUERY_GET_URI + "/?query=%20select%20id%20from%20accounts%20where%20login%20=%20%27" + userName + "%27";
        String getResponse = RestAdapter.doGET(QUERY_URL_CONNECTION, query);
        String acct_id = "";
        try {
            if (getResponse != null) {
                System.out.println("get response: " + getResponse);
                JSONObject jsonObj = new JSONObject(getResponse);
                JSONArray items = jsonObj.getJSONArray("items");

                if (items != null && items.length() == 0)
                    return;

                JSONObject item = items.getJSONObject(0);
                JSONArray rows = item.getJSONArray("rows");

                if (rows == null || rows.length() == 0)
                    return;
                JSONArray fields = (JSONArray) rows.get(0);
                acct_id = fields.getString(0);
            }
            System.out.println("account: " + acct_id);
            if (acct_id != null && !acct_id.isEmpty()) {
                JSONObject gcmCustomField = new JSONObject();
                gcmCustomField.put(tokenType, token);
                JSONObject mobileCustomFieldPkg = new JSONObject();
                mobileCustomFieldPkg.put("Mobile", gcmCustomField);
                JSONObject customFields = new JSONObject();
                customFields.put("customFields", mobileCustomFieldPkg);
                JSONArray updatePayload = new JSONArray();
                updatePayload.put(customFields);
                RestAdapter.doPATCH(QUERY_URL_CONNECTION, "accounts/" + acct_id, updatePayload.toString());
            }
        } catch (Exception e) {
            e.printStackTrace();
        }

    }
    
    public static void MCSRegistration(String token, String oracleMobileBackendId, String basicAuth){
        try {
            JSONObject payload = new JSONObject();
            
            ApplicationInformation ai = AdfmfContainerUtilities.getApplicationInformation();
            
            JSONObject mobileClient = new JSONObject();
            String deviceOS = DeviceManagerFactory.getDeviceManager().getOs();
            String platformName = "Android";
            if (deviceOS.contains("iOS")) {
                platformName = "IOS";
            }
            mobileClient.put("id", ai.getId());
            mobileClient.put("version", ai.getVersion());
            mobileClient.put("platform", platformName);
            
            payload.put("notificationToken", token);
            payload.put("mobileClient", mobileClient);
            
            HashMap<String, String> properties = new HashMap<String, String>();
            properties.put("Oracle-Mobile-Backend-ID", oracleMobileBackendId);
            properties.put("Authorization", "Basic "+basicAuth);
            properties.put("Content-Type", "application/json");
            
            RestAdapter.doGeneralHttp("MCSServerConnection", "/mobile/platform/devices/register", payload.toString(), "POST", properties);
            
        } catch (Exception e) {
            e.printStackTrace();
        }
        
    }
    
    public static String getUserDisplayName() {
        
        String userName = (String) AdfmfJavaUtilities.evaluateELExpression("#{securityContext.userName}");
        String QUERY_GET_URI =
            (String) AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.QUERY_RESULTS_URI}");

        String query =
            QUERY_GET_URI + "/?query=%20select%20Name.First,Name.Last%20from%20accounts%20where%20login%20=%20%27" + userName + "%27";
        String getResponse = RestAdapter.doGET(QUERY_URL_CONNECTION, query);
        String acct_name = "";
        try {
            if (getResponse != null) {
                System.out.println("get response: " + getResponse);
                JSONObject jsonObj = new JSONObject(getResponse);
                JSONArray items = jsonObj.getJSONArray("items");

                if (items != null && items.length() == 0)
                    return acct_name;

                JSONObject item = items.getJSONObject(0);
                JSONArray rows = item.getJSONArray("rows");

                if (rows == null || rows.length() == 0)
                    return acct_name;
                JSONArray fields = (JSONArray) rows.get(0);
                String acct_first_name = fields.getString(0);
                String acct_last_name = fields.getString(1);
                acct_name = acct_first_name + " " + acct_last_name;
            }
            System.out.println("account: " + acct_name);
            
        } catch (Exception e) {
            e.printStackTrace();
        }
        return acct_name;

    }

}
