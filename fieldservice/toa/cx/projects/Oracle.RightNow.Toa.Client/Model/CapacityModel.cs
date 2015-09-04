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
 *  date: Mon Aug 24 09:01:17 PDT 2015

 *  revision: rnw-15-11-fixes-release-01
*  SHA1: $Id: b64fb25589190273b7a5460c3c223030b9a1261e $
* *********************************************************************************************
*  File: CapacityModel.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oracle.RightNow.Toa.Client.Model
{
    public class CapacityModel : ToaModel
    {
        private DateTime[] quotaDates;
        private string[] categories;
        private string[] resourceIds;
        private string[] timeslots;
        private bool aggregateResults;
        private bool aggregateResultsSpecified;
        private bool calculateTotals;
        private bool calculateTotalsSpecified;
        private bool determineLocationByWorkZone;
        private bool determineLocationByWorkZoneSpecified;
        private Dictionary<DateTime, DayQuota> dayQuota;

        public Dictionary<DateTime, DayQuota> DayQuota
        {
            get { return dayQuota; }
            set { dayQuota = value; }
        }
        public bool DetermineLocationByWorkZoneSpecified
        {
            get { return determineLocationByWorkZoneSpecified; }
            set { determineLocationByWorkZoneSpecified = value; }
        }
        private string[] dayQuotaField;
        private string[] workzoneQuotaField;
        private string[] timeSlotQuotaField;
        private string[] categoryQuotaField;
        private ActivityField[] activityField;
        private Capacity[] capacityField;
        private TimeSlotInfo[] timeSlotInfo;

        public TimeSlotInfo[] TimeSlotInfo
        {
            get { return timeSlotInfo; }
            set { timeSlotInfo = value; }
        }
        private string[] location;

        public Capacity[] CapacityField
        {
            get { return capacityField; }
            set { capacityField = value; }
        }

        private bool calculateDuration;
        private bool calculateDurationSpecified;

        public bool CalculateDurationSpecified
        {
            get { return calculateDurationSpecified; }
            set { calculateDurationSpecified = value; }
        }
        private bool calculateTravelTime;
        private bool calculateTravelTimeSpecified;

        public bool CalculateTravelTimeSpecified
        {
            get { return calculateTravelTimeSpecified; }
            set { calculateTravelTimeSpecified = value; }
        }

        private bool calculateWorkSkill;
        private bool calculateWorkSkillSpecified;

        public bool CalculateWorkSkillSpecified
        {
            get { return calculateWorkSkillSpecified; }
            set { calculateWorkSkillSpecified = value; }
        }
        private bool returnTimeSlotInfo;
        private bool returnTimeSlotInfoSpecified;

        public bool ReturnTimeSlotInfoSpecified
        {
            get { return returnTimeSlotInfoSpecified; }
            set { returnTimeSlotInfoSpecified = value; }
        }
        private bool dontAggregateResults;

        private bool dontAggregateResultsSpecified;

        public bool DontAggregateResultsSpecified
        {
            get { return dontAggregateResultsSpecified; }
            set { dontAggregateResultsSpecified = value; }
        }
        private int minTimeEndOfTimeSlot;
        private bool minTimeEndOfTimeSlotSpecified;

        public bool MinTimeEndOfTimeSlotSpecified
        {
            get { return minTimeEndOfTimeSlotSpecified; }
            set { minTimeEndOfTimeSlotSpecified = value; }
        }
        private int defaultDuration;
        private bool defaultDurationSpecified;

        public bool DefaultDurationSpecified
        {
            get { return defaultDurationSpecified; }
            set { defaultDurationSpecified = value; }
        }
        private string[] workSkill;
        private long activityTravelTime;

        public long ActivityTravelTime
        {
            get { return activityTravelTime; }
            set { activityTravelTime = value; }
        }

        public bool CalculateDuration
        {
            get { return calculateDuration; }
            set { calculateDuration = value; }
        }

        public bool CalculateTravelTime
        {
            get { return calculateTravelTime; }
            set { calculateTravelTime = value; }
        }

        public bool CalculateWorkSkill
        {
            get { return calculateWorkSkill; }
            set { calculateWorkSkill = value; }
        }

        public bool ReturnTimeSlotInfo
        {
            get { return returnTimeSlotInfo; }
            set { returnTimeSlotInfo = value; }
        }

        public bool DontAggregateResults
        {
            get { return dontAggregateResults; }
            set { dontAggregateResults = value; }
        }

        public int MinTimeEndOfTimeSlot
        {
            get { return minTimeEndOfTimeSlot; }
            set { minTimeEndOfTimeSlot = value; }
        }

        public int DefaultDuration
        {
            get { return defaultDuration; }
            set { defaultDuration = value; }
        }

        public string[] WorkSkill
        {
            get { return workSkill; }
            set { workSkill = value; }
        }

        public string[] Location
        {
            get { return location; }
            set { location = value; }
        }

        public bool DetermineLocationByWorkZone
        {
            get { return determineLocationByWorkZone; }
            set { determineLocationByWorkZone = value; }
        }

        public ActivityField[] ActivityField
        {
            get { return activityField; }
            set { activityField = value; }
        }

        public DateTime[] QuotaDates
        {
            get { return quotaDates; }
            set { quotaDates = value; }
        }

        public string[] Categories
        {
            get { return categories; }
            set { categories = value; }
        }

        public string[] ResourceIds
        {
            get { return resourceIds; }
            set { resourceIds = value; }
        }

        public string[] Timeslots
        {
            get { return timeslots; }
            set { timeslots = value; }
        }

        public bool AggregateResults
        {
            get { return aggregateResults; }
            set { aggregateResults = value; }
        }

        public bool AggregateResultsSpecified
        {
            get { return aggregateResultsSpecified; }
            set { aggregateResultsSpecified = value; }
        }

        public bool CalculateTotals
        {
            get { return calculateTotals; }
            set { calculateTotals = value; }
        }

        public bool CalculateTotalsSpecified
        {
            get { return calculateTotalsSpecified; }
            set { calculateTotalsSpecified = value; }
        }

        public string[] DayQuotaField
        {
            get { return dayQuotaField; }
            set { dayQuotaField = value; }
        }

        public string[] TimeSlotQuotaField
        {
            get { return timeSlotQuotaField; }
            set { timeSlotQuotaField = value; }
        }

        public string[] CategoryQuotaField
        {
            get { return categoryQuotaField; }
            set { categoryQuotaField = value; }
        }

        public string[] WorkzoneQuotaField
        {
            get { return workzoneQuotaField; }
            set { workzoneQuotaField = value; }
        }

        
        private long activityDuration;
        private bool activityTravelTimeSpecified;

        public bool ActivityTravelTimeSpecified
        {
            get { return activityTravelTimeSpecified; }
            set { activityTravelTimeSpecified = value; }
        }
        private bool activityDurationSpecified;

        public bool ActivityDurationSpecified
        {
            get { return activityDurationSpecified; }
            set { activityDurationSpecified = value; }
        }

        public long ActivityDuration
        {
            get { return activityDuration; }
            set { activityDuration = value; }
        }

        public activity_field_element[] getActivityFieldElement()
        {
            List<activity_field_element> fieldElement = new List<activity_field_element>();
            if (activityField != null)
            {
                foreach (ActivityField field in activityField)
                {
                    activity_field_element element = new activity_field_element();
                    element.name = field.Name;
                    element.value = field.Value;
                    fieldElement.Add(element);
                }
            }

            return fieldElement.ToArray();
        }

        public capacity_element[] getCapacityElement()
        {
            List<capacity_element> fieldElement = new List<capacity_element>();
            if (CapacityField != null)
            {
                foreach (Capacity field in capacityField)
                {
                    capacity_element element = new capacity_element();
                    element.location = field.Location;
                    element.quota = field.Quota;
                    element.time_slot = field.TimeSlot;
                    element.work_skill = field.WorkSkill;
                    element.available = field.Available;
                    element.date = field.Date;

                    fieldElement.Add(element);
                }
            }

            return fieldElement.ToArray();
        }

        internal time_slot_info_element[] getTimeSlotInfoElement()
        {
            List<time_slot_info_element> fieldElement = new List<time_slot_info_element>();
            if (TimeSlotInfo != null)
            {
                foreach (TimeSlotInfo field in TimeSlotInfo)
                {
                    time_slot_info_element element = new time_slot_info_element();
                    element.name = field.Name;
                    element.label = field.Label;
                    element.time_from = field.TimeFrom;
                    element.time_to = field.TimeTo;
                    element.time_toSpecified = field.TimeToSpecified;
                    element.time_fromSpecified = field.TimeFromSpecified;

                    fieldElement.Add(element);
                }
            }

            return fieldElement.ToArray();
        }
    }

    public class DayQuota
    {
        private DateTime quotaDate;

        public DateTime QuotaDate
        {
            get { return quotaDate; }
            set { quotaDate = value; }
        }

        private float dailyQuota;

        public float DailyQuota
        {
            get { return dailyQuota; }
            set { dailyQuota = value; }
        }

        private Dictionary<string, float> timeSlotQuota;

        public Dictionary<string, float> TimeSlotQuota
        {
            get { return timeSlotQuota; }
            set { timeSlotQuota = value; }
        }
    }

    public class Capacity
    {
        private string location;

        public string Location
        {
            get { return location; }
            set { location = value; }
        }
        private DateTime date;

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }
        private string timeSlot;

        public string TimeSlot
        {
            get { return timeSlot; }
            set { timeSlot = value; }
        }
        private string workSkill;

        public string WorkSkill
        {
            get { return workSkill; }
            set { workSkill = value; }
        }
        private long quota;

        public long Quota
        {
            get { return quota; }
            set { quota = value; }
        }
        private long available;

        public long Available
        {
            get { return available; }
            set { available = value; }
        }
    }

    public class ActivityField
    {
        private string nameField;
        private string valueField;

        public string Name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        public string Value
        {
            get { return valueField; }
            set { valueField = value; }
        }
    }

    public class TimeSlotInfo
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string label;

        public string Label
        {
            get { return label; }
            set { label = value; }
        }

        private DateTime timeFrom;

        public DateTime TimeFrom
        {
            get { return timeFrom; }
            set { timeFrom = value; }
        }

        private bool timeFromSpecified;

        public bool TimeFromSpecified
        {
            get { return timeFromSpecified; }
            set { timeFromSpecified = value; }
        }

        private DateTime timeTo;

        public DateTime TimeTo
        {
            get { return timeTo; }
            set { timeTo = value; }
        }

        private bool timeToSpecified;

        public bool TimeToSpecified
        {
            get { return timeToSpecified; }
            set { timeToSpecified = value; }
        }
    }
}