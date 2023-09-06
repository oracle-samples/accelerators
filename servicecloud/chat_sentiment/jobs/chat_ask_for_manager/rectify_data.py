################################################################################################
#  This file is part of the Oracle B2C Service Accelerator Reference Integration set published
#  by Oracle B2C Service licensed under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23C (August 2023) 
#  date: Tue Aug 22 11:57:47 IST 2023
 
#  revision: RNW-23C
#  SHA1: $Id: 603c014b4760ceb852f0167eeb5f5e39f72529cd $
################################################################################################
#  File: rectify_data.py
################################################################################################

import ast
import logging
import os
import pandas as pd

from io import BytesIO
from datetime import datetime

from oci.object_storage.models import CreateBucketDetails

from common.constants import LOGGING_FORMAT, MAIN_SOURCE_BUCKET_LOCATION, \
    TARGET_COLUMNS, FEATURE_COLUMNS, LIMIT_200K, \
    LANGUAGE_BUCKET_LOCATION, TEXT_COLUMN, LABEL_COLUMN, MINIMUM_SAMPLES_PER_TARGET, COMPARTMENT_ID
from common.storage import get_object_storage_client, get_csv_file_names, get_bucket_details, is_bucket_exists
from sklearn.utils import shuffle

from common.utils import upload_rectified_data

logging.basicConfig(level=logging.INFO, format=LOGGING_FORMAT)
logger = logging.getLogger(os.path.basename(__file__))

DEFAULT_ENCODING = 'utf-8'

LANGUAGE_TRAINING_COLUMNS = [TEXT_COLUMN, LABEL_COLUMN]


def prepare_for_language_dataframe(df, features, target):
    df = df.dropna(subset=[target])
    df[features] = df[features].fillna('')
    if len(df) > 0:
        df[TEXT_COLUMN] = df[features].astype(str).apply(lambda x: ' '.join(x), axis=1)
        df[TEXT_COLUMN] = df[TEXT_COLUMN].apply(lambda x: x.replace("\u00A0", " ").encode('latin-1', errors='replace')
                                                .decode("utf-8", errors="ignore"))
        df = df[df[TEXT_COLUMN].agg(len) > 2]  # At least two chars should be available
    else:
        df = pd.DataFrame(columns=LANGUAGE_TRAINING_COLUMNS)
    return df[LANGUAGE_TRAINING_COLUMNS]


def rectify():
    logger.info(f"*** Scheduled the job at {datetime.now()} ***")
    client = get_object_storage_client()
    minumum_samples_per_target = int(os.getenv(MINIMUM_SAMPLES_PER_TARGET, 20))
    bucket_location = os.getenv(MAIN_SOURCE_BUCKET_LOCATION)
    language_bucket_location = os.getenv(LANGUAGE_BUCKET_LOCATION)
    lang_bucket_name, lang_folder_name = get_bucket_details(language_bucket_location)
    if not is_bucket_exists(lang_bucket_name, client):
        namespace = client.get_namespace(compartment_id=os.getenv(COMPARTMENT_ID)).data
        client.create_bucket(namespace_name=namespace,
                             create_bucket_details=CreateBucketDetails(name=lang_bucket_name,
                                                                       compartment_id=os.getenv(COMPARTMENT_ID)))

    feature_columns = ast.literal_eval(os.getenv(FEATURE_COLUMNS, "[]"))
    target_columns = ast.literal_eval(os.getenv(TARGET_COLUMNS, "[]"))
    columns_to_use = feature_columns + target_columns
    bucket_name, folder_name = get_bucket_details(bucket_location)
    namespace, recent_files = get_csv_file_names(bucket_name=bucket_name, prefix=folder_name, client=client)
    sorted_file_paths = sorted(recent_files, key=lambda x: x.split('_')[-1], reverse=True)
    target_df = pd.DataFrame(columns=LANGUAGE_TRAINING_COLUMNS)
    chunk_data = [target_df]
    for object_name in sorted_file_paths:
        content_data = client.get_object(namespace, bucket_name, object_name)
        if content_data.data:
            bytes_stream = BytesIO(content_data.data.content)
            df = pd.read_csv(bytes_stream,
                             header=0,
                             usecols=columns_to_use, encoding="utf-8")
            target_dfs = []
            for target in target_columns:
                target_df = prepare_for_language_dataframe(df.copy(), feature_columns, target)
                target_dfs.append(target_df)
            df = pd.concat(target_dfs).reset_index(drop=True)
            chunk_data.append(df)
        else:
            chunk_data.append(pd.DataFrame(columns=LANGUAGE_TRAINING_COLUMNS))
    df = pd.concat(chunk_data).reset_index(drop=True)
    df = df.drop_duplicates(subset=LANGUAGE_TRAINING_COLUMNS)
    logger.info(f"*** {df.shape[0]} rows fetched ***")
    label_distribution = df[LABEL_COLUMN].value_counts()
    labels_to_keep = label_distribution[label_distribution > minumum_samples_per_target].index.tolist()
    df = df[df[LABEL_COLUMN].isin(labels_to_keep)]
    df_first_200k = df[:LIMIT_200K]
    df_first_200k = shuffle(df_first_200k)
    upload_rectified_data(df_first_200k, language_bucket_location, client)


if __name__ == '__main__':
    rectify()
