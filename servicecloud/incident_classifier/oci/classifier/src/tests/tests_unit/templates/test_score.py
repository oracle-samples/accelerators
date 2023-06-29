################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:25 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 57e73cb0dc21a24c699bfcdb59343eb086076c96 $
################################################################################################
#  File: test_score.py
################################################################################################
import os.path
import pickle
from os.path import dirname, abspath

import cloudpickle
import pytest

from ai4service_automated_classification.ml.DummyModel import DummyModel
from unittest.mock import mock_open, patch


@pytest.fixture(scope="function")
def model_fixture(monkeypatch):
    import os
    import spacy

    def mock_listdir(file):
        return ['templates.pkl']

    def mock_spacy_load(modelname):
        return None

    monkeypatch.setattr(os, "listdir", mock_listdir)
    monkeypatch.setattr(spacy, "load", mock_spacy_load)


def test_predict(data_request_payload, model_fixture):
    read_data = pickle.dumps(DummyModel())
    mockOpen = mock_open(read_data=read_data)
    with patch('builtins.open', mockOpen):
        from templates.score import predict
        response = predict(data_request_payload)
        assert 'prediction' in response


def test_predict_empty(data_request_empty_payload, model_fixture):
    base_dir = dirname(dirname(dirname(abspath(__file__))))
    with open(os.path.join(base_dir,'resources/templates.pkl'), 'rb') as file:
        model = cloudpickle.load(file)
        read_data = pickle.dumps(model)
        mockOpen = mock_open(read_data=read_data)
        with patch('builtins.open', mockOpen):
            from templates.score import predict
            response = predict(data_request_empty_payload)
            assert 'error' in response


def test_predict_stop_word(data_request_stop_word_payload, model_fixture):
    read_data = pickle.dumps(DummyModel())
    mockOpen = mock_open(read_data=read_data)
    with patch('builtins.open', mockOpen):
        from templates.score import predict
        response = predict(data_request_stop_word_payload)
        assert 'prediction' in response


def test_predict_null(data_request_null_payload, model_fixture):
    read_data = pickle.dumps(DummyModel())
    mockOpen = mock_open(read_data=read_data)
    with patch('builtins.open', mockOpen):
        from templates.score import predict
        response = predict(data_request_null_payload)
        assert 'prediction' in response


def test_predict_null_inquiry(data_request_null_inquiry_payload, model_fixture):
    read_data = pickle.dumps(DummyModel())
    mockOpen = mock_open(read_data=read_data)
    with patch('builtins.open', mockOpen):
        from templates.score import predict
        response = predict(data_request_null_inquiry_payload)
        assert 'prediction' in response
