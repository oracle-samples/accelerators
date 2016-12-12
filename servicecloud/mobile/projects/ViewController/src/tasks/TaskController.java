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
 *  date: Tue Aug 23 16:36:00 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: 279390f9a250998262ef6e0c448ebf2a756fd53b $
 * *********************************************************************************************
 *  File: TaskController.java
 * *********************************************************************************************/

package tasks;

import java.util.ArrayList;
import java.util.List;

import oracle.adfmf.framework.api.AdfmfContainerUtilities;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;

import oracle.adfmf.json.JSONObject;

import rest_adapter.RestAdapter;

import util.TaskStatus;
import util.Util;

public class TaskController {
    private Task task;
    private Task cachedTask;

    public TaskController() {
        super();
        System.out.println("Some Constructor in Task" );
    }
    
    public void setTask(Task task) {
        this.task = task;
    }

    public Task getTask() {
        if (cachedTask != null) {
            System.out.println("Retrieving cachedTask in getTask");
            task = cachedTask;

            fetchAndSetIDsFromScope();
            
            cachedTask = null;
            return task; 
        }
        
        // else
        Integer id;
        Object ido = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.taskId}");
        System.out.println("Obtaining taskId in getTask: "+ido);
        if (ido instanceof String) {
            id = Integer.valueOf((String) ido);
        }
        else {
            id = (Integer) ido;
        }
        
        if (null != task && null != id && id == task.getId()) {
            return task;
        }
        
        task = new Task();
        if (id!=null && id != 0)
            task = loadTask(id.toString());
        
