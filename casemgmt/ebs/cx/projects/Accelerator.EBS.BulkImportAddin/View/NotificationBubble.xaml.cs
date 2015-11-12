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
 *  SHA1: $Id: 3b81d3aeb1b5cdb18b921c99b5105a7b37463960 $
 * *********************************************************************************************
 *  File: NotificationBubble.xaml.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Accelerator.EBS.BulkImportAddin
{
    /// <summary>
    /// Interaction logic for NotificationBubble.xaml
    /// </summary>
    public partial class NotificationBubble : Window, INotifyPropertyChanged
    {
        private string _Message;

        public string Message 
        {
            get { return _Message; }
            set
            {
                if (_Message != value)
                {
                    _Message = value;
                    OnPropertyChanged("Message");
                }
            }
        }

        public NotificationBubble(string msg)
        {
            InitializeComponent();
            _Message = msg;
            DataContext = this;
            Show();
            Closed += NotificationBubbleClosed;
        }

        public new void Show()
        {
            Topmost = true;
            base.Show();
            var sc = Screen.PrimaryScreen.WorkingArea;
            var top = sc.Bottom - ActualHeight;
            top -= 75;
            Left = sc.Right - ActualWidth;
            foreach (Window win in System.Windows.Application.Current.Windows)
            {
                string name = win.GetType().Name;
                if (this != win && "NotificationBubble".Equals(name))
                {
                    top = win.Top - win.ActualHeight;
                    win.Topmost = true;
                }
            }
            Top = top;
        }

        private void AnimationCompleted(object sender, EventArgs e)
        {
            if (!IsMouseOver)
                Close();
        }

        private void NotificationBubbleClosed(object sender, EventArgs e)
        {
            foreach (Window win in System.Windows.Application.Current.Windows)
            {
                string name = win.GetType().Name;
                if (this != win && "NotificationBubble".Equals(name))
                    if (win.Top < Top)
                        win.Top = win.Top + ActualHeight;
            }
        }

        private void bubbleGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(String propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
