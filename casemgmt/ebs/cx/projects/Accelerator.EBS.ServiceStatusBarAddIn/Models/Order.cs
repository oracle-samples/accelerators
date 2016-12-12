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
 *  date: Thu Nov 12 00:52:48 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: e40e85de92ffd70ad8844577d6cf4516903daa73 $
 * *********************************************************************************************
 *  File: Order.cs
 * *********************************************************************************************/

using Accelerator.EBS.SharedServices.Providers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accelerator.EBS.SharedServices.ProxyClasses.OrderMgmt;
using PROCESS_ORDER = Accelerator.EBS.SharedServices.ProxyClasses.OrderMgmtInbound;
using ORDERS = Accelerator.EBS.SharedServices.ProxyClasses.OrdersByContact;

using ORDER_HEADER = Accelerator.EBS.SharedServices.ProxyClasses.OrdersByContact.APPSOE_ORDER_CUST_ORX3349377X2X3;
using ITEM_HEADER = Accelerator.EBS.SharedServices.ProxyClasses.OrderMgmt.APPSOE_ORDER_PUB_LX219471X29X751;
using ITEM_VALUE = Accelerator.EBS.SharedServices.ProxyClasses.OrderMgmt.APPSOE_ORDER_PUB_X219471X29X1188;
using System.Diagnostics;

namespace Accelerator.EBS.SharedServices
{
    public class Order : ModelObjectBase
    {
        public static string GetOrderURL { get; set; }
        public static string OrderInboundURL { get; set; }

        private static IEBSProvider _provider = null;

        public static OutputParameters2 LookupOrder(decimal order_id, int _logIncidentId = 0, int _logContactId = 0)
        {
            return Order._provider.LookupOrderDetail(order_id, _logIncidentId, _logContactId);
        }

        public static Order GetOrderDetails(decimal orderId, int incidentId, int contactId)
        {
            Order result;
            try
            {
                result =  new Order(LookupOrder(orderId, incidentId, contactId));
            }
            catch
            {
                result = new Order();
            }
            return result;
        }

        public static List<Order> GetOrdersByIncident(int incidentId, int contactId)
        {   
            var result = new List<Order>();
            try
            {
                var op = Order.LookupOrdersByIncident(incidentId, incidentId, contactId);
                foreach (var oh in op.X_ORDERS)
                {
                    result.Add(new Order(oh));
                }
            }
            catch
            {
                // ignore
            }
            return result;
        }

        public static ORDERS.OutputParameters LookupOrdersByContact(decimal contact_id, int _logIncidentId = 0, int _logContactId = 0)
        {
            return Order._provider.LookupOrdersByContact(contact_id, _logIncidentId, _logContactId);
        }

        public static ORDERS.OutputParameters1 LookupOrdersByIncident(decimal incident_id, int _logIncidentId = 0, int _logContactId = 0)
        {
            return Order._provider.LookupOrdersByIncident(incident_id, _logIncidentId, _logContactId);
        }

        public static void InitEBSProvider()
        {
            Type t = Type.GetType(ServiceProvider);

            try
            {
                _provider = Activator.CreateInstance(t) as IEBSProvider;
                _provider.InitForOrder(GetOrderURL, OrderInboundURL);
                _provider.log = ConfigurationSetting.logWrap;
            }
            catch (Exception ex)
            {
                if (ConfigurationSetting.logWrap != null)
                {
                    string logMessage = "Error in init Provider in Order Model. Error: " + ex.Message;
                    string logNote = "";
                    ConfigurationSetting.logWrap.ErrorLog(logMessage: logMessage, logNote: logNote);
                }
                throw;
            }
        }

        public string ATTRIBUTE15;
        public string ATTRIBUTE16;
        public decimal? HEADER_ID;
        public string FLOW_STATUS_CODE;
        public DateTime? LAST_UPDATE_DATE;
        public DateTime? CREATION_DATE;
        public List<OrderItem> Items;
        public int ContactId;
        public int IncidentId;

        public Order()
        {
            Items = new List<OrderItem>();
        }

        public Order(ORDER_HEADER order)
        {
            ATTRIBUTE15 = order.ATTRIBUTE15;
            ATTRIBUTE16 = order.ATTRIBUTE16;
            HEADER_ID = order.HEADER_ID;
            FLOW_STATUS_CODE = order.FLOW_STATUS_CODE;
            LAST_UPDATE_DATE = order.LAST_UPDATE_DATE;
            CREATION_DATE = order.CREATION_DATE;
            Items = new List<OrderItem>();
        }

        public Order(OutputParameters2 order)
        {
            Items = new List<OrderItem>();
            var keys = order.X_LINE_TBL;
            var vals = order.X_LINE_VAL_TBL;
            for (int i = 0; i < keys.Length && i < vals.Length; i++)
            {
                Items.Add(new OrderItem(keys[i], vals[i]));
            }
        }

