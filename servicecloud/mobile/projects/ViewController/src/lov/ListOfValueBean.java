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

package lov;

import java.util.HashMap;
import java.util.Map;

public class ListOfValueBean {
    private ListOfValue[] incidentStatuses = null;
    private ListOfValue[] orgIndustries = null;
    private ListOfValue[] taskStatuses = null;
    private ListOfValue[] taskPriorities = null;
    private ListOfValue[] taskTypes = null; 
    private ListOfValue[] tasksNotesChannels = null;
    private ListOfValue[] assetStatuses = null;
    private ListOfValue[] assetCurrencyLov = null;
     
    public ListOfValueBean() {
        super();
    }
    
    public ListOfValue[] getIncidentStatuses() {
            this.incidentStatuses = ListOfValueController.getLov("/incidents/statusWithType/status");
            return incidentStatuses;
    }
    
    public void setIncidentStatuses(ListOfValue[] incidentStatuses) {
        this.incidentStatuses = incidentStatuses;
    }
    
    public ListOfValue[] getOrgIndustries() {
            this.orgIndustries = ListOfValueController.getLov("/organizations/industry");
            return this.orgIndustries;
    }
    
    public void setOrgIndustries(ListOfValue[] orgIndustries) {
        this.orgIndustries = orgIndustries;
    }

    public void setTaskStatuses(ListOfValue[] taskStatuses) {
        this.taskStatuses = taskStatuses;
    }

    public ListOfValue[] getTaskStatuses() {
        taskStatuses = ListOfValueController.getLov("/tasks/statusWithType/status");
        return taskStatuses;
    }

    public void setAssetStatuses(ListOfValue[] assetStatuses) {
        this.assetStatuses = assetStatuses;
    }

    public ListOfValue[] getAssetStatuses() {
        assetStatuses = ListOfValueController.getLov("/assets/statusWithType/status");
        return assetStatuses;
    }

    public void setTaskPriorities(ListOfValue[] taskPriorities) {
        this.taskPriorities = taskPriorities;
    }

    public ListOfValue[] getTaskPriorities() {
        this.taskPriorities = ListOfValueController.getLov("/tasks/priority");
        return taskPriorities;
    }

    public void setTaskTypes(ListOfValue[] taskTypes) {
        this.taskTypes = taskTypes;
    }

    public ListOfValue[] getTaskTypes() {
        this.taskTypes = ListOfValueController.getLov("/tasks/taskType");
        return taskTypes;
    }

    public void setTasksNotesChannels(ListOfValue[] tasksNotesChannels) {
        this.tasksNotesChannels = tasksNotesChannels;
    }

    public ListOfValue[] getTasksNotesChannels() {
        this.tasksNotesChannels = ListOfValueController.getLov("/tasks/notes/channel");
        return this.tasksNotesChannels;
    }

    public void setAssetCurrencyLov(ListOfValue[] assetCurrencyLov) {
        this.assetCurrencyLov = assetCurrencyLov;
    }

    public ListOfValue[] getAssetCurrencyLov() {
        assetCurrencyLov = ListOfValueController.getLov("/assets/price/currency");
        return assetCurrencyLov;
    }
}
