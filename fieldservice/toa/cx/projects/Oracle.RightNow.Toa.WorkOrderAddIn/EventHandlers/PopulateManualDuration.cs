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
 *  SHA1: $Id: 85ba8ee97f73cae64f5fe0275866ff8980aee188 $
 * *********************************************************************************************
 *  File: PopulateManualDuration.cs
 * ****************************************************************************************** */

using Oracle.RightNow.Toa.Client.Common;
using Oracle.RightNow.Toa.Client.Logs;
using Oracle.RightNow.Toa.Client.Rightnow;
using RightNow.AddIns.AddInViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Oracle.RightNow.Toa.WorkOrderAddIn.EventHandlers
{
    public class PopulateManualDuration : IHandler
    {

        private IRecordContext _recordContext;
        private Oracle.RightNow.Toa.Client.Logs.IToaLog _log;
        ICustomObject _workOrderRecord { get; set; }

        public PopulateManualDuration(IRecordContext RecordContext)
        {
            _recordContext = RecordContext;
            _workOrderRecord = _recordContext.GetWorkspaceRecord(_recordContext.WorkspaceTypeName) as ICustomObject;
            _log = ToaLogService.GetLog();
        }

        public void Handler()
        {
            try
            {
                string duration = null;
                bool manual_duration;
                IGenericField durationField = null;
                IList<IGenericField> fields = _workOrderRecord.GenericFields;
                foreach (IGenericField field in fields)
                {
                    switch (field.Name)
                    {
                        case "Duration":
                            durationField = field;
                            break;
                        case "WO_Type":
                            if (field.DataValue.Value == null) break;
                            int workorderTypeId = (Int32)field.DataValue.Value;
                            string[] workordertype = RightNowConnectService.GetService().GetWorkOrderTypeFromID(workorderTypeId);
                            manual_duration = (workordertype[1].Equals("1")) ? true : false;
                            if (manual_duration)
                            {
                                duration = workordertype[2];
                            }
                            break;
                    }
                }

                if (duration != null)
                {
                    durationField.DataValue.Value = duration;
                }
                else
                {
                    durationField.DataValue.Value = null;
                }
                _recordContext.RefreshWorkspace();

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex.StackTrace);
                MessageBox.Show(ToaExceptionMessages.UnexpectedError, ToaExceptionMessages.TitleError,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
    }
}
