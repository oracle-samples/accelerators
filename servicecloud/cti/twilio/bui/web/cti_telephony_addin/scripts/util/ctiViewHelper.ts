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
 *  File: ctiViewHelper.ts
 * ****************************************************************************************** */

import $ = require('jquery');
import {CtiConstants} from './ctiConstants';
import {AgentData} from "../model/agentData";
import {CtiMessages} from "./ctiMessages";

/**
 * CtiViewHelper - Does all dynamic rendering of the UI
 *
 */
export class CtiViewHelper {

  private static selection:any = {};
  //private static isOutbound: boolean = false;

  public static renderIncomingView(ctiData:any, audio:any):void {
    //Clear Contact Mail
    var incomingMailElement:any = $("#incoming_contact_mail");
    var incomingNameElement:any = $("#incoming_name");
    var incomingNumberElement:any = $("#incoming_number");
    var acceptButton = $('#notif_accept_btn');
    var rejectButton = $('#notif_reject_btn');
    acceptButton.attr('class', "button button-green");
    rejectButton.attr('class', "button button-red");

    incomingMailElement.html('');

    incomingNameElement.html(ctiData.contact.name);
    incomingNameElement.attr('title', ctiData.contact.name);

    incomingNumberElement.html(ctiData.contact.phone);
    incomingNumberElement.attr('title', ctiData.contact.phone);

    if (ctiData.contact.name !== CtiConstants.UNKNOWN_CALLER) {
      var html = "<i class='fa fa-envelope-o'></i> <span class='contact_email'>" + ctiData.contact.email + "</span>";
      incomingMailElement.html(html);
      incomingMailElement.attr('title', ctiData.contact.email);
    }
    $("#incoming_contact_image").attr('src', ctiData.contact.dp + '&randomId=' +CtiViewHelper.getRandomId());

    acceptButton.off().on("click", (event) => {
      acceptButton.attr('class', "button button-green disabled");
      ctiData.accept();
      audio.pause();
      audio.currentTime = 0;
    });
    rejectButton.off().on("click", (event) => {
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
  }

  public static showOnCallUI(phone: string, displayPic: string): void {
    $("#incomingdialog").hide();
    $("#dialer").hide();

    var onCallMailElement:any = $("#oncall_contact_mail");
    var onCallNameElement:any = $("#oncall_name");
    var onCallNumberElement:any = $("#oncall_number");
    var onCallImageElement = $("#oncall_contact_image");

    onCallImageElement.attr('src', displayPic + '&randomId=' +CtiViewHelper.getRandomId());

    onCallMailElement.html('');

    // Prepare the dialog
    onCallNameElement.html('Searching contact..');
    onCallNameElement.attr('title', 'Searching contact..');
    onCallNumberElement.html(phone);
    onCallNumberElement.attr('title', 'phone');
    var onCallMuteButton:any = $("#oncall_mute_btn");
    onCallMuteButton.attr('class', 'button button-blue disabled');
    var transferIconElement: any =  $('#oncall_transfer_icon');
    transferIconElement.attr('class', 'fa fa-angle-double-right');
    $("#oncall_mute_btn>i").attr('class', 'fa fa-microphone');
    $("#oncalldialog").show();
    CtiViewHelper.disableOnCallControls();
  }

  public static renderOnCallView(ctiData:any, audio:any, transferButtonListener: any):void {
    //TODO - Removed commented out code
    //CtiViewHelper.isOutbound = isOutbound;
    audio.currentTime = 0;
    $("#incomingdialog").hide();
    $("#dialer").hide();
    var onCallMailElement:any = $("#oncall_contact_mail");
    var onCallNameElement:any = $("#oncall_name");
    var onCallNumberElement:any = $("#oncall_number");
    var onCallMuteIcon:any = $("#oncall_mute_btn>i");
    onCallMuteIcon.attr('class', 'fa fa-microphone');
    var onCallMuteButton:any = $("#oncall_mute_btn");
    onCallMuteButton.attr('class', 'button button-blue');
    var transferButtonElement: any = $('#oncall_transfer_btn');
    var transferIconElement: any =  $('#oncall_transfer_icon');
    transferIconElement.attr('class', 'fa fa-angle-double-right');

    //Clear Contact Mail
    onCallMailElement.html('');

    // Prepare the dialog
    onCallNameElement.html(ctiData.contact.name);
    onCallNameElement.attr('title', ctiData.contact.name);

    onCallNumberElement.html(ctiData.contact.phone);
    onCallNumberElement.attr('title', ctiData.contact.phone);

    if (ctiData.contact.name !== CtiConstants.UNKNOWN_CALLER) {
      var html = "<i class='fa fa-envelope-o'></i> <span class='contact_email'>" + ctiData.contact.email + "</span>";
      onCallMailElement.html(html);
      onCallMailElement.attr('title', ctiData.contact.email);
    }
    $("#oncall_contact_image").attr('src', ctiData.contact.dp + '&randomId=' +CtiViewHelper.getRandomId());

    $("#oncall_hangup_btn").off().on("click", (event) => {
      ctiData.hangup();
    });

    //Handle Mute
    onCallMuteButton.off().on("click", (event) => {
      event.preventDefault();
      var onCallDialogElement:any = $("#oncalldialog");
      var muted = onCallMuteIcon.hasClass("fa fa-microphone-slash");
      
      ctiData.mute(!muted);

      if (muted) {
        onCallMuteIcon.removeClass().addClass("fa fa-microphone");
      } else {
        onCallMuteIcon.removeClass().addClass("fa fa-microphone-slash");
      }

    });

    //Call transfer search button
   /* if(isOutbound) {
      $('#oncall_transfer_btn').attr('class', 'collapse');
    }else{*/

      transferButtonElement.off('click').on('click', (event: any) => {

        var isListed: boolean = transferIconElement.hasClass('fa-angle-double-down');
        if(isListed) {
          //Already listed toggle - to hidden
          transferIconElement.attr('class', 'fa fa-angle-double-right');
          $("#cti_connected_agents").hide('slide');
        }else{
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
  }

  public static showTransferButton(id: string): void {
    $('#'+id).attr('class', 'pull-right transfer-call-div button button-red');
  }

  public static hideTransferButton(id: string): void {
    $('#'+id).attr('class', 'collapse');
  }

  public static renderAgentList(agentList: AgentData[], clickHandler: any): void {
    var agentListElement: any = $('#agent_list');
    if(agentList && agentList.length === 0){
      agentListElement.html(CtiViewHelper.getEmptyAgentListUI());
    }else if(agentList) {
      agentListElement.html(CtiViewHelper.getAgentListUI(agentList));
      agentListElement.off('click').on('click', '.transfer-call-div', (event: any) => {
        if(event && event.currentTarget && event.currentTarget.id){
          clickHandler(event.currentTarget.id, event.currentTarget.getAttribute('agent_name'));
          //$('#'+event.currentTarget.id).attr('class', 'pull-right transfer-call-div button button-red disabled');
        }
      });
    }
  }

  public static renderAgentSearchUI(): void {
    var agentSearchUIText: string = '<div class="agent-search" >' +
        'Searching agents...<br>' +
        '<i class="fa fa-spinner fa-pulse fa-2x fa-fw" ></i>' +
        '</div>';

    $('#agent_list').html(agentSearchUIText);
  }

  private static getEmptyAgentListUI(): string {
    return '<div class=agent-search>'+
        CtiMessages.MESSAGE_NO_ONLINE_AGENTS + '</div>';
  }

  private static getAgentListUI(agentList: AgentData[]): string {
    var agentListUI: string = '';
    for(var agentData of agentList) {
      var eMail: string = agentData.email? agentData.email : CtiMessages.MESSAGE_MAIL_NOT_AVAILABLE;
      agentListUI += '<div class="agent-data" >' +
              '<img src="'+agentData.dp+'&randomId='+CtiViewHelper.getRandomId()+'" >'+
              '<div class="agent-name">' +
                '<span title="'+agentData.name+'">'+agentData.name+'</span><br> '+
                '<span title="'+agentData.email+'">'+eMail+'</span> '+
              '</div>'+
              '<div id="'+agentData.worker+'" agent_name="'+agentData.name+'" class="collapse pull-right transfer-call-div button button-red" >' +
                '<i class="fa fa-share-square"></i> '+
              '</div>'+
          '</div>'
    }

    return agentListUI;
  }

  public static renderCallDisconnectView():void {
    $("#oncalldialog").hide();
    $("#cti_connected_agents").hide();
    $('#bui_cti_dialer_number').val('');
    $("#dialer").show();
  }

  public static renderCallCancelledView():void {
    $("#incomingdialog").hide();
    $("#dialer").show();
  }

  public static renderCallTimeOutView():void {
    $("#incomingdialog").hide("fade");
    $("#dialer").show();
  }

  public static enableOnCallControls():void {
    var transferButton: any = $('#oncall_transfer_btn');
    transferButton.attr('class', 'button button-blue');
    transferButton.attr('disabled', '');
    var hangupButton: any = $('#oncall_hangup_btn');
    hangupButton.attr('class', 'button button-red pull-right');
    hangupButton.attr('disabled', '');
  }

  public static disableOnCallControls():void {
    var transferButton: any = $('#oncall_transfer_btn');
    transferButton.attr('class', 'button button-blue disabled');
    transferButton.attr('disabled', 'disabled');
    var hangupButton: any = $('#oncall_hangup_btn');
    hangupButton.attr('class', 'button button-red pull-right disabled');
    hangupButton.attr('disabled', 'disabled');
  }

  public static addDialKey(dialKey:string):void {
    var id:string = dialKey;
    var idHash:any = {
      "+": 'plus',
      "#": 'hash'
    };
    if (isNaN(<any>dialKey)) {
      id = idHash[dialKey];
    }
    $("#dialpad_" + id).off().on("click", (event) => CtiViewHelper.appendDigitAtEnd(dialKey));
  }

  public static addDialPadControls(outgoingHandler: any):void {
    var dialerInputElement:any = $('#bui_cti_dialer_number');
    dialerInputElement.off('keypress').on('keypress', (event)=> {
      var charCode = event.charCode;
      return (charCode >= 48 && charCode <= 57) || charCode === 43 || charCode === 35;
    });
    dialerInputElement.off('blur').on('blur', (event)=> {
      var inputValue:string = dialerInputElement.val() + '';
      var validInput:string[] = [];
      var validIndex = 0;
      for (var i = 0; i < inputValue.length; i++) {
        var charCode:number = inputValue.charCodeAt(i);
        if ((charCode >= 48 && charCode <= 57) || charCode === 43 || charCode === 35) {
          validInput[validIndex] = inputValue.charAt(i);
          validIndex++
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

    $("#dialpad_remove_digit").off().on("click", (event) => {
      CtiViewHelper.removeDigitFromEnd();
    });

    $("#dialpad_make_call").off().on("click", outgoingHandler);

  }

  public static renderOutgoingUI (event, outgoingHangupHandler) : void {
    var outNumber:string = $('#bui_cti_dialer_number').val() + '';
    if (outNumber) {
      var outgoingElement:any = $('#out_going_number');
      $('#dialer').hide('slide');
      outgoingElement.html(outNumber);
      outgoingElement.attr('title', outNumber);

      $('#outgoingdialog').show('slide');

      $("#end_outgoing_btn").off().on("click", outgoingHangupHandler);
    }
  };

  public static renderOutgoingHangupUI(event: any): void {
    $('#outgoingdialog').hide('slide');
    $('#bui_cti_dialer_number').val('');
    $('#dialer').show('slide');
  }

  public static updateOutgoingContactDetails(contactName, displayPic) : void {
    if(contactName){
      $('#oncall_name').html(contactName);
    }

    if(displayPic) {
      $('#oncall_contact_image').attr('src', displayPic + '&randomId=' +CtiViewHelper.getRandomId());
    }
  }

  public static getOutgoingContactNumber() : string {
    return $('#bui_cti_dialer_number').val() + '';
  }

  public static appendDigitAtEnd(digit:string):void {
    var inputElement:any = $('#bui_cti_dialer_number');
    var numberPlaced:string = inputElement.val() + '';
    var selectionStart:number = inputElement[0].selectionStart;
    var selectionEnd:number = inputElement[0].selectionEnd;

    numberPlaced = numberPlaced.substring(0, selectionStart) + digit + numberPlaced.substring(selectionEnd);
    inputElement.val(numberPlaced);

    CtiViewHelper.correctCursorAfterValueChange(inputElement[0], selectionStart, selectionEnd, numberPlaced.length, true);
  }

  public static removeDigitFromEnd():void {
    var inputElement:any = $('#bui_cti_dialer_number');
    var numberPlaced:string = inputElement.val() + '';
    var selectionStart:number = inputElement[0].selectionStart;
    var selectionEnd:number = inputElement[0].selectionEnd;
    if (selectionEnd === selectionStart) {
      numberPlaced = numberPlaced.substring(0, selectionStart - 1) + numberPlaced.substring(selectionEnd);
    } else {
      numberPlaced = numberPlaced.substring(0, selectionStart) + numberPlaced.substring(selectionEnd);
    }
    inputElement.val(numberPlaced);
    CtiViewHelper.correctCursorAfterValueChange(inputElement[0], selectionStart, selectionEnd, numberPlaced.length, false);
  }

  public static correctCursorAfterValueChange(element, selectionStart, selectionEnd, inputLength, isForward) {
    //Set focus to the previous/next character char
    var index:number = selectionStart;
    if (isForward) {
      if (index < inputLength) {
        index++;
      }
    } else {
      if (selectionStart === selectionEnd && index > 0) {
        index--;
      }
    }
    element.focus();
    if (element.setSelectionRange) {
      element.setSelectionRange(index, index);
    } else {
      if (element.createTextRange) {
        var range = element.createTextRange();
        range.collapse(true);
        range.moveEnd('character', index);
        range.moveStart('character', index);
        range.select();
      }
    }
  }

  public static getRandomId(): string {
    return new Date().valueOf()+'_';
  }
}