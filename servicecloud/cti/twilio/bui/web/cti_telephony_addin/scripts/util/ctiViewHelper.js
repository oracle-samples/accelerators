/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set 
 *  published by Oracle under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: Telephony and SMS Accelerator for Twilio
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17D (November 2017)
 *  reference: 161212-000059
 *  date: Monday Oct 30 13:8:16 UTC 2017
 *  revision: rnw-17-11-fixes-releases
 * 
 *  SHA1: $Id: c0d561a67693b4a99784fada662b8396dbde1fc2 $
 * *********************************************************************************************
 *  File: ctiViewHelper.js
 * ****************************************************************************************** */
define(["require", "exports", "jquery", "./ctiConstants", "./ctiMessages"], function (require, exports, $, ctiConstants_1, ctiMessages_1) {
    "use strict";
    exports.__esModule = true;
    /**
     * CtiViewHelper - Does all dynamic rendering of the UI
     *
     */
    var CtiViewHelper = /** @class */ (function () {
        function CtiViewHelper() {
        }
        //private static isOutbound: boolean = false;
        CtiViewHelper.renderIncomingView = function (ctiData, audio) {
            //Clear Contact Mail
            var incomingMailElement = $("#incoming_contact_mail");
            var incomingNameElement = $("#incoming_name");
            var incomingNumberElement = $("#incoming_number");
            var acceptButton = $('#notif_accept_btn');
            var rejectButton = $('#notif_reject_btn');
            acceptButton.attr('class', "button button-green");
            rejectButton.attr('class', "button button-red");
            incomingMailElement.html('');
            incomingNameElement.html(ctiData.contact.name);
            incomingNameElement.attr('title', ctiData.contact.name);
            incomingNumberElement.html(ctiData.contact.phone);
            incomingNumberElement.attr('title', ctiData.contact.phone);
            if (ctiData.contact.name !== ctiConstants_1.CtiConstants.UNKNOWN_CALLER) {
                var html = "<i class='fa fa-envelope-o'></i> <span class='contact_email'>" + ctiData.contact.email + "</span>";
                incomingMailElement.html(html);
                incomingMailElement.attr('title', ctiData.contact.email);
            }
            $("#incoming_contact_image").attr('src', ctiData.contact.dp + '&randomId=' + CtiViewHelper.getRandomId());
            acceptButton.off().on("click", function (event) {
                acceptButton.attr('class', "button button-green disabled");
                ctiData.accept();
                audio.pause();
                audio.currentTime = 0;
            });
            rejectButton.off().on("click", function (event) {
                rejectButton.attr('class', "button button-red disabled");
                ctiData.reject();
                $("#incomingdialog").hide();
                $("#dialer").show();
                audio.pause();
                audio.currentTime = 0;
            });
            document.getElementById('dialer').style.display = 'none';
            document.getElementById('oncalldialog').style.display = 'none';
            document.getElementById('incomingdialog').style.display = 'block';
            audio.currentTime = 0;
            audio.play();
        };
        CtiViewHelper.showOnCallUI = function (phone, displayPic) {
            $("#incomingdialog").hide();
            $("#dialer").hide();
            var onCallMailElement = $("#oncall_contact_mail");
            var onCallNameElement = $("#oncall_name");
            var onCallNumberElement = $("#oncall_number");
            var onCallImageElement = $("#oncall_contact_image");
            onCallImageElement.attr('src', displayPic + '&randomId=' + CtiViewHelper.getRandomId());
            onCallMailElement.html('');
            // Prepare the dialog
            onCallNameElement.html('Searching contact..');
            onCallNameElement.attr('title', 'Searching contact..');
            onCallNumberElement.html(phone);
            onCallNumberElement.attr('title', 'phone');
            var onCallMuteButton = $("#oncall_mute_btn");
            onCallMuteButton.attr('class', 'button button-blue disabled');
            var transferIconElement = $('#oncall_transfer_icon');
            transferIconElement.attr('class', 'fa fa-angle-double-right');
            $("#oncall_mute_btn>i").attr('class', 'fa fa-microphone');
            $("#oncalldialog").show();
            CtiViewHelper.disableOnCallControls();
        };
        CtiViewHelper.renderOnCallView = function (ctiData, audio, transferButtonListener) {
            //TODO - Removed commented out code
            //CtiViewHelper.isOutbound = isOutbound;
            audio.currentTime = 0;
            $("#incomingdialog").hide();
            $("#dialer").hide();
            var onCallMailElement = $("#oncall_contact_mail");
            var onCallNameElement = $("#oncall_name");
            var onCallNumberElement = $("#oncall_number");
            var onCallMuteIcon = $("#oncall_mute_btn>i");
            onCallMuteIcon.attr('class', 'fa fa-microphone');
            var onCallMuteButton = $("#oncall_mute_btn");
            onCallMuteButton.attr('class', 'button button-blue');
            var transferButtonElement = $('#oncall_transfer_btn');
            var transferIconElement = $('#oncall_transfer_icon');
            transferIconElement.attr('class', 'fa fa-angle-double-right');
            //Clear Contact Mail
            onCallMailElement.html('');
            // Prepare the dialog
            onCallNameElement.html(ctiData.contact.name);
            onCallNameElement.attr('title', ctiData.contact.name);
            onCallNumberElement.html(ctiData.contact.phone);
            onCallNumberElement.attr('title', ctiData.contact.phone);
            if (ctiData.contact.name !== ctiConstants_1.CtiConstants.UNKNOWN_CALLER) {
                var html = "<i class='fa fa-envelope-o'></i> <span class='contact_email'>" + ctiData.contact.email + "</span>";
                onCallMailElement.html(html);
                onCallMailElement.attr('title', ctiData.contact.email);
            }
            $("#oncall_contact_image").attr('src', ctiData.contact.dp + '&randomId=' + CtiViewHelper.getRandomId());
            $("#oncall_hangup_btn").off().on("click", function (event) {
                ctiData.hangup();
            });
            //Handle Mute
            onCallMuteButton.off().on("click", function (event) {
                event.preventDefault();
                var onCallDialogElement = $("#oncalldialog");
                var muted = onCallMuteIcon.hasClass("fa fa-microphone-slash");
                ctiData.mute(!muted);
                if (muted) {
                    onCallMuteIcon.removeClass().addClass("fa fa-microphone");
                }
                else {
                    onCallMuteIcon.removeClass().addClass("fa fa-microphone-slash");
                }
            });
            //Call transfer search button
            /* if(isOutbound) {
               $('#oncall_transfer_btn').attr('class', 'collapse');
             }else{*/
            transferButtonElement.off('click').on('click', function (event) {
                var isListed = transferIconElement.hasClass('fa-angle-double-down');
                if (isListed) {
                    //Already listed toggle - to hidden
                    transferIconElement.attr('class', 'fa fa-angle-double-right');
                    $("#cti_connected_agents").hide('slide');
                }
                else {
                    transferButtonListener();
                    transferIconElement.attr('class', 'fa fa-angle-double-down');
                    $("#cti_connected_agents").show('slide');
                }
            });
            transferIconElement.attr('class', 'fa fa-angle-double-right');
            transferButtonElement.attr('class', 'button button-blue disabled');
            // }
            $("#oncalldialog").show();
            CtiViewHelper.disableOnCallControls();
        };
        CtiViewHelper.showTransferButton = function (id) {
            $('#' + id).attr('class', 'pull-right transfer-call-div button button-red');
        };
        CtiViewHelper.hideTransferButton = function (id) {
            $('#' + id).attr('class', 'collapse');
        };
        CtiViewHelper.renderAgentList = function (agentList, clickHandler) {
            var agentListElement = $('#agent_list');
            if (agentList && agentList.length === 0) {
                agentListElement.html(CtiViewHelper.getEmptyAgentListUI());
            }
            else if (agentList) {
                agentListElement.html(CtiViewHelper.getAgentListUI(agentList));
                agentListElement.off('click').on('click', '.transfer-call-div', function (event) {
                    if (event && event.currentTarget && event.currentTarget.id) {
                        clickHandler(event.currentTarget.id, event.currentTarget.getAttribute('agent_name'));
                        //$('#'+event.currentTarget.id).attr('class', 'pull-right transfer-call-div button button-red disabled');
                    }
                });
            }
        };
        CtiViewHelper.renderAgentSearchUI = function () {
            var agentSearchUIText = '<div class="agent-search" >' +
                'Searching agents...<br>' +
                '<i class="fa fa-spinner fa-pulse fa-2x fa-fw" ></i>' +
                '</div>';
            $('#agent_list').html(agentSearchUIText);
        };
        CtiViewHelper.getEmptyAgentListUI = function () {
            return '<div class=agent-search>' +
                ctiMessages_1.CtiMessages.MESSAGE_NO_ONLINE_AGENTS + '</div>';
        };
        CtiViewHelper.getAgentListUI = function (agentList) {
            var agentListUI = '';
            for (var _i = 0, agentList_1 = agentList; _i < agentList_1.length; _i++) {
                var agentData = agentList_1[_i];
                var eMail = agentData.email ? agentData.email : ctiMessages_1.CtiMessages.MESSAGE_MAIL_NOT_AVAILABLE;
                agentListUI += '<div class="agent-data" >' +
                    '<img src="' + agentData.dp + '&randomId=' + CtiViewHelper.getRandomId() + '" >' +
                    '<div class="agent-name">' +
                    '<span title="' + agentData.name + '">' + agentData.name + '</span><br> ' +
                    '<span title="' + agentData.email + '">' + eMail + '</span> ' +
                    '</div>' +
                    '<div id="' + agentData.worker + '" agent_name="' + agentData.name + '" class="collapse pull-right transfer-call-div button button-red" >' +
                    '<i class="fa fa-share-square"></i> ' +
                    '</div>' +
                    '</div>';
            }
            return agentListUI;
        };
        CtiViewHelper.renderCallDisconnectView = function () {
            $("#oncalldialog").hide();
            $("#cti_connected_agents").hide();
            $('#bui_cti_dialer_number').val('');
            $("#dialer").show();
        };
        CtiViewHelper.renderCallCancelledView = function () {
            $("#incomingdialog").hide();
            $("#dialer").show();
        };
        CtiViewHelper.renderCallTimeOutView = function () {
            $("#incomingdialog").hide("fade");
            $("#dialer").show();
        };
        CtiViewHelper.enableOnCallControls = function () {
            var transferButton = $('#oncall_transfer_btn');
            transferButton.attr('class', 'button button-blue');
            transferButton.attr('disabled', '');
            var hangupButton = $('#oncall_hangup_btn');
            hangupButton.attr('class', 'button button-red pull-right');
            hangupButton.attr('disabled', '');
        };
        CtiViewHelper.disableOnCallControls = function () {
            var transferButton = $('#oncall_transfer_btn');
            transferButton.attr('class', 'button button-blue disabled');
            transferButton.attr('disabled', 'disabled');
            var hangupButton = $('#oncall_hangup_btn');
            hangupButton.attr('class', 'button button-red pull-right disabled');
            hangupButton.attr('disabled', 'disabled');
        };
        CtiViewHelper.addDialKey = function (dialKey) {
            var id = dialKey;
            var idHash = {
                "+": 'plus',
                "#": 'hash'
            };
            if (isNaN(dialKey)) {
                id = idHash[dialKey];
            }
            $("#dialpad_" + id).off().on("click", function (event) { return CtiViewHelper.appendDigitAtEnd(dialKey); });
        };
        CtiViewHelper.addDialPadControls = function (outgoingHandler) {
            var dialerInputElement = $('#bui_cti_dialer_number');
            dialerInputElement.off('keypress').on('keypress', function (event) {
                var charCode = event.charCode;
                return (charCode >= 48 && charCode <= 57) || charCode === 43 || charCode === 35;
            });
            dialerInputElement.off('blur').on('blur', function (event) {
                var inputValue = dialerInputElement.val() + '';
                var validInput = [];
                var validIndex = 0;
                for (var i = 0; i < inputValue.length; i++) {
                    var charCode = inputValue.charCodeAt(i);
                    if ((charCode >= 48 && charCode <= 57) || charCode === 43 || charCode === 35) {
                        validInput[validIndex] = inputValue.charAt(i);
                        validIndex++;
                    }
                }
                if (inputValue.length !== validInput.length) {
                    $('#bui_cti_dialer_number').val(validInput.join(''));
                }
            });
            var dialerKey = '123456789+0#';
            for (var id = 0; id < dialerKey.length; id++) {
                CtiViewHelper.addDialKey(dialerKey[id]);
            }
            $("#dialpad_remove_digit").off().on("click", function (event) {
                CtiViewHelper.removeDigitFromEnd();
            });
            $("#dialpad_make_call").off().on("click", outgoingHandler);
        };
        CtiViewHelper.renderOutgoingUI = function (event, outgoingHangupHandler) {
            var outNumber = $('#bui_cti_dialer_number').val() + '';
            if (outNumber) {
                var outgoingElement = $('#out_going_number');
                $('#dialer').hide('slide');
                outgoingElement.html(outNumber);
                outgoingElement.attr('title', outNumber);
                $('#outgoingdialog').show('slide');
                $("#end_outgoing_btn").off().on("click", outgoingHangupHandler);
            }
        };
        ;
        CtiViewHelper.renderOutgoingHangupUI = function (event) {
            $('#outgoingdialog').hide('slide');
            $('#bui_cti_dialer_number').val('');
            $('#dialer').show('slide');
        };
        CtiViewHelper.updateOutgoingContactDetails = function (contactName, displayPic) {
            if (contactName) {
                $('#oncall_name').html(contactName);
            }
            if (displayPic) {
                $('#oncall_contact_image').attr('src', displayPic + '&randomId=' + CtiViewHelper.getRandomId());
            }
        };
        CtiViewHelper.getOutgoingContactNumber = function () {
            return $('#bui_cti_dialer_number').val() + '';
        };
        CtiViewHelper.appendDigitAtEnd = function (digit) {
            var inputElement = $('#bui_cti_dialer_number');
            var numberPlaced = inputElement.val() + '';
            var selectionStart = inputElement[0].selectionStart;
            var selectionEnd = inputElement[0].selectionEnd;
            numberPlaced = numberPlaced.substring(0, selectionStart) + digit + numberPlaced.substring(selectionEnd);
            inputElement.val(numberPlaced);
            CtiViewHelper.correctCursorAfterValueChange(inputElement[0], selectionStart, selectionEnd, numberPlaced.length, true);
        };
        CtiViewHelper.removeDigitFromEnd = function () {
            var inputElement = $('#bui_cti_dialer_number');
            var numberPlaced = inputElement.val() + '';
            var selectionStart = inputElement[0].selectionStart;
            var selectionEnd = inputElement[0].selectionEnd;
            if (selectionEnd === selectionStart) {
                numberPlaced = numberPlaced.substring(0, selectionStart - 1) + numberPlaced.substring(selectionEnd);
            }
            else {
                numberPlaced = numberPlaced.substring(0, selectionStart) + numberPlaced.substring(selectionEnd);
            }
            inputElement.val(numberPlaced);
            CtiViewHelper.correctCursorAfterValueChange(inputElement[0], selectionStart, selectionEnd, numberPlaced.length, false);
        };
        CtiViewHelper.correctCursorAfterValueChange = function (element, selectionStart, selectionEnd, inputLength, isForward) {
            //Set focus to the previous/next character char
            var index = selectionStart;
            if (isForward) {
                if (index < inputLength) {
                    index++;
                }
            }
            else {
                if (selectionStart === selectionEnd && index > 0) {
                    index--;
                }
            }
            element.focus();
            if (element.setSelectionRange) {
                element.setSelectionRange(index, index);
            }
            else {
                if (element.createTextRange) {
                    var range = element.createTextRange();
                    range.collapse(true);
                    range.moveEnd('character', index);
                    range.moveStart('character', index);
                    range.select();
                }
            }
        };
        CtiViewHelper.getRandomId = function () {
            return new Date().valueOf() + '_';
        };
        CtiViewHelper.selection = {};
        return CtiViewHelper;
    }());
    exports.CtiViewHelper = CtiViewHelper;
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY3RpVmlld0hlbHBlci5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbImN0aVZpZXdIZWxwZXIudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7Ozs7O2dHQUtnRzs7OztJQU9oRzs7O09BR0c7SUFDSDtRQUFBO1FBcVlBLENBQUM7UUFsWUMsNkNBQTZDO1FBRS9CLGdDQUFrQixHQUFoQyxVQUFpQyxPQUFXLEVBQUUsS0FBUztZQUNyRCxvQkFBb0I7WUFDcEIsSUFBSSxtQkFBbUIsR0FBTyxDQUFDLENBQUMsd0JBQXdCLENBQUMsQ0FBQztZQUMxRCxJQUFJLG1CQUFtQixHQUFPLENBQUMsQ0FBQyxnQkFBZ0IsQ0FBQyxDQUFDO1lBQ2xELElBQUkscUJBQXFCLEdBQU8sQ0FBQyxDQUFDLGtCQUFrQixDQUFDLENBQUM7WUFDdEQsSUFBSSxZQUFZLEdBQUcsQ0FBQyxDQUFDLG1CQUFtQixDQUFDLENBQUM7WUFDMUMsSUFBSSxZQUFZLEdBQUcsQ0FBQyxDQUFDLG1CQUFtQixDQUFDLENBQUM7WUFDMUMsWUFBWSxDQUFDLElBQUksQ0FBQyxPQUFPLEVBQUUscUJBQXFCLENBQUMsQ0FBQztZQUNsRCxZQUFZLENBQUMsSUFBSSxDQUFDLE9BQU8sRUFBRSxtQkFBbUIsQ0FBQyxDQUFDO1lBRWhELG1CQUFtQixDQUFDLElBQUksQ0FBQyxFQUFFLENBQUMsQ0FBQztZQUU3QixtQkFBbUIsQ0FBQyxJQUFJLENBQUMsT0FBTyxDQUFDLE9BQU8sQ0FBQyxJQUFJLENBQUMsQ0FBQztZQUMvQyxtQkFBbUIsQ0FBQyxJQUFJLENBQUMsT0FBTyxFQUFFLE9BQU8sQ0FBQyxPQUFPLENBQUMsSUFBSSxDQUFDLENBQUM7WUFFeEQscUJBQXFCLENBQUMsSUFBSSxDQUFDLE9BQU8sQ0FBQyxPQUFPLENBQUMsS0FBSyxDQUFDLENBQUM7WUFDbEQscUJBQXFCLENBQUMsSUFBSSxDQUFDLE9BQU8sRUFBRSxPQUFPLENBQUMsT0FBTyxDQUFDLEtBQUssQ0FBQyxDQUFDO1lBRTNE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O1lBQ0gsQ0FBQztZQUFDLElBQUksQ0FBQyxDQUFDO2dCQUNOLEVBQUUsQ0FBQyxDQUFDLGNBQWMsS0FBSyxZQUFZLElBQUksS0FBSyxHQUFHLENBQUMsQ0FBQyxDQUFDLENBQUM7b0JBQ2pELEtBQUssRUFBRSxDQUFDO2dCQUNWLENBQUM7WUFDSCxDQUFDO1lBQ0QsT0FBTyxDQUFDLEtBQUssRUFBRSxDQUFDO1lBQ2hCLEVBQUUsQ0FBQyxDQUFDLE9BQU8sQ0FBQyxpQkFBaUIsQ0FBQyxDQUFDLENBQUM7Z0JBQzlCLE9BQU8sQ0FBQyxpQkFBaUIsQ0FBQyxLQUFLLEVBQUUsS0FBSyxDQUFDLENBQUM7WUFDMUMsQ0FBQztZQUFDLElBQUksQ0FBQyxDQUFDO2dCQUNOLEVBQUUsQ0FBQyxDQUFDLE9BQU8sQ0FBQyxlQUFlLENBQUMsQ0FBQyxDQUFDO29CQUM1QixJQUFJLEtBQUssR0FBRyxPQUFPLENBQUMsZUFBZSxFQUFFLENBQUM7b0JBQ3RDLEtBQUssQ0FBQyxRQUFRLENBQUMsSUFBSSxDQUFDLENBQUM7b0JBQ3JCLEtBQUssQ0FBQyxPQUFPLENBQUMsV0FBVyxFQUFFLEtBQUssQ0FBQyxDQUFDO29CQUNsQyxLQUFLLENBQUMsU0FBUyxDQUFDLFdBQVcsRUFBRSxLQUFLLENBQUMsQ0FBQztvQkFDcEMsS0FBSyxDQUFDLE1BQU0sRUFBRSxDQUFDO2dCQUNqQixDQUFDO1lBQ0gsQ0FBQztRQUNILENBQUM7UUFFYSx5QkFBVyxHQUF6QjtZQUNFLE1BQU0sQ0FBQyxJQUFJLElBQUksRUFBRSxDQUFDLE9BQU8sRUFBRSxHQUFDLEdBQUcsQ0FBQztRQUNsQyxDQUFDO1FBbFljLHVCQUFTLEdBQU8sRUFBRSxDQUFDO1FBbVlwQyxvQkFBQztLQUFBLEFBcllELElBcVlDO0lBcllZLHNDQUFhIiwic291cmNlc0NvbnRlbnQiOlsiLyogKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXG4gKiAgJEFDQ0VMRVJBVE9SX0hFQURFUl9QTEFDRV9IT0xERVIkXG4gKiAgU0hBMTogJElkOiBjMGQ1NjFhNjc2OTNiNGE5OTc4NGZhZGE2NjJiODM5NmRiZGUxZmMyICRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogIEZpbGU6ICRBQ0NFTEVSQVRPUl9IRUFERVJfRklMRV9OQU1FX1BMQUNFX0hPTERFUiRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiAqL1xuXG5pbXBvcnQgJCA9IHJlcXVpcmUoJ2pxdWVyeScpO1xuaW1wb3J0IHtDdGlDb25zdGFudHN9IGZyb20gJy4vY3RpQ29uc3RhbnRzJztcbmltcG9ydCB7QWdlbnREYXRhfSBmcm9tIFwiLi4vbW9kZWwvYWdlbnREYXRhXCI7XG5pbXBvcnQge0N0aU1lc3NhZ2VzfSBmcm9tIFwiLi9jdGlNZXNzYWdlc1wiO1xuXG4vKipcbiAqIEN0aVZpZXdIZWxwZXIgLSBEb2VzIGFsbCBkeW5hbWljIHJlbmRlcmluZyBvZiB0aGUgVUlcbiAqXG4gKi9cbmV4cG9ydCBjbGFzcyBDdGlWaWV3SGVscGVyIHtcblxuICBwcml2YXRlIHN0YXRpYyBzZWxlY3Rpb246YW55ID0ge307XG4gIC8vcHJpdmF0ZSBzdGF0aWMgaXNPdXRib3VuZDogYm9vbGVhbiA9IGZhbHNlO1xuXG4gIHB1YmxpYyBzdGF0aWMgcmVuZGVySW5jb21pbmdWaWV3KGN0aURhdGE6YW55LCBhdWRpbzphbnkpOnZvaWQge1xuICAgIC8vQ2xlYXIgQ29udGFjdCBNYWlsXG4gICAgdmFyIGluY29taW5nTWFpbEVsZW1lbnQ6YW55ID0gJChcIiNpbmNvbWluZ19jb250YWN0X21haWxcIik7XG4gICAgdmFyIGluY29taW5nTmFtZUVsZW1lbnQ6YW55ID0gJChcIiNpbmNvbWluZ19uYW1lXCIpO1xuICAgIHZhciBpbmNvbWluZ051bWJlckVsZW1lbnQ6YW55ID0gJChcIiNpbmNvbWluZ19udW1iZXJcIik7XG4gICAgdmFyIGFjY2VwdEJ1dHRvbiA9ICQoJyNub3RpZl9hY2NlcHRfYnRuJyk7XG4gICAgdmFyIHJlamVjdEJ1dHRvbiA9ICQoJyNub3RpZl9yZWplY3RfYnRuJyk7XG4gICAgYWNjZXB0QnV0dG9uLmF0dHIoJ2NsYXNzJywgXCJidXR0b24gYnV0dG9uLWdyZWVuXCIpO1xuICAgIHJlamVjdEJ1dHRvbi5hdHRyKCdjbGFzcycsIFwiYnV0dG9uIGJ1dHRvbi1yZWRcIik7XG5cbiAgICBpbmNvbWluZ01haWxFbGVtZW50Lmh0bWwoJycpO1xuXG4gICAgaW5jb21pbmdOYW1lRWxlbWVudC5odG1sKGN0aURhdGEuY29udGFjdC5uYW1lKTtcbiAgICBpbmNvbWluZ05hbWVFbGVtZW50LmF0dHIoJ3RpdGxlJywgY3RpRGF0YS5jb250YWN0Lm5hbWUpO1xuXG4gICAgaW5jb21pbmdOdW1iZXJFbGVtZW50Lmh0bWwoY3RpRGF0YS5jb250YWN0LnBob25lKTtcbiAgICBpbmNvbWluZ051bWJlckVsZW1lbnQuYXR0cigndGl0bGUnLCBjdGlEYXRhLmNvbnRhY3QucGhvbmUpO1xuXG4gICAgaWYgKGN0aURhdGEuY29udGFjdC5uYW1lICE9PSBDdGlDb25zdGFudHMuVU5LTk9XTl9DQUxMRVIpIHtcbiAgICAgIHZhciBodG1sID0gXCI8aSBjbGFzcz0nZmEgZmEtZW52ZWxvcGUtbyc+PC9pPiA8c3BhbiBjbGFzcz0nY29udGFjdF9lbWFpbCc+XCIgKyBjdGlEYXRhLmNvbnRhY3QuZW1haWwgKyBcIjwvc3Bhbj5cIjtcbiAgICAgIGluY29taW5nTWFpbEVsZW1lbnQuaHRtbChodG1sKTtcbiAgICAgIGluY29taW5nTWFpbEVsZW1lbnQuYXR0cigndGl0bGUnLCBjdGlEYXRhLmNvbnRhY3QuZW1haWwpO1xuICAgIH1cbiAgICAkKFwiI2luY29taW5nX2NvbnRhY3RfaW1hZ2VcIikuYXR0cignc3JjJywgY3RpRGF0YS5jb250YWN0LmRwICsgJyZyYW5kb21JZD0nICtDdGlWaWV3SGVscGVyLmdldFJhbmRvbUlkKCkpO1xuXG4gICAgYWNjZXB0QnV0dG9uLm9mZigpLm9uKFwiY2xpY2tcIiwgKGV2ZW50KSA9PiB7XG4gICAgICBhY2NlcHRCdXR0b24uYXR0cignY2xhc3MnLCBcImJ1dHRvbiBidXR0b24tZ3JlZW4gZGlzYWJsZWRcIik7XG4gICAgICBjdGlEYXRhLmFjY2VwdCgpO1xuICAgICAgYXVkaW8ucGF1c2UoKTtcbiAgICAgIGF1ZGlvLmN1cnJlbnRUaW1lID0gMDtcbiAgICB9KTtcbiAgICByZWplY3RCdXR0b24ub2ZmKCkub24oXCJjbGlja1wiLCAoZXZlbnQpID0+IHtcbiAgICAgIHJlamVjdEJ1dHRvbi5hdHRyKCdjbGFzcycsIFwiYnV0dG9uIGJ1dHRvbi1yZWQgZGlzYWJsZWRcIik7XG4gICAgICBjdGlEYXRhLnJlamVjdCgpO1xuICAgICAgJChcIiNpbmNvbWluZ2RpYWxvZ1wiKS5oaWRlKCk7XG4gICAgICAkKFwiI2RpYWxlclwiKS5zaG93KCk7XG4gICAgICBhdWRpby5wYXVzZSgpO1xuICAgICAgYXVkaW8uY3VycmVudFRpbWUgPSAwO1xuICAgIH0pO1xuXG4gICAgZG9jdW1lbnQuZ2V0RWxlbWVudEJ5SWQoJ2RpYWxlcicpLnN0eWxlLmRpc3BsYXkgPSAnbm9uZSc7XG4gICAgZG9jdW1lbnQuZ2V0RWxlbWVudEJ5SWQoJ29uY2FsbGRpYWxvZycpLnN0eWxlLmRpc3BsYXkgPSAnbm9uZSc7XG4gICAgZG9jdW1lbnQuZ2V0RWxlbWVudEJ5SWQoJ2luY29taW5nZGlhbG9nJykuc3R5bGUuZGlzcGxheSA9ICdibG9jayc7XG5cbiAgICBhdWRpby5jdXJyZW50VGltZSA9IDA7XG4gICAgYXVkaW8ucGxheSgpO1xuICB9XG5cbiAgcHVibGljIHN0YXRpYyBzaG93T25DYWxsVUkocGhvbmU6IHN0cmluZywgZGlzcGxheVBpYzogc3RyaW5nKTogdm9pZCB7XG4gICAgJChcIiNpbmNvbWluZ2RpYWxvZ1wiKS5oaWRlKCk7XG4gICAgJChcIiNkaWFsZXJcIikuaGlkZSgpO1xuXG4gICAgdmFyIG9uQ2FsbE1haWxFbGVtZW50OmFueSA9ICQoXCIjb25jYWxsX2NvbnRhY3RfbWFpbFwiKTtcbiAgICB2YXIgb25DYWxsTmFtZUVsZW1lbnQ6YW55ID0gJChcIiNvbmNhbGxfbmFtZVwiKTtcbiAgICB2YXIgb25DYWxsTnVtYmVyRWxlbWVudDphbnkgPSAkKFwiI29uY2FsbF9udW1iZXJcIik7XG4gICAgdmFyIG9uQ2FsbEltYWdlRWxlbWVudCA9ICQoXCIjb25jYWxsX2NvbnRhY3RfaW1hZ2VcIik7XG5cbiAgICBvbkNhbGxJbWFnZUVsZW1lbnQuYXR0cignc3JjJywgZGlzcGxheVBpYyArICcmcmFuZG9tSWQ9JyArQ3RpVmlld0hlbHBlci5nZXRSYW5kb21JZCgpKTtcblxuICAgIG9uQ2FsbE1haWxFbGVtZW50Lmh0bWwoJycpO1xuXG4gICAgLy8gUHJlcGFyZSB0aGUgZGlhbG9nXG4gICAgb25DYWxsTmFtZUVsZW1lbnQuaHRtbCgnU2VhcmNoaW5nIGNvbnRhY3QuLicpO1xuICAgIG9uQ2FsbE5hbWVFbGVtZW50LmF0dHIoJ3RpdGxlJywgJ1NlYXJjaGluZyBjb250YWN0Li4nKTtcbiAgICBvbkNhbGxOdW1iZXJFbGVtZW50Lmh0bWwocGhvbmUpO1xuICAgIG9uQ2FsbE51bWJlckVsZW1lbnQuYXR0cigndGl0bGUnLCAncGhvbmUnKTtcbiAgICB2YXIgb25DYWxsTXV0ZUJ1dHRvbjphbnkgPSAkKFwiI29uY2FsbF9tdXRlX2J0blwiKTtcbiAgICBvbkNhbGxNdXRlQnV0dG9uLmF0dHIoJ2NsYXNzJywgJ2J1dHRvbiBidXR0b24tYmx1ZSBkaXNhYmxlZCcpO1xuICAgIHZhciB0cmFuc2Zlckljb25FbGVtZW50OiBhbnkgPSAgJCgnI29uY2FsbF90cmFuc2Zlcl9pY29uJyk7XG4gICAgdHJhbnNmZXJJY29uRWxlbWVudC5hdHRyKCdjbGFzcycsICdmYSBmYS1hbmdsZS1kb3VibGUtcmlnaHQnKTtcbiAgICAkKFwiI29uY2FsbF9tdXRlX2J0bj5pXCIpLmF0dHIoJ2NsYXNzJywgJ2ZhIGZhLW1pY3JvcGhvbmUnKTtcbiAgICAkKFwiI29uY2FsbGRpYWxvZ1wiKS5zaG93KCk7XG4gICAgQ3RpVmlld0hlbHBlci5kaXNhYmxlT25DYWxsQ29udHJvbHMoKTtcbiAgfVxuXG4gIHB1YmxpYyBzdGF0aWMgcmVuZGVyT25DYWxsVmlldyhjdGlEYXRhOmFueSwgYXVkaW86YW55LCB0cmFuc2ZlckJ1dHRvbkxpc3RlbmVyOiBhbnkpOnZvaWQge1xuICAgIC8vVE9ETyAtIFJlbW92ZWQgY29tbWVudGVkIG91dCBjb2RlXG4gICAgLy9DdGlWaWV3SGVscGVyLmlzT3V0Ym91bmQgPSBpc091dGJvdW5kO1xuICAgIGF1ZGlvLmN1cnJlbnRUaW1lID0gMDtcbiAgICAkKFwiI2luY29taW5nZGlhbG9nXCIpLmhpZGUoKTtcbiAgICAkKFwiI2RpYWxlclwiKS5oaWRlKCk7XG4gICAgdmFyIG9uQ2FsbE1haWxFbGVtZW50OmFueSA9ICQoXCIjb25jYWxsX2NvbnRhY3RfbWFpbFwiKTtcbiAgICB2YXIgb25DYWxsTmFtZUVsZW1lbnQ6YW55ID0gJChcIiNvbmNhbGxfbmFtZVwiKTtcbiAgICB2YXIgb25DYWxsTnVtYmVyRWxlbWVudDphbnkgPSAkKFwiI29uY2FsbF9udW1iZXJcIik7XG4gICAgdmFyIG9uQ2FsbE11dGVJY29uOmFueSA9ICQoXCIjb25jYWxsX211dGVfYnRuPmlcIik7XG4gICAgb25DYWxsTXV0ZUljb24uYXR0cignY2xhc3MnLCAnZmEgZmEtbWljcm9waG9uZScpO1xuICAgIHZhciBvbkNhbGxNdXRlQnV0dG9uOmFueSA9ICQoXCIjb25jYWxsX211dGVfYnRuXCIpO1xuICAgIG9uQ2FsbE11dGVCdXR0b24uYXR0cignY2xhc3MnLCAnYnV0dG9uIGJ1dHRvbi1ibHVlJyk7XG4gICAgdmFyIHRyYW5zZmVyQnV0dG9uRWxlbWVudDogYW55ID0gJCgnI29uY2FsbF90cmFuc2Zlcl9idG4nKTtcbiAgICB2YXIgdHJhbnNmZXJJY29uRWxlbWVudDogYW55ID0gICQoJyNvbmNhbGxfdHJhbnNmZXJfaWNvbicpO1xuICAgIHRyYW5zZmVySWNvbkVsZW1lbnQuYXR0cignY2xhc3MnLCAnZmEgZmEtYW5nbGUtZG91YmxlLXJpZ2h0Jyk7XG5cbiAgICAvL0NsZWFyIENvbnRhY3QgTWFpbFxuICAgIG9uQ2FsbE1haWxFbGVtZW50Lmh0bWwoJycpO1xuXG4gICAgLy8gUHJlcGFyZSB0aGUgZGlhbG9nXG4gICAgb25DYWxsTmFtZUVsZW1lbnQuaHRtbChjdGlEYXRhLmNvbnRhY3QubmFtZSk7XG4gICAgb25DYWxsTmFtZUVsZW1lbnQuYXR0cigndGl0bGUnLCBjdGlEYXRhLmNvbnRhY3QubmFtZSk7XG5cbiAgICBvbkNhbGxOdW1iZXJFbGVtZW50Lmh0bWwoY3RpRGF0YS5jb250YWN0LnBob25lKTtcbiAgICBvbkNhbGxOdW1iZXJFbGVtZW50LmF0dHIoJ3RpdGxlJywgY3RpRGF0YS5jb250YWN0LnBob25lKTtcblxuICAgIGlmIChjdGlEYXRhLmNvbnRhY3QubmFtZSAhPT0gQ3RpQ29uc3RhbnRzLlVOS05PV05fQ0FMTEVSKSB7XG4gICAgICB2YXIgaHRtbCA9IFwiPGkgY2xhc3M9J2ZhIGZhLWVudmVsb3BlLW8nPjwvaT4gPHNwYW4gY2xhc3M9J2NvbnRhY3RfZW1haWwnPlwiICsgY3RpRGF0YS5jb250YWN0LmVtYWlsICsgXCI8L3NwYW4+XCI7XG4gICAgICBvbkNhbGxNYWlsRWxlbWVudC5odG1sKGh0bWwpO1xuICAgICAgb25DYWxsTWFpbEVsZW1lbnQuYXR0cigndGl0bGUnLCBjdGlEYXRhLmNvbnRhY3QuZW1haWwpO1xuICAgIH1cbiAgICAkKFwiI29uY2FsbF9jb250YWN0X2ltYWdlXCIpLmF0dHIoJ3NyYycsIGN0aURhdGEuY29udGFjdC5kcCArICcmcmFuZG9tSWQ9JyArQ3RpVmlld0hlbHBlci5nZXRSYW5kb21JZCgpKTtcblxuICAgICQoXCIjb25jYWxsX2hhbmd1cF9idG5cIikub2ZmKCkub24oXCJjbGlja1wiLCAoZXZlbnQpID0+IHtcbiAgICAgIGN0aURhdGEuaGFuZ3VwKCk7XG4gICAgfSk7XG5cbiAgICAvL0hhbmRsZSBNdXRlXG4gICAgb25DYWxsTXV0ZUJ1dHRvbi5vZmYoKS5vbihcImNsaWNrXCIsIChldmVudCkgPT4ge1xuICAgICAgZXZlbnQucHJldmVudERlZmF1bHQoKTtcbiAgICAgIHZhciBvbkNhbGxEaWFsb2dFbGVtZW50OmFueSA9ICQoXCIjb25jYWxsZGlhbG9nXCIpO1xuICAgICAgdmFyIG11dGVkID0gb25DYWxsTXV0ZUljb24uaGFzQ2xhc3MoXCJmYSBmYS1taWNyb3Bob25lLXNsYXNoXCIpO1xuICAgICAgXG4gICAgICBjdGlEYXRhLm11dGUoIW11dGVkKTtcblxuICAgICAgaWYgKG11dGVkKSB7XG4gICAgICAgIG9uQ2FsbE11dGVJY29uLnJlbW92ZUNsYXNzKCkuYWRkQ2xhc3MoXCJmYSBmYS1taWNyb3Bob25lXCIpO1xuICAgICAgfSBlbHNlIHtcbiAgICAgICAgb25DYWxsTXV0ZUljb24ucmVtb3ZlQ2xhc3MoKS5hZGRDbGFzcyhcImZhIGZhLW1pY3JvcGhvbmUtc2xhc2hcIik7XG4gICAgICB9XG5cbiAgICB9KTtcblxuICAgIC8vQ2FsbCB0cmFuc2ZlciBzZWFyY2ggYnV0dG9uXG4gICAvKiBpZihpc091dGJvdW5kKSB7XG4gICAgICAkKCcjb25jYWxsX3RyYW5zZmVyX2J0bicpLmF0dHIoJ2NsYXNzJywgJ2NvbGxhcHNlJyk7XG4gICAgfWVsc2V7Ki9cblxuICAgICAgdHJhbnNmZXJCdXR0b25FbGVtZW50Lm9mZignY2xpY2snKS5vbignY2xpY2snLCAoZXZlbnQ6IGFueSkgPT4ge1xuXG4gICAgICAgIHZhciBpc0xpc3RlZDogYm9vbGVhbiA9IHRyYW5zZmVySWNvbkVsZW1lbnQuaGFzQ2xhc3MoJ2ZhLWFuZ2xlLWRvdWJsZS1kb3duJyk7XG4gICAgICAgIGlmKGlzTGlzdGVkKSB7XG4gICAgICAgICAgLy9BbHJlYWR5IGxpc3RlZCB0b2dnbGUgLSB0byBoaWRkZW5cbiAgICAgICAgICB0cmFuc2Zlckljb25FbGVtZW50LmF0dHIoJ2NsYXNzJywgJ2ZhIGZhLWFuZ2xlLWRvdWJsZS1yaWdodCcpO1xuICAgICAgICAgICQoXCIjY3RpX2Nvbm5lY3RlZF9hZ2VudHNcIikuaGlkZSgnc2xpZGUnKTtcbiAgICAgICAgfWVsc2V7XG4gICAgICAgICAgdHJhbnNmZXJCdXR0b25MaXN0ZW5lcigpO1xuICAgICAgICAgIHRyYW5zZmVySWNvbkVsZW1lbnQuYXR0cignY2xhc3MnLCAnZmEgZmEtYW5nbGUtZG91YmxlLWRvd24nKTtcbiAgICAgICAgICAkKFwiI2N0aV9jb25uZWN0ZWRfYWdlbnRzXCIpLnNob3coJ3NsaWRlJyk7XG4gICAgICAgIH1cblxuICAgICAgfSk7XG5cbiAgICAgIHRyYW5zZmVySWNvbkVsZW1lbnQuYXR0cignY2xhc3MnLCAnZmEgZmEtYW5nbGUtZG91YmxlLXJpZ2h0Jyk7XG4gICAgICB0cmFuc2ZlckJ1dHRvbkVsZW1lbnQuYXR0cignY2xhc3MnLCAnYnV0dG9uIGJ1dHRvbi1ibHVlIGRpc2FibGVkJyk7XG4gICAvLyB9XG5cbiAgICAkKFwiI29uY2FsbGRpYWxvZ1wiKS5zaG93KCk7XG4gICAgQ3RpVmlld0hlbHBlci5kaXNhYmxlT25DYWxsQ29udHJvbHMoKTtcbiAgfVxuXG4gIHB1YmxpYyBzdGF0aWMgc2hvd1RyYW5zZmVyQnV0dG9uKGlkOiBzdHJpbmcpOiB2b2lkIHtcbiAgICAkKCcjJytpZCkuYXR0cignY2xhc3MnLCAncHVsbC1yaWdodCB0cmFuc2Zlci1jYWxsLWRpdiBidXR0b24gYnV0dG9uLXJlZCcpO1xuICB9XG5cbiAgcHVibGljIHN0YXRpYyBoaWRlVHJhbnNmZXJCdXR0b24oaWQ6IHN0cmluZyk6IHZvaWQge1xuICAgICQoJyMnK2lkKS5hdHRyKCdjbGFzcycsICdjb2xsYXBzZScpO1xuICB9XG5cbiAgcHVibGljIHN0YXRpYyByZW5kZXJBZ2VudExpc3QoYWdlbnRMaXN0OiBBZ2VudERhdGFbXSwgY2xpY2tIYW5kbGVyOiBhbnkpOiB2b2lkIHtcbiAgICB2YXIgYWdlbnRMaXN0RWxlbWVudDogYW55ID0gJCgnI2FnZW50X2xpc3QnKTtcbiAgICBpZihhZ2VudExpc3QgJiYgYWdlbnRMaXN0Lmxlbmd0aCA9PT0gMCl7XG4gICAgICBhZ2VudExpc3RFbGVtZW50Lmh0bWwoQ3RpVmlld0hlbHBlci5nZXRFbXB0eUFnZW50TGlzdFVJKCkpO1xuICAgIH1lbHNlIGlmKGFnZW50TGlzdCkge1xuICAgICAgYWdlbnRMaXN0RWxlbWVudC5odG1sKEN0aVZpZXdIZWxwZXIuZ2V0QWdlbnRMaXN0VUkoYWdlbnRMaXN0KSk7XG4gICAgICBhZ2VudExpc3RFbGVtZW50Lm9mZignY2xpY2snKS5vbignY2xpY2snLCAnLnRyYW5zZmVyLWNhbGwtZGl2JywgKGV2ZW50OiBhbnkpID0+IHtcbiAgICAgICAgaWYoZXZlbnQgJiYgZXZlbnQuY3VycmVudFRhcmdldCAmJiBldmVudC5jdXJyZW50VGFyZ2V0LmlkKXtcbiAgICAgICAgICBjbGlja0hhbmRsZXIoZXZlbnQuY3VycmVudFRhcmdldC5pZCwgZXZlbnQuY3VycmVudFRhcmdldC5nZXRBdHRyaWJ1dGUoJ2FnZW50X25hbWUnKSk7XG4gICAgICAgICAgLy8kKCcjJytldmVudC5jdXJyZW50VGFyZ2V0LmlkKS5hdHRyKCdjbGFzcycsICdwdWxsLXJpZ2h0IHRyYW5zZmVyLWNhbGwtZGl2IGJ1dHRvbiBidXR0b24tcmVkIGRpc2FibGVkJyk7XG4gICAgICAgIH1cbiAgICAgIH0pO1xuICAgIH1cbiAgfVxuXG4gIHB1YmxpYyBzdGF0aWMgcmVuZGVyQWdlbnRTZWFyY2hVSSgpOiB2b2lkIHtcbiAgICB2YXIgYWdlbnRTZWFyY2hVSVRleHQ6IHN0cmluZyA9ICc8ZGl2IGNsYXNzPVwiYWdlbnQtc2VhcmNoXCIgPicgK1xuICAgICAgICAnU2VhcmNoaW5nIGFnZW50cy4uLjxicj4nICtcbiAgICAgICAgJzxpIGNsYXNzPVwiZmEgZmEtc3Bpbm5lciBmYS1wdWxzZSBmYS0yeCBmYS1md1wiID48L2k+JyArXG4gICAgICAgICc8L2Rpdj4nO1xuXG4gICAgJCgnI2FnZW50X2xpc3QnKS5odG1sKGFnZW50U2VhcmNoVUlUZXh0KTtcbiAgfVxuXG4gIHByaXZhdGUgc3RhdGljIGdldEVtcHR5QWdlbnRMaXN0VUkoKTogc3RyaW5nIHtcbiAgICByZXR1cm4gJzxkaXYgY2xhc3M9YWdlbnQtc2VhcmNoPicrXG4gICAgICAgIEN0aU1lc3NhZ2VzLk1FU1NBR0VfTk9fT05MSU5FX0FHRU5UUyArICc8L2Rpdj4nO1xuICB9XG5cbiAgcHJpdmF0ZSBzdGF0aWMgZ2V0QWdlbnRMaXN0VUkoYWdlbnRMaXN0OiBBZ2VudERhdGFbXSk6IHN0cmluZyB7XG4gICAgdmFyIGFnZW50TGlzdFVJOiBzdHJpbmcgPSAnJztcbiAgICBmb3IodmFyIGFnZW50RGF0YSBvZiBhZ2VudExpc3QpIHtcbiAgICAgIHZhciBlTWFpbDogc3RyaW5nID0gYWdlbnREYXRhLmVtYWlsPyBhZ2VudERhdGEuZW1haWwgOiBDdGlNZXNzYWdlcy5NRVNTQUdFX01BSUxfTk9UX0FWQUlMQUJMRTtcbiAgICAgIGFnZW50TGlzdFVJICs9ICc8ZGl2IGNsYXNzPVwiYWdlbnQtZGF0YVwiID4nICtcbiAgICAgICAgICAgICAgJzxpbWcgc3JjPVwiJythZ2VudERhdGEuZHArJyZyYW5kb21JZD0nK0N0aVZpZXdIZWxwZXIuZ2V0UmFuZG9tSWQoKSsnXCIgPicrXG4gICAgICAgICAgICAgICc8ZGl2IGNsYXNzPVwiYWdlbnQtbmFtZVwiPicgK1xuICAgICAgICAgICAgICAgICc8c3BhbiB0aXRsZT1cIicrYWdlbnREYXRhLm5hbWUrJ1wiPicrYWdlbnREYXRhLm5hbWUrJzwvc3Bhbj48YnI+ICcrXG4gICAgICAgICAgICAgICAgJzxzcGFuIHRpdGxlPVwiJythZ2VudERhdGEuZW1haWwrJ1wiPicrZU1haWwrJzwvc3Bhbj4gJytcbiAgICAgICAgICAgICAgJzwvZGl2PicrXG4gICAgICAgICAgICAgICc8ZGl2IGlkPVwiJythZ2VudERhdGEud29ya2VyKydcIiBhZ2VudF9uYW1lPVwiJythZ2VudERhdGEubmFtZSsnXCIgY2xhc3M9XCJjb2xsYXBzZSBwdWxsLXJpZ2h0IHRyYW5zZmVyLWNhbGwtZGl2IGJ1dHRvbiBidXR0b24tcmVkXCIgPicgK1xuICAgICAgICAgICAgICAgICc8aSBjbGFzcz1cImZhIGZhLXNoYXJlLXNxdWFyZVwiPjwvaT4gJytcbiAgICAgICAgICAgICAgJzwvZGl2PicrXG4gICAgICAgICAgJzwvZGl2PidcbiAgICB9XG5cbiAgICByZXR1cm4gYWdlbnRMaXN0VUk7XG4gIH1cblxuICBwdWJsaWMgc3RhdGljIHJlbmRlckNhbGxEaXNjb25uZWN0VmlldygpOnZvaWQge1xuICAgICQoXCIjb25jYWxsZGlhbG9nXCIpLmhpZGUoKTtcbiAgICAkKFwiI2N0aV9jb25uZWN0ZWRfYWdlbnRzXCIpLmhpZGUoKTtcbiAgICAkKCcjYnVpX2N0aV9kaWFsZXJfbnVtYmVyJykudmFsKCcnKTtcbiAgICAkKFwiI2RpYWxlclwiKS5zaG93KCk7XG4gIH1cblxuICBwdWJsaWMgc3RhdGljIHJlbmRlckNhbGxDYW5jZWxsZWRWaWV3KCk6dm9pZCB7XG4gICAgJChcIiNpbmNvbWluZ2RpYWxvZ1wiKS5oaWRlKCk7XG4gICAgJChcIiNkaWFsZXJcIikuc2hvdygpO1xuICB9XG5cbiAgcHVibGljIHN0YXRpYyByZW5kZXJDYWxsVGltZU91dFZpZXcoKTp2b2lkIHtcbiAgICAkKFwiI2luY29taW5nZGlhbG9nXCIpLmhpZGUoXCJmYWRlXCIpO1xuICAgICQoXCIjZGlhbGVyXCIpLnNob3coKTtcbiAgfVxuXG4gIHB1YmxpYyBzdGF0aWMgZW5hYmxlT25DYWxsQ29udHJvbHMoKTp2b2lkIHtcbiAgICB2YXIgdHJhbnNmZXJCdXR0b246IGFueSA9ICQoJyNvbmNhbGxfdHJhbnNmZXJfYnRuJyk7XG4gICAgdHJhbnNmZXJCdXR0b24uYXR0cignY2xhc3MnLCAnYnV0dG9uIGJ1dHRvbi1ibHVlJyk7XG4gICAgdHJhbnNmZXJCdXR0b24uYXR0cignZGlzYWJsZWQnLCAnJyk7XG4gICAgdmFyIGhhbmd1cEJ1dHRvbjogYW55ID0gJCgnI29uY2FsbF9oYW5ndXBfYnRuJyk7XG4gICAgaGFuZ3VwQnV0dG9uLmF0dHIoJ2NsYXNzJywgJ2J1dHRvbiBidXR0b24tcmVkIHB1bGwtcmlnaHQnKTtcbiAgICBoYW5ndXBCdXR0b24uYXR0cignZGlzYWJsZWQnLCAnJyk7XG4gIH1cblxuICBwdWJsaWMgc3RhdGljIGRpc2FibGVPbkNhbGxDb250cm9scygpOnZvaWQge1xuICAgIHZhciB0cmFuc2ZlckJ1dHRvbjogYW55ID0gJCgnI29uY2FsbF90cmFuc2Zlcl9idG4nKTtcbiAgICB0cmFuc2ZlckJ1dHRvbi5hdHRyKCdjbGFzcycsICdidXR0b24gYnV0dG9uLWJsdWUgZGlzYWJsZWQnKTtcbiAgICB0cmFuc2ZlckJ1dHRvbi5hdHRyKCdkaXNhYmxlZCcsICdkaXNhYmxlZCcpO1xuICAgIHZhciBoYW5ndXBCdXR0b246IGFueSA9ICQoJyNvbmNhbGxfaGFuZ3VwX2J0bicpO1xuICAgIGhhbmd1cEJ1dHRvbi5hdHRyKCdjbGFzcycsICdidXR0b24gYnV0dG9uLXJlZCBwdWxsLXJpZ2h0IGRpc2FibGVkJyk7XG4gICAgaGFuZ3VwQnV0dG9uLmF0dHIoJ2Rpc2FibGVkJywgJ2Rpc2FibGVkJyk7XG4gIH1cblxuICBwdWJsaWMgc3RhdGljIGFkZERpYWxLZXkoZGlhbEtleTpzdHJpbmcpOnZvaWQge1xuICAgIHZhciBpZDpzdHJpbmcgPSBkaWFsS2V5O1xuICAgIHZhciBpZEhhc2g6YW55ID0ge1xuICAgICAgXCIrXCI6ICdwbHVzJyxcbiAgICAgIFwiI1wiOiAnaGFzaCdcbiAgICB9O1xuICAgIGlmIChpc05hTig8YW55PmRpYWxLZXkpKSB7XG4gICAgICBpZCA9IGlkSGFzaFtkaWFsS2V5XTtcbiAgICB9XG4gICAgJChcIiNkaWFscGFkX1wiICsgaWQpLm9mZigpLm9uKFwiY2xpY2tcIiwgKGV2ZW50KSA9PiBDdGlWaWV3SGVscGVyLmFwcGVuZERpZ2l0QXRFbmQoZGlhbEtleSkpO1xuICB9XG5cbiAgcHVibGljIHN0YXRpYyBhZGREaWFsUGFkQ29udHJvbHMob3V0Z29pbmdIYW5kbGVyOiBhbnkpOnZvaWQge1xuICAgIHZhciBkaWFsZXJJbnB1dEVsZW1lbnQ6YW55ID0gJCgnI2J1aV9jdGlfZGlhbGVyX251bWJlcicpO1xuICAgIGRpYWxlcklucHV0RWxlbWVudC5vZmYoJ2tleXByZXNzJykub24oJ2tleXByZXNzJywgKGV2ZW50KT0+IHtcbiAgICAgIHZhciBjaGFyQ29kZSA9IGV2ZW50LmNoYXJDb2RlO1xuICAgICAgcmV0dXJuIChjaGFyQ29kZSA+PSA0OCAmJiBjaGFyQ29kZSA8PSA1NykgfHwgY2hhckNvZGUgPT09IDQzIHx8IGNoYXJDb2RlID09PSAzNTtcbiAgICB9KTtcbiAgICBkaWFsZXJJbnB1dEVsZW1lbnQub2ZmKCdibHVyJykub24oJ2JsdXInLCAoZXZlbnQpPT4ge1xuICAgICAgdmFyIGlucHV0VmFsdWU6c3RyaW5nID0gZGlhbGVySW5wdXRFbGVtZW50LnZhbCgpICsgJyc7XG4gICAgICB2YXIgdmFsaWRJbnB1dDpzdHJpbmdbXSA9IFtdO1xuICAgICAgdmFyIHZhbGlkSW5kZXggPSAwO1xuICAgICAgZm9yICh2YXIgaSA9IDA7IGkgPCBpbnB1dFZhbHVlLmxlbmd0aDsgaSsrKSB7XG4gICAgICAgIHZhciBjaGFyQ29kZTpudW1iZXIgPSBpbnB1dFZhbHVlLmNoYXJDb2RlQXQoaSk7XG4gICAgICAgIGlmICgoY2hhckNvZGUgPj0gNDggJiYgY2hhckNvZGUgPD0gNTcpIHx8IGNoYXJDb2RlID09PSA0MyB8fCBjaGFyQ29kZSA9PT0gMzUpIHtcbiAgICAgICAgICB2YWxpZElucHV0W3ZhbGlkSW5kZXhdID0gaW5wdXRWYWx1ZS5jaGFyQXQoaSk7XG4gICAgICAgICAgdmFsaWRJbmRleCsrXG4gICAgICAgIH1cbiAgICAgIH1cbiAgICAgIGlmIChpbnB1dFZhbHVlLmxlbmd0aCAhPT0gdmFsaWRJbnB1dC5sZW5ndGgpIHtcbiAgICAgICAgJCgnI2J1aV9jdGlfZGlhbGVyX251bWJlcicpLnZhbCh2YWxpZElucHV0LmpvaW4oJycpKTtcbiAgICAgIH1cbiAgICB9KTtcblxuICAgIHZhciBkaWFsZXJLZXkgPSAnMTIzNDU2Nzg5KzAjJztcbiAgICBmb3IgKHZhciBpZCA9IDA7IGlkIDwgZGlhbGVyS2V5Lmxlbmd0aDsgaWQrKykge1xuICAgICAgQ3RpVmlld0hlbHBlci5hZGREaWFsS2V5KGRpYWxlcktleVtpZF0pO1xuICAgIH1cblxuICAgICQoXCIjZGlhbHBhZF9yZW1vdmVfZGlnaXRcIikub2ZmKCkub24oXCJjbGlja1wiLCAoZXZlbnQpID0+IHtcbiAgICAgIEN0aVZpZXdIZWxwZXIucmVtb3ZlRGlnaXRGcm9tRW5kKCk7XG4gICAgfSk7XG5cbiAgICAkKFwiI2RpYWxwYWRfbWFrZV9jYWxsXCIpLm9mZigpLm9uKFwiY2xpY2tcIiwgb3V0Z29pbmdIYW5kbGVyKTtcblxuICB9XG5cbiAgcHVibGljIHN0YXRpYyByZW5kZXJPdXRnb2luZ1VJIChldmVudCwgb3V0Z29pbmdIYW5ndXBIYW5kbGVyKSA6IHZvaWQge1xuICAgIHZhciBvdXROdW1iZXI6c3RyaW5nID0gJCgnI2J1aV9jdGlfZGlhbGVyX251bWJlcicpLnZhbCgpICsgJyc7XG4gICAgaWYgKG91dE51bWJlcikge1xuICAgICAgdmFyIG91dGdvaW5nRWxlbWVudDphbnkgPSAkKCcjb3V0X2dvaW5nX251bWJlcicpO1xuICAgICAgJCgnI2RpYWxlcicpLmhpZGUoJ3NsaWRlJyk7XG4gICAgICBvdXRnb2luZ0VsZW1lbnQuaHRtbChvdXROdW1iZXIpO1xuICAgICAgb3V0Z29pbmdFbGVtZW50LmF0dHIoJ3RpdGxlJywgb3V0TnVtYmVyKTtcblxuICAgICAgJCgnI291dGdvaW5nZGlhbG9nJykuc2hvdygnc2xpZGUnKTtcblxuICAgICAgJChcIiNlbmRfb3V0Z29pbmdfYnRuXCIpLm9mZigpLm9uKFwiY2xpY2tcIiwgb3V0Z29pbmdIYW5ndXBIYW5kbGVyKTtcbiAgICB9XG4gIH07XG5cbiAgcHVibGljIHN0YXRpYyByZW5kZXJPdXRnb2luZ0hhbmd1cFVJKGV2ZW50OiBhbnkpOiB2b2lkIHtcbiAgICAkKCcjb3V0Z29pbmdkaWFsb2cnKS5oaWRlKCdzbGlkZScpO1xuICAgICQoJyNidWlfY3RpX2RpYWxlcl9udW1iZXInKS52YWwoJycpO1xuICAgICQoJyNkaWFsZXInKS5zaG93KCdzbGlkZScpO1xuICB9XG5cbiAgcHVibGljIHN0YXRpYyB1cGRhdGVPdXRnb2luZ0NvbnRhY3REZXRhaWxzKGNvbnRhY3ROYW1lLCBkaXNwbGF5UGljKSA6IHZvaWQge1xuICAgIGlmKGNvbnRhY3ROYW1lKXtcbiAgICAgICQoJyNvbmNhbGxfbmFtZScpLmh0bWwoY29udGFjdE5hbWUpO1xuICAgIH1cblxuICAgIGlmKGRpc3BsYXlQaWMpIHtcbiAgICAgICQoJyNvbmNhbGxfY29udGFjdF9pbWFnZScpLmF0dHIoJ3NyYycsIGRpc3BsYXlQaWMgKyAnJnJhbmRvbUlkPScgK0N0aVZpZXdIZWxwZXIuZ2V0UmFuZG9tSWQoKSk7XG4gICAgfVxuICB9XG5cbiAgcHVibGljIHN0YXRpYyBnZXRPdXRnb2luZ0NvbnRhY3ROdW1iZXIoKSA6IHN0cmluZyB7XG4gICAgcmV0dXJuICQoJyNidWlfY3RpX2RpYWxlcl9udW1iZXInKS52YWwoKSArICcnO1xuICB9XG5cbiAgcHVibGljIHN0YXRpYyBhcHBlbmREaWdpdEF0RW5kKGRpZ2l0OnN0cmluZyk6dm9pZCB7XG4gICAgdmFyIGlucHV0RWxlbWVudDphbnkgPSAkKCcjYnVpX2N0aV9kaWFsZXJfbnVtYmVyJyk7XG4gICAgdmFyIG51bWJlclBsYWNlZDpzdHJpbmcgPSBpbnB1dEVsZW1lbnQudmFsKCkgKyAnJztcbiAgICB2YXIgc2VsZWN0aW9uU3RhcnQ6bnVtYmVyID0gaW5wdXRFbGVtZW50WzBdLnNlbGVjdGlvblN0YXJ0O1xuICAgIHZhciBzZWxlY3Rpb25FbmQ6bnVtYmVyID0gaW5wdXRFbGVtZW50WzBdLnNlbGVjdGlvbkVuZDtcblxuICAgIG51bWJlclBsYWNlZCA9IG51bWJlclBsYWNlZC5zdWJzdHJpbmcoMCwgc2VsZWN0aW9uU3RhcnQpICsgZGlnaXQgKyBudW1iZXJQbGFjZWQuc3Vic3RyaW5nKHNlbGVjdGlvbkVuZCk7XG4gICAgaW5wdXRFbGVtZW50LnZhbChudW1iZXJQbGFjZWQpO1xuXG4gICAgQ3RpVmlld0hlbHBlci5jb3JyZWN0Q3Vyc29yQWZ0ZXJWYWx1ZUNoYW5nZShpbnB1dEVsZW1lbnRbMF0sIHNlbGVjdGlvblN0YXJ0LCBzZWxlY3Rpb25FbmQsIG51bWJlclBsYWNlZC5sZW5ndGgsIHRydWUpO1xuICB9XG5cbiAgcHVibGljIHN0YXRpYyByZW1vdmVEaWdpdEZyb21FbmQoKTp2b2lkIHtcbiAgICB2YXIgaW5wdXRFbGVtZW50OmFueSA9ICQoJyNidWlfY3RpX2RpYWxlcl9udW1iZXInKTtcbiAgICB2YXIgbnVtYmVyUGxhY2VkOnN0cmluZyA9IGlucHV0RWxlbWVudC52YWwoKSArICcnO1xuICAgIHZhciBzZWxlY3Rpb25TdGFydDpudW1iZXIgPSBpbnB1dEVsZW1lbnRbMF0uc2VsZWN0aW9uU3RhcnQ7XG4gICAgdmFyIHNlbGVjdGlvbkVuZDpudW1iZXIgPSBpbnB1dEVsZW1lbnRbMF0uc2VsZWN0aW9uRW5kO1xuICAgIGlmIChzZWxlY3Rpb25FbmQgPT09IHNlbGVjdGlvblN0YXJ0KSB7XG4gICAgICBudW1iZXJQbGFjZWQgPSBudW1iZXJQbGFjZWQuc3Vic3RyaW5nKDAsIHNlbGVjdGlvblN0YXJ0IC0gMSkgKyBudW1iZXJQbGFjZWQuc3Vic3RyaW5nKHNlbGVjdGlvbkVuZCk7XG4gICAgfSBlbHNlIHtcbiAgICAgIG51bWJlclBsYWNlZCA9IG51bWJlclBsYWNlZC5zdWJzdHJpbmcoMCwgc2VsZWN0aW9uU3RhcnQpICsgbnVtYmVyUGxhY2VkLnN1YnN0cmluZyhzZWxlY3Rpb25FbmQpO1xuICAgIH1cbiAgICBpbnB1dEVsZW1lbnQudmFsKG51bWJlclBsYWNlZCk7XG4gICAgQ3RpVmlld0hlbHBlci5jb3JyZWN0Q3Vyc29yQWZ0ZXJWYWx1ZUNoYW5nZShpbnB1dEVsZW1lbnRbMF0sIHNlbGVjdGlvblN0YXJ0LCBzZWxlY3Rpb25FbmQsIG51bWJlclBsYWNlZC5sZW5ndGgsIGZhbHNlKTtcbiAgfVxuXG4gIHB1YmxpYyBzdGF0aWMgY29ycmVjdEN1cnNvckFmdGVyVmFsdWVDaGFuZ2UoZWxlbWVudCwgc2VsZWN0aW9uU3RhcnQsIHNlbGVjdGlvbkVuZCwgaW5wdXRMZW5ndGgsIGlzRm9yd2FyZCkge1xuICAgIC8vU2V0IGZvY3VzIHRvIHRoZSBwcmV2aW91cy9uZXh0IGNoYXJhY3RlciBjaGFyXG4gICAgdmFyIGluZGV4Om51bWJlciA9IHNlbGVjdGlvblN0YXJ0O1xuICAgIGlmIChpc0ZvcndhcmQpIHtcbiAgICAgIGlmIChpbmRleCA8IGlucHV0TGVuZ3RoKSB7XG4gICAgICAgIGluZGV4Kys7XG4gICAgICB9XG4gICAgfSBlbHNlIHtcbiAgICAgIGlmIChzZWxlY3Rpb25TdGFydCA9PT0gc2VsZWN0aW9uRW5kICYmIGluZGV4ID4gMCkge1xuICAgICAgICBpbmRleC0tO1xuICAgICAgfVxuICAgIH1cbiAgICBlbGVtZW50LmZvY3VzKCk7XG4gICAgaWYgKGVsZW1lbnQuc2V0U2VsZWN0aW9uUmFuZ2UpIHtcbiAgICAgIGVsZW1lbnQuc2V0U2VsZWN0aW9uUmFuZ2UoaW5kZXgsIGluZGV4KTtcbiAgICB9IGVsc2Uge1xuICAgICAgaWYgKGVsZW1lbnQuY3JlYXRlVGV4dFJhbmdlKSB7XG4gICAgICAgIHZhciByYW5nZSA9IGVsZW1lbnQuY3JlYXRlVGV4dFJhbmdlKCk7XG4gICAgICAgIHJhbmdlLmNvbGxhcHNlKHRydWUpO1xuICAgICAgICByYW5nZS5tb3ZlRW5kKCdjaGFyYWN0ZXInLCBpbmRleCk7XG4gICAgICAgIHJhbmdlLm1vdmVTdGFydCgnY2hhcmFjdGVyJywgaW5kZXgpO1xuICAgICAgICByYW5nZS5zZWxlY3QoKTtcbiAgICAgIH1cbiAgICB9XG4gIH1cblxuICBwdWJsaWMgc3RhdGljIGdldFJhbmRvbUlkKCk6IHN0cmluZyB7XG4gICAgcmV0dXJuIG5ldyBEYXRlKCkudmFsdWVPZigpKydfJztcbiAgfVxufSJdfQ==