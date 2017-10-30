/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set 
 *  published by Oracle under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: Telephony and SMS Accelerator for Twilio
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17D (November 2017)
 *  reference: 161212-000059
 *  date: Monday Oct 30 13:4:53 UTC 2017
 *  revision: rnw-17-11-fixes-releases
 * 
 *  SHA1: $Id: b24b8a5de8db615b696b83f5bbd22e75ea226000 $
 * *********************************************************************************************
 *  File: ctiMessages.ts
 * ****************************************************************************************** */

export class CtiMessages {

    //COMMON
    public static MESSAGE_APPENDER: string = ' >> ';
    public static MESSAGE_EXCEPTION: string = "Exception Caught";

    //ServerEventHandler
    public static MESSAGE_EVENT_DISPATCH: string = 'Dispatching Event ';
    public static MESSAGE_WITH_DATA: string = ' with Data';

    //TwilioAdapter
    public static MESSAGE_HANDLER_ADDED:string = 'Handler added for event ';
    public static MESSAGE_HANDLER_REMOVED: string = 'Handler removed for event ';
    public static MESSAGE_ALL_HANDLERS_REMOVED: string = 'Removing all event handlers..';

    //TwilioCommunicationHandler
    public static MESSAGE_LOGIN: string = 'Logging in';
    public static MESSAGE_LOGIN_ERROR_DEVICE: string = 'An error occurred. Unable to login. ';
    public static MESSAGE_DEVICE_INIT_FAILURE: string = 'Unable to initialize device due to ';
    public static MESSAGE_LOGIN_FAILURE: string = 'Login failed due to ';
    public static MESSAGE_LOGOUT: string = 'Logging out..';
    public static MESSAGE_NOT_LOGGEDIN_FOR_ACTION: string = 'You are not Logged-in to perform this action!';
    public static MESSAGE_REQUEST_ACTIVITY_UPDATE: string = 'Requesting activity update';
    public static MESSAGE_ACTIVITY_UPDATE_ERROR: string = 'Error while updating activity';
    public static MESSAGE_CONTACT_SEARCH: string = 'Searching for contact ';
    public static MESSAGE_CONTACT_SEARCH_SUCCESS: string = 'Contact search succeeded ';
    public static MESSAGE_DIALING: string = 'Dialing ';
    public static MESSAGE_SEARCH_AVAILABLE_AGENTS: string = 'Searching for available agents..';
    public static MESSAGE_AGENT_SEARCH_SUCCESS: string = 'Search for available agents completed';
    public static MESSAGE_AGENT_SEARCH_FAILURE: string = 'Search for available agents failed';
    public static MESSAGE_CTI_AUTHORIZE: string = 'Requesting for CTI Authorization..';
    public static MESSAGE_CTI_ENABLED: string = 'CTI is available to agent';
    public static MESSAGE_CTI_DISABLED: string = 'CTI not available for agent';
    public static MESSAGE_CTI_AUTHORIZATION_FAILURE: string = 'Unable to authorize agent for CTI';
    public static MESSAGE_REQUEST_CALL_TRANSFER: string = 'Requesting for call transfer to agent';
    public static MESSAGE_UPDATE_DEVICE: string = 'Updating device/worker with new token..';
    public static MESSAGE_UPDATE_DEVICE_SUCCESS: string = 'Device/worker renewed successfully';
    public static MESSAGE_TOKEN_UPDATE_FAILURE: string = 'Unable to renew capability token';
    public static MESSAGE_LOG_ACTION: string = 'Action logged';
    public static MESSAGE_LOG_ATION_FAILURE: string = 'Unable to log action';
    public static MESSAGE_DEVICE_INITIALIZE: string = 'One-time initialization of Twilio Device';
    public static MESSAGE_INCOMING_CONNECTION: string = 'Incoming connection from ';
    public static MESSAGE_CONNECTION_ESTABLISHED: string = 'Connection established with server';
    public static MESSAGE_CONNECTION_BROKE: string = 'Disconnected from server';
    public static MESSAGE_DISPATCH_LOGIN_SUCCESS: string = 'Dispatching event Login Success';
    public static MESSAGE_RESERVATION_CREATED: string = 'Reservation created for agent.';
    public static MESSAGE_RESERVATION_CANCELLED: string = 'Reservation canceled';
    public static MESSAGE_RESERVATION_ACCEPTED: string = 'Reservation accepted';
    public static MESSAGE_RESERVATION_REJECTED: string = 'Reservation rejected';
    public static MESSAGE_TOKEN_EXPIRED: string = 'Received Token Expired message. Requesting for renewal..';

