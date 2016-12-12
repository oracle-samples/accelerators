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
 *  date: Thu Sep  3 23:13:59 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
*  SHA1: $Id: 413445f51f7d58c7b305dcb21e2dfbaa0edc6383 $
* *********************************************************************************************
*  File: ToaMD5HashUtil.cs
* ****************************************************************************************** */

using System.Security.Cryptography;
using System.Text;

namespace Oracle.RightNow.Toa.Client.Common
{
    /// <summary>
    /// This class is used for encrypting string using MD5Hashing.
    /// 
    /// </summary>
    public class ToaMD5HashUtil
    {

        /// <summary>
        /// Get encrypted string using MD5Hashing.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMd5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }

        /// <summary>
        /// Get authentication string using MD5Hashing.
        /// </summary>
        /// <param name="utcnow"></param>
        /// <param name="passworkd"></param>
        /// <returns></returns>
        public static string AuthString(string utcnow, string password)
        {
            string authString = GetMd5Hash((utcnow + GetMd5Hash(password)));
            return authString;
        }
    }
}
