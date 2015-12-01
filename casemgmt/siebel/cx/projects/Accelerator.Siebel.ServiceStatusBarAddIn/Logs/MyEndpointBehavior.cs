/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC Contact Center + Siebel Case Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  Siebel release: 8.1.1.15
 *  reference: 150520-000047
 *  date: Mon Nov 30 20:14:29 PST 2015

 *  revision: rnw-15-11-fixes-release-2
 *  SHA1: $Id: 0a8b4d95f2cf6aade2700547e5a337558eb383ef $
 * *********************************************************************************************
 *  File: MyEndpointBehavior.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace Accelerator.Siebel.SharedServices.Logs
{
    class MyEndpointBehavior: IEndpointBehavior
    {
        #region IEndpointBehavior Members
        public MyMessageInspector msgInspector;

        public MyEndpointBehavior()
        {
            this.msgInspector = new MyMessageInspector();
        }
        public void AddBindingParameters(
            ServiceEndpoint endpoint,
            BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(
            ServiceEndpoint endpoint, 
            ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(this.msgInspector);
        }

        public void ApplyDispatchBehavior(
            ServiceEndpoint endpoint, 
            EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(
            ServiceEndpoint endpoint)
        {
        }

        #endregion
    }



    public class MyMessageInspector : IClientMessageInspector
    {
        #region IClientMessageInspector Members
        public string reqPayload;
        public string resPayload;

        public void AfterReceiveReply(
            ref Message reply, 
            object correlationState)
        {
            resPayload = reply.ToString();
        }

        public object BeforeSendRequest(
            ref Message request, 
            IClientChannel channel)
        {
            reqPayload = request.ToString();
            return null;
        }

        #endregion
    }
}
