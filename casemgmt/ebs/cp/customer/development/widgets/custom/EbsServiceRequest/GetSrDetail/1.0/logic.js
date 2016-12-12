/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:36 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 0edf1eee2127b84052fc462bbcfca129ed6e2854 $
 * *********************************************************************************************
 *  File: logic.js
 * ****************************************************************************************** */

RightNow.namespace('Custom.Widgets.EbsServiceRequest');
Custom.Widgets.EbsServiceRequest.GetSrDetail = RightNow.Widgets.extend({
    /**
     * Widget constructor.
     */
    constructor: function() {
        this._contentDiv = this.Y.one(this.baseSelector + '_Content');
        this._loadingDiv = this.Y.one(this.baseSelector + '_Loading');

        this._showSpinner();

        // send the request
        this.getSrDetailAjaxEndpoint();
    },
    /**
     * Makes an AJAX request for `get_sr_detail_ajax_endpoint`.
     */
    getSrDetailAjaxEndpoint: function() {
        var eventObj = new RightNow.Event.EventObject(this, {data: {
                w_id: this.data.info.w_id,
                sr_id: this.data.js.sr_id,
                ext_server_type: this.data.js.ext_server_type
            }});
        RightNow.Ajax.makeRequest(this.data.attrs.get_sr_detail_ajax_endpoint, eventObj.data, {
            successHandler: this.getSrDetailAjaxEndpointCallback,
            scope: this,
            data: eventObj,
            json: true,
            timeout: this.data.attrs.timeout,
            failureHandler: this.ajaxFailureHandler
        });
    },
    /**
     * Failure handler for the AJAX request
     */
    ajaxFailureHandler: function() {
        this._showErrorMessage(this.data.attrs.ajax_timeout_message);
    },
    /**
     * Handles the AJAX response for `getSrDetail_ajax_endpoint`.
     * @param {object} response JSON-parsed response from the server
     */
    getSrDetailAjaxEndpointCallback: function(response) {
        if (response.error !== null) {
            this._showErrorMessage(response.error);
            return;
        }
        // fire a event to render the SR detail data
        var eventObject = new RightNow.Event.EventObject(this, {data: {sr_detail_data: response.result}});
        RightNow.Event.fire('evt_SrDetailReturned', eventObject);

        this._hideSpinner();
    },
    /**
     * Display error message
     */
    _showErrorMessage: function(message) {
        this._hideSpinner();
        if (this.data.js.development_mode) {
            this._contentDiv.setHTML(message);
        } else {
            this._contentDiv.setHTML(this.data.attrs.ajax_failure_message);
        }
    },
    /**
     * show the spinner
     */
    _showSpinner: function() {
        this._loadingDiv.addClass('rn_Loading');
    },
    /**
     * hide the spinner
     */
    _hideSpinner: function() {
        this._loadingDiv.removeClass('rn_Loading');
    }
});