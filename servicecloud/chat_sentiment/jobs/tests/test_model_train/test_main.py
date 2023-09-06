################################################################################################
#  This file is part of the Oracle B2C Service Accelerator Reference Integration set published
#  by Oracle B2C Service licensed under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23C (August 2023) 
#  date: Tue Aug 22 11:57:50 IST 2023
 
#  revision: RNW-23C
#  SHA1: $Id: e1ec62352256e0c96cd625f044c7653c0c118445 $
################################################################################################
#  File: test_main.py
################################################################################################
import unittest
from unittest.mock import Mock, patch
from chat_model_train_src.main import run


class TestMainScript(unittest.TestCase):

    @patch("chat_model_train_src.main.uuid4", return_value="1111-2222-3333-4444")
    @patch("chat_model_train_src.main.get_object_list")
    @patch("chat_model_train_src.main.create_project")
    @patch("chat_model_train_src.main.create_language_model")
    @patch("chat_model_train_src.main.create_language_endpoint")
    @patch("chat_model_train_src.main.get_linked_model_endpoints")
    @patch("chat_model_train_src.main.get_active_models")
    @patch("chat_model_train_src.main.delete_model")
    @patch("chat_model_train_src.main.get_language_client")
    def test_run(self, mock_get_language_client, mock_delete_model, mock_get_active_models,
                 mock_get_linked_model_endpoints, mock_create_language_endpoint,
                 mock_create_language_model, mock_create_project, mock_get_object_list, mock_uuid):
        mock_language_client = Mock()
        mock_bucket_response = ("test_bucket", "test_namespace", ["file1", "file2"])
        mock_create_project_response = Mock(data=Mock(id="mock_project_id"))
        mock_create_language_model_response = Mock(data=Mock(id="mock_model_id"))
        mock_create_language_endpoint_response = Mock(data=Mock(id="mock_endpoint_id"))
        mock_get_linked_model_endpoints_response = Mock(data=Mock(items=[Mock(id="endpoint1", model_id="model1")]))

        mock_get_language_client.return_value = mock_language_client
        mock_get_object_list.return_value = mock_bucket_response
        mock_create_project.return_value = "mock_project_id"
        mock_create_language_model.return_value = "mock_model_id"
        mock_create_language_endpoint.return_value = mock_create_language_endpoint_response
        mock_get_linked_model_endpoints.return_value = mock_get_linked_model_endpoints_response
        mock_get_active_models.return_value = ["model1", "model2"]

        mock_opc_id = f'rnow-accelerator-{mock_uuid.return_value}'

        with unittest.mock.patch('os.getenv', side_effect=lambda key, opt=None: {
            "LANGUAGE_BUCKET_LOCATION": "oci://test_bucket",
            "PROJECT_NAME": "test_project",
            "MODEL_ENDPOINT_NAME": "test_endpoint",
            "INFERENCE_UNIT": "2",
            "PROJECT_ID": None,
        }[key]):
            run(is_first_time=True)

            mock_get_object_list.assert_called_once_with("oci://test_bucket")
            mock_create_project.assert_called_once_with(
                client=mock_language_client,
                display_name="test_project",
                opc_request_id=mock_opc_id
            )
            mock_create_language_model.assert_called_once_with(
                mock_create_project_response.data.id,
                "test_namespace",
                "test_bucket",
                ["file1", "file2"],
                mock_language_client,
                mock_opc_id
            )
            mock_create_language_endpoint.assert_called_once_with(
                mock_create_language_model_response.data.id,
                "test_endpoint",
                mock_language_client,
                num_of_inference_unit=2,
                opc_request_id=mock_opc_id
            )
            mock_get_linked_model_endpoints.assert_not_called()
            mock_delete_model.assert_not_called()

    @patch("chat_model_train_src.main.uuid4", return_value="1111-2222-3333-4444")
    @patch("chat_model_train_src.main.get_object_list")
    @patch("chat_model_train_src.main.create_project")
    @patch("chat_model_train_src.main.create_language_model")
    @patch("chat_model_train_src.main.create_language_endpoint")
    @patch("chat_model_train_src.main.get_linked_model_endpoints")
    @patch("chat_model_train_src.main.get_active_models")
    @patch("chat_model_train_src.main.delete_model")
    @patch("chat_model_train_src.main.get_language_client")
    def test_run_with_false(self, mock_get_language_client, mock_delete_model, mock_get_active_models,
                            mock_get_linked_model_endpoints, mock_create_language_endpoint,
                            mock_create_language_model, mock_create_project, mock_get_object_list, mock_uuid):
        mock_language_client = Mock()
        mock_bucket_response = ("test_bucket", "test_namespace", ["file1", "file2"])
        mock_create_project_response = Mock(data=Mock(id="mock_project_id"))
        mock_create_language_endpoint_response = Mock(data=Mock(id="mock_endpoint_id"))
        mock_get_linked_model_endpoints_response = (["endpoint1"], ["model1"])

        mock_get_language_client.return_value = mock_language_client
        mock_get_object_list.return_value = mock_bucket_response
        mock_create_project.return_value = "mock_project_id"
        mock_create_language_model.return_value = "mock_model_id"
        mock_create_language_endpoint.return_value = mock_create_language_endpoint_response
        mock_get_linked_model_endpoints.return_value = mock_get_linked_model_endpoints_response
        mock_get_active_models.return_value = ["model1", "model2", "model3", "model4", "model5", "model6", "model7"]

        mock_opc_id = f'rnow-accelerator-{mock_uuid.return_value}'

        with unittest.mock.patch('os.getenv', side_effect=lambda key, opt=None: {
            "LANGUAGE_BUCKET_LOCATION": "oci://test_bucket",
            "PROJECT_NAME": "test_project",
            "MODEL_ENDPOINT_NAME": "test_endpoint",
            "INFERENCE_UNIT": "2",
            "PROJECT_ID": None,
        }[key]):
            run(is_first_time=False)

            mock_get_object_list.assert_called_once_with("oci://test_bucket")
            mock_create_project.assert_called_once_with(
                client=mock_language_client,
                display_name="test_project",
                opc_request_id=mock_opc_id
            )
            mock_create_language_model.assert_called_once_with(
                mock_create_project_response.data.id,
                "test_namespace",
                "test_bucket",
                ["file1", "file2"],
                mock_language_client,
                mock_opc_id
            )
            mock_create_language_endpoint.assert_not_called()
            mock_get_linked_model_endpoints.assert_called()
            mock_delete_model.assert_called()


if __name__ == "__main__":
    unittest.main()
