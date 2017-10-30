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
 *  SHA1: $Id: 7cb46288d1f68e66049dae8722290385e3c29019 $
 * *********************************************************************************************
 *  File: twilioMessagingAdapter.js
 * ****************************************************************************************** */
define(["require", "exports"], function (require, exports) {
    "use strict";
    exports.__esModule = true;
    /**
     * Implements the functionality defined by ICtiMessagingAdapter
     *
     */
    var TwilioMessagingAdapter = /** @class */ (function () {
        function TwilioMessagingAdapter() {
            this.messageConfiguration = {
                providerName: 'Twilio',
                providerPath: 'cc/CTI'
            };
        }
        /**
         * This method submits a request to send message and
         * returns the jquery promise
         *
         * @param message
         * @param profileData
         * @returns {any}
         */
        TwilioMessagingAdapter.prototype.sendMessage = function (message, profileData) {
            var messageUrl = profileData.interfaceUrl.match(/^[^\/]+:\/\/[^\/]+\//)[0].replace(/^http(?!s)/i, 'https')
                + this.messageConfiguration.providerPath + '/sendSMS';
            var incidentId = null;
            if (message.incidentWorkspace && message.incidentWorkspace.getWorkspaceRecordId() > 0) {
                incidentId = message.incidentWorkspace.getWorkspaceRecordId();
            }
            return $.ajax({
                url: messageUrl,
                type: "POST",
                data: {
                    session_id: profileData.sessionId,
                    message: message.message,
                    number: message.contact.phone,
                    incident: incidentId
                },
                dataType: "JSON"
            });
        };
        /**
         * this function search for a contact
         *
         * @param cId
         * @param sessionId
         * @param serverUri
         * @param phone
         */
        TwilioMessagingAdapter.prototype.searchContact = function (phone, cId, sessionId, serverUri) {
            var searchUrl = serverUri.match(/^[^\/]+:\/\/[^\/]+\//)[0].replace(/^http(?!s)/i, 'https')
                + this.messageConfiguration.providerPath + '/searchPhone';
            return $.ajax({
                url: searchUrl,
                type: 'POST',
                data: {
                    phone: phone,
                    session_id: sessionId,
                    id: cId
                }
            });
        };
        return TwilioMessagingAdapter;
    }());
    exports.TwilioMessagingAdapter = TwilioMessagingAdapter;
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoidHdpbGlvTWVzc2FnaW5nQWRhcHRlci5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbInR3aWxpb01lc3NhZ2luZ0FkYXB0ZXIudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7Ozs7O2dHQUtnRzs7OztJQU9oRzs7O09BR0c7SUFDSDtRQUlJO1lBQ0ksSUFBSSxDQUFDLG9CQUFvQixHQUE2QjtnQkFDbEQsWUFBWSxFQUFFLFFBQVE7Z0JBQ3RCLFlBQVksRUFBRSxRQUFRO2FBQ3pCLENBQUM7UUFDTixDQUFDO1FBRUQ7Ozs7Ozs7V0FPRztRQUNJLDRDQUFXLEdBQWxCLFVBQW1CLE9BQWdCLEVBQUUsV0FBeUI7WUFFMUQsSUFBSSxVQUFVLEdBQUcsV0FBVyxDQUFDLFlBQVksQ0FBQyxLQUFLLENBQUMsc0JBQXNCLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxPQUFPLENBQUMsYUFBYSxFQUFFLE9BQU8sQ0FBQztrQkFDcEcsSUFBSSxDQUFDLG9CQUFvQixDQUFDLFlBQVksR0FBQyxVQUFVLENBQUM7WUFDeEQsSUFBSSxVQUFVLEdBQUcsSUFBSSxDQUFDO1lBQ3RCLEVBQUUsQ0FBQSxDQUFDLE9BQU8sQ0FBQyxpQkFBaUIsSUFBSSxPQUFPLENBQUMsaUJBQWlCLENBQUMsb0JBQW9CLEVBQUUsR0FBRyxDQUFDLENBQUMsQ0FBQSxDQUFDO2dCQUNsRixVQUFVLEdBQUcsT0FBTyxDQUFDLGlCQUFpQixDQUFDLG9CQUFvQixFQUFFLENBQUM7WUFDbEUsQ0FBQztZQUVGLE1BQU0sQ0FBQyxDQUFDLENBQUMsSUFBSSxDQUFDO2dCQUNULEdBQUcsRUFBRSxVQUFVO2dCQUNmLElBQUksRUFBRSxNQUFNO2dCQUNaLElBQUksRUFBRTtvQkFDRixVQUFVLEVBQUUsV0FBVyxDQUFDLFNBQVM7b0JBQ2pDLE9BQU8sRUFBRSxPQUFPLENBQUMsT0FBTztvQkFDeEIsTUFBTSxFQUFFLE9BQU8sQ0FBQyxPQUFPLENBQUMsS0FBSztvQkFDN0IsUUFBUSxFQUFFLFVBQVU7aUJBQ3ZCO2dCQUNELFFBQVEsRUFBRSxNQUFNO2FBQ25CLENBQUMsQ0FBQztRQUNQLENBQUM7UUFFRDs7Ozs7OztXQU9HO1FBQ0ksOENBQWEsR0FBcEIsVUFBcUIsS0FBYSxFQUFFLEdBQVcsRUFBRSxTQUFpQixFQUFFLFNBQWlCO1lBQ2pGLElBQUksU0FBUyxHQUFHLFNBQVMsQ0FBQyxLQUFLLENBQUMsc0JBQXNCLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxPQUFPLENBQUMsYUFBYSxFQUFFLE9BQU8sQ0FBQztrQkFDcEYsSUFBSSxDQUFDLG9CQUFvQixDQUFDLFlBQVksR0FBQyxjQUFjLENBQUM7WUFFNUQsTUFBTSxDQUFDLENBQUMsQ0FBQyxJQUFJLENBQUM7Z0JBQ1YsR0FBRyxFQUFFLFNBQVM7Z0JBQ2QsSUFBSSxFQUFFLE1BQU07Z0JBQ1osSUFBSSxFQUFFO29CQUNGLEtBQUssRUFBRSxLQUFLO29CQUNaLFVBQVUsRUFBRSxTQUFTO29CQUNyQixFQUFFLEVBQUUsR0FBRztpQkFDVjthQUNKLENBQUMsQ0FBQztRQUNQLENBQUM7UUFFTCw2QkFBQztJQUFELENBQUMsQUFoRUQsSUFnRUM7SUFoRVksd0RBQXNCIiwic291cmNlc0NvbnRlbnQiOlsiLyogKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXG4gKiAgJEFDQ0VMRVJBVE9SX0hFQURFUl9QTEFDRV9IT0xERVIkXG4gKiAgU0hBMTogJElkOiA3Y2I0NjI4OGQxZjY4ZTY2MDQ5ZGFlODcyMjI5MDM4NWUzYzI5MDE5ICRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogIEZpbGU6ICRBQ0NFTEVSQVRPUl9IRUFERVJfRklMRV9OQU1FX1BMQUNFX0hPTERFUiRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiAqL1xuXG5pbXBvcnQge0lDdGlNZXNzYWdpbmdBZGFwdGVyfSBmcm9tIFwiLi4vY29udHJhY3RzL2lDdGlNZXNzYWdpbmdBZGFwdGVyXCI7XG5pbXBvcnQge01lc3NhZ2V9IGZyb20gXCIuLi9tb2RlbC9tZXNzYWdlXCI7XG5pbXBvcnQge0N0aU1lc3NhZ2VDb25maWd1cmF0aW9ufSBmcm9tIFwiLi4vbW9kZWwvY3RpTWVzc2FnZUNvbmZpZ3VyYXRpb25cIjtcbmltcG9ydCB7QWdlbnRQcm9maWxlfSBmcm9tIFwiLi4vbW9kZWwvYWdlbnRQcm9maWxlXCI7XG5cbi8qKlxuICogSW1wbGVtZW50cyB0aGUgZnVuY3Rpb25hbGl0eSBkZWZpbmVkIGJ5IElDdGlNZXNzYWdpbmdBZGFwdGVyXG4gKiBcbiAqL1xuZXhwb3J0IGNsYXNzIFR3aWxpb01lc3NhZ2luZ0FkYXB0ZXIgaW1wbGVtZW50cyBJQ3RpTWVzc2FnaW5nQWRhcHRlciB7XG5cbiAgICBwcml2YXRlIG1lc3NhZ2VDb25maWd1cmF0aW9uOiBDdGlNZXNzYWdlQ29uZmlndXJhdGlvbjtcblxuICAgIGNvbnN0cnVjdG9yKCkge1xuICAgICAgICB0aGlzLm1lc3NhZ2VDb25maWd1cmF0aW9uID0gPEN0aU1lc3NhZ2VDb25maWd1cmF0aW9uPiB7XG4gICAgICAgICAgICBwcm92aWRlck5hbWU6ICdUd2lsaW8nLFxuICAgICAgICAgICAgcHJvdmlkZXJQYXRoOiAnY2MvQ1RJJ1xuICAgICAgICB9O1xuICAgIH1cblxuICAgIC8qKlxuICAgICAqIFRoaXMgbWV0aG9kIHN1Ym1pdHMgYSByZXF1ZXN0IHRvIHNlbmQgbWVzc2FnZSBhbmRcbiAgICAgKiByZXR1cm5zIHRoZSBqcXVlcnkgcHJvbWlzZVxuICAgICAqXG4gICAgICogQHBhcmFtIG1lc3NhZ2VcbiAgICAgKiBAcGFyYW0gcHJvZmlsZURhdGFcbiAgICAgKiBAcmV0dXJucyB7YW55fVxuICAgICAqL1xuICAgIHB1YmxpYyBzZW5kTWVzc2FnZShtZXNzYWdlOiBNZXNzYWdlLCBwcm9maWxlRGF0YTogQWdlbnRQcm9maWxlKTogYW55IHtcblxuICAgICAgICB2YXIgbWVzc2FnZVVybCA9IHByb2ZpbGVEYXRhLmludGVyZmFjZVVybC5tYXRjaCgvXlteXFwvXSs6XFwvXFwvW15cXC9dK1xcLy8pWzBdLnJlcGxhY2UoL15odHRwKD8hcykvaSwgJ2h0dHBzJylcbiAgICAgICAgICAgICsgdGhpcy5tZXNzYWdlQ29uZmlndXJhdGlvbi5wcm92aWRlclBhdGgrJy9zZW5kU01TJztcbiAgICAgICAgdmFyIGluY2lkZW50SWQgPSBudWxsO1xuICAgICAgICBpZihtZXNzYWdlLmluY2lkZW50V29ya3NwYWNlICYmIG1lc3NhZ2UuaW5jaWRlbnRXb3Jrc3BhY2UuZ2V0V29ya3NwYWNlUmVjb3JkSWQoKSA+IDApe1xuICAgICAgICAgICAgaW5jaWRlbnRJZCA9IG1lc3NhZ2UuaW5jaWRlbnRXb3Jrc3BhY2UuZ2V0V29ya3NwYWNlUmVjb3JkSWQoKTtcbiAgICAgICAgfVxuXG4gICAgICAgcmV0dXJuICQuYWpheCh7XG4gICAgICAgICAgICB1cmw6IG1lc3NhZ2VVcmwsXG4gICAgICAgICAgICB0eXBlOiBcIlBPU1RcIixcbiAgICAgICAgICAgIGRhdGE6IHtcbiAgICAgICAgICAgICAgICBzZXNzaW9uX2lkOiBwcm9maWxlRGF0YS5zZXNzaW9uSWQsXG4gICAgICAgICAgICAgICAgbWVzc2FnZTogbWVzc2FnZS5tZXNzYWdlLFxuICAgICAgICAgICAgICAgIG51bWJlcjogbWVzc2FnZS5jb250YWN0LnBob25lLFxuICAgICAgICAgICAgICAgIGluY2lkZW50OiBpbmNpZGVudElkXG4gICAgICAgICAgICB9LFxuICAgICAgICAgICAgZGF0YVR5cGU6IFwiSlNPTlwiXG4gICAgICAgIH0pO1xuICAgIH1cblxuICAgIC8qKlxuICAgICAqIHRoaXMgZnVuY3Rpb24gc2VhcmNoIGZvciBhIGNvbnRhY3RcbiAgICAgKlxuICAgICAqIEBwYXJhbSBjSWRcbiAgICAgKiBAcGFyYW0gc2Vzc2lvbklkXG4gICAgICogQHBhcmFtIHNlcnZlclVyaVxuICAgICAqIEBwYXJhbSBwaG9uZVxuICAgICAqL1xuICAgIHB1YmxpYyBzZWFyY2hDb250YWN0KHBob25lOiBzdHJpbmcsIGNJZDogc3RyaW5nLCBzZXNzaW9uSWQ6IHN0cmluZywgc2VydmVyVXJpOiBzdHJpbmcpOiBhbnkge1xuICAgICAgICB2YXIgc2VhcmNoVXJsID0gc2VydmVyVXJpLm1hdGNoKC9eW15cXC9dKzpcXC9cXC9bXlxcL10rXFwvLylbMF0ucmVwbGFjZSgvXmh0dHAoPyFzKS9pLCAnaHR0cHMnKVxuICAgICAgICAgICAgKyB0aGlzLm1lc3NhZ2VDb25maWd1cmF0aW9uLnByb3ZpZGVyUGF0aCsnL3NlYXJjaFBob25lJztcblxuICAgICAgICByZXR1cm4gJC5hamF4KHtcbiAgICAgICAgICAgIHVybDogc2VhcmNoVXJsLFxuICAgICAgICAgICAgdHlwZTogJ1BPU1QnLFxuICAgICAgICAgICAgZGF0YToge1xuICAgICAgICAgICAgICAgIHBob25lOiBwaG9uZSxcbiAgICAgICAgICAgICAgICBzZXNzaW9uX2lkOiBzZXNzaW9uSWQsXG4gICAgICAgICAgICAgICAgaWQ6IGNJZFxuICAgICAgICAgICAgfVxuICAgICAgICB9KTtcbiAgICB9XG5cbn0iXX0=