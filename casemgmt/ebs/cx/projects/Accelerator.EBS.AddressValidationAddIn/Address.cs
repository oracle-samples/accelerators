/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:41 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: c779d559e1f029db40ec5e8bf90dd478fd0cb1c9 $
 * *********************************************************************************************
 *  File: Address.cs
 * *********************************************************************************************/

using System;
using System.Collections;
using System.Xml;
using System.Text.RegularExpressions;
using System.Linq;

namespace Accelerator.EBS.AddressValidationAddIn
{
    /*
     * see https://www.usps.com/business/web-tools-apis/address-information-api.htm
     * <AddressValidateRequest USERID="XXXXX"> 
     *   <Address ID="1">  
     *     <Address1 />      
     *     <Address2>205 bagwell ave</Address2>       
     *     <City>nutter fort</City>      
     *     <State>wv</State>      
     *     <Zip5></Zip5>       
     *     <Zip4></Zip5>  
     *    </Address>          
     * </AddressValidateRequest>
     */

    [Serializable()]
    public class Address
    {
        // first index is 0, so just put HOLDER there
        private static string[] statesArray = new string[60] {
            "HOLDER",
"AK",
"AL",
"AR",
"AS",
"AZ",
"CA",
"CO",
"CT",
"DC",
"DE",
"FL",
"FM",
"GA",
"GU",
"HI",
"IA",
"ID",
"IL",
"IN",
"KS",
"KY",
"LA",
"MA",
"MD",
"ME",
"MH",
"MI",
"MN",
"MO",
"MP",
"MS",
"MT",
"NC",
"ND",
"NE",
"NH",
"NJ",
"NM",
"NV",
"NY",
"OH",
"OK",
"OR",
"PA",
"PR",
"PW",
"RI",
"SC",
"SD",
"TN",
"TX",
"UT",
"VA",
"VI",
"VT",
"WA",
"WI",
"WV",
"WY"};

        private static Hashtable _reverseStateTable = new Hashtable { 
        { "AK", 1 }, 
        { "AL", 2 },
        { "AR", 3 }, 
        { "AS", 4 },
        { "AZ", 5 }, 
        { "CA", 6 },
        { "CO", 7 }, 
        { "CT", 8 },
        { "DC", 9 }, 
        { "DE", 10 },
        { "FL", 11 }, 
        { "FM", 12 },
        { "GA", 13 }, 
        { "GU", 14 },
        { "HI", 15 }, 
        { "IA", 16 },
        { "ID", 17 }, 
        { "IL", 18 },
        { "IN", 19 }, 
        { "KS", 20 },
        { "KY", 21 }, 
        { "LA", 22 },
        { "MA", 23 }, 
        { "MD", 24 },
        { "ME", 25 }, 
        { "MH", 26 },
        { "MI", 27 },
        { "MN", 28 },
        { "MO", 29 },
        { "MP", 30 },
        { "MS", 31 },
        { "MT", 32 },
        { "NC", 33 },
        { "ND", 34 },
        { "NE", 35 },
        { "NH", 36 },
        { "NJ", 37 },
        { "NM", 38 },
        { "NV", 39 }, 
        { "NY", 40 },
        { "OH", 41 }, 
        { "OK", 42 },
        { "OR", 43 }, 
        { "PA", 44 },
        { "PR", 45 }, 
        { "PW", 46 },
        { "RI", 47 }, 
        { "SC", 48 },
        { "SD", 49 }, 
        { "TN", 50 },
        { "TX", 51 }, 
        { "UT", 52 },
        { "VA", 53 }, 
        { "VI", 54 },
        { "VT", 55 }, 
        { "WA", 56 },
        { "WI", 57 }, 
        { "WV", 58 },
        { "WY", 59 }      
        };

        public Address()
        {
            this._ID = 1;            
        }

        private int _ID;

        // ID of this address, only 1 address to verify so far
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private string _Address1 = "";

        /* Address Line 1 is used to provide an apartment or suite
         * number, if applicable. Maximum characters allowed: 38
         */
        public string Address1
        {
            get { return _Address1; }
            set
            {
                if (value != null && value.Length > 38)
                    throw new Exception("Address1 exceeded maximum of 38 characters.");
                _Address1 = value;
            }
        }

        private string _Address2 = "";

