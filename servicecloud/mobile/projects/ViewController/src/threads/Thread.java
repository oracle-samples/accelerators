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
 *  SHA1: $Id: 2bfc00257337b9d31c35994bcfb44d736f57c09c $
 * *********************************************************************************************
 *  File: Thread.java
 * *********************************************************************************************/

package threads;

public class Thread {
    private String _text;
    private Integer _entryTypeId;
    private String _entryType;
    private Integer _channelId;
    private String _channel;
    private String _dateCreated;

    public void setDateCreated(String _dateCreated) {
        this._dateCreated = _dateCreated;
    }

    public String getDateCreated() {
        return _dateCreated;
    }

    public void setText(String _text) {
        this._text = _text;
    }

    public String getText() {
        return _text;
    }

    public void setEntryTypeId(Integer _entryTypeId) {
        this._entryTypeId = _entryTypeId;
    }

    public Integer getEntryTypeId() {
        return _entryTypeId;
    }

    public void setEntryType(String _entryType) {
        this._entryType = _entryType;
    }

    public String getEntryType() {
        return _entryType;
    }

    public void setChannelId(Integer _channelId) {
        this._channelId = _channelId;
    }

    public Integer getChannelId() {
        return _channelId;
    }

    public void setChannel(String _channel) {
        this._channel = _channel;
    }

    public String getChannel() {
        return _channel;
    }
   
    public Thread() {
        super();
    }
}
