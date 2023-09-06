################################################################################################
#  This file is part of the Oracle B2C Service Accelerator Reference Integration set published
#  by Oracle B2C Service licensed under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23C (August 2023) 
#  date: Tue Aug 22 11:57:49 IST 2023
 
#  revision: RNW-23C
#  SHA1: $Id: 7b30f52cfc4bd4df992c02bb63c1c3cf77c52514 $
################################################################################################
#  File: test_chat_ingest_roql.py
################################################################################################
import unittest
from unittest.mock import patch, Mock
from chat_ingest_src import ingest_from_roql


class TestIngestFromROQLReport(unittest.TestCase):

    @patch('chat_ingest_src.ingest_from_roql.call_roql_api')
    def test_build_roql_data_frame(self, mock_call_roql_api):
        report_name = 'report1'
        report_query = 'SELECT * FROM table WHERE date BETWEEN START_DATE AND END_DATE'
        record_position = 0

        mock_response = Mock()
        mock_response.status_code = 200
        mock_response.json.return_value = {
            'items': [
                {
                    'count': 2,
                    'rows': [{'col1': 'val1', 'col2': 'val2'}],
                    'columnNames': ['col1', 'col2']
                }
            ]
        }
        mock_response2 = Mock()
        mock_response2.status_code = 200
        mock_response2.json.return_value = {
            'items': [
                {
                    'count': 0,
                    'rows': [],
                    'columnNames': ['col1', 'col2']
                }
            ]
        }
        mock_call_roql_api.side_effect = [mock_response, mock_response2]

        result_columns, result_rows = ingest_from_roql.build_roql_data_frame(report_name, '2023-08-14', report_query,
                                                                             record_position)
        self.assertEqual(result_columns, ['col1', 'col2'])
        self.assertEqual(result_rows, [{'col1': 'val1', 'col2': 'val2'}])

    @patch('chat_ingest_src.ingest_from_roql.invoke_roql_api')
    def test_call_roql_api(self, mock_invoke_roql_api):
        record_position = 0
        report_name = 'report1'
        report_query = 'SELECT * FROM table WHERE date BETWEEN START_DATE AND END_DATE'

        mock_response = Mock()
        mock_response.status_code = 200
        mock_invoke_roql_api.return_value = mock_response

        response = ingest_from_roql.call_roql_api(record_position, report_name, report_query)
        self.assertEqual(response, mock_response)

    @patch('chat_ingest_src.ingest_from_roql.invoke_roql_api')
    def test_build_chat_data(self, mock_invoke_roql_api):
        report_name = 'report1'
        report_query = 'SELECT * FROM table WHERE date BETWEEN START_DATE AND END_DATE'
        is_report_present = True

        mock_response = Mock()
        mock_response.status_code = 200
        mock_response.json.return_value = {
            'items': [
                {
                    'count': 1,
                    'rows': [{'col1': 'val1', 'col2': 'val2'}],
                    'columnNames': ['col1', 'col2']
                }
            ]
        }
        mock_response2 = Mock()
        mock_response2.status_code = 200
        mock_response2.json.return_value = {
            'items': [
                {
                    'count': 0,
                    'rows': [],
                    'columnNames': ['col1', 'col2']
                }
            ]
        }

        mock_invoke_roql_api.side_effect = [mock_response, mock_response2]

        result_columns, result_rows = ingest_from_roql.build_chat_data(report_name, report_query, is_report_present)
        self.assertEqual(result_columns, ['col1', 'col2'])
        self.assertEqual(result_rows, [{'col1': 'val1', 'col2': 'val2'}])


if __name__ == '__main__':
    unittest.main()