        /* Street address
         * Maximum characters allowed: 38
         */
        public string Address2
        {
            get { return _Address2; }
            set
            {
                if (value != null && value.Length > 38)
                    throw new Exception("Address2 exceeded maximum of 38 characters.");
                _Address2 = value;
            }
        }

        private string _City = "";

        /* City
         * Either the City and State or Zip are required.
         * Maximum characters allowed: 15
         */
        public string City
        {
            get { return _City; }
            set
            {
                if (value != null && !Regex.IsMatch(value, @"^[a-zA-Z\x20]+$"))
                    throw new Exception("Invalid City.");
                if (value != null && value.Length > 15)
                    throw new Exception("City exceeded maximum of 15 characters.");
                _City = value;
            }
        }

        private int _StateID = 0;

        /* StateID
         * stateorProvinceID from RightNow 
         */
        public int StateID 
        {
            get { return _StateID; }
            set
            {
                if (value != 0)
                {
                    _StateID = Convert.ToInt16(value);
                    _State = statesArray[_StateID];
                }
            }
        }

        private string _State = "";

        /* State
         * Either the City and State or Zip are required.
         * Maximum characters allowed = 2
         */
        public string State
        {
            get { return _State; }
            set
            {
                if (value != null && value.Length != 2)
                    throw new Exception("State abbreviation must be 2 characters.");
                _State = value;
            }
        }

        private string _Zip = "";

        /* Zipcode
         * whole Zip5-Zip4 if Zip4 is provided else only Zip5
         */
        public string Zip
        {
            get { return _Zip; }
            set
            {
                if (value != null)
                {
                    if (value.Length == 10 && value.Contains("-"))
                    {
                        _Zip5 = value.Split('-')[0];

                        if (!_Zip5.All(char.IsDigit))
                            throw new Exception("Invalid Zip Code.");

                        _Zip4 = value.Split('-')[1];

                        if (!_Zip4.All(char.IsDigit))
                            throw new Exception("Invalid Zip Code.");

                        _Zip = value;
                    }
                    else if (value.Length == 5 && value.All(char.IsDigit))
                    {
                        _Zip5 = value;
                        _Zip = value;
                    }
                    else
                        throw new Exception("Invalid Zip Code.");
                }
            }
        }

        private string _Zip5 = "";

        /* Zipcode
         * Maximum characters allowed = 5
         */
        public string Zip5
        {
            get { return _Zip5; }
            set
            {
                if (value != null && value.Length > 5)
                    throw new Exception("Zip exceeded maximum of 5 characters.");
                _Zip5 = value;
            }
        }

        private string _Zip4 = "";

        /* Zip code extension
         * Maximum characters allowed = 4
         */
        public string Zip4
        {
            get { return _Zip4; }
            set
            {
                if (value != null && value.Length > 4)
                    throw new Exception("Zip code extension exceeded maximum of 4 characters.");
                _Zip4 = value;
            }
        }

        public string ReturnText;

        // Get an Address object from an xml response.
        public static Address getAddress(string xml)
        {
            Address a = new Address();

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(xml);

            System.Xml.XmlNode element = doc.SelectSingleNode("/AddressValidateResponse/Address/Address1");
            if (element != null)
                a._Address1 = element.InnerText;
            element = doc.SelectSingleNode("/AddressValidateResponse/Address/Address2");
            if (element != null)
                a._Address2 = element.InnerText;
            element = doc.SelectSingleNode("/AddressValidateResponse/Address/City");
            if (element != null)
                a._City = element.InnerText;
            element = doc.SelectSingleNode("/AddressValidateResponse/Address/State");
            if (element != null)
            {
                a._State = element.InnerText;
                a.StateID = (int)_reverseStateTable[a._State];
            }
            element = doc.SelectSingleNode("/AddressValidateResponse/Address/Zip5");
            if (element != null)
                a._Zip5 = element.InnerText;
            element = doc.SelectSingleNode("/AddressValidateResponse/Address/Zip4");
            if (element != null)
                a._Zip4 = element.InnerText;

            a.Zip = a.Zip5 + "-" + a.Zip4;

            if (a._Address1 != null && a._Address2 != null)
                a._Address2 += " " + a._Address1;

            element = doc.SelectSingleNode("/AddressValidateResponse/Address/ReturnText");
            if (element != null)
                a.ReturnText = element.InnerText; 

            return a;
        }
    }
}
