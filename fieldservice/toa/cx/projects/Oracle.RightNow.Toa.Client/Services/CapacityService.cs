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
*  SHA1: $Id: 3645e5e33805132928340903d28b8fe70e856d43 $
* *********************************************************************************************
*  File: CapacityService.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using Oracle.RightNow.Toa.Client.Common;
using Oracle.RightNow.Toa.Client.Exceptions;
using Oracle.RightNow.Toa.Client.Logs;
using Oracle.RightNow.Toa.Client.Model;
using System.Windows.Forms;
using Oracle.RightNow.Toa.Client.Rightnow;
using System.ServiceModel;
using System.Web.Helpers;

namespace Oracle.RightNow.Toa.Client.Services
{
    public class CapacityService : ICapacityService
    {
        private toa_capacity_interface _toaCapacityInterface;
        private IToaLog _log;
        private CapacityService()
        {

        }

        /// <summary>
        /// Get Work Order Area mapped to zipcode
        /// </summary>
        /// <param name="capacityModel">Capacity Model object</param>
        /// <param name="capacityCallback">Callback method</param>
        public void GetWorkOrderArea(CapacityModel capacityModel, CapacityServiceDelegate capacityCallback)
        {
            _log.Notice("Inside GetWorkOrderArea");
            var backgroundService = new ToaBackgroundServiceUtil();

            backgroundService.RunAsync(() =>
            {
                try
                {
                    capacity_element[] capacityElement = capacityModel.getCapacityElement();
                    time_slot_info_element[] timeSlotInfoElement = capacityModel.getTimeSlotInfoElement();
                    long activityTravelTime = capacityModel.ActivityTravelTime;
                    bool activityTravelTimeSpecified = capacityModel.AggregateResultsSpecified;

                    _toaCapacityInterface.get_capacity(ToaUserUtil.GetCapacityUser(), capacityModel.QuotaDates, capacityModel.Location,
                        capacityModel.CalculateDuration, capacityModel.CalculateDurationSpecified, capacityModel.CalculateTravelTime, capacityModel.CalculateTravelTimeSpecified,
                        capacityModel.CalculateWorkSkill, capacityModel.CalculateWorkSkillSpecified, capacityModel.ReturnTimeSlotInfo, capacityModel.ReturnTimeSlotInfoSpecified,
                        capacityModel.DetermineLocationByWorkZone, capacityModel.DetermineLocationByWorkZoneSpecified, capacityModel.DontAggregateResults, capacityModel.DontAggregateResultsSpecified,
                        capacityModel.MinTimeEndOfTimeSlot, capacityModel.MinTimeEndOfTimeSlotSpecified,
                        capacityModel.DefaultDuration, capacityModel.DefaultDurationSpecified, capacityModel.Timeslots, capacityModel.WorkSkill,
                        capacityModel.getActivityFieldElement(),
                        out activityTravelTime, out activityTravelTimeSpecified, out activityTravelTime, out activityTravelTimeSpecified, out capacityElement, out timeSlotInfoElement);

                    CapacityModel response = new CapacityModel();

                    HashSet<string> locations = new HashSet<string>();
                    if (capacityElement != null)
                    {
                        foreach (capacity_element ce in capacityElement)
                        {
                            if (!locations.Contains(ce.location))
                            {
                                locations.Add(ce.location);
                            }
                        }
                    }

                    response.Location = new string[locations.Count];
                    locations.CopyTo(response.Location);
                    // initialize  toa result and activity model object
                    var toaRequestResult = new ToaRequestResult();
                    toaRequestResult.DataModels.Add(response);

                    toaRequestResult.ResultCode = ToaRequestResultCode.Success;

                    if (locations.Count == 0)
                    {
                        List<ReportMessageModel> reportMessageModel = new List<ReportMessageModel>();
                        _log.Error("Unable to determine work zone for given fields");
                        reportMessageModel.Add(new ReportMessageModel("No Work Order Areas exist for this Postal Code, please update the Postal Code field or submit with no timeslot and data selected", null, null, "Unable to determine work zone for given fields"));
                        toaRequestResult.ReportMessages = reportMessageModel;
                        toaRequestResult.ResultCode = ToaRequestResultCode.Failure;
                    }

                    capacityCallback.Invoke(toaRequestResult);
                }
                catch (Exception exception)
                {
                    _log.Error("Unable to fetch Work Order Area");
                    _log.Error(exception.StackTrace);
                    MessageBox.Show("No Work Order Areas exist for this Postal Code, please update the Postal Code field or submit with no timeslot and data selected");
                }
            });
        }

