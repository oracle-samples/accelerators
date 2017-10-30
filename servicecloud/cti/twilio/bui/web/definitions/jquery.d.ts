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
 *  SHA1: $Id: 3ca7e911d593ad3cb01df6dad13a50501d7c74b8 $
 * *********************************************************************************************
 *  File: jquery.d.ts
 * ****************************************************************************************** */

interface JQueryMain {
    ajax(settings: any): any;
    (callback: any): JQuery;
    each(
        collection: any,
        callback: (arrayIndex: any, value: any) => any
    ): any;
}

interface JQuery {
    html(htmlString: string): JQuery;
    attr(attributeName: string, value: any): JQuery;
    attr(attributeName: string): JQuery;
    off(): JQuery;
    on(events: string, handler: any): JQuery;
    on(events: string, selector: any, handler: any): JQuery;
    hide(element?: string): JQuery;
    show(element?: string): JQuery;
    val(value?: any): JQuery;

}
declare module "jquery" {
    export = $;
}
declare var $: JQueryMain;
