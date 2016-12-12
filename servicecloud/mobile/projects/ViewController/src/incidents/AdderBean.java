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
 *  date: Tue Aug 23 16:35:57 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: a18c0e011f33632ace2be5f44b5cb24235617e6c $
 * *********************************************************************************************
 *  File: AdderBean.java
 * *********************************************************************************************/

package incidents;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.RandomAccessFile;

import java.text.SimpleDateFormat;

import java.util.Base64;

import java.util.Date;

import oracle.adf.model.datacontrols.device.DeviceManager;
import oracle.adf.model.datacontrols.device.DeviceManagerFactory;

import oracle.adfmf.framework.api.AdfmfContainerUtilities;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;
import oracle.adfmf.java.beans.PropertyChangeListener;
import oracle.adfmf.java.beans.PropertyChangeSupport;

import rest_adapter.RestAdapter;

import util.Util;

public class AdderBean {
    private String attachmentName = "file1.txt";
    private String attNameFileDisplayed;
    private transient PropertyChangeSupport propertyChangeSupport = new PropertyChangeSupport(this);
    protected String pictureData="";
    private String isPictureDataSet = "0";

    private int quality = 25;

    // FILE is prob 1, 0 is DATA
    private int destination = DeviceManager.CAMERA_DESTINATIONTYPE_FILE_URI;
    private int source = DeviceManager.CAMERA_SOURCETYPE_PHOTOLIBRARY;
    private boolean allowEdit = false;
    private int encoding = DeviceManager.CAMERA_ENCODINGTYPE_JPEG;
    private int targetWidth = 300;
    private int targetHeight= 300;
    private String publishImgFn = "S-Pen-" + (int) (Math.random() * 10000) + 1  + ".jpg";
    private Integer sizeSlider = 0; // 0 small 1 medium 2 lartge.

    public AdderBean() {
        super();
    }

    public void setSizeSlider(Integer sizeSlider) {
        this.sizeSlider = sizeSlider;
    }

    public Integer getSizeSlider() {
        return sizeSlider;
    }

    public void setSource(int source) {
        int oldSource = this.source;
        this.source = source;
        propertyChangeSupport.firePropertyChange("source", oldSource, source);
    }

    public int getSource() {
        return source;
    }

    public void setPictureData(String pictureData) {
        String oldPictureData = this.pictureData;
        this.pictureData = pictureData;
        propertyChangeSupport.firePropertyChange("pictureData", oldPictureData, pictureData);
    }

    public String getPictureData() {
        return pictureData;
    }

    public void setIsPictureDataSet(String isPictureDataSet) {
        String oldIs = this.isPictureDataSet;
        this.isPictureDataSet = isPictureDataSet;
        propertyChangeSupport.firePropertyChange("isPictureDataSet", oldIs, isPictureDataSet);
    }

    public String getIsPictureDataSet() {
        return isPictureDataSet;
    }


    public void addPropertyChangeListener(PropertyChangeListener l) {
        propertyChangeSupport.addPropertyChangeListener(l);
    }

    public void removePropertyChangeListener(PropertyChangeListener l) {
        propertyChangeSupport.removePropertyChangeListener(l);
    }

    /**
     * names selected already
     * @param attNameFileDisplayed
     */
    public void setAttNameFileDisplayed(String attNameFileDisplayed) {
        String oldNameFile = this.attNameFileDisplayed;
        this.attNameFileDisplayed = attNameFileDisplayed;
        propertyChangeSupport.firePropertyChange("attNameFileDisplayed", oldNameFile, attNameFileDisplayed);
    }

    public String getAttNameFileDisplayed() {
        return attNameFileDisplayed;
    }

    /**
     * current list of names
     * @param attachmentName
     */
    public void setAttachmentName(String attachmentName) {
        String oldName = this.attachmentName;
        this.attachmentName = attachmentName;
        propertyChangeSupport.firePropertyChange("attachmentName", oldName, attachmentName);
    }

    public String getAttachmentName() {
        return attachmentName;
    }


    public void setDestination(int destination) {
        this.destination = destination;
    }

    /**
     *
     * @return -1 on not set, 1 on set in temp file, 0 on set in data.
     */
    public int getDestination() {
//        if ("0".equals(getIsPictureDataSet()) )
//            return -1;
//        else
            return destination;
    }

