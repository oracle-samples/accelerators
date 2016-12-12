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
 *  date: Mon Nov 30 20:14:21 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: 136db49e98d686530dfbf790474d43195db778fa $
 * *********************************************************************************************
 *  File: logic.js
 * ****************************************************************************************** */

RightNow.namespace('Custom.Widgets.SiebelServiceRequest.PostalValidate');
Custom.Widgets.SiebelServiceRequest.PostalValidate = RightNow.Widgets.TextInput.extend({ 
    /**
     * Place all properties that intend to
     * override those of the same name in
     * the parent inside `overrides` THANK YOU.
     */
    overrides: {
        /**
         * Overrides RightNow.Widgets.TextInput#constructor.
         */
        constructor: function() {
            // Call into parent's constructor
            this.parent();
            
            // The current widget id is : this.input.get('id'); e.g. rn_PostalValidate_24_Contact.Address.PostalCode
            // The current widget value : this.input.get('value'); 
            
            ///////////
            this._validationResultDisplay = this.Y.one(this.baseSelector + '_ValidationResultDisplay');
            this._verifyButton = this.Y.one(this.baseSelector + '_VerifySubmit');
            this._verifyButton.on('click', this._onVerifyButtonClick, this);
            this._loadingDiv = this.Y.one(this.baseSelector + '_Loading');
            
            this._validationQuestion = this.Y.one(this.baseSelector + '_ValidationQuestion');
            this._validateCheckbox = this.Y.one(this.baseSelector + '_ValidateCheckbox');
            this._validateCheckbox.on('click', this._onValidateUseItClick, this);
                        
            this._veroAddress = null;
            this.missingIdName = null;
            
            // tryna determine address as an Object first;
            this.addressObjName = 
                    this.data.js.name.split('.').slice(0, -1).join('.');
            
            // the defaults for fields, as an object:
            var whichStreet = this.addressObjName+'.Street';
            var whichCity = this.addressObjName+'.City';
            var whichCountry = this.addressObjName+'.Country';
            var whichState = this.addressObjName+'.StateOrProvince';
            var whichZip = this.addressObjName+'.PostalCode';

            if (this.data.attrs.field_street||this.data.attrs.field_city
                    ||this.data.attrs.field_state||this.data.attrs.field_zip) {
                // but, if these are specified, we need to use individual fields
                whichStreet = this.data.attrs.field_street;
                whichCity = this.data.attrs.field_city;
                whichState = this.data.attrs.field_state;
                whichCountry = this.data.attrs.field_country;
                whichZip = this.data.attrs.field_zip;
                console.log('This widget ['+this.input.get('id')+'] Using individual fields, zip field is:'+whichZip);
            } else {
                console.log('This widget ['+this.input.get('id')+'] using address object:'+ this.addressObjName);
            }
            
            this.idStreet = this.getFirstElementIdByName(whichStreet);
            this.idCity   = this.getFirstElementIdByName(whichCity);
            this.idCountry = this.getFirstElementIdByName(whichCountry);
            this.idState = this.getFirstElementIdByName(whichState);
            this.idZip = this.getFirstElementIdByName(whichZip);
            this.idCheckbox =  'rn_'+this.instanceID + '_ValidateCheckbox';
            
            // HTML error finding - if all fields are there. 
            if (!this.idStreet) this.missingIdName = "Address.Street";
            if (!this.idCity) this.missingIdName = "Address.City";
            if (!this.idState) this.missingIdName = "Address.StateOrProvince";
            if (!this.idZip) this.missingIdName = "Address.PostalCode";            
            // Not checking for (!this.idCountry) because we can assume USA l8r
            
//          this works for IDs only in this widget (instanceID):
//          this.idZip = 'rn_'+this.instanceID + '_' +this.data['js']['name'];  // 'name' is the field to submit to
//            
//          The ID with this.baseSelector + is, e.g. : #rn_PostalValidate_24_ValidateCheckbox
//          While the real(concat) elem ID is, e.g. rn_PostalValidate_24_Contact.Address.PostalCode
//          Cannot assume Zip and current WidgetField are the same.
            console.log('idZip we\'ll be getting: '+ this.idZip);
            console.log('idCheckbox we\'ll be getting: '+ this.idCheckbox + ", this._validateCheckbox:"+this._validateCheckbox);
            if (this.missingIdName) console.log('missingIdName found: '+ this.missingIdName);
        },

        /**
         * Overridable methods from TextInput:
         *
         * Call `this.parent()` inside of function bodies
         * (with expected parameters) to call the parent
         * method being overridden.
         */
        /*
        // swapLabel: function(container, requiredness, label, template)
        // constraintChange: function(evt, constraint)
        // getVerificationValue: function()
        // onValidate: function(type, args)
        // _displayError: function(errors, errorLocation)
        // toggleErrorIndicator: function(showOrHide, fieldToHighlight, labelToHighlight)
        // _toggleFormSubmittingFlag: function(event)
        // _blurValidate: function(event, validateVerifyField)
        // _validateVerifyField: function(errors)
        // _checkExistingAccount: function()
        // _massageValueForModificationCheck: function(value)
        // _onAccountExistsResponse: function(response, originalEventObject)
        // onProvinceChange: function(type, args)
        // _initializeMask: function()
        // _createMaskArray: function(mask)
        // _getSimpleMaskString: function()
        // _compareInputToMask: function(submitting)
        // _showMaskMessage: function(error)
        // _setMaskMessage: function(message)
        // _showMask: function()
        // _hideMaskMessage: function()
        // _onValidateFailure: function()
        */
       
       /**
        * This one is at Submit
        */
        onValidate: function(type, args) {
            console.log("onValidate called, type is: "+type);  // type = submit
            // args[0].data has stuff: [{"w_id":"FormSubmit_35","data":{"form":"rn_CreateAccount","f_tok":"X","error_location":"rn_ErrorLocation","timeout":0},"filters":{}}]
            
            if (this._veroAddress !== null) {
                return this.parent();
            } else {
                
                if (this.data.attrs.verification_required) {
                    var but_label = this.data.attrs.label_button||'Verify Address';
                    var eventObject = this.createEventObject();
                    var errors = [{ message : 'Address validation is required to submit this form.',
                                    id: this.idStreet }, 
                                  { message : 'Please click the "'+but_label+'" button!',
                                    id: 'rn_'+this.instanceID + '_VerifySubmit' }];
                        // all u nd2do is defn obj EACH err, it byps the code 
                        // in parent for Field Label display.  ^^^
                        // i.e. array of errs. 
                        // importante: need the ID to lead to a Button (or field in Wiedget this.input.get('id')).
                        // If you make 2 err msgs, the red's turned on =)
                    console.log('Displaying Submit errors on top of page..');
                    
                    this.lastErrorLocation = args[0].data.error_location;
                    this._displayError(errors, this.lastErrorLocation);
                    this.toggleErrorIndicator(true);
                    RightNow.Event.fire("evt_formFieldValidateFailure", eventObject);
                    return false;
                } else {
                    this.toggleErrorIndicator(false);
                    return this.parent();
                }
//                RightNow.Event.fire("evt_formFieldValidatePass", eventObject);
//                return eventObject;
            }
        }
    },
    
    /* Event handler executed when verify button is being clicked
     * @param {Object} evt Click event
     */
    _onVerifyButtonClick: function(evt) {
        this.toggleErrorIndicator(false);  // in case there was an err on top of the screen.
        if (!this.addressObjName && !this.data.attrs.field_street) {
            this.displayVerificationLines(false, 'Specify correct attribute for \'name\' such as ="Contact.Address.Street.<br>Misconfigured!"', false);
            this._hideSpinner();
            return;
        } else if (this.missingIdName) {
            this.displayVerificationLines(false, 'Need HTML widget which submits to name="'+this.missingIdName+'"', false);
            this._hideSpinner();
            return;
        }
        this._showSpinner();
        this._veroAddress = null;
        
        this._validationQuestion.removeClass('rn_ValidPostalValidationResult');
        this._validationQuestion.addClass('rn_Hidden');
        var chkBox = document.getElementById(this.idCheckbox);
        if (chkBox)
            chkBox.checked = false;
        
        this._street = document.getElementById(this.idStreet).value;
        this._city = document.getElementById(this.idCity).value;
        this._zip = document.getElementById(this.idZip).value;  // might not be current field!

        var list = document.getElementById(this.idState);
        this._state = typeof list.options!== 'undefined' ? 
            list.options[list.selectedIndex].text : list.value;
                
        // used only for checking!;
        list = document.getElementById(this.idCountry);
        if (this.idCountry && list) {
            this._country = typeof list.options!== 'undefined' ? 
                list.options[list.selectedIndex].text : list.value;
        } 
        // Do not check for country=USA; call the controller for a LOG // return;
        if (!(this._country = this._country.trim())) {
            this._country = 'US'; // just assuming 'US' for this ..USPS widget.
        }


        //
        // console.log('this.data: '+JSON.stringify(this.data)); //large
        // console.log('this.form: '+JSON.stringify(this.form));// undefined
        
        var eventObj = new RightNow.Event.EventObject(
                this, 
            {data: {
                w_id: this.data.info.w_id,
                street: this._street,
                city: this._city,
                state: this._state,
                zip: this._zip,
                country: this._country
            }});

        // Make AJAX request:
        RightNow.Ajax.makeRequest(this.data.attrs.addressverification_ajax_endpoint, eventObj.data,
        {
            successHandler: this.addressVerificationCallback,
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
        this.displayVerificationLines(false, this.data.attrs.ajax_timeout_message, true);
    },
    
    /**
     * Callback function of the validation Ajax request.
     * display the corresponding hint
     * @param {String} verify result
     */
    addressVerificationCallback: function(result) {
        this._hideSpinner();
//        console.log("hid the spinner. result is: "+ result===null?'not null':'null');
//        console.log("addressVerificationCallback. result is: "+ JSON.stringify(result));
        
        if (typeof (result) === 'undefined' || result === null) {
            console.log("err disp "+this.data.attrs.ajax_failure_message);
            this.displayVerificationLines(false, this.data.attrs.ajax_failure_message, true);
            return false;
        }

        if (result.isValid === false) {
            var addtlMessage = result.message ? result.message 
                : ('AddrNotValid '+result.message);

            console.log("err notvalid: "+addtlMessage);
            this.displayVerificationLines(false, addtlMessage, result.isError);
            return false;
        }
        
        // combining the ZIP
        if (result.address.ZIP4 !== null && result.address.ZIP4 !== '' ) {
            result.address.ZIP5 = result.address.ZIP5 +'-'+result.address.ZIP4;
        }
        // saving for later use:
        this._veroAddress = result.address;
        
        var eventObject = this.createEventObject();
        
        console.log('Valid address, displaying "'+result.message+'"' );
        this.displayVerificationLines(true, result.message, result.isError,
             result.address.ADDRESS2 + ', '+result.address.CITY+ ', '+result.address.STATE+', '+result.address.ZIP5);
        RightNow.Event.fire('evt_formFieldValidatePass', eventObject);
        return eventObject;
    },
    
    /**
     * Display the verification lines
     * @param {boolean} Indicates if the serial number is valid
     * @param {String} Validation result message
     * @param {boolean} is Error..
     * @param {String} Additional address to display before the checbox
     */
    displayVerificationLines: function(isValid, message, isError, addtlAddress) {
        addtlAddress = typeof addtlAddress !== 'undefined' ? addtlAddress : '';
           
        this.toggleErrorIndicator(false);
        this._validationResultDisplay.empty();
        this._validationResultDisplay.removeClass('rn_Hidden');
        this._validationResultDisplay.removeClass('rn_InvalidPostalValidationHint');
        this._validationResultDisplay.removeClass('rn_ValidPostalValidationResult');

        this._validationQuestion.removeClass('rn_Hidden');
        this._validationQuestion.removeClass('rn_ValidPostalValidationResult');
        if (isValid === false) {
            if (this.data.js.development_mode) {
               this._validationResultDisplay.insert(message);
            } else {
                 if(isError)
                    this._validationResultDisplay.insert(this.data.attrs.ajax_failure_message);
                 else
                    this._validationResultDisplay.insert(message);
            }

            this._validationResultDisplay.addClass('rn_InvalidPostalValidationHint');
            this._validationQuestion.addClass('rn_Hidden');

            this.toggleErrorIndicator(true);
        } else {
            this._validationResultDisplay.insert(message);
            this._validationResultDisplay.addClass('rn_ValidPostalValidationResult');

            // if address is small, same line, otherwise display underneath.
            if ((message+addtlAddress).length <= 60) {
                this._validationResultDisplay.insert('&nbsp;&nbsp;&nbsp;'+addtlAddress);
            } else {
                this._validationResultDisplay.insert('<br/>'+addtlAddress);
            }
            // hiddenClass for this removed above.
            this._validationQuestion.addClass('rn_ValidPostalValidationResult');            
        }
    },

    /**
     * When you click the 'Do you want to use it?' Checkbox
     */
    _onValidateUseItClick: function(evt) {
        var chkBox = document.getElementById(this.idCheckbox);
        var checked = false;
        if (chkBox)
            checked = chkBox.checked;

        console.log('this._onValidateUseItClick called, checked:'+checked); 
        //console.table(evt);
       
        if (checked) {
            // Setting the fields by following the result JS object:  ZIP5 should be updated;
            document.getElementById(this.idStreet).value = this._veroAddress.ADDRESS2;
            document.getElementById(this.idCity).value = this._veroAddress.CITY;        
            document.getElementById(this.idZip).value = this._veroAddress.ZIP5;

            var ddl = document.getElementById(this.idState); // is drop-down list
            if (typeof ddl.options!== 'undefined') {
                var optslen = ddl.options.length;
                for (var i=0; i<optslen; i++){
                    if (ddl.options[i].text == this._veroAddress.STATE){
                        ddl.selectedIndex = i;    // dunno why doing both 
                        ddl.options[i].selected = true;                
                        break;
                    } else {
                        ddl.options[i].selected = false;
                    }
                }
            } else {
                // if it's a custom field and not a list. 
                ddl.value = this._veroAddress.STATE;
            }
        }
    },

    /**
     * show the spinner
     */
    _showSpinner: function() {
        this._loadingDiv.addClass('rn_PostalValidationLoadingIndicator');
        this._verifyButton.setAttribute('disabled', 'disabled');
        this._validateCheckbox.setAttribute('disabled', 'disabled');
    },
    /**
     * hide the spinner
     */
    _hideSpinner: function() {
        this._loadingDiv.removeClass('rn_PostalValidationLoadingIndicator');
        this._verifyButton.removeAttribute('disabled');
        this._validateCheckbox.removeAttribute('disabled');
    },
    
    /**
     * Gets these elements by name(Field) and retrns the ID of the first one. (always only 1/first)
     */
    getFirstElementIdByName: function(nameAttr) {
        var id = null;
        var elements = document.getElementsByName(nameAttr)
        for(var i=0; i<elements.length; i++) {
            var input = elements[i];
            id = input.id;
            //console.log(i+': '+ input.value+': '+id);
            break;
        }
        return id;
    }
    
});