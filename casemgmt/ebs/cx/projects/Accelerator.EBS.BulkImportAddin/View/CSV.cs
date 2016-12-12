/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:42 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: a2b171e4caed073ce89f2ac2e8c96003df55b296 $
 * *********************************************************************************************
 *  File: CSV.cs
 * *********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Accelerator.EBS.BulkImportAddin
{
    public sealed class CSV : IDisposable
    {
        // Defines the name of the data - mostly cosmetic
        private string dataName = "";

        // Characters for processing
        public char charDelimiter      = ',';
        public char charQuote          = '"';
        public char charEscapeQuote    = '\\';

        // Validation results
        const int VALIDATION_OK                           = 0;
        const int VALIDATION_ERROR_WRONG_NUMBER_OF_FIELDS = 2;
        const int VALIDATION_ERROR_UNCLOSED_QUOTES        = 4;
        const int VALIDATION_ERROR_OTHER                  = 5;

        public int TotalRowCount         { get; private set; }
        public int GoodRowCount          { get; private set; }
        public int BadRowCount           { get; private set; }

        // Container for error and duplicate records (useful for dumping them to a separate file for further analysis)
        public List<List<string>> errorRecords     = null;

        // Streams and readers
        private StreamReader reader = null;
        private Stream dataStream = null;

        // Flags
        private bool completed = false;
        private bool hasHeaders = false;

        // First record
        private List<string> firstRecord = null;

        public Encoding encoding;
        public int MaxRecords { get; private set; }

        // Pass in a Stream
        public CSV(Stream dataStream, bool hasHeaders = true, char charDelimiter = ',', char charQuote = '"', char charEscapeQuote = '\\', Encoding fileEncoding = null, int maxRecords = 0, string name = "file")
        {
            // Pass straight to Initialize() method
            this.dataStream = dataStream;
            this.MaxRecords = maxRecords;
            // Set private class properties
            this.charDelimiter = charDelimiter;
            this.charQuote = charQuote;
            this.charEscapeQuote = charEscapeQuote;
            this.hasHeaders = hasHeaders;
            this.dataName = name;

            // Start stream reader
            if (fileEncoding != null)
            {
                this.encoding = fileEncoding;
                this.reader = new StreamReader(this.dataStream, fileEncoding);
            }
            else
            {
                this.encoding = Encoding.UTF8;
                this.reader = new StreamReader(this.dataStream);
            }

            // Pull first record and reset stream
            this.firstRecord = this.ReadRecord();
            this.Reset();
        }

        /// <summary>
        /// If a stream has not been read yet, this simply reads all records from the stream 
        /// </summary>
        private void ReadAllRecords()
        {
            if (!this.completed)
            {
                // Skip first record if need be
                List<string> csvRecord = null;
                if (this.hasHeaders)
                    csvRecord = this.ReadRecord();

                // Loop through entire stream
                while (this.reader.EndOfStream == false)
                {
                    // Get record
                    csvRecord = this.ReadRecord();

                    // Skip blank records
                    if (csvRecord.Count == 0)
                    {
                        continue;
                    }

                    // Perform validation
                    int validationResult = validateRecord(csvRecord);

                    // Increment record counter
                    this.TotalRowCount++;
                    AddRecordToDataTable(csvRecord, validationResult);
                    if (0 != this.MaxRecords && this.GoodRowCount == this.MaxRecords)
                    {
                        break;
                    }
                }
                reader.Close();
                this.completed = true;
            }
        }

        /// <summary>
        /// Resets the data stream and reader
        /// </summary>
        private void Reset()
        {
            // Reset data stream and reader
            this.dataStream.Position = 0;
            this.reader.DiscardBufferedData();

            // Reset completed flag
            this.completed = false;

            // Clear errors
            this.errorRecords = new List<List<string>>();

            // Reset counters
            this.BadRowCount       = 0;
            this.GoodRowCount      = 0;
            this.TotalRowCount     = 0;
        }
      
        // Data storage
        private DataTable csvData = null;

        /// <summary>
        /// Reads all records from a stream 
        /// and populates a new DataTable with the records, and returns the
        /// resulting DataTable.
        /// </summary>
        /// <returns>DataTable containing CSV data</returns>
        public DataTable ToDataTable()
        {
            if (csvData == null)
            {
                // Define DataTable structure
                csvData = new DataTable(dataName);
                for (int i = 0; i < this.firstRecord.Count; i++)
                {
                    DataColumn dc = new DataColumn();
                    dc.ColumnName = (hasHeaders ? this.firstRecord[i] : "" + i);
                    csvData.Columns.Add(dc);
                }
                this.ReadAllRecords();
            }
            return this.csvData;
        }

        /// <summary>
        /// Populate datatable
        /// </summary>
        private void AddRecordToDataTable(List<string> csvRecord, int validationResult)
        {
            // Determine how to process record based on validation result
            switch(validationResult)
            {
                // Validation passed
                case CSV.VALIDATION_OK:
                    this.GoodRowCount++;
                    // Create new DataRow and add to DataTable
                    int i = 0;
                    DataRow dr = this.csvData.NewRow();
                    for (i = 0; i < csvRecord.Count; i++)
                    {
                            dr[i] = csvRecord[i];
                    }
                    this.csvData.Rows.Add(dr);
                    break; 
                default:
                    this.BadRowCount++;
                    break;
            }
        }

        // Validate record
        private int validateRecord(List<string> record)
        {
            if (record.Count != this.firstRecord.Count)
            {
                // Too many or too few fields
                return CSV.VALIDATION_ERROR_WRONG_NUMBER_OF_FIELDS;
            }
            else
            {
                    return CSV.VALIDATION_OK;          
            }
        }

        /// <summary>
        /// Read a single record from the stream using the current encoding and return it as
        /// a List object containing string values.
        /// 
        /// </summary>
        /// <returns></returns>
        private long ReadRecordCount = 0;
        private List<string> ReadRecord()
        {
            ReadRecordCount++;

            // States of the reader
            bool isEscapedChar      = false;
            bool inQuotes           = false;

            // Byte values that will be checked
            int chrCurrent          = 0;
            int chrLast             = 0;
            int chrDelimiter        = (int)this.charDelimiter;
            int chrQuote            = (int)this.charQuote;
            int chrEscapeQuote      = (int)this.charEscapeQuote;
            int chrCarriageReturn   = 13;
            int chrNewLine          = 10;

            // Record container and the field container
            List<string> recordFields = new List<string>();
            StringBuilder fieldValue = new StringBuilder();

            // Line number
            int numLines = 1;
            //int last

            // Multibyte character encoding
            Decoder decoder = this.encoding.GetDecoder();
            char[] chrCompleteCurrent = new char[1];

            while (reader.EndOfStream == false)
            {
                // Get next character
                chrCurrent = this.reader.Read();

                // Handle multibyte characters
                int charCount = decoder.GetChars(new[] { (byte)chrCurrent }, 0, 1, chrCompleteCurrent, 0);
                if (charCount == 0)
                {
                    continue;
                }

                // Check character
                if (chrCurrent == chrQuote)
                {
                    // Special exception - when the escape character is also the quote character, like "I like to say ""Hi!"" to everyone!"
                    if (!isEscapedChar && inQuotes && (chrQuote == chrEscapeQuote) && (chrQuote == reader.Peek()))
                    {
                        // It is - we're dealing with a double-quote escape sequence
                        isEscapedChar = true;
                    }
                    else if (isEscapedChar)
                    {
                        // Append character
                        fieldValue.Append((char)chrCurrent);

                        // Turn off escaping for next chars
                        isEscapedChar = false;
                    }
                    else
                    {
                        // Quoted section beginning or ending - toggle on/off
                        inQuotes = !inQuotes;

                        // If we're ending a quoted value, the next character should be a comma or a line-ending. 
                        // Otherwise, we've just hit the end of a record that was missing an ending quote, so
                        // rewind until we get to the start of the current record.
                        if (!inQuotes)
                        {
                            int nextChar = reader.Peek();
                            if ((nextChar != chrDelimiter) && (nextChar != chrCarriageReturn) && (nextChar != chrNewLine))
                            {
                                // Problem Scenarios
                                // Scenario #1: ABC,"DEF,"GHI"
                                //                     ^^^
                                // Scenario #2: ABC,"DEF","GHI
                                //              JKL,"MNO",PQR
                                //                ^^^

                                // Handle Scenarios by closing quotes
                                if (chrLast == chrDelimiter)
                                {
                                    // Assume the last field didn't close properly. 

                                    // Step 1: Fix last field
                                    string lastFieldValue = fieldValue.ToString();
                                    lastFieldValue = lastFieldValue.TrimEnd((char)chrDelimiter);
                                    recordFields.Add(lastFieldValue);

                                    // Step 2: Start new field (we're resuming quotes)
                                    inQuotes = true;
                                    fieldValue.Clear();
                                }
                            }
                        }
                    }
                }
                else if ((chrCurrent == chrEscapeQuote) && inQuotes)
                {
                    isEscapedChar = true;
                }
                else if (chrCurrent == chrDelimiter)
                {
                    if (inQuotes)
                    {
                        // We're inside quotes, so just append the delimiter
                        fieldValue.Append((char)chrCurrent);
                    }
                    else
                    {
                        // End of field
                        recordFields.Add(fieldValue.ToString());
                        fieldValue.Clear();
                    }
                }
                else if (chrCurrent == chrCarriageReturn)
                {
                    // (DOS-style line endings)

                    // Increment line counter
                    numLines++;

                    // See if the next byte is a newline character, and if so, read it to advance the position
                    bool nextCharIsNewLine = false;
                    if (reader.Peek() == chrNewLine)
                    {
                        nextCharIsNewLine = true;
                        reader.Read();
                    }

                    if (!inQuotes)
                    {
                        // Probable line ending if we aren't in quotes and we've hit a carriage return
                        break;
                    }
                    else
                    {
                        // We're inside quotes, so just append the character
                        fieldValue.Append((char)chrCurrent);
                        if (nextCharIsNewLine)
                        {
                            fieldValue.Append((char)chrNewLine);
                        }
                    }
                }
                else if (chrCurrent == chrNewLine)
                {
                    // (*NIX-style line endings)

                    // Increment line counter
                    numLines++;

                    if (!inQuotes)
                    {
                        // Probable line ending if we aren't in quotes and we've hit a newline
                        break;
                    }
                    else
                    {
                        // We're inside quotes, so just append the character
                        fieldValue.Append((char)chrCurrent);
                    }
                }
                else
                {
                    // Append character
                    fieldValue.Append((char)chrCurrent);
                }

                // Keep track of last character
                chrLast = chrCurrent;
            }

            // Add remaining field
            if ((fieldValue.Length > 0) || (chrLast == chrDelimiter))
            {
                recordFields.Add(fieldValue.ToString());
            }

            // If we find an empty line, skip it and just read the next record
            while ((recordFields.Count == 0) && (reader.EndOfStream != false))
            {
                recordFields = this.ReadRecord();
            }

            return recordFields;
        }

        public void Dispose()
        {
            if (null != csvData) csvData.Dispose();
            if (null !=dataStream) dataStream.Dispose();
            if (null != reader) reader.Dispose();
        }
    }
}
