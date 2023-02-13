################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:48 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 03531e14ffa3eca2f2c4f933941f02f2f4de37c7 $
################################################################################################
#  File: data_test_build_model.py
################################################################################################
from datetime import datetime as dt

import pytest

from ai4service_automated_classification.constants import (
    CONFIG_MODULE_NAME, TARGET_COLUMNS_TRAIN, FEATURE_COLUMNS_TRAIN, RANDOM_SEED, MODEL_ID, INITIAL_PRODUCT_ID_COLUMN,
    PRODUCT_ID_COLUMN, DATA_COLUMN, PRODUCT_FEATURE_COLUMNS)
from ai4service_automated_classification.ml.train_test_splitter import RandomTrainTestSplitter
from ai4service_automated_classification.ml.pipeline_train_evaluate import get_pipeline, \
    split_train_evaluate, evaluate

'''
@pytest.fixture(scope="session")
def model_trainer():
    """Instantiate a ModelTrainer for the AutomatedClassification App."""
    # incidents, hierarchy = dummy_data
    model_trainer = AutomatedClassificationModelTrainer("random_namespace")

    yield model_trainer
'''


@pytest.fixture(scope="session")
def trained_model(no_disposition_data, model_trainer):
    """Train the templates on dummy data from OCS."""
    incidents, hierarchy = no_disposition_data
    initial_id_column = INITIAL_PRODUCT_ID_COLUMN
    feature_columns = [DATA_COLUMN, initial_id_column]
    target_column = PRODUCT_ID_COLUMN
    pipeline = get_pipeline(hierarchy[target_column], incidents[target_column], initial_id_column, target_column)
    model_fitted, model_metrics, _ = split_train_evaluate(incidents[target_column], pipeline,
                                                          feature_columns, target_column)
    model_trainer.process_data_metrics(data=incidents[target_column])
    trained_model = model_trainer.process_model_metrics(model_fitted, model_metrics, "random_location")
    return trained_model


'''
@pytest.fixture(scope="session")
def model_and_metadata_container(processed_dummy_data):
    """Create a container for templates and metadata."""
    qa_data_sample, hier_data_sample = processed_dummy_data
    p = get_pipeline(hier_data_sample[PRODUCT_ID_COLUMN], qa_data_sample[PRODUCT_ID_COLUMN],
                     INITIAL_PRODUCT_ID_COLUMN, PRODUCT_ID_COLUMN)
    configs = read_app_information(CONFIG_MODULE_NAME)
    application_name = configs['APPLICATION']
    application_version = configs['APP_VERSION']
    data_uri = OcsPath
    model_config = read_app_information(config_module_name=CONFIG_MODULE_NAME, config_section='templates')
    model_path = model_config['MODEL_PATH']
    model_id = MODEL_ID
    model_metadata = TrainedModelMetadata(
        app=application_name,
        model_version=application_version,
        time_trained=str(dt.now()),
        train_data_location=data_uri.ocs_path,
        model_location=model_path,
        target_columns=TARGET_COLUMNS_TRAIN,
        features_used=FEATURE_COLUMNS_TRAIN,
        model_id=model_id)
    model_container = ModelAndMetadata(templates=p, metadata=model_metadata)
    return model_container
'''


@pytest.fixture(scope="session")
def fixture_model_trainer_evaluate(trained_model, no_disposition_data):
    """Calculate templates metric scores."""
    qa_data_sample, _ = no_disposition_data
    train_test_splitter = RandomTrainTestSplitter(target_columns=[PRODUCT_ID_COLUMN],
                                                  random_seed=RANDOM_SEED,
                                                  stratify_targets=False)
    _, x_test, _, y_test = train_test_splitter.split(qa_data_sample[PRODUCT_ID_COLUMN])
    model_metric_scores = evaluate(trained_model.model, x_test, y_test.values, PRODUCT_FEATURE_COLUMNS,
                                   PRODUCT_ID_COLUMN)

    return model_metric_scores
