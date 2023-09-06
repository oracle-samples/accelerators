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
#  SHA1: $Id: 61017d49106307e1323af83cc194c5566b30da84 $
################################################################################################
#  File: rectify_emotions.py
################################################################################################

import logging
import os

import pandas as pd
import sys

from io import BytesIO
from datetime import datetime
from os.path import dirname, abspath

from oci.object_storage.models import CreateBucketDetails

from common.constants import LOGGING_FORMAT, MAIN_SOURCE_BUCKET_LOCATION, LIMIT_200K, \
    LANGUAGE_BUCKET_LOCATION, COMPARTMENT_ID, NEUTRAL_CHAT, \
    NEGATIVE_CHAT, POSITIVE_CHAT, ADD_ON_PERCENTAGE_FOR_POSITIVE_SAMPLE, ADD_ON_PERCENTAGE_FOR_NEUTRAL_SAMPLE
from common.storage import get_object_storage_client, get_csv_file_names, get_bucket_details, is_bucket_exists
from sklearn.utils import shuffle

from common.utils import upload_rectified_data

base_dir = dirname(dirname(dirname(abspath(__file__))))
sys.path.append(base_dir)

logging.basicConfig(level=logging.INFO, format=LOGGING_FORMAT)
logger = logging.getLogger(os.path.basename(__file__))


def rectify():
    logger.info(f"*** Scheduled the job at {datetime.now()} ***")
    client = get_object_storage_client()
    bucket_location = os.getenv(MAIN_SOURCE_BUCKET_LOCATION)
    language_bucket_location = os.getenv(LANGUAGE_BUCKET_LOCATION)
    lang_bucket_name, lang_folder_name = get_bucket_details(language_bucket_location)
    if not is_bucket_exists(lang_bucket_name, client):
        namespace = client.get_namespace(compartment_id=os.getenv(COMPARTMENT_ID)).data
        client.create_bucket(namespace_name=namespace,
                             create_bucket_details=CreateBucketDetails(name=lang_bucket_name,
                                                                       compartment_id=os.getenv(COMPARTMENT_ID)))

    columns_to_use = ['text', 'labels', 'emotion', 'score']
    bucket_name, folder_name = get_bucket_details(bucket_location)
    namespace, recent_files = get_csv_file_names(bucket_name=bucket_name, prefix=folder_name, client=client)
    sorted_file_paths = sorted(recent_files, key=lambda x: x.split('_')[-1], reverse=True)
    target_df = pd.DataFrame(columns=columns_to_use)
    chunk_data = [target_df]
    for object_name in sorted_file_paths:
        content_data = client.get_object(namespace, bucket_name, object_name)
        if content_data.data:
            bytes_stream = BytesIO(content_data.data.content)
            df = pd.read_csv(bytes_stream,
                             header=0,
                             usecols=columns_to_use, encoding="utf-8")
            chunk_data.append(df)
        else:
            chunk_data.append(pd.DataFrame(columns=columns_to_use))
    labeled_chat = pd.concat(chunk_data).reset_index(drop=True)

    logger.info(f"*** {labeled_chat.shape[0]} rows fetched ***")

    neutral_chat = labeled_chat.loc[labeled_chat['labels'] == NEUTRAL_CHAT]
    positive_chat = labeled_chat.loc[labeled_chat['labels'] == POSITIVE_CHAT]
    negative_chat = labeled_chat.loc[labeled_chat['labels'] == NEGATIVE_CHAT]
    negative_chat_count = len(negative_chat)
    labeled_chat = pd.concat([negative_chat,
                              positive_chat.head(negative_chat_count +
                                                 int(negative_chat_count *
                                                     int(os.getenv(ADD_ON_PERCENTAGE_FOR_POSITIVE_SAMPLE)) / 100)),
                              neutral_chat.head(negative_chat_count +
                                                int(negative_chat_count *
                                                    int(os.getenv(ADD_ON_PERCENTAGE_FOR_NEUTRAL_SAMPLE)) / 100))])
    labeled_chat = labeled_chat.sample(frac=1).reset_index(drop=True)
    logger.info(f"*** Balanced Data store start for {bucket_location} ***")
    labeled_chat = shuffle(labeled_chat)
    labeled_chat = labeled_chat.drop_duplicates(subset=["text"])
    df_first_200k = labeled_chat[:LIMIT_200K]
    df_first_200k = shuffle(df_first_200k)
    upload_rectified_data(df_first_200k, language_bucket_location, client)


if __name__ == '__main__':
    rectify()
