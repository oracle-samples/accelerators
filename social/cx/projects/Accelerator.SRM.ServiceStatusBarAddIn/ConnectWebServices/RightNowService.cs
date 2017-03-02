/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2014, 2015, 2016, 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + SRM Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17.2 (February 2017) 
 *  reference: 160628-000117
 *  date: Fri Feb 10 19:47:42 PST 2017
 
 *  revision: rnw-17-2-fixes-release-2
 *  SHA1: $Id: 28118b646183eda68c4ac0e267591f2afab26ceb $
 * *********************************************************************************************
 *  File: RightNowService.cs
 * *********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RightNow.AddIns.AddInViews;
using System.ServiceModel;
using Accelerator.SRM.SharedServices.RightNowServiceReference;
using Accelerator.SRM.SharedServices.Logs;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Accelerator.SRM.SharedServices
{
    public class RightNowService
    {
        #region Properties
        /// <summary>
        /// Field to store the created RightNowClient
        /// </summary>
        public RightNowSyncPortClient _rnowClient;

        /// <summary>
        /// Field to store the created header
        /// </summary>
        public ClientInfoHeader _rnowClientInfoHeader;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_gContext"></param>
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

        #endregion

        #region Methods

        /// <summary>
        /// Performs a passed ROQL query.
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public string[] queryData(string queryString)
        {
            byte[] outByte = new byte[1000];
            
            CSVTableSet tableSet = _rnowClient.QueryCSV(_rnowClientInfoHeader, queryString, 100, ",", false, false, out outByte);

            CSVTable[] csvTables = tableSet.CSVTables;
            CSVTable table = csvTables[0];
            string[] rowData = table.Rows;

            return rowData;
        }

        /// <summary>
        /// Async method for performing a ROQL query.
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public async Task<string[]> queryDataAsync(string queryString)
        {
            try
            {
                var request = new QueryCSVRequest(_rnowClientInfoHeader, queryString, 100, ",", false, false);
                var response = await _rnowClient.QueryCSVAsync(request);

                if (response.CSVTableSet.CSVTables.Length > 0)
                {
                    var tableSet = response.CSVTableSet;
                    CSVTable[] csvTables = tableSet.CSVTables;
                    CSVTable table = csvTables[0];
                    string[] rowData = table.Rows;

                    return rowData;
                }
            }
            catch (Exception e)
            {
                //Write an exception to the debugger.  Since this is an async method, an exception may be thrown by
                //a continuace exception.
                System.Diagnostics.Debug.WriteLine(string.Format("{0}\n{1}", e.Message, e.StackTrace));
            }

            return null;
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

        public void destroyObjects(RNObject[] objects)
        {
            DestroyProcessingOptions dpo = new DestroyProcessingOptions(){
                SuppressExternalEvents = false,
                SuppressRules = false
            };

            _rnowClient.Destroy(_rnowClientInfoHeader, objects, dpo);
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

        #endregion
}
