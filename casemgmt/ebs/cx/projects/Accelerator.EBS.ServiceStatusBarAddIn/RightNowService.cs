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
 *  date: Thu Nov 12 00:52:51 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: a7f4bc483d7a55df5bc4304cb8e89fa4867691d8 $
 * *********************************************************************************************
 *  File: RightNowService.cs
 * *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RightNow.AddIns.AddInViews;
using System.ServiceModel;
using Accelerator.EBS.SharedServices.RightNowServiceReference;
using Accelerator.EBS.SharedServices.Logs;
using System.ServiceModel.Channels;

namespace Accelerator.EBS.SharedServices
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
            binding2.MaxReceivedMessageSize = 5 * 1024 * 1024;
            binding2.MaxBufferSize = 5 * 1024 * 1024;
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

        /// <summary>
        /// Returns the contents of the given incident attachment.
        /// </summary>
        /// <param name="incidentId"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public byte[] getFileData(int incidentId, int fileId)
        {
            Incident incident = new Incident();
            incident.ID = new ID(); // incident id
            incident.ID.id = incidentId;
            incident.ID.idSpecified = true;
            FileAttachmentCommon file = new FileAttachmentCommon();
            file.ID = new ID(); // file id
            file.ID.id = fileId;
            file.ID.idSpecified = true;
            return _rnowClient.GetFileData(_rnowClientInfoHeader, incident , file.ID, false);
        }
    }
}
