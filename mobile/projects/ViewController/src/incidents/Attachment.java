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
 *  date: Tue Aug 23 16:35:57 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: 84bd2fd91e9104e0b37d12e84f735abc15eb7836 $
 * *********************************************************************************************
 *  File: Attachment.java
 * *********************************************************************************************/

package incidents;

import java.math.BigDecimal;

import java.text.ParseException;
import java.text.SimpleDateFormat;

import java.util.Date;

import oracle.adfmf.json.JSONException;
import oracle.adfmf.json.JSONObject;

import rest_adapter.RestAdapter;

import util.Util;

public class Attachment {
    private String _id;

    private String _size;
    private String _fileName;
    private String _url;
    private String _contentType;
    private String _updatedTime;
    private String _createdTime;

    public Attachment () {
        super();
    }

    /**
     * Constr that fills in the details by the given URL
     * @param url
     */
    public Attachment(  String url ) {
        _url = url;
    }

    public void setId(String _id) {
        this._id = _id;
    }

    public String getId() {
        return _id;
    }

    public void setSize(String _size) {
        this._size = _size;
    }

    public String getSize() {
        if (!isLoaded())
            loadAttachment();

        return _size;
    }

    /**
     * You never actually set that one, calculated each time in KB/MB/GB
     * @return
     */
    public String getSizeWithLetter() {
        String sSize = getSize();

        int size = Integer.valueOf(sSize);
        if (size >= 1024) {
            String[] ltrs = { "K", "M", "G", "T", "P", "H", "E"};
            int i;
            for ( i = 1; i <= ltrs.length ; i ++ ) {  // itz 1-N, remember.
                if ((size / (int)Math.pow(1024, i)) <= 1 ) {
                    break;
                }
            }
            if (i >= ltrs.length+1) // upr bnd case
                i = ltrs.length;

            i = i - 1; // we need the biggest whole number giving power (and Ltr index) in all cases.

            float f = (float) size / (float)Math.pow(1024, i);
            f = round(f, 2);
            sSize = Float.toString(f) + " " + ltrs[i-1]+"B";  // ltr is 0-N-1 remember.
        }
        return sSize;
    }

    /**
     * Round to certain number of decimals
     *
     * @param d
     * @param decimalPlace
     * @return
     */
    public static float round(float d, int decimalPlace) {
        BigDecimal bd = new BigDecimal(Float.toString(d));
        bd = bd.setScale(decimalPlace, BigDecimal.ROUND_HALF_UP);
        return bd.floatValue();
    }

    public void setFileName(String _fileName) {
        this._fileName = _fileName;
    }

    public String getFileName() {
        if (!isLoaded())
            loadAttachment();

        return _fileName;
    }

    public void setContentType(String _contentType) {
        this._contentType = _contentType;
    }

    public String getContentType() {
        return _contentType;
    }

    public void setUpdatedTime(String _updatedTime) {
        this._updatedTime = _updatedTime;
    }

    public String getUpdatedTime() {
        return _updatedTime;
    }

    public void setCreatedTime(String _createdTime) {
        this._createdTime = _createdTime;
    }

    public String getCreatedTime() {
        return _createdTime;
    }
    
    /**
     * this function converts specifically for the objects/Date to a Display string per requirements. 
     * IF the internal created Time is *already* in the Required PST .. AND formatted since it comes from the Report FWK, 
     * Then this function is probably not needed. 
     * @return
     */
    public String getCreatedTimeDisplayString() {

        return getCreatedTime();
    }

    private boolean isLoaded() {
        return _id != null;
    }

    private void loadAttachment() {
        // reserve(previous) mechanism to load the ATT Metadata via REST if necessary
        if ( _url != null ) {
            try{
                String restResourceURI = _url;
                System.out.println("uri/loadAttachment obj-> " + restResourceURI);

                String response = RestAdapter.doGET(IncidentAttachmentsController.QUERY_URL_CONNECTION, restResourceURI);
                System.out.println("response/loadAttachment obj JSONlen: " + (response==null?"null":response.length()));

                if (response != null) {
                    JSONObject attAttribs = new JSONObject(response);

                    try {
                        _id = String.valueOf(attAttribs.getInt("id"));
                        _fileName = attAttribs.getString("fileName");
                        _size = String.valueOf(attAttribs.getInt("size"));
                        _contentType = attAttribs.getString("contentType");
                        _createdTime = attAttribs.getString("createdTime");
                        _updatedTime = attAttribs.getString("updatedTime");
                    } catch (JSONException j) {
                        System.out.println("JSONException in attrs in loadAttach: "+ j);
                    }
                }

            }catch (Exception e) {
                System.out.println("Better luck next time from loadAttach: ");
                e.printStackTrace();
            }
        } else {
            System.out.println("Cannot load attachment b/c _url is null: "+_id);
        }
    }

    /**
     *
     * @return
     */
    public String toString() {
        String si = "";
        try {
            si = "Id:" + getId() + "/";
        } catch (Exception e) {
            System.out.println(e.getMessage());
        }
        return si + super.toString();
    }
}
