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
 *  SHA1: $Id: a1550c6e9f0258c0da2756e49c14c1729e42ec41 $
 * *********************************************************************************************
 *  File: app_main.js
 * ****************************************************************************************** */
/*global require*/
'use strict';
require.config({
    baseUrl: './',
    paths: {
        jquery: '../../ext-lib/jquery/dist/jquery.min'
    }
});
requirejs(['../scripts/client/ctiTelephonyAddin.js']);
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiYXBwX21haW4uanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJhcHBfbWFpbi50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTs7Ozs7Z0dBS2dHO0FBRWhHLGtCQUFrQjtBQUNsQixZQUFZLENBQUM7QUFDYixPQUFPLENBQUMsTUFBTSxDQUFDO0lBQ1gsT0FBTyxFQUFFLElBQUk7SUFDYixLQUFLLEVBQUU7UUFDSCxNQUFNLEVBQUUsc0NBQXNDO0tBQ2pEO0NBQ0osQ0FBQyxDQUFDO0FBRUgsU0FBUyxDQUFDLENBQUMsd0NBQXdDLENBQUMsQ0FBQyxDQUFDIiwic291cmNlc0NvbnRlbnQiOlsiLyogKiAqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXG4gKiAgJEFDQ0VMRVJBVE9SX0hFQURFUl9QTEFDRV9IT0xERVIkXG4gKiAgU0hBMTogJElkOiBhMTU1MGM2ZTlmMDI1OGMwZGEyNzU2ZTQ5YzE0YzE3MjllNDJlYzQxICRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxuICogIEZpbGU6ICRBQ0NFTEVSQVRPUl9IRUFERVJfRklMRV9OQU1FX1BMQUNFX0hPTERFUiRcbiAqICoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKiAqL1xuXG4vKmdsb2JhbCByZXF1aXJlKi9cbid1c2Ugc3RyaWN0JztcbnJlcXVpcmUuY29uZmlnKHtcbiAgICBiYXNlVXJsOiAnLi8nLFxuICAgIHBhdGhzOiB7XG4gICAgICAgIGpxdWVyeTogJy4uLy4uL2V4dC1saWIvanF1ZXJ5L2Rpc3QvanF1ZXJ5Lm1pbidcbiAgICB9XG59KTtcblxucmVxdWlyZWpzKFsnLi4vc2NyaXB0cy9jbGllbnQvY3RpVGVsZXBob255QWRkaW4uanMnXSk7XG5cbiJdfQ==