        return task;
    }
        
    public Task loadTask(String id) {
        Task task = new Task();
        System.out.println("Some Task id: " + id);
        Object obj = null;
        
        try {
            obj = Util.loadObject("tasks.Task", "tasks", id,  TaskAttributes.taskAttrs);
            
            if (obj!=null) {
                task = (Task)obj;
            } else {
                System.out.println("Some TASK id LOADED Error NULL: " + id);
            }
            
        } catch (Exception e) {
            System.out.println("Exception loading.. " + e);
        }
        
        this.task = task;
        return task;
    }
    
    public void cacheEditedTask(Task task) {
        //System.out.println("Caching Task");
        this.cachedTask = task;
    }
    
    
    public Task newTask() {
        if (cachedTask != null) {
            System.out.println("Retrieving cachedTask");
            task = cachedTask;
            
            fetchAndSetIDsFromScope();

            cachedTask = null;
            return task; 
        }
        
        Task task = new Task();
        task.setStatusId(TaskStatus.NOT_STARTED.getId());
        return task;
    }
    
    public void submitNewTask(Task task) {
        // Validate required fields
        if (! verifyInvalidFieldsDisplayAlert() )
            return;
        
        // Create task
        Util.createObject(task, "tasks", TaskAttributes.taskEditAttrs);
        
        AdfmfContainerUtilities.invokeContainerJavaScriptFunction(
            AdfmfJavaUtilities.getFeatureId(),
            "adf.mf.api.amx.doNavigation",
            new Object[] {"__back"});
    }

    public void submitNewNote() {
        // Validation
        Object o = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.newNoteChannel}");
        Integer newNoteChannel = null == o ? null : (Integer) o;
        o = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.newNoteText}");
        String newNoteText = null == o ? null : (String) o;
        o = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.taskId}");
        String taskId = null == o ? null : (String) o;
        ArrayList<String> invalidFields = new ArrayList<String>(20);
        if(Util.isNullOrEmpty(newNoteText)) newNoteText = " ";
        if (invalidFields.size() > 0) {
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(
                AdfmfJavaUtilities.getFeatureId(),
                "navigator.notification.alert",
                new Object[] { Util.join(invalidFields, ", ", null), null, "Alert", "OK" });
            return;
        }

        // Create
        JSONObject payload = new JSONObject();

        String result = "{}";
        try {
            payload.put("text", newNoteText);
            if (null != newNoteChannel) {
                payload.put("channel", new JSONObject().put("id", Integer.valueOf(newNoteChannel).intValue()));
            }
            String res = RestAdapter.doPOST(Util.QUERY_URL_CONNECTION, "tasks/" + taskId + "/notes", payload.toString());
            result = res;
        }
        catch (Exception e) {
            e.printStackTrace();
        }

        AdfmfContainerUtilities.invokeContainerJavaScriptFunction(
            AdfmfJavaUtilities.getFeatureId(),
            "adf.mf.api.amx.doNavigation",
            new Object[] {"__back"});
    }

    public void initNote() {
        AdfmfJavaUtilities.setELValue("#{pageFlowScope.newNoteText}", null);
        AdfmfJavaUtilities.setELValue("#{pageFlowScope.newNoteChannel}", null);
    }

    public void submitEditedTask(Task task) {
        // Validate required fields
        if ( ! verifyInvalidFieldsDisplayAlert() )
            return;

        System.out.println("Reached Save Edited. Will update task ID: "+task.getId());
        
        // Submit edited task
        boolean updateOk;
        updateOk = Util.updateObject(task, "tasks", Integer.toString(task.getId()), TaskAttributes.taskEditAttrs);
        if (updateOk) 
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                "adf.mf.api.amx.doNavigation", new Object[] {"__back"});
    }

    private void fetchAndSetIDsFromScope() {
        Object accountId = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.accountId}");
        Object accountName = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.accountName}");
        if(accountId != null){
            task.setAssignedId(Integer.parseInt((String)accountId));
            task.setAssigned((String)accountName);
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.accountId}", null);
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.accountName}", null);
        }
        Object organizationId = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.organizationId}");
        Object organizationName = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.organizationName}");
        if (organizationId != null) {
            task.setOrganizationId(Integer.parseInt((String)organizationId));
            task.setOrganization((String)organizationName);
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.organizationId}", null);
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.organizationName}", null);
        }
        Object incidentId = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.incidentId}");
        Object incidentRefNo = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.incidentRefNo}");
        if (incidentId != null) {
            task.setIncidentId(Integer.parseInt((String)incidentId));
            task.setIncidentRefNo((String)incidentRefNo);    
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.incidentId}", null);
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.incidentRefNo}", null);
        }
        Object contactId = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.contactId}");
        Object contactName = AdfmfJavaUtilities.evaluateELExpression("#{pageFlowScope.contactName}");
        if(contactId != null){
            task.setContactId(Integer.parseInt((String)contactId));
            task.setContact((String)contactName);
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.contactId}", null);
            AdfmfJavaUtilities.setELValue("#{pageFlowScope.contactName}", null);
        }
    }

    /**
     * returns false if the fields required are not filled in. 
     * 
     * @return
     */
    private boolean verifyInvalidFieldsDisplayAlert() {
        
        List<String> invalidFields = new ArrayList<String>();
        if (task.getName() == null || task.getName().trim().isEmpty()) {
            invalidFields.add("Name");
        }
        if (task.getStatusId() == null || task.getStatusId() == 0) {
            invalidFields.add("Status");
        }
        if (task.getAssignedId() == null || task.getAssignedId() == 0) {
            invalidFields.add("Assigned");
        }
        // Dynamically validate required fields based on Task Type selected; and nullify other related fields.
        if (task.getTaskTypeId() != null) {
            switch (task.getTaskTypeId()) {
                case 1:
                    if (task.getIncidentId() == null || task.getIncidentId() == 0) {
                        invalidFields.add("Incident");
                    }
                    break;
                case 2:
                    if (task.getContactId() == null || task.getContactId() == 0) {
                        invalidFields.add("Contact");
                    }
                    break;
                case 3:
                    if (task.getOrganizationId() == null || task.getOrganizationId() == 0) {
                        invalidFields.add("Organization");
                    }
                    break;
                default:
            }
        }
        if (!invalidFields.isEmpty()) {
            AdfmfContainerUtilities.invokeContainerJavaScriptFunction(AdfmfJavaUtilities.getFeatureId(),
                                                                      "navigator.notification.alert", new Object[] {
                                                                      Util.join(invalidFields, ", ",
                                                                                " cannot be empty!"), null, "Alert",
                                                                      "OK"
            });
            return false;
        }

        return true;
    }
}