    public void addFileNameToList() {
        String dir = AdfmfJavaUtilities.getDirectoryPathRoot(AdfmfJavaUtilities.DownloadDirectory);
        String path = "file://" + dir + "/" + attachmentName;
        System.out.println("Directory and file is : "+path);

        // Some encoding is necessary on the URL so it doesn't have spaces
        // replace " " with "%20"
        StringBuffer buffer = new StringBuffer();
        String replacedString = " ";
        String replacement = "%20";
        int index = 0, previousIndex = 0;
        index = path.indexOf(replacedString, index);
        while (index != -1) {
            buffer.append(path.substring(previousIndex, index)).append(replacement);
            previousIndex = index + 1;
            index = path.indexOf(replacedString, index + replacedString.length());
        }
        buffer.append(path.substring(previousIndex, path.length()));

        setAttNameFileDisplayed(attachmentName);// buffer.toString());

        // TODO  l.o.f.
        try {
            File file = new File("/"+dir+"/");
            File[] files = file.listFiles();

            if (files != null)
                for (int i = 0; i < files.length; i++) {
                    if (!files[i].isDirectory() )
                        System.out.println("File found3: "+files[i].toString());
                }
        } catch (Exception e) {
            System.err.println("Exception doing file list : " + e);
        }
    }

    /**
     *
     * @param incident  with inc data or ID
     * @param isPictureDataSet flag, if pic set; bool or string.
     * @param pictureData - IMG filename SIMPLY. Or data if we use another process.
     * @param attachmentName - attachment FILE name, if it's a file.
     */
    public void uploadAttachment(Incident incident, boolean isPictureDataSet, String pictureData, String attachmentName) {
        System.out.println("uploadAttachment called..");
        System.out.println("uploadAttachment incident .."+incident.getId() + " isPicDataSet: "+isPictureDataSet );
        System.out.println("uploadAttachment file pic .."+pictureData + " fileIfFileName "+attachmentName );
        if ("".equals(pictureData) || pictureData == null || !"1".equals(getIsPictureDataSet()) ) {  
            // TODO check attachmentName too, l8r.. huh?
            System.out.println("uploadAttachment cancelling due to name/not set" );
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                    "navigator.notification.alert",
                                    new Object[] {"Please select a pic (filename!)", null, "Alert", "OK"});
            return;
        }

