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
#  SHA1: $Id: 1fccda5bfa141219a1dbed9575aa41afdcb02a1d $
################################################################################################
#  File: test_utils.py
################################################################################################
import unittest
from unittest.mock import Mock, patch

import pandas as pd

from common.utils import (
    get_language_client,
    data_to_csv,
    wait_for_resource_status,
    upload_rectified_data, get_token, get_vault_client, get_api_headers, create_url, invoke_roql_api
)


class TestUtils(unittest.TestCase):

    def test_get_language_client_with_resource_principal(self):
        with unittest.mock.patch('os.getenv', return_value='some_value'):
            with unittest.mock.patch('oci.auth.signers.get_resource_principals_signer', return_value=Mock()):
                with unittest.mock.patch('oci.ai_language.AIServiceLanguageClient', return_value=Mock()):
                    client = get_language_client()
                    self.assertIsNotNone(client)

    def test_get_language_client_without_resource_principal(self):
        # Mocking the oci.config.from_file function
        with unittest.mock.patch('oci.config.from_file', return_value=Mock()):
            with unittest.mock.patch('oci.ai_language.AIServiceLanguageClient', return_value=Mock()):
                client = get_language_client()
                self.assertIsNotNone(client)

    def test_data_to_csv(self):
        sample_data = {
            'col1': [1, 2, 3],
            'col2': ['a', 'b', 'c']
        }
        df = pd.DataFrame(sample_data)
        csv_string = data_to_csv(df)

        self.assertIn('col1,col2\r\n1,a\r\n2,b\r\n3,c\r\n', csv_string)

    def test_wait_for_resource_status(self):
        mock_client = Mock()
        mock_client.get_job.return_value.data.lifecycle_state = 'IN_PROGRESS'

        # Testing timeout case
        with self.assertRaises(TimeoutError):
            wait_for_resource_status(mock_client, 'job_id', 'Job', 'SUCCEEDED', timeout=5)

    def test_upload_rectified_data(self):
        mock_client = Mock()
        mock_client.put_object.return_value = None

        sample_data = {
            'text': ['abc', 'def', 'ghi'],
            "labels": [1, 2, 3]
        }
        df = pd.DataFrame(sample_data)

        with unittest.mock.patch('os.path.join', return_value='mocked_path'):
            with unittest.mock.patch('common.storage.get_bucket_details', return_value=('my-bucket', 'my-folder')):
                with unittest.mock.patch('common.storage.delete_folder', return_value=None):
                    upload_rectified_data(df, 'oci://my-bucket/my-folder', mock_client)
                    mock_client.put_object.assert_called_once()

    @patch('oci.auth')
    @patch('oci.vault.VaultsClient')
    def test_get_vault_client_with_resource_principal_enabled(self, mock_vault, mock_oci):
        with unittest.mock.patch('os.getenv', side_effect=lambda key: {
            'OCI_RESOURCE_PRINCIPAL_VERSION': 'someversion',
        }[key]):
            mock_signer = Mock()
            mock_oci.signers.get_resource_principals_signer.return_value = mock_signer
            mock_oci.signers.get_resource_principals_signer.side_effect = [
                mock_signer,
                None
            ]
            result = get_vault_client()
            mock_oci.signers.get_resource_principals_signer.assert_called_once()
            self.assertIsNotNone(result)
            self.assertEqual(result, mock_vault({}, signer=mock_signer))

    @patch('oci.config.from_file')
    @patch('oci.vault.VaultsClient')
    def test_get_vault_client_without_resource_principal(self, mock_vault, mock_config):
        result = get_vault_client()
        self.assertIsNotNone(result)
        self.assertEqual(result, mock_vault(mock_config))

    @patch('jwt.encode', return_value="encoded_value")
    def test_get_token_oauth(self, mock_encode):
        mocked_vault = Mock(name="myvault")
        mocked_get_secret = Mock(data=Mock(id="some_id"))
        mocked_vault.get_secret.side_effect = mocked_get_secret
        with unittest.mock.patch('common.utils.get_vault_client',
                                 return_value=mocked_vault):
            mock_secret_client = Mock(name="secret_client")
            mock_secret_client.get_secret_bundle.return_value = Mock(
                data=Mock(secret_bundle_content=Mock(content="YmFzZV82NF9zdHJpbmc=")))
            with unittest.mock.patch('common.utils.get_secret_client',
                                     return_value=mock_secret_client):
                with unittest.mock.patch('os.getenv', side_effect=lambda key: {
                    'AUTH_TYPE': 'OAUTH',
                    'OAUTH_USER': 'test_user',
                    'OAUTH_ENTITY': 'test_entity',
                    'OAUTH_PATH': 'test_path',
                    'JWT_TIME_EXPIRY_IN_MINUTE': '30',
                    'INGESTION_SECRET_ID': 'some_secret_id'
                }[key]):
                    result = get_token()
                    self.assertEqual(result, 'Bearer encoded_value')

    def test_get_token_basic(self):
        mocked_vault = Mock(name="myvault")
        mocked_get_secret = Mock(data=Mock(id="some_id"))
        mocked_vault.get_secret.side_effect = mocked_get_secret
        with unittest.mock.patch('common.utils.get_vault_client',
                                 return_value=mocked_vault):
            mock_secret_client = Mock(name="secret_client")
            mock_secret_client.get_secret_bundle.return_value = Mock(
                data=Mock(secret_bundle_content=Mock(content="YmFzZV82NF9zdHJpbmc=")))
            with unittest.mock.patch('common.utils.get_secret_client',
                                     return_value=mock_secret_client):
                with unittest.mock.patch('os.getenv', side_effect=lambda key: {
                    'AUTH_TYPE': 'BASIC',
                    'INGESTION_SECRET_ID': 'some_secret_id'
                }[key]):
                    result = get_token()
                    self.assertEqual(result, 'Basic YmFzZV82NF9zdHJpbmc=')

    @patch('common.utils.get_api_headers', return_value="headers")
    @patch('common.utils.logger')
    @patch('common.utils.requests')
    def test_invoke_roql_api(self, mock_requests, mock_logger, mock_get_api_headers):
        mock_response = Mock(status_code=200,
                             json=Mock(return_value={'items': [{'count': 1, 'rows': [{'data': 'value'}]}]}))
        mock_requests.request.return_value = mock_response
        result = invoke_roql_api('mocked_url')
        mock_requests.request.assert_called_once_with("GET", 'mocked_url', headers="headers", verify=True)
        self.assertEqual(result, mock_response)

    @patch('common.utils.get_token', return_value="Basic 123=")
    def test_get_api_headers(self, mock_token):
        headers_to_be = {
            'osvc-crest-application-context': f'{"Language Text Classification"}',
            'Authorization': 'Basic 123=',
            'Content-Type': 'application/json'
        }
        headers = get_api_headers()
        self.assertEqual(headers, headers_to_be)

    def test_create_url(self):
        domain = "example.com"
        path = "/api/"
        query_params = "param1=value1&param2=value2"
        expected_url = "https://example.com/api/param1=value1&param2=value2"
        url = create_url(domain, path, query_params)
        self.assertEqual(url, expected_url)


if __name__ == '__main__':
    unittest.main()
