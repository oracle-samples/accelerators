/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + OSC Lead Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.11 (November 2015)
 *  OSC release: Release 10
 *  reference: 150505-000122, 160620-000160
 *  date: Mon Sep 19 02:05:25 PDT 2016

 *  revision: rnw-15-11-fixes-release-3
*  SHA1: $Id: 2b01bae1afe6b7f112e06f87c04de7d82a3e7cc7 $
* *********************************************************************************************
*  File: OSCBackgroundServiceUtil.cs
* ****************************************************************************************** */

using System;
using System.ComponentModel;

namespace Accelerator.SalesCloud.Client.Common
{
    public class OSCBackgroundServiceUtil 
    {
        /// <summary>
        /// Runs the specified action asynchronously.
        /// </summary>
        /// <param name="actionToRun">The action to run.</param>
        /// <exception cref="ArgumentNullException"><paramref name="actionToRun"/> is null.</exception>
        public void RunAsync(Action actionToRun)
        {
            if (actionToRun == null)
                throw new ArgumentNullException("actionToRun");

            var worker = new BackgroundWorker();

            DoWorkEventHandler doWorkHandler = null;
            doWorkHandler = (sender, e) =>
            {
                actionToRun();
                worker.DoWork -= doWorkHandler;
            };
            worker.DoWork += doWorkHandler;

            worker.RunWorkerCompleted += BackgroundWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void BackgroundWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Todo: add logging message once thread is completed.
            Console.WriteLine("Thread is completed");
        }
    }
}
