/* * *******************************************************************************************
*  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + OFSC Reference Integration
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (Feb 2015)
 *  OFSC release: 15.2 (Feb 2015)
 *  reference: 150622-000130
 *  date: Thu Sep  3 23:14:02 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
*  SHA1: $Id: 242d92293be5b30f0c751f76085144594ecd1833 $
* *********************************************************************************************
*  File: InboundRequest.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.RightNow.Toa.Client.Common;
using Oracle.RightNow.Toa.Client.InboundProxyService;
using Oracle.RightNow.Toa.Client.Model;

namespace Oracle.RightNow.Toa.Client.Services
{
    public class InboundRequest
    {
        private UploadType _uploadType;
        private string _transactionId;
        private string _uploadDate;
        private string _providerGroup;
        private string _defaultAppointmentPool;
        private ProcessingMode _processingMode;
        private AllowChangeDate _allowChangeDate;
        private PropertiesMode _propertiesMode;
        private ActivitySettingsModel _activitySettings;
        private InventorySettingsModel _inventorySettings;
        private List<IToaModel> _dataModels;

        public InboundRequest(List<IToaModel> models)
        {
            _uploadType = UploadType.Incremental;
            _uploadDate = DateTime.Now.ToString("yyyy-MM-dd");
            //_activitySettings = new ActivitySettingsModel();
            //_inventorySettings = new InventorySettingsModel();
            //_processingMode = ProcessingMode.None;
            //_allowChangeDate = AllowChangeDate.Yes;
            //_propertiesMode = PropertiesMode.None;
            //_transactionId = "";
            //_providerGroup = "";
            //_defaultAppointmentPool = "";
            _dataModels = models;
        }

        public UploadType UploadType
        {
            get { return _uploadType; }
            set { _uploadType = value; }
        }

        public string TransactionId
        {
            get { return _transactionId; }
            set { _transactionId = value; }
        }

        public string UploadDate
        {
            get { return _uploadDate; }
            set { _uploadDate = value; }
        }

        public string ProviderGroup
        {
            get { return _providerGroup; }
            set { _providerGroup = value; }
        }

        public string DefaultAppointmentPool
        {
            get { return _defaultAppointmentPool; }
            set { _defaultAppointmentPool = value; }
        }

        public ProcessingMode ProcessingMode
        {
            get { return _processingMode; }
            set { _processingMode = value; }
        }

        public AllowChangeDate AllowChangeDate
        {
            get { return _allowChangeDate; }
            set { _allowChangeDate = value; }
        }

        public PropertiesMode PropertiesMode
        {
            get { return _propertiesMode; }
            set { _propertiesMode = value; }
        }

        public List<IToaModel> DataModels
        {
            get { return _dataModels; }
            set { _dataModels = value; }
        }

        public ActivitySettingsModel ActivitySettings
        {
            get { return _activitySettings; }
            set { _activitySettings = value; }
        }

        public InventorySettingsModel InventorySettings
        {
            get { return _inventorySettings; }
            set { _inventorySettings = value; }
        }

        internal InboundInterfaceElement GetInboundRequestElement()
        {
            var inboundRequest = new InboundInterfaceElement();

            // Initialize User
            inboundRequest.user = ToaUserUtil.GetInboundUser();
            
            //Initialize Head
            var head = new HeadElement();
            head.allow_change_date = ToaStringsUtil.GetString(_allowChangeDate);
            head.date = _uploadDate;
            head.default_appointment_pool = _defaultAppointmentPool;
            head.id = _transactionId;
            head.processing_mode = ToaStringsUtil.GetString(_processingMode);
            head.properties_mode = ToaStringsUtil.GetString(_propertiesMode);
            head.upload_type = ToaStringsUtil.GetString(_uploadType);
            head.provider_group = _providerGroup;
            if(null != _activitySettings)
                head.appointment = _activitySettings.GetActivitySettings();
            if(null != _inventorySettings)
                head.inventory = _inventorySettings.GetInventorySettings();
            inboundRequest.head = head;

            //initialize Data
            var data = new DataElement();
            var noOfModels = _dataModels.Count;
            CommandElement [] commands = null;
            ProviderElement [] providers = null;
            if (UploadType == UploadType.Full) // Full Upload
            {
                providers = new ProviderElement[noOfModels];
                foreach (var model in _dataModels)
                {
                    if (model is WorkOrderModel)
                    {
                        var activityModel = ((WorkOrderModel) model);
                        var providerElement = new ProviderElement();
                        providerElement.appointment = activityModel.GetActivityElement();
                        providers[--noOfModels] = providerElement;
                    }
                    else if (model is InventoryModel)
                    {
                        //TODO: Need to initialize inventory object 
                    }
                }
                data.providers = providers;
            }
            else // Incremental Upload
            {
                commands = new CommandElement[noOfModels];
                foreach (var model in _dataModels)
                {
                    if (model is WorkOrderModel)
                    {
                        var workOrderModel = ((WorkOrderModel)model);
                        var command = new CommandElement();
                        command.appointment = workOrderModel.GetActivityElement();
                        command.date = workOrderModel.AssignedDate;
                        command.external_id = workOrderModel.ExternalId;
                        command.type = ToaStringsUtil.GetString(workOrderModel.CommandType);
                        commands[--noOfModels] = command;
                    }
                    else if (model is InventoryModel)
                    {
                        //TODO: Need to initialize inventory object
                    }
                }
                data.commands = commands;
            }
            
            

            inboundRequest.data = data;

            return inboundRequest;
        }
    }

    
    
}
