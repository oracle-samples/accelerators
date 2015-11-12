/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 150520-000047
 *  date: Thu Nov 12 00:55:30 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 35d0202222aa8bffce6c5bc1d1437a3a7ac17aee $
 * *********************************************************************************************
 *  File: logic.js
 * ****************************************************************************************** */

RightNow.namespace('Custom.Widgets.SiebelServiceRequest.SrListGrid');
Custom.Widgets.SiebelServiceRequest.SrListGrid = RightNow.Widgets.extend({
    /**
     * Place all properties that intend to
     * override those of the same name in
     * the parent inside `overrides`.
     */

    constructor: function() {
        this._contentDiv = this.Y.one(this.baseSelector + '_Content');
        this._loadingDiv = this.Y.one(this.baseSelector + '_Loading');

        // show the spinner
        this._showSpinner();

        // send AJAX request
        this.getSrListAjaxEndpoint();
    },
    /**
     * Makes an AJAX request for `get_sr_list_ajax_endpoint`.
     */
    getSrListAjaxEndpoint: function() {
        // Make AJAX request:
        var eventObj = new RightNow.Event.EventObject(this, {data: {
                w_id: this.data.info.w_id,
                ext_server_type: this.data.js.ext_server_type
            }});
        RightNow.Ajax.makeRequest(this.data.attrs.get_sr_list_ajax_endpoint, eventObj.data, {
            successHandler: this.getSrListAjaxEndpointCallback,
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
     * Handel the getSrList AJAX request
     * @param {object} response JSON-parsed response from the server
     * @param {object} originalEventObj `eventObj` from #get_sr_list_ajax_endpoint
     */
    getSrListAjaxEndpointCallback: function(response) {
        if (response.error !== null) {
            this._showErrorMessage(response.error);
            return;
        }

        this.renderView(response.result);
        this._hideSpinner();
    },
    /**
     * Renders the `view.ejs` JavaScript template.
     */
    renderView: function(newdata) {
        var cols = newdata.headers.length,
                data = {
                    tableID: this.baseDomID + '_Grid',
                    caption: this.data.attrs.label_caption,
                    headers: [],
                    rows: []
                }, anchor, width, row, i, j, td;

        if (newdata.error) {
            RightNow.UI.Dialog.messageDialog(newdata.error, {'icon': 'WARN'});
        }

        // build up table data and add the new results to the widget's DOM
        if (this.data.attrs.headers) {
            if (newdata.row_num) {
                data.headers.push({label: this.data.attrs.label_row_number});
            }
            for (i = 0; i < cols; i++) {
                if (!newdata.headers[i].visible)
                    continue;

                td = {label: newdata.headers[i].heading};

                if (width = newdata.headers[i].width) {
                    td.style = 'width: "' + width + '%"';
                }

                data.headers.push(td);
            }
        }
        newdata.total_num = newdata.data.length;
        if (newdata.total_num > 0) {
            for (i = 0; i < newdata.total_num; i++) {
                row = [];
                if (newdata.row_num) {
                    row.push(i + 1);
                }
                for (j = 0; j < cols; j++) {
                    if (!newdata.headers[j].visible)
                        continue;
                    row.push(newdata.data[i][j]);
                }
                data.rows.push(row);
            }

            if (this.data.attrs.hide_when_no_results) {
                RightNow.UI.show(this.baseSelector);
            }
        }
        else if (this.data.attrs.hide_when_no_results) {
            RightNow.UI.hide(this.baseSelector);
        }

        this._contentDiv.set('innerHTML', new EJS({text: this.getStatic().templates.view}).render(data));
    },
    /**
     * Changes the loading icon and hides/unhide the data.
     * @param {Boolean} loading Whether to add or remove the loading indicators
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
