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
#  SHA1: $Id: 17f89a25a1105d5a726383d08a4152bc947235d7 $
################################################################################################
#  File: populate_sentiments.py
################################################################################################

import logging
import os
from datetime import datetime
from functools import lru_cache

from oci.object_storage import ObjectStorageClient
from oci.object_storage.models import CreateBucketDetails
from pandas import DataFrame
from tornado import concurrent
from transformers import pipeline

from common.constants import EMOTION_THRESHOLD, LOGGING_FORMAT, EMOTION_MODEL, MAIN_SOURCE_BUCKET_LOCATION, \
    DATE_TIME_FORMAT, COMPARTMENT_ID, NEUTRAL_CHAT, POSITIVE_CHAT, NEGATIVE_CHAT, negative_list, \
    CUSTOM_EMOTION_REPORT_NAME, EMOTION_FEEDBACK_QUERY
from chat_ingest_src.ingest_from_report import populate_rows_from_report
from chat_ingest_src.ingest_from_roql import fetch_report
from chat_ingest_src.preprocess_data import remove_html_single
from common.storage import get_object_storage_client, is_directory, get_bucket_details, is_bucket_exists, \
    store_or_update_csv

LOGGING_LEVEL = logging.INFO
logging.basicConfig(level=LOGGING_LEVEL, format=LOGGING_FORMAT)
logger = logging.getLogger()

MAX_WORKERS = max(os.cpu_count() - 1, 1)


@lru_cache(maxsize=10)
def load_model():
    os.environ['GIT_PYTHON_REFRESH'] = "quiet"
    emotion_model = os.getenv(EMOTION_MODEL, "arpanghoshal/EmoRoBERTa")
    emotion = pipeline('sentiment-analysis', model=emotion_model)
    return emotion


def split_set_into_equal_sizes(input_set, subset_size):
    input_list = list(input_set)
    num_elements = len(input_list)
    num_subsets = num_elements // subset_size

    subsets = []
    for i in range(num_subsets):
        subset = set(input_list[i * subset_size: (i + 1) * subset_size])
        subsets.append(subset)

    if num_elements % subset_size != 0:
        remaining_subset = set(input_list[num_subsets * subset_size:])
        subsets.append(remaining_subset)

    return subsets


def predict(input_data, chat_id, model):
    logger.info("Before prediction: Data length - {}, First element - {}".format(len(input_data), input_data[0]))
    emotion_labels = model.predict(input_data)
    logger.info("After prediction: Data length - {}, First element - {}".format(len(input_data), input_data[0]))
    return emotion_labels, input_data, chat_id


def process_chat(df, chat_ids):
    chat_futures = []
    text_list_of_list = []
    chat_result = []
    model = load_model()
    with concurrent.futures.ThreadPoolExecutor(max_workers=MAX_WORKERS) as executor_chat:

        for chat_id in chat_ids:
            text_list = []
            chat_conversations = df.loc[df['Chat ID'] == chat_id]
            for index, rows in chat_conversations.iterrows():
                text = remove_html_single(rows[1])
                text_list.append(text[:512])
            text_list_of_list.append((text_list, chat_id))

        for text_list, chat_id in text_list_of_list:
            chat_futures.append(executor_chat.submit(predict, text_list, chat_id, model))

        chat_futures_subset = split_set_into_equal_sizes(chat_futures, MAX_WORKERS)

        for current_feature_set in chat_futures_subset:
            concurrent.futures.wait(current_feature_set)

            for future_current in concurrent.futures.as_completed(current_feature_set):
                try:
                    prediction, text_list, chat_id = future_current.result()

                    for row_num, current in enumerate(prediction):
                        emotion = prediction[row_num]['label']
                        emotion_score = round(prediction[row_num]['score'], 2) * 100

                        if emotion_score > int(os.getenv(EMOTION_THRESHOLD)):
                            if emotion in negative_list:
                                sentiment = NEGATIVE_CHAT
                            elif emotion == 'neutral':
                                sentiment = NEUTRAL_CHAT
                            else:
                                sentiment = POSITIVE_CHAT

                            chat_result.append({'Chat ID': chat_id,
                                                'text': text_list[row_num].encode('latin-1', errors='replace')
                                               .decode("utf-8", errors="ignore"), 'labels': sentiment,
                                                'emotion': emotion,
                                                'score': round(prediction[row_num]['score'], 2) * 100})
                except Exception as exception:
                    logger.error('generated an exception: %s' % exception)

    return chat_result