    //CtiTelephonyAddin
    public static MESSAGE_LOAD_EXTENSION: string = 'Loading extension..';
    public static MESSAGE_OBTAINED_SDK: string = 'Obtained SDK..';
    public static MESSAGE_INITIALIZE_ADDIN: string = 'Initializing CTI Addin- Toolbar menu..';
    public static MESSAGE_INITIALIZE_SIDEPANEL: string = 'Initializing CTI Addin- SidePane menu..';
    public static MESSAGE_REGISTER_EVENT_HANDLERS: string = 'Registering event handlers..';
    public static MESSAGE_HANDLE_OUTGOING_CALL: string = 'Handling outbound to';
    public static MESSAGE_INITIATE_LOGIN: string = 'Initiating login process..';
    public static MESSAGE_PARTIAL_LOGOUT: string = 'You are not completely logged out.';
    public static MESSAGE_ALREADY_LOGGED_IN: string = 'Already logged in.';
    public static MESSAGE_UI_UPDATE_AFTER_LOGIN_SUCCESS: string = 'Updating UI after successful login';
    public static MESSAGE_CLIENT_STATUS: string = 'Client status';
    public static MESSAGE_SERVER_STATUS: string = 'Status Update from Server';
    public static MESSAGE_HANDLE_CALL_INCOMING: string = 'Handling incoming call..';
    public static MESSAGE_HANDLE_CALL_CONNECTED: string = 'Handling connected call..';
    public static MESSAGE_HANDLE_CALL_DISCONNECT: string = 'Handling call disconnect..';
    public static MESSAGE_HANDLE_CALL_CANCEL: string = 'Handling cancelled call..';
    public static MESSAGE_HANDLE_CALL_TIMEOUT: string = 'Handling timed out call..';
    public static MESSAGE_HANDLE_AGENT_SEARCH_COMPLETION: string = 'Handling agent search completion..';
    public static MESSAGE_HANDLE_TOKEN_EXPIRY: string = 'Handling Token expiry..';
    public static MESSAGE_OPEN_INTERACTION_WORKSPACE: string = 'Opening interaction workspace..';
    public static MESSAGE_MAIL_NOT_AVAILABLE: string = 'Mail not available';
    public static MESSAGE_INITIATE_AGENT_SEARCH: string = 'Initiating search for available agents..';
    public static MESSAGE_INITIATE_TRANSFER: string ='Initiating transfer call request..';
    public static MESSAGE_BY_AGENT: string = ' by agent ';
    public static MESSAGE_INITIATE_ACTIVITY_UPDATE: string = 'Initiating activity update request';
    public static MESSAGE_CALL_SUMMARIZED: string = 'Call already summarized..';
    public static MESSAGE_SUMMARIZE_CALL: string = 'Summarizing call..';
    public static MESSAGE_CALL_DURATION: string = 'Call Duration: ';
    public static MESSAGE_CALL_START: string = 'Call started at: ';
    public static MESSAGE_CALL_END: string = 'Call ended at: ';
    public static MESSAGE_CALL_TRANSFERRED_TO: string = 'Call transferred to ';
    public static MESSAGE_CALL_ACCEPTED_BY_AGENT: string = 'Call accepted by agent ';
    public static MESSAGE_CALL_REJECTED_BY_AGENT: string = 'Call rejected by agent ';
    public static MESSAGE_WAIT_WHILE_TRANSFER: string = 'Please wait. Your call is being transferred..!!';

    //CtiViewHelper
    public static MESSAGE_NO_ONLINE_AGENTS: string = 'No agents are available now';
}