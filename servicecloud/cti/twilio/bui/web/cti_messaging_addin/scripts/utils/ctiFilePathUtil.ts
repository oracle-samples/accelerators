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
 *  SHA1: $Id: fe328afe56a452fb129eee8cc97f84c6731b8b75 $
 * *********************************************************************************************
 *  File: ctiFilePathUtil.ts
 * ****************************************************************************************** */

export class CtiFilePathUtil {
    /**
     * This function calculates the absolute file path
     * for the addin html
     *
     * @returns {string}
     */

    public static getAbsolutePath(addinFilePath: string): string {
        var base: string = window.location.href;
        var relative: string = addinFilePath;
        var stack = base.split("/"),
            parts = relative.split("/");
        stack.pop();
        for (var i=0; i<parts.length; i++) {
            if (parts[i] == ".")
                continue;
            if (parts[i] == "..")
                stack.pop();
            else
                stack.push(parts[i]);
        }
        return stack.join("/");
    }
}