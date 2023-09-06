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
#  SHA1: $Id: f53b296c8234d002cad72d0f8e31f2e5910ff32b $
################################################################################################
#  File: test_project.py
################################################################################################
import unittest
from unittest.mock import Mock, patch
from oci.ai_language.models import CreateProjectDetails
from chat_model_train_src.project import create_project


class TestProjectCreation(unittest.TestCase):

    def setUp(self):
        self.mock_client = Mock()

    @patch("chat_model_train_src.project.get_language_client")
    @patch("chat_model_train_src.project.os.getenv")
    @patch("chat_model_train_src.project.wait_for_resource_status")
    def test_create_project(self, mock_wait_for_resource_status, mock_getenv, mock_get_language_client):
        mock_getenv.side_effect = lambda x: "mock_compartment_id" if x == "COMPARTMENT_ID" else None

        project_id = "mock_project_id"
        response_data = Mock(id=project_id, LIFECYCLE_STATE_ACTIVE="ACTIVE")
        self.mock_client.create_project.return_value.data = response_data
        mock_get_language_client.return_value = self.mock_client
        mock_wait_for_resource_status.return_value = True

        result = create_project(client=self.mock_client, display_name="test_project")

        self.assertEqual(result, project_id)
        self.mock_client.create_project.assert_called_once_with(
            CreateProjectDetails(display_name="test_project", compartment_id="mock_compartment_id"))
        mock_wait_for_resource_status.assert_called_once_with(
            self.mock_client, project_id, "project", "ACTIVE", opc_request_id="rnow-accelerator", timeout=120
        )


if __name__ == "__main__":
    unittest.main()
