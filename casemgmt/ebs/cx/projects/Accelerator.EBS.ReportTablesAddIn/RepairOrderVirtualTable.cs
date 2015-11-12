/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:45 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: eeb2c4c55ab6cfcb7e00fd09ae9aad247915357b $
 * *********************************************************************************************
 *  File: RepairOrderVirtualTable.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using Accelerator.EBS.SharedServices;
using RightNow.AddIns.AddInViews;
using RightNow.AddIns.Common;

namespace Accelerator.EBS.ReportTablesAddin 
{
    /// <summary>
    /// It shows EBS Repair Orders in a virtual table
    /// </summary>
    public class RepairOrderVirtualTable : ReportTable
    {
        public RepairOrderVirtualTable(EBSVirtualReportTablesPackage package)
            : base(package)
        {
                //add columns
            this.Name = "RepairOrderTable";
            this.Label = "EBS Repair Order Table";
            this.Description = "EBS Repair Order Table";
            addColumns(RepairOrder.getRepairOrderListSchema());
                //add filters
            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.Integer,
                Label = "HiddenIncidentID",
                Name = "HiddenIncidentID",
                CanDisplay = false,
                CanFilter = true
            });
            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.Integer,
                Label = "HiddenContactID",
                Name = "HiddenContactID",
                CanDisplay = false,
                CanFilter = true
            });
            this.Columns.Add(new ReportColumn()
            {
                DataType = ReportColumnType.String,
                Label = "SR_DETAIL_LINK",
                Name = "SR_DETAIL_LINK",
                CanDisplay = true,
                CanFilter = false
            });
        }

        /// <summary>
        /// Gets rows for the report
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="filterNode"></param>
        public override IList<IReportRow> GetRows(IList<string> columns, IReportFilterNode filterNode)
        {
            IList<IReportRow> reportRows = new List<IReportRow>();
            int rntContactId = 0, rntIncidentId = 0;
            decimal contact_id = getContactID(filterNode, out rntContactId);
                //check if ebs contact id is present
            switch (String.Format("{0:0}", contact_id))
            {
                case "-1":
                    return reportRows;
            }
                //get ebs incident id
            decimal incident_id = getIncidentID(filterNode, rntContactId, out rntIncidentId);
            RepairOrder[] items = RepairOrder.LookupRepairOrderList(contact_id, incident_id, rntIncidentId, rntContactId);
            foreach (RepairOrder item in items)
            {
                ReportDataRow reportDataRow = new ReportDataRow(this.Columns.Count);
                if (item != null) 
                        addItem(ref columns, ref reportDataRow, ref reportRows, item);
            }            
            return reportRows;
        }

        private decimal getContactID(IReportFilterNode filterNode, out int rntContactId)
        {
            IRecordContext _context = ((EBSVirtualReportTablesPackage)this.Parent)._globalContext.AutomationContext.CurrentWorkspace;
            string logMessage;
            string logNote;
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
                            new string[] { "Accelerator$ebs_contact_party_id" }, 0, rntContactId)
                            ["Accelerator$ebs_contact_party_id"];
                        if (null == val)
                        {
                            logMessage = "Provide an EBS Contact ID to search for Repair Orders. No search performed.";
                            logNote = null;
                            ConfigurationSetting.logWrap.DebugLog(0, rntContactId, logMessage, logNote);
                            return -1;
                        }
                        else if (Convert.ToDecimal(val) <= 0)
                        {
                            logMessage = "Provide a valid EBS Contact ID to search for Repair Orders. No search performed for ebs_contact_party_id " + val;
                            logNote = null;
                            ConfigurationSetting.logWrap.DebugLog(0, rntContactId, logMessage, logNote);
                            return -1;
                        }
                        else
                            return Convert.ToDecimal(val);

                    }
                }
            }
            rntContactId = 0;
            decimal contact_id = 0;
            object filter_value = getEqualsFilterValue(filterNode, "HiddenContactID", true);
            if (null != filter_value)
            {
                contact_id = Convert.ToDecimal(filter_value);
            }
            if (contact_id > 0)
            {
                return contact_id;
            }
            logMessage = "Provide a valid EBS Contact ID to search for Repair Orders. Ignoring ebs_contact_party_id " 
                + contact_id;
            logNote = null;
            ConfigurationSetting.logWrap.DebugLog(logMessage: logMessage, logNote: logNote);
            return 0;
        }

        private decimal getIncidentID(IReportFilterNode filterNode, int rntContactId, out int rntIncidentId)
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
                            new string[] { "Accelerator$ebs_sr_id" }, rntIncidentId, rntContactId)
                            ["Accelerator$ebs_sr_id"];
                        return (null != val) ? Convert.ToDecimal(val) : 0;
                    }
                }
            }
            decimal incident_id = 0;
            rntIncidentId = 0;
            object filter_value = getEqualsFilterValue(filterNode, "HiddenIncidentID", false);
            if (null != filter_value)
            {
                incident_id = Convert.ToDecimal(filter_value);
            }            
            if (incident_id > 0)
            {
                return incident_id;
            }
            string logMessage = "Provide a valid EBS Incident ID to search for Repair Orders. Ignoring ebs_sr_id " + incident_id;
            string logNote = null;
            ConfigurationSetting.logWrap.DebugLog(rntIncidentId, 0, logMessage, logNote);
            return 0;
        }
    }
}