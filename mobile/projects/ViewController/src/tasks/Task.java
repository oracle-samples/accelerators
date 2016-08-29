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
 *  date: Tue Aug 23 16:35:59 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: c6af81626391b1c7f9985610f83435db428a8ec3 $
 * *********************************************************************************************
 *  File: Task.java
 * *********************************************************************************************/

package tasks;

import java.util.Date;

import oracle.adfmf.java.beans.PropertyChangeListener;
import oracle.adfmf.java.beans.PropertyChangeSupport;

import util.TaskStatus;

public class Task {
    public static final String RESOURCE_NAME = "tasks";

    private Integer _id;

    private String _name;
    private Integer _assignedId;
    private String _assigned;
    private Date _dueDate;
    private String _dueDateString; //NB

    private Integer _statusId;
    private String _status;

    private Integer _percentComplete = 0; 
    private String _notes;

    private Integer _priorityId;
    private String _priority;
    
    private Date _plannedCompletion;
    private String _plannedCompletionString;
    private Date _dateComplete;
    private String _dateCompleteString;

    private Integer _taskTypeId;
    private String _taskType;

    private Integer _contactId;
    private String _contact;
    private Integer _organizationId;
    private String _organization;
    
    private Integer _incidentId;
    private String _incidentRefNo;

    private Integer _taskCounter = 0;
    private PropertyChangeSupport _propertyChangeSupport = new PropertyChangeSupport(this);

    public void setId(Integer _id) {
        this._id = _id;
    }

    public Integer getId() {
        return _id;
    }

    public void setName(String _name) {
        this._name = _name;
    }

    public String getName() {
        return _name;
    }

    public void setAssignedId(Integer _assignedId) {
        if(_assignedId == 0)
            _assignedId = null;
        this._assignedId = _assignedId;
    }

    public Integer getAssignedId() {
        return _assignedId;
    }

    public void setAssigned(String _assigned) {
        this._assigned = _assigned;
    }

    public String getAssigned() {
        return _assigned;
    }

    public void setDueDate(Date _dueDate) {
        this._dueDate = _dueDate;
    }

    public Date getDueDate() {
        return _dueDate;
    }

    public void setDueDateString(String _dueDateString) {
        this._dueDateString = _dueDateString;
    }

    public String getDueDateString() {
        return _dueDateString;
    } // NB

