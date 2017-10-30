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
 *  SHA1: $Id: ed91cae504d149f15678239d1282294f6e8eb908 $
 * *********************************************************************************************
 *  File: twilio.d.ts
 * ****************************************************************************************** */

interface TwilioMain {
    Device;
    TaskRouter;
}

declare module "Twilio" {
    export = Twilio;
}

declare var Twilio: TwilioMain;