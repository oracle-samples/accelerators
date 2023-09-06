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
#  SHA1: $Id: f965e894a3840f84fc8674cb94c62a953f2ae9dc $
################################################################################################
#  File: test_language.py
################################################################################################
import unittest
from unittest.mock import Mock, patch
from oci.ai_language.models import Model, Endpoint
from chat_model_train_src.language import (
    define_text_classification_model,
    define_object_storage_dataset,
    define_model_details,
    create_language_model,
    define_language_endpoint,
    create_language_endpoint,
    delete_model,
    get_active_models,
    get_linked_model_endpoints,
)
from common.constants import MODEL_TRAINING_TIMEOUT


class TestLanguageModelUtils(unittest.TestCase):

    def setUp(self):
        self.mock_client = Mock()

    def test_define_text_classification_model(self):
        model = define_text_classification_model()
        self.assertEqual(model.model_type, "TEXT_CLASSIFICATION")
        self.assertEqual(model.language_code, "en")
        self.assertEqual(model.classification_mode.classification_mode, "MULTI_LABEL")

    def test_define_object_storage_dataset(self):
        dataset = define_object_storage_dataset("namespace", "bucket", ["object1", "object2"])
        self.assertEqual(dataset.location_details.namespace_name, "namespace")
        self.assertEqual(dataset.location_details.bucket_name, "bucket")
        self.assertEqual(dataset.location_details.object_names, ["object1", "object2"])

    def test_define_model_details(self):
        with unittest.mock.patch('os.getenv', side_effect=lambda key, opt=None: {
            'COMPARTMENT_ID': 'compartment_id'
        }[key]):
            model_details = define_model_details("project_id", "namespace", "bucket", ["object1", "object2"])
            self.assertEqual(model_details.compartment_id, "compartment_id")
            self.assertEqual(model_details.project_id, "project_id")
            self.assertEqual(model_details.model_details.model_type, "TEXT_CLASSIFICATION")
            self.assertEqual(model_details.training_dataset.location_details.namespace_name, "namespace")
            self.assertEqual(model_details.training_dataset.location_details.bucket_name, "bucket")
            self.assertEqual(model_details.training_dataset.location_details.object_names, ["object1", "object2"])

    @patch("chat_model_train_src.language.wait_for_resource_status")
    def test_create_language_model(self, mock_wait_for_resource_status):
        model_response = Model(id="model_id", lifecycle_state="ACTIVE")

        self.mock_client.create_model.return_value.data = model_response
        mock_wait_for_resource_status.return_value = True  # Assume resource becomes active

        model_id = create_language_model("project_id", "namespace", "bucket", ["object1", "object2"],
                                         client=self.mock_client)

        self.assertEqual(model_id, "model_id")
        self.mock_client.create_model.assert_called_once()
        mock_wait_for_resource_status.assert_called_once_with(
            self.mock_client,
            "model_id",
            "model",
            "ACTIVE",
            opc_request_id="rnow-accelerator",
            timeout=MODEL_TRAINING_TIMEOUT
        )

    @patch("chat_model_train_src.language.wait_for_resource_status")
    def test_create_language_model_timeout(self, mock_wait_for_resource_status):
        model_response = Model(id="model_id", lifecycle_state="CREATING")

        self.mock_client.create_model.return_value.data = model_response
        mock_wait_for_resource_status.side_effect = TimeoutError("Timeout waiting for resource status")

        with self.assertRaises(TimeoutError) as context:
            create_language_model("project_id", "namespace", "bucket", ["object1", "object2"],
                                  client=self.mock_client)

        self.assertEqual(str(context.exception), "Timeout waiting for resource status")

        self.mock_client.create_model.assert_called_once()
        mock_wait_for_resource_status.assert_called_once_with(
            self.mock_client,
            "model_id",
            "model",
            "ACTIVE",
            opc_request_id="rnow-accelerator",
            timeout=MODEL_TRAINING_TIMEOUT
        )

    def test_define_language_endpoint(self):
        with unittest.mock.patch('os.getenv', side_effect=lambda key, opt=None: {
            'COMPARTMENT_ID': 'compartment_id'
        }[key]):
            endpoint = define_language_endpoint("model_id", "endpoint_name", num_of_inference_unit=2)
            self.assertEqual(endpoint.compartment_id, "compartment_id")  # Update with actual value
            self.assertEqual(endpoint.model_id, "model_id")
            self.assertEqual(endpoint.display_name, "endpoint_name")
            self.assertEqual(endpoint.inference_units, 2)

    def test_delete_model(self):
        model_id = "model_id"
        delete_model(model_id, client=self.mock_client)
        self.mock_client.delete_model.assert_called_once_with(model_id=model_id, opc_request_id="rnow-accelerator")

    def test_get_active_models(self):
        with unittest.mock.patch('os.getenv', side_effect=lambda key, opt=None: {
            'COMPARTMENT_ID': 'compartment_id'
        }[key]):
            project_id = "project_id"
            list_model_response = Mock()
            list_model_response.data.items = [Model(id="model1"), Model(id="model2")]
            list_model_response.has_next_page = False
            list_model_response.next_page = None

            self.mock_client.list_models.side_effect = [list_model_response]
            models = get_active_models(project_id, client=self.mock_client)

            self.assertEqual(models, ["model1", "model2"])
            self.mock_client.list_models.assert_called_once_with(
                compartment_id="compartment_id", project_id=project_id,
                sort_by="timeCreated", sort_order="DESC", lifecycle_state="ACTIVE",
                page=None, opc_request_id="rnow-accelerator"
            )

    def test_get_linked_model_endpoints(self):
        with unittest.mock.patch('os.getenv', side_effect=lambda key, opt=None: {
            'COMPARTMENT_ID': 'compartment_id'
        }[key]):
            project_id = "project_id"
            list_model_response = Mock()
            list_model_response.data.items = [Endpoint(id="endpoint1", model_id="model1")]
            list_model_response.has_next_page = False
            list_model_response.next_page = None

            self.mock_client.list_endpoints.side_effect = [list_model_response]

            endpoints, models = get_linked_model_endpoints(project_id, client=self.mock_client)

            self.assertEqual(endpoints, ["endpoint1"])
            self.assertEqual(models, ["model1"])
            self.mock_client.list_endpoints.assert_called_once_with(
                lifecycle_state="ACTIVE", sort_by="timeCreated", sort_order="DESC",
                compartment_id="compartment_id", project_id=project_id,
                page=None, opc_request_id="rnow-accelerator"
            )

    @patch("chat_model_train_src.language.wait_for_resource_status")
    def test_create_language_endpoint(self, mock_wait_for_resource):
        self.mock_client.create_endpoint.return_value.data.id = "endpoint_id"
        endpoint_id = create_language_endpoint("model_id", "endpoint_name", client=self.mock_client,
                                               num_of_inference_unit=2)
        self.assertEqual(endpoint_id, "endpoint_id")
        self.mock_client.create_endpoint.assert_called_once()
        mock_wait_for_resource.assert_called_once()


if __name__ == "__main__":
    unittest.main()
