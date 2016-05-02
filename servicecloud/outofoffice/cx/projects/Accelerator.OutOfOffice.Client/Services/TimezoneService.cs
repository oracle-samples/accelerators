/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC Out of Office Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.5 (May 2016) 
 *  reference: 150916-000080
 *  date: Thu Mar 17 23:37:54 PDT 2016
 
 *  revision: rnw-16-5-fixes-release-1
 *  SHA1: $Id: 0921f62fee79405b7e09494c60b64593508cd99a $
 * *********************************************************************************************
 *  File: TimezoneService.cs
 * ****************************************************************************************** */

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Accelerator.OutOfOffice.Client.Common;
using Accelerator.OutOfOffice.Client.Logs;

namespace Accelerator.OutOfOffice.Client.Services
{
    public class TimezoneService
    {

        public static TimezoneService _timezoneService;
        private static object _sync = new object();
        public static Dictionary<string, string> _timezonemap; 



        private TimezoneService()
        {
            _timezonemap = new Dictionary<string, string>()
            {
                {"America/Chicago","Central Standard Time"},{"America/Denver","Mountain Standard Time"},
                {"America/Los_Angeles","Pacific Standard Time"},{"America/New_York","Eastern Standard Time"},
                {"Asia/Hong_Kong","China Standard Time"},{"Australia/Brisbane","E. Australia Standard Time"},
                {"Pacific/Auckland","New Zealand Standard Time"}, {"Europe/London","GMT Standard Time"},
                {"Etc/GMT+11","UTC-11 "},{"Pacific/Niue","UTC-11 "},
                {"America/Anchorage","Alaskan Standard Time "},{"America/Anguilla","SA Western Standard Time "},
                {"America/Antigua","SA Western Standard Time"},{"America/Araguaina","SA Eastern Standard Time"},
                {"America/Aruba","SA Western Standard Time"},{"America/Asuncion","Paraguay Standard Time "},
                {"America/Barbados","SA Western Standard Time"},{"America/Belem","SA Eastern Standard Time"},
                {"America/Belize","Central America Standard Time"},{"America/Boa_Vista","SA Western Standard Time"},
                {"America/Bogota","SA Pacific Standard Time"},{"America/Boise","Mountain Standard Time"},
                {"America/Buenos_Aires","Argentina Standard Time"},{"America/Cambridge_Bay","Mountain Standard Time"},
                {"America/Cancun","Eastern Standard Time (Mexico) "},{"America/Caracas","Venezuela Standard Time"},
                {"America/Catamarca","Argentina Standard Time"},{"America/Cayenne","SA Eastern Standard Time"},
                {"America/Cayman","SA Pacific Standard Time"},
                {"America/Chihuahua","Mountain Standard Time (Mexico)"},{"America/Cordoba","Argentina Standard Time"},
                {"America/Costa_Rica","Central America Standard Time"},{"America/Cuiaba","Central Brazilian Standard Time"},
                {"America/Curacao","SA Western Standard Time"},{"America/Danmarkshavn","UTC"},
                {"America/Dawson_Creek","US Mountain Standard Time"},{"America/Dawson","Pacific Standard Time"},
                {"America/Detroit","Eastern Standard Time"},
                {"America/Dominica","SA Western Standard Time"},{"America/Edmonton","Mountain Standard Time"},
                {"America/Eirunepe","SA Pacific Standard Time"},{"America/El_Salvador","Central America Standard Time"},
                {"America/Fortaleza","SA Eastern Standard Time"},
                {"America/Glace_Bay","Atlantic Standard Time"},{"America/Goose_Bay","Atlantic Standard Time"},
                {"America/Grand_Turk","SA Western Standard Time"},{"America/Grenada","SA Western Standard Time"},
                {"America/Guadeloupe","SA Western Standard Time"},{"America/Guatemala","Central America Standard Time"},
                {"America/Guayaquil","SA Pacific Standard Time"},{"America/Guyana","SA Western Standard Time"},
                {"America/Halifax","Atlantic Standard Time"},{"America/Havana","Eastern Standard Time"},
                {"America/Hermosillo","US Mountain Standard Time"},{"America/Indiana/Knox","Central Standard Time"},
                {"America/Indiana/Marengo","US Eastern Standard Time"},{"America/Indianapolis","US Eastern Standard Time"},
                {"America/Indiana/Vevay","US Eastern Standard Time"},{"America/Inuvik","Mountain Standard Time"},
                {"America/Iqaluit","Eastern Standard Time"},{"America/Jamaica","SA Pacific Standard Time"},
                {"America/Jujuy","Argentina Standard Time"},{"America/Juneau","Alaskan Standard Time"},
                {"America/Kentucky/Louisville","Eastern Standard Time"},{"America/Kentucky/Monticello","Eastern Standard Time"},
                {"America/Knox_IN","Central Standard Time"},{"America/La_Paz","SA Western Standard Time"},
                {"America/Lima","A Pacific Standard Time"},
                {"America/Louisville","Eastern Standard Time"},{"America/Maceio","SA Eastern Standard Time"},
                {"America/Managua","Central America Standard Time"},{"America/Manaus","SA Western Standard Time"},
                {"America/Martinique","SA Western Standard Time"},{"America/Mazatlan","Mountain Standard Time (Mexico)"},
                {"America/Mendoza","Argentina Standard Time"},{"America/Menominee","Central Standard Time"},
                {"America/Merida","Central Standard Time (Mexico)"},{"America/Mexico_City","Central Standard Time (Mexico)"},
                {"America/Monterrey","Central Standard Time (Mexico)"},{"America/Montevideo","Montevideo Standard Time"},
                {"America/Montreal","Eastern Standard Time"},{"America/Montserrat","SA Western Standard Time"},
                {"America/Nassau","Eastern Standard Time"},
                {"America/Nipigon","Eastern Standard Time"},{"America/Nome","Alaskan Standard Time"},
                {"America/Noronha","UTC-02"},{"America/Panama","SA Pacific Standard Time"},{"America/Pangnirtung","Eastern Standard Time"},
                {"America/Paramaribo","SA Eastern Standard Time"},{"America/Phoenix","US Mountain Standard Time"},
                {"America/Port-au-Prince","Eastern Standard Time"},{"America/Port_of_Spain","SA Western Standard Time"},
                {"America/Porto_Velho","SA Western Standard Time"},{"America/Puerto_Rico","SA Western Standard Time"},
                {"America/Rainy_River","Central Standard Time"},{"America/Rankin_Inlet","Central Standard Time"},
                {"America/Recife","SA Eastern Standard Time"},{"America/Regina","Canada Central Standard Time"},
                {"Acre","SA Pacific Standard Time"},{"America/Rio_Branco","SA Pacific Standard Time"},
                {"America/Santiago","Pacific SA Standard Time"},{"America/Santo_Domingo","SA Western Standard Time"},
                {"America/Sao_Paulo","E. South America Standard Time"},{"America/St_Johns","Newfoundland Standard Time"},
                {"America/St_Kitts","SA Western Standard Time"},{"America/St_Thomas","SA Western Standard Time"},
                {"America/St_Vincent","SA Western Standard Time"},{"America/Swift_Current","Canada Central Standard Time"},
                {"America/Tegucigalpa","Central America Standard Time"},{"America/Thule","Atlantic Standard Time"},
                {"America/Thunder_Bay","Eastern Standard Time"},{"America/Tijuana","Pacific Standard Time"},
                {"America/Tortola","SA Western Standard Time"},{"America/Vancouver","Pacific Standard Time"},
                {"America/Whitehorse","Pacific Standard Time"},{"America/Winnipeg","Central Standard Time"},
                {"America/Yakutat","Alaskan Standard Time"},{"America/Yellowknife","Mountain Standard Time"},
                {"Asia/Aden","Arab Standard Time"},{"Asia/Almaty","Central Asia Standard Time"},{"Asia/Amman","Jordan Standard Time"},
                {"Asia/Aqtau","West Asia Standard Time"},{"Asia/Aqtobe","West Asia Standard Time"},
                {"Asia/Ashgabat","West Asia Standard Time"},{"Asia/Baghdad","Arabic Standard Time"},
                {"Asia/Bahrain","Arab Standard Time"},{"Asia/Bangkok","SE Asia Standard Time"},
                {"Asia/Beirut","Middle East Standard Time"},{"Asia/Bishkek","Central Asia Standard Time"},
                {"Asia/Brunei","Singapore Standard Time"},{"Asia/Calcutta","India Standard Time"},
                {"Asia/Choibalsan","Ulaanbaatar Standard Time"},{"Asia/Colombo","Sri Lanka Standard Time"},
                {"Asia/Dacca","Bangladesh Standard Time"},{"Asia/Damascus","Syria Standard Time"},{"Asia/Dhaka","Bangladesh Standard Time"},
                {"Asia/Dili","Tokyo Standard Time"},{"Asia/Dubai","Arabian Standard Time"},{"Asia/Dushanbe","West Asia Standard Time"},
                {"Asia/Hovd","SE Asia Standard Time"},{"Asia/Irkutsk","North Asia East Standard Time"},
                {"Asia/Jakarta","SE Asia Standard Time"},{"Asia/Jayapura","Tokyo Standard Time"},
                {"Asia/Jerusalem","Israel Standard Time"},{"Asia/Kabul","Afghanistan Standard Time"},{"Asia/Kamchatka","Russia Time Zone 11"},
                {"Asia/Karachi","Pakistan Standard Time"},{"Asia/Katmandu","Nepal Standard Time"},{"Asia/Krasnoyarsk","North Asia Standard Time"},
                {"Asia/Kuala_Lumpur","Singapore Standard Time"},{"Asia/Kuching","Singapore Standard Time"},{"Asia/Kuwait","Arab Standard Time"},
                {"Asia/Macau","China Standard Time"},{"Asia/Magadan","Magadan Standard Time"},
                {"Asia/Makassar","Singapore Standard Time"},{"Asia/Manila","Singapore Standard Time"},{"Asia/Muscat","Arabian Standard Time"},
                {"Asia/Nicosia","GTB Standard Time"},{"Asia/Novosibirsk","N. Central Asia Standard Time"},{"Asia/Omsk","N. Central Asia Standard Time"},
                {"Asia/Phnom_Penh","SE Asia Standard Time"},{"Asia/Pontianak","SE Asia Standard Time"},{"Asia/Pyongyang","Korea Standard Time"},
                {"Asia/Qatar","Arab Standard Time"},{"Asia/Qyzylorda","Central Asia Standard Time"},{"Asia/Rangoon","Myanmar Standard Time"},
                {"Asia/Riyadh","Arab Standard Time"},{"Asia/Saigon","SE Asia Standard Time"},{"Asia/Sakhalin","Vladivostok Standard Time"},
                {"Asia/Samarkand","West Asia Standard Time"},{"Asia/Seoul","Korea Standard Time"},{"Asia/Shanghai","China Standard Time"},
                {"Asia/Singapore","Singapore Standard Time"},{"Asia/Taipei","Taipei Standard Time"},{"Asia/Tashkent","West Asia Standard Time"},
                {"Asia/Tbilisi","Georgian Standard Time"},{"Asia/Tehran","Iran Standard Time"},{"Asia/Thimphu","Bangladesh Standard Time"},
                {"Asia/Tokyo","Tokyo Standard Time"},{"Asia/Ulaanbaatar","Ulaanbaatar Standard Time"},{"Asia/Urumqi","Central Asia Standard Time"},
                {"Asia/Vientiane","SE Asia Standard Time"},{"Asia/Vladivostok","Vladivostok Standard Time"},{"Asia/Yakutsk","Yakutsk Standard Time"},
                {"Asia/Yekaterinburg","Ekaterinburg Standard Time"},{"Asia/Yerevan","Caucasus Standard Time"},
                {"Atlantic/Madeira","GMT Standard Time"},{"Australia/Adelaide","Cen. Australia Standard Time"},
                {"Australia/Darwin","AUS Central Standard Time"},
                {"Tasmania","Tasmania Standard Time"},{"Australia/Hobart","Tasmania Standard Time"},
                {"Australia/Lindeman","E. Australia Standard Time"},{"Australia/Broken_Hill","Cen. Australia Standard Time"},
                {"Australia/Melbourne","AUS Eastern Standard Time"},{"Australia/Perth","W. Australia Standard Time"},
                {"Australia/Sydney","AUS Eastern Standard Time"},{"Australia/Tasmania","Tasmania Standard Time"},
                {"vladivostok Standard Time","Vladivostok Standard Time"},{"Cuba","Eastern Standard Time"},{"Egypt","Egypt Standard Time"},
                {"EST","Eastern Standard Time"},{"Etc/GMT-10","West Pacific Standard Time"},{"Etc/GMT-11","Central Pacific Standard Time"},
                {"Etc/GMT-12","UTC+12"},{"Etc/GMT-13","Tonga Standard Time"},{"Etc/GMT-14","Line Islands Standard Time"},
                {"Etc/GMT-1","W. Central Africa Standard Time"},{"Etc/GMT-2","South Africa Standard Time"},{"Etc/GMT-3","E. Africa Standard Time"},
                {"Etc/GMT-4","Arabian Standard Time"},{"Etc/GMT-5","West Asia Standard Time"},{"Etc/GMT-6","Central Asia Standard Time"},
                {"Etc/GMT-7","SE Asia Standard Time"},{"Etc/GMT-8","Singapore Standard Time"},{"Etc/GMT-9","Tokyo Standard Time"},
                {"Etc/GMT+10","Hawaiian Standard Time"},{"Etc/GMT+12","Dateline Standard Time"},{"Etc/GMT+1","Cape Verde Standard Time"},
                {"Etc/GMT+2","UTC-02"},{"Etc/GMT+3","SA Eastern Standard Time"},{"Etc/GMT+4","SA Western Standard Time"},
                {"Etc/GMT+5","SA Pacific Standard Time"},{"Etc/GMT+6","Central America Standard Time"},{"Etc/GMT+7","US Mountain Standard Time"},
                {"Europe/Amsterdam","W. Europe Standard Time"},{"Europe/Andorra","W. Europe Standard Time"},{"Europe/Athens","GTB Standard Time"},
                {"Europe/Belgrade","Central Europe Standard Time"},{"Europe/Berlin","W. Europe Standard Time"},
                {"Europe/Bratislava","Central Europe Standard Time"},{"Europe/Brussels","Romance Standard Time"},
                {"Europe/Bucharest","E. Europe Standard Time"},{"Europe/Budapest","Central Europe Standard Time"},
                {"Europe/Chisinau","GTB Standard Time"},{"Europe/Copenhagen","Romance Standard Time"},
                {"Europe/Dublin","GMT Standard Time"},{"Europe/Gibraltar","W. Europe Standard Time"},
                {"Europe/Helsinki","FLE Standard Time"},{"Europe/Istanbul","Turkey Standard Time"},
                {"Europe/Kaliningrad","Kaliningrad Standard Time"},{"Europe/Kiev","FLE Standard Time"},
                {"Europe/Lisbon","GMT Standard Time"},{"Europe/Ljubljana","Central Europe Standard Time"},
                {"Europe/Luxembourg","W. Europe Standard Time"},{"Europe/Madrid","Romance Standard Time"},
                {"Europe/Malta","W. Europe Standard Time"},{"Europe/Minsk","Belarus Standard Time"},
                {"Europe/Monaco","W. Europe Standard Time"},{"Europe/Moscow","Russian Standard Time"},
                {"Europe/Nicosia","GTB Standard Time"},{"Europe/Oslo","W. Europe Standard Time"},
                {"Europe/Paris","Romance Standard Time"},{"Europe/Prague","Central Europe Standard Time"},
                {"Europe/Riga","FLE Standard Time"},{"Europe/Rome","W. Europe Standard Time"},
                {"Europe/Samara","Russia Time Zone 3"},{"Europe/San_Marino","W. Europe Standard Time"},
                {"Europe/Sarajevo","Central European Standard Time"},{"Europe/Simferopol","Russian Standard Time"},
                {"Europe/Skopje","Central European Standard Time"},{"Europe/Sofia","FLE Standard Time"},
                {"Europe/Stockholm","W. Europe Standard Time"},{"Europe/Tallinn","FLE Standard Time"},
                {"Europe/Tirane","Central Europe Standard Time"},{"Europe/Uzhgorod","FLE Standard Time"},
                {"Europe/Vaduz","W. Europe Standard Time"},{"Europe/Vatican","W. Europe Standard Time"},
                {"Europe/Vienna","W. Europe Standard Time"},{"Europe/Vilnius","FLE Standard Time"},
                {"Europe/Warsaw","Central European Standard Time"},{"Europe/Zagreb","Central European Standard Time"},
                {"Europe/Zaporozhye","FLE Standard Time"},{"Europe/Zurich","W. Europe Standard Time"},
                {"GB","GMT Standard Time"},{"GMT0","GMT Standard Time"},
                {"GMT-0","GMT Standard Time"},{"GMT+0","GMT Standard Time"},
                {"Greenwich","GMT Standard Time"},{"Hongkong","China Standard Time"},
                {"HST","China Standard Time"},{"Iceland","GMT Standard Time"},
                {"Mexico/BajaSur","Mountain Standard Time"},
                {"Pacific/Apia","Samoa Standard Time"},{"Pacific/Efate","Central Pacific Standard Time"},
                {"Pacific/Fakaofo","Tonga Standard Time"},{"Pacific/Fiji","Fiji Standard Time"},
                {"Pacific/Guam","West Pacific Standard Time"},{"Hawaii","Hawaiian Standard Time"},
                {"Kosrae","Central Pacific Standard Time"},{"Pacific/Kosrae","Central Pacific Standard Time"},
                {"Pacific/Majuro","UTC+12"},{"Pacific/Nauru","UTC+12"},
                {"Pacific/Norfolk","Central Pacific Standard Time"},{"Pacific/Noumea","Central Pacific Standard Time"},
                {"Pacific/Tahiti","Hawaiian Standard Time"},
                {"Pacific/Tarawa","UTC+12"},
                {"Pacific/Truk","West Pacific Standard Time"},
                {"Pacific/Wake","UTC+12"},
                {"US/Alaska","Alaskan Standard Time"},
                {"US/Arizona","US Mountain Standard Time"},
                {"US/Central","Central America Standard Time"},
                {"US/Mountain","US Mountain Standard Time"},
                {"Zulu","GMT Standard Time"},
                {"Singapore","Singapore Standard Time"},
                {"Iran","Iran Standard Time"},
                {"Israel","Israel Standard Time"},
                {"Japan","Tokyo Standard Time"},
                {"Europe/Belfast","GMT Standard Time"},
                {"Etc/Greenwich","GMT Standard Time"},
                {"Europe/Tiraspol","E. Europe Standard Time"},
                {"US/Hawaii","Hawaiian Standard Time"},
                {"US/Michigan","Eastern Standard Time"},
                {"US/Samoa","Samoa Standard Time"},
                {"Portugal","GMT Standard Time"},
                {"Poland","Central European Standard Time"},
                {"Turkey","Turkey Standard Time"},
                {"Victoria","AUS Eastern Standard Time"},
                {"Canada/Saskatchewan","Central Standard Time"},
                {"Canada/Newfoundland","Newfoundland and Labrador Standard Time"},
                {"Canada/Yukon","Pacific Standard Time"},
                {"Australia/Victoria","AUS Eastern Standard Time"},
                {"Australia/Queensland","E. Australia Standard Time"},
                {"America/Adak","Hawaiian Standard Time"},
                {"America/Shiprock","Mountain Standard Time"},
                {"America/St_Lucia","Atlantic Standard Time"},
                {"Asia/Chongqing","China Standard Time"},
                {"Asia/Gaza","Middle East Standard Time"},
                {"Asia/Harbin","China Standard Time"},
                {"Asia/Kashgar","North Asia Standard Time"},
                {"Australia/Canberra","AUS Eastern Standard Time"},
                {"Australia/Lord_Howe","E. Australia Standard Time"}
            };
        }

        public Dictionary<string, string> TimezoneMap
        {
            get { return _timezonemap; }
            set { _timezonemap = value; }
        }

        public string GetWindowsTimezone(string linuxTimezone)
        {
            OSvCLogService.GetLog().Debug("TimezoneService - GetWindowsTimezone() - Entry");

            string windowsTimezone = null;
            if (_timezonemap.ContainsKey(linuxTimezone))
            {

                _timezonemap.TryGetValue(linuxTimezone, out windowsTimezone);
            }
            
            OSvCLogService.GetLog().Debug("TimezoneService - GetWindowsTimezone() - Exit");
            return windowsTimezone;
        }

        public static TimezoneService GetService()
        {
            if (_timezoneService != null)
            {
                return _timezoneService;
            }

            try
            {
                lock (_sync)
                {
                    _timezoneService = new TimezoneService();
                    
                }
            }
            catch (Exception e)
            {
                _timezoneService = null;
                MessageBox.Show(OOOExceptionMessages.TimezoneServiceNotInitialized, Common.Common.ErrorLabel, MessageBoxButtons.OK, MessageBoxIcon.Error);
                OSvCLogService.GetLog().Error(e.Message, e.StackTrace);
            }
            return _timezoneService;
        }
    }
}
