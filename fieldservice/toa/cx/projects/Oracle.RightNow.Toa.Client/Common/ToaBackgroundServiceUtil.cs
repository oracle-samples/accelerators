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
 *  date: Mon Aug 24 09:01:16 PDT 2015

 *  revision: rnw-15-11-fixes-release-01
*  SHA1: $Id: 263fc9c141e96468520d0d9f00467ec34b16ce8b $
* *********************************************************************************************
*  File: ToaBackgroundServiceUtil.cs
* ****************************************************************************************** */

using System;
using System.ComponentModel;

namespace Oracle.RightNow.Toa.Client.Common
{
    public class ToaBackgroundServiceUtil 
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