        /// <summary>
        /// Get Quota for month
        /// </summary>
        /// <param name="capacityModel"></param>
        /// <returns></returns>
        public ToaRequestResult GetQuotaForMonth(CapacityModel capacityModel)
        {
            _log.Notice("Inside GetQuotaForMonth");
            var toaRequestResult = new ToaRequestResult();
            try
            {
                capacity_element[] capacityElement = capacityModel.getCapacityElement();
                time_slot_info_element[] timeSlotInfoElement = capacityModel.getTimeSlotInfoElement();
                long activityTravelTime = capacityModel.ActivityTravelTime;
                bool activityTravelTimeSpecified = capacityModel.ActivityTravelTimeSpecified;
                long activityDuration = capacityModel.ActivityDuration;
                bool activityDurationSpecified = capacityModel.ActivityDurationSpecified;

                _toaCapacityInterface.get_capacity(ToaUserUtil.GetCapacityUser(), capacityModel.QuotaDates, capacityModel.Location,
                    capacityModel.CalculateDuration, capacityModel.CalculateDurationSpecified, capacityModel.CalculateTravelTime, capacityModel.CalculateTravelTimeSpecified,
                    capacityModel.CalculateWorkSkill, capacityModel.CalculateWorkSkillSpecified, capacityModel.ReturnTimeSlotInfo, capacityModel.ReturnTimeSlotInfoSpecified,
                    capacityModel.DetermineLocationByWorkZone, capacityModel.DetermineLocationByWorkZoneSpecified, capacityModel.DontAggregateResults, capacityModel.DontAggregateResultsSpecified,
                    capacityModel.MinTimeEndOfTimeSlot, capacityModel.MinTimeEndOfTimeSlotSpecified,
                    capacityModel.DefaultDuration, capacityModel.DefaultDurationSpecified, capacityModel.Timeslots, capacityModel.WorkSkill,
                    capacityModel.getActivityFieldElement(),
                    out activityDuration, out activityDurationSpecified, out activityTravelTime, out activityTravelTimeSpecified, out capacityElement, out timeSlotInfoElement);
            
                CapacityModel response = new CapacityModel();
                
                if (activityDuration != null)
                {
                    response.ActivityDuration = activityDuration;
                }

                if (activityTravelTime != null)
                {
                    response.ActivityTravelTime = activityTravelTime;
                }

                Dictionary<DateTime, DayQuota> dayQuota = new Dictionary<DateTime, DayQuota>();
                if (capacityElement != null)
                {
                    foreach (capacity_element ce in capacityElement)
                    {
                        if (dayQuota.ContainsKey(ce.date))
                        {
                            DayQuota quota = dayQuota[ce.date];
                            if (quota.TimeSlotQuota == null)
                            {
                                quota.TimeSlotQuota = new Dictionary<string, float>();
                            }
                            float quotaAvailable = (((ce.available - (activityDuration + activityTravelTime)) * 100) / ce.quota);

                            quota.TimeSlotQuota.Add(ce.time_slot, quotaAvailable);
                        }
                        else
                        {
                            DayQuota quota = new DayQuota();
                            quota.QuotaDate = ce.date;
                            quota.TimeSlotQuota = new Dictionary<string, float>();
                            float quotaAvailable = (((ce.available - (activityDuration + activityTravelTime)) * 100) / ce.quota);
                            quota.TimeSlotQuota.Add(ce.time_slot, quotaAvailable);
                            dayQuota.Add(ce.date, quota);
                        }
                    }
                }

                _log.Debug("Monthy Quota Response: ", Json.Encode(capacityElement));
                _log.Debug("TimeSlots: ", Json.Encode(timeSlotInfoElement));

                HashSet<string> timeSlotSet = new HashSet<string>();
                if (timeSlotInfoElement != null)
                {
                    foreach (time_slot_info_element ts in timeSlotInfoElement)
                    {
                        timeSlotSet.Add(ts.label);
                    }
                }

                if (timeSlotSet.Count > 0)
                {
                    response.Timeslots = new string[timeSlotSet.Count];
                    timeSlotSet.CopyTo(response.Timeslots);
                    Array.Sort(response.Timeslots);
                }

                DateTime currentDate = DateTime.Today;
                bool todayExists = Array.IndexOf(capacityModel.QuotaDates, currentDate) >= 0;

                //Fix the closing quota issue
                if (todayExists)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (dayQuota.ContainsKey(currentDate))
                        {
                            DayQuota quota = dayQuota[currentDate];
                            foreach (string timeSlot in response.Timeslots)
                            {
                                if (!quota.TimeSlotQuota.ContainsKey(timeSlot))
                                {
                                    quota.TimeSlotQuota.Add(timeSlot, 0);
                                }
                            }
                        }
                        else
                        {
                            DayQuota quota = new DayQuota();
                            quota.QuotaDate = currentDate;
                            quota.TimeSlotQuota = new Dictionary<string, float>();
                            foreach (string timeSlot in response.Timeslots)
                            {
                                quota.TimeSlotQuota.Add(timeSlot, 0);
                            }
                            dayQuota.Add(currentDate, quota);
                        }

                        currentDate = DateTime.Today.AddDays(1);
                        bool currentDateExists = Array.IndexOf(capacityModel.QuotaDates, currentDate) >= 0;
                        if (!currentDateExists)
                        {
                            break;
                        }
                    }
                }    
                    