        public Order Save()
        {
            string request = "";
            string response = "";
            string logMessage, logNote;
            var provider = _provider as LiveEBSProvider;
            if (String.IsNullOrWhiteSpace(provider.OrderInboundURL))
            {
                throw new Exception("Provider's InitForOrder not run.");
            }

            PROCESS_ORDER.OE_INBOUND_INT_Service client = EBSProxyFactory.GetOrderInboundServiceInstance(provider.OrderInboundURL, ConfigurationSetting.username, ConfigurationSetting.password, ConfigurationSetting.EBSServiceTimeout);
            PROCESS_ORDER.SOAHeader hdr = new PROCESS_ORDER.SOAHeader();
            hdr.Responsibility = "ORDER_MGMT_SUPER_USER";
            hdr.RespApplication = "ONT";
            hdr.Org_Id = "204";
            hdr.SecurityGroup = "STANDARD";
            hdr.NLSLanguage = "AMERICAN";

            client.SOAHeaderValue = hdr;

            var ip = new PROCESS_ORDER.InputParameters();

            ip.P_API_VERSION_NUMBER = 1;
            ip.P_API_VERSION_NUMBERSpecified = true;
            ip.P_INIT_MSG_LIST = "T";
            ip.P_RETURN_VALUES = "T";
            ip.P_ACTION_COMMIT = "T";
            ip.P_RTRIM_DATA = "T";

            ip.P_HEADER_REC = new PROCESS_ORDER.APPSOE_ORDER_PUB_HEADER_REC_TYPE();
            ip.P_HEADER_REC.ATTRIBUTE15 = ATTRIBUTE15;
            ip.P_HEADER_REC.BOOKED_FLAG = "N";
            ip.P_HEADER_REC.ORDER_TYPE_ID = 1430;
            ip.P_HEADER_REC.ORDER_TYPE_IDSpecified = true;
            ip.P_HEADER_REC.OPERATION = "CREATE";

            ip.P_LINE_TBL = new PROCESS_ORDER.APPSOE_ORDER_PUB_LINE_REC_TYPE[Items.Count];
            var items = Items.ToArray();
            for (int i = 0; i < Items.Count; i++)
            {
                var src = items[i];
                var dest = ip.P_LINE_TBL[i] = new PROCESS_ORDER.APPSOE_ORDER_PUB_LINE_REC_TYPE();
                dest.INVENTORY_ITEM_ID = src.INVENTORY_ITEM_ID;
                dest.INVENTORY_ITEM_IDSpecified = true;
                dest.ORDERED_QUANTITY = src.ORDERED_QUANTITY;
                dest.ORDERED_QUANTITYSpecified = true;
                dest.UNIT_SELLING_PRICE = src.UNIT_SELLING_PRICE;
                dest.UNIT_SELLING_PRICESpecified = true;
                dest.OPERATION = "CREATE";
            }
            PROCESS_ORDER.OutputParameters op = null;
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                var serializer = LiveEBSProvider.serializer;
                request = serializer.Serialize(ip);

                logMessage = "Request of saving Order (PROCESS_ORDER). ";
                logNote = "Request Payload: " + request;
                provider.log.DebugLog(IncidentId, ContactId, logMessage, logNote);
                // call the web service, catch the exception right away
                stopwatch.Start();
                op = client.PROCESS_ORDER(ip);
                stopwatch.Stop();
                response = serializer.Serialize(op);
                HEADER_ID = op.X_HEADER_REC.HEADER_ID;
                CREATION_DATE = op.X_HEADER_REC.CREATION_DATE;
                LAST_UPDATE_DATE = op.X_HEADER_REC.LAST_UPDATE_DATE;
                FLOW_STATUS_CODE = op.X_HEADER_REC.FLOW_STATUS_CODE;
            }
            catch (Exception ex)
            {
                provider.handleEBSException(ex, "PROCESS_ORDER", IncidentId, ContactId);
                // will throw the new exception (either timeout or error communicating ...)
                throw;
            }

            if (op.X_RETURN_STATUS == "S")
            {
                logMessage = "Response of saving Order(PROCESS_ORDER). ";
                logNote = "Response Payload: " + response;
                provider.log.DebugLog(IncidentId, ContactId, logMessage, logNote, (int)stopwatch.ElapsedMilliseconds);
            }
            else
            {
                logMessage = "Response of saving Order (PROCESS_ORDER (Failure). ";
                logNote = "Response Payload: " + response;
                provider.log.ErrorLog(IncidentId, ContactId, logMessage, logNote);
            }
            return this;
        }
    }

    public class OrderItem : ModelObjectBase
    {
        public decimal? LINE_ID;
        public string INVENTORY_ITEM;
        public decimal? ORDERED_QUANTITY;
        public decimal? UNIT_SELLING_PRICE;
        public decimal? INVENTORY_ITEM_ID;

        public OrderItem()
        {
        }

        public OrderItem(ITEM_HEADER header, ITEM_VALUE value)
        {
            LINE_ID = header.LINE_ID;
            INVENTORY_ITEM = value.INVENTORY_ITEM;
            ORDERED_QUANTITY = header.ORDERED_QUANTITY;
            UNIT_SELLING_PRICE = header.UNIT_SELLING_PRICE;
            INVENTORY_ITEM_ID = header.INVENTORY_ITEM_ID;
        }
    }
}
