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
 *  SHA1: $Id: 69ac8c6014ffe435fb185c7a5ba688f18b8c0f36 $
 * *********************************************************************************************
 *  File: ctiMessagingAddinInitializer.ts
 * ****************************************************************************************** */

import {MessageConstants} from "../utils/messageConstants";
import {CtiFilePathUtil} from "../utils/ctiFilePathUtil";
import ISidePane = ORACLE_SERVICE_CLOUD.ISidePane;

export class CtiMessagingAddinInitializer {

    private static addinFilePath:string = 'ctiMessagingAddin.html';

    public static initialize(): void {
        //TODO - Authorization
        ORACLE_SERVICE_CLOUD.extension_loader.load(MessageConstants.BUI_CTI_SMS_ADDIN_ID, MessageConstants.BUI_CTI_SMS_ADDIN_VERSION).then((sdk) => {
            sdk.registerUserInterfaceExtension((userInterfaceContext) => {
                userInterfaceContext.getLeftSidePaneContext().then(
                    (leftSidePaneContext) => {
                        leftSidePaneContext.getSidePane(MessageConstants.BUI_CTI_LEFT_PANEL_SMS_MENU_ID).then((leftPanelMenu: ISidePane) => {
                            //TODO - check this path
                            leftPanelMenu.setContentUrl(CtiFilePathUtil.getAbsolutePath(CtiMessagingAddinInitializer.addinFilePath));
                            leftPanelMenu.setVisible(false);
                            leftPanelMenu.render();
                        });
                    }
                );
            });
        });
    }
}

CtiMessagingAddinInitializer.initialize();