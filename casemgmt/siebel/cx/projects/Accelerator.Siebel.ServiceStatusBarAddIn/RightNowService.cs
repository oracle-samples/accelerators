/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.5 (May 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 141216-000121
 *  date: Wed Sep  2 23:14:42 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: 31bd751d0b18d22454d5940bf135677b677c03e6 $
 * *********************************************************************************************
 *  File: RightNowService.cs
 * *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RightNow.AddIns.AddInViews;
using System.ServiceModel;
using Accelerator.Siebel.SharedServices.RightNowServiceReference;
using Accelerator.Siebel.SharedServices.Logs;
using System.ServiceModel.Channels;

namespace Accelerator.Siebel.SharedServices
{
    public class RightNowService
    {
        public RightNowSyncPortClient _rnowClient;
        public ClientInfoHeader _rnowClientInfoHeader;

        public RightNowService(IGlobalContext _gContext)
        {
            // Set up SOAP API request to retrieve Endpoint Configuration - 
            // Get the SOAP API url of current site as SOAP Web Service endpoint
            EndpointAddress endPointAddr = new EndpointAddress(_gContext.GetInterfaceServiceUrl(ConnectServiceType.Soap));


            // Minimum required
            BasicHttpBinding binding2 = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
            binding2.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;

            // Optional depending upon use cases
            binding2.MaxReceivedMessageSize = 1024 * 1024;
            binding2.MaxBufferSize = 1024 * 1024;
            binding2.MessageEncoding = WSMessageEncoding.Mtom;

            // Create client proxy class
            _rnowClient = new RightNowSyncPortClient(binding2, endPointAddr);
            BindingElementCollection elements = _rnowClient.Endpoint.Binding.CreateBindingElements();
            elements.Find<SecurityBindingElement>().IncludeTimestamp = false;
            _rnowClient.Endpoint.Binding = new CustomBinding(elements);

            // Add SOAP msg inspector behavior
            //_rnowClient.Endpoint.Behaviors.Add(new LogMsgBehavior());

            // Ask the Add-In framework the handle the session logic
            _gContext.PrepareConnectSession(_rnowClient.ChannelFactory);

            // Set up query and set request
            _rnowClientInfoHeader = new ClientInfoHeader();
            _rnowClientInfoHeader.AppID = "Case Management Accelerator Services";
        }

        public string[] queryData(string queryString)
        {
            byte[] outByte = new byte[1000];
            
            CSVTableSet tableSet = _rnowClient.QueryCSV(_rnowClientInfoHeader, queryString, 100, ",", false, false, out outByte);

            CSVTable[] csvTables = tableSet.CSVTables;
            CSVTable table = csvTables[0];
            string[] rowData = table.Rows;

            return rowData;
        }

        public void updateObject(RNObject[] objects)
        {
            //Create the update processiong options
            UpdateProcessingOptions options = new UpdateProcessingOptions();
            options.SuppressExternalEvents = false;
            options.SuppressRules = false;


            //Invoke the Update operation
            _rnowClient.Update(this._rnowClientInfoHeader, objects, options);
        }

        public RNObject[] createObject(RNObject[] objects)
        {
            //Create the update processiong options
            CreateProcessingOptions options = new CreateProcessingOptions();
            options.SuppressExternalEvents = false;
            options.SuppressRules = false;


            //Invoke the Update operation
            RNObject[] results = _rnowClient.Create(this._rnowClientInfoHeader, objects, options);
            return results;
        }

        public string[] getReportResult(int reportId, List<AnalyticsReportFilter> filterList)
        {
            //Use RightNow SOAP API to run report and get RNow Contact search results
            //Set up Analytics Report
            //Create new AnalyticsReport Object
            AnalyticsReport analyticsReport = new AnalyticsReport();
            //create limit and start parameters. Specifies the max number of rows to return (10,000 is the overall maximum)
            //start specifies the starting row
            int limit = 10;
            int start = 0;

            //Specify a report ID
            ID reportID = new ID();
            //contact search report
            reportID.id = reportId;
            reportID.idSpecified = true;
            analyticsReport.ID = reportID;

            analyticsReport.Filters = filterList.ToArray();

            CSVTableSet thisset = new CSVTableSet();
            byte[] fd;

            // Run AnalyticsReport via SOAP API
            thisset = _rnowClient.RunAnalyticsReport(_rnowClientInfoHeader, analyticsReport, limit, start, ",", false, true, out fd);

            // Get Report Search Result
            CSVTable[] tableResults = thisset.CSVTables;
            String[] searchResults = tableResults[0].Rows;

            return searchResults;
        }
    }
}
