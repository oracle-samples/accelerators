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
 *  SHA1: $Id: 71acfc7d13754b0f4f22eb3444e673fc5c809609 $
 * *********************************************************************************************
 *  File: ctiMessageViewHelper.js
 * ****************************************************************************************** */
define(["require", "exports", "jquery"], function (require, exports, $) {
    "use strict";
    exports.__esModule = true;
    /**
     * This utility class is used to update the UI
     */
    var CtiMessageViewHelper = /** @class */ (function () {
        function CtiMessageViewHelper() {
        }
        /**
         * Enable messaging for a workspace
         *
         * @param message
         * @param inputListener
         * @param buttonClickHandler
         */
        CtiMessageViewHelper.enableMessagingView = function (message, inputListener, buttonClickHandler) {
            var inputElement = $("#messaging_content");
            $("#messaging_contact_image").attr('src', message.contact.dp);
            $("#messaging_name").html(message.contact.name);
            $('#messaging_contact_number').html(message.contact.phone);
            inputElement.val(message.message);
            inputElement.off().on('change', function (event) {
                inputListener(message.key, event);
            });
            var sendButtonElement = $("#message_send_btn");
            sendButtonElement.off().on('click', function (event) {
                sendButtonElement.attr('class', 'button button-green disabled');
                buttonClickHandler(message.key);
            });
        };
        /**
         * Change UI when sending message is success
         */
        CtiMessageViewHelper.enableSendButtonControlOnSuccess = function () {
            var sendButtonElement = $("#message_send_btn");
            var inputElement = $("#messaging_content");
            inputElement.val('');
            sendButtonElement.attr('class', 'button button-green');
        };
        /**
         * Change UI when sending message is failure
         */
        CtiMessageViewHelper.enableSendButtonControlOnFailure = function () {
            var sendButtonElement = $("#message_send_btn");
            alert('Unable to send message.');
            sendButtonElement.attr('class', 'button button-green');
        };
        return CtiMessageViewHelper;
    }());
    exports.CtiMessageViewHelper = CtiMessageViewHelper;
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiY3RpTWVzc2FnZVZpZXdIZWxwZXIuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJjdGlNZXNzYWdlVmlld0hlbHBlci50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTs7Ozs7Z0dBS2dHOzs7O0lBS2hHOztPQUVHO0lBQ0g7UUFBQTtRQTJDQSxDQUFDO1FBMUNHOzs7Ozs7V0FNRztRQUNXLHdDQUFtQixHQUFqQyxVQUFrQyxPQUFnQixFQUFFLGFBQWtCLEVBQUUsa0JBQXVCO1lBQzNGLElBQUksWUFBWSxHQUFRLENBQUMsQ0FBQyxvQkFBb0IsQ0FBQyxDQUFDO1lBQ2hELENBQUMsQ0FBQywwQkFBMEIsQ0FBQyxDQUFDLElBQUksQ0FBQyxLQUFLLEVBQUUsT0FBTyxDQUFDLE9BQU8sQ0FBQyxFQUFFLENBQUMsQ0FBQztZQUM5RCxDQUFDLENBQUMsaUJBQWlCLENBQUMsQ0FBQyxJQUFJLENBQUMsT0FBTyxDQUFDLE9BQU8sQ0FBQyxJQUFJLENBQUMsQ0FBQztZQUNoRCxDQUFDLENBQUMsMkJBQTJCLENBQUMsQ0FBQyxJQUFJLENBQUMsT0FBTyxDQUFDLE9BQU8sQ0FBQyxLQUFLLENBQUMsQ0FBQztZQUMzRCxZQUFZLENBQUMsR0FBRyxDQUFDLE9BQU8sQ0FBQyxPQUFPLENBQUMsQ0FBQztZQUNsQyxZQUFZLENBQUMsR0FBRyxFQUFFLENBQUMsRUFBRSxDQUFDLFFBQVEsRUFBRSxVQUFDLEtBQVU7Z0JBQ3ZDLGFBQWEsQ0FBQyxPQUFPLENBQUMsR0FBRyxFQUFFLEtBQUssQ0FBQyxDQUFDO1lBQ3RDLENBQUMsQ0FBQyxDQUFDO1lBQ0gsSUFBSSxpQkFBaUIsR0FBUSxDQUFDLENBQUMsbUJBQW1CLENBQUMsQ0FBQztZQUNwRCxpQkFBaUIsQ0FBQyxHQUFHLEVBQUUsQ0FBQyxFQUFFLENBQUMsT0FBTyxFQUFFLFVBQUMsS0FBVTtnQkFDM0MsaUJBQWlCLENBQUMsSUFBSSxDQUFDLE9BQU8sRUFBRSw4QkFBOEIsQ0FBQyxDQUFDO2dCQUNoRSxrQkFBa0IsQ0FBQyxPQUFPLENBQUMsR0FBRyxDQUFDLENBQUM7WUFDcEMsQ0FBQyxDQUFDLENBQUM7UUFDUCxDQUFDO1FBRUQ7O1dBRUc7UUFDVyxxREFBZ0MsR0FBOUM7WUFDSSxJQUFJLGlCQUFpQixHQUFRLENBQUMsQ0FBQyxtQkFBbUIsQ0FBQyxDQUFDO1lBQ3BELElBQUksWUFBWSxHQUFRLENBQUMsQ0FBQyxvQkFBb0IsQ0FBQyxDQUFDO1lBQ2hELFlBQVksQ0FBQyxHQUFHLENBQUMsRUFBRSxDQUFDLENBQUM7WUFDckIsaUJBQWlCLENBQUMsSUFBSSxDQUFDLE9BQU8sRUFBRSxxQkFBcUIsQ0FBQyxDQUFDO1FBQzNELENBQUM7UUFFRDs7V0FFRztRQUNXLHFEQUFnQyxHQUE5QztZQUNJLElBQUksaUJBQWlCLEdBQVEsQ0FBQyxDQUFDLG1CQUFtQixDQUFDLENBQUM7WUFDcEQsS0FBSyxDQUFDLHlCQUF5QixDQUFDLENBQUM7WUFDakMsaUJBQWlCLENBQUMsSUFBSSxDQUFDLE9BQU8sRUFBRSxxQkFBcUIsQ0FBQyxDQUFDO1FBQzNELENBQUM7UUFFTCwyQkFBQztJQUFELENBQUMsQUEzQ0QsSUEyQ0M7SUEzQ1ksb0RBQW9CIiwic291cmNlc0NvbnRlbnQiOlsiLyogKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXG4gKiAgJEFDQ0VMRVJBVE9SX0hFQURFUl9QTEFDRV9IT0xERVIkXG4gKiAgU0hBMTogJElkOiA3MWFjZmM3ZDEzNzU0YjBmNGYyMmViMzQ0NGU2NzNmYzVjODA5NjA5ICRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogIEZpbGU6ICRBQ0NFTEVSQVRPUl9IRUFERVJfRklMRV9OQU1FX1BMQUNFX0hPTERFUiRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiAqL1xuXG5pbXBvcnQgJCA9IHJlcXVpcmUoJ2pxdWVyeScpO1xuaW1wb3J0IHtNZXNzYWdlfSBmcm9tIFwiLi4vbW9kZWwvbWVzc2FnZVwiO1xuXG4vKipcbiAqIFRoaXMgdXRpbGl0eSBjbGFzcyBpcyB1c2VkIHRvIHVwZGF0ZSB0aGUgVUlcbiAqL1xuZXhwb3J0IGNsYXNzIEN0aU1lc3NhZ2VWaWV3SGVscGVyIHtcbiAgICAvKipcbiAgICAgKiBFbmFibGUgbWVzc2FnaW5nIGZvciBhIHdvcmtzcGFjZVxuICAgICAqXG4gICAgICogQHBhcmFtIG1lc3NhZ2VcbiAgICAgKiBAcGFyYW0gaW5wdXRMaXN0ZW5lclxuICAgICAqIEBwYXJhbSBidXR0b25DbGlja0hhbmRsZXJcbiAgICAgKi9cbiAgICBwdWJsaWMgc3RhdGljIGVuYWJsZU1lc3NhZ2luZ1ZpZXcobWVzc2FnZTogTWVzc2FnZSwgaW5wdXRMaXN0ZW5lcjogYW55LCBidXR0b25DbGlja0hhbmRsZXI6IGFueSk6IHZvaWQge1xuICAgICAgICB2YXIgaW5wdXRFbGVtZW50OiBhbnkgPSAkKFwiI21lc3NhZ2luZ19jb250ZW50XCIpO1xuICAgICAgICAkKFwiI21lc3NhZ2luZ19jb250YWN0X2ltYWdlXCIpLmF0dHIoJ3NyYycsIG1lc3NhZ2UuY29udGFjdC5kcCk7XG4gICAgICAgICQoXCIjbWVzc2FnaW5nX25hbWVcIikuaHRtbChtZXNzYWdlLmNvbnRhY3QubmFtZSk7XG4gICAgICAgICQoJyNtZXNzYWdpbmdfY29udGFjdF9udW1iZXInKS5odG1sKG1lc3NhZ2UuY29udGFjdC5waG9uZSk7XG4gICAgICAgIGlucHV0RWxlbWVudC52YWwobWVzc2FnZS5tZXNzYWdlKTtcbiAgICAgICAgaW5wdXRFbGVtZW50Lm9mZigpLm9uKCdjaGFuZ2UnLCAoZXZlbnQ6IGFueSkgPT4ge1xuICAgICAgICAgICAgaW5wdXRMaXN0ZW5lcihtZXNzYWdlLmtleSwgZXZlbnQpO1xuICAgICAgICB9KTtcbiAgICAgICAgdmFyIHNlbmRCdXR0b25FbGVtZW50OiBhbnkgPSAkKFwiI21lc3NhZ2Vfc2VuZF9idG5cIik7XG4gICAgICAgIHNlbmRCdXR0b25FbGVtZW50Lm9mZigpLm9uKCdjbGljaycsIChldmVudDogYW55KSA9PiB7XG4gICAgICAgICAgICBzZW5kQnV0dG9uRWxlbWVudC5hdHRyKCdjbGFzcycsICdidXR0b24gYnV0dG9uLWdyZWVuIGRpc2FibGVkJyk7XG4gICAgICAgICAgICBidXR0b25DbGlja0hhbmRsZXIobWVzc2FnZS5rZXkpO1xuICAgICAgICB9KTtcbiAgICB9XG5cbiAgICAvKipcbiAgICAgKiBDaGFuZ2UgVUkgd2hlbiBzZW5kaW5nIG1lc3NhZ2UgaXMgc3VjY2Vzc1xuICAgICAqL1xuICAgIHB1YmxpYyBzdGF0aWMgZW5hYmxlU2VuZEJ1dHRvbkNvbnRyb2xPblN1Y2Nlc3MoKTogdm9pZCB7XG4gICAgICAgIHZhciBzZW5kQnV0dG9uRWxlbWVudDogYW55ID0gJChcIiNtZXNzYWdlX3NlbmRfYnRuXCIpO1xuICAgICAgICB2YXIgaW5wdXRFbGVtZW50OiBhbnkgPSAkKFwiI21lc3NhZ2luZ19jb250ZW50XCIpO1xuICAgICAgICBpbnB1dEVsZW1lbnQudmFsKCcnKTtcbiAgICAgICAgc2VuZEJ1dHRvbkVsZW1lbnQuYXR0cignY2xhc3MnLCAnYnV0dG9uIGJ1dHRvbi1ncmVlbicpO1xuICAgIH1cblxuICAgIC8qKlxuICAgICAqIENoYW5nZSBVSSB3aGVuIHNlbmRpbmcgbWVzc2FnZSBpcyBmYWlsdXJlXG4gICAgICovXG4gICAgcHVibGljIHN0YXRpYyBlbmFibGVTZW5kQnV0dG9uQ29udHJvbE9uRmFpbHVyZSgpOiB2b2lkIHtcbiAgICAgICAgdmFyIHNlbmRCdXR0b25FbGVtZW50OiBhbnkgPSAkKFwiI21lc3NhZ2Vfc2VuZF9idG5cIik7XG4gICAgICAgIGFsZXJ0KCdVbmFibGUgdG8gc2VuZCBtZXNzYWdlLicpO1xuICAgICAgICBzZW5kQnV0dG9uRWxlbWVudC5hdHRyKCdjbGFzcycsICdidXR0b24gYnV0dG9uLWdyZWVuJyk7XG4gICAgfVxuXG59XG4iXX0=