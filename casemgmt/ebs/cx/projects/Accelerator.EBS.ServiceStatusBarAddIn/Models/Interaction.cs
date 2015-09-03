/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.5 (May 2015)
 *  EBS release: 12.1.3
 *  reference: 150202-000157
 *  date: Wed Sep  2 23:11:41 PDT 2015

 *  revision: rnw-15-8-fixes-release-01
 *  SHA1: $Id: ca1a49d115b471878f2f605d4fd0cb02bb185467 $
 * *********************************************************************************************
 *  File: Interaction.cs
 * *********************************************************************************************/

using Accelerator.EBS.SharedServices.Providers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accelerator.EBS.SharedServices
{
    public class Interaction : ModelObjectBase
    {
        public static string CreateInteractionURL { get; set; }

        public string ErrorMessage { get; set; }
        public decimal? SrID { get; set; }
        public string Author { get; set; }
        public DateTime Created { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string Channel { get; set; }
        public decimal? InteractionID { get; set; }
        public decimal? CreatedByID { get; set; }
        public string Status { get; set; }

        private static IEBSProvider _provider = null;

        public bool Create(int _logIncidentId = 0, int _logContactId  = 0)
        {
            if (_provider == null)
            {
                throw new Exception("EBS Provider not initialized.");
            }

            //Switch Provider to call web service
            Interaction interaction = Interaction._provider.CreateInteraction(this, _logIncidentId, _logContactId);
            this.InteractionID = interaction.InteractionID;
            this.ErrorMessage = interaction.ErrorMessage;

            return String.IsNullOrWhiteSpace(this.ErrorMessage);
        }

        public static void InitEBSProvider()
        {
            Type t = Type.GetType(ServiceProvider);

            try
            {
                _provider = Activator.CreateInstance(t) as IEBSProvider;
                _provider.InitForInteraction(CreateInteractionURL, ServiceUsername, ServicePassword, ServiceClientTimeout);
                _provider.log = ConfigurationSetting.logWrap;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
