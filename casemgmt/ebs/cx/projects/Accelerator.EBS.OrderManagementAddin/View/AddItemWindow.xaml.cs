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
 *  date: Thu Nov 12 00:52:43 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 7b468eea8f7749fc32f2d596518d8191c7b8560f $
 * *********************************************************************************************
 *  File: AddItemWindow.xaml.cs
 * *********************************************************************************************/

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
using System.Windows.Shapes;

namespace Accelerator.EBS.OrderManagementAddin
{
    /// <summary>
    /// Interaction logic for AddItemWindow.xaml
    /// </summary>
    public partial class AddItemWindow : Window
    {
        public OrderManagementViewModel _Model;

        public AddItemWindow()
        {
            InitializeComponent();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            var selected = _Model.CurrentParent.SelectedItem;
            if (null == selected) return;
            var item = new SalesItemViewModel(selected);
            if (IsValid())
            {
                _Model.CurrentParent.Items.Add(item);
                selected.Quantity = 1;
            }
        }

        private bool IsValid()
        {
            decimal? qty;
            try
            {
                qty = Convert.ToDecimal(quantity.Text);
                //price = Convert.ToDecimal(unitPrice.Text);
            }
            catch
            {
                return false;
            }
            if (0 >= qty)
                return false;
            return true;
        }
    }
}