        File file = new File(pictureData);
        String fileName = null;
        RandomAccessFile raf = null;
        byte[] bytes = new byte[0];
        boolean success = false;
        try {
            try {
                raf = new RandomAccessFile(file, "r");
            } catch (FileNotFoundException e) {
                // if not work, strip the FN of f/ & ?\d and openy again.
                pictureData = pictureData.replaceAll("(file|content):[\\/]*([^?]*)([?0-9A-Za-z=]*)", "/$2");
                //  file:/// not work. Needs a path! "file:[\\/]*", "/"
                //  Andoird: sends content://media if not mod.
                System.out.println("uploadAttachment file pic regexd .."+pictureData  );

                file = new File(pictureData);
                raf = new RandomAccessFile(file, "r");
            }

            bytes = new byte[(int) raf.length()];
            raf.readFully(bytes);

            fileName = file.getName();
            success = true; // reading done

            raf.close();
        } catch (FileNotFoundException e) {
            String picFNError = "Error finding file!";
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                    "navigator.notification.alert",
                                    new Object[] { picFNError, null, "Alert", "OK"});
            System.out.println(e);
        } catch (IOException ie) {
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                    "navigator.notification.alert",
                                    new Object[] {"Error reading file!", null, "Alert", "OK"});
            System.out.println(ie);
        } finally {
            try {
                if (raf!=null)
                    raf.close();
            } catch (IOException e) { System.out.println("/* 42 */"); }
        }

        // REST : uploading
        if (success) {
            String encodeToString = Base64.getEncoder().encodeToString(bytes); // mimeEncoder outputs literal \r\n
            System.out.println("Setting att name to send:"+fileName + "(File Read)");
            incident.setFileName(fileName);
            incident.setData(encodeToString);

            RestAdapter.Status statO = Util.updateObject(incident, "incidents/"+incident.getId()+"/fileAttachments",
                                 IncidentAttributes.incidentAttachmentAttrs);
            if (statO!=null && "200".equals(statO.getStatus())) {
                setPictureData("");         // removing IMG path
                setIsPictureDataSet("0");   // removing flag to display
                AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                        "navigator.notification.alert",
                                        new Object[] {"Attachment is uploaded successfully", null, "Status", null });
                // return to the attachments page per req
                AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                                                    "adf.mf.api.amx.doNavigation",
                                                                    new Object[] {"__back"});
            } else {
                String popupDiagErrMsg = statO != null ? statO.getMessage() :
                    "There was an error uploading the attachment"; // generic

                AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                        "navigator.notification.alert",
                                        new Object[] { popupDiagErrMsg, null, "Upload Error", null });
                // no return to the attachments .. req
            }
        }
    }
    
    /**
     *
     * @param incident  with inc data or ID
     * @param isPictureDataSet flag, if pic set; bool or string.
     * @param pictureData - IMG filename SIMPLY. Or data if we use another process.
     * @param attachmentName - attachment FILE name, if it's a file.
     */
    public void uploadSPenAttachment(Incident incident, boolean isPictureDataSet, String pictureData, String attachmentName) {
        System.out.println("uploadAttachment called.. (SPEN VERSION) ");
        System.out.println("uploadAttachment incident .."+incident.getId() + " isPicDataSet: "+isPictureDataSet );
        //System.out.println("uploadAttachment file pic .."+pictureData !TMI!+ " fileIfFileName "+attachmentName );
        System.out.println("getIsPictureDataSet() .."+getIsPictureDataSet() );
        if ("".equals(pictureData) || pictureData == null || !"1".equals(getIsPictureDataSet()) ) {
        System.out.println("uploadAttachment cancelling due to name/not set" );
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                    "navigator.notification.alert",
                                    new Object[] {"Please select a pic (filename!)", null, "Alert", "OK"});
            return;
        } else {
            System.out.println("uploadSPenAttachment PICTUREDATA LENGTH : "+ pictureData.length() );
        }

        try {
           //pictureData = pictureData.replaceAll("(file|content):[\\/]*([^?]*)([?0-9A-Za-z=]*)", "/$2");

           // REST : uploading

           String encodeToString; // mimeEncoder outputs literal \r\n; somebody unnecessarily copy/pasted too much of my code; 
           encodeToString = pictureData;
        
           System.out.println("Setting att name to send:"+publishImgFn + "(File Read)");
           incident.setFileName(publishImgFn);
           incident.setData(encodeToString);

           RestAdapter.Status statO = Util.updateObject(incident, "incidents/"+incident.getId()+"/fileAttachments",
                                IncidentAttributes.incidentAttachmentAttrs);
           if (statO!=null && "200".equals(statO.getStatus())) {
               setPictureData("");         // removing IMG path
               setIsPictureDataSet("0");   // removing flag to display
               AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                       "navigator.notification.alert",
                                       new Object[] {"Attachment is uploaded successfully", null, "Status", null });
               // return to the attachments page per req
               AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                                                   "adf.mf.api.amx.doNavigation",
                                                                   new Object[] {"__back"});
           } else {
               String popupDiagErrMsg = statO != null ? statO.getMessage() :
                   "There was an error uploading the attachment"; // generic

               AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                       "navigator.notification.alert",
                                       new Object[] { popupDiagErrMsg, null, "Upload Error", null });
               // no return to the attachments .. req
           }
       } catch (Exception e) {
            System.out.println("Exception in uploading SPen Attachment :  "+e);
            // TODO:  e.printStackTrace();
        }

    }

    /**
     *  Device spec
     *
     * @return
     */
    public String GetPicture(){
        // getting a PFL if we're called with the Camera button
        Object attachmentSourceType = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.attachmentSourceType}");
        if (attachmentSourceType instanceof String){
            String pflSrc = String.valueOf(attachmentSourceType);
            if (pflSrc.startsWith("snap")) {
                // this case possibly not used if you have a direct actionListener to the Snap function
                setSource( DeviceManager.CAMERA_SOURCETYPE_CAMERA );
                System.out.println("Before calling getPicture, set  source to Camera SNAP: " + source);
            } else {
                setSource( DeviceManager.CAMERA_SOURCETYPE_PHOTOLIBRARY );
            }
        }

        switch (getSizeSlider()) {
        case 1:
            quality = 50;
            targetHeight = 1024;
            targetWidth = 1024;
            break;
        case 2:
            quality = 75;
            targetHeight = 2048;
            targetWidth = 2048;
            break;
        case 0:        
        default:
            quality = 25;
            targetHeight = 500;
            targetWidth = 500;
            break;
        }
        setDestination(DeviceManager.CAMERA_DESTINATIONTYPE_FILE_URI);

        DeviceManager deviceManager=DeviceManagerFactory.getDeviceManager();
        System.out.println("Before calling getPicture: quality: "+quality+" dest:"+destination
                            +" source:"+source+" enc:"+encoding+" tWidth:"+targetWidth+" tHeight:"+targetHeight);

        String pictureName=deviceManager.getPicture(quality, destination, source, allowEdit, encoding, targetWidth, targetHeight);
        if(pictureName!=null && pictureName.trim().length()>0){
            System.out.println("GetPicture chosen a pic path of: "+pictureName);
            setPictureData(pictureName);
            setIsPictureDataSet("1");
        }
        return pictureName;
    }

    /**
     * testing
     * @param quality
     * @param width
     * @param height
     * @return
     */
    public String GetPictureWithParamsZeroWidth(int quality, int width, int height) {
        DeviceManager deviceManager=DeviceManagerFactory.getDeviceManager();
        setDestination(DeviceManager.CAMERA_DESTINATIONTYPE_FILE_URI);
        System.out.println("Before calling  GetPictureWithParamsZeroWidth: quality: "+quality+" dest:"+destination
                            +" enc:"+encoding+" Width:"+width+" Height:"+height);

        String pictureName=deviceManager.getPicture(quality, destination,
                                                    DeviceManager.CAMERA_SOURCETYPE_PHOTOLIBRARY,
                                                    allowEdit, encoding, width, height);
        if(pictureName!=null && pictureName.trim().length()>0) {
            System.out.println("GetPicture chosen a pic path of: "+pictureName);
            setPictureData(pictureName);
            setIsPictureDataSet("1");
        }
        return pictureName;
    }

    /**
     *  Device spec
     *
     * @return
     */
    public String SnapPhotoWithCamera(){
        DeviceManager deviceManager=DeviceManagerFactory.getDeviceManager();
        setDestination(DeviceManager.CAMERA_DESTINATIONTYPE_FILE_URI);

        System.out.println("Before calling getPicture/SnapPhoto, quality: "+quality+" dest:"+destination
                            +" source:"+source+" enc:"+encoding+" tWidth:"+targetWidth+" tHeight:"+targetHeight);

        String pictureName=deviceManager.getPicture(quality, destination, DeviceManager.CAMERA_SOURCETYPE_CAMERA,
                                                    allowEdit, encoding, targetWidth, targetWidth);
        if(pictureName!=null && pictureName.trim().length()>0){
            System.out.println("SnapPhotoWithCamera chosen a pic path of: "+pictureName);
            setPictureData(pictureName);
            setIsPictureDataSet("1");

            // navigate to pic preview page
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                                                "adf.mf.api.amx.doNavigation",
                                                                new Object[] {"goToAddAtt"});

        }
        return pictureName;
    }

    public void LaunchPen(){
        setDestination(DeviceManager.CAMERA_DESTINATIONTYPE_DATA_URL);
        
        System.out.println("Android JVM Launchpen; destination : "+destination + " initValue is(f): "+DeviceManager.CAMERA_DESTINATIONTYPE_FILE_URI);
        setIsPictureDataSet("0");
        
        String pictureName = "popupId";
        AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                                            "launchSurfacePopup",
                                                            new Object[] {pictureName, "retType"});
