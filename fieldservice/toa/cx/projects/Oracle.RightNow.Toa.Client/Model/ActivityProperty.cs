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
 *  date: Thu Sep  3 23:14:00 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
*  SHA1: $Id: 89350e2132531133b846d165c6717e72983f8113 $
* *********************************************************************************************
*  File: ActivityProperty.cs
* ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oracle.RightNow.Toa.Client.Model
{
    public static class ActivityProperty
    {
        public static readonly string ApptNumber = "appt_number";
        public static readonly string CustomerNumber = "customer_number";
        public static readonly string Address = "address";
        public static readonly string Cell = "cell";
        public static readonly string City = "city";
        public static readonly string CoordinateX = "coordx";
        public static readonly string CoordinateY = "coordy";
        public static readonly string Email = "email";
        public static readonly string Duration = "duration";
        public static readonly string Language = "language";
        public static readonly string Name = "name";
        public static readonly string Points = "Points";
        public static readonly string Phone = "phone";
        public static readonly string ReminderTime = "reminder_time";
        public static readonly string ServiceWindowStart = "service_window_start";
        public static readonly string ServiceWindowEnd = "service_window_end";
        public static readonly string SlaWindowStart = "sla_window_start";
        public static readonly string SlaWindowEnd = "sla_window_end";
        public static readonly string State = "state";
        public static readonly string TeamId = "team_id";
        public static readonly string TimeSlot = "time_slot";
        public static readonly string TimeZone = "time_zone";
        public static readonly string WorkType = "worktype";
        public static readonly string WorkTypeLabel = "worktype_label";
        public static readonly string Zip = "zip";
        public static readonly string Invsn = "invsn";
        public static readonly string TOA_Report_Success_Code = "0";
        public static readonly string Model = "model";
        public static readonly string InvTypeLabel = "invtype_label";
        public static readonly string ALL_DAY_TIME_SLOT_LABEL = "all-day";
    }
}
