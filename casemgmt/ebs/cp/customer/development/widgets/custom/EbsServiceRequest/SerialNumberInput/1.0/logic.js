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
 *  date: Thu Nov 12 00:52:37 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 07ff810c2ee400d53c19d227b1bf7f2f9d70621f $
 * *********************************************************************************************
 *  File: logic.js
 * ****************************************************************************************** */

RightNow.namespace('Custom.Widgets.EbsServiceRequest.SerialNumberInput');
Custom.Widgets.EbsServiceRequest.SerialNumberInput = RightNow.Field.extend({
    overrides: {
        constructor: function() {
            this.parent();

            this.input = this.Y.one(this._inputSelector);
            if (!this.input)
                return;

            if (this.data.attrs.hint && !this.data.attrs.hide_hint && !this.data.attrs.always_show_hint) {
                this._initializeHint();
            }
            if (this.data.attrs.initial_focus && this.input.focus)
                this.input.focus();

            //setup mask
            if (this.data.js.mask) {
                this._initia
                lizeMask();
            }
            //province changing: update phone/postal masks - note: field may get mask from country selection
            if (RightNow.Text.beginsWith(this._fieldName, "Contact.Phones.")
                    || this._fieldName === "Contact.Address.PostalCode") {
                RightNow.Event.on("evt_provinceResponse", this.onProvinceChange, this);
            }

            //check for existing username/email
            if (this.data.attrs.validate_on_blur) {
                this.input.on("blur", this._blurValidate, this);
                //Add blur validation to Validate Field
                if (this.data.attrs.require_validation) {
                    this.Y.Event.attach("blur", this._blurValidate, this._inputSelector + '_Validate', this, true);
                }
            }

            //add change handlers for dynamic forms
            this.input.on('change', function() {
                this.fire('change', this);
            }, this);

            if (this.data.attrs.require_validation) {
                this.Y.one(this._inputSelector + '_Validate').on('change', function() {
                    this.fire('change', this);
                }, this);
            }

            this._isFormSubmitting = false;

            this.parentForm().on("submit", this.onValidate, this)
                    .on("send", this._toggleFormSubmittingFlag, this)
                    .on("response", this._toggleFormSubmittingFlag, this);

            this.on("constraintChange", this.constraintChange, this);

            RightNow.Event.on("evt_formValidateFailure", this._onValidateFailure, this);

            ///////////
            this._validationResultDisplay = this.Y.one(this.baseSelector + '_ValidationResultDisplay');
            this._verifyButton = this.Y.one(this.baseSelector + '_VerifySubmit');
            this._verifyButton.on('click', this._onSerialNumberValidationClick, this);
            this._loadingDiv = this.Y.one(this.baseSelector + '_Loading');

            RightNow.Event.subscribe('evt_productCategorySelected', this._onProductSelected, this);

            this._productID = null;
        }
    },
    /**
     * Used by Dynamic Forms to switch between a required and a non-required label
     * @param  {Object} container    The DOM node containing the label
     * @param  {Boolean} requiredness True or false
     * @param  {String} label        The label text to be inserted
     * @param  {String} template     The template text
     */
    swapLabel: function(container, requiredness, label, template) {
        var templateObject = {
            label: label,
            instanceID: this.instanceID,
            fieldName: this._fieldName,
            required: requiredness,
            requiredMarkLabel: RightNow.Interface.getMessage("FIELD_REQUIRED_MARK_LBL"),
            requiredLabel: RightNow.Interface.getMessage("REQUIRED_LBL"),
            hint: this.data.attrs.hint
        };

        container.setHTML('');
        container.append(new EJS({text: template}).render(templateObject));
    },
    /**
     * Triggered whenever a constraint is changed.
     * @param  {String} evt        The event name
     * @param  {Object} constraint A list of constraints being changed
     */
    constraintChange: function(evt, constraint) {
        constraint = constraint[0];
        if (constraint.required === this.data.attrs.required)
            return;

        //Remove the highlight
        this.toggleErrorIndicator(false);
        if (this.data.attrs.require_validation) {
            this.toggleErrorIndicator(false, this.Y.one(this._inputSelector + '_Validate'), this.Y.one(this._inputSelector + '_LabelValidate'));
        }

        //If the requiredness changed and the form has already validated clear the messages
        if (this.data.attrs.required && this.lastErrorLocation) {
            this.Y.one('#' + this.lastErrorLocation).all("[data-field='" + this._fieldName + "']").remove();
        }

        //Replace any old labels with new labels
        if (this.data.attrs.label_input) {
            this.swapLabel(this.Y.one(this.baseSelector + '_LabelContainer'), constraint.required, this.data.attrs.label_input, this.getStatic().templates.label);
        }

        if (this.data.attrs.require_validation) {
            var labelValidate = RightNow.Text.sprintf(this.data.attrs.label_validation, this.data.attrs.label_input);
            this.swapLabel(this.Y.one(this.baseSelector + '_LabelValidateContainer'), constraint.required, labelValidate, this.getStatic().templates.labelValidate);
        }

        this.data.attrs.required = constraint.required;
    },
    /**
     * A convenience method for retrieving the correct verification field value.
     * @return The verify field value
     */
    getVerificationValue: function() {
        return this._getTextFieldValue(this.Y.one(this._inputSelector + '_Validate'));
    },
    /**
     * Event handler executed when form is being submitted
     *
     * @param type String Event name
     * @param args Object Event arguments
     */
    onValidate: function(type, args) {
        var eventObject = this.createEventObject(),
                errors = [];

        this.toggleErrorIndicator(false);

        if (!this.validate(errors) || (this.data.attrs.require_validation && !this._validateVerifyField(errors)) || !this._compareInputToMask(true)) {
            this.lastErrorLocation = args[0].data.error_location;
            this._displayError(errors, this.lastErrorLocation);
            RightNow.Event.fire("evt_formFieldValidateFailure", eventObject);
            return false;
        }

        RightNow.Event.fire("evt_formFieldValidatePass", eventObject);
        return eventObject;
    },
    _displayError: function(errors, errorLocation) {
        var commonErrorDiv = this.Y.one("#" + errorLocation),
                verifyField;

        if (commonErrorDiv) {
            for (var i = 0, errorString = "", message, id = this.input.get("id"); i < errors.length; i++) {
                message = errors[i];
                if (typeof message === "object" && message !== null && message.id && message.message) {
                    id = verifyField = message.id;
                    message = message.message;
                }
                else {
                    message = (message.indexOf("%s") > -1) ? RightNow.Text.sprintf(message, this.data.attrs.label_input) : this.data.attrs.label_input + " " + message;
                }
                errorString += "<div data-field=\"" + this._fieldName + "\"><b><a href='javascript:void(0);' onclick='document.getElementById(\"" + id +
                        "\").focus(); return false;'>" + message + "</a></b></div> ";
            }
            commonErrorDiv.append(errorString);
        }

        if (!verifyField || errors.length > 1) {
            this.toggleErrorIndicator(true);
        }
    },
    /**
     * This function highlights the form field where the error was found
     *
     * @param showOrHide Boolean Should the highlight be shown
     * @param fieldToHighlight Node of the field to highlight
     * @param labelToHighlight Node of the label to highlight
     */
    toggleErrorIndicator: function(showOrHide, fieldToHighlight, labelToHighlight) {
        var method = ((showOrHide) ? "addClass" : "removeClass");
        if (fieldToHighlight && labelToHighlight) {
            fieldToHighlight[method]("rn_ErrorField");
            labelToHighlight[method]("rn_ErrorLabel");
        }
        else {
            this.input[method]("rn_ErrorField");
            this.Y.one(this.baseSelector + "_Label")[method]("rn_ErrorLabel");
        }
    },
    /**
     * Keep track of what state the form is in. We need to know if it is being submitted
     * so that we don't show any alert dialogs for onBlur errors.
     * @param {String} event Name of event being fired, either 'send' or 'response'
     */
    _toggleFormSubmittingFlag: function(event) {
        this._isFormSubmitting = (event === 'send');
    },
    /**
     * Validates that the input field has a value (if required) and that the value is
     * of the correct format.
     * @param {Object} event Blur event
     * @param {Boolean=} validateVerifyField Whether to validate the input field or
     *   its verify field; defaults to verifying the input field (false)
     */
    _blurValidate: function(event, validateVerifyField) {
        if (this._dialogShowing)
            return;

        this._trimField();

        if (validateVerifyField) {
            this._validateVerifyField();
        }
        else {
            var valid = this.validate();
            if (valid) {
                if (this._fieldName === "Contact.Login" || this.isCommonEmailType()) {
                    this._checkExistingAccount();
                }
            }
            this.toggleErrorIndicator(!valid);
        }
    },
    /**
     * Validates the field's "verify" field, whose value must match the field value.
     * @param {Array=} Array of error messages to populate; optional
     */
    _validateVerifyField: function(errors) {
        errors = errors || [];

        var valid = true,
                verifyField = this.Y.one(this._inputSelector + '_Validate'),
                verifyLabel = this.Y.one(this._inputSelector + '_LabelValidate');

        if (verifyField && this.data.attrs.require_validation) {
            var verifyValue = this.getVerificationValue(),
                    label = RightNow.Text.sprintf(this.data.attrs.label_validation, this.data.attrs.label_error || this.data.attrs.label_input);

            if (this.data.attrs.required && !verifyValue) {
                errors.push({message: RightNow.Text.sprintf(this.data.attrs.label_required, label), id: verifyField.get("id")});
                valid = false;
            }
            else if (verifyValue !== this.getValue()) {
                errors.push({message: RightNow.Text.sprintf(this.data.attrs.label_validation_incorrect, label, this.data.attrs.label_input), id: verifyField.get("id")});
                valid = false;
            }
            this.toggleErrorIndicator(!valid, verifyField, verifyLabel);
        }

        return valid;
    },
    /**
     * --------------------------------------------------------
     * Business Rules Events and Functions:
     * --------------------------------------------------------
     */

    /**
     * Event handler for when email or login field blurs
     * Check to see if the username/email is unique
     */
    _checkExistingAccount: function() {
        var massagedNewValue = this._massageValueForModificationCheck(this._value);
        if (this._value === "" || massagedNewValue === this._seenValue || (this.data.js.previousValue && this._value.replace('&', '&amp;').replace("'", '&#039;') === this.data.js.previousValue))
            return;

        this._seenValue = massagedNewValue;

        var eventObject = new RightNow.Event.EventObject(this, {data: {contactToken: this.data.js.contactToken}});

        if (this.isCommonEmailType())
            eventObject.data.email = this._value;
        else if (this._fieldName === "Contact.Login")
            eventObject.data.login = this._value;

        if (RightNow.Event.fire("evt_accountExistsRequest", eventObject)) {
            RightNow.Ajax.makeRequest(this.data.attrs.existing_contact_check_ajax, eventObject.data, {
                successHandler: this._onAccountExistsResponse,
                scope: this,
                data: eventObject,
                json: true
            });
        }
    },
    /**
     * Sometimes we need to have more complex rules to determine if two field values are the same.
     * @param value String string To massage.
     * @returns String Massaged value.
     */
    _massageValueForModificationCheck: function(value) {
        if (this.Y.Lang.isUndefined(value) || value === null || value === "") {
            return "";
        }
        if (this.isCommonEmailType()) {
            value = value.toLowerCase();
        }
        return value;
    },
    /**
     * If the response has a message and we aren't in the process of submitting
     * then alert the message; otherwise no duplicate account exists.
     * @param Object|Boolean response Server response to request
     * @param Object originalEventObject Event arguments
     */
    _onAccountExistsResponse: function(response, originalEventObject) {
        if (RightNow.Event.fire("evt_accountExistsResponse", response, originalEventObject)) {
            if (response !== false && this._isFormSubmitting === false) {
                this.toggleErrorIndicator(true);
                //create action dialog with link to acct assistance page
                var warnDialog,
                        handleOK = function() {
                            warnDialog.hide();
                            this._dialogShowing = false;
                            this.input.focus();
                        };

                warnDialog = RightNow.UI.Dialog.messageDialog(response.message, {icon: "WARN", exitCallback: {fn: handleOK, scope: this}});
                this._dialogShowing = true;
                warnDialog.show();
            }
        }
    },
    /**
     * --------------------------------------------------------
     * Mask Functions
     * --------------------------------------------------------
     */

    /**
     * Event handler for when province/state data is returned from the server
     *
     * @param type String Event name
     * @param args Object Event arguments
     */
    onProvinceChange: function(type, args)
    {
        var eventObj = args[0],
                resetMask = false;

        if (!eventObj.ProvincesLength)
            this.data.js.mask = "";

        if (this._fieldName === "Contact.Address.PostalCode" && "PostalMask" in eventObj)
        {
            resetMask = true;
            this.data.js.mask = eventObj.PostalMask;
        }
        else if ("PhoneMask" in eventObj)
        {
            resetMask = true;
            this.data.js.mask = eventObj.PhoneMask;
        }

        if (this._maskNodeOnPage)
            this._maskNodeOnPage.remove();

        if (resetMask && this.data.js.mask)
            this._initializeMask();
    },
    /**
     * Creates a mask overlay
     */
    _initializeMask: function()
    {
        this.input.on("keyup", this._compareInputToMask, this);
        this.input.on("blur", this._hideMaskMessage, this);
        this.input.on("focus", this._compareInputToMask, this);
        this.data.js.mask = this._createMaskArray(this.data.js.mask);
        //Set up mask overlay
        var overlay = this.Y.Node.create("<div class='rn_MaskOverlay'>");
        if (this.Y.Overlay) {
            this._maskNode = new this.Y.Overlay({
                bodyContent: overlay,
                visible: false,
                align: {
                    node: this.input,
                    points: [this.Y.WidgetPositionAlign.TL, this.Y.WidgetPositionAlign.BL]
                }
            });
            this._maskNode.render(this.baseSelector);
        }
        else {
            this._maskNode = overlay.addClass("rn_Hidden");
            this.input.insert(this._maskNode, "after");
        }

        if (this.data.attrs.always_show_mask) {
            //Write mask onto the page
            var maskMessageOnPage = this._getSimpleMaskString(),
                    widgetContainer = this.Y.one(this.baseSelector);
            if (maskMessageOnPage && widgetContainer) {
                var messageNode = this.Y.Node.create("<div class='rn_Mask'>" + RightNow.Interface.getMessage("EXPECTED_INPUT_LBL") + ": " + maskMessageOnPage + "</div>");
                if (widgetContainer.get("lastChild").hasClass("rn_HintText")) {
                    messageNode.addClass("rn_MaskBuffer");
                }
                this._maskNodeOnPage = messageNode;
                widgetContainer.append(messageNode);
            }
        }
    },
    /**
     * Creates a mask array based on the passed-in
     * string mask value.
     * @param mask String The new mask to apply to the field
     * @return Array the newly created mask array
     */
    _createMaskArray: function(mask)
    {
        if (!mask)
            return;
        var maskArray = [];
        for (var i = 0, j = 0, size = mask.length / 2; i < size; i++)
        {
            maskArray[i] = mask.substring(j, j + 2);
            j += 2;
        }
        return maskArray;
    },
    /**
     * Builds up simple mask string example based off of mask characters
     */
    _getSimpleMaskString: function() {
        if (!this.data.js.mask)
            return "";

        var maskString = "";
        for (var i = 0; i < this.data.js.mask.length; i++) {
            switch (this.data.js.mask[i].charAt(0)) {
                case "F":
                    maskString += this.data.js.mask[i].charAt(1);
                    break;
                case "U":
                    switch (this.data.js.mask[i].charAt(1)) {
                        case "#":
                            maskString += "#";
                            break;
                        case "A":
                        case "C":
                            maskString += "@";
                            break;
                        case "L":
                            maskString += "A";
                            break;
                    }
                    break;
                case "L":
                    switch (this.data.js.mask[i].charAt(1)) {
                        case "#":
                            maskString += "#";
                            break;
                        case "A":
                        case "C":
                            maskString += "@";
                            break;
                        case "L":
                            maskString += "a";
                            break;
                    }
                    break;
                case "M":
                    switch (this.data.js.mask[i].charAt(1)) {
                        case "#":
                            maskString += "#";
                            break;
                        case "A":
                        case "C":
                        case "L":
                            maskString += "@";
                            break;
                    }
                    break;
            }
        }
        return maskString;
    },
    /**
     * Compares entered value to required mask format
     * @param submitting Boolean Whether the form is submitting or not;
     * don't display the mask message if the form is submitting.
     * @return Boolean denoting of value coforms to mask
     */
    _compareInputToMask: function(submitting) {
        if (!this.data.js.mask)
            return true;

        var error = [],
                value = this.input.get("value");
        if (value.length > 0) {
            for (var i = 0, tempRegExVal; i < value.length; i++) {
                if (i < this.data.js.mask.length) {
                    tempRegExVal = "";
                    switch (this.data.js.mask[i].charAt(0)) {
                        case 'F':
                            if (value.charAt(i) !== this.data.js.mask[i].charAt(1))
                                error.push([i, this.data.js.mask[i]]);
                            break;
                        case 'U':
                            switch (this.data.js.mask[i].charAt(1)) {
                                case '#':
                                    tempRegExVal = /^[0-9]+$/;
                                    break;
                                case 'A':
                                    tempRegExVal = /^[0-9A-Z]+$/;
                                    break;
                                case 'L':
                                    tempRegExVal = /^[A-Z]+$/;
                                    break;
                                case 'C':
                                    tempRegExVal = /^[^a-z]+$/;
                                    break;
                            }
                            break;
                        case 'L':
                            switch (this.data.js.mask[i].charAt(1)) {
                                case '#':
                                    tempRegExVal = /^[0-9]+$/;
                                    break;
                                case 'A':
                                    tempRegExVal = /^[0-9a-z]+$/;
                                    break;
                                case 'L':
                                    tempRegExVal = /^[a-z]+$/;
                                    break;
                                case 'C':
                                    tempRegExVal = /^[^A-Z]+$/;
                                    break;
                            }
                            break;
                        case 'M':
                            switch (this.data.js.mask[i].charAt(1)) {
                                case '#':
                                    tempRegExVal = /^[0-9]+$/;
                                    break;
                                case 'A':
                                    tempRegExVal = /^[0-9a-zA-Z]+$/;
                                    break;
                                case 'L':
                                    tempRegExVal = /^[a-zA-Z]+$/;
                                    break;
                            }
                            break;
                    }
                    if ((tempRegExVal !== "") && !(tempRegExVal.test(value.charAt(i))))
                        error.push([i, this.data.js.mask[i]]);
                }
                else
                {
                    error.push([i, "LEN"]);
                }
            }
            //input matched mask but length didn't match up
            if ((!error.length) && (value.length < this.data.js.mask.length) && (!this.data.attrs.always_show_mask || submitting === true))
            {
                for (i = value.length; i < this.data.js.mask.length; i++)
                    error.push([i, "MISS"]);
            }
            if (error.length > 0)
            {
                //input didn't match mask
                this._showMaskMessage(error);
                if (submitting === true)
                    this._reportError(RightNow.Interface.getMessage("PCT_S_DIDNT_MATCH_EXPECTED_INPUT_LBL"));
                return false;
            }
            //no mask errors
            this._showMaskMessage(null);
            return true;
        }
        //haven't entered anything yet...
        if (!this.data.attrs.always_show_mask && submitting !== true)
            this._showMaskMessage(error);
        return true;
    },
    /**
     * Actually shows the error message to the user
     * @param error Array Collection of details about error to display
     */
    _showMaskMessage: function(error) {
        if (error === null) {
            this._hideMaskMessage();
        }
        else {
            if (!this._showMaskMessage._maskMessages) {
                //set a static variable containing error messages so it's lazily defined once across widget instances
                this._showMaskMessage._maskMessages = {
                    "F": RightNow.Interface.getMessage('WAITING_FOR_CHARACTER_LBL'),
                    "U#": RightNow.Interface.getMessage('PLEASE_TYPE_A_NUMBER_MSG'),
                    "L#": RightNow.Interface.getMessage('PLEASE_TYPE_A_NUMBER_MSG'),
                    "M#": RightNow.Interface.getMessage('PLEASE_TYPE_A_NUMBER_MSG'),
                    "UA": RightNow.Interface.getMessage('PLEASE_ENTER_UPPERCASE_LETTER_MSG'),
                    "UL": RightNow.Interface.getMessage('PLEASE_ENTER_AN_UPPERCASE_LETTER_MSG'),
                    "UC": RightNow.Interface.getMessage('PLS_ENTER_UPPERCASE_LETTER_SPECIAL_MSG'),
                    "LA": RightNow.Interface.getMessage('PLEASE_ENTER_LOWERCASE_LETTER_MSG'),
                    "LL": RightNow.Interface.getMessage('PLEASE_ENTER_A_LOWERCASE_LETTER_MSG'),
                    "LC": RightNow.Interface.getMessage('PLS_ENTER_LOWERCASE_LETTER_SPECIAL_MSG'),
                    "MA": RightNow.Interface.getMessage('PLEASE_ENTER_A_LETTER_OR_A_NUMBER_MSG'),
                    "ML": RightNow.Interface.getMessage('PLEASE_ENTER_A_LETTER_MSG'),
                    "MC": RightNow.Interface.getMessage('PLEASE_ENTER_LETTER_SPECIAL_CHAR_MSG'),
                    "LEN": RightNow.Interface.getMessage('THE_INPUT_IS_TOO_LONG_MSG'),
                    "MISS": RightNow.Interface.getMessage('THE_INPUT_IS_TOO_SHORT_MSG')
                };
            }
            var message = "",
                    sampleMaskString = this._getSimpleMaskString().split("");
            for (var i = 0, type; i < error.length; i++) {
                type = error[i][1];
                //F means format char should have followed
                if (type.charAt(0) === "F") {
                    message += "<b>" + RightNow.Interface.getMessage('CHARACTER_LBL') + " " + (error[i][0] + 1) + "</b> " + RightNow.Interface.getMessage('WAITING_FOR_CHARACTER_LBL') + type.charAt(1) + " ' <br>";
                    sampleMaskString[(error[i][0])] = "<span style='color:#E80000;'>" + sampleMaskString[(error[i][0])] + "</span>";
                }
                else {
                    if (type !== "MISS") {
                        message += "<b>" + RightNow.Interface.getMessage('CHARACTER_LBL') + " " + (error[i][0] + 1) + "</b> " + this._showMaskMessage._maskMessages[type] + "<br>";
                        if (type !== "LEN") {
                            sampleMaskString[(error[i][0])] = "<span style='color:#E80000;'>" + sampleMaskString[(error[i][0])] + "</span>";
                        }
                        else {
                            break;
                        }
                    }
                }
            }
            sampleMaskString = sampleMaskString.join("");
            this._setMaskMessage(RightNow.Interface.getMessage('EXPECTED_INPUT_LBL') + ": " + sampleMaskString + "<br>" + message);
            this._showMask();
        }
    },
    /**
     * Sets mask message.
     * @param message String message to set
     */
    _setMaskMessage: function(message)
    {
        var overlayContent = this._maskNode.get('bodyContent');
        if (overlayContent) {
            overlayContent.set('innerHTML', message);
        }
        else {
            this._maskNode.set('innerHTML', message);
        }
    },
    /**
     * Shows mask message.
     */
    _showMask: function()
    {
        if (this.Y.Overlay)
            this._maskNode.show();
        else
            RightNow.UI.show(this._maskNode);
    },
    /**
     * Hides mask message.
     */
    _hideMaskMessage: function()
    {
        if (this.Y.Overlay && this._maskNode.get("visible") !== false)
            this._maskNode.hide();
        else
            RightNow.UI.hide(this._maskNode);
    },
    /**
     * Reposition mask overlay, as field's Y changes when error div is displayed
     */
    _onValidateFailure: function()
    {
        // Make sure field has a mask that is visible
        if (this.data.js.mask && this._maskNode.align && this.Y.Overlay && this._maskNode.get("visible") !== false) {
            this._maskNode.align();
        }
    },
    ////////////////////
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

        if (this.input.get('value') === '') {
            this.displayVerifyResult(false, 'Please input serial number first', false);
            this._hideSpinner();
            return;
        }

        // Make AJAX request:
        var eventObj = new RightNow.Event.EventObject(this, {data: {
                w_id: this.data.info.w_id,
                product_id: this._productID,
                serial_number: this.input.get('value')
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
     * Callback function of the validation Ajax request.
     * display the corresponding hint
     * @param {String} verify result
     */
    serialNumberValidationCallback: function(result) {
        this._hideSpinner();

        if (typeof (result) === 'undefined' || result === null) {
            this.displayVerifyResult(false, this.data.attrs.ajax_failure_message, true);
            return false;
        }

        var eventObject = this.createEventObject();
        if (result.isValid === false) {
            this.displayVerifyResult(false, result.message, result.isError);
            return false;
        }

        this.displayVerifyResult(true, result.message, result.isError);
        RightNow.Event.fire('evt_formFieldValidatePass', eventObject);
        return eventObject;
    },
    /**
     * Display the verify result
     * @param {boolean} Indicates if the serial number is valid
     * @param {String} Validation result message
     */
    displayVerifyResult: function(isValid, message, isError) {
        this.toggleErrorIndicator(false);
        this._validationResultDisplay.empty();
        this._validationResultDisplay.removeClass('rn_Hidden');
        this._validationResultDisplay.removeClass('rn_InvalidSerialNumberValidationHint');
        this._validationResultDisplay.removeClass('rn_ValidSerialNumberValidationResult');

        if (isValid === false) {
            if (this.data.js.development_mode) {
               this._validationResultDisplay.insert(message);
            } else {
                 if(isError)
                    this._validationResultDisplay.insert(this.data.attrs.ajax_failure_message);
                 else
                    this._validationResultDisplay.insert(message);
            }

            this._validationResultDisplay.addClass('rn_InvalidSerialNumberValidationHint');
            this.toggleErrorIndicator(true);
        } else {
            this._validationResultDisplay.insert(message);
            this._validationResultDisplay.addClass('rn_ValidSerialNumberValidationResult');
        }
    },
    /**
     * show the spinner
     */
    _showSpinner: function() {
        this._loadingDiv.addClass('rn_SerialNumberValidationLoadingIndicator');
        this._verifyButton.setAttribute('disabled', 'disabled');
    },
    /**
     * hide the spinner
     */
    _hideSpinner: function() {
        this._loadingDiv.removeClass('rn_SerialNumberValidationLoadingIndicator');
        this._verifyButton.removeAttribute('disabled');
    }
});
