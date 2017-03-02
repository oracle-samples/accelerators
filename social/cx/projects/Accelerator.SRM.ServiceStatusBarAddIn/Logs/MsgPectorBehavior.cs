/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2014, 2015, 2016, 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + SRM Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17.2 (February 2017) 
 *  reference: 160628-000117
 *  date: Fri Feb 10 19:47:43 PST 2017
 
 *  revision: rnw-17-2-fixes-release-2
 *  SHA1: $Id: 1a569bb845711486622230e38628d9c6488db5a4 $
 * *********************************************************************************************
 *  File: MsgPectorBehavior.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace Accelerator.SRM.SharedServices.Logs
{
    class MsgPectorBehavior: IEndpointBehavior
    {
        #region IEndpointBehavior Members
        public MyMessageInspector msgInspector;

        public MsgPectorBehavior()
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
            int start = reqPayload.IndexOf("<Action s:mustUnderstand=");
            int end = reqPayload.IndexOf("</Action>");
            end += 9;

            reqPayload = reqPayload.Remove(start, end - start);
            return null;
        }

        #endregion
    }
}
