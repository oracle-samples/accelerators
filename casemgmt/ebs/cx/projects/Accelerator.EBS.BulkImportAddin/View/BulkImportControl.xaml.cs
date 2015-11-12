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
 *  date: Thu Nov 12 00:52:42 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: 10a1d0f9eaa77bdf93d1192ab5b9fa853442f466 $
 * *********************************************************************************************
 *  File: BulkImportControl.xaml.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Accelerator.EBS.BulkImportAddin
{
    /// <summary>
    /// Interaction logic for BulkImportControl.xaml
    /// </summary>
    public partial class BulkImportControl : UserControl
    {
        internal BulkImportViewModel _Model;
        private ImportFile _DefaultFile;

        public BulkImportControl()
        {
            InitializeComponent();
            InitializeControl(new Proxy());
        }

        public BulkImportControl(Proxy proxy)
        {
            InitializeComponent();
            InitializeControl(proxy);
        }

        // initialize control
        private void InitializeControl(Proxy proxy)
        {
            _Model = new BulkImportViewModel(proxy, this);
            _DefaultFile = new ImportFile(_Model, 0, string.Empty, string.Empty, string.Empty);
            _LastSelectionTime = new Stopwatch();
            _LastSelectionTime.Restart();
            _PreviousIndex = -1;
            _PreviousFile = _DefaultFile;
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = _Model;
            if (_Model.IsRecordLoaded || _Model.InDesignMode)
            {
                _Model.Render(true);
            }
            if (_Model.InDesignMode)
            {
                importFileList.SelectedIndex = 1;
                ImportFile importFile = _Model._Proxy.GetDummyFile();
                importFile._ShowAll = false;
                repairOrderList.ItemsSource = importFile.RepairOrders;
                _Model.CurrentFile = importFile;
                importFile.IsImportEnabled = true;
                _Model.InDemo = true;
                importFile.SelectAll();
                _Model._Filter = "Search";
                fileFilter.Foreground = Brushes.Gray;
                _Model.IsRunning = true;
            }
        }

        private void repairOrderList_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;
        }

        Stopwatch _LastSelectionTime;
        int _PreviousIndex = -1;
        ImportFile _PreviousFile;
        // handle file selection changed
        private void importFileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView aList = e.OriginalSource as ListView;
            if (null == aList) return;
            SwitchFile(aList, e);
        }

        private void SwitchFile(ListView aList, RoutedEventArgs e)
        {
            ImportFile importFile = aList.SelectedItem as ImportFile;
            if (aList.SelectedIndex == -1) return;
            if (FileIsNotCurrent()) return;
            if (_LastSelectionTime.ElapsedMilliseconds < 400)
            {
                aList.SelectedIndex = _PreviousIndex;
                _Model.CurrentFile = _PreviousFile;
                importFileList.SelectedItem = _PreviousFile;
                repairOrderList.ItemsSource = _PreviousFile.RepairOrders;
            }
            else
            {
                _PreviousIndex = aList.SelectedIndex;
                _PreviousFile = _Model.CurrentFile;
                _Model.CurrentFile = importFile;
                importFileList.SelectedItem = importFile;
                repairOrderList.ItemsSource = importFile.RepairOrders;
                importFile._ShowAll = false;
                _Model.loadFile(importFile);
                _LastSelectionTime.Restart();
            }
            e.Handled = true;
        }

        private void Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            if (null == _Model.CurrentFile)
                return;
            _Model.ImportFileList.Remove(_Model.CurrentFile);
            _Model.FileList.Refresh();
            _Model.CurrentFile.RepairOrders.Clear();
            _Model.CurrentFile.Clean();
            _Model.CurrentFile = null;
        }

        private void selectAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (FileIsNotCurrent()) return;
            _Model.CurrentFile.SelectAll();
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
            if (null == _Model.CurrentFile) return;
            DataGrid dg = sender as DataGrid;
            if (null == dg) return;
            if (!_Model.CurrentFile.IsImportEnabled) return;
            var col = dg.CurrentCell.Column;
            if (null == col) return;
            if (!((string)col.Header == "Selected")) return;
            RepairOrder item = dg.CurrentItem as RepairOrder;
            if (null == item) return;
            if (RepairOrder.ImportStatus.New != item.Status) return;
            item.Selected = !item.Selected;
            e.Handled = true;
        }

        private void importButton_Click(object sender, RoutedEventArgs e)
        {
            if (_Model.IsRunning)
            {
                System.Windows.MessageBox.Show("Wait for the current job to complete.", "EBS Bulk Import", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            if (FileIsNotCurrent()) return;
            _Model.CurrentFile.Import();
        }

        private bool FileIsNotCurrent()
        {
            if (null == _Model.CurrentFile) return true;
            if ((Convert.ToString(_Model.CurrentFile.FileId) != fileId.Text)) return true;
            return false;
        }

        private void showAllRowsButton_Click(object sender, RoutedEventArgs e)
        {
            if (null == _Model.CurrentFile)
                return;
            _Model.CurrentFile._ShowAll = true;
            _Model.loadFile(_Model.CurrentFile);
        }

        private void fileScroller_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            fileScroller.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
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
            if (!string.IsNullOrWhiteSpace(name))
                e.Column.Header = name;
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

        internal void ResetGrid()
        {
            importFileList.SelectedIndex = -1;
            _Model.CurrentFile = _DefaultFile;
            importFileList.SelectedItem = _Model.CurrentFile;
            repairOrderList.ItemsSource = _Model.CurrentFile.RepairOrders;
        }

        private void removeRowButton_Click(object sender, RoutedEventArgs e)
        {
            if (FileIsNotCurrent()) return;
            if (_Model.CurrentFile.RemoveSelectedRows())
            {
                Storyboard sb = FindResource("removeButtonStoryboard") as Storyboard;
                Storyboard.SetTarget(sb, removeButtonImage);
                sb.Begin();
            }
        }

        private void demoButton_Click(object sender, RoutedEventArgs e)
        {
            if (0 == _Model.EbsSrId || 0 == _Model.EbsOwnerId)
            {
                _Model.InDemo = true;
                e.Handled = true;
                System.Windows.MessageBox.Show("Cannot switch to Live mode. Either this incident is not associated with an EBS Service Request or custom config ebs_default_sr_owner_id is missing.", "EBS Bulk Import", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            if (_Model.IsRunning)
            {
                _Model.InDemo = !_Model.InDemo;
                e.Handled = true;
                System.Windows.MessageBox.Show("Wait for the current job to complete.", "EBS Bulk Import", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            if (!_Model.InDemo)
            {
                _Model.InDemo = false;
                _Model.Render(true);
                e.Handled = true;
                return;
            }
            if (_Model.IsIncomplete)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Some files are partially imported. Do you want to leave?",
                    "EBS Bulk Import", MessageBoxButton.YesNo, MessageBoxImage.Question);
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
                    "EBS Bulk Import", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (MessageBoxResult.No == result)
                {
                    _Model.InDemo = !_Model.InDemo;
                    e.Handled = true;
                    return;
                }
            }
            _Model.Render(true);
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
