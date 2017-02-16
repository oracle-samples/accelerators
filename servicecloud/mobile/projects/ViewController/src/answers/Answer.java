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

package answers;

import java.util.Date;

public class Answer {
    private Integer _id;
    private String _summary;
    private String _question;
    private String _status;
    private String _solution;
    private String _product; 
    private Date _updatedTime;
    private String _updatedTimeString;
    
    public Answer() {
        super();
    }

    public void setId(Integer _id) {
        this._id = _id;
    }

    public Integer getId() {
        return _id;
    }

    public void setSummary(String _summary) {
        this._summary = _summary;
    }

    public String getSummary() {
        return _summary;
    }

    public void setQuestion(String _question) {
        this._question = _question;
    }

    public String getQuestion() {
        return _question;
    }

    /*public void setStatus(String _status) {
        this._status = _status;
    }

    public String getStatus() {
        return _status;
    }

    public void setLastUpdDate(String _lastUpdDate) {
        this._lastUpdDate = _lastUpdDate;
    }

    public String getLastUpdDate() {
        return _lastUpdDate;
    }*/

    public void setSolution(String _solution) {
        this._solution = _solution;
    }

    public String getSolution() {
        return _solution;
    }

   /* public void setProduct(String _product) {
        this._product = _product;
    }

    public String getProduct() {
        return _product;
    }*/
    public void setUpdatedTime(Date _updatedTime) {
        this._updatedTime = _updatedTime;
    }

    public Date getUpdatedTime() {
        return _updatedTime;
    }

    public void setUpdatedTimeString(String _updatedTimeString) {
        this._updatedTimeString = _updatedTimeString;
    }

    public String getUpdatedTimeString() {
        return _updatedTimeString;
    }

    public void setStatus(String _status) {
        this._status = _status;
    }

    public String getStatus() {
        return _status;
    }

    public void setProduct(String _product) {
        this._product = _product;
    }

    public String getProduct() {
        return _product;
    }
}
