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
 *  date: Thu Nov 12 00:52:46 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: a309b990bc9f9fcdd91ae14271a079e522133a1b $
 * *********************************************************************************************
 *  File: ReportTable.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Accelerator.EBS.SharedServices;
using RightNow.AddIns.AddInViews;
using System.Reflection;

/*    class ReportTable comes with the sample code
 *    It has the table Description, Label, Name, 
 *    and the columns
 *    Base class for any virtual report table
 */
namespace Accelerator.EBS.ReportTablesAddin
{
    public abstract class ReportTable : IReportTable
    {
        protected const string TYPE_VALUE_DELIMITER = " ###  ";
        public ReportTable(IReportTablePackage package)
        {
            this.parent = package;
            this.columns = new List<IReportTableColumn>();        
        }

        private IReportTablePackage parent;
        public IReportTablePackage Parent
        {
            get
            {
                return this.parent;
            }
        }

        #region IReportTable Members

        private IList<IReportTableColumn> columns;
        public IList<IReportTableColumn> Columns
        {
            get { return this.columns; }
        }

        public string Description { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }

        public abstract IList<IReportRow> GetRows(IList<string> columns, IReportFilterNode node);

        // get the ebs_contact_party_id custom attribute on Contact Workspace
        protected int getContactPartyIdCustomAttr(IContact contactRecord)
        {
            IList<ICustomAttribute> customAttributes = contactRecord.CustomAttributes;
            foreach (ICustomAttribute cusAttr in customAttributes)
            {
                if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$ebs_contact_party_id")
                    return cusAttr.GenericField.DataValue.Value != null ? (int)cusAttr.GenericField.DataValue.Value : 0;
            }
            return 0;
        }

        // get the ebs_order_id custom attribute on Incident Workspace
        protected int getEbsOrderIdCustomAttr(IIncident incidentRecord)
        {
            IList<ICustomAttribute> customAttributes = incidentRecord.CustomAttributes;
            foreach (ICustomAttribute cusAttr in customAttributes)
            {
                if (cusAttr.PackageName == "Accelerator" && cusAttr.GenericField.Name == "Accelerator$ebs_order_id")
                    return cusAttr.GenericField.DataValue.Value != null ? (int)cusAttr.GenericField.DataValue.Value : 0;
            }
            return 0;
        }

        // add the columns dynamically for order management tables (can be backported for other tables)
        protected void addColumns(Type tableInfo)
        {
            foreach (PropertyInfo propertyInfo in tableInfo.GetProperties())
            {
                if (propertyInfo.PropertyType.Name == "Boolean" && propertyInfo.Name.EndsWith("Specified"))
                    continue;
                else
                {
                    ReportColumn reportCol = new ReportColumn();
                    reportCol.Name = propertyInfo.Name;
                    reportCol.Label = propertyInfo.Name;
                    reportCol.CanDisplay = true;
                    reportCol.CanFilter = true;

                    switch (propertyInfo.PropertyType.Name)
                    {
                        case "Nullable`1": // because of System.Nullable<generic>
                            string nullableType = propertyInfo.PropertyType.GetGenericArguments()[0].ToString();
                            if (nullableType == "System.Decimal")
                                reportCol.DataType = ReportColumnType.Integer;
                            else if (nullableType == "System.DateTime")
                                reportCol.DataType = ReportColumnType.DateTime;
                            break;
                        case "String":
                            reportCol.DataType = ReportColumnType.String;
                            break;
                        case "Boolean":
                            reportCol.DataType = ReportColumnType.Boolean;
                            break;
                    }
                    this.Columns.Add(reportCol);
                }
            }
        }

        // add the columns dynamically
        protected void addColumns(Dictionary<string, string> dictDetail)
        {
            foreach (KeyValuePair<string, string> entry in dictDetail)
            {
                ReportColumn reportCol = new ReportColumn();
                string type = (Regex.Split (entry.Value,TYPE_VALUE_DELIMITER))[0];
                reportCol.Name = entry.Key;
                reportCol.Label = entry.Key;
                reportCol.CanDisplay = true;
                reportCol.CanFilter = true;

                switch (type)
                {
                    case "Integer":
                        reportCol.DataType = ReportColumnType.Integer;
                        break;
                    case "String":
                        reportCol.DataType = ReportColumnType.String;
                        break;
                    case "DateTime":
                        reportCol.DataType = ReportColumnType.DateTime;
                        break;
                    case "Boolean":
                        reportCol.DataType = ReportColumnType.Boolean;
                        break;

                }
                this.Columns.Add(reportCol);
            }          
        }

