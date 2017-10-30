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
 *  SHA1: $Id: 3736c865a28707b8427d97a183f3b231a615ac9d $
 * *********************************************************************************************
 *  File: serverEventHandler.js
 * ****************************************************************************************** */
define(["require", "exports", "jquery", "../util/ctiLogger", "../util/ctiMessages"], function (require, exports, $, ctiLogger_1, ctiMessages_1) {
    "use strict";
    exports.__esModule = true;
    /**
     * ServerEventHandler - This class defines and handles the events used for communication
     * between addin and the adapter. This acts as pub-sub system for the CTI addin.
     *
     * It stores handlers for various events and invokes them when an event occurs.
     *
     * We can add required events in the store and register handlers. later when we despatch
     * an event, the corresponding handler will be invoked
     */
    var ServerEventHandler = /** @class */ (function () {
        function ServerEventHandler() {
            this.store = {
                // Internal Events
                'reservation.created': [],
                'reservation.timeout': [],
                'token.expired': [],
                // Application level events
                'cti.enabled': [],
                'cti.disabled': [],
                'activity.update': [],
                'login.success': [],
                'login.failed': [],
                'incoming': [],
                'connected': [],
                'disconnected': [],
                'timeout': [],
                'canceled': [],
                'search.contact.complete': [],
                'search.contact.failed': [],
                'search.agentlist.complete': []
            };
            this.logPreMessage = 'ServerEventHandler' + ctiMessages_1.CtiMessages.MESSAGE_APPENDER;
        }
        /**
         * Binds handler to an event
         *
         * @param handle
         * @param event
         * @returns {this}
         */
        ServerEventHandler.prototype.on = function (handle, event) {
            if (typeof event !== 'function') {
                if ((typeof event === 'object') && (typeof event.func !== 'function')) {
                    return;
                }
            }
            if (!(handle in this.store)) {
                return;
            }
            this.store[handle].push(event);
            return this;
        };
        /**
         * Removes handler for an event
         *
         * @param handle
         * @returns {this}
         */
        ServerEventHandler.prototype.off = function (handle) {
            if (!(handle in this.store)) {
                return;
            }
            this.store[handle] = [];
            return this;
        };
        /**
         * Invoke an event handler along with associated data
         *
         * @param handle
         * @param data
         * @returns {this}
         */
        ServerEventHandler.prototype.despatch = function (handle, data) {
            var _this = this;
            if (!(handle in this.store)) {
                return;
            }
            ctiLogger_1.CtiLogger.logInfoMessage(this.logPreMessage + ctiMessages_1.CtiMessages.MESSAGE_EVENT_DISPATCH
                + ctiMessages_1.CtiMessages.MESSAGE_APPENDER + handle + ctiMessages_1.CtiMessages.MESSAGE_WITH_DATA + ctiMessages_1.CtiMessages.MESSAGE_APPENDER + JSON.stringify(data));
            $.each(this.store[handle], function (idx) {
                var func = _this.store[handle][idx];
                if (typeof func === 'object') {
                    func.count--;
                    func.func(data);
                    if (func.count <= 0) {
                        _this.store[handle].splice(idx, 1);
                    }
                    return _this;
                }
                func(data);
            });
            return this;
        };
        return ServerEventHandler;
    }());
    exports.ServerEventHandler = ServerEventHandler;
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoic2VydmVyRXZlbnRIYW5kbGVyLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsic2VydmVyRXZlbnRIYW5kbGVyLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBOzs7OztnR0FLZ0c7Ozs7SUFLaEc7Ozs7Ozs7O09BUUc7SUFDSDtRQUFBO1lBRVUsVUFBSyxHQUFPO2dCQUNsQixrQkFBa0I7Z0JBQ2xCLHFCQUFxQixFQUFFLEVBQUU7Z0JBQ3pCLHFCQUFxQixFQUFFLEVBQUU7Z0JBQ3pCLGVBQWUsRUFBRSxFQUFFO2dCQUVuQiwyQkFBMkI7Z0JBQzNCLGFBQWEsRUFBRSxFQUFFO2dCQUNqQixjQUFjLEVBQUUsRUFBRTtnQkFDbEIsaUJBQWlCLEVBQUUsRUFBRTtnQkFDckIsZUFBZSxFQUFFLEVBQUU7Z0JBQ25CLGNBQWMsRUFBRSxFQUFFO2dCQUNsQixVQUFVLEVBQUUsRUFBRTtnQkFDZCxXQUFXLEVBQUUsRUFBRTtnQkFDZixjQUFjLEVBQUUsRUFBRTtnQkFDbEIsU0FBUyxFQUFFLEVBQUU7Z0JBQ2IsVUFBVSxFQUFFLEVBQUU7Z0JBQ2QseUJBQXlCLEVBQUUsRUFBRTtnQkFDN0IsdUJBQXVCLEVBQUUsRUFBRTtnQkFDM0IsMkJBQTJCLEVBQUUsRUFBRTthQUNoQyxDQUFDO1lBRU0sa0JBQWEsR0FBVyxvQkFBb0IsR0FBRyx5QkFBVyxDQUFDLGdCQUFnQixDQUFDO1FBbUV0RixDQUFDO1FBakVDOzs7Ozs7V0FNRztRQUNJLCtCQUFFLEdBQVQsVUFBVSxNQUFXLEVBQUUsS0FBVTtZQUMvQixFQUFFLENBQUMsQ0FBQyxPQUFPLEtBQUssS0FBSyxVQUFVLENBQUMsQ0FBQyxDQUFDO2dCQUNoQyxFQUFFLENBQUMsQ0FBQyxDQUFFLE9BQU8sS0FBSyxLQUFLLFFBQVEsQ0FBRSxJQUFJLENBQUUsT0FBTyxLQUFLLENBQUMsSUFBSSxLQUFLLFVBQVUsQ0FBRSxDQUFDLENBQUMsQ0FBQztvQkFDMUUsTUFBTSxDQUFDO2dCQUNULENBQUM7WUFDSCxDQUFDO1lBRUQsRUFBRSxDQUFDLENBQUMsQ0FBQyxDQUFFLE1BQU0sSUFBSSxJQUFJLENBQUMsS0FBSyxDQUFFLENBQUMsQ0FBQyxDQUFDO2dCQUM5QixNQUFNLENBQUM7WUFDVCxDQUFDO1lBRUQsSUFBSSxDQUFDLEtBQUssQ0FBQyxNQUFNLENBQUMsQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLENBQUM7WUFDL0IsTUFBTSxDQUFDLElBQUksQ0FBQztRQUNkLENBQUM7UUFFRDs7Ozs7V0FLRztRQUNJLGdDQUFHLEdBQVYsVUFBVyxNQUFXO1lBQ3BCLEVBQUUsQ0FBQyxDQUFDLENBQUMsQ0FBRSxNQUFNLElBQUksSUFBSSxDQUFDLEtBQUssQ0FBRSxDQUFDLENBQUMsQ0FBQztnQkFDOUIsTUFBTSxDQUFDO1lBQ1QsQ0FBQztZQUVELElBQUksQ0FBQyxLQUFLLENBQUMsTUFBTSxDQUFDLEdBQUcsRUFBRSxDQUFDO1lBQ3hCLE1BQU0sQ0FBQyxJQUFJLENBQUM7UUFDZCxDQUFDO1FBRUQ7Ozs7OztXQU1HO1FBQ0kscUNBQVEsR0FBZixVQUFnQixNQUFXLEVBQUUsSUFBUztZQUF0QyxpQkFtQkM7WUFsQkMsRUFBRSxDQUFDLENBQUMsQ0FBQyxDQUFFLE1BQU0sSUFBSSxJQUFJLENBQUMsS0FBSyxDQUFFLENBQUMsQ0FBQyxDQUFDO2dCQUM5QixNQUFNLENBQUM7WUFDVCxDQUFDO1lBQ0QscUJBQVMsQ0FBQyxjQUFjLENBQUMsSUFBSSxDQUFDLGFBQWEsR0FBRyx5QkFBVyxDQUFDLHNCQUFzQjtrQkFDNUUseUJBQVcsQ0FBQyxnQkFBZ0IsR0FBRyxNQUFNLEdBQUcseUJBQVcsQ0FBQyxpQkFBaUIsR0FBRyx5QkFBVyxDQUFDLGdCQUFnQixHQUFHLElBQUksQ0FBQyxTQUFTLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQztZQUNqSSxDQUFDLENBQUMsSUFBSSxDQUFDLElBQUksQ0FBQyxLQUFLLENBQUMsTUFBTSxDQUFDLEVBQUUsVUFBQyxHQUFHO2dCQUM3QixJQUFJLElBQUksR0FBRyxLQUFJLENBQUMsS0FBSyxDQUFDLE1BQU0sQ0FBQyxDQUFDLEdBQUcsQ0FBQyxDQUFDO2dCQUNuQyxFQUFFLENBQUMsQ0FBQyxPQUFPLElBQUksS0FBSyxRQUFRLENBQUMsQ0FBQyxDQUFDO29CQUM3QixJQUFJLENBQUMsS0FBSyxFQUFFLENBQUM7b0JBQ2IsSUFBSSxDQUFDLElBQUksQ0FBQyxJQUFJLENBQUMsQ0FBQztvQkFDaEIsRUFBRSxDQUFDLENBQUMsSUFBSSxDQUFDLEtBQUssSUFBSSxDQUFDLENBQUMsQ0FBQyxDQUFDO3dCQUNwQixLQUFJLENBQUMsS0FBSyxDQUFDLE1BQU0sQ0FBQyxDQUFDLE1BQU0sQ0FBQyxHQUFHLEVBQUUsQ0FBQyxDQUFDLENBQUM7b0JBQ3BDLENBQUM7b0JBQ0QsTUFBTSxDQUFDLEtBQUksQ0FBQztnQkFDZCxDQUFDO2dCQUNELElBQUksQ0FBQyxJQUFJLENBQUMsQ0FBQztZQUNiLENBQUMsQ0FBQyxDQUFDO1lBQ0gsTUFBTSxDQUFDLElBQUksQ0FBQztRQUNkLENBQUM7UUFFSCx5QkFBQztJQUFELENBQUMsQUEzRkQsSUEyRkM7SUEzRlksZ0RBQWtCIiwic291cmNlc0NvbnRlbnQiOlsiLyogKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXG4gKiAgJEFDQ0VMRVJBVE9SX0hFQURFUl9QTEFDRV9IT0xERVIkXG4gKiAgU0hBMTogJElkOiAzNzM2Yzg2NWEyODcwN2I4NDI3ZDk3YTE4M2YzYjIzMWE2MTVhYzlkICRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogIEZpbGU6ICRBQ0NFTEVSQVRPUl9IRUFERVJfRklMRV9OQU1FX1BMQUNFX0hPTERFUiRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiAqL1xuXG5pbXBvcnQgJCA9IHJlcXVpcmUoJ2pxdWVyeScpO1xuaW1wb3J0IHtDdGlMb2dnZXJ9IGZyb20gXCIuLi91dGlsL2N0aUxvZ2dlclwiO1xuaW1wb3J0IHtDdGlNZXNzYWdlc30gZnJvbSBcIi4uL3V0aWwvY3RpTWVzc2FnZXNcIjtcbi8qKlxuICogU2VydmVyRXZlbnRIYW5kbGVyIC0gVGhpcyBjbGFzcyBkZWZpbmVzIGFuZCBoYW5kbGVzIHRoZSBldmVudHMgdXNlZCBmb3IgY29tbXVuaWNhdGlvblxuICogYmV0d2VlbiBhZGRpbiBhbmQgdGhlIGFkYXB0ZXIuIFRoaXMgYWN0cyBhcyBwdWItc3ViIHN5c3RlbSBmb3IgdGhlIENUSSBhZGRpbi5cbiAqXG4gKiBJdCBzdG9yZXMgaGFuZGxlcnMgZm9yIHZhcmlvdXMgZXZlbnRzIGFuZCBpbnZva2VzIHRoZW0gd2hlbiBhbiBldmVudCBvY2N1cnMuXG4gKlxuICogV2UgY2FuIGFkZCByZXF1aXJlZCBldmVudHMgaW4gdGhlIHN0b3JlIGFuZCByZWdpc3RlciBoYW5kbGVycy4gbGF0ZXIgd2hlbiB3ZSBkZXNwYXRjaFxuICogYW4gZXZlbnQsIHRoZSBjb3JyZXNwb25kaW5nIGhhbmRsZXIgd2lsbCBiZSBpbnZva2VkXG4gKi9cbmV4cG9ydCBjbGFzcyBTZXJ2ZXJFdmVudEhhbmRsZXIge1xuXG4gIHByaXZhdGUgc3RvcmU6YW55ID0ge1xuICAgIC8vIEludGVybmFsIEV2ZW50c1xuICAgICdyZXNlcnZhdGlvbi5jcmVhdGVkJzogW10sXG4gICAgJ3Jlc2VydmF0aW9uLnRpbWVvdXQnOiBbXSxcbiAgICAndG9rZW4uZXhwaXJlZCc6IFtdLFxuXG4gICAgLy8gQXBwbGljYXRpb24gbGV2ZWwgZXZlbnRzXG4gICAgJ2N0aS5lbmFibGVkJzogW10sXG4gICAgJ2N0aS5kaXNhYmxlZCc6IFtdLFxuICAgICdhY3Rpdml0eS51cGRhdGUnOiBbXSxcbiAgICAnbG9naW4uc3VjY2Vzcyc6IFtdLFxuICAgICdsb2dpbi5mYWlsZWQnOiBbXSxcbiAgICAnaW5jb21pbmcnOiBbXSxcbiAgICAnY29ubmVjdGVkJzogW10sXG4gICAgJ2Rpc2Nvbm5lY3RlZCc6IFtdLFxuICAgICd0aW1lb3V0JzogW10sXG4gICAgJ2NhbmNlbGVkJzogW10sXG4gICAgJ3NlYXJjaC5jb250YWN0LmNvbXBsZXRlJzogW10sXG4gICAgJ3NlYXJjaC5jb250YWN0LmZhaWxlZCc6IFtdLFxuICAgICdzZWFyY2guYWdlbnRsaXN0LmNvbXBsZXRlJzogW11cbiAgfTtcblxuICBwcml2YXRlIGxvZ1ByZU1lc3NhZ2U6IHN0cmluZyA9ICdTZXJ2ZXJFdmVudEhhbmRsZXInICsgQ3RpTWVzc2FnZXMuTUVTU0FHRV9BUFBFTkRFUjtcblxuICAvKipcbiAgICogQmluZHMgaGFuZGxlciB0byBhbiBldmVudFxuICAgKlxuICAgKiBAcGFyYW0gaGFuZGxlXG4gICAqIEBwYXJhbSBldmVudFxuICAgKiBAcmV0dXJucyB7dGhpc31cbiAgICovXG4gIHB1YmxpYyBvbihoYW5kbGU6IGFueSwgZXZlbnQ6IGFueSk6U2VydmVyRXZlbnRIYW5kbGVyIHtcbiAgICBpZiAodHlwZW9mIGV2ZW50ICE9PSAnZnVuY3Rpb24nKSB7XG4gICAgICBpZiAoKCB0eXBlb2YgZXZlbnQgPT09ICdvYmplY3QnICkgJiYgKCB0eXBlb2YgZXZlbnQuZnVuYyAhPT0gJ2Z1bmN0aW9uJyApKSB7XG4gICAgICAgIHJldHVybjtcbiAgICAgIH1cbiAgICB9XG5cbiAgICBpZiAoISggaGFuZGxlIGluIHRoaXMuc3RvcmUgKSkge1xuICAgICAgcmV0dXJuO1xuICAgIH1cblxuICAgIHRoaXMuc3RvcmVbaGFuZGxlXS5wdXNoKGV2ZW50KTtcbiAgICByZXR1cm4gdGhpcztcbiAgfVxuXG4gIC8qKlxuICAgKiBSZW1vdmVzIGhhbmRsZXIgZm9yIGFuIGV2ZW50XG4gICAqXG4gICAqIEBwYXJhbSBoYW5kbGVcbiAgICogQHJldHVybnMge3RoaXN9XG4gICAqL1xuICBwdWJsaWMgb2ZmKGhhbmRsZTogYW55KTpTZXJ2ZXJFdmVudEhhbmRsZXIge1xuICAgIGlmICghKCBoYW5kbGUgaW4gdGhpcy5zdG9yZSApKSB7XG4gICAgICByZXR1cm47XG4gICAgfVxuXG4gICAgdGhpcy5zdG9yZVtoYW5kbGVdID0gW107XG4gICAgcmV0dXJuIHRoaXM7XG4gIH1cblxuICAvKipcbiAgICogSW52b2tlIGFuIGV2ZW50IGhhbmRsZXIgYWxvbmcgd2l0aCBhc3NvY2lhdGVkIGRhdGFcbiAgICpcbiAgICogQHBhcmFtIGhhbmRsZVxuICAgKiBAcGFyYW0gZGF0YVxuICAgKiBAcmV0dXJucyB7dGhpc31cbiAgICovXG4gIHB1YmxpYyBkZXNwYXRjaChoYW5kbGU6IGFueSwgZGF0YTogYW55KTpTZXJ2ZXJFdmVudEhhbmRsZXIge1xuICAgIGlmICghKCBoYW5kbGUgaW4gdGhpcy5zdG9yZSApKSB7XG4gICAgICByZXR1cm47XG4gICAgfVxuICAgIEN0aUxvZ2dlci5sb2dJbmZvTWVzc2FnZSh0aGlzLmxvZ1ByZU1lc3NhZ2UgKyBDdGlNZXNzYWdlcy5NRVNTQUdFX0VWRU5UX0RJU1BBVENIXG4gICAgICArIEN0aU1lc3NhZ2VzLk1FU1NBR0VfQVBQRU5ERVIgKyBoYW5kbGUgKyBDdGlNZXNzYWdlcy5NRVNTQUdFX1dJVEhfREFUQSArIEN0aU1lc3NhZ2VzLk1FU1NBR0VfQVBQRU5ERVIgKyBKU09OLnN0cmluZ2lmeShkYXRhKSk7XG4gICAgJC5lYWNoKHRoaXMuc3RvcmVbaGFuZGxlXSwgKGlkeCk9PiB7XG4gICAgICB2YXIgZnVuYyA9IHRoaXMuc3RvcmVbaGFuZGxlXVtpZHhdO1xuICAgICAgaWYgKHR5cGVvZiBmdW5jID09PSAnb2JqZWN0Jykge1xuICAgICAgICBmdW5jLmNvdW50LS07XG4gICAgICAgIGZ1bmMuZnVuYyhkYXRhKTtcbiAgICAgICAgaWYgKGZ1bmMuY291bnQgPD0gMCkge1xuICAgICAgICAgIHRoaXMuc3RvcmVbaGFuZGxlXS5zcGxpY2UoaWR4LCAxKTtcbiAgICAgICAgfVxuICAgICAgICByZXR1cm4gdGhpcztcbiAgICAgIH1cbiAgICAgIGZ1bmMoZGF0YSk7XG4gICAgfSk7XG4gICAgcmV0dXJuIHRoaXM7XG4gIH1cblxufVxuIl19