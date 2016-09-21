/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015,2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + OSC Lead Management Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.11 (November 2015)
 *  OSC release: Release 10
 *  reference: 150505-000122, 160620-000160
 *  date: Mon Sep 19 02:05:25 PDT 2016

 *  revision: rnw-15-11-fixes-release-3
 *  SHA1: $Id: 71b8068ca07d1f22ac1cbe71e97145b6649a2243 $
 * *********************************************************************************************
 *  File: EmptyElementInspector.cs
 * ****************************************************************************************** */

using System;
using System.IO;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Windows;
using System.Xml;
using Accelerator.SalesCloud.Client.Logs;

namespace Accelerator.SalesCloud.Client.Interceptors
{
    #region Request Response interceptor

    public class EmptyElementInspector : IClientMessageInspector
    {
        private IOSCLog _logger;

        public EmptyElementInspector()
        {
            _logger = OSCLogService.GetLog();
        }

        /// <summary>
        /// This will parse the response received from the service and remove all empty elements from it
        /// </summary>
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            MemoryStream memoryStream = new MemoryStream();
            XmlWriter xmlWriter = XmlWriter.Create(memoryStream);
            reply.WriteMessage(xmlWriter);
            xmlWriter.Flush();
            memoryStream.Position = 0;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(memoryStream);

            XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
            xmlNamespaceManager.AddNamespace("env", "http://schemas.xmlsoap.org/soap/envelope/");
            XmlNode header = xmlDocument.SelectSingleNode("//env:Header", xmlNamespaceManager);
            if (header != null)
            {
                header.ParentNode.RemoveChild(header);
            }

            XmlNodeList nodes = xmlDocument.SelectNodes("//node()");
            foreach (XmlNode node in nodes)
            {
                if (node.NodeType == XmlNodeType.Element && node.ChildNodes.Count == 0 && node.InnerXml == "" && node.Attributes.Count == 0)
                {
                    node.ParentNode.RemoveChild(node);
                }
            }
            memoryStream = new MemoryStream();
            xmlDocument.Save(memoryStream);
            memoryStream.Position = 0;
            XmlReader xmlReader = XmlReader.Create(memoryStream);
            System.ServiceModel.Channels.Message newMessage = System.ServiceModel.Channels.Message.CreateMessage(xmlReader, int.MaxValue, reply.Version);
            newMessage.Headers.CopyHeadersFrom(reply.Headers);
            newMessage.Properties.CopyProperties(reply.Properties);

            //Logging processed response
            MessageBuffer buffer = newMessage.CreateBufferedCopy(Int32.MaxValue);
            newMessage = buffer.CreateMessage();
            LogRequestResponse(buffer, "Opportunity Service Response");

            reply = newMessage;
        }

        /// <summary>
        /// This method removes the empty tags from the soap request before sending the request to the server.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            MemoryStream memoryStream = new MemoryStream();
            XmlWriter xmlWriter = XmlWriter.Create(memoryStream);
            request.WriteMessage(xmlWriter);
            xmlWriter.Flush();
            memoryStream.Position = 0;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(memoryStream);

            XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
            xmlNamespaceManager.AddNamespace("env", "http://schemas.xmlsoap.org/soap/envelope/");
            XmlNode header = xmlDocument.SelectSingleNode("//env:Header", xmlNamespaceManager);
            if (header != null)
            {
                header.ParentNode.RemoveChild(header);
            }

            XmlNodeList nodes = xmlDocument.SelectNodes("//node()");
            foreach (XmlNode node in nodes)
            {
                if (node.NodeType == XmlNodeType.Element && node.ChildNodes.Count == 0 && node.InnerXml == "")
                {
                    node.ParentNode.RemoveChild(node);
                }
            }
            memoryStream = new MemoryStream();
            xmlDocument.Save(memoryStream);
            memoryStream.Position = 0;
            XmlReader xmlReader = XmlReader.Create(memoryStream);
            System.ServiceModel.Channels.Message newMessage = System.ServiceModel.Channels.Message.CreateMessage(xmlReader, int.MaxValue, request.Version);
            newMessage.Headers.CopyHeadersFrom(request.Headers);
            newMessage.Properties.CopyProperties(request.Properties);

            //Logging processed request
            MessageBuffer buffer = newMessage.CreateBufferedCopy(Int32.MaxValue);
            newMessage = buffer.CreateMessage();
            LogRequestResponse(buffer, "Opportunity Service Request");

            request = newMessage;
            return request;
        }

        /// <summary>
        /// Logs the request or response
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="message"></param>
        private void LogRequestResponse(MessageBuffer buffer, string message)
        {
            Message msg = buffer.CreateMessage();
            StringBuilder sb = new StringBuilder();
            using (System.Xml.XmlWriter xw = System.Xml.XmlWriter.Create(sb))
            {
                msg.WriteMessage(xw);
                xw.Close();
            }
            _logger.Debug(message, sb.ToString());
        }
    }

    /// <summary>
    /// This class is used to add the custom inspector to process the response before de-serialization
    /// </summary>
    public class EmptyElementBehavior : System.ServiceModel.Description.IEndpointBehavior
    {
        public void AddBindingParameters(System.ServiceModel.Description.ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            //no-op
        }

        public void ApplyClientBehavior(System.ServiceModel.Description.ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            EmptyElementInspector inspector = new EmptyElementInspector();
            clientRuntime.MessageInspectors.Add(inspector);
        }

        public void ApplyDispatchBehavior(System.ServiceModel.Description.ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            //no-op
        }

        public void Validate(System.ServiceModel.Description.ServiceEndpoint endpoint)
        {
            //no-op
        }
    }

    #endregion

}
