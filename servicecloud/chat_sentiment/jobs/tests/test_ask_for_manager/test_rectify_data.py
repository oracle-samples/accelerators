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
#  SHA1: $Id: 738bf672adbea03715df1fe73dd16cadff7391da $
################################################################################################
#  File: test_rectify_data.py
################################################################################################
import unittest
import pandas as pd
from unittest.mock import patch, Mock
from chat_ask_for_manager.rectify_data import prepare_for_language_dataframe, rectify


class TestManagerAskRectifyData(unittest.TestCase):

    def test_prepare_for_language_dataframe(self):
        features = ['feature1', 'feature2']
        target = 'labels'
        data = {
            'feature1': ['foo', 'bar', 'baz'],
            'feature2': ['alpha', 'beta', 'gamma'],
            'feature3': ['apple', 'banana', 'cherry'],
            'labels': ['label1', 'label2', 'label3']
        }
        df = pd.DataFrame(data)

        expected_df = pd.DataFrame({
            "text": ['foo alpha', 'bar beta', 'baz gamma'],
            'labels': ['label1', 'label2', 'label3']
        })

        result_df = prepare_for_language_dataframe(df, features, target)
        self.assertEqual(result_df["text"].tolist(), expected_df["text"].tolist())

    def test_empty_prepare_for_language_dataframe(self):
        features = ['feature1', 'feature2']
        target = 'labels'
        data = {
            'feature1': [],
            'feature2': [],
            'feature3': [],
            'labels': []
        }
        df = pd.DataFrame(data)

        expected_df = pd.DataFrame({
            "text": [],
            'labels': []
        })

        result_df = prepare_for_language_dataframe(df, features, target)
        self.assertEqual(result_df["text"].tolist(), expected_df["text"].tolist())

    @patch('chat_ask_for_manager.rectify_data.upload_rectified_data')
    @patch('chat_ask_for_manager.rectify_data.BytesIO')
    @patch('chat_ask_for_manager.rectify_data.pd.read_csv', return_value=pd.DataFrame({
        "col1": ["1m", "2s", "3ws", "1w", "2s", "3ws", "2s"],
        "col2": ["1m", "2s", "3ws", "1m", "2s", "3ws", "2s"],
        "labels": [1, 2, 3, 1, 2, 3, 2]

    }))
    @patch('chat_ask_for_manager.rectify_data.get_csv_file_names',
           return_value=('test_namespace', ["file1.csv", "file2.csv"]))
    @patch('chat_ask_for_manager.rectify_data.is_bucket_exists', return_value=True)
    @patch('chat_ask_for_manager.rectify_data.get_bucket_details', return_value=('my-bucket', 'my-folder'))
    @patch('chat_ask_for_manager.rectify_data.get_object_storage_client')
    def test_rectify(self, mock_object_storage_client, mock_bucket_details, mock_is_bucket_exists,
                     mock_get_csv_file_names, mock_pd, mock_bytes, mock_upload):
        with unittest.mock.patch('os.getenv', side_effect=lambda key, opt=None: {
            'MINIMUM_SAMPLES_PER_TARGET': '1',
            'MAIN_SOURCE_BUCKET_LOCATION': 'oci://main/data',
            'LANGUAGE_BUCKET_LOCATION': 'oci://lang/data',
            'FEATURE_COLUMNS': '["col1","col2"]',
            'TARGET_COLUMNS': '["labels"]'
        }[key]):
            mock_object_storage_client.return_value = Mock()
            mock_object_storage_client.get_object.side_effect = [b'col1,col2,labels\ndata1,data2,1\n',
                                                                 b'col1,col2,labels\ndata1,data2,2\n',
                                                                 b'col1,col2,labels\ndata1,data2,3\n']
            rectify()
            mock_object_storage_client.assert_called_once()
            self.assertEqual(mock_bucket_details.call_count, 2)
            mock_is_bucket_exists.assert_called_once()
            mock_get_csv_file_names.assert_called_once()
            mock_pd.assert_called()
            self.assertEqual(mock_bytes.call_count, 2)
            mock_upload.assert_called_once()

    @patch('chat_ask_for_manager.rectify_data.upload_rectified_data')
    @patch('chat_ask_for_manager.rectify_data.BytesIO')
    @patch('chat_ask_for_manager.rectify_data.pd.read_csv', return_value=pd.DataFrame({
        "col1": ["1m", "2s", "3ws", "1w", "2s", "3ws", "2s"],
        "col2": ["1m", "2s", "3ws", "1m", "2s", "3ws", "2s"],
        "labels": [1, 2, 3, 1, 2, 3, 2]

    }))
    @patch('chat_ask_for_manager.rectify_data.get_csv_file_names',
           return_value=('test_namespace', ["file1.csv", "file2.csv"]))
    @patch('chat_ask_for_manager.rectify_data.is_bucket_exists', return_value=False)
    @patch('chat_ask_for_manager.rectify_data.get_bucket_details', return_value=('my-bucket', 'my-folder'))
    @patch('chat_ask_for_manager.rectify_data.get_object_storage_client')
    def test_rectify_bucket_not_exist(self, mock_object_storage_client, mock_bucket_details, mock_is_bucket_exists,
                                      mock_get_csv_file_names, mock_pd, mock_bytes, mock_upload):
        with unittest.mock.patch('os.getenv', side_effect=lambda key, opt=None: {
            'COMPARTMENT_ID': 'test_compartment_id',
            'MINIMUM_SAMPLES_PER_TARGET': '1',
            'MAIN_SOURCE_BUCKET_LOCATION': 'oci://main/data',
            'LANGUAGE_BUCKET_LOCATION': 'oci://lang/data',
            'FEATURE_COLUMNS': '["col1","col2"]',
            'TARGET_COLUMNS': '["labels"]'
        }[key]):
            mock_object_storage_client.return_value = Mock()
            mock_object_storage_client.get_object.side_effect = [b'col1,col2,labels\ndata1,data2,1\n',
                                                                 b'col1,col2,labels\ndata1,data2,2\n',
                                                                 b'col1,col2,labels\ndata1,data2,3\n']
            mock_object_storage_client.get_namespace.return_value = Mock(data="namespace")
            mock_object_storage_client.create_bucket.return_value = Mock()
            rectify()
            mock_object_storage_client.assert_called_once()
            self.assertEqual(mock_bucket_details.call_count, 2)
            mock_is_bucket_exists.assert_called_once()
            mock_get_csv_file_names.assert_called_once()
            mock_pd.assert_called()
            self.assertEqual(mock_bytes.call_count, 2)
            mock_upload.assert_called_once()


if __name__ == '__main__':
    unittest.main()
