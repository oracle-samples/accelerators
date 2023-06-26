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
#  date: Mon Jun 26 10:43:22 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 4da70cf552a5dde469a554d7f0df962bf4229c3e $
################################################################################################
#  File: test_strategy.py
################################################################################################
from datetime import datetime

import oci
import pytest
from oci.object_storage.models import ObjectSummary, ListObjects

from ai4service_automated_classification.ml.util.Strategy import LastIngestionInFolderStrategy
from ai4service_automated_classification.ml.util.ocs_util import CSV_PATTERN
from ai4service_automated_classification.utils.object_storage.os import OcsBucket
from ai4service_automated_classification.utils.object_storage.utils import FileInfo


@pytest.fixture(scope='function')
def mock_ocs_bucket(monkeypatch):
    def mock_constructor(self, **kwargs):
        return None

    def mock_obj_storage_client():
        return None

    def mock_obj_storage_list_objects(arg1, arg2, **kwargs):
        list_objects = ListObjects(objects=[
            ObjectSummary(name=1),
            ObjectSummary(name=2),
            ObjectSummary(name='3'),
            ObjectSummary(name='4'),
            ObjectSummary(name='5'),
            ObjectSummary(name='6'),
            ObjectSummary(name='7')
        ])
        return oci.Response(status='', headers={}, request=None, data=list_objects)

    monkeypatch.setattr(OcsBucket, "__init__", mock_constructor)
    monkeypatch.setattr(OcsBucket, "object_storage", mock_obj_storage_client, raising=False)
    monkeypatch.setattr(OcsBucket, "namespace", "mockNamespace", raising=False)
    monkeypatch.setattr(OcsBucket, "bucket_name", "mockBucket", raising=False)
    monkeypatch.setattr(OcsBucket.object_storage, "list_objects", mock_obj_storage_list_objects, raising=False)


@pytest.fixture
def mock_files_to_load(mock_ocs_bucket, monkeypatch):
    def mock_files_to_load(self, path):
        return [FileInfo(name=path + "filename-20210813024600-1.csv", time_created=datetime(2021, 8, 13, 2, 46, 0)),
                FileInfo(name=path + "filename-20210813024600-2.csv", time_created=datetime(2021, 8, 13, 2, 46, 1)),
                FileInfo(name=path + "filename-20210813024500-1.csv", time_created=datetime(2021, 8, 13, 2, 45, 00)),
                FileInfo(name=path + "filename-20210813024400-1.csv", time_created=datetime(2021, 8, 13, 2, 44, 00)),
                FileInfo(name=path + "filename-20210813024300-1.csv", time_created=datetime(2021, 8, 13, 2, 43, 00)),
                FileInfo(name=path + "filename-20210813024200-1.csv", time_created=datetime(2021, 8, 13, 2, 42, 00)),
                FileInfo(name=path + "filename-20210813024100-1.csv", time_created=datetime(2021, 8, 13, 2, 41, 00))]

    monkeypatch.setattr(OcsBucket, "get_objects_with_prefix", mock_files_to_load)


@pytest.fixture
def mock_files_to_load_exception(mock_ocs_bucket, monkeypatch):
    def mock_files_exception(self, path):
        return [FileInfo(name=path + "filename1.csv", time_created=datetime(2021, 8, 13, 2, 46, 1)),
                FileInfo(name=path + "filename2.csv", time_created=datetime(2021, 8, 13, 2, 46, 2)),
                FileInfo(name=path + "filename-20210813024500-1.csv", time_created=datetime(2021, 8, 13, 2, 45, 00)),
                FileInfo(name=path + "filename-20210813024400-1.csv", time_created=datetime(2021, 8, 13, 2, 44, 00)),
                FileInfo(name=path + "filename-20210813024300-1.csv", time_created=datetime(2021, 8, 13, 2, 43, 00)),
                FileInfo(name=path + "filename-20210813024200-1.csv", time_created=datetime(2021, 8, 13, 2, 42, 00)),
                FileInfo(name=path + "filename-20210813024100-1.csv", time_created=datetime(2021, 8, 13, 2, 41, 00))]

    monkeypatch.setattr(OcsBucket, "get_objects_with_prefix", mock_files_exception)


def test_reading_strategy(mock_files_to_load):
    strategy = LastIngestionInFolderStrategy(OcsBucket(), regex_filter=CSV_PATTERN)
    files_to_load = strategy.get_paths_to_load('testing/here/', return_full_paths=False)
    expected = [
        FileInfo("filename-20210813024600-1.csv", time_created=datetime(2021, 8, 13, 2, 46, 0)),
        FileInfo("filename-20210813024600-2.csv", time_created=datetime(2021, 8, 13, 2, 46, 1))
    ]
    assert expected == files_to_load


def test_reading_strategy_with_exception(mock_files_to_load_exception, caplog):
    strategy = LastIngestionInFolderStrategy(OcsBucket(), regex_filter=CSV_PATTERN)
    with pytest.raises(IndexError) as e:
        strategy.get_paths_to_load('testing/here/', return_full_paths=False)
    assert "Timestamp was not found in the last ingested file name: filename2.csv" in caplog.messages
    assert e.type == IndexError