     public void setStatusId(Integer _statusId) {
            Integer oldStatusId = this._statusId;
            this._statusId = _statusId;
            if (oldStatusId == null || oldStatusId.intValue() != _statusId.intValue()) {
                if (_statusId == TaskStatus.COMPLETED.getId()) {
                    setPercentComplete(100);
                    setDateComplete(new Date());
                } else if ( _statusId == TaskStatus.NOT_STARTED.getId()) {
                    setPercentComplete(0);
                    setDateComplete(null);
                }
                /* 
                 * Special case: from Complete back to any other statuses,
                 * we don't know what reasonable percentage to mapped to for other statues (e.g. In Progress, Waiting)
                 * So we change the status back to "Not Started" and percentage to "0".
                 * 
                 */
                if (oldStatusId != null && oldStatusId == TaskStatus.COMPLETED.getId() && _percentComplete == 100) {
                    setPercentComplete(0);
                    setDateComplete(null);
                }
            }
            _propertyChangeSupport.firePropertyChange("statusId", oldStatusId, _statusId);
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

    public void setPercentComplete(Integer _percentComplete) {
        Integer oldPercentComplete = this._percentComplete;
        this._percentComplete = _percentComplete;
        if (oldPercentComplete == null || oldPercentComplete.intValue() != _percentComplete.intValue()) {
            switch (_percentComplete) {
                case 0:
                    setStatusId(TaskStatus.NOT_STARTED.getId());
                    setDateComplete(null);
                    break;
                case 100:
                    setStatusId(TaskStatus.COMPLETED.getId());
                    setDateComplete(new Date());
                    break;
                default:
                    setStatusId(TaskStatus.IN_PROGRESS.getId());
                    setDateComplete(null);
                    break;
            }
        }
        _propertyChangeSupport.firePropertyChange("percentComplete", oldPercentComplete, _percentComplete);
    }

    public Integer getPercentComplete() {
        return _percentComplete;
    }

    public void setNotes(String _notes) {
        this._notes = _notes;
    }

    public String getNotes() {
        return _notes;
    }

    public void setPriority(String _priority) {
        this._priority = _priority;
    }

    public String getPriority() {
        return _priority;
    }

    public void setPlannedCompletion(Date _plannedCompletion) {
        this._plannedCompletion = _plannedCompletion;
    }

    public Date getPlannedCompletion() {
        return _plannedCompletion;
    }

    public void setDateComplete(Date _dateComplete) {
        Date oldDateComplete = this._dateComplete;
        this._dateComplete = _dateComplete;
        if (oldDateComplete == null || !oldDateComplete.equals(_dateComplete)) {
            if (_dateComplete != null) {
                setStatusId(TaskStatus.COMPLETED.getId());
            } else if (_statusId != null && _statusId == TaskStatus.COMPLETED.getId()) {
                setStatusId(TaskStatus.NOT_STARTED.getId());
            }
        }
        _propertyChangeSupport.firePropertyChange("dateComplete", oldDateComplete, _dateComplete);
    }

    public Date getDateComplete() {
        return _dateComplete;
    }

    public void setTaskTypeId(Integer _taskTypeId) {
        this._taskTypeId = _taskTypeId;
    }

    public Integer getTaskTypeId() {
        return _taskTypeId;
    }

    public void setTaskType(String _taskType) {
        this._taskType = _taskType;
    }

    public String getTaskType() {
        return _taskType;
    }

    public void setContactId(Integer _contactId) {
        if(_contactId == 0)
            _contactId = null;
        this._contactId = _contactId;
    }

    public Integer getContactId() {
        return _contactId;
    }

    public void setContact(String _contact) {
        this._contact = _contact;
    }

    public String getContact() {
        return _contact;
    }

    public void setOrganizationId(Integer _organizationId) {
        if(_organizationId == 0)
            _organizationId = null;
        this._organizationId = _organizationId;
    }

    public Integer getOrganizationId() {
        return _organizationId;
    }

    public void setOrganization(String _organization) {
        this._organization = _organization;
    }

    public String getOrganization() {
        return _organization;
    }

    public void setIncidentId(Integer _incidentId) {
        if(_incidentId == 0)
            _incidentId = null;
        this._incidentId = _incidentId;
    }

    public Integer getIncidentId() {
        return _incidentId;
    }

    public void setIncidentRefNo(String _incidentRefNo) {
        this._incidentRefNo = _incidentRefNo;
    }

    public String getIncidentRefNo() {
        return _incidentRefNo;
    }

    public void setPlannedCompletionString(String _plannedCompletionString) {
        this._plannedCompletionString = _plannedCompletionString;
    }

    public String getPlannedCompletionString() {
        return _plannedCompletionString;
    }

    public void setDateCompleteString(String _dateCompleteString) {
        this._dateCompleteString = _dateCompleteString;
    }

    public String getDateCompleteString() {
        return _dateCompleteString;
    }

    public void setTaskCounter(Integer _taskCounter) {
        this._taskCounter = _taskCounter;
    }

    public Integer getTaskCounter() {
        return _taskCounter;
    }

    public Task() {
        super();
    }

    public void setPriorityId(Integer _priorityId) {
        this._priorityId = _priorityId;
    }

    public Integer getPriorityId() {
        return _priorityId;
    }

    public void addPropertyChangeListener(PropertyChangeListener l) {
        _propertyChangeSupport.addPropertyChangeListener(l);
    }

    public void removePropertyChangeListener(PropertyChangeListener l) {
        _propertyChangeSupport.removePropertyChangeListener(l);
    }
}
