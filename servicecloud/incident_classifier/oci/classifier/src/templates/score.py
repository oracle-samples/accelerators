################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:53 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 8964ec05bf0ad18a8735d57d74b345b65ae0b703 $
################################################################################################
#  File: score.py
################################################################################################
"""
   Inference script. This script is used for prediction by scoring server when schema is known.
"""
import logging
import os
import spacy

from cloudpickle import cloudpickle
from functools import lru_cache
from pandas import DataFrame

from ai4service_automated_classification.utils.data import text_clean
from training_scripts.utils import set_config

model_name = 'templates.pkl'

INQUIRY_COLUMN = "inquiry"
PRODUCT_COLUMN = "product"
CATEGORY_COLUMN = "category"
DISPOSITION_COLUMN = "disposition"
FEATURE_COLUMNS_SERVE = [INQUIRY_COLUMN, PRODUCT_COLUMN, CATEGORY_COLUMN, DISPOSITION_COLUMN]

DATA_COLUMN = "data"
SUBJECT_COLUMN = "subject"
TEXT_COLUMN = "text"
INITIAL_PRODUCT_ID_COLUMN = "initial product"
INITIAL_CATEGORY_ID_COLUMN = "initial category"
INITIAL_DISPOSITION_ID_COLUMN = "initial disposition"
FEATURE_COLUMNS_TRAIN = [DATA_COLUMN, INITIAL_PRODUCT_ID_COLUMN, INITIAL_CATEGORY_ID_COLUMN,
                         INITIAL_DISPOSITION_ID_COLUMN]

HTML_PARSER = 'html5lib'

# logging configuration - OPTIONAL
logging.basicConfig(format='%(name)s - %(levelname)s - %(message)s', level=logging.INFO)
logger_pred = logging.getLogger('templates-prediction')
logger_pred.setLevel(logging.INFO)


@lru_cache(maxsize=10)
def load_model(model_file_name=model_name):
    """
    Loads templates from the serialized format

    @param model_file_name: location of the model as .pkl file
    @return: templates:  a templates instance on which predict API can be invoked

    """
    os.environ['GIT_PYTHON_REFRESH'] = "quiet"
    set_config()
    model_dir = os.path.dirname(os.path.realpath(__file__))
    contents = os.listdir(model_dir)
    if model_file_name in contents:
        logger_pred.info(f'Start loading {model_file_name} from templates directory {model_dir} ...')
        with open(os.path.join(os.path.dirname(os.path.realpath(__file__)), model_file_name), "rb") as file:
            loaded_model = cloudpickle.load(file)

        logger_pred.info("Model is successfully loaded.")
        return loaded_model
    else:
        raise Exception(f'{model_file_name} is not found in templates directory {model_dir}')


@lru_cache(maxsize=50)
def load_spacy_model():
    try:
        return spacy.load("en_core_web_lg")
    except IOError as err:
        logger_pred.error(f"### {str(err)} ###")
        return None


def pre_inference(input_json, nlp=load_spacy_model()):
    """
    Preprocess data

    @param  nlp: language model through spacy API
    @param input_json: Data format as expected by the predict API of the core estimator.
    @return: data: Data format after any processing.

    """
    json_data = input_json.get('jsonData', {})

    feature_available = [f in json_data for f in FEATURE_COLUMNS_SERVE]
    if all(feature_available):
        for initial_target in FEATURE_COLUMNS_SERVE[1:]:
            if json_data[initial_target] is None:
                json_data[initial_target] = 0
        train_dict = {
            train_df_feature: [json_data[input_field]]
            for input_field, train_df_feature in zip(FEATURE_COLUMNS_SERVE, FEATURE_COLUMNS_TRAIN)
        }
        input_df = DataFrame(train_dict, index=[0])
        try:
            input_df[DATA_COLUMN] = input_df[DATA_COLUMN].apply(lambda text: text_clean(text, nlp))
        except Exception as err:
            logger_pred.error(f"Caught Exception during text_clean: {str(err)}")
            logger_pred.debug(f"Data: {input_df[DATA_COLUMN].values[0]}")
        return input_df[FEATURE_COLUMNS_TRAIN]
    else:
        return []


def post_inference(yhat):
    """
    Post-process the templates results

    @param yhat: Data format after calling templates.predict.
    @return: yhat: Data format after any processing.

    """
    return yhat


def predict(data, model=load_model()):
    """
    Returns prediction given the templates and data to predict

    @param  model: Model instance returned by load_model API
    @param  data: Data format as expected by the predict API of the core estimator.
                   For eg. in case of sckit models it could be numpy array/List of list/Pandas DataFrame
    @return: predictions: Output from model
        Format: {'jsonData': output from templates.predict method}

    """
    logger_pred.debug(data)
    features = pre_inference(data)
    if len(features) == 0:
        logger_pred.error(f"Features Length:{len(features) }")
        return {"error": True, "message": "Invalid Input"}
    else:
        try:
            input_data = features[DATA_COLUMN]
            isEmpty = input_data.isna().to_list()[0] or len(input_data.to_list()[0]) == 0
            if isEmpty:
                return {"error": True, "message": "After processing, incident gets empty body"}
            yhat = post_inference(
                model.predict_proba(features)
            )
            logger_pred.info(yhat)
            return {'prediction': yhat}
        except AttributeError as err:
            logger_pred.error(err)
            return {"error": True, "message": f"{err}"}
