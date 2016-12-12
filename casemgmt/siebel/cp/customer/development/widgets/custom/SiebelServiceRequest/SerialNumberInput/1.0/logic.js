/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 150520-000047
 *  date: Mon Nov 30 20:14:22 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: 2c5bca1b4a375f4c376795274f4f3b144d10dc8f $
 * *********************************************************************************************
 *  File: logic.js
 * ****************************************************************************************** */

RightNow.namespace('Custom.Widgets.SiebelServiceRequest.ExtentedSerailNumberValidation2');
Custom.Widgets.SiebelServiceRequest.SerialNumberInput = RightNow.Widgets.TextInput.extend({
    overrides: {
        constructor: function() {
            this._validationResultDisplay = this.Y.one(this.baseSelector + "_ValidationResultDisplay");
            this._verifyButton = this.Y.one(this.baseSelector + "_VerifySubmit");
            this._verifyButton.on("click", this._onSerialNumberValidationClick, this);
            this._loadingDiv = this.Y.one(this.baseSelector + "_Loading");

            RightNow.Event.subscribe("evt_productCategorySelected", this._onProductSelected, this);

            this._productID = null;

            this.parent();
        }
    },
    /**
     * Event handler executed whenever a product is selected from product menu
     * @param {String} type Event name
     * @param {Object} args Event arguments
     */
    _onProductSelected: function(type, args) {
        this._productID = args[0].data.hierChain[args[0].data.hierChain.length - 1];
    },
    /* Event handler executed when verify button is being clicked
     * @param {Object} evt Click event
     */
    _onSerialNumberValidationClick: function(evt) {
        this._showSpinner();

        if (this.input.get("value") === "") {
            this.displayVerifyResult(false, "Please input Serial Number first", false);
            this._hideSpinner();
            return;
        }

        // Make AJAX request:
        var eventObj = new RightNow.Event.EventObject(this, {data: {
                w_id: this.data.info.w_id,
                product_id: this._productID,
                serial_number: this.input.get("value")
            }});
        RightNow.Ajax.makeRequest(this.data.attrs.serial_number_validate_ajax, eventObj.data, {
            successHandler: this.serialNumberValidationCallback,
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
        this._hideSpinner();
        this.displayVerifyResult(false, this.data.attrs.ajax_timeout_message, true);
    },
    /**
     * Callback function of the validation AJAX request.
     * display the corresponding hint
     * @param {String} verify result
     */
    serialNumberValidationCallback: function(result) {
        this._hideSpinner();

        if (typeof (result) === 'undefined' || result === null) {
            this.displayVerifyResult(false, 'No response from Siebel server', true);
            return false;
        }

        var eventObject = this.createEventObject();
        if (result.isValid === false) {
            this.displayVerifyResult(false, result.message, result.isError);
            return false;
        }

        this.displayVerifyResult(true, result.message, result.isError);
        RightNow.Event.fire("evt_formFieldValidatePass", eventObject, result.isError);
        return eventObject;
    },
    /**
     * Display the validation result
     * @param {boolean} Indicates if the serial number is valid
     * @param {String} Validation result message
     */
    displayVerifyResult: function(isValid, message, isError) {
        this.toggleErrorIndicator(false);
        this._validationResultDisplay.empty();
        this._validationResultDisplay.removeClass('rn_Hidden');
        this._validationResultDisplay.removeClass('rn_InvalidResult');
        this._validationResultDisplay.removeClass('rn_ValidResult');

        if (isValid === false) {
            if (this.data.js.development_mode) {
                this._validationResultDisplay.insert(message);
            } else {
                if (isError)
                    this._validationResultDisplay.insert(this.data.attrs.ajax_failure_message);
                else
                    this._validationResultDisplay.insert(message);
            }

            this._validationResultDisplay.addClass('rn_InvalidResult');
            this.toggleErrorIndicator(true);
        } else {
            this._validationResultDisplay.insert(message);
            this._validationResultDisplay.addClass('rn_ValidResult');
        }
    },
    /**
     * show the spinner
     */
    _showSpinner: function() {
        this._loadingDiv.addClass('rn_LoadingIndicator');
        this._verifyButton.setAttribute("disabled", "disabled");
    },
    /**
     * hide the spinner
     */
    _hideSpinner: function() {
        this._loadingDiv.removeClass('rn_LoadingIndicator');
        this._verifyButton.removeAttribute("disabled");
    },
});