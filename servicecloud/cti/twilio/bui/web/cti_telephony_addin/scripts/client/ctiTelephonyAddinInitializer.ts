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
 *  SHA1: $Id: 0432a2fc676de8f507bef8b05cf7a300da9c4e3e $
 * *********************************************************************************************
 *  File: ctiTelephonyAddinInitializer.ts
 * ****************************************************************************************** */

///<reference path='../../../definitions/osvcExtension.d.ts' />

import {CtiConstants} from './../util/ctiConstants';
/**
 * This class initializes the CTI Addin.
 * 1. It crates a left SidePane addin, set iit's content URL and set visibility
 *    to false, so that it will be rendered only after agent login to the tool
 *
 * 2. When the content is downloaded, it executes the CtiConsoleAddin.initialize()
 *    function which in turn render the GlobalHeaderMenu icon with the
 *    login option
 *
 */
export class CtiTelephonyAddinInitializer {
  private static addinFilePath:string = 'ctiTelephonyAddin.html';

  /**
   * This function calculates the absolute file path
   * for the addin html
   * 
   * @returns {string}
   */
  private static getAbsolutePath(): string {
    var base: string = window.location.href;
    var relative: string = CtiTelephonyAddinInitializer.addinFilePath;
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

  /**
   * This is the entry point of CTI Addin.
   * This function initialize the sidepane addin, set content url and
   * set visibility to false.
   * 
   */
  public static initialize():void {
    ORACLE_SERVICE_CLOUD.extension_loader.load(CtiConstants.BUI_CTI_ADDIN_ID, CtiConstants.BUI_CTI_ADDIN_VERSION).then((sdk) => {
      sdk.registerUserInterfaceExtension((userInterfaceContext) => {
        userInterfaceContext.getLeftSidePaneContext().then(
          (leftSidePaneContext) => {
            leftSidePaneContext.getSidePane(CtiConstants.BUI_CTI_LEFT_PANEL_MENU_ID).then((leftPanelMenu) => {
              leftPanelMenu.setContentUrl(CtiTelephonyAddinInitializer.getAbsolutePath());
              leftPanelMenu.setLabel(CtiConstants.BUI_CTI_LEFT_PANEL_MENU_DEFAULT_LABEL);
              leftPanelMenu.setVisible(false);
              var icon = leftPanelMenu.createIcon(CtiConstants.BUI_CTI_LEFT_PANEL_ICON_TYPE);
              icon.setIconClass(CtiConstants.BUI_CTI_LEFT_PANEL_ICON);
              leftPanelMenu.addIcon(icon);
              leftPanelMenu.render();
            });
          }
        );
      });
    });
  }
}

//Initialize the addin
CtiTelephonyAddinInitializer.initialize();