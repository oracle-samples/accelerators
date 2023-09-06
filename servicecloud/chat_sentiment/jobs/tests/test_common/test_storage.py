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
#  SHA1: $Id: 78ff665ee36ad7f65cb2fea3267da84fab0ff343 $
################################################################################################
#  File: test_storage.py
################################################################################################
import unittest
from unittest.mock import patch, Mock

from oci import Response

from common import storage


class TestStorageUtils(unittest.TestCase):

    def test_get_bucket_name(self):
        location = "oci://my-bucket/"
        expected_name = "my-bucket"
        result = storage.get_name("bucket", location)
        self.assertEqual(result, expected_name)

    def test_get_folder_name(self):
        location = "oci://my-bucket/my-folder"
        expected_name = "my-folder"
        result = storage.get_name("folder", location)
        self.assertEqual(result, expected_name)

    def test_invalid_location(self):
        location = "invalid-location"
        with self.assertRaises(AttributeError):
            storage.get_name("bucket", location)

    def test_get_object_storage_client(self):
        with unittest.mock.patch('oci.config.from_file', return_value=Mock()):
            with unittest.mock.patch('oci.object_storage.ObjectStorageClient', return_value=Mock()):
                result = storage.get_object_storage_client()
                self.assertIsNotNone(result)

    def test_get_bucket_details(self):
        bucket_location = "oci://my-bucket/my-folder"
        result = storage.get_bucket_details(bucket_location)
        self.assertEqual(result, ('my-bucket', 'my-folder'))

    @patch('common.storage.get_object_storage_client', return_value='object_storage_client')
    def test_is_directory_true(self, mock_get_object_storage_client):
        bucket_location = "oci://my-bucket/my-folder/"
        mock_get_object_storage_client.get_namespace.return_value = Mock(data="namespace")
        mock_get_object_storage_client.list_objects.return_value = Mock(
            data=Mock(objects=['object1.csv', 'object2.csv']))
        result = storage.is_directory(bucket_location, mock_get_object_storage_client)
        self.assertTrue(result)

    @patch('common.storage.get_bucket_details', return_value=('my-bucket', 'file.csv'))
    @patch('common.storage.get_object_storage_client', return_value='object_storage_client')
    @patch('common.storage.ObjectStorageClient.list_objects', side_effect=Exception("Error"))
    def test_is_directory_false(self, mock_list_objects, mock_get_object_storage_client, mock_get_bucket_details):
        bucket_location = "oci://my-bucket/file.csv"
        mock_get_object_storage_client.get_namespace.return_value = Mock(data="namespace")
        result = storage.is_directory(bucket_location, mock_get_object_storage_client)
        mock_get_bucket_details.assert_called_once_with(bucket_location)
        self.assertFalse(result)

    @patch('common.storage.get_object_storage_client', return_value='object_storage_client')
    @patch('common.storage.get_bucket_details', return_value=('my-bucket', 'my-folder'))
    @patch('common.storage.is_bucket_exists', return_value=True)
    @patch('common.storage.get_csv_file_names', return_value=('namespace', ['file1.csv', 'file2.csv']))
    def test_get_object_list(self, mock_get_csv_file_names, mock_is_bucket_exists, mock_get_bucket_details,
                             mock_get_object_storage_client):
        bucket_location = "oci://my-bucket/my-folder/"
        result = storage.get_object_list(bucket_location)

        mock_get_object_storage_client.assert_called_once()
        mock_get_bucket_details.assert_called_once_with(bucket_location)
        mock_is_bucket_exists.assert_called_once_with('my-bucket', 'object_storage_client')
        mock_get_csv_file_names.assert_called_once_with(bucket_name='my-bucket', prefix='my-folder',
                                                        client='object_storage_client')

        self.assertEqual(result, ('my-bucket', 'namespace', ['file1.csv', 'file2.csv']))

    @patch('common.storage.is_bucket_exists', return_value=True)
    @patch('common.storage.get_csv_file_names', return_value=('namespace', ['file1.csv', 'file2.csv']))
    @patch('common.storage.get_bucket_details', return_value=('my-bucket', 'my-folder'))
    @patch('common.storage.store_or_update_csv')
    @patch('common.storage.oci.object_storage.ObjectStorageClient')
    def test_store_dataframe(self, mock_object_storage_client, mock_store_or_update_csv, mock_get_bucket_details,
                             mock_get_csv_file_names, mock_is_bucket_exists):
        ocs_client = Mock()
        data = 'sample_data'
        storage.store_dataframe(ocs_client, 'oci://my-bucket/my-folder/', 'data', data, num_files_to_keep=2)
        mock_get_csv_file_names.assert_called_once_with(bucket_name='my-bucket', prefix='my-folder', client=ocs_client)
        mock_store_or_update_csv.assert_called_once_with('oci://my-bucket/my-folder/data.csv', data, ocs_client)

    @patch('common.storage.get_object_storage_client', return_value=Mock())
    @patch('common.storage.is_bucket_exists', return_value=True)
    def test_get_csv_file_names(self, mock_is_bucket_exists, mock_get_object_storage_client):
        bucket_name = 'my-bucket'
        prefix = 'my-folder/'
        mock_get_object_storage_client.get_namespace.return_value = Mock(data="namespace")
        file1 = Mock(name="file1.csv")
        file1.name = "file1.csv"
        file2 = Mock(name="file2.csv")
        file2.name = "file2.csv"
        mock_get_object_storage_client.list_objects.side_effect = [Mock(data=Mock(objects=[file1,
                                                                                           file2],
                                                                                  next_start_with=1)),
                                                                   Mock(data=Mock(objects=[],
                                                                                  next_start_with=None))]
        result = storage.get_csv_file_names(bucket_name, prefix, mock_get_object_storage_client)
        mock_is_bucket_exists.assert_called_once_with(bucket_name, mock_get_object_storage_client)
        self.assertEqual(result, ('namespace', ['file1.csv', 'file2.csv']))

    @patch('common.storage.get_object_storage_client', return_value=Mock())
    @patch('common.storage.is_bucket_exists', return_value=True)
    def test_store_csv(self, mock_is_bucket_exists, mock_get_object_storage_client):
        mock_create_bucket_details = mock_get_object_storage_client.return_value.create_bucket
        bucket_location = 'oci://my-bucket/my-folder/'
        filename = 'file.csv'
        data = 'csv,data'
        resonse_to_be = Response(status=201, headers=Mock(), request=Mock(),
                                 data=data)
        mock_get_object_storage_client.put_object.return_value = resonse_to_be

        result = storage.store_csv(bucket_location, filename, data, mock_get_object_storage_client)

        mock_is_bucket_exists.assert_called_once_with('my-bucket', mock_get_object_storage_client)
        mock_create_bucket_details.assert_not_called()  # Not called since the bucket exists
        self.assertEqual(result, resonse_to_be)

    @patch('common.storage.is_bucket_exists', return_value=False)
    @patch('common.storage.get_object_storage_client', return_value=Mock())
    def test_store_csv_when_bucket_not_exist(self, mock_is_bucket_exists, mock_get_object_storage_client):
        mock_get_object_storage_client.get_namespace.return_value = Mock(data="namespace")

        bucket_location = 'oci://my-bucket/my-folder/'
        filename = 'file.csv'
        data = 'csv,data'
        resonse_to_be = Response(status=201, headers=Mock(), request=Mock(),
                                 data=data)
        mock_get_object_storage_client.put_object.return_value = resonse_to_be
        mock_is_bucket_exists.return_value = False
        result = storage.store_csv(bucket_location, filename, data, mock_get_object_storage_client)
        self.assertEqual(result, resonse_to_be)

    @patch('common.storage.get_object_storage_client')
    @patch('common.storage.get_bucket_details')
    @patch('common.storage.is_bucket_exists')
    @patch('common.storage.get_csv_file_names')
    def test_delete_folder(self, mock_get_csv_file_names, mock_is_bucket_exists, mock_get_bucket_details,
                           mock_get_object_storage_client):
        mock_client = Mock()
        mock_get_object_storage_client.return_value = mock_client

        mock_get_bucket_details.return_value = ('test_bucket', 'test_folder')
        mock_is_bucket_exists.return_value = True
        file1 = Mock(name="file1.csv")
        file1.name = "file1.csv"
        file2 = Mock(name="file2.csv")
        file2.name = "file2.csv"

        mock_objects = [file1, file2]
        mock_get_csv_file_names.return_value = ('test_namespace', mock_objects)

        storage.delete_folder('oci://test_bucket/test_folder', mock_client)

        mock_get_bucket_details.assert_called_once_with('oci://test_bucket/test_folder')
        mock_is_bucket_exists.assert_called_once_with('test_bucket', mock_client)
        mock_get_csv_file_names.assert_called_once_with(bucket_name='test_bucket', prefix='test_folder',
                                                        client=mock_client)
        self.assertEqual(mock_client.delete_object.call_count, 2)


if __name__ == '__main__':
    unittest.main()
