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
 *  SHA1: $Id: 856f79ab1fe08da48005150723b04b6924fc9db4 $
 * *********************************************************************************************
 *  File: ctiConstants.ts
 * ****************************************************************************************** */

/**
 * All constants used by CTI Addin
 */
export class CtiConstants {
  
  public static BUI_CTI_ADDIN_ID: string = 'bui_cti_addin';
  public static BUI_CTI_ADDIN_VERSION: string  = '1.0';
  public static BUI_CTI_LEFT_PANEL_MENU_ID: string  = 'bui-cti-left-panel-addin-10456824-65465';
  public static BUI_CTI_LEFT_PANEL_MENU_DEFAULT_LABEL: string  = 'BUI CTI ADD-IN';
  public static BUI_CTI_LEFT_PANEL_ICON_TYPE: string  = 'font awesome';
  public static BUI_CTI_LEFT_PANEL_ICON: string  = 'fa-phone-square';
  public static BUI_CTI_LEFT_PANEL_ICON_NOTIFY: string  = 'fa-phone-square flash-animated';
  public static BUI_CTI_RIBBONBAR_ICON_WAIT: string  = 'fa fa-hourglass-half flash-animated';

  public static BUI_CTI_RIBBONBAR_MENU_ID: string  = 'bui-cti-toolbar-menu-addin-900824-65465';
  public static BUI_CTI_RIBBONBAR_ICON_TYPE: string  = 'font awesome';
  public static BUI_CTI_RIBBONBAR_ICON_DEFAULT_CLASS: string  = 'fa fa-toggle-off';
  public static BUI_CTI_RIBBONBAR_ICON_DEFAULT_COLOR: string  = 'black';

  public static BUI_CTI_ICON_CLASS_AVAILABLE: string  = 'fa fa-toggle-on';
  public static BUI_CTI_ICON_CLASS_NOT_AVAILABLE: string  = 'fa fa-toggle-on';
  public static BUI_CTI_ICON_CLASS_BUSY: string  = 'fa fa-toggle-on';

  public static BUI_CTI_LABEL_LOGGED_IN: string  = 'Logged-in';
  public static BUI_CTI_LABEL_LOGGED_OUT: string  = 'Logged-out';
  public static BUI_CTI_LABEL_AVAILABLE: string  = 'Available';
  public static BUI_CTI_LABEL_NOT_AVAILABLE: string  = 'Not Available';
  public static BUI_CTI_LABEL_BUSY: string  = 'Busy';
  public static BUI_CTI_LABEL_LOGIN: string  = 'Login';
  public static BUI_CTI_LABEL_LOGOUT: string  = 'Logout';
  public static BUI_CTI_LABEL_INCOMING_CALL: string  = 'Incoming Call..';
  public static BUI_CTI_LABEL_WAIT: string  = 'Please wait..';

  public static NOT_AVAILABLE: string  = 'Offline';
  public static AVAILABLE: string  = 'Ready';
  public static BUSY: string  = 'Busy';
  public static WAIT: string  = 'Wait';

  public static UNKNOWN_CALLER: string = 'Unknown Caller';
  public static DEFAULT_DISPLAY_ICON: string = 'https://www.gravatar.com/avatar/23463b99b62a72f26ed677cc556c44e8?d=mm';
  
}