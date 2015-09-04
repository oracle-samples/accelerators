/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + OFSC Reference Integration
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (Feb 2015)
 *  OFSC release: 15.2 (Feb 2015)
 *  reference: 150622-000130
 *  date: Mon Aug 24 09:01:22 PDT 2015

 *  revision: rnw-15-11-fixes-release-01
 *  SHA1: $Id: 66bdf5b02613ffb58e0813c6fa9002fb6e80462d $
 * *********************************************************************************************
 *  File: WorkOrderAreaControl.xaml.cs
 * ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;
using RightNow.AddIns.AddInViews;

namespace Oracle.RightNow.Toa.WorkOrderAreaAddIn
{
    /// <summary>
    /// Interaction logic for WorkOrderArea.xaml
    /// </summary>
    public partial class WorkOrderAreaControl : UserControl
    {
        private IRecordContext _recordContext;
        public WorkOrderAreaControl(IRecordContext _rcontext)
        {
            InitializeComponent();
            _recordContext = _rcontext;
        }

        /// <summary>
        /// Fetch Work Order Areas for a given zipcode
        /// </summary>
        /// <param name="_rcontext"></param>
        public void GetWorkOrderAreas(IRecordContext _rcontext)
        {
            ICustomObject record = _rcontext.GetWorkspaceRecord(_rcontext.WorkspaceTypeName) as ICustomObject;

            IList<IGenericField> fields = record.GenericFields;
            string postalCode = null;
            foreach (IGenericField field in fields)
            {

                if (field.Name == "Contact_Postal_Code")
                {
                    postalCode = field.DataValue.Value.ToString();
                    break;
                }
            }

            var WorkOrderAreaViewModel = new WorkOrderAreaViewModel();

            WorkOrderAreaViewModel.getWorkOrderArea(postalCode);
            DataContext = WorkOrderAreaViewModel;
        }

        /// <summary>
        /// WorkOrderAreaCombobox selection changed event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkOrderAreaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {           
            ICustomObject record = this._recordContext.GetWorkspaceRecord(this._recordContext.WorkspaceTypeName) as ICustomObject;

            IList<IGenericField> fields = record.GenericFields;
            foreach (IGenericField field in fields)
            {
                if (field.Name == "WO_Area")
                {
                    field.DataValue.Value = (string)WorkOrderAreaComboBox.SelectedItem;
                }
                _recordContext.RefreshWorkspace();
            }
        }
    }
}