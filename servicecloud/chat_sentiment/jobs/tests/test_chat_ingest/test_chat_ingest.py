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
#  SHA1: $Id: c091ee73ce8a4541b3813eec2b428384ac7bf8c4 $
################################################################################################
#  File: test_chat_ingest.py
################################################################################################
import unittest
from unittest.mock import patch, Mock
from chat_ingest_src import ingest_from_report


class TestIngestChatFromReport(unittest.TestCase):

    @patch('chat_ingest_src.ingest_from_report.post_call_api')
    def test_build_data_frame(self, mock_post_call_api):
        with unittest.mock.patch('os.getenv', side_effect=lambda key, opt=None: {
            'CX_DOMAIN': 'some_domain',
        }[key]):
            report_name = 'report1'
            column_names = ['col1', 'col2']
            rows = [{'col1': 'val1', 'col2': 'val2'}]
            start_date = '2023-08-01'
            end_date = '2023-08-14'
            record_position = 0

            mock_response = Mock()
            mock_response.status_code = 200
            mock_response.json.return_value = {
                'count': 2,
                'rows': rows,
                'columnNames': column_names
            }

            mock_response2 = Mock()
            mock_response2.status_code = 200
            mock_response2.json.return_value = {
                'count': 0,
                'rows': [],
                'columnNames': column_names
            }

            mock_post_call_api.side_effect = [mock_response, mock_response2]

            result_rows, result_column_names = ingest_from_report.build_data_frame(report_name, column_names, rows,
                                                                                   start_date, end_date,
                                                                                   record_position)
            self.assertEqual(result_rows, rows)
            self.assertEqual(result_column_names, column_names)

    @patch('chat_ingest_src.ingest_from_report.requests.request')
    @patch('chat_ingest_src.ingest_from_report.get_api_headers')
    def test_post_call_api(self, mock_headers, mock_request):
        url = 'https://example.com/api'
        payload = {'key': 'value'}

        mock_response = Mock()
        mock_response.status_code = 200
        mock_request.return_value = mock_response

        response = ingest_from_report.post_call_api(payload, url)
        self.assertEqual(response, mock_response)
        mock_headers.assert_called_once()

    @patch('chat_ingest_src.ingest_from_report.build_data_frame', return_value=([], ['col1', 'col2']))
    def test_populate_rows_from_report(self, mock_build_data_frame):
        with unittest.mock.patch('os.getenv', side_effect=lambda key, opt=None: {
            'INGESTION_JOB_INITIAL_DATA_FETCH_DAYS': '365',
        }[key]):
            result_columns, result_rows = ingest_from_report.populate_rows_from_report("report_name")
            self.assertEqual(result_columns, ['col1', 'col2'])
            self.assertEqual(result_rows, [])


if __name__ == '__main__':
    unittest.main()
