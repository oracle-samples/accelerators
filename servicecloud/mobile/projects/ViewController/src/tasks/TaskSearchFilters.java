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

import java.util.Date;

public class TaskSearchFilters {
    private String _name;
    
    private String _assigneeFirstName;
    private String _assigneeLastName;
    
    private Integer _statusId;
    private String _status;  // NB no!
    
    private Date _dueDate;
    private String _dueDateString;

    private String _login;

    public TaskSearchFilters() {
        super();
    }
    
    public TaskSearchFilters(TaskSearchFilters filters) {
        super();
        _name = filters._name;
        _assigneeFirstName = filters._assigneeFirstName;
        _assigneeLastName = filters._assigneeLastName;
        _status = filters._status;
        _dueDate = filters._dueDate;
        _dueDateString = filters._dueDateString;
        
    }

    public void setName(String _name) {
        this._name = _name;
    }

    public String getName() {
        return _name;
    }

    public void setStatusId(Integer _statusId) {
        this._statusId = _statusId;
    }

    public Integer getStatusId() {
        return _statusId;
    }

    public void setStatus(String _status) {
        this._status = _status;
    }

    public String getStatus() {
        return _status;
    }

    public void setDueDate(Date _dueDate) {
        this._dueDate = _dueDate;
        // System.out.println("setDueDate in TaskSearchFilters is : "+_dueDate);
    }

    public Date getDueDate() {
        return _dueDate;
    }

    public void setDueDateString(String _dueDateString) {
        this._dueDateString = _dueDateString;
    }

    public String getDueDateString() {
        return _dueDateString;
    }
    

    public void setLogin(String _login) {
        this._login = _login;
    }

    public String getLogin() {
        return _login;
    }


    public void setAssigneeFirstName(String _assigneeFirstName) {
        this._assigneeFirstName = _assigneeFirstName;
    }

    public String getAssigneeFirstName() {
        return _assigneeFirstName;
    }

    public void setAssigneeLastName(String _assigneeLastName) {
        this._assigneeLastName = _assigneeLastName;
    }

    public String getAssigneeLastName() {
        return _assigneeLastName;
    }
}
