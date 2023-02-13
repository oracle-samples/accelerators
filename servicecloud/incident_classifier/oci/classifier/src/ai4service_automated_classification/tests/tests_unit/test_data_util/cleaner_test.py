#!/usr/bin/env python3
################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:50 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 28a3938859d9178a3aceb6c75339f15120731097 $
################################################################################################
#  File: cleaner_test.py
################################################################################################

import datetime
import logging
from unittest.mock import patch

import oci
import pytest
from oci.exceptions import ServiceError

from ai4service_automated_classification.constants import INCIDENT_COLUMN
from ai4service_automated_classification.ml.util.ocs_util import retrieve_files, delete_files
from ai4service_automated_classification.utils.object_storage.os import OcsBucket
from ai4service_automated_classification.utils.object_storage.utils import parse_ocs_uri, FileInfo

retrieve_expected = [
    ((parse_ocs_uri('oci://bucket/ai4service/data/autoclassif/', INCIDENT_COLUMN), datetime.datetime(2021, 6, 25)),
     ["ai4service/data/autoclassif/filename1.csv",
      "ai4service/data/autoclassif/filename2.csv",
      "ai4service/data/autoclassif/filename3.csv",
      "ai4service/data/autoclassif/filename4.csv",
      "ai4service/data/autoclassif/filename5.csv"])
]

delete_expected = [
    (parse_ocs_uri('oci://bucket/ai4service/data/autoclassif/', INCIDENT_COLUMN),
     ["ai4service/data/autoclassif/filename1.csv",
      "ai4service/data/autoclassif/filename2.csv",
      "ai4service/data/autoclassif/filename3.csv",
      "ai4service/data/autoclassif/filename4.csv",
      "ai4service/data/autoclassif/filename5.csv",
      "ai4service/data/autoclassif/filename6.csv",
      "ai4service/data/autoclassif/filename7.csv"]),
    (parse_ocs_uri('oci://bucket/ai4service/data/testing/', INCIDENT_COLUMN),
     ["ai4service/data/testing/filename1.csv",
      "ai4service/data/testing/filename2.csv",
      "ai4service/data/testing/filename3.csv",
      "ai4service/data/testing/filename4.csv",
      "ai4service/data/testing/filename5.csv",
      "ai4service/data/testing/filename6.csv",
      "ai4service/data/testing/filename7.csv"])
]


@pytest.fixture(scope='function')
def mock_ocs_bucket(monkeypatch):
    """
    Mock ocs bucket.

    @param monkeypatch: pytest monkeypatch
    @return: None
    """

    def mock_constructor(self, **kwargs):
        return None

    def mock_obj_storage_client():
        return None

    def mock_delete_obj(self, name):
        return oci.Response(200, None, 'Deleted', oci.Request('POST', 'http://localhost'))

    def mock_files_to_load(self, path):
        return [FileInfo(name=path + "filename1.csv"),
                FileInfo(name=path + "filename2.csv"),
                FileInfo(name=path + "filename3.csv"),
                FileInfo(name=path + "filename4.csv"),
                FileInfo(name=path + "filename5.csv"),
                FileInfo(name=path + "filename6.csv"),
                FileInfo(name=path + "filename7.csv")]

    monkeypatch.setattr(OcsBucket, "__init__", mock_constructor)
    monkeypatch.setattr(OcsBucket, "object_storage", mock_obj_storage_client, raising=False)
    monkeypatch.setattr(OcsBucket, "namespace", "mockNamespace", raising=False)
    monkeypatch.setattr(OcsBucket, "bucket_name", "mockBucket", raising=False)
    monkeypatch.setattr(OcsBucket, "delete_object", mock_delete_obj, raising=False)
    monkeypatch.setattr(OcsBucket, "get_objects_with_prefix", mock_files_to_load)


@pytest.fixture(scope='function')
def mock_retrieve_files(mock_ocs_bucket, monkeypatch):
    def mock_files_to_load(self, path):
        return [FileInfo(name=path + "filename1.csv", time_created=datetime.datetime(2021, 5, 25)),
                FileInfo(name=path + "filename2.csv", time_created=datetime.datetime(2021, 5, 24)),
                FileInfo(name=path + "filename3.csv", time_created=datetime.datetime(2021, 5, 23)),
                FileInfo(name=path + "filename4.csv", time_created=datetime.datetime(2021, 5, 22)),
                FileInfo(name=path + "filename5.csv", time_created=datetime.datetime(2021, 5, 21)),
                FileInfo(name=path + "filename6.csv", time_created=datetime.datetime(2021, 6, 25)),
                FileInfo(name=path + "filename7.csv", time_created=datetime.datetime(2021, 6, 26))]

    monkeypatch.setattr(OcsBucket, "get_objects_with_prefix", mock_files_to_load)


@pytest.mark.parametrize("test_input, expected", retrieve_expected)
def test_retrieve_files_valid(test_input, expected, mock_retrieve_files):
    path, date = test_input
    files = retrieve_files(path.ocs_path, OcsBucket(), date)
    assert len(files) == len(expected)


@pytest.mark.parametrize("test_input, expected", delete_expected)
def test_delete_files_valid(test_input, expected, mock_ocs_bucket, caplog):
    caplog.set_level(logging.DEBUG)
    delete_files(test_input)
    messages = ";;;".join(caplog.messages)
    for i in range(len(expected)):
        assert expected[i] in messages


@pytest.mark.parametrize("test_input, expected", delete_expected)
def test_delete_files_invalid(test_input, expected, mock_ocs_bucket, caplog):
    caplog.set_level(logging.WARNING)
    with patch('ai4service_automated_classification.utils.object_storage.os.OcsBucket.delete_object',
               side_effect=ServiceError(404, 'ObjectNotFound', {'opc-request-id': 1},
                                        'The service returned error code 404')):
        delete_files(test_input)
        messages = ";;;".join(caplog.messages)
        for i in range(len(expected)):
            assert (expected[i] + "' could not succeed.") in messages