/*
        //AdfmfJavaUtilities.getELValue("#{applicationScope.AdderBean.pictureData}");
        setIsPictureDataSet("1");
        // navigate to pic preview page
        AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                                                "adf.mf.api.amx.doNavigation",
                                                                    new Object[] {"goToAddSPenImage"});
*/
    }
    
    public void PublishSPenImage() {
        publishImgFn = "S-Pen-" + new SimpleDateFormat("MMdd_HHmmssS").format(new Date()) +".jpg";
        
        System.out.println("Android JVM PublishSPenImage; destination is : "+destination+" ; SETTING DATA FROM VAR! "+
                           "Filename be: "+ publishImgFn);

        Object obj = AdfmfJavaUtilities.getELValue("#{applicationScope.sPenData}");
        if (obj!=null && obj instanceof String) {
            setIsPictureDataSet("1");
            
            System.out.println("Android JVM PublishSPenImage; data len is : "+((String)obj).length());

            // val to display
            AdfmfJavaUtilities.setELValue("#{bindings.pictureData.inputValue}", obj );
            AdfmfJavaUtilities.setELValue("#{applicationScope.sPenData}", "");          // remove old val of course
        } else {
            System.out.println("Android JVM PublishSPenImage; data WAS NULL SO NOT SETTING A VAR: "+ obj);
        }
    }
}
