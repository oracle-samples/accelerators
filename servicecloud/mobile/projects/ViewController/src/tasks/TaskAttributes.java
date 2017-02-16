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

package tasks;

import java.util.ArrayList;

import util.AttrType;
import util.Attribute;

public class TaskAttributes {
    public TaskAttributes() {
        super();
    }
    
    protected static ArrayList<Attribute> taskAttrs = new ArrayList<Attribute>();
    protected static ArrayList<Attribute> taskEditAttrs = new ArrayList<Attribute>();
    
    static {
        taskAttrs.add(new Attribute("Id", AttrType.INT, "id"));
        taskAttrs.add(new Attribute("Name", AttrType.STRING, "name"));
        taskAttrs.add(new Attribute("Notes", AttrType.STRING, "notes.text")); // NB FT TBD TODO
        taskAttrs.add(new Attribute("Assigned", AttrType.STRING, "assignedToAccount.lookupName"));
        taskAttrs.add(new Attribute("AssignedId", AttrType.MENU, "assignedToAccount.id"));
        taskAttrs.add(new Attribute("DueDate", AttrType.DATETIME, "dueTime"));
        taskAttrs.add(new Attribute("Status", AttrType.STRING, "statusWithType.status.lookupName"));
        taskAttrs.add(new Attribute("StatusId", AttrType.MENU, "statusWithType.status.id"));
        taskAttrs.add(new Attribute("PercentComplete", AttrType.INT, "percentComplete"));
        taskAttrs.add(new Attribute("Priority", AttrType.STRING, "priority.lookupName"));
        taskAttrs.add(new Attribute("PriorityId", AttrType.MENU, "priority.id"));
        taskAttrs.add(new Attribute("PlannedCompletion", AttrType.DATETIME, "plannedCompletionTime"));
        taskAttrs.add(new Attribute("DateComplete", AttrType.DATETIME, "completedTime"));
        taskAttrs.add(new Attribute("TaskType", AttrType.STRING, "taskType.lookupName"));
        taskAttrs.add(new Attribute("TaskTypeId", AttrType.MENU, "taskType.id"));
        taskAttrs.add(new Attribute("IncidentRefNo", AttrType.STRING, "serviceSettings.incident.lookupName"));
        taskAttrs.add(new Attribute("IncidentId", AttrType.MENU, "serviceSettings.incident.id"));
        taskAttrs.add(new Attribute("Contact", AttrType.STRING, "contact.lookupName"));
        taskAttrs.add(new Attribute("ContactId", AttrType.MENU, "contact.id"));
        taskAttrs.add(new Attribute("Organization", AttrType.STRING, "organization.lookupName"));
        taskAttrs.add(new Attribute("OrganizationId", AttrType.MENU, "organization.id"));
        
        // For edit functionaility, lookupName cannot be modified
        taskEditAttrs.add(new Attribute("Name", AttrType.STRING, "name"));
        taskEditAttrs.add(new Attribute("AssignedId", AttrType.MENU, "assignedToAccount.id"));
        taskEditAttrs.add(new Attribute("DueDate", AttrType.DATETIME, "dueTime"));
        taskEditAttrs.add(new Attribute("StatusId", AttrType.MENU, "statusWithType.status.id"));
        taskEditAttrs.add(new Attribute("PercentComplete", AttrType.INT, "percentComplete"));
        taskEditAttrs.add(new Attribute("PriorityId", AttrType.MENU, "priority.id"));
        taskEditAttrs.add(new Attribute("PlannedCompletion", AttrType.DATETIME, "plannedCompletionTime"));
        taskEditAttrs.add(new Attribute("DateComplete", AttrType.DATETIME, "completedTime"));
        taskEditAttrs.add(new Attribute("TaskTypeId", AttrType.MENU, "taskType.id"));
        taskEditAttrs.add(new Attribute("IncidentId", AttrType.MENU, "serviceSettings.incident.id"));
        taskEditAttrs.add(new Attribute("ContactId", AttrType.MENU, "contact.id"));
        taskEditAttrs.add(new Attribute("OrganizationId", AttrType.MENU, "organization.id"));
    }
}
