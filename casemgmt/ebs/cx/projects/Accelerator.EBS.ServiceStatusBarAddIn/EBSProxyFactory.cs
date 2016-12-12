/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:47 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 0836ba6218bca26beb6d6ae20a59905263d222d6 $
 * *********************************************************************************************
 *  File: EBSProxyFactory.cs
 * *********************************************************************************************/

using Accelerator.EBS.SharedServices.ProxyClasses.ServiceRequest;
using Accelerator.EBS.SharedServices.ProxyClasses.ServiceRequest.Lookup;
using Accelerator.EBS.SharedServices.ProxyClasses.RepairOrder;
using Accelerator.EBS.SharedServices.ProxyClasses.Interaction;
using Accelerator.EBS.SharedServices.ProxyClasses.Contact;
using Accelerator.EBS.SharedServices.ProxyClasses.Item;
using Accelerator.EBS.SharedServices.ProxyClasses.Entitlement;
using Accelerator.EBS.SharedServices.ProxyClasses.RepairOrderList;
using Accelerator.EBS.SharedServices.ProxyClasses.RepairLogisticsList;
using Accelerator.EBS.SharedServices.ProxyClasses.OrderMgmt;
using Accelerator.EBS.SharedServices.ProxyClasses.OrdersByContact;

using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Security;
using Microsoft.Web.Services3.Security.Tokens;

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using Accelerator.EBS.SharedServices.ProxyClasses.OrderMgmtInbound;

namespace Accelerator.EBS.SharedServices
{
    internal static class EBSProxyFactory
    {
        private static CS_SERVICEREQUEST_PUB_Service _sr_client;
        private static CSD_REPAIRS_PUB_Service _depot_client;
        private static JTF_NOTES_PUB_Service _interaction_client;
        private static HZ_INTEGRATION_PUB_Service _contact_client;
        private static CSI_ITEM_INSTANCE_PUB_Service _item_client;
        private static OKS_ENTITLEMENTS_PUB_Service _entitlement_client;
        private static CSD_REPAIR_ORDERS_WEB_Service _repair_order_list_client;
        private static CSD_LOGISTICS_WEB_Service _repair_logistics_client;
        private static OE_ORDER_PUB_Service _order_client;
        private static OE_ORDER_CUST_Service _orderByCust_client;
        private static OE_INBOUND_INT_Service _order_inbound;

        private static void serviceClientSetting(WebServicesClientProtocol serviceClient, string url, string username = null, string password = null, int timeout = -1)
        {
            serviceClient.Url = url;
            /* timeout is 0 if config verb EBSServiceTimeout is not defined.
             * It is set at the _sr_client CS_SERVICEREQUEST_PUB_Service() instance
             * level, so all the ws calls (from this proxy client) have this timeout setting
             */
            serviceClient.Timeout = timeout == 0 ? -1 : timeout;

            if (!String.IsNullOrWhiteSpace(username) && !String.IsNullOrWhiteSpace(password))
            {
                // add wsse:Security headers.                    
                UsernameToken userNameToken = new UsernameToken(username, password, PasswordOption.SendPlainText);
                SoapContext soapContext = serviceClient.RequestSoapContext;
                soapContext.Security.Tokens.Add(userNameToken);
            }
        }
        public static CS_SERVICEREQUEST_PUB_Service GetSRServiceInstance(string url, string username = null, string password = null, int timeout = -1)
        {
            if (_sr_client == null)
            {
                _sr_client = new CS_SERVICEREQUEST_PUB_Service();
                serviceClientSetting(_sr_client, url, username, password, timeout);
            }
            return _sr_client;
        }

        public static OE_ORDER_PUB_Service GetOrderServiceInstance(string url, string username = null, string password = null, int timeout = -1)
        {
            if (_order_client == null)
            {
                _order_client = new OE_ORDER_PUB_Service();
                serviceClientSetting(_order_client, url, username, password, timeout);
            }
            return _order_client;
        }

        public static OE_INBOUND_INT_Service GetOrderInboundServiceInstance(string url, string username = null, string password = null, int timeout = -1)
        {
            if (_order_inbound == null)
            {
                _order_inbound = new OE_INBOUND_INT_Service();
                serviceClientSetting(_order_inbound, url, username, password, timeout);
            }
            return _order_inbound;
        }

        public static OE_ORDER_CUST_Service GetOrdersByContactServiceInstance(string url, string username = null, string password = null, int timeout = -1)
        {
            if (_orderByCust_client == null)
            {
                _orderByCust_client = new OE_ORDER_CUST_Service();
                serviceClientSetting(_orderByCust_client, url, username, password, timeout);
            }
            return _orderByCust_client;
        }

        public static JTF_NOTES_PUB_Service GetNoteInstance(string url, string username = null, string password = null, int timeout = -1)
        {
            if (_interaction_client == null)
            {
                _interaction_client = new JTF_NOTES_PUB_Service();
                serviceClientSetting(_interaction_client, url, username, password, timeout);  
            }

            return _interaction_client;
        }

        public static CSD_REPAIRS_PUB_Service GetDepotInstance(string url, string username = null, string password = null, int timeout = -1)
        {
            if (_depot_client == null)
            {
                _depot_client = new CSD_REPAIRS_PUB_Service();
                serviceClientSetting(_depot_client, url, username, password, timeout);  
            }

            return _depot_client;
        }

        public static HZ_INTEGRATION_PUB_Service GetContactInstance(string url, string username = null, string password = null, int timeout = -1)
        {
            if (_contact_client == null)
            {
                _contact_client = new HZ_INTEGRATION_PUB_Service();
                serviceClientSetting(_contact_client, url, username, password, timeout);
            }
          
            return _contact_client;
        }


        public static CSI_ITEM_INSTANCE_PUB_Service GetItemInstance(string url, string username = null, string password = null, int timeout = -1)
        {
            if (_item_client != null)
            {
                return _item_client;
            }
            _item_client = new CSI_ITEM_INSTANCE_PUB_Service();
            _item_client.Timeout = timeout;
            serviceClientSetting(_item_client, url, username, password, timeout);
            return _item_client;
        }

        public static OKS_ENTITLEMENTS_PUB_Service GetEntitlementInstance(string url, string username = null, string password = null, int timeout = -1)
        {
            if (_entitlement_client != null)
            {
                return _entitlement_client;
            }
            _entitlement_client = new OKS_ENTITLEMENTS_PUB_Service();
            _entitlement_client.Timeout = timeout;
            serviceClientSetting(_entitlement_client, url, username, password, timeout);
            return _entitlement_client;
        }

        public static CSD_REPAIR_ORDERS_WEB_Service GetRepairOrderListInstance(string url, string username = null, string password = null, int timeout = -1)
        {
            if (_repair_order_list_client != null)
            {
                return _repair_order_list_client;
            }
            _repair_order_list_client = new CSD_REPAIR_ORDERS_WEB_Service();
            _repair_order_list_client.Timeout = timeout;
            serviceClientSetting(_repair_order_list_client, url, username, password, timeout);
            return _repair_order_list_client;
        }

        public static CSD_LOGISTICS_WEB_Service GetRepairLogisticsInstance(string url, string username = null, string password = null, int timeout = -1)
        {
            if (_repair_logistics_client != null)
            {
                return _repair_logistics_client;
            }
            _repair_logistics_client = new CSD_LOGISTICS_WEB_Service();
            _repair_logistics_client.Timeout = timeout;
            serviceClientSetting(_repair_logistics_client, url, username, password, timeout);
            return _repair_logistics_client;
        }

    }
}