def upload_csv_file(ingest_bucket_location, filename, labeled_chat, object_storage_client: ObjectStorageClient = None):
    if object_storage_client is None:
        object_storage_client = get_object_storage_client()
    logger.info("*** After processing data ***")
    labeled_chat = labeled_chat.sort_values(by=["Chat ID"])
    labeled_chat = labeled_chat.drop_duplicates(subset='text', keep="last")
    if len(labeled_chat) > 0:
        result_csv = labeled_chat.to_csv(encoding='utf-8')
        logger.info(f"*** Ingest Data store start for {ingest_bucket_location} ***")
        rectified_data_path = os.path.join(ingest_bucket_location, filename)
        store_or_update_csv(rectified_data_path, result_csv, object_storage_client)
        logger.info(f"*** Ingest Data store completed for {ingest_bucket_location} ***")
    else:
        logger.info("*** Not Enough Feedback To Store ***")


def process_sentiments(columns, rows):
    labeled_chat = DataFrame(columns=['Chat ID', 'text', 'labels', 'emotion', 'score'])
    df = DataFrame(columns=columns, data=rows)
    if len(df) > 0:
        unique_chats = set(df['Chat ID'])
        chat_with_label = process_chat(df, unique_chats)
        labeled_chat = labeled_chat.append(chat_with_label)
    return labeled_chat


def label2id(label):
    if label == "neutral":
        return NEUTRAL_CHAT
    elif label == "positive":
        return POSITIVE_CHAT
    elif label == "negative":
        return NEGATIVE_CHAT


def process_feedback_sentiments(columns, rows):
    columns_to_use = ['Chat ID', 'text', 'labels', 'emotion', 'score']
    df = DataFrame(columns=columns, data=rows)
    if len(df) > 0:
        df['labels'] = df['labels'].apply(lambda label: label2id(label))
        return df[columns_to_use]
    else:
        return DataFrame(columns=columns_to_use)


def process_emotion_jobs():
    ingest_bucket_location = os.getenv(MAIN_SOURCE_BUCKET_LOCATION)
    object_storage_client = get_object_storage_client()
    bucket_name, folder_name = get_bucket_details(ingest_bucket_location)
    if not is_bucket_exists(bucket_name, object_storage_client):
        namespace = object_storage_client.get_namespace(compartment_id=os.getenv(COMPARTMENT_ID)).data
        object_storage_client.create_bucket(namespace_name=namespace,
                                            create_bucket_details=CreateBucketDetails(name=bucket_name,
                                                                                      compartment_id=os.getenv(
                                                                                          COMPARTMENT_ID)))

    is_report_present = is_directory(ingest_bucket_location, object_storage_client)
    logger.info(f"*** Data Ingested Already: {is_report_present} ***")

    if not is_report_present:
        logger.info("*** Ingesting Data From Site Configuration Analytics Report Query ***")
        report_columns, report_rows = populate_rows_from_report(CUSTOM_EMOTION_REPORT_NAME)
        labeled_chat = process_sentiments(report_columns, report_rows)
        upload_csv_file(ingest_bucket_location,
                        f"one_time_ingest_{datetime.now().strftime(DATE_TIME_FORMAT)}.csv", labeled_chat,
                        object_storage_client)
    else:
        logger.info("*** Getting Feedback from B2C site about incorrect emotions ***")
        report_name = "aia_emotions"
        columns, rows = fetch_report(report_name, EMOTION_FEEDBACK_QUERY)
        labeled_chat = process_feedback_sentiments(columns, rows)
        upload_csv_file(ingest_bucket_location,
                        f"feedback_ingest_{datetime.now().strftime(DATE_TIME_FORMAT)}.csv", labeled_chat,
                        object_storage_client)


if __name__ == '__main__':
    logger.info(f"*** Getting Sentiment Configuration From Site Configuration ***")
    process_emotion_jobs()
