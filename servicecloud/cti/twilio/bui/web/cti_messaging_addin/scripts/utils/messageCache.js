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
 *  SHA1: $Id: 1da0f95da651d91031c287ce3e0d368a7e8e7868 $
 * *********************************************************************************************
 *  File: messageCache.js
 * ****************************************************************************************** */
define(["require", "exports"], function (require, exports) {
    "use strict";
    exports.__esModule = true;
    /**
     * This class keeps all messages for
     * currently opened workspaces
     */
    var MessageCache = /** @class */ (function () {
        function MessageCache() {
        }
        /**
         * Returns the message with given key
         *
         * @param key
         * @returns {Message}
         */
        MessageCache.get = function (key) {
            return MessageCache.messages[key];
        };
        /**
         * Add a message to the cache, with given key
         *
         * @param key
         * @param message
         */
        MessageCache.put = function (key, message) {
            MessageCache.messages[key] = message;
            MessageCache.cacheSize++;
        };
        /**
         * Removes  message with given key
         *
         * @param key
         */
        MessageCache.remove = function (key) {
            if (MessageCache.messages[key]) {
                MessageCache.messages[key] = null;
                MessageCache.cacheSize--;
            }
        };
        MessageCache.clearMessage = function (key) {
            if (MessageCache.messages[key]) {
                MessageCache.messages[key].message = '';
            }
        };
        /**
         * Clear all messages from cache
         *
         */
        MessageCache.clearCache = function () {
            MessageCache.messages = {};
            MessageCache.cacheSize = 0;
        };
        /**
         * Return the number of messages in cache
         *
         * @returns {number}
         */
        MessageCache.getCacheSize = function () {
            return MessageCache.cacheSize;
        };
        MessageCache.messages = {};
        MessageCache.cacheSize = 0;
        return MessageCache;
    }());
    exports.MessageCache = MessageCache;
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoibWVzc2FnZUNhY2hlLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsibWVzc2FnZUNhY2hlLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBOzs7OztnR0FLZ0c7Ozs7SUFHaEc7OztPQUdHO0lBQ0g7UUFBQTtRQTREQSxDQUFDO1FBeERHOzs7OztXQUtHO1FBQ1csZ0JBQUcsR0FBakIsVUFBa0IsR0FBVztZQUN6QixNQUFNLENBQUMsWUFBWSxDQUFDLFFBQVEsQ0FBQyxHQUFHLENBQUMsQ0FBQztRQUN0QyxDQUFDO1FBRUQ7Ozs7O1dBS0c7UUFDVyxnQkFBRyxHQUFqQixVQUFrQixHQUFXLEVBQUUsT0FBZ0I7WUFDM0MsWUFBWSxDQUFDLFFBQVEsQ0FBQyxHQUFHLENBQUMsR0FBRyxPQUFPLENBQUM7WUFDckMsWUFBWSxDQUFDLFNBQVMsRUFBRSxDQUFDO1FBQzdCLENBQUM7UUFFRDs7OztXQUlHO1FBQ1csbUJBQU0sR0FBcEIsVUFBcUIsR0FBVztZQUM1QixFQUFFLENBQUEsQ0FBQyxZQUFZLENBQUMsUUFBUSxDQUFDLEdBQUcsQ0FBQyxDQUFDLENBQUEsQ0FBQztnQkFDM0IsWUFBWSxDQUFDLFFBQVEsQ0FBQyxHQUFHLENBQUMsR0FBRyxJQUFJLENBQUM7Z0JBQ2xDLFlBQVksQ0FBQyxTQUFTLEVBQUUsQ0FBQztZQUM3QixDQUFDO1FBQ0wsQ0FBQztRQUVhLHlCQUFZLEdBQTFCLFVBQTJCLEdBQVc7WUFDbEMsRUFBRSxDQUFBLENBQUMsWUFBWSxDQUFDLFFBQVEsQ0FBQyxHQUFHLENBQUMsQ0FBQyxDQUFBLENBQUM7Z0JBQzNCLFlBQVksQ0FBQyxRQUFRLENBQUMsR0FBRyxDQUFDLENBQUMsT0FBTyxHQUFHLEVBQUUsQ0FBQztZQUM1QyxDQUFDO1FBQ0wsQ0FBQztRQUVEOzs7V0FHRztRQUNXLHVCQUFVLEdBQXhCO1lBQ0ksWUFBWSxDQUFDLFFBQVEsR0FBRyxFQUFFLENBQUM7WUFDM0IsWUFBWSxDQUFDLFNBQVMsR0FBRyxDQUFDLENBQUM7UUFDL0IsQ0FBQztRQUVEOzs7O1dBSUc7UUFDVyx5QkFBWSxHQUExQjtZQUNJLE1BQU0sQ0FBQyxZQUFZLENBQUMsU0FBUyxDQUFDO1FBQ2xDLENBQUM7UUExRGMscUJBQVEsR0FBNkIsRUFBRSxDQUFDO1FBQ3hDLHNCQUFTLEdBQVcsQ0FBQyxDQUFDO1FBMER6QyxtQkFBQztLQUFBLEFBNURELElBNERDO0lBNURZLG9DQUFZIiwic291cmNlc0NvbnRlbnQiOlsiLyogKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXG4gKiAgJEFDQ0VMRVJBVE9SX0hFQURFUl9QTEFDRV9IT0xERVIkXG4gKiAgU0hBMTogJElkOiAxZGEwZjk1ZGE2NTFkOTEwMzFjMjg3Y2UzZTBkMzY4YTdlOGU3ODY4ICRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogIEZpbGU6ICRBQ0NFTEVSQVRPUl9IRUFERVJfRklMRV9OQU1FX1BMQUNFX0hPTERFUiRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiAqL1xuXG5pbXBvcnQge01lc3NhZ2V9IGZyb20gXCIuLi9tb2RlbC9tZXNzYWdlXCI7XG4vKipcbiAqIFRoaXMgY2xhc3Mga2VlcHMgYWxsIG1lc3NhZ2VzIGZvclxuICogY3VycmVudGx5IG9wZW5lZCB3b3Jrc3BhY2VzXG4gKi9cbmV4cG9ydCBjbGFzcyBNZXNzYWdlQ2FjaGUge1xuICAgIHByaXZhdGUgc3RhdGljIG1lc3NhZ2VzOiB7W2tleTogc3RyaW5nXTogTWVzc2FnZX0gPSB7fTtcbiAgICBwcml2YXRlIHN0YXRpYyBjYWNoZVNpemU6IG51bWJlciA9IDA7XG5cbiAgICAvKipcbiAgICAgKiBSZXR1cm5zIHRoZSBtZXNzYWdlIHdpdGggZ2l2ZW4ga2V5XG4gICAgICpcbiAgICAgKiBAcGFyYW0ga2V5XG4gICAgICogQHJldHVybnMge01lc3NhZ2V9XG4gICAgICovXG4gICAgcHVibGljIHN0YXRpYyBnZXQoa2V5OiBzdHJpbmcpOiBNZXNzYWdlIHtcbiAgICAgICAgcmV0dXJuIE1lc3NhZ2VDYWNoZS5tZXNzYWdlc1trZXldO1xuICAgIH1cblxuICAgIC8qKlxuICAgICAqIEFkZCBhIG1lc3NhZ2UgdG8gdGhlIGNhY2hlLCB3aXRoIGdpdmVuIGtleVxuICAgICAqXG4gICAgICogQHBhcmFtIGtleVxuICAgICAqIEBwYXJhbSBtZXNzYWdlXG4gICAgICovXG4gICAgcHVibGljIHN0YXRpYyBwdXQoa2V5OiBzdHJpbmcsIG1lc3NhZ2U6IE1lc3NhZ2UpOiB2b2lkIHtcbiAgICAgICAgTWVzc2FnZUNhY2hlLm1lc3NhZ2VzW2tleV0gPSBtZXNzYWdlO1xuICAgICAgICBNZXNzYWdlQ2FjaGUuY2FjaGVTaXplKys7XG4gICAgfVxuXG4gICAgLyoqXG4gICAgICogUmVtb3ZlcyAgbWVzc2FnZSB3aXRoIGdpdmVuIGtleVxuICAgICAqXG4gICAgICogQHBhcmFtIGtleVxuICAgICAqL1xuICAgIHB1YmxpYyBzdGF0aWMgcmVtb3ZlKGtleTogc3RyaW5nKTogdm9pZCB7XG4gICAgICAgIGlmKE1lc3NhZ2VDYWNoZS5tZXNzYWdlc1trZXldKXtcbiAgICAgICAgICAgIE1lc3NhZ2VDYWNoZS5tZXNzYWdlc1trZXldID0gbnVsbDtcbiAgICAgICAgICAgIE1lc3NhZ2VDYWNoZS5jYWNoZVNpemUtLTtcbiAgICAgICAgfVxuICAgIH1cblxuICAgIHB1YmxpYyBzdGF0aWMgY2xlYXJNZXNzYWdlKGtleTogc3RyaW5nKTogdm9pZCB7XG4gICAgICAgIGlmKE1lc3NhZ2VDYWNoZS5tZXNzYWdlc1trZXldKXtcbiAgICAgICAgICAgIE1lc3NhZ2VDYWNoZS5tZXNzYWdlc1trZXldLm1lc3NhZ2UgPSAnJztcbiAgICAgICAgfVxuICAgIH1cblxuICAgIC8qKlxuICAgICAqIENsZWFyIGFsbCBtZXNzYWdlcyBmcm9tIGNhY2hlXG4gICAgICpcbiAgICAgKi9cbiAgICBwdWJsaWMgc3RhdGljIGNsZWFyQ2FjaGUoKTogdm9pZCB7XG4gICAgICAgIE1lc3NhZ2VDYWNoZS5tZXNzYWdlcyA9IHt9O1xuICAgICAgICBNZXNzYWdlQ2FjaGUuY2FjaGVTaXplID0gMDtcbiAgICB9XG5cbiAgICAvKipcbiAgICAgKiBSZXR1cm4gdGhlIG51bWJlciBvZiBtZXNzYWdlcyBpbiBjYWNoZVxuICAgICAqIFxuICAgICAqIEByZXR1cm5zIHtudW1iZXJ9XG4gICAgICovXG4gICAgcHVibGljIHN0YXRpYyBnZXRDYWNoZVNpemUoKTogbnVtYmVyIHtcbiAgICAgICAgcmV0dXJuIE1lc3NhZ2VDYWNoZS5jYWNoZVNpemU7XG4gICAgfVxufSJdfQ==