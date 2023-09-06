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
#  SHA1: $Id: 511600c28040871b9bf84c094d402514c85302af $
################################################################################################
#  File: test_chat_status_update.py
################################################################################################
import unittest
from unittest.mock import MagicMock, patch
import json

import pandas as pd

from chat_status_update_src import mark_completed_chat_to_inactive


class TestChatStatusUpdate(unittest.TestCase):

    @patch('chat_status_update_src.mark_completed_chat_to_inactive.get_api_headers')
    @patch('chat_status_update_src.mark_completed_chat_to_inactive.post_call_api')
    def test_get_report_response(self, mock_post_call_api, mock_get_api_headers):
        with unittest.mock.patch('os.getenv', side_effect=lambda key, opt=None: {
            'CX_DOMAIN': 'example.com'
        }[key]):
            mock_get_api_headers.return_value = {}
            mock_response = MagicMock()
            mock_response.status_code = 200
            mock_response.json.return_value = {
                'count': 1,
                'columnNames': ['ChatId', 'Date of Chat Session Completion'],
                'rows': [[123, '2023-08-15T10:00:00']]
            }
            mock_post_call_api.return_value = mock_response

            column_names = []
            rows = []
            payload = '{"lookupName": "GetCompletedChats", "limit": 10000, "offset": 0}'
            mark_completed_chat_to_inactive.get_report_response(column_names, rows, payload)

            # Assert the mock calls and expected behavior
            mock_post_call_api.assert_called_with(payload,
                                                  'https://example.com/services/rest/connect/v1.4/analyticsReportResults')
            self.assertEqual(column_names, ['ChatId', 'Date of Chat Session Completion'])
            self.assertEqual(rows, [[123, '2023-08-15T10:00:00']])

    @patch('chat_status_update_src.mark_completed_chat_to_inactive.get_api_headers')
    @patch('chat_status_update_src.mark_completed_chat_to_inactive.patch_call_api')
    def test_update_parent(self, mock_patch_call_api, mock_get_api_headers):
        with unittest.mock.patch('os.getenv', side_effect=lambda key, opt=None: {
            'CX_DOMAIN': 'example.com'
        }[key]):
            mock_get_api_headers.return_value = {}
            mock_response = MagicMock()
            mock_response.status_code = 200
            mock_patch_call_api.return_value = mock_response

            parent_id = '123'
            completed_date = '2023-08-15T10:00:00'
            mark_completed_chat_to_inactive.update_parent(parent_id, completed_date)

            mock_patch_call_api.assert_called_with(
                json.dumps({"IsActive": False, "CompletedTime": completed_date}),
                f'https://example.com/services/rest/connect/v1.4/AIML.ChatAIResultSummary/{parent_id}'
            )

    @patch('chat_status_update_src.mark_completed_chat_to_inactive.DataFrame')
    @patch('chat_status_update_src.mark_completed_chat_to_inactive.update_parent')
    @patch('chat_status_update_src.mark_completed_chat_to_inactive.get_report_response')
    def test_update_inactive_chats(self, mock_get_report_response, mock_update_parent, mock_dataframe):
        mock_dataframe.side_effect = [pd.DataFrame({
            "ID": ['456'],
            "ChatId": ['123'],
            "Text": ["some chat"]
        }), pd.DataFrame({
            "ID": ['456'],
            "ChatId": ['123'],
            "Text": ["some chat"],
            'Date of Chat Session Completion': '2023-08-15T10:00:00'
        })]

        mock_get_report_response.side_effect = [
            # For active chats
            (
                ['ChatId', 'ID'],
                [['123', '456']]
            ),
            # For completed chats
            (
                ['ChatId', 'Date of Chat Session Completion'],
                [['123', '2023-08-15T10:00:00']]
            )
        ]

        mark_completed_chat_to_inactive.update_inactive_chats()
        mock_update_parent.assert_called_with('456', '2023-08-15T10:00:00')


if __name__ == '__main__':
    unittest.main()
