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
 *  date: Mon Aug 24 09:01:19 PDT 2015

 *  revision: rnw-15-11-fixes-release-01
*  SHA1: $Id: cc3389be1a5c2d0420afc47eb49104bfb602a1db $
* *********************************************************************************************
*  File: InboundService.cs
* ****************************************************************************************** */

using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.RightNow.Toa.Client.Common;
using Oracle.RightNow.Toa.Client.InboundProxyService;
using Oracle.RightNow.Toa.Client.Logs;
using Oracle.RightNow.Toa.Client.Rightnow;
using System.Xml.Serialization;
using System.IO;
using System.Web.Helpers;

namespace Oracle.RightNow.Toa.Client.Services
{
    public class InboundService : IInboundService
    {
        private InboundInterfacePortClient _inboundInterfaceService;
        private static InboundService _inboundService;
        private static object _sync = new object();
        private IToaLog _log;

        /// <summary>
        /// Get Inbound Service object
        /// </summary>
        /// <returns></returns>
        public static InboundService GetService()
        {
            if (_inboundService != null)
            {
                return _inboundService;
            }

            if (!RightNowConfigService.IsConfigured())
            {
                return null;
            }

            try
            {
                lock (_sync)
                {
                    if (_inboundService == null)
                    {
                        var inboundWsdlUrl = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.ToaInboundServiceUrl);

                        EndpointAddress endpoint = new EndpointAddress(inboundWsdlUrl);
                        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
                        _inboundService = new InboundService();
                        _inboundService._inboundInterfaceService = new InboundInterfacePortClient(binding, endpoint);
                        _inboundService._log = ToaLogService.GetLog();
                    }
                }
            }
            catch (Exception e)
            {
                _inboundService = null;
                MessageBox.Show(ToaExceptionMessages.InboudServiceNotInitialized, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return _inboundService;
        }

        private InboundService()
        {

        }

        /// <summary>
        /// Begin request for Inbound service.
        /// </summary>
        /// <param name="inboundRequest"></param>
        /// <param name="inboundServiceCallback"></param>
        public void BeginRequest(InboundRequest inboundRequest, InboundServiceDelegate inboundServiceCallback)
        {
            _log.Debug("InboundService - BeginRequest() - Start");
            var backgroundService = new ToaBackgroundServiceUtil();
            backgroundService.RunAsync(() =>
                {
                    _log.Debug("InboundService - BeginRequest() - Thread Started");
                    var inboundRequestElement = inboundRequest.GetInboundRequestElement();
                    var inboundResult = _inboundInterfaceService.inbound_interface(inboundRequestElement);

                    //TODO: Initialize response
                    var toaResponse = new ToaRequestResult();
                    inboundServiceCallback.Invoke(toaResponse);
                    _log.Debug("InboundService - BeginRequest() - Thread Ended");

                });
            _log.Debug("InboundService - BeginRequest() - End");
        }

        /// <summary>
        /// Begin request for Inbound service.
        /// </summary>
        /// <param name="inboundRequest"></param>
        /// <param name="inboundServiceCallback"></param>
        public ToaRequestResult BeginSyncRequest(InboundRequest inboundRequest)
        {
            _log.Debug("InboundService - BeginRequest() - Start");
            var inboundRequestElement = inboundRequest.GetInboundRequestElement();
            _log.Debug(ToaLogMessages.InboundServiceRequest,Json.Encode(inboundRequestElement));

            //Validate the current sitename.
            if (!ToaCommonUtil.ValidateCurrentSiteName())
            {
                return null;
            }

            var inboundResult = _inboundInterfaceService.inbound_interface(inboundRequestElement);
            _log.Debug(ToaLogMessages.InboundServiceResponse, Json.Encode(inboundResult));

            //TODO: Initialize response
            var toaResponse = new ToaRequestResult();
            _log.Notice("Started processing response element");
            toaResponse.processResponseElement(inboundResult);
            _log.Debug("InboundService - BeginRequest() - End");
            return toaResponse;
        }
    }
}