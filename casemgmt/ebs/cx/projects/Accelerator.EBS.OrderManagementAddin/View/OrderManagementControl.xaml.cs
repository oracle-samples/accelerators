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
 *  date: Thu Nov 12 00:52:43 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 60beed7d48296a7fb3256b0cff4ffba6f2ded754 $
 * *********************************************************************************************
 *  File: OrderManagementControl.xaml.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;

namespace Accelerator.EBS.OrderManagementAddin
{
    /// <summary>
    /// Interaction logic for OrderManagementControl.xaml
    /// </summary>
    public partial class OrderManagementControl : UserControl
    {
        internal OrderManagementViewModel _Model;

        public OrderManagementControl()
        {
            InitializeComponent();
            InitializeControl(new Proxy());
        }

        void webBrowser1_Loaded(object sender, RoutedEventArgs e)
        {
            webBrowser1.NavigateToString(Properties.Resources.OrderManagementReadMe);
        }

        public OrderManagementControl(Proxy proxy)
        {
            InitializeComponent();
            InitializeControl(proxy);
        }

        // initialize control
        private void InitializeControl(Proxy proxy)
        {
            _Model = new OrderManagementViewModel(proxy, this);
            _PreviousFile = _Model._DefaultFile;
            this.Loaded += MainWindow_Loaded;
            webBrowser1.Loaded += webBrowser1_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = _Model;
            if (_Model.IsRecordLoaded || _Model.InDesignMode || _Model.InDemo)
            {
                _Model.Refresh();
            }
            if (_Model.InDesignMode)
            {
                SalesOrderViewModel importFile = _Model._Proxy.GetDummyFile();
                importFile._ShowAll = false;
                repairOrderList.ItemsSource = importFile.Items;
                _Model.CurrentParent = importFile;
                importFile.IsNewItemEnabled = true;
                _Model.InDemo = true;
                importFile.SelectAll();
                _Model._Filter = "Search";
                _Model.IsRunning = true;
            }
            _Model._Proxy.MainWindowLoaded();
        }

        private void repairOrderList_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;
        }

