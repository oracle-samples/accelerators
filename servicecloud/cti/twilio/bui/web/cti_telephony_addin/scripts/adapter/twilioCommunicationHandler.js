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
 *  SHA1: $Id: 2415247e88a6b0fc326b8b343967a25c131c6a20 $
 * *********************************************************************************************
 *  File: twilioCommunicationHandler.js
 * ****************************************************************************************** */
define(["require", "exports", "./serverEventHandler", "jquery", "../util/ctiConstants", "../util/ctiLogger", "../util/ctiMessages"], function (require, exports, serverEventHandler_1, $, ctiConstants_1, ctiLogger_1, ctiMessages_1) {
    "use strict";
    exports.__esModule = true;
    /**
     * TwilioCommunicationHandler - All communication with the custom controller
     *  is handled in this class. Here we submit requests to the server and despatch (publish)
     *  associated events based on result.
     */
    var TwilioCommunicationHandler = /** @class */ (function () {
        function TwilioCommunicationHandler() {
            var _this = this;
            this.activities = {};
            this.loggedIn = false;
            this.firstLogin = true;
            this.logPreMessage = 'TwilioCommunicationHandler' + ctiMessages_1.CtiMessages.MESSAGE_APPENDER;
            /**
             * initializeTwilioDevice - This method initializes the Twilio Device on login success.
             *
             * Attaching handlers for Twilio.Device.incoming and Twilio.Device.connect
             * has to be done only once on first login. Repeated attachments will result
             * in multiple invocation of handlers.
             *
             * @param data
             */
            this.initializeTwilioDevice = function (data) {
                _this.dequeueConfigId = data.config.number;
                _this.deviceToken = data.device;
                _this.workerToken = data.worker;
                _this.routes = data.config.routes;
                _this.ICE = data.ICE;
                if (_this.firstLogin) {
                    ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                        ctiMessages_1.CtiMessages.MESSAGE_DEVICE_INITIALIZE);
                    Twilio.Device.setup(_this.deviceToken, {
                        iceServers: data.ICE
                    });
                    Twilio.Device.incoming(function (conn) {
                        ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                            ctiMessages_1.CtiMessages.MESSAGE_INCOMING_CONNECTION + conn.parameters.From);
                        conn.accept();
                    });
                    Twilio.Device.connect(function (conn) {
                        ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                            ctiMessages_1.CtiMessages.MESSAGE_CONNECTION_ESTABLISHED);
                        var data = {
                            contact: _this.contactInfo,
                            incident: _this.incidentId,
                            hangup: function () {
                                conn.disconnect();
                                _this.updateActivity("Ready");
                            },
                            mute: function (flag) {
                                conn.mute(flag);
                            }
                        };
                        _this.callSid = conn.parameters.CallSid;
                        _this.serverEventHandler.despatch("connected", data);
                    });
                    Twilio.Device.disconnect(function (conn) {
                        ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                            ctiMessages_1.CtiMessages.MESSAGE_CONNECTION_BROKE);
                        _this.attributes = null;
                        _this.incidentId = null;
                        _this.updateActivity("Ready");
                        _this.serverEventHandler.despatch("disconnected", {});
                    });
                }
                if (_this.workerToken == null) {
                    throw ("Worker token is null. Cannot provide access to Twilio for this user.");
                }
                _this.worker = new Twilio.TaskRouter.Worker(_this.workerToken);
                _this.worker.on("ready", function (worker) {
                    ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage + "Worker " + worker.friendlyName + " is ready");
                    _this.loggedIn = true;
                    ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                        ctiMessages_1.CtiMessages.MESSAGE_DISPATCH_LOGIN_SUCCESS);
                    _this.serverEventHandler.despatch('login.success', name);
                    _this.updateActivity("Ready");
                });
                _this.worker.on("reservation.created", function (reservation) {
                    ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                        ctiMessages_1.CtiMessages.MESSAGE_RESERVATION_CREATED);
                    _this.reservation = reservation;
                    var phone = reservation.task.attributes.from;
                    _this.attributes = reservation.task.attributes;
                    phone = phone.replace("client:", "");
                    var timeout = 30;
                    var contact = reservation.task.attributes.contact;
                    _this.incidentId = reservation.task.attributes.incident;
                    contact['name'] = contact.firstName + " " + contact.lastName;
                    _this.contactInfo = contact;
                    var data = {
                        timeout: timeout,
                        accept: function () {
                            clearTimeout(_this.timeoutHandle);
                            reservation.dequeue(_this.dequeueConfigId);
                        },
                        reject: function () {
                            clearTimeout(_this.timeoutHandle);
                            _this.attributes = null;
                            _this.incidentId = null;
                            reservation.reject();
                        }
                    };
                    data["contact"] = contact;
                    _this.timeoutHandle = setTimeout(function () {
                        _this.serverEventHandler.despatch("timeout", {});
                        if (_this.reservation) {
                            _this.reservation.reject();
                            _this.reservation = null;
                        }
                        _this.attributes = null;
                        _this.incidentId = null;
                    }, timeout * 1000);
                    _this.serverEventHandler.despatch("incoming", data);
                });
                _this.worker.on("reservation.canceled", function (reservation) {
                    ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                        ctiMessages_1.CtiMessages.MESSAGE_RESERVATION_CANCELLED);
                    _this.attributes = null;
                    _this.incidentId = null;
                    clearTimeout(_this.timeoutHandle);
                    _this.serverEventHandler.despatch("canceled", {});
                });
                _this.worker.on("reservation.accepted", function (reservation) {
                    clearTimeout(_this.timeoutHandle);
                    ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                        ctiMessages_1.CtiMessages.MESSAGE_RESERVATION_ACCEPTED);
                });
                _this.worker.on("reservation.rejected", function (reservation) {
                    clearTimeout(_this.timeoutHandle);
                    _this.attributes = null;
                    _this.incidentId = null;
                    ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                        ctiMessages_1.CtiMessages.MESSAGE_RESERVATION_REJECTED);
                    _this.serverEventHandler.despatch("canceled", {});
                });
                _this.worker.on("token.expired", function () {
                    ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                        ctiMessages_1.CtiMessages.MESSAGE_TOKEN_EXPIRED);
                    _this.serverEventHandler.despatch('token.expired', {});
                });
                _this.worker.on("activity.update", function (worker) {
                    _this.serverEventHandler.despatch('activity.update', worker.activityName);
                });
                _this.worker.activities.fetch(function (error, activityList) {
                    if (error) {
                        ctiLogger_1.CtiLogger.logErrorMessage(_this.logPreMessage + error.code);
                        ctiLogger_1.CtiLogger.logErrorMessage(_this.logPreMessage + error.message);
                        return;
                    }
                    var data = activityList.data;
                    for (var i = 0; i < data.length; i++) {
                        _this.activities[data[i].friendlyName] = data[i].sid;
                    }
                    _this.worker.update("ActivitySid", _this.activities["Ready"], function (error, worker) {
                        if (error) {
                            ctiLogger_1.CtiLogger.logErrorMessage(_this.logPreMessage + error.code);
                            ctiLogger_1.CtiLogger.logErrorMessage(_this.logPreMessage + error.message);
                        }
                        else {
                            ctiLogger_1.CtiLogger.logInfoMessage(worker.activityName);
                        }
                    });
                });
            };
            this.serverEventHandler = new serverEventHandler_1.ServerEventHandler();
        }
        /**
         * Submits a login request with server. Also
         * it publishes 'login.success' and 'login.failed' events
         *
         *
         * @param serverURI
         * @param sessionId
         * @param agentId
         */
        TwilioCommunicationHandler.prototype.login = function (serverURI, sessionId, agentId) {
            var _this = this;
            this.sessionId = sessionId;
            this.agentId = agentId;
            this.serverURI = serverURI.replace(/\/$/, "");
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_LOGIN);
            $.ajax({
                url: this.serverURI + "/login",
                type: "POST",
                data: {
                    session_id: this.sessionId
                },
                dataType: "JSON"
            })
                .done(function (data) {
                if (data && !data.success) {
                    ctiLogger_1.CtiLogger.logErrorMessage(_this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_LOGIN_ERROR_DEVICE +
                        JSON.stringify(data));
                    _this.serverEventHandler.despatch('login.failed', data);
                    return;
                }
                try {
                    _this.initializeTwilioDevice(data);
                    _this.firstLogin = false;
                }
                catch (exception) {
                    ctiLogger_1.CtiLogger.logErrorMessage(_this.logPreMessage +
                        ctiMessages_1.CtiMessages.MESSAGE_EVENT_DISPATCH + JSON.stringify(exception));
                    _this.serverEventHandler.despatch('login.failed', {});
                }
            })
                .fail(function (failureMessage) {
                ctiLogger_1.CtiLogger.logErrorMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_LOGIN_FAILURE + JSON.stringify(failureMessage));
                _this.serverEventHandler.despatch('login.failed', name);
            });
        };
        /**
         * Submits a status update to the server with
         * status Offline
         */
        TwilioCommunicationHandler.prototype.logout = function () {
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_LOGOUT);
            this.updateActivity(ctiConstants_1.CtiConstants.NOT_AVAILABLE);
            this.workerName = null;
            this.worker = null;
            this.sessionId = null;
            this.agentId = null;
            this.loggedIn = false;
        };
        /**
         * Submits status update request to the server.
         * @param name
         */
        TwilioCommunicationHandler.prototype.updateActivity = function (name) {
            var _this = this;
            if (!this.loggedIn) {
                throw (ctiMessages_1.CtiMessages.MESSAGE_NOT_LOGGEDIN_FOR_ACTION);
            }
            if (name in this.activities) {
                ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_REQUEST_ACTIVITY_UPDATE +
                    ctiMessages_1.CtiMessages.MESSAGE_APPENDER + name);
                this.worker.update("ActivitySid", this.activities[name], function (error, worker) {
                    if (error) {
                        ctiLogger_1.CtiLogger.logErrorMessage(_this.logPreMessage +
                            ctiMessages_1.CtiMessages.MESSAGE_ACTIVITY_UPDATE_ERROR + JSON.stringify(error));
                    }
                    else {
                        ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage + worker.activityName);
                    }
                });
            }
        };
        /**
         * Returns the currently associated
         * instance of ServerEventHandler
         *
         * @returns {ServerEventHandler}
         */
        TwilioCommunicationHandler.prototype.getServerEventHandler = function () {
            return this.serverEventHandler;
        };
        /**
         * Search for a given contact.
         * Publish events 'search.contact.complete' and 'search.contact.failed'
         *
         * @param contact
         * @param sessionId
         */
        TwilioCommunicationHandler.prototype.searchContact = function (contact, sessionId) {
            var _this = this;
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage +
                ctiMessages_1.CtiMessages.MESSAGE_CONTACT_SEARCH +
                ctiMessages_1.CtiMessages.MESSAGE_APPENDER + contact);
            $.ajax({
                url: this.serverURI + '/searchPhone',
                type: 'POST',
                data: {
                    phone: contact,
                    session_id: sessionId
                }
            }).done(function (searchResult) {
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_CONTACT_SEARCH_SUCCESS +
                    ctiMessages_1.CtiMessages.MESSAGE_APPENDER + searchResult);
                var searchResultJson = JSON.parse(searchResult);
                _this.dialedContact = searchResultJson.contact;
                _this.serverEventHandler.despatch('search.contact.complete', searchResultJson);
            }).fail(function (message) {
                ctiLogger_1.CtiLogger.logErrorMessage(_this.logPreMessage + 'Contact search failed >> ' + message);
                _this.serverEventHandler.despatch('search.contact.failed', { success: false });
            });
        };
        /**
         * Make an outbound call
         *
         * @param contact
         */
        TwilioCommunicationHandler.prototype.dialANumber = function (contact) {
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage +
                ctiMessages_1.CtiMessages.MESSAGE_DIALING +
                ctiMessages_1.CtiMessages.MESSAGE_APPENDER + contact);
            Twilio.Device.connect({ To: contact,
                Direction: 'outbound' });
        };
        /**
         * Search for all available agents.
         * Publish event - 'search.agentlist.complete'
         *
         * @param sessionId
         */
        TwilioCommunicationHandler.prototype.getAvailableAgents = function (sessionId) {
            var _this = this;
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage +
                ctiMessages_1.CtiMessages.MESSAGE_SEARCH_AVAILABLE_AGENTS);
            this.sessionId = sessionId;
            $.ajax({
                url: this.serverURI + '/getConnectedAgents',
                type: 'POST',
                data: {
                    session_id: sessionId
                }
            }).done(function (searchResult) {
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_AGENT_SEARCH_SUCCESS +
                    ctiMessages_1.CtiMessages.MESSAGE_APPENDER + searchResult);
                _this.serverEventHandler.despatch('search.agentlist.complete', JSON.parse(searchResult));
            }).fail(function (message) {
                ctiLogger_1.CtiLogger.logErrorMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_AGENT_SEARCH_FAILURE +
                    ctiMessages_1.CtiMessages.MESSAGE_APPENDER + message);
                _this.serverEventHandler.despatch('search.agentlist.complete', []);
            });
        };
        /**
         * Check for CTI access for the given agent.
         * Publish events - 'cti.enabled' and 'cti.disabled'
         *
         * @param interfaceUrl
         * @param servicePath
         * @param sessionId
         */
        TwilioCommunicationHandler.prototype.isCtiEnabled = function (interfaceUrl, servicePath, sessionId) {
            var _this = this;
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_CTI_AUTHORIZE);
            var requestUrl = interfaceUrl.match(/^[^\/]+:\/\/[^\/]+\//)[0].replace(/^http(?!s)/i, 'https')
                + servicePath + '/isCTIEnabled';
            $.ajax({
                url: requestUrl,
                type: 'POST',
                data: {
                    session_id: sessionId
                }
            }).done(function (data) {
                var jsonData = JSON.parse(data);
                if (jsonData && jsonData.enabled) {
                    ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_CTI_ENABLED);
                    _this.serverEventHandler.despatch('cti.enabled', {});
                }
                else {
                    _this.serverEventHandler.despatch('cti.disabled', {});
                    ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_CTI_DISABLED);
                }
            }).fail(function (data) {
                _this.serverEventHandler.despatch('cti.disabled', {});
                ctiLogger_1.CtiLogger.logErrorMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_CTI_AUTHORIZATION_FAILURE +
                    ctiMessages_1.CtiMessages.MESSAGE_APPENDER + data);
            });
        };
        /**
         * Submits transfer call request to the server.
         *
         * @param sessionId
         * @param workerId
         * @param incidentId
         */
        TwilioCommunicationHandler.prototype.transferCurrentCall = function (sessionId, workerId, incidentId) {
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage +
                ctiMessages_1.CtiMessages.MESSAGE_REQUEST_CALL_TRANSFER +
                ctiMessages_1.CtiMessages.MESSAGE_APPENDER + workerId);
            this.sessionId = sessionId;
            var lookup = false;
            if (!this.attributes) {
                lookup = true;
                this.attributes = {
                    callSid: this.callSid,
                    contact: this.dialedContact,
                    incident: null
                };
            }
            if (incidentId) {
                this.attributes.incident = incidentId;
            }
            else {
                this.attributes.incident = null;
            }
            $.ajax({
                url: this.serverURI + '/transferCall',
                type: 'POST',
                data: {
                    session_id: sessionId,
                    attributes: JSON.stringify(this.attributes),
                    worker: workerId,
                    lookup: lookup
                }
            });
        };
        /**
         * Renews the TWILIO capability token for
         * Worker and Device
         *
         * @param sessionId
         */
        TwilioCommunicationHandler.prototype.renewToken = function (sessionId) {
            var _this = this;
            $.ajax({
                type: "POST",
                url: this.serverURI + "/renewTokens",
                data: {
                    session_id: sessionId,
                    tokens: 'device,worker'
                }
            })
                .done(function (data) {
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_UPDATE_DEVICE);
                var jsonData = JSON.parse(data);
                _this.workerToken = jsonData.worker;
                _this.deviceToken = jsonData.device;
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage + jsonData);
                Twilio.Device.setup(_this.deviceToken, {
                    iceServers: _this.ICE
                });
                try {
                    _this.worker.updateToken(_this.workerToken);
                    ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_UPDATE_DEVICE_SUCCESS);
                }
                catch (message) {
                    ctiLogger_1.CtiLogger.logErrorMessage(_this.logPreMessage +
                        ctiMessages_1.CtiMessages.MESSAGE_TOKEN_UPDATE_FAILURE +
                        ctiMessages_1.CtiMessages.MESSAGE_APPENDER + message);
                }
            })
                .fail(function (message) {
                ctiLogger_1.CtiLogger.logErrorMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_TOKEN_UPDATE_FAILURE +
                    ctiMessages_1.CtiMessages.MESSAGE_APPENDER + message);
            });
        };
        ;
        /**
         * Log a given message on server
         *
         * @param sessionId
         * @param message
         */
        TwilioCommunicationHandler.prototype.logAuditMessage = function (sessionId, message) {
            var _this = this;
            $.ajax({
                type: "POST",
                url: this.serverURI + "/logCallAction",
                data: {
                    session_id: sessionId,
                    action: message
                }
            })
                .done(function (data) {
                ctiLogger_1.CtiLogger.logInfoMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_LOG_ACTION +
                    ctiMessages_1.CtiMessages.MESSAGE_APPENDER + message);
            })
                .fail(function (message) {
                ctiLogger_1.CtiLogger.logErrorMessage(_this.logPreMessage +
                    ctiMessages_1.CtiMessages.MESSAGE_LOG_ATION_FAILURE +
                    ctiMessages_1.CtiMessages.MESSAGE_APPENDER + message);
            });
        };
        return TwilioCommunicationHandler;
    }());
    exports.TwilioCommunicationHandler = TwilioCommunicationHandler;
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoidHdpbGlvQ29tbXVuaWNhdGlvbkhhbmRsZXIuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJ0d2lsaW9Db21tdW5pY2F0aW9uSGFuZGxlci50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTs7Ozs7Z0dBS2dHOzs7O0lBV2hHOzs7O09BSUc7SUFDSDtRQTBCSTtZQUFBLGlCQUVDO1lBM0JPLGVBQVUsR0FBUSxFQUFFLENBQUM7WUFDckIsYUFBUSxHQUFZLEtBQUssQ0FBQztZQWdCMUIsZUFBVSxHQUFZLElBQUksQ0FBQztZQU0zQixrQkFBYSxHQUFXLDRCQUE0QixHQUFHLHlCQUFXLENBQUMsZ0JBQWdCLENBQUM7WUFzVTVGOzs7Ozs7OztlQVFHO1lBQ0ssMkJBQXNCLEdBQUcsVUFBQyxJQUFTO2dCQUN2QyxLQUFJLENBQUMsZUFBZSxHQUFHLElBQUksQ0FBQyxNQUFNLENBQUMsTUFBTSxDQUFDO2dCQUMxQyxLQUFJLENBQUMsV0FBVyxHQUFHLElBQUksQ0FBQyxNQUFNLENBQUM7Z0JBQy9CLEtBQUksQ0FBQyxXQUFXLEdBQUcsSUFBSSxDQUFDLE1BQU0sQ0FBQztnQkFDL0IsS0FBSSxDQUFDLE1BQU0sR0FBRyxJQUFJLENBQUMsTUFBTSxDQUFDLE1BQU0sQ0FBQztnQkFDakMsS0FBSSxDQUFDLEdBQUcsR0FBRSxJQUFJLENBQUMsR0FBRyxDQUFDO2dCQUVuQixFQUFFLENBQUEsQ0FBQyxLQUFJLENBQUMsVUFBVSxDQUFDLENBQUEsQ0FBQztvQkFDaEIscUJBQVMsQ0FBQyxjQUFjLENBQUMsS0FBSSxDQUFDLGFBQWE7d0JBQ3ZDLHlCQUFXLENBQUMseUJBQXlCLENBQUUsQ0FBQztvQkFFNUMsTUFBTSxDQUFDLE1BQU0sQ0FBQyxLQUFLLENBQUUsS0FBSSxDQUFDLFdBQVcsRUFBRTt3QkFDbkMsVUFBVSxFQUFFLElBQUksQ0FBQyxHQUFHO3FCQUN2QixDQUFFLENBQUM7b0JBRUosTUFBTSxDQUFDLE1BQU0sQ0FBQyxRQUFRLENBQUUsVUFBQyxJ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VyxDQUFDLHlCQUF5QjtvQkFDckMseUJBQVcsQ0FBQyxnQkFBZ0IsR0FBRyxPQUFPLENBQUMsQ0FBQztZQUNoRCxDQUFDLENBQUMsQ0FBQTtRQUNWLENBQUM7UUFtTEwsaUNBQUM7SUFBRCxDQUFDLEFBOWdCRCxJQThnQkM7SUE5Z0JZLGdFQUEwQiIsInNvdXJjZXNDb250ZW50IjpbIi8qICogKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogICRBQ0NFTEVSQVRPUl9IRUFERVJfUExBQ0VfSE9MREVSJFxuICogIFNIQTE6ICRJZDogMjQxNTI0N2U4OGE2YjBmYzMyNmI4YjM0Mzk2N2EyNWMxMzFjNmEyMCAkXG4gKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKipcbiAqICBGaWxlOiAkQUNDRUxFUkFUT1JfSEVBREVSX0ZJTEVfTkFNRV9QTEFDRV9IT0xERVIkXG4gKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiogKi9cblxuLy8vPHJlZmVyZW5jZSBwYXRoPVwiLi4vLi4vLi4vZGVmaW5pdGlvbnMvdHdpbGlvLmQudHNcIiAvPlxuXG5pbXBvcnQge1NlcnZlckV2ZW50SGFuZGxlcn0gZnJvbSBcIi4vc2VydmVyRXZlbnRIYW5kbGVyXCI7XG5pbXBvcnQgJCA9IHJlcXVpcmUoJ2pxdWVyeScpO1xuaW1wb3J0IHtDdGlDb25zdGFudHN9IGZyb20gXCIuLi91dGlsL2N0aUNvbnN0YW50c1wiO1xuaW1wb3J0IHtDdGlMb2dnZXJ9IGZyb20gXCIuLi91dGlsL2N0aUxvZ2dlclwiO1xuaW1wb3J0IHtDb250YWN0fSBmcm9tIFwiLi4vbW9kZWwvY29udGFjdFwiO1xuaW1wb3J0IHtDdGlNZXNzYWdlc30gZnJvbSBcIi4uL3V0aWwvY3RpTWVzc2FnZXNcIjtcblxuLyoqXG4gKiBUd2lsaW9Db21tdW5pY2F0aW9uSGFuZGxlciAtIEFsbCBjb21tdW5pY2F0aW9uIHdpdGggdGhlIGN1c3RvbSBjb250cm9sbGVyXG4gKiAgaXMgaGFuZGxlZCBpbiB0aGlzIGNsYXNzLiBIZXJlIHdlIHN1Ym1pdCByZXF1ZXN0cyB0byB0aGUgc2VydmVyIGFuZCBkZXNwYXRjaCAocHVibGlzaClcbiAqICBhc3NvY2lhdGVkIGV2ZW50cyBiYXNlZCBvbiByZXN1bHQuXG4gKi9cbmV4cG9ydCBjbGFzcyBUd2lsaW9Db21tdW5pY2F0aW9uSGFuZGxlciB7XG4gICAgcHJpdmF0ZSBhY3Rpdml0aWVzOiBhbnkgPSB7fTtcbiAgICBwcml2YXRlIGxvZ2dlZEluOiBib29sZWFuID0gZmFsc2U7XG5cbiAgICBwcml2YXRlIHdvcmtlck5hbWU6IHN0cmluZztcbiAgICBwcml2YXRlIHNlc3Npb25JZDogc3RyaW5nO1xuICAgIHByaXZhdGUgYWdlbnRJZDogc3RyaW5nO1xuICAgIHByaXZhdGUgc2VydmVyVVJJOiBzdHJpbmc7XG4gICAgcHJpdmF0ZSBkZXZpY2VUb2tlbjogYW55O1xuICAgIHByaXZhdGUgd29ya2VyVG9rZW46IGFueTtcbiAgICBwcml2YXRlIHdvcmtlcjogYW55O1xuICAgIHByaXZhdGUgcmVzZXJ2YXRpb246IGFueTtcbiAgICBwcml2YXRlIGNvbnRhY3RJbmZvOiBhbnk7XG4gICAgcHJpdmF0ZSBzZXJ2ZXJFdmVudEhhbmRsZXI6U2VydmVyRXZlbnRIYW5kbGVyO1xuICAgIHByaXZhdGUgcm91dGVzOiBhbnk7XG4gICAgcHJpdmF0ZSBpbmNpZGVudElkOiBhbnk7XG4gICAgcHJpdmF0ZSBhdHRyaWJ1dGVzOiBhbnk7XG4gICAgcHJpdmF0ZSBkZXF1ZXVlQ29uZmlnSWQ6IHN0cmluZztcbiAgICBwcml2YXRlIGZpcnN0TG9naW46IGJvb2xlYW4gPSB0cnVlO1xuICAgIHByaXZhdGUgdGltZW91dEhhbmRsZTogbnVtYmVyO1xuICAgIHByaXZhdGUgSUNFOiBhbnk7XG4gICAgcHJpdmF0ZSBkaWFsZWRDb250YWN0OiBDb250YWN0O1xuICAgIHByaXZhdGUgY2FsbFNpZDogc3RyaW5nO1xuXG4gICAgcHJpdmF0ZSBsb2dQcmVNZXNzYWdlOiBzdHJpbmcgPSAnVHdpbGlvQ29tbXVuaWNhdGlvbkhhbmRsZXInICsgQ3RpTWVzc2FnZXMuTUVTU0FHRV9BUFBFTkRFUjtcblxuICAgIGNvbnN0cnVjdG9yKCkge1xuICAgICAgICB0aGlzLnNlcnZlckV2ZW50SGFuZGxlciA9IG5ldyBTZXJ2ZXJFdmVudEhhbmRsZXIoKTtcbiAgICB9XG5cbiAgICAvKipcbiAgICAgKiBTdWJtaXRzIGEgbG9naW4gcmVxdWVzdCB3aXRoIHNlcnZlci4gQWxzb1xuICAgICAqIGl0IHB1Ymxpc2hlcyAnbG9naW4uc3VjY2VzcycgYW5kICdsb2dpbi5mYWlsZWQnIGV2ZW50c1xuICAgICAqXG4gICAgICpcbiAgICAgKiBAcGFyYW0gc2VydmVyVVJJXG4gICAgICogQHBhcmFtIHNlc3Npb25JZFxuICAgICAqIEBwYXJhbSBhZ2VudElkXG4gICAgICovXG4gICAgcHVibGljIGxvZ2luKHNlcnZlclVSSSwgc2Vzc2lvbklkLCBhZ2VudElkKTogYW55IHtcbiAgICAgICAgdGhpcy5zZXNzaW9uSWQgPSBzZXNzaW9uSWQ7XG4gICAgICAgIHRoaXMuYWdlbnRJZCA9IGFnZW50SWQ7XG4gICAgICAgIHRoaXMuc2VydmVyVVJJID0gc2VydmVyVVJJLnJlcGxhY2UoL1xcLyQvLCBcIlwiKTtcbiAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArIEN0aU1lc3NhZ2VzLk1FU1NBR0VfTE9HSU4pO1xuXG4gICAgICAgICQuYWpheCh7XG4gICAgICAgICAgICB1cmw6IHRoaXMuc2VydmVyVVJJICsgXCIvbG9naW5cIixcbiAgICAgICAgICAgIHR5cGU6IFwiUE9TVFwiLFxuICAgICAgICAgICAgZGF0YToge1xuICAgICAgICAgICAgICAgIHNlc3Npb25faWQ6IHRoaXMuc2Vzc2lvbklkXG4gICAgICAgICAgICB9LFxuICAgICAgICAgICAgZGF0YVR5cGU6IFwiSlNPTlwiXG4gICAgICAgIH0pXG4gICAgICAgICAgICAuZG9uZSgoZGF0YTogYW55KSA9PiB7XG5cbiAgICAgICAgICAgICAgICBpZihkYXRhICYmICFkYXRhLnN1Y2Nlc3Mpe1xuICAgICAgICAgICAgICAgICAgICBDdGlMb2dnZXIubG9nRXJyb3JNZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArIEN0aU1lc3NhZ2VzLk1FU1NBR0VfTE9HSU5fRVJST1JfREVWSUNFICtcbiAgICAgICAgICAgICAgICAgICAgICAgIEpTT04uc3RyaW5naWZ5KGRhdGEpKTtcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5zZXJ2ZXJFdmVudEhhbmRsZXIuZGVzcGF0Y2goICdsb2dpbi5mYWlsZWQnLCBkYXRhICk7XG4gICAgICAgICAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgdHJ5e1xuICAgICAgICAgICAgICAgICAgICB0aGlzLmluaXRpYWxpemVUd2lsaW9EZXZpY2UoZGF0YSk7XG4gICAgICAgICAgICAgICAgICAgIHRoaXMuZmlyc3RMb2dpbiA9IGZhbHNlO1xuICAgICAgICAgICAgICAgIH1jYXRjaChleGNlcHRpb24pIHtcbiAgICAgICAgICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0Vycm9yTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgK1xuICAgICAgICAgICAgICAgICAgICAgICAgQ3RpTWVzc2FnZXMuTUVTU0FHRV9FVkVOVF9ESVNQQVRDSCArIEpTT04uc3RyaW5naWZ5KGV4Y2VwdGlvbikpO1xuICAgICAgICAgICAgICAgICAgICB0aGlzLnNlcnZlckV2ZW50SGFuZGxlci5kZXNwYXRjaCggJ2xvZ2luLmZhaWxlZCcsIHt9ICk7XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgfSlcbiAgICAgICAgICAgIC5mYWlsKCAoZmFpbHVyZU1lc3NhZ2U6IGFueSkgPT4ge1xuICAgICAgICAgICAgICAgIEN0aUxvZ2dlci5sb2dFcnJvck1lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICtcbiAgICAgICAgICAgICAgICAgICAgQ3RpTWVzc2FnZXMuTUVTU0FHRV9MT0dJTl9GQUlMVVJFICsgSlNPTi5zdHJpbmdpZnkoZmFpbHVyZU1lc3NhZ2UpKTtcbiAgICAgICAgICAgICAgICB0aGlzLnNlcnZlckV2ZW50SGFuZGxlci5kZXNwYXRjaCggJ2xvZ2luLmZhaWxlZCcsIG5hbWUgKTtcbiAgICAgICAgfSk7XG5cbiAgICB9XG5cbiAgICAvKipcbiAgICAgKiBTdWJtaXRzIGEgc3RhdHVzIHVwZGF0ZSB0byB0aGUgc2VydmVyIHdpdGhcbiAgICAgKiBzdGF0dXMgT2ZmbGluZVxuICAgICAqL1xuICAgIHB1YmxpYyBsb2dvdXQoKTogdm9pZCB7XG4gICAgICAgIEN0aUxvZ2dlci5sb2dJbmZvTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgKyBDdGlNZXNzYWdlcy5NRVNTQUdFX0xPR09VVCk7XG4gICAgICAgIHRoaXMudXBkYXRlQWN0aXZpdHkoQ3RpQ29uc3RhbnRzLk5PVF9BVkFJTEFCTEUpO1xuICAgICAgICB0aGlzLndvcmtlck5hbWUgPSBudWxsO1xuICAgICAgICB0aGlzLndvcmtlciA9IG51bGw7XG4gICAgICAgIHRoaXMuc2Vzc2lvbklkID0gbnVsbDtcbiAgICAgICAgdGhpcy5hZ2VudElkID0gbnVsbDtcbiAgICAgICAgdGhpcy5sb2dnZWRJbiA9IGZhbHNlO1xuICAgIH1cblxuICAgIC8qKlxuICAgICAqIFN1Ym1pdHMgc3RhdHVzIHVwZGF0ZSByZXF1ZXN0IHRvIHRoZSBzZXJ2ZXIuXG4gICAgICogQHBhcmFtIG5hbWVcbiAgICAgKi9cbiAgICBwdWJsaWMgdXBkYXRlQWN0aXZpdHkobmFtZTogc3RyaW5nKTogdm9pZCB7XG4gICAgICAgIGlmKCAhdGhpcy5sb2dnZWRJbiApXG4gICAgICAgIHtcbiAgICAgICAgICAgIHRocm93KEN0aU1lc3NhZ2VzLk1FU1NBR0VfTk9UX0xPR0dFRElOX0ZPUl9BQ1RJT04pO1xuICAgICAgICB9XG4gICAgICAgIGlmKCBuYW1lIGluIHRoaXMuYWN0aXZpdGllcyApXG4gICAgICAgIHtcbiAgICAgICAgICAgIEN0aUxvZ2dlci5sb2dJbmZvTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgK1xuICAgICAgICAgICAgICAgIEN0aU1lc3NhZ2VzLk1FU1NBR0VfUkVRVUVTVF9BQ1RJVklUWV9VUERBVEUgK1xuICAgICAgICAgICAgICAgIEN0aU1lc3NhZ2VzLk1FU1NBR0VfQVBQRU5ERVIgKyBuYW1lKTtcbiAgICAgICAgICAgIHRoaXMud29ya2VyLnVwZGF0ZShcIkFjdGl2aXR5U2lkXCIsIHRoaXMuYWN0aXZpdGllc1tuYW1lXSwgKGVycm9yLCB3b3JrZXIpID0+IHtcbiAgICAgICAgICAgICAgICBpZihlcnJvcikge1xuICAgICAgICAgICAgICAgICAgICBDdGlMb2dnZXIubG9nRXJyb3JNZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArXG4gICAgICAgICAgICAgICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX0FDVElWSVRZX1VQREFURV9FUlJPUiArIEpTT04uc3RyaW5naWZ5KGVycm9yKSk7XG4gICAgICAgICAgICAgICAgfSBlbHNlIHtcbiAgICAgICAgICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArIHdvcmtlci5hY3Rpdml0eU5hbWUpO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgIH0pO1xuICAgICAgICB9XG4gICAgfVxuXG4gICAgLyoqXG4gICAgICogUmV0dXJucyB0aGUgY3VycmVudGx5IGFzc29jaWF0ZWRcbiAgICAgKiBpbnN0YW5jZSBvZiBTZXJ2ZXJFdmVudEhhbmRsZXJcbiAgICAgKlxuICAgICAqIEByZXR1cm5zIHtTZXJ2ZXJFdmVudEhhbmRsZXJ9XG4gICAgICovXG4gICAgcHVibGljIGdldFNlcnZlckV2ZW50SGFuZGxlcigpOlNlcnZlckV2ZW50SGFuZGxlciB7XG4gICAgICAgIHJldHVybiB0aGlzLnNlcnZlckV2ZW50SGFuZGxlcjtcbiAgICB9XG5cbiAgICAvKipcbiAgICAgKiBTZWFyY2ggZm9yIGEgZ2l2ZW4gY29udGFjdC5cbiAgICAgKiBQdWJsaXNoIGV2ZW50cyAnc2VhcmNoLmNvbnRhY3QuY29tcGxldGUnIGFuZCAnc2VhcmNoLmNvbnRhY3QuZmFpbGVkJ1xuICAgICAqXG4gICAgICogQHBhcmFtIGNvbnRhY3RcbiAgICAgKiBAcGFyYW0gc2Vzc2lvbklkXG4gICAgICovXG4gICAgcHVibGljIHNlYXJjaENvbnRhY3QoY29udGFjdDogc3RyaW5nLCBzZXNzaW9uSWQ6IHN0cmluZyk6IHZvaWQge1xuICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICtcbiAgICAgICAgICAgIEN0aU1lc3NhZ2VzLk1FU1NBR0VfQ09OVEFDVF9TRUFSQ0ggK1xuICAgICAgICAgICAgQ3RpTWVzc2FnZXMuTUVTU0FHRV9BUFBFTkRFUiArIGNvbnRhY3QpO1xuICAgICAgICAkLmFqYXgoe1xuICAgICAgICAgICAgdXJsOiB0aGlzLnNlcnZlclVSSSArICcvc2VhcmNoUGhvbmUnLFxuICAgICAgICAgICAgdHlwZTogJ1BPU1QnLFxuICAgICAgICAgICAgZGF0YToge1xuICAgICAgICAgICAgICAgIHBob25lOiBjb250YWN0LFxuICAgICAgICAgICAgICAgIHNlc3Npb25faWQ6IHNlc3Npb25JZFxuICAgICAgICAgICAgfVxuICAgICAgICB9KS5kb25lKChzZWFyY2hSZXN1bHQ6IGFueSkgPT4ge1xuICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArXG4gICAgICAgICAgICAgICAgQ3RpTWVzc2FnZXMuTUVTU0FHRV9DT05UQUNUX1NFQVJDSF9TVUNDRVNTICtcbiAgICAgICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX0FQUEVOREVSICsgc2VhcmNoUmVzdWx0KTtcbiAgICAgICAgICAgIHZhciBzZWFyY2hSZXN1bHRKc29uOiBhbnkgPSBKU09OLnBhcnNlKHNlYXJjaFJlc3VsdCk7XG4gICAgICAgICAgICB0aGlzLmRpYWxlZENvbnRhY3QgPSBzZWFyY2hSZXN1bHRKc29uLmNvbnRhY3Q7XG4gICAgICAgICAgICB0aGlzLnNlcnZlckV2ZW50SGFuZGxlci5kZXNwYXRjaCgnc2VhcmNoLmNvbnRhY3QuY29tcGxldGUnLCBzZWFyY2hSZXN1bHRKc29uKTtcbiAgICAgICAgfSkuZmFpbCgobWVzc2FnZTogYW55KSA9PiB7XG4gICAgICAgICAgICBDdGlMb2dnZXIubG9nRXJyb3JNZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArICdDb250YWN0IHNlYXJjaCBmYWlsZWQgPj4gJyttZXNzYWdlKTtcbiAgICAgICAgICAgIHRoaXMuc2VydmVyRXZlbnRIYW5kbGVyLmRlc3BhdGNoKCdzZWFyY2guY29udGFjdC5mYWlsZWQnLCB7c3VjY2VzczogZmFsc2V9KTtcbiAgICAgICAgfSk7XG4gICAgfVxuXG4gICAgLyoqXG4gICAgICogTWFrZSBhbiBvdXRib3VuZCBjYWxsXG4gICAgICpcbiAgICAgKiBAcGFyYW0gY29udGFjdFxuICAgICAqL1xuICAgIHB1YmxpYyBkaWFsQU51bWJlcihjb250YWN0OiBzdHJpbmcpOiB2b2lkIHtcbiAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArXG4gICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX0RJQUxJTkcgK1xuICAgICAgICAgICAgQ3RpTWVzc2FnZXMuTUVTU0FHRV9BUFBFTkRFUiArIGNvbnRhY3QpO1xuICAgICAgICBUd2lsaW8uRGV2aWNlLmNvbm5lY3Qoe1RvOiBjb250YWN0LFxuICAgICAgICBEaXJlY3Rpb246ICdvdXRib3VuZCd9KTtcbiAgICB9XG5cblxuICAgIC8qKlxuICAgICAqIFNlYXJjaCBmb3IgYWxsIGF2YWlsYWJsZSBhZ2VudHMuXG4gICAgICogUHVibGlzaCBldmVudCAtICdzZWFyY2guYWdlbnRsaXN0LmNvbXBsZXRlJ1xuICAgICAqXG4gICAgICogQHBhcmFtIHNlc3Npb25JZFxuICAgICAqL1xuICAgIHB1YmxpYyBnZXRBdmFpbGFibGVBZ2VudHMoc2Vzc2lvbklkOiBzdHJpbmcpOiB2b2lkIHtcbiAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArXG4gICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX1NFQVJDSF9BVkFJTEFCTEVfQUdFTlRTKTtcbiAgICAgICAgdGhpcy5zZXNzaW9uSWQgPSBzZXNzaW9uSWQ7XG4gICAgICAgICQuYWpheCh7XG4gICAgICAgICAgICB1cmw6IHRoaXMuc2VydmVyVVJJICsgJy9nZXRDb25uZWN0ZWRBZ2VudHMnLFxuICAgICAgICAgICAgdHlwZTogJ1BPU1QnLFxuICAgICAgICAgICAgZGF0YToge1xuICAgICAgICAgICAgICAgIHNlc3Npb25faWQ6IHNlc3Npb25JZFxuICAgICAgICAgICAgfVxuICAgICAgICB9KS5kb25lKChzZWFyY2hSZXN1bHQ6IHN0cmluZykgPT4ge1xuICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArXG4gICAgICAgICAgICAgICAgQ3RpTWVzc2FnZXMuTUVTU0FHRV9BR0VOVF9TRUFSQ0hfU1VDQ0VTUyArXG4gICAgICAgICAgICAgICAgQ3RpTWVzc2FnZXMuTUVTU0FHRV9BUFBFTkRFUiArIHNlYXJjaFJlc3VsdCk7XG4gICAgICAgICAgICB0aGlzLnNlcnZlckV2ZW50SGFuZGxlci5kZXNwYXRjaCgnc2VhcmNoLmFnZW50bGlzdC5jb21wbGV0ZScsIEpTT04ucGFyc2Uoc2VhcmNoUmVzdWx0KSk7XG4gICAgICAgIH0pLmZhaWwoKG1lc3NhZ2U6IGFueSkgPT4ge1xuICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0Vycm9yTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgK1xuICAgICAgICAgICAgICAgIEN0aU1lc3NhZ2VzLk1FU1NBR0VfQUdFTlRfU0VBUkNIX0ZBSUxVUkUgK1xuICAgICAgICAgICAgICAgIEN0aU1lc3NhZ2VzLk1FU1NBR0VfQVBQRU5ERVIgKyBtZXNzYWdlKTtcbiAgICAgICAgICAgIHRoaXMuc2VydmVyRXZlbnRIYW5kbGVyLmRlc3BhdGNoKCdzZWFyY2guYWdlbnRsaXN0LmNvbXBsZXRlJywgW10pO1xuICAgICAgICB9KTtcbiAgICB9XG5cbiAgICAvKipcbiAgICAgKiBDaGVjayBmb3IgQ1RJIGFjY2VzcyBmb3IgdGhlIGdpdmVuIGFnZW50LlxuICAgICAqIFB1Ymxpc2ggZXZlbnRzIC0gJ2N0aS5lbmFibGVkJyBhbmQgJ2N0aS5kaXNhYmxlZCdcbiAgICAgKlxuICAgICAqIEBwYXJhbSBpbnRlcmZhY2VVcmxcbiAgICAgKiBAcGFyYW0gc2VydmljZVBhdGhcbiAgICAgKiBAcGFyYW0gc2Vzc2lvbklkXG4gICAgICovXG4gICAgcHVibGljIGlzQ3RpRW5hYmxlZChpbnRlcmZhY2VVcmw6IHN0cmluZywgc2VydmljZVBhdGg6IHN0cmluZywgc2Vzc2lvbklkOiBzdHJpbmcpOiB2b2lkIHtcbiAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArIEN0aU1lc3NhZ2VzLk1FU1NBR0VfQ1RJX0FVVEhPUklaRSk7XG4gICAgICAgIHZhciByZXF1ZXN0VXJsID0gaW50ZXJmYWNlVXJsLm1hdGNoKC9eW15cXC9dKzpcXC9cXC9bXlxcL10rXFwvLylbMF0ucmVwbGFjZSgvXmh0dHAoPyFzKS9pLCAnaHR0cHMnKVxuICAgICAgICAgICAgKyBzZXJ2aWNlUGF0aCArICcvaXNDVElFbmFibGVkJztcblxuICAgICAgICAkLmFqYXgoe1xuICAgICAgICAgICAgdXJsOiByZXF1ZXN0VXJsLFxuICAgICAgICAgICAgdHlwZTogJ1BPU1QnLFxuICAgICAgICAgICAgZGF0YToge1xuICAgICAgICAgICAgICAgIHNlc3Npb25faWQ6IHNlc3Npb25JZFxuICAgICAgICAgICAgfVxuICAgICAgICB9KS5kb25lKChkYXRhOiBzdHJpbmcpID0+IHtcbiAgICAgICAgICAgIHZhciBqc29uRGF0YTogYW55ID0gSlNPTi5wYXJzZShkYXRhKTtcbiAgICAgICAgICAgIGlmKGpzb25EYXRhICYmIGpzb25EYXRhLmVuYWJsZWQpe1xuICAgICAgICAgICAgICAgIEN0aUxvZ2dlci5sb2dJbmZvTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgKyBDdGlNZXNzYWdlcy5NRVNTQUdFX0NUSV9FTkFCTEVEKTtcbiAgICAgICAgICAgICAgICB0aGlzLnNlcnZlckV2ZW50SGFuZGxlci5kZXNwYXRjaCgnY3RpLmVuYWJsZWQnLCB7fSk7XG4gICAgICAgICAgICB9ZWxzZXtcbiAgICAgICAgICAgICAgICB0aGlzLnNlcnZlckV2ZW50SGFuZGxlci5kZXNwYXRjaCgnY3RpLmRpc2FibGVkJywge30pO1xuICAgICAgICAgICAgICAgIEN0aUxvZ2dlci5sb2dJbmZvTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgKyBDdGlNZXNzYWdlcy5NRVNTQUdFX0NUSV9ESVNBQkxFRCk7XG4gICAgICAgICAgICB9XG4gICAgICAgIH0pLmZhaWwoKGRhdGE6IGFueSkgPT4ge1xuICAgICAgICAgICAgdGhpcy5zZXJ2ZXJFdmVudEhhbmRsZXIuZGVzcGF0Y2goJ2N0aS5kaXNhYmxlZCcsIHt9KTtcbiAgICAgICAgICAgIEN0aUxvZ2dlci5sb2dFcnJvck1lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICtcbiAgICAgICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX0NUSV9BVVRIT1JJWkFUSU9OX0ZBSUxVUkUgK1xuICAgICAgICAgICAgICAgIEN0aU1lc3NhZ2VzLk1FU1NBR0VfQVBQRU5ERVIgKyBkYXRhKTtcbiAgICAgICAgfSk7XG4gICAgfVxuXG4gICAgLyoqXG4gICAgICogU3VibWl0cyB0cmFuc2ZlciBjYWxsIHJlcXVlc3QgdG8gdGhlIHNlcnZlci5cbiAgICAgKlxuICAgICAqIEBwYXJhbSBzZXNzaW9uSWRcbiAgICAgKiBAcGFyYW0gd29ya2VySWRcbiAgICAgKiBAcGFyYW0gaW5jaWRlbnRJZFxuICAgICAqL1xuICAgIHB1YmxpYyB0cmFuc2ZlckN1cnJlbnRDYWxsKHNlc3Npb25JZDogc3RyaW5nLCB3b3JrZXJJZDogc3RyaW5nLCBpbmNpZGVudElkOiBudW1iZXIpOiB2b2lkIHtcbiAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArXG4gICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX1JFUVVFU1RfQ0FMTF9UUkFOU0ZFUiArXG4gICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX0FQUEVOREVSICsgd29ya2VySWQpO1xuICAgICAgICB0aGlzLnNlc3Npb25JZCA9IHNlc3Npb25JZDtcbiAgICAgICAgdmFyIGxvb2t1cDogYm9vbGVhbiA9IGZhbHNlO1xuICAgICAgICBpZighdGhpcy5hdHRyaWJ1dGVzKXsgLy9PdXRib3VuZCBjYWxsXG4gICAgICAgICAgICBsb29rdXAgPSB0cnVlO1xuICAgICAgICAgICAgdGhpcy5hdHRyaWJ1dGVzID0ge1xuICAgICAgICAgICAgICAgIGNhbGxTaWQ6IHRoaXMuY2FsbFNpZCxcbiAgICAgICAgICAgICAgICBjb250YWN0OiB0aGlzLmRpYWxlZENvbnRhY3QsXG4gICAgICAgICAgICAgICAgaW5jaWRlbnQ6IG51bGxcbiAgICAgICAgICAgIH1cbiAgICAgICAgfVxuXG4gICAgICAgIGlmKGluY2lkZW50SWQpe1xuICAgICAgICAgICAgdGhpcy5hdHRyaWJ1dGVzLmluY2lkZW50ID0gaW5jaWRlbnRJZDtcbiAgICAgICAgfWVsc2V7XG4gICAgICAgICAgICB0aGlzLmF0dHJpYnV0ZXMuaW5jaWRlbnQgPSBudWxsO1xuICAgICAgICB9XG5cbiAgICAgICAgJC5hamF4KHtcbiAgICAgICAgICAgIHVybDogdGhpcy5zZXJ2ZXJVUkkgKyAnL3RyYW5zZmVyQ2FsbCcsXG4gICAgICAgICAgICB0eXBlOiAnUE9TVCcsXG4gICAgICAgICAgICBkYXRhOiB7XG4gICAgICAgICAgICAgICAgc2Vzc2lvbl9pZDogc2Vzc2lvbklkLFxuICAgICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IEpTT04uc3RyaW5naWZ5KHRoaXMuYXR0cmlidXRlcyksXG4gICAgICAgICAgICAgICAgd29ya2VyOiB3b3JrZXJJZCxcbiAgICAgICAgICAgICAgICBsb29rdXA6IGxvb2t1cFxuICAgICAgICAgICAgfVxuICAgICAgICB9KTtcbiAgICB9XG5cbiAgICAvKipcbiAgICAgKiBSZW5ld3MgdGhlIFRXSUxJTyBjYXBhYmlsaXR5IHRva2VuIGZvclxuICAgICAqIFdvcmtlciBhbmQgRGV2aWNlXG4gICAgICpcbiAgICAgKiBAcGFyYW0gc2Vzc2lvbklkXG4gICAgICovXG4gICAgcHVibGljIHJlbmV3VG9rZW4gKHNlc3Npb25JZDogc3RyaW5nKTogdm9pZHtcbiAgICAgICAgJC5hamF4KHtcbiAgICAgICAgICAgICAgICB0eXBlOiBcIlBPU1RcIixcbiAgICAgICAgICAgICAgICB1cmw6IHRoaXMuc2VydmVyVVJJICsgXCIvcmVuZXdUb2tlbnNcIixcbiAgICAgICAgICAgICAgICBkYXRhOiB7XG4gICAgICAgICAgICAgICAgICAgIHNlc3Npb25faWQ6IHNlc3Npb25JZCxcbiAgICAgICAgICAgICAgICAgICAgdG9rZW5zOiAnZGV2aWNlLHdvcmtlcidcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICB9KVxuICAgICAgICAgICAgLmRvbmUoIChkYXRhOiBhbnkpID0+IHtcbiAgICAgICAgICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICsgQ3RpTWVzc2FnZXMuTUVTU0FHRV9VUERBVEVfREVWSUNFKTtcbiAgICAgICAgICAgICAgICBcbiAgICAgICAgICAgICAgICB2YXIganNvbkRhdGE6IGFueSA9IEpTT04ucGFyc2UoZGF0YSk7XG4gICAgICAgICAgICAgICAgdGhpcy53b3JrZXJUb2tlbiA9IGpzb25EYXRhLndvcmtlcjtcbiAgICAgICAgICAgICAgICB0aGlzLmRldmljZVRva2VuID0ganNvbkRhdGEuZGV2aWNlO1xuXG4gICAgICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArIGpzb25EYXRhKTtcblxuICAgICAgICAgICAgICAgIFR3aWxpby5EZXZpY2Uuc2V0dXAodGhpcy5kZXZpY2VUb2tlbiwge1xuICAgICAgICAgICAgICAgICAgICBpY2VTZXJ2ZXJzOiB0aGlzLklDRVxuICAgICAgICAgICAgICAgIH0pO1xuXG4gICAgICAgICAgICAgICAgdHJ5e1xuICAgICAgICAgICAgICAgICAgICB0aGlzLndvcmtlci51cGRhdGVUb2tlbiggdGhpcy53b3JrZXJUb2tlbiApO1xuICAgICAgICAgICAgICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICsgQ3RpTWVzc2FnZXMuTUVTU0FHRV9VUERBVEVfREVWSUNFX1NVQ0NFU1MpO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICBjYXRjaChtZXNzYWdlKXtcbiAgICAgICAgICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0Vycm9yTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgK1xuICAgICAgICAgICAgICAgICAgICAgICAgQ3RpTWVzc2FnZXMuTUVTU0FHRV9UT0tFTl9VUERBVEVfRkFJTFVSRSArXG4gICAgICAgICAgICAgICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX0FQUEVOREVSICsgbWVzc2FnZSk7XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgfSlcbiAgICAgICAgICAgIC5mYWlsKChtZXNzYWdlOiBhbnkpID0+IHtcbiAgICAgICAgICAgICAgICBDdGlMb2dnZXIubG9nRXJyb3JNZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArXG4gICAgICAgICAgICAgICAgICAgIEN0aU1lc3NhZ2VzLk1FU1NBR0VfVE9LRU5fVVBEQVRFX0ZBSUxVUkUgK1xuICAgICAgICAgICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX0FQUEVOREVSICsgbWVzc2FnZSk7XG4gICAgICAgICAgICB9KVxuICAgIH07XG5cbiAgICAvKipcbiAgICAgKiBMb2cgYSBnaXZlbiBtZXNzYWdlIG9uIHNlcnZlclxuICAgICAqXG4gICAgICogQHBhcmFtIHNlc3Npb25JZFxuICAgICAqIEBwYXJhbSBtZXNzYWdlXG4gICAgICovXG4gICAgcHVibGljIGxvZ0F1ZGl0TWVzc2FnZShzZXNzaW9uSWQ6IHN0cmluZywgbWVzc2FnZTogc3RyaW5nKSB7XG4gICAgICAgICQuYWpheCh7XG4gICAgICAgICAgICAgICAgdHlwZTogXCJQT1NUXCIsXG4gICAgICAgICAgICAgICAgdXJsOiB0aGlzLnNlcnZlclVSSSArIFwiL2xvZ0NhbGxBY3Rpb25cIixcbiAgICAgICAgICAgICAgICBkYXRhOiB7XG4gICAgICAgICAgICAgICAgICAgIHNlc3Npb25faWQ6IHNlc3Npb25JZCxcbiAgICAgICAgICAgICAgICAgICAgYWN0aW9uOiBtZXNzYWdlXG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgfSlcbiAgICAgICAgICAgIC5kb25lKCAoZGF0YTogYW55KSA9PiB7XG4gICAgICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArXG4gICAgICAgICAgICAgICAgICAgIEN0aU1lc3NhZ2VzLk1FU1NBR0VfTE9HX0FDVElPTiArXG4gICAgICAgICAgICAgICAgICAgIEN0aU1lc3NhZ2VzLk1FU1NBR0VfQVBQRU5ERVIgKyBtZXNzYWdlKTtcbiAgICAgICAgICAgIH0pXG4gICAgICAgICAgICAuZmFpbCgobWVzc2FnZTogYW55KSA9PiB7XG4gICAgICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0Vycm9yTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgK1xuICAgICAgICAgICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX0xPR19BVElPTl9GQUlMVVJFICtcbiAgICAgICAgICAgICAgICAgICAgQ3RpTWVzc2FnZXMuTUVTU0FHRV9BUFBFTkRFUiArIG1lc3NhZ2UpO1xuICAgICAgICAgICAgfSlcbiAgICB9XG5cblxuICAgIC8qKlxuICAgICAqIGluaXRpYWxpemVUd2lsaW9EZXZpY2UgLSBUaGlzIG1ldGhvZCBpbml0aWFsaXplcyB0aGUgVHdpbGlvIERldmljZSBvbiBsb2dpbiBzdWNjZXNzLlxuICAgICAqXG4gICAgICogQXR0YWNoaW5nIGhhbmRsZXJzIGZvciBUd2lsaW8uRGV2aWNlLmluY29taW5nIGFuZCBUd2lsaW8uRGV2aWNlLmNvbm5lY3RcbiAgICAgKiBoYXMgdG8gYmUgZG9uZSBvbmx5IG9uY2Ugb24gZmlyc3QgbG9naW4uIFJlcGVhdGVkIGF0dGFjaG1lbnRzIHdpbGwgcmVzdWx0XG4gICAgICogaW4gbXVsdGlwbGUgaW52b2NhdGlvbiBvZiBoYW5kbGVycy5cbiAgICAgKlxuICAgICAqIEBwYXJhbSBkYXRhXG4gICAgICovXG4gICAgcHJpdmF0ZSBpbml0aWFsaXplVHdpbGlvRGV2aWNlID0gKGRhdGE6IGFueSkgPT4ge1xuICAgICAgICB0aGlzLmRlcXVldWVDb25maWdJZCA9IGRhdGEuY29uZmlnLm51bWJlcjtcbiAgICAgICAgdGhpcy5kZXZpY2VUb2tlbiA9IGRhdGEuZGV2aWNlO1xuICAgICAgICB0aGlzLndvcmtlclRva2VuID0gZGF0YS53b3JrZXI7XG4gICAgICAgIHRoaXMucm91dGVzID0gZGF0YS5jb25maWcucm91dGVzO1xuICAgICAgICB0aGlzLklDRSA9ZGF0YS5JQ0U7XG5cbiAgICAgICAgaWYodGhpcy5maXJzdExvZ2luKXtcbiAgICAgICAgICAgIEN0aUxvZ2dlci5sb2dJbmZvTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgK1xuICAgICAgICAgICAgICAgIEN0aU1lc3NhZ2VzLk1FU1NBR0VfREVWSUNFX0lOSVRJQUxJWkUgKTtcblxuICAgICAgICAgICAgVHdpbGlvLkRldmljZS5zZXR1cCggdGhpcy5kZXZpY2VUb2tlbiwge1xuICAgICAgICAgICAgICAgIGljZVNlcnZlcnM6IGRhdGEuSUNFXG4gICAgICAgICAgICB9ICk7XG5cbiAgICAgICAgICAgIFR3aWxpby5EZXZpY2UuaW5jb21pbmcoIChjb25uOiBhbnkpID0+IHtcbiAgICAgICAgICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICtcbiAgICAgICAgICAgICAgICAgICAgQ3RpTWVzc2FnZXMuTUVTU0FHRV9JTkNPTUlOR19DT05ORUNUSU9OKyBjb25uLnBhcmFtZXRlcnMuRnJvbSk7XG4gICAgICAgICAgICAgICAgY29ubi5hY2NlcHQoKTtcbiAgICAgICAgICAgIH0pO1xuXG4gICAgICAgICAgICBUd2lsaW8uRGV2aWNlLmNvbm5lY3QoIChjb25uOiBhbnkpID0+IHtcbiAgICAgICAgICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICtcbiAgICAgICAgICAgICAgICAgICAgQ3RpTWVzc2FnZXMuTUVTU0FHRV9DT05ORUNUSU9OX0VTVEFCTElTSEVEKTtcbiAgICAgICAgICAgICAgICB2YXIgZGF0YSA9IHtcbiAgICAgICAgICAgICAgICAgICAgY29udGFjdDogdGhpcy5jb250YWN0SW5mbyxcbiAgICAgICAgICAgICAgICAgICAgaW5jaWRlbnQ6IHRoaXMuaW5jaWRlbnRJZCxcblxuICAgICAgICAgICAgICAgICAgICBoYW5ndXA6ICgpID0+IHtcbiAgICAgICAgICAgICAgICAgICAgICAgIGNvbm4uZGlzY29ubmVjdCgpO1xuICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy51cGRhdGVBY3Rpdml0eShcIlJlYWR5XCIpO1xuICAgICAgICAgICAgICAgICAgICB9LFxuXG4gICAgICAgICAgICAgICAgICAgIG11dGU6ICggZmxhZyApID0+IHtcbiAgICAgICAgICAgICAgICAgICAgICAgIGNvbm4ubXV0ZSggZmxhZyApO1xuICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgfTtcbiAgICAgICAgICAgICAgICB0aGlzLmNhbGxTaWQgPSBjb25uLnBhcmFtZXRlcnMuQ2FsbFNpZDtcbiAgICAgICAgICAgICAgICB0aGlzLnNlcnZlckV2ZW50SGFuZGxlci5kZXNwYXRjaCggXCJjb25uZWN0ZWRcIiwgZGF0YSApO1xuICAgICAgICAgICAgfSk7XG5cbiAgICAgICAgICAgIFR3aWxpby5EZXZpY2UuZGlzY29ubmVjdCggKGNvbm46IGFueSkgPT4ge1xuICAgICAgICAgICAgICAgIEN0aUxvZ2dlci5sb2dJbmZvTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgK1xuICAgICAgICAgICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX0NPTk5FQ1RJT05fQlJPS0UpO1xuICAgICAgICAgICAgICAgIHRoaXMuYXR0cmlidXRlcyA9bnVsbDtcbiAgICAgICAgICAgICAgICB0aGlzLmluY2lkZW50SWQgPSBudWxsO1xuICAgICAgICAgICAgICAgIHRoaXMudXBkYXRlQWN0aXZpdHkoXCJSZWFkeVwiKTtcbiAgICAgICAgICAgICAgICB0aGlzLnNlcnZlckV2ZW50SGFuZGxlci5kZXNwYXRjaCggXCJkaXNjb25uZWN0ZWRcIiwge30gKTtcbiAgICAgICAgICAgIH0pO1xuICAgICAgICB9XG5cbiAgICAgICAgaWYodGhpcy53b3JrZXJUb2tlbiA9PSBudWxsKXtcbiAgICAgICAgICAgIHRocm93KFwiV29ya2VyIHRva2VuIGlzIG51bGwuIENhbm5vdCBwcm92aWRlIGFjY2VzcyB0byBUd2lsaW8gZm9yIHRoaXMgdXNlci5cIik7XG4gICAgICAgIH1cblxuICAgICAgICB0aGlzLndvcmtlciA9IG5ldyBUd2lsaW8uVGFza1JvdXRlci5Xb3JrZXIoIHRoaXMud29ya2VyVG9rZW4gKTtcblxuICAgICAgICB0aGlzLndvcmtlci5vbihcInJlYWR5XCIsICAoIHdvcmtlcjogYW55ICkgPT4ge1xuICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArIFwiV29ya2VyIFwiICsgd29ya2VyLmZyaWVuZGx5TmFtZSArIFwiIGlzIHJlYWR5XCIpO1xuICAgICAgICAgICAgdGhpcy5sb2dnZWRJbiA9IHRydWU7XG4gICAgICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICtcbiAgICAgICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX0RJU1BBVENIX0xPR0lOX1NVQ0NFU1MpO1xuICAgICAgICAgICAgdGhpcy5zZXJ2ZXJFdmVudEhhbmRsZXIuZGVzcGF0Y2goICdsb2dpbi5zdWNjZXNzJywgbmFtZSApO1xuICAgICAgICAgICAgdGhpcy51cGRhdGVBY3Rpdml0eShcIlJlYWR5XCIpO1xuICAgICAgICB9KTtcblxuICAgICAgICB0aGlzLndvcmtlci5vbihcInJlc2VydmF0aW9uLmNyZWF0ZWRcIiwgKHJlc2VydmF0aW9uOiBhbnkpID0+IHtcbiAgICAgICAgICAgIEN0aUxvZ2dlci5sb2dJbmZvTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgK1xuICAgICAgICAgICAgICAgIEN0aU1lc3NhZ2VzLk1FU1NBR0VfUkVTRVJWQVRJT05fQ1JFQVRFRCk7XG4gICAgICAgICAgICB0aGlzLnJlc2VydmF0aW9uID0gcmVzZXJ2YXRpb247XG4gICAgICAgICAgICB2YXIgcGhvbmUgPSByZXNlcnZhdGlvbi50YXNrLmF0dHJpYnV0ZXMuZnJvbTtcbiAgICAgICAgICAgIHRoaXMuYXR0cmlidXRlcyA9IHJlc2VydmF0aW9uLnRhc2suYXR0cmlidXRlcztcbiAgICAgICAgICAgIHBob25lID0gcGhvbmUucmVwbGFjZShcImNsaWVudDpcIixcIlwiKTtcbiAgICAgICAgICAgIHZhciB0aW1lb3V0ID0gMzA7XG5cbiAgICAgICAgICAgIHZhciBjb250YWN0OiBhbnkgPSByZXNlcnZhdGlvbi50YXNrLmF0dHJpYnV0ZXMuY29udGFjdDtcbiAgICAgICAgICAgIHRoaXMuaW5jaWRlbnRJZCA9IHJlc2VydmF0aW9uLnRhc2suYXR0cmlidXRlcy5pbmNpZGVudDtcblxuICAgICAgICAgICAgY29udGFjdFsnbmFtZSddID0gY29udGFjdC5maXJzdE5hbWUgKyBcIiBcIiArIGNvbnRhY3QubGFzdE5hbWU7XG4gICAgICAgICAgICB0aGlzLmNvbnRhY3RJbmZvID0gY29udGFjdDtcblxuICAgICAgICAgICAgdmFyIGRhdGEgPSB7XG4gICAgICAgICAgICAgICAgdGltZW91dDogdGltZW91dCxcblxuICAgICAgICAgICAgICAgIGFjY2VwdDogKCkgPT4ge1xuICAgICAgICAgICAgICAgICAgICBjbGVhclRpbWVvdXQoIHRoaXMudGltZW91dEhhbmRsZSApO1xuICAgICAgICAgICAgICAgICAgICByZXNlcnZhdGlvbi5kZXF1ZXVlKCB0aGlzLmRlcXVldWVDb25maWdJZCApO1xuICAgICAgICAgICAgICAgIH0sXG4gICAgICAgICAgICAgICAgcmVqZWN0OiAoKSA9PiB7XG4gICAgICAgICAgICAgICAgICAgIGNsZWFyVGltZW91dCggdGhpcy50aW1lb3V0SGFuZGxlICk7XG4gICAgICAgICAgICAgICAgICAgIHRoaXMuYXR0cmlidXRlcyA9IG51bGw7XG4gICAgICAgICAgICAgICAgICAgIHRoaXMuaW5jaWRlbnRJZCA9IG51bGw7XG4gICAgICAgICAgICAgICAgICAgIHJlc2VydmF0aW9uLnJlamVjdCgpO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgIH07XG5cbiAgICAgICAgICAgIGRhdGFbXCJjb250YWN0XCJdID0gY29udGFjdDtcbiAgICAgICAgICAgIHRoaXMudGltZW91dEhhbmRsZSA9IHNldFRpbWVvdXQoICgpID0+IHtcbiAgICAgICAgICAgICAgICB0aGlzLnNlcnZlckV2ZW50SGFuZGxlci5kZXNwYXRjaCggXCJ0aW1lb3V0XCIsIHt9ICk7XG4gICAgICAgICAgICAgICAgaWYodGhpcy5yZXNlcnZhdGlvbikge1xuICAgICAgICAgICAgICAgICAgICB0aGlzLnJlc2VydmF0aW9uLnJlamVjdCgpO1xuICAgICAgICAgICAgICAgICAgICB0aGlzLnJlc2VydmF0aW9uID0gbnVsbDtcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgdGhpcy5hdHRyaWJ1dGVzID0gbnVsbDtcbiAgICAgICAgICAgICAgICB0aGlzLmluY2lkZW50SWQgPSBudWxsO1xuICAgICAgICAgICAgfSwgdGltZW91dCAqIDEwMDAgKTtcbiAgICAgICAgICAgIHRoaXMuc2VydmVyRXZlbnRIYW5kbGVyLmRlc3BhdGNoKCBcImluY29taW5nXCIsIGRhdGEgKTtcbiAgICAgICAgfSk7XG5cbiAgICAgICAgdGhpcy53b3JrZXIub24oXCJyZXNlcnZhdGlvbi5jYW5jZWxlZFwiLCAocmVzZXJ2YXRpb24pID0+IHtcbiAgICAgICAgICAgIEN0aUxvZ2dlci5sb2dJbmZvTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgK1xuICAgICAgICAgICAgICAgIEN0aU1lc3NhZ2VzLk1FU1NBR0VfUkVTRVJWQVRJT05fQ0FOQ0VMTEVEKTtcbiAgICAgICAgICAgIHRoaXMuYXR0cmlidXRlcyA9IG51bGw7XG4gICAgICAgICAgICB0aGlzLmluY2lkZW50SWQgPSBudWxsO1xuICAgICAgICAgICAgY2xlYXJUaW1lb3V0KCB0aGlzLnRpbWVvdXRIYW5kbGUgKTtcbiAgICAgICAgICAgIHRoaXMuc2VydmVyRXZlbnRIYW5kbGVyLmRlc3BhdGNoKCBcImNhbmNlbGVkXCIsIHt9ICk7XG4gICAgICAgIH0pO1xuXG4gICAgICAgIHRoaXMud29ya2VyLm9uKFwicmVzZXJ2YXRpb24uYWNjZXB0ZWRcIiwgKHJlc2VydmF0aW9uKSA9PiB7XG4gICAgICAgICAgICBjbGVhclRpbWVvdXQoIHRoaXMudGltZW91dEhhbmRsZSApO1xuICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0luZm9NZXNzYWdlKHRoaXMubG9nUHJlTWVzc2FnZSArXG4gICAgICAgICAgICAgICAgQ3RpTWVzc2FnZXMuTUVTU0FHRV9SRVNFUlZBVElPTl9BQ0NFUFRFRCk7XG4gICAgICAgIH0pO1xuXG4gICAgICAgIHRoaXMud29ya2VyLm9uKFwicmVzZXJ2YXRpb24ucmVqZWN0ZWRcIiwgKHJlc2VydmF0aW9uKSA9PiB7XG4gICAgICAgICAgICBjbGVhclRpbWVvdXQoIHRoaXMudGltZW91dEhhbmRsZSApO1xuICAgICAgICAgICAgdGhpcy5hdHRyaWJ1dGVzID0gbnVsbDtcbiAgICAgICAgICAgIHRoaXMuaW5jaWRlbnRJZCA9IG51bGw7XG4gICAgICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICtcbiAgICAgICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX1JFU0VSVkFUSU9OX1JFSkVDVEVEKTtcbiAgICAgICAgICAgIHRoaXMuc2VydmVyRXZlbnRIYW5kbGVyLmRlc3BhdGNoKCBcImNhbmNlbGVkXCIsIHt9ICk7XG4gICAgICAgIH0pO1xuXG4gICAgICAgIHRoaXMud29ya2VyLm9uKFwidG9rZW4uZXhwaXJlZFwiLCAoKSA9PiB7XG4gICAgICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICtcbiAgICAgICAgICAgICAgICBDdGlNZXNzYWdlcy5NRVNTQUdFX1RPS0VOX0VYUElSRUQpO1xuICAgICAgICAgICAgdGhpcy5zZXJ2ZXJFdmVudEhhbmRsZXIuZGVzcGF0Y2goJ3Rva2VuLmV4cGlyZWQnLCB7fSk7XG4gICAgICAgIH0pO1xuXG4gICAgICAgIHRoaXMud29ya2VyLm9uKFwiYWN0aXZpdHkudXBkYXRlXCIsICh3b3JrZXIpID0+IHtcbiAgICAgICAgICAgIHRoaXMuc2VydmVyRXZlbnRIYW5kbGVyLmRlc3BhdGNoKCAnYWN0aXZpdHkudXBkYXRlJywgd29ya2VyLmFjdGl2aXR5TmFtZSApO1xuICAgICAgICB9KTtcblxuICAgICAgICB0aGlzLndvcmtlci5hY3Rpdml0aWVzLmZldGNoKFxuICAgICAgICAgICAgKGVycm9yOiBhbnksIGFjdGl2aXR5TGlzdDogYW55KSA9PiB7XG4gICAgICAgICAgICAgICAgaWYoZXJyb3IpIHtcbiAgICAgICAgICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0Vycm9yTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgKyBlcnJvci5jb2RlKTtcbiAgICAgICAgICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0Vycm9yTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgKyBlcnJvci5tZXNzYWdlKTtcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICB2YXIgZGF0YSA9IGFjdGl2aXR5TGlzdC5kYXRhO1xuICAgICAgICAgICAgICAgIGZvcih2YXIgaT0wOyBpPGRhdGEubGVuZ3RoOyBpKyspIHtcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5hY3Rpdml0aWVzW2RhdGFbaV0uZnJpZW5kbHlOYW1lXSA9IGRhdGFbaV0uc2lkO1xuICAgICAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgICAgIHRoaXMud29ya2VyLnVwZGF0ZShcIkFjdGl2aXR5U2lkXCIsIHRoaXMuYWN0aXZpdGllc1tcIlJlYWR5XCJdLCAoZXJyb3IsIHdvcmtlcikgPT4ge1xuICAgICAgICAgICAgICAgICAgICBpZihlcnJvcikge1xuICAgICAgICAgICAgICAgICAgICAgICAgQ3RpTG9nZ2VyLmxvZ0Vycm9yTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgKyBlcnJvci5jb2RlKTtcbiAgICAgICAgICAgICAgICAgICAgICAgIEN0aUxvZ2dlci5sb2dFcnJvck1lc3NhZ2UodGhpcy5sb2dQcmVNZXNzYWdlICsgZXJyb3IubWVzc2FnZSk7XG4gICAgICAgICAgICAgICAgICAgIH0gZWxzZSB7XG4gICAgICAgICAgICAgICAgICAgICAgICBDdGlMb2dnZXIubG9nSW5mb01lc3NhZ2Uod29ya2VyLmFjdGl2aXR5TmFtZSk7XG4gICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICB9KTtcbiAgICAgICAgICAgIH1cbiAgICAgICAgKTtcbiAgICB9O1xuXG59Il19