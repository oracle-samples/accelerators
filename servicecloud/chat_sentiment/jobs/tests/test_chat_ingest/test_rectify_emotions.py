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
#  SHA1: $Id: 59740d708adafdf72b411898687a51ca56b06b4f $
################################################################################################
#  File: test_rectify_emotions.py
################################################################################################
import unittest
from unittest.mock import MagicMock, patch

from chat_ingest_src import rectify_emotions


class TestRectifyChatData(unittest.TestCase):

    @patch('chat_ingest_src.rectify_emotions.get_csv_file_names')
    @patch('chat_ingest_src.rectify_emotions.get_bucket_details')
    @patch('chat_ingest_src.rectify_emotions.get_object_storage_client')
    def test_rectify(self, mock_get_object_storage_client,
                     mock_get_bucket_details, mock_get_csv_file_names):
        mock_objects = ["file1.csv", "file2.csv"]
        mock_get_csv_file_names.return_value = ('test_namespace', mock_objects)

        mock_get_bucket_details.return_value = ('test_bucket', 'test_folder')
        mock_client = MagicMock()

        mock_get_object_storage_client.return_value = mock_client
        mock_get_object_storage_client.get_object.side_effect = [b'col1,col2,labels\ndata1,data2,1\n',
                                                                 b'col1,col2,labels\ndata1,data2,2\n',
                                                                 b'col1,col2,labels\ndata1,data2,3\n']

        with unittest.mock.patch('os.getenv', side_effect=lambda key, opt=None: {
            'MAIN_SOURCE_BUCKET_LOCATION': 'main_bucket',
            'LANGUAGE_BUCKET_LOCATION': 'language_bucket_location',
            'COMPARTMENT_ID': 'compartment_id',
            'MINIMUM_SAMPLES_PER_TARGET': '1',
            'NEUTRAL_CHAT': '10',
            'POSITIVE_CHAT': '20',
            'NEGATIVE_CHAT': '30',
            'ADD_ON_PERCENTAGE_FOR_POSITIVE_SAMPLE': '10',
            'ADD_ON_PERCENTAGE_FOR_NEUTRAL_SAMPLE': '10'
        }[key]):
            # Mock CSV file data
            csv_data = b'Chat ID,text,labels,emotion,score\n' \
                       b'1,Text 1,0,neutral,0.5\n' \
                       b'2,Text 2,1,happy,0.8\n' \
                       b'3,Text 1,0,neutral,0.5\n' \
                       b'4,Text 2,1,happy,0.8\n' \
                       b'5,Text 1,0,neutral,0.5\n' \
                       b'6,Text 2,1,happy,0.8\n' \
                       b'7,Text 1,0,neutral,0.5\n' \
                       b'8,Text 2,2,negative,0.8\n' \
                       b'9,Text 1,0,neutral,0.5\n' \
                       b'10,Text 2,1,happy,0.8\n'
            mock_data = MagicMock()
            mock_data.data.content = csv_data
            mock_client.get_object.return_value = mock_data

            mock_upload_rectified_data = MagicMock()
            with patch('chat_ingest_src.rectify_emotions.upload_rectified_data', mock_upload_rectified_data):
                rectify_emotions.rectify()

                mock_get_object_storage_client.assert_called_once()
                self.assertEqual(mock_client.get_object.call_count,2)
                mock_upload_rectified_data.assert_called_once()


if __name__ == '__main__':
    unittest.main()
