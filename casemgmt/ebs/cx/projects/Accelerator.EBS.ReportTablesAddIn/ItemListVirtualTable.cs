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
 *  date: Thu Nov 12 00:52:45 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 0ef187120eff09e5f9ea494dec99799bb4f0cb69 $
 * *********************************************************************************************
 *  File: ItemListVirtualTable.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using Accelerator.EBS.SharedServices;
using RightNow.AddIns.AddInViews;
using RightNow.AddIns.Common;


namespace Accelerator.EBS.ReportTablesAddin 
{

    /// <summary>
    /// It shows the EBS item instances in a virtual table
    /// </summary>
    public class ItemListVirtualTable : ReportTable
    {
        public ItemListVirtualTable(EBSVirtualReportTablesPackage package)
            : base(package)
        {
                //add columns
            this.Name = "ItemTable";
            this.Label = "EBS Item Table";
            this.Description = "EBS Item Table";
            addColumns(Item.getDetailedSchema());
            
                //add filters
            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "HiddenSerialNumber",
                Name = "HiddenSerialNumber",
                CanDisplay = false,
                CanFilter = true
            });
            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.Integer,
                Label = "HiddenContactOrgID",
                Name = "HiddenContactOrgID",
                CanDisplay = false,
                CanFilter = true
            });
            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "HiddenActiveInstanceOnly",
                Name = "HiddenActiveInstanceOnly",
                CanDisplay = false,
                CanFilter = true
            });
        }

        /// <summary>
        /// Get rows for the report
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="filterNode"></param>
        public override IList<IReportRow> GetRows(IList<string> columns, IReportFilterNode filterNode)
        {
            IList<IReportRow> reportRows = new List<IReportRow>();
            int rntContactId = 0, rntIncidentId = 0;
            decimal contactOrgID = getContactOrgID(filterNode, out rntContactId);
            string serial_number = getSerialNumber(filterNode, rntContactId, out rntIncidentId);
            string active_instance_only = getActiveInstanceFlag(filterNode);
            Item[] items = Item.LookupItemList(serial_number, contactOrgID, active_instance_only,
                rntIncidentId, rntContactId);
            foreach (Item item in items)
            {
                ReportDataRow reportDataRow = new ReportDataRow(this.Columns.Count);
                if (item != null)
                    addItem(ref columns, ref reportDataRow, ref reportRows, item);
            }
            return reportRows;
        }

        private decimal getContactOrgID(IReportFilterNode filterNode, out int rntContactId)
        {
            IRecordContext _context = ((EBSVirtualReportTablesPackage)this.Parent)._globalContext.AutomationContext.CurrentWorkspace;
            if (null != _context)
            {
                WorkspaceRecordType workspaceType = _context.WorkspaceType;
                if (    (workspaceType == WorkspaceRecordType.Incident) ||
                        (workspaceType == WorkspaceRecordType.Contact))
                {
                    IContact contactRecord = _context.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Contact) as IContact;
                    if (null != contactRecord)
                    {
                        IList<ICustomAttribute> customAttributes = contactRecord.CustomAttributes;
                        rntContactId = contactRecord.ID;
                        object val = CustomAttrHelper.fetchCustomAttrValue(customAttributes,
                            new string[] { "Accelerator$ebs_contact_org_id" }, 0, rntContactId)
                            ["Accelerator$ebs_contact_org_id"];
                        if (null == val)
                            return 0;
                        else
                            return Convert.ToDecimal(val);
                    }
                }
            }
            decimal contact_org_id = 0;
            rntContactId = 0;
            object filter_value = getEqualsFilterValue(filterNode, "HiddenContactOrgID", true);
            if (null != filter_value)
            {
                contact_org_id = Convert.ToDecimal(filter_value);
            }
            if (contact_org_id > 0)
            {
                return contact_org_id;
            }
            string logMessage = "Provide a valid EBS Contact Org ID to search for products. Ignoring contact_org_id " 
                + contact_org_id;
            string logNote = null;
            ConfigurationSetting.logWrap.DebugLog(logMessage: logMessage, logNote: logNote);
            return 0;
        }

        private string getSerialNumber(IReportFilterNode filterNode, int rntContactId, out int rntIncidentId)
        {
            IRecordContext _context = ((EBSVirtualReportTablesPackage)this.Parent)._globalContext.AutomationContext.CurrentWorkspace;
            if (null != _context)
            {
                WorkspaceRecordType workspaceType = _context.WorkspaceType;
                if (workspaceType == WorkspaceRecordType.Incident)
                {
                    IIncident incidentRecord = _context.GetWorkspaceRecord(RightNow.AddIns.Common.WorkspaceRecordType.Incident) as IIncident;
                    if (null != incidentRecord)
                    {
                        IList<ICustomAttribute> customAttributes = incidentRecord.CustomAttributes;
                        rntIncidentId = incidentRecord.ID;
                        object val = CustomAttrHelper.fetchCustomAttrValue(customAttributes,
                            new string[] { "Accelerator$ebs_serial_number" }, rntIncidentId, rntContactId)
                            ["Accelerator$ebs_serial_number"];                  
                        return (null != val) ? val.ToString() : "";                        
                    }
                }
            }
            string serial_number = "";
            rntIncidentId = 0;
            object filter_value = getEqualsFilterValue(filterNode, "HiddenSerialNumber", true);
            if (null != filter_value)
            {
                serial_number = Convert.ToString(filter_value);
            }
            if (!String.IsNullOrWhiteSpace(serial_number))
            {
                return serial_number;
            }
            return "";
        }

        private string getActiveInstanceFlag(IReportFilterNode filterNode)
        {
            string flag = "";
            object filter_value = getEqualsFilterValue(filterNode, "HiddenActiveInstanceOnly", false);
            if (null != filter_value)
            {
                flag = Convert.ToString(filter_value);
            }
            if (!String.IsNullOrEmpty(flag))
            {
                return flag;
            }
            return "F";
        }
    }
}