                response.DayQuota = dayQuota;
                // initialize  toa result and activity model object
                
                toaRequestResult.DataModels.Add(response);
                if (dayQuota.Count == 0 || timeSlotSet.Count == 0)
                {
                    toaRequestResult.ResultCode = ToaRequestResultCode.Failure;
                }
                else
                {
                    toaRequestResult.ResultCode = ToaRequestResultCode.Success;
                }

                return toaRequestResult;
            }
            catch (Exception exception)
            {
                _log.Error("Unable to fetch Quota for given dates");
                _log.Error(exception.StackTrace);
                MessageBox.Show("No quota available for the Work Order Type and Work Order Area selected.  Please reselect, if you continue to receive this warning, please contact support for assistance.",
                    "Warning: No Quota Available", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                toaRequestResult.ResultCode = ToaRequestResultCode.Failure;
            }
            return toaRequestResult;
        }

        /// <summary>
        /// Get instance of CapacityService Object
        /// </summary>
        /// <returns></returns>
        public static ICapacityService GetService()
        {
            if (!RightNowConfigService.IsConfigured())
            {
                return null;
            }
            CapacityService service = null;
            try
            {
                string endPointUrl = RightNowConfigService.GetConfigValue(RightNowConfigKeyNames.ToaCapacityServiceUrl);
                service = new CapacityService();
                service._toaCapacityInterface = new toa_capacity_interface(endPointUrl);
                service._log = ToaLogService.GetLog();
            }
            catch (Exception e)
            {
                service = null;
                service._log.Error("Unable to create CapacityService Object");
                service._log.Error("Exception: " + e.StackTrace);
                MessageBox.Show(ToaExceptionMessages.CapacityServiceNotInitialized, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return service;
        }
    }
}