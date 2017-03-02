/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2014, 2015, 2016, 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + SRM Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17.2 (February 2017) 
 *  reference: 160628-000117
 *  date: Fri Feb 10 19:47:44 PST 2017
 
 *  revision: rnw-17-2-fixes-release-2
 *  SHA1: $Id: eca30aa65243864fb4583de4446b04114cc33af1 $
 * *********************************************************************************************
 *  File: ProxyFactory.cs
 * *********************************************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using Accelerator.SRM.SharedServices.Logs;

namespace Accelerator.SRM.SharedServices
{
    /// <summary>
    /// Factory class to create an instance of the SRM proxy client.
    /// </summary>
    internal static class ProxyFactory
    {     
        private static TClient GetServiceClient<TClient, TInterface>(string url, string Username, string Password, int timeout)
            where TClient : class
            where TInterface : class
        {
            SecurityBindingElement securityElement = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
            securityElement.AllowInsecureTransport = true;
            TextMessageEncodingBindingElement encodingElement = new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8);
            
            //Handle HTTPS transport binding
            HttpTransportBindingElement transportElement;
            if (url.StartsWith("https://"))
            {
                transportElement = new HttpsTransportBindingElement();
            }
            else
            {
                transportElement = new HttpTransportBindingElement();
            }

            transportElement.MaxReceivedMessageSize = Int32.MaxValue;
            securityElement.EnableUnsecuredResponse = true;

            TClient clientt = Activator.CreateInstance(typeof(TClient), new object[] 
            { new CustomBinding(securityElement, encodingElement, transportElement), 
            new EndpointAddress(url) }) as TClient;
            ClientBase<TInterface> client = clientt as ClientBase<TInterface>;
            client.ClientCredentials.UserName.UserName = Username;
            client.ClientCredentials.UserName.Password = Password;
            client.Endpoint.Binding.SendTimeout = TimeSpan.FromMilliseconds(timeout);
            client.Endpoint.Binding.ReceiveTimeout = TimeSpan.FromMilliseconds(timeout);
            MsgPectorBehavior eBehavior = new MsgPectorBehavior();
            client.Endpoint.EndpointBehaviors.Add(eBehavior);

            return clientt;
        }
    }
}