        SalesOrderViewModel _PreviousFile;
        // handle file selection changed
        private void parentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView aList = e.OriginalSource as ListView;
            if (null == aList) return;
            SwitchFile(aList, e);
        }

        private void SwitchFile(ListView aList, RoutedEventArgs e)
        {
            SalesOrderViewModel importFile = aList.SelectedItem as SalesOrderViewModel;
            if (aList.SelectedIndex == -1) return;
            if (FileIsNotCurrent()) return;
            _Model.CurrentParent = importFile;
            repairOrderList.ItemsSource = importFile.Items;
            importFile._ShowAll = false;
            _Model.loadFile(importFile);
            if (null != e)
            {
                e.Handled = true;
            }
        }

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            if (null == _Model.CurrentParent)
                return;
            if (_Model.IsRunning)
            {
                System.Windows.MessageBox.Show("Please wait for the current operation to complete.",
                    string.Format("{0} Order Management", _Model.ServerType), MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            if (SalesOrderViewModel.JobStatus.Done != _Model.CurrentParent._JobStatus
                && SalesOrderViewModel.JobStatus.DoneError != _Model.CurrentParent._JobStatus)
                return;
            _Model.CurrentParent.Items.Clear();
            _Model.CurrentParent.Clean();
            var curr = _Model.CurrentParent;
            ResetGrid(true);
            _Model.ParentList.Remove(curr);
        }

        private void selectAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (FileIsNotCurrent()) return;
            _Model.CurrentParent.SelectAll();
        }

        private void repairOrderList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                updateSelected(e, sender);
        }

        private void repairOrderList_SelectedCellsChanged(object sender, MouseButtonEventArgs e)
        {
            updateSelected(e, sender);
        }

        private void updateSelected(RoutedEventArgs e, object sender)
        {
            e.Handled = true;
            return;
        }

        private void newItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (_Model.IsRunning)
            {
                System.Windows.MessageBox.Show("Please wait for the current operation to complete.",
                    string.Format("{0} Order Management", _Model.ServerType), MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            if (FileIsNotCurrent()) return;
            if (0 != _Model.CurrentParent.HeaderId) return;

            var win = new AddItemWindow();
            win._Model = _Model;
            win.DataContext = _Model;
            win.ShowDialog();
        }

        private void removeItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (_Model.IsRunning)
            {
                System.Windows.MessageBox.Show("Please wait for the current operation to complete.", 
                    string.Format("{0} Order Management", _Model.ServerType), MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            if (FileIsNotCurrent()) return;
            if (0 != _Model.CurrentParent.HeaderId) return;

            var item = repairOrderList.SelectedItem as SalesItemViewModel;
            _Model.CurrentParent.Items.Remove(item);
        }


        private bool FileIsNotCurrent()
        {
            if (null == _Model.CurrentParent) return true;
            if ((Convert.ToString(_Model.CurrentParent.FileId) != fileId.Text)) return true;
            return false;
        }

        private void showAllRowsButton_Click(object sender, RoutedEventArgs e)
        {
            if (null == _Model.CurrentParent)
                return;
            _Model.CurrentParent._ShowAll = true;
            _Model.loadFile(_Model.CurrentParent);
        }

        private void fileScroller_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //fileScroller.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            ScrollViewer sc = sender as ScrollViewer;
            if (e.Delta > 0)
            {
                sc.LineUp();
            }
            else
            {
                sc.LineDown();
            }
            e.Handled = true;
        }

        private void repairOrderList_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var name = GetColumnName(e.PropertyDescriptor);
            if (null == name) e.Column.Visibility = System.Windows.Visibility.Hidden;
            if (!string.IsNullOrWhiteSpace(name))
            {
                e.Column.Header = name;
                e.Column.MaxWidth = 400;
            }
        }

        // get column names for display
        private string GetColumnName(object p)
        {
            var desc = p as PropertyDescriptor;
            if (null != desc)
            {
                var name = desc.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;
                if (null != name && DisplayNameAttribute.Default != name)
                {
                    return name.DisplayName;
                }
            }
            else
            {
                var pinfo = p as PropertyInfo;
                if (null != pinfo)
                {
                    Object[] attrs = pinfo.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    for (int i = 0; i < attrs.Length; ++i)
                    {
                        var name = attrs[i] as DisplayNameAttribute;
                        if (null != name && DisplayNameAttribute.Default != name)
                        {
                            return name.DisplayName;
                        }
                    }
                }
            }
            return null;
        }

        internal void ResetGrid(bool setDefault, int index = 0)
        {
            if (setDefault)
            {
                _Model.CurrentParent = _Model._DefaultFile;
            }
            parentList.SelectedIndex = index;
            repairOrderList.ItemsSource = _Model.CurrentParent.Items;
        }

        private void demoButton_Click(object sender, RoutedEventArgs e)
        {
            if (0 == _Model.EbsSrId || 0 == _Model.EbsOwnerId)
            {
                _Model.InDemo = true;
                e.Handled = true;
                System.Windows.MessageBox.Show("Cannot switch to Live mode. Either this incident is not associated with an EBS Service Request or custom config ebs_default_sr_owner_id is missing.",
                    string.Format("{0} Order Management", _Model.ServerType), MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            if (_Model.IsRunning)
            {
                _Model.InDemo = !_Model.InDemo;
                e.Handled = true;
                System.Windows.MessageBox.Show("Wait for the current job to complete.", string.Format("{0} Order Management", _Model.ServerType), MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            if (!_Model.InDemo)
            {
                _Model.InDemo = false;
                _Model.Refresh();
                e.Handled = true;
                return;
            }
            if (_Model.IsIncomplete)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Some orders are partially imported. Do you want to leave?",
                    string.Format("{0} Order Management", _Model.ServerType), MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (MessageBoxResult.No == result)
                {
                    _Model.InDemo = !_Model.InDemo;
                    e.Handled = true;
                    return;
                }
            }
            else
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("You will lose all changes. Do you still want to leave?",
                    string.Format("{0} Order Management", _Model.ServerType), MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (MessageBoxResult.No == result)
                {
                    _Model.InDemo = !_Model.InDemo;
                    e.Handled = true;
                    return;
                }
            }
            _Model.Refresh();
        }

        private void newParentButton_Click(object sender, RoutedEventArgs e)
        {
            if (_Model.IsRunning)
            {
                System.Windows.MessageBox.Show("Please wait for the current operation to complete.",
                    string.Format("{0} Order Management", _Model.ServerType), MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            _Model.Filter = "";
            SalesOrderViewModel order = _Model.StartNewOrder();
            _Model.ParentList.Add(order);
            _Model.CurrentParent = order;
            parentList.SelectedItem = order;
            parentList.SelectedIndex = _Model.ParentList.Count - 1;
            ResetGrid(false, index:parentList.SelectedIndex);
        }
    }

    public class GAnimation : AnimationTimeline
    {
        protected override Freezable CreateInstanceCore()
        {
            return new GAnimation();
        }

        public override Type TargetPropertyType
        {
            get
            {
                return typeof(GridLength);
            }
        }

        public static DependencyProperty FromProp;
        public GridLength From
        {
            get
            {
                return (GridLength)GetValue(GAnimation.FromProp);
            }
            set
            {
                SetValue(GAnimation.FromProp, value);
            }
        }

        public static DependencyProperty ToProp;
        public GridLength To
        {
            get
            {
                return (GridLength)GetValue(GAnimation.ToProp);
            }
            set
            {
                SetValue(GAnimation.ToProp, value);
            }
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock clock)
        {
            double from = ((GridLength)GetValue(GAnimation.FromProp)).Value;
            double to = ((GridLength)GetValue(GAnimation.ToProp)).Value;
            if (from > to)
            {
                return new GridLength((from - to) * (1 - clock.CurrentProgress.Value) + to, GridUnitType.Pixel);
            }
            else
                return new GridLength((to - from) * clock.CurrentProgress.Value + from, GridUnitType.Pixel);
        }

        static GAnimation()
        {
            ToProp = DependencyProperty.Register("To", typeof(GridLength), typeof(GAnimation));
            FromProp = DependencyProperty.Register("From", typeof(GridLength), typeof(GAnimation));
        }

    }

}
