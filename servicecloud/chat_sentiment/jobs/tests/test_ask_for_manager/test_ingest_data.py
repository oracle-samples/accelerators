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
#  SHA1: $Id: 30b52975c04d59e8b380b29a972ed3fa4c27aa72 $
################################################################################################
#  File: test_ingest_data.py
################################################################################################
import unittest
from unittest.mock import patch, Mock
from datetime import datetime

import pandas as pd

from chat_ask_for_manager.ingest_data import call_roql_api, build_and_store_report, find_first_thread
from common.constants import MANAGER_ASK_FEEDBACK_QUERY


class TestChatAskForManagerIngestData(unittest.TestCase):

    @patch('chat_ask_for_manager.ingest_data.invoke_roql_api')
    @patch('chat_ask_for_manager.ingest_data.logger')
    @patch('chat_ask_for_manager.ingest_data.os')
    def test_call_roql_api(self, mock_os, mock_logger, mock_invoke_roql_api):
        mock_record_position = 0
        mock_report_name = 'test_report'
        mock_report_query = 'SELECT * FROM incidents'
        mock_url = 'https://mocked_domain/services/rest/connect/v1.4/queryResults/?query=SELECT * FROM incidents LIMIT 1000 OFFSET 0'
        mock_os.getenv.return_value = 'mocked_domain'
        mock_invoke_response = Mock(status_code=200,
                                    json=Mock(return_value={'items': [{'count': 1, 'rows': [{'data': 'value'}]}]}))
        mock_invoke_roql_api.return_value = mock_invoke_response

        result = call_roql_api(mock_record_position, mock_report_name, mock_report_query)

        mock_os.getenv.assert_called_once_with('CX_DOMAIN')
        mock_logger.info.assert_called_once_with(f"*** Getting Data For {mock_report_name} ***")
        mock_invoke_roql_api.assert_called_once_with(mock_url)
        self.assertEqual(result, mock_invoke_response)

    @patch('chat_ask_for_manager.ingest_data.logger')
    @patch('chat_ask_for_manager.ingest_data.call_roql_api')
    @patch('chat_ask_for_manager.ingest_data.is_bucket_exists')
    @patch('chat_ask_for_manager.ingest_data.get_bucket_details')
    @patch('chat_ask_for_manager.ingest_data.is_directory')
    @patch('chat_ask_for_manager.ingest_data.store_dataframe')
    @patch('chat_ask_for_manager.ingest_data.get_csv_file_names')
    @patch('chat_ask_for_manager.ingest_data.datetime')
    @patch('os.cpu_count', return_value=2)
    @patch('chat_ask_for_manager.ingest_data.pd')
    def test_build_and_store_report_present(self, mock_pd, mock_os, mock_datetime, mock_get_csv_file_names,
                                            mock_store_or_update_csv, mock_is_directory, mock_get_bucket_details,
                                            mock_is_bucket_exists, mock_call_roql_api, mock_logger):
        mock_ocs_client = Mock()
        mock_is_bucket_exists.return_value = True
        mock_is_directory.return_value = True
        mock_response = Mock()
        mock_response.status_code = 200
        mock_response.json.side_effect = [{
            "items": [
                {
                    "count": 5,
                    "rows": [
                        {"column1": "value1", "column2": "value2"},
                        {"column1": "value3", "column2": "value4"},
                        {"column1": "value5", "column2": "value6"},
                        {"column1": "value7", "column2": "value8"},
                        {"column1": "value9", "column2": "value10"}
                    ],
                    "columnNames": ["column1", "column2"]
                }
            ]
        },
            {
                "items": [
                    {
                        "count": 0,
                        "rows": [],
                        "columnNames": ["column1", "column2"]
                    }
                ]
            }
        ]

        mock_call_roql_api.return_value = mock_response
        mock_get_bucket_details.return_value = ('test_bucket', 'test_folder')
        mock_pd.read_csv.return_value = Mock()
        mock_datetime.now.return_value = datetime.fromisoformat('2023-08-14 12:00:00')
        mock_datetime.combine.side_effect = lambda date, time: date
        mock_report_name = 'aia_supervisor_ask'
        mock_report_query = MANAGER_ASK_FEEDBACK_QUERY

        build_and_store_report(mock_report_name, mock_report_query, True, mock_ocs_client,
                               'oci://test_bucket/test_folder')

        mock_pd.read_csv.assert_not_called()
        mock_store_or_update_csv.assert_called_once()
        expected_log = "***  : get data for date 2023-08-13 and found rows 5 ***"
        self.assertEqual(mock_logger.info.call_args_list[0].args[0], expected_log)

    @patch('chat_ask_for_manager.ingest_data.store_dataframe')
    @patch('chat_ask_for_manager.ingest_data.pd')
    def test_build_and_store_report_not_present(self, mock_pd, mock_store_or_update_csv):
        mock_ocs_client = Mock()
        mock_report_name = 'aia_supervisor_ask'
        mock_report_query = MANAGER_ASK_FEEDBACK_QUERY
        build_and_store_report(mock_report_name, mock_report_query, False, mock_ocs_client,
                               'oci://test_bucket/test_folder')

        mock_pd.read_csv.assert_called_once()
        mock_store_or_update_csv.assert_called_once()

    def test_find_first_thread_with_thread_id(self):
        columns = ["Incident ID", "Thread Id", "Text"]
        rows = [
            [1, 1, "Message 1"],
            [1, 2, "Message 2"],
            [2, 1, "Message 3"],
        ]
        expected_rows = [
            [1, 1, "Message 1"],
            [2, 1, "Message 3"],
        ]
        expected_df = pd.DataFrame(expected_rows, columns=columns)

        result_df = find_first_thread(columns, rows)

        self.assertEqual(result_df["Incident ID"].tolist(), expected_df["Incident ID"].tolist())
        self.assertEqual(result_df["Thread Id"].tolist(), expected_df["Thread Id"].tolist())
        self.assertEqual(result_df["Text"].tolist(), expected_df["Text"].tolist())

    def test_find_first_thread_without_thread_id(self):
        columns = ["Incident ID", "Text"]
        rows = [
            [1, "Message 1"],
            [2, "Message 2"],
            [1, "Message 3"],
        ]
        input_df = pd.DataFrame(rows, columns=columns)
        result_df = find_first_thread(columns, rows)
        self.assertEqual(result_df["Incident ID"].tolist(), input_df["Incident ID"].tolist())
        self.assertEqual(result_df["Text"].tolist(), input_df["Text"].tolist())


if __name__ == '__main__':
    unittest.main()
