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
 *  SHA1: $Id: 9c7600394aa307f781c098bfad67158f6217c7e5 $
 * *********************************************************************************************
 *  File: app_messaging.js
 * ****************************************************************************************** */
/*global require*/
'use strict';
require.config({
    baseUrl: './',
    paths: {
        jquery: '../../ext-lib/jquery/dist/jquery.min'
    }
});
requirejs(['../scripts/client/ctiMessagingAddin.js']);
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiYXBwX21lc3NhZ2luZy5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbImFwcF9tZXNzYWdpbmcudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7Ozs7O2dHQUtnRztBQUVoRyxrQkFBa0I7QUFDbEIsWUFBWSxDQUFDO0FBQ2IsT0FBTyxDQUFDLE1BQU0sQ0FBQztJQUNYLE9BQU8sRUFBRSxJQUFJO0lBQ2IsS0FBSyxFQUFFO1FBQ0gsTUFBTSxFQUFFLHNDQUFzQztLQUNqRDtDQUNKLENBQUMsQ0FBQztBQUVILFNBQVMsQ0FBQyxDQUFDLHdDQUF3QyxDQUFDLENBQUMsQ0FBQyIsInNvdXJjZXNDb250ZW50IjpbIi8qICogKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogICRBQ0NFTEVSQVRPUl9IRUFERVJfUExBQ0VfSE9MREVSJFxuICogIFNIQTE6ICRJZDogOWM3NjAwMzk0YWEzMDdmNzgxYzA5OGJmYWQ2NzE1OGY2MjE3YzdlNSAkXG4gKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKipcbiAqICBGaWxlOiAkQUNDRUxFUkFUT1JfSEVBREVSX0ZJTEVfTkFNRV9QTEFDRV9IT0xERVIkXG4gKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiogKi9cblxuLypnbG9iYWwgcmVxdWlyZSovXG4ndXNlIHN0cmljdCc7XG5yZXF1aXJlLmNvbmZpZyh7XG4gICAgYmFzZVVybDogJy4vJyxcbiAgICBwYXRoczoge1xuICAgICAgICBqcXVlcnk6ICcuLi8uLi9leHQtbGliL2pxdWVyeS9kaXN0L2pxdWVyeS5taW4nXG4gICAgfVxufSk7XG5cbnJlcXVpcmVqcyhbJy4uL3NjcmlwdHMvY2xpZW50L2N0aU1lc3NhZ2luZ0FkZGluLmpzJ10pO1xuXG4iXX0=