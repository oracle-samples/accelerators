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
 *  SHA1: $Id: 8449e222a43cba89f512916513ae8c03d64ced6e $
 * *********************************************************************************************
 *  File: app_init.js
 * ****************************************************************************************** */
/*global require*/
'use strict';
require.config({
    baseUrl: './',
    paths: {}
});
requirejs(['../scripts/client/ctiTelephonyAddinInitializer.js']);
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiYXBwX2luaXQuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJhcHBfaW5pdC50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTs7Ozs7Z0dBS2dHO0FBRWhHLGtCQUFrQjtBQUNsQixZQUFZLENBQUM7QUFDYixPQUFPLENBQUMsTUFBTSxDQUFDO0lBQ1gsT0FBTyxFQUFFLElBQUk7SUFDYixLQUFLLEVBQUUsRUFDTjtDQUNKLENBQUMsQ0FBQztBQUVILFNBQVMsQ0FBQyxDQUFDLG1EQUFtRCxDQUFDLENBQUMsQ0FBQyIsInNvdXJjZXNDb250ZW50IjpbIi8qICogKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogICRBQ0NFTEVSQVRPUl9IRUFERVJfUExBQ0VfSE9MREVSJFxuICogIFNIQTE6ICRJZDogODQ0OWUyMjJhNDNjYmE4OWY1MTI5MTY1MTNhZThjMDNkNjRjZWQ2ZSAkXG4gKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKipcbiAqICBGaWxlOiAkQUNDRUxFUkFUT1JfSEVBREVSX0ZJTEVfTkFNRV9QTEFDRV9IT0xERVIkXG4gKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiogKi9cblxuLypnbG9iYWwgcmVxdWlyZSovXG4ndXNlIHN0cmljdCc7XG5yZXF1aXJlLmNvbmZpZyh7XG4gICAgYmFzZVVybDogJy4vJyxcbiAgICBwYXRoczoge1xuICAgIH1cbn0pO1xuXG5yZXF1aXJlanMoWycuLi9zY3JpcHRzL2NsaWVudC9jdGlUZWxlcGhvbnlBZGRpbkluaXRpYWxpemVyLmpzJ10pO1xuXG4iXX0=