        // add the columns dynamically
        protected void addColumns(Dictionary<string, ReportColumnType> dictDetail)
        {
            foreach (KeyValuePair<string, ReportColumnType> entry in dictDetail)
            {
                ReportColumn reportCol = new ReportColumn();
                reportCol.Name = entry.Key;
                reportCol.Label = entry.Key;
                reportCol.CanDisplay = true;
                reportCol.CanFilter = false;
                reportCol.DataType = entry.Value;
                this.Columns.Add(reportCol);
            }    
        }

        protected void addItem(ref IList<string> columns, ref ReportDataRow reportDataRow, ref  IList<IReportRow> reportRows, IReport item)
        {
            foreach (var column in columns)
            {
                ReportDataCell reportDataCell = new ReportDataCell();
                Tuple<ReportColumnType, object> field = item.getVirtualColumnValue(column.Replace(this.Parent.Name + "$" + this.Name + ".", ""));
                ReportColumnType type = field.Item1;
                switch (type)
                {
                    case ReportColumnType.Integer:
                        reportDataCell.GenericValue = Convert.ToInt32(field.Item2);
                        break;
                    case ReportColumnType.String:
                        reportDataCell.GenericValue = Convert.ToString(field.Item2);
                        break;
                    case ReportColumnType.DateTime:
                        reportDataCell.GenericValue = Convert.ToDateTime(field.Item2);
                        break;
                    case ReportColumnType.Boolean:
                        reportDataCell.GenericValue = Convert.ToBoolean(field.Item2);
                        break;
                }
                reportDataRow.Cells.Add(reportDataCell);
            }
            reportRows.Add(reportDataRow);
        }

        // used by SR and Contact Details, and null check before doing Convert.
        protected void addDetailRow(ref Dictionary<string, string> dictSRDetail, ref IList<string> columns, ref ReportDataRow reportDataRow, ref  IList<IReportRow> reportRows)
        {
            foreach (string column in columns)
            {
                ReportDataCell reportDataCell = new ReportDataCell();
                // put the "SRData$SRDetailTable." package name back, that's required by the report framework
                string pkgNtblName = "SRData$" + this.Name + ".";
                string removePkgTblName = column.Replace(pkgNtblName, "");
                string[] typeValArray = Regex.Split(dictSRDetail[removePkgTblName], TYPE_VALUE_DELIMITER);
                string type = typeValArray[0];
                string val = typeValArray[1];

                switch (type)
                {
                    case "String":
                        reportDataCell.GenericValue = val;
                        break;
                    case "Integer":
                        if (val != null && val != "")
                            reportDataCell.GenericValue = Convert.ToInt32(val);
                        else
                            reportDataCell.GenericValue = null;
                        break;
                    case "DateTime":
                        if (val != null && val != "")
                            reportDataCell.GenericValue = Convert.ToDateTime(val);
                        else
                            reportDataCell.GenericValue = null;
                        break;
                    case "Boolean":
                        if (val != null && val != "")
                            reportDataCell.GenericValue = Convert.ToBoolean(val);
                        else
                            reportDataCell.GenericValue = null;
                        break;
                }

                reportDataRow.Cells.Add(reportDataCell);
            }
            reportRows.Add(reportDataRow);
        }


        protected object getEqualsFilterValue(IReportFilterNode filterNode, string filter_name, bool is_mandatory)
        {
            string mandatory = is_mandatory ? "This is a required filter. " : "";
            string logMessage, logNote;
            if ((null == filterNode) || (null == filterNode.FilterNodes))
            {
                if (is_mandatory)
                {
                    logMessage = this.Name + " is missing the filter " + filter_name;
                    logNote = mandatory + "Alternatively, you may want to read filter values from the workspace.";
                    ConfigurationSetting.logWrap.DebugLog(logMessage: logMessage, logNote: logNote);
                }
                return null;
            }
            IReportFilterNode node = filterNode.FilterNodes.ToList<IReportFilterNode>()
                .Find(fn => fn.ReportFilter.Expression == string.Format("{0}${1}." + filter_name, this.Parent.Name, this.Name));
            if (null == node)
            {
                if (is_mandatory)
                {
                    logMessage = this.Name + " is missing the required filter " + filter_name;
                    logNote = mandatory + "Alternatively, you may want to read this value from the workspace.";
                    ConfigurationSetting.logWrap.DebugLog(logMessage: logMessage, logNote: logNote);
                }
                return null;
            }
            if (ReportFilterOperatorType.Equals != node.ReportFilter.OperatorType)
            {
                logMessage = this.Name + " requires an Equals operator on the filter " + filter_name;
                logNote = mandatory + Convert.ToString(node.ReportFilter.OperatorType) + " operator found on " 
                    + node.ReportFilter.Expression;
                ConfigurationSetting.logWrap.DebugLog(logMessage: logMessage, logNote: logNote);
                return null;
            }
            return node.ReportFilter.Value;
        }
        #endregion
    }
}
