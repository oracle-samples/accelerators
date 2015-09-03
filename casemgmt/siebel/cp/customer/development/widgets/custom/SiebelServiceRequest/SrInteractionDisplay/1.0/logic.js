RightNow.namespace('Custom.Widgets.SiebelServiceRequest');
Custom.Widgets.SiebelServiceRequest.SrInteractionDisplay = RightNow.Widgets.extend({
    /**
     * Widget constructor.
     */
    constructor: function() {
        this._contentDiv = this.Y.one(this.baseSelector + '_Content');
        this._loadingDiv = this.Y.one(this.baseSelector + '_Loading');

        this._showSpinner();

        // send the request
        this.getInteractionAjaxEndpoint();
    },
    /**
     * Makes an AJAX request for `getInteraction_ajax_endpoint`.
     */
    getInteractionAjaxEndpoint: function() {
        // Make AJAX request:
        var eventObj = new RightNow.Event.EventObject(this, {data: {
                w_id: this.data.info.w_id,
                sr_id: this.data.js.sr_id,
                ext_server_type: this.data.js.ext_server_type
            }});
        RightNow.Ajax.makeRequest(this.data.attrs.get_interaction_ajax_endpoint, eventObj.data, {
            successHandler: this.getInteractionAjaxEndpointCallback,
            scope: this,
            data: eventObj,
            json: true,
            timeout: this.data.attrs.timeout,
            failureHandler: this.ajaxFailureHandler
        });
    },
    /**
     * Ajax request failure hander
     */
    ajaxFailureHandler: function() {
        this._showErrorMessage(this.data.attrs.ajax_failure_message);
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
     * Handles the AJAX response for `default_ajax_endpoint`.
     * @param {object} response JSON-parsed response from the server
     * @param {object} originalEventObj `eventObj` from #getDefault_ajax_endpoint
     */
    getInteractionAjaxEndpointCallback: function(response) {
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
    renderView: function(data) {
        var content = new EJS({text: this.getStatic().templates.view}).render({data: data});
        this._contentDiv.set('innerHTML', content);
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