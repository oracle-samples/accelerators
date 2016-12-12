/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + OFSC Reference Integration
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (Feb 2015)
 *  OFSC release: 15.2 (Feb 2015)
 *  reference: 150622-000130
 *  date: Thu Sep  3 23:14:04 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
 *  SHA1: $Id: eb41148d6af7e97f2495000ab681b94cb3dcab54 $
 * *********************************************************************************************
 *  File: PopulateContactDetailsHandler.cs
 * ****************************************************************************************** */

using Oracle.RightNow.Toa.Client.Common;
using Oracle.RightNow.Toa.Client.Rightnow;
using Oracle.RightNow.Toa.Client.RightNowProxyService;
using RightNow.AddIns.AddInViews;
using RightNow.AddIns.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Oracle.RightNow.Toa.WorkOrderAddIn.EventHandlers
{
    public class PopulateContactDetailsHandler
    {
        /// <summary>
        /// The current workspace record context.
        /// </summary>
        private IRecordContext _recordContext;

        ICustomObject _workOrderRecord { get; set; }

        public PopulateContactDetailsHandler(IRecordContext RecordContext)
        {
            _recordContext = RecordContext;
            _workOrderRecord = _recordContext.GetWorkspaceRecord(_recordContext.WorkspaceTypeName) as ICustomObject;
        }

        public void PopulateContactDetails()
        {
            IContact contactRecord = _recordContext.GetWorkspaceRecord(WorkspaceRecordType.Contact) as IContact;
            IIncident incidentRecord = _recordContext.GetWorkspaceRecord(WorkspaceRecordType.Incident) as IIncident;
            
            IList<IGenericField> fields = _workOrderRecord.GenericFields;

            foreach (IGenericField field in fields)
            {
                switch (field.Name)
                {

                    case "Contact_Street":
                        if (contactRecord != null && contactRecord.AddrStreet != null)
                        {
                            field.DataValue.Value = contactRecord.AddrStreet;
                        }
                        break;
                    case "Contact_Province_State":
                        if (contactRecord != null && contactRecord.AddrProvID != null)
                        {
                            field.DataValue.Value = contactRecord.AddrProvID;
                        }
                        break;
                    case "Contact_City":
                        if (contactRecord != null && contactRecord.AddrCity != null)
                        {
                            field.DataValue.Value = contactRecord.AddrCity;
                        }
                        break;
                    case "Contact_Postal_Code":
                        if (contactRecord != null && contactRecord.AddrPostalCode != null)
                        {
                            field.DataValue.Value = contactRecord.AddrPostalCode;
                        }
                        break;
                    case "Contact_Email":
                        if (contactRecord != null && contactRecord.EmailAddr != null)
                        {
                            field.DataValue.Value = contactRecord.EmailAddr;
                        }
                        break;
                    case "Contact_Phone":
                        if (contactRecord != null && contactRecord.PhHome != null)
                        {
                            field.DataValue.Value = contactRecord.PhHome;
                        }
                        else if (contactRecord != null && contactRecord.PhOffice != null)
                        {
                            field.DataValue.Value = contactRecord.PhOffice;
                        }
                        break;
                    case "Contact_Mobile_Phone":
                        if (contactRecord != null && contactRecord.PhMobile != null)
                        {
                            field.DataValue.Value = contactRecord.PhMobile;
                        }
                        break;
                    case "Resolution_Due":
                        string[] milestones = RightNowConnectService.GetService().GetResolutionDueFromID(incidentRecord.ID);
                        if(milestones != null && milestones.Count() > 0)
                        {
                            field.DataValue.Value = milestones[0];
                        }
                        break;
                }
                _recordContext.RefreshWorkspace();
            }
        }

    }
}
