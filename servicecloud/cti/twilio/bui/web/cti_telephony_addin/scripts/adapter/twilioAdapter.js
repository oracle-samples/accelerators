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
 *  SHA1: $Id: 2985e7d6db45a10cda4b4295fab9e6896d301030 $
 * *********************************************************************************************
 *  File: twilioAdapter.js
 * ****************************************************************************************** */
define(["require", "exports", "./twilioCommunicationHandler", "../util/ctiLogger", "../util/ctiMessages"], function (require, exports, twilioCommunicationHandler_1, ctiLogger_1, ctiMessages_1) {
    "use strict";
    exports.__esModule = true;
    /**
     *This class implements the ICTIAdapter interface, which will be used by the
     * adddin to communicate with underlying CTI platform
     *
     */
    var TwilioAdapter = /** @class */ (function () {
        function TwilioAdapter() {
            this.logPreMessage = 'TwilioAdapter' + ctiMessages_1.CtiMessages.MESSAGE_APPENDER;
            this.ctiConfiguration = {
                providerName: 'Twilio',
                providerPath: 'cc/CTI',
                defaultStatus: 'Ready',
                recordByDefault: false
            };
            this.eventHandlers = {};
            this.serverCommunicationHandler = new twilioCommunicationHandler_1.TwilioCommunicationHandler();
        }
        /**
         * This function handles login to the underlying
         * CTI Tool/Platform
         *
         * @param profileData
           */
        TwilioAdapter.prototype.login = function (profileData) {
            var loginUrl = profileData.interfaceUrl.match(/^[^\/]+:\/\/[^\/]+\//)[0].replace(/^http(?!s)/i, 'https') + this.ctiConfiguration.providerPath;
            this.eventHandlers = {};
            this.serverCommunicationHandler.login(loginUrl, profileData.sessionId, profileData.accountId);
        };
        /**
         * Adds handler for an event. This events will be used by the adapter
         * while logging in to register with the tool
         *
         * @param eventName
         * @param handler
         */
        TwilioAdapter.prototype.addEventHandler = function (eventName, handler) {
            this.eventHandlers[eventName] = handler;
            //Currently one handler per event is allowed
            this.serverCommunicationHandler.getServerEventHandler()
                .off(eventName).on(eventName, handler);
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_HANDLER_ADDED
                + ctiMessages_1.CtiMessages.MESSAGE_APPENDER + eventName);
        };
        /**
         * Clears handler associated with an event
         *
         * @param eventName
         */
        TwilioAdapter.prototype.clearEventHandler = function (eventName) {
            this.eventHandlers[eventName] = null;
            this.serverCommunicationHandler.getServerEventHandler().off(eventName);
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage +
                ctiMessages_1.CtiMessages.MESSAGE_HANDLER_REMOVED + ctiMessages_1.CtiMessages.MESSAGE_APPENDER + eventName);
        };
        /**
         * Clears all event handlers added to the adapter
         *
         */
        TwilioAdapter.prototype.clearAllEventHandlers = function () {
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_ALL_HANDLERS_REMOVED);
            for (var key in this.eventHandlers) {
                if (key) {
                    this.eventHandlers[key] = null;
                    this.serverCommunicationHandler.getServerEventHandler().off(key);
                }
            }
            this.isHandlersAttached = false;
        };
        /**
         * This function handles logout from the underlying
         * CTI Tool/Platform
         *
         */
        TwilioAdapter.prototype.logout = function () {
            this.serverCommunicationHandler.logout();
            this.clearAllEventHandlers();
        };
        /**
         * This function handles activity updates to the underlying
         * CTI Tool/Platform
         *
         * @param status
         */
        TwilioAdapter.prototype.updateActivity = function (status) {
            this.serverCommunicationHandler.updateActivity(status);
        };
        /**
         * This function returns the basic CTI configuration required by the
         * addin
         *
         * @returns {CtiConfiguration}
         */
        TwilioAdapter.prototype.getConfiguration = function () {
            return this.ctiConfiguration;
        };
        /**
         * This function returns the handler associated with an event
         *
         * @param eventName
         * @returns {any}
         */
        TwilioAdapter.prototype.getEventHandler = function (eventName) {
            return this.eventHandlers[eventName];
        };
        /**
         * this function search for a contact
         *
         * @param contact
         * @param sessionId
         */
        TwilioAdapter.prototype.searchContact = function (contact, sessionId) {
            this.serverCommunicationHandler.searchContact(contact, sessionId);
        };
        /**
         * Make an outbound call with the given
         * contact
         *
         * @param contact
           */
        TwilioAdapter.prototype.dialANumber = function (contact) {
            this.serverCommunicationHandler.dialANumber(contact);
        };
        /**
         * Searches for all available agents in the system
         *
         * @param sessionId
           */
        TwilioAdapter.prototype.searchAvailableAgents = function (sessionId) {
            this.serverCommunicationHandler.getAvailableAgents(sessionId);
        };
        /**
         * Authorize agent for CTI Access
         *
         * @param interfaceUrl
         * @param sessionId
           */
        TwilioAdapter.prototype.authorizeAgent = function (interfaceUrl, sessionId) {
            this.serverUrl = interfaceUrl.match(/^[^\/]+:\/\/[^\/]+\//)[0].replace(/^http(?!s)/i, 'https');
            this.serverCommunicationHandler.isCtiEnabled(interfaceUrl, this.ctiConfiguration.providerPath, sessionId);
        };
        /**
         * Transfer a call to another agent(agent with a given workerId)
         *
         * @param sessionId
         * @param workerId
         * @param incidentId
           */
        TwilioAdapter.prototype.transferCall = function (sessionId, workerId, incidentId) {
            this.serverCommunicationHandler.transferCurrentCall(sessionId, workerId, incidentId);
        };
        /**
         * Initiates a request to renew the CTI Capability Token
         * @param sessionId
           */
        TwilioAdapter.prototype.renewCtiToken = function (sessionId) {
            this.serverCommunicationHandler.renewToken(sessionId);
        };
        /**
         * Logs a message at the server side
         *
         * @param sessionId
         * @param message
           */
        TwilioAdapter.prototype.logMessage = function (sessionId, message) {
            this.serverCommunicationHandler.logAuditMessage(sessionId, new Date() + message);
        };
        /**
         * Returns the url of incoming notification audio
         * @returns {string}
           */
        TwilioAdapter.prototype.getRingMediaUrl = function () {
            //TODO - Correct this url
            return 'https://rameshtr.info/ring.mp3';
            //return this.serverUrl + this.ctiConfiguration.providerPath +'/ringMedia';
        };
        return TwilioAdapter;
    }());
    exports.TwilioAdapter = TwilioAdapter;
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoidHdpbGlvQWRhcHRlci5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbInR3aWxpb0FkYXB0ZXIudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7Ozs7O2dHQUtnRzs7OztJQVVoRzs7OztPQUlHO0lBQ0g7UUFRRTtZQUhRLGtCQUFhLEdBQVcsZUFBZSxHQUFHLHlCQUFXLENBQUMsZ0JBQWdCLENBQUM7WUFJN0UsSUFBSSxDQUFDLGdCQUFnQixHQUFxQjtnQkFDeEMsWUFBWSxFQUFFLFFBQVE7Z0JBQ3RCLFlBQVksRUFBRSxRQUFRO2dCQUN0QixhQUFhLEVBQUUsT0FBTztnQkFDdEIsZUFBZSxFQUFFLEtBQUs7YUFDdkIsQ0FBQztZQUNGLElBQUksQ0FBQyxhQUFhLEdBQUcsRUFBRSxDQUFDO1lBQ3hCLElBQUksQ0FBQywwQkFBMEIsR0FBRyxJQUFJLHVEQUEwQixFQUFFLENBQUM7UUFDckUsQ0FBQztRQUNEOzs7OzthQUtLO1FBQ0UsNkJBQUssR0FBWixVQUFhLFdBQXlCO1lBQ3BDLElBQUksUUFBUSxHQUFHLFdBQVcsQ0FBQyxZQUFZLENBQUMsS0FBSyxDQUFDLHNCQUFzQixDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsT0FBTyxDQUFDLGFBQWEsRUFBRSxPQUFPLENBQUMsR0FBRyxJQUFJLENBQUMsZ0JBQWdCLENBQUMsWUFBWSxDQUFDO1lBQzlJLElBQUksQ0FBQyxhQUFhLEdBQUcsRUFBRSxDQUFDO1lBQ3hCLElBQUksQ0FBQywwQkFBMEIsQ0FBQyxLQUFLLENBQUMsUUFBUSxFQUFFLFdBQVcsQ0FBQyxTQUFTLEVBQUUsV0FBVyxDQUFDLFNBQVMsQ0FBQyxDQUFDO1FBQ2hHLENBQUM7UUFFRDs7Ozs7O1dBTUc7UUFDSSx1Q0FBZSxHQUF0QixVQUF1QixTQUFpQixFQUFFLE9BQVk7WUFDcEQsSUFBSSxDQUFDLGFBQWEsQ0FBQyxTQUFTLENBQUMsR0FBRyxPQUFPLENBQUM7WUFDeEMsNENBQTRDO1lBQzVDLElBQUksQ0FBQywwQkFBMEIsQ0FB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LENBQUM7WUFDL0YsSUFBSSxDQUFDLDBCQUEwQixDQUFDLFlBQVksQ0FBQyxZQUFZLEVBQUUsSUFBSSxDQUFDLGdCQUFnQixDQUFDLFlBQVksRUFBRSxTQUFTLENBQUMsQ0FBQztRQUM1RyxDQUFDO1FBRUQ7Ozs7OzthQU1LO1FBQ0Usb0NBQVksR0FBbkIsVUFBb0IsU0FBaUIsRUFBRSxRQUFnQixFQUFFLFVBQW9CO1lBQzNFLElBQUksQ0FBQywwQkFBMEIsQ0FBQyxtQkFBbUIsQ0FBQyxTQUFTLEVBQUUsUUFBUSxFQUFFLFVBQVUsQ0FBQyxDQUFDO1FBQ3ZGLENBQUM7UUFFRDs7O2FBR0s7UUFDRSxxQ0FBYSxHQUFwQixVQUFxQixTQUFpQjtZQUNwQyxJQUFJLENBQUMsMEJBQTBCLENBQUMsVUFBVSxDQUFDLFNBQVMsQ0FBQyxDQUFBO1FBQ3ZELENBQUM7UUFFRDs7Ozs7YUFLSztRQUNFLGtDQUFVLEdBQWpCLFVBQWtCLFNBQWlCLEVBQUUsT0FBZTtZQUNsRCxJQUFJLENBQUMsMEJBQTBCLENBQUMsZUFBZSxDQUFDLFNBQVMsRUFBRSxJQUFJLElBQUksRUFBRSxHQUFHLE9BQU8sQ0FBQyxDQUFDO1FBQ25GLENBQUM7UUFFRDs7O2FBR0s7UUFDRSx1Q0FBZSxHQUF0QjtZQUNFLHlCQUF5QjtZQUN6QixNQUFNLENBQUMsZ0NBQWdDLENBQUM7WUFDeEMsMkVBQTJFO1FBQzdFLENBQUM7UUFDSCxvQkFBQztJQUFELENBQUMsQUFoTUQsSUFnTUM7SUFoTVksc0NBQWEiLCJzb3VyY2VzQ29udGVudCI6WyIvKiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKipcbiAqICAkQUNDRUxFUkFUT1JfSEVBREVSX1BMQUNFX0hPTERFUiRcbiAqICBTSEExOiAkSWQ6IDI5ODVlN2Q2ZGI0NWExMGNkYTRiNDI5NWZhYjllNjg5NmQzMDEwMzAgJFxuICogKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXG4gKiAgRmlsZTogJEFDQ0VMRVJBVE9SX0hFQURFUl9GSUxFX05BTUVfUExBQ0VfSE9MREVSJFxuICogKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqICovXG5cbmltcG9ydCB7SUNUSUFkYXB0ZXJ9IGZyb20gJy4uL2NvbnRyYWN0cy9pQ1RJQWRhcHRlcic7XG5pbXBvcnQge0FnZW50UHJvZmlsZX0gZnJvbSAnLi4vbW9kZWwvYWdlbnRQcm9maWxlJztcbmltcG9ydCB7Q3RpQ29uZmlndXJhdGlvbn0gZnJvbSAnLi4vbW9kZWwvY3RpQ29uZmlndXJhdGlvbic7XG5pbXBvcnQge0N0aUNvbnN0YW50c30gZnJvbSBcIi4uL3V0aWwvY3RpQ29uc3RhbnRzXCI7XG5pbXBvcnQge1R3aWxpb0NvbW11bmljYXRpb25IYW5kbGVyfSBmcm9tIFwiLi90d2lsaW9Db21tdW5pY2F0aW9uSGFuZGxlclwiO1xuaW1wb3J0IHtDdGlMb2dnZXJ9IGZyb20gXCIuLi91dGlsL2N0aUxvZ2dlclwiO1xuaW1wb3J0IHtDdGlNZXNzYWdlc30gZnJvbSBcIi4uL3V0aWwvY3RpTWVzc2FnZXNcIjtcblxuLyoqXG4gKlRoaXMgY2xhc3MgaW1wbGVtZW50cyB0aGUgSUNUSUFkYXB0ZXIgaW50ZXJmYWNlLCB3aGljaCB3aWxsIGJlIHVzZWQgYnkgdGhlXG4gKiBhZGRkaW4gdG8gY29tbXVuaWNhdGUgd2l0aCB1bmRlcmx5aW5nIENUSSBwbGF0Zm9ybVxuICogXG4gKi9cbmV4cG9ydCBjbGFzcyBUd2lsaW9BZGFwdGVyIGltcGxlbWVudHMgSUNUSUFkYXB0ZXIge1xuICBwcml2YXRlIGN0aUNvbmZpZ3VyYXRpb246IEN0aUNvbmZpZ3VyYXRpb247XG4gIHByaXZhdGUgZXZlbnRIYW5kbGVyczoge1trZXk6IHN0cmluZ106IGFueX07XG4gIHByaXZhdGUgc2VydmVyQ29tbXVuaWNhdGlvbkhhbmRsZXI6IFR3aWxpb0NvbW11bmljYXRpb25IYW5kbGVyO1xuICBwcml2YXRlIGlzSGFuZGxlcnNBdHRhY2hlZDogYm9vbGVhbjtcbiAgcHJpdmF0ZSBsb2dQcmVNZXNzYWdlOiBzdHJpbmcgPSAnVHdpbGlvQWRhcHRlcicgKyBDdGlNZXNzYWdlcy5NRVNTQUdFX0FQUEVOREVSO1xuICBwcml2YXRlIHNlcnZlclVybDogc3RyaW5nO1xuXG4gIGNvbnN0cnVjdG9yKCkge1xuICAgIHRoaXMuY3RpQ29uZmlndXJhdGlvbiA9IDxDdGlDb25maWd1cmF0aW9uPntcbiAgICAgIHByb3ZpZGVyTmFtZTogJ1R3aWxpbycsXG4gICAgICBwcm92aWRlclBhdGg6ICdjYy9DVEknLFxuICAgICAgZGVmYXVsdFN0YXR1czogJ1JlYWR5JyxcbiAgICAgIHJlY29yZEJ5RGVmYXVsdDogZmFsc2VcbiAgICB9O1xuICAgIHRoaXMuZXZlbnRIYW5kbGVycyA9IHt9O1xuICAgIHRoaXMuc2VydmVyQ29tbXVuaWNhdGlvbkhhbmRsZXIgPSBuZXcgVHdpbGlvQ29tbXVuaWNhdGlvbkhhbmRsZXIoKTtcbiAgfVxuICAvKipcbiAgICogVGhpcyBmdW5jdGlvbiBoYW5kbGVzIGxvZ2luIHRvIHRoZSB1bmRlcmx5aW5nXG4gICAqIENUSSBUb29sL1BsYXRmb3JtXG4gICAqXG4gICAqIEBwYXJhbSBwcm9maWxlRGF0YVxuICAgICAqL1xuICBwdWJsaWMgbG9naW4ocHJvZmlsZURhdGE6IEFnZW50UHJvZmlsZSk6IHZvaWQge1xuICAgIHZhciBsb2dpblVybCA9IHByb2ZpbGVEYXRhLmludGVyZmFjZVVybC5tYXRjaCgvXlteXFwvXSs6XFwvXFwvW15cXC9dK1xcLy8pWzBdLnJlcGxhY2UoL15odHRwKD8hcykvaSwgJ2h0dHBzJykgKyB0aGlzLmN0aUNvbmZpZ3VyYXRpb24ucHJvdmlkZXJQYXRoO1xuICAgIHRoaXMuZXZlbnRIYW5kbGVycyA9IHt9O1xuICAgIHRoaXMuc2VydmVyQ29tbXVuaWNhdGlvbkhhbmRsZXIubG9naW4obG9naW5VcmwsIHByb2ZpbGVEYXRhLnNlc3Npb25JZCwgcHJvZmlsZURhdGEuYWNjb3VudElkKTtcbiAgfVxuXG4gIC8qKlxuICAgKiBBZGRzIGhhbmRsZXIgZm9yIGFuIGV2ZW50LiBUaGlzIGV2ZW50cyB3aWxsIGJlIHVzZWQgYnkgdGhlIGFkYXB0ZXJcbiAgICogd2hpbGUgbG9nZ2luZyBpbiB0byByZWdpc3RlciB3aXRoIHRoZSB0b29sXG4gICAqXG4gICAqIEBwYXJhbSBldmVudE5hbWVcbiAgICogQHBhcmFtIGhhbmRsZXJcbiAgICovXG4gIHB1YmxpYyBhZGRFdmVudEhhbmRsZXIoZXZlbnROYW1lOiBzdHJpbmcsIGhhbmRsZXI6IGFueSk6IHZvaWQge1xuICAgIHRoaXMuZXZlbnRIYW5kbGVyc1tldmVudE5hbWVdID0gaGFuZGxlcjtcbiAgICAvL0N1cnJlbnRseSBvbmUgaGFuZGxlciBwZXIgZXZlbnQgaXMgYWxsb3dlZFxuICAgIHRoaXMuc2VydmVyQ29tbXVuaWNhdGlvbkhhbmRsZXIuZ2V0U2VydmVyRXZlbnRIYW5kbGVyKClcbiAgICAgICAgLm9mZihldmVudE5hbWUpLm9uKGV2ZW50TmFtZSwgaGFuZGxlcik7XG5cbiAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICsgQ3RpTWVzc2FnZXMuTUVTU0FHRV9IQU5ETEVSX0FEREVEXG4gICAgICArIEN0aU1lc3NhZ2VzLk1FU1NBR0VfQVBQRU5ERVIgKyBldmVudE5hbWUpO1xuICB9XG5cbiAgLyoqXG4gICAqIENsZWFycyBoYW5kbGVyIGFzc29jaWF0ZWQgd2l0aCBhbiBldmVudFxuICAgKlxuICAgKiBAcGFyYW0gZXZlbnROYW1lXG4gICAqL1xuICBwdWJsaWMgY2xlYXJFdmVudEhhbmRsZXIoZXZlbnROYW1lOiBzdHJpbmcpOiB2b2lkIHtcbiAgICB0aGlzLmV2ZW50SGFuZGxlcnNbZXZlbnROYW1lXSA9IG51bGw7XG4gICAgdGhpcy5zZXJ2ZXJDb21tdW5pY2F0aW9uSGFuZGxlci5nZXRTZXJ2ZXJFdmVudEhhbmRsZXIoKS5vZmYoZXZlbnROYW1lKTtcbiAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICtcbiAgICAgICAgQ3RpTWVzc2FnZXMuTUVTU0FHRV9IQU5ETEVSX1JFTU9WRUQgKyBDdGlNZXNzYWdlcy5NRVNTQUdFX0FQUEVOREVSICsgZXZlbnROYW1lKTtcbiAgfVxuXG4gIC8qKlxuICAgKiBDbGVhcnMgYWxsIGV2ZW50IGhhbmRsZXJzIGFkZGVkIHRvIHRoZSBhZGFwdGVyXG4gICAqXG4gICAqL1xuICBwdWJsaWMgY2xlYXJBbGxFdmVudEhhbmRsZXJzKCk6IHZvaWQge1xuICAgIEN0aUxvZ2dlci5sb2dJbmZvTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgKyBDdGlNZXNzYWdlcy5NRVNTQUdFX0FMTF9IQU5ETEVSU19SRU1PVkVEKTtcbiAgICBmb3IodmFyIGtleSBpbiB0aGlzLmV2ZW50SGFuZGxlcnMpIHtcbiAgICAgIGlmKGtleSkge1xuICAgICAgICB0aGlzLmV2ZW50SGFuZGxlcnNba2V5XSA9IG51bGw7XG4gICAgICAgIHRoaXMuc2VydmVyQ29tbXVuaWNhdGlvbkhhbmRsZXIuZ2V0U2VydmVyRXZlbnRIYW5kbGVyKCkub2ZmKGtleSk7XG4gICAgICB9XG4gICAgfVxuICAgIHRoaXMuaXNIYW5kbGVyc0F0dGFjaGVkID0gZmFsc2U7XG4gIH1cblxuICAvKipcbiAgICogVGhpcyBmdW5jdGlvbiBoYW5kbGVzIGxvZ291dCBmcm9tIHRoZSB1bmRlcmx5aW5nXG4gICAqIENUSSBUb29sL1BsYXRmb3JtXG4gICAqXG4gICAqL1xuICBwdWJsaWMgbG9nb3V0KCk6IHZvaWQge1xuICAgIHRoaXMuc2VydmVyQ29tbXVuaWNhdGlvbkhhbmRsZXIubG9nb3V0KCk7XG4gICAgdGhpcy5jbGVhckFsbEV2ZW50SGFuZGxlcnMoKTtcbiAgfVxuXG4gIC8qKlxuICAgKiBUaGlzIGZ1bmN0aW9uIGhhbmRsZXMgYWN0aXZpdHkgdXBkYXRlcyB0byB0aGUgdW5kZXJseWluZ1xuICAgKiBDVEkgVG9vbC9QbGF0Zm9ybVxuICAgKlxuICAgKiBAcGFyYW0gc3RhdHVzXG4gICAqL1xuICBwdWJsaWMgdXBkYXRlQWN0aXZpdHkoc3RhdHVzOiBzdHJpbmcpOiB2b2lkIHtcbiAgICB0aGlzLnNlcnZlckNvbW11bmljYXRpb25IYW5kbGVyLnVwZGF0ZUFjdGl2aXR5KHN0YXR1cyk7XG4gIH1cblxuICAvKipcbiAgICogVGhpcyBmdW5jdGlvbiByZXR1cm5zIHRoZSBiYXNpYyBDVEkgY29uZmlndXJhdGlvbiByZXF1aXJlZCBieSB0aGVcbiAgICogYWRkaW5cbiAgICpcbiAgICogQHJldHVybnMge0N0aUNvbmZpZ3VyYXRpb259XG4gICAqL1xuICBwdWJsaWMgZ2V0Q29uZmlndXJhdGlvbigpOiBDdGlDb25maWd1cmF0aW9uIHtcbiAgICByZXR1cm4gdGhpcy5jdGlDb25maWd1cmF0aW9uO1xuICB9XG5cbiAgLyoqXG4gICAqIFRoaXMgZnVuY3Rpb24gcmV0dXJucyB0aGUgaGFuZGxlciBhc3NvY2lhdGVkIHdpdGggYW4gZXZlbnRcbiAgICpcbiAgICogQHBhcmFtIGV2ZW50TmFtZVxuICAgKiBAcmV0dXJucyB7YW55fVxuICAgKi9cbiAgcHVibGljIGdldEV2ZW50SGFuZGxlcihldmVudE5hbWU6IHN0cmluZyk6IGFueSB7XG4gICAgcmV0dXJuIHRoaXMuZXZlbnRIYW5kbGVyc1tldmVudE5hbWVdO1xuICB9XG5cbiAgLyoqXG4gICAqIHRoaXMgZnVuY3Rpb24gc2VhcmNoIGZvciBhIGNvbnRhY3RcbiAgICpcbiAgICogQHBhcmFtIGNvbnRhY3RcbiAgICogQHBhcmFtIHNlc3Npb25JZFxuICAgKi9cbiAgcHVibGljIHNlYXJjaENvbnRhY3QoY29udGFjdDogc3RyaW5nLCBzZXNzaW9uSWQ6IHN0cmluZyk6IHZvaWQge1xuICAgIHRoaXMuc2VydmVyQ29tbXVuaWNhdGlvbkhhbmRsZXIuc2VhcmNoQ29udGFjdChjb250YWN0LCBzZXNzaW9uSWQpO1xuICB9XG5cbiAgLyoqXG4gICAqIE1ha2UgYW4gb3V0Ym91bmQgY2FsbCB3aXRoIHRoZSBnaXZlblxuICAgKiBjb250YWN0XG4gICAqXG4gICAqIEBwYXJhbSBjb250YWN0XG4gICAgICovXG4gIHB1YmxpYyBkaWFsQU51bWJlcihjb250YWN0OiBzdHJpbmcpOiB2b2lkIHtcbiAgICB0aGlzLnNlcnZlckNvbW11bmljYXRpb25IYW5kbGVyLmRpYWxBTnVtYmVyKGNvbnRhY3QpO1xuICB9XG5cbiAgLyoqXG4gICAqIFNlYXJjaGVzIGZvciBhbGwgYXZhaWxhYmxlIGFnZW50cyBpbiB0aGUgc3lzdGVtXG4gICAqXG4gICAqIEBwYXJhbSBzZXNzaW9uSWRcbiAgICAgKi9cbiAgcHVibGljIHNlYXJjaEF2YWlsYWJsZUFnZW50cyhzZXNzaW9uSWQ6IHN0cmluZyk6IHZvaWQge1xuICAgIHRoaXMuc2VydmVyQ29tbXVuaWNhdGlvbkhhbmRsZXIuZ2V0QXZhaWxhYmxlQWdlbnRzKHNlc3Npb25JZCk7XG4gIH1cblxuICAvKipcbiAgICogQXV0aG9yaXplIGFnZW50IGZvciBDVEkgQWNjZXNzXG4gICAqXG4gICAqIEBwYXJhbSBpbnRlcmZhY2VVcmxcbiAgICogQHBhcmFtIHNlc3Npb25JZFxuICAgICAqL1xuICBwdWJsaWMgYXV0aG9yaXplQWdlbnQoaW50ZXJmYWNlVXJsOiBzdHJpbmcsIHNlc3Npb25JZDogc3RyaW5nKTogdm9pZCB7XG4gICAgdGhpcy5zZXJ2ZXJVcmwgPSBpbnRlcmZhY2VVcmwubWF0Y2goL15bXlxcL10rOlxcL1xcL1teXFwvXStcXC8vKVswXS5yZXBsYWNlKC9eaHR0cCg/IXMpL2ksICdodHRwcycpO1xuICAgIHRoaXMuc2VydmVyQ29tbXVuaWNhdGlvbkhhbmRsZXIuaXNDdGlFbmFibGVkKGludGVyZmFjZVVybCwgdGhpcy5jdGlDb25maWd1cmF0aW9uLnByb3ZpZGVyUGF0aCwgc2Vzc2lvbklkKTtcbiAgfVxuXG4gIC8qKlxuICAgKiBUcmFuc2ZlciBhIGNhbGwgdG8gYW5vdGhlciBhZ2VudChhZ2VudCB3aXRoIGEgZ2l2ZW4gd29ya2VySWQpXG4gICAqXG4gICAqIEBwYXJhbSBzZXNzaW9uSWRcbiAgICogQHBhcmFtIHdvcmtlcklkXG4gICAqIEBwYXJhbSBpbmNpZGVudElkXG4gICAgICovXG4gIHB1YmxpYyB0cmFuc2ZlckNhbGwoc2Vzc2lvbklkOiBzdHJpbmcsIHdvcmtlcklkOiBzdHJpbmcsIGluY2lkZW50SWQgPzogbnVtYmVyKTogdm9pZCB7XG4gICAgdGhpcy5zZXJ2ZXJDb21tdW5pY2F0aW9uSGFuZGxlci50cmFuc2ZlckN1cnJlbnRDYWxsKHNlc3Npb25JZCwgd29ya2VySWQsIGluY2lkZW50SWQpO1xuICB9XG5cbiAgLyoqXG4gICAqIEluaXRpYXRlcyBhIHJlcXVlc3QgdG8gcmVuZXcgdGhlIENUSSBDYXBhYmlsaXR5IFRva2VuXG4gICAqIEBwYXJhbSBzZXNzaW9uSWRcbiAgICAgKi9cbiAgcHVibGljIHJlbmV3Q3RpVG9rZW4oc2Vzc2lvbklkOiBzdHJpbmcpIHtcbiAgICB0aGlzLnNlcnZlckNvbW11bmljYXRpb25IYW5kbGVyLnJlbmV3VG9rZW4oc2Vzc2lvbklkKVxuICB9XG5cbiAgLyoqXG4gICAqIExvZ3MgYSBtZXNzYWdlIGF0IHRoZSBzZXJ2ZXIgc2lkZVxuICAgKlxuICAgKiBAcGFyYW0gc2Vzc2lvbklkXG4gICAqIEBwYXJhbSBtZXNzYWdlXG4gICAgICovXG4gIHB1YmxpYyBsb2dNZXNzYWdlKHNlc3Npb25JZDogc3RyaW5nLCBtZXNzYWdlOiBzdHJpbmcpOiB2b2lkIHtcbiAgICB0aGlzLnNlcnZlckNvbW11bmljYXRpb25IYW5kbGVyLmxvZ0F1ZGl0TWVzc2FnZShzZXNzaW9uSWQsIG5ldyBEYXRlKCkgKyBtZXNzYWdlKTtcbiAgfVxuXG4gIC8qKlxuICAgKiBSZXR1cm5zIHRoZSB1cmwgb2YgaW5jb21pbmcgbm90aWZpY2F0aW9uIGF1ZGlvXG4gICAqIEByZXR1cm5zIHtzdHJpbmd9XG4gICAgICovXG4gIHB1YmxpYyBnZXRSaW5nTWVkaWFVcmwoKTogc3RyaW5nIHtcbiAgICAvL1RPRE8gLSBDb3JyZWN0IHRoaXMgdXJsXG4gICAgcmV0dXJuICdodHRwczovL3JhbWVzaHRyLmluZm8vcmluZy5tcDMnO1xuICAgIC8vcmV0dXJuIHRoaXMuc2VydmVyVXJsICsgdGhpcy5jdGlDb25maWd1cmF0aW9uLnByb3ZpZGVyUGF0aCArJy9yaW5nTWVkaWEnO1xuICB9XG59XG4iXX0=