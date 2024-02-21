
################################################################################################
#  $ACCELERATOR_HEADER_PLACE_HOLDER$
#  SHA1: $Id:  $
################################################################################################
#  File: $ACCELERATOR_HEADER_FILE_NAME_PLACE_HOLDER$
################################################################################################
####################################
# DataScience related resources
####################################

import ast
import logging
import os
from collections import Counter

import pandas as pd
import sys

from io import BytesIO
from datetime import datetime
from os.path import dirname, abspath
from tornado import concurrent

from oci.object_storage.models import CreateBucketDetails

from constants import LOGGING_FORMAT, MAIN_SOURCE_BUCKET_LOCATION, \
    TARGET_COLUMNS, FEATURE_COLUMNS, LIMIT_200K, \
    LANGUAGE_BUCKET_LOCATION, TEXT_COLUMN, LABEL_COLUMN, MINIMUM_SAMPLES_PER_TARGET, COMPARTMENT_ID, ENCODING
from preprocess import clean
from storage import get_object_storage_client, store_or_update_csv, \
    get_csv_file_names, get_bucket_details, delete_folder, is_bucket_exists
from sklearn.utils import shuffle

base_dir = dirname(dirname(dirname(abspath(__file__))))
sys.path.append(base_dir)

logging.basicConfig(level=logging.INFO, format=LOGGING_FORMAT)
logger = logging.getLogger(os.path.basename(__file__))

DEFAULT_ENCODING = 'utf-8'

LANGUAGE_TRAINING_COLUMNS = [TEXT_COLUMN, LABEL_COLUMN]


def prepare_for_language_dataframe(df, features, targets):
    df = df.dropna(subset=targets, how="all")
    df[features] = df[features].fillna('')

    def combine_targets(row):
        label_parts = []
        for target in targets:
            if target in row.index and pd.notnull(row[target]):
                label_parts.append(f"{target.replace(' ', '_')}_{row[target]}")
        return "|".join(label_parts) if label_parts else None

    if len(df) > 0:
        df[TEXT_COLUMN] = df[features].astype(str).apply(lambda x: ' '.join(x), axis=1)
        df[TEXT_COLUMN] = df[TEXT_COLUMN].apply(lambda text: clean(text))
        df[LABEL_COLUMN] = df.apply(combine_targets, axis=1)
        df = df[df[TEXT_COLUMN].agg(len) > 2]  # At least two chars should be available
    else:
        df = pd.DataFrame(columns=LANGUAGE_TRAINING_COLUMNS)
    df = df.reset_index(drop=True)
    return df[LANGUAGE_TRAINING_COLUMNS]


def get_clean_dataframe(client, namespace, bucket_name, object_name, columns_to_use, feature_columns, target_columns):
    content_data = client.get_object(namespace, bucket_name, object_name)
    if content_data.data:
        bytes_stream = BytesIO(content_data.data.content)
        df = pd.read_csv(bytes_stream,
                         header=0,
                         usecols=columns_to_use, encoding="utf-8")
        target_dfs = prepare_for_language_dataframe(df.copy(), feature_columns, target_columns)
        return target_dfs
    else:
        return pd.DataFrame(columns=LANGUAGE_TRAINING_COLUMNS)


def rectify():
    logger.info(f"*** Scheduled the job at {datetime.now()} ***")
    client = get_object_storage_client()
    minumum_samples_per_target = int(os.getenv(MINIMUM_SAMPLES_PER_TARGET, max(os.cpu_count() - 1, 1)))
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
    chunk_data = [pd.DataFrame(columns=LANGUAGE_TRAINING_COLUMNS)]

    futures = []
    tasks = []
    executor = concurrent.futures.ThreadPoolExecutor(max_workers=10)

    for object_name in sorted_file_paths:
        tasks.append((client, namespace, bucket_name, object_name, columns_to_use, feature_columns, target_columns))

    for filter_task_args in tasks:
        future = executor.submit(get_clean_dataframe, *filter_task_args)
        futures.append(future)

    concurrent.futures.wait(futures)

    for future in concurrent.futures.as_completed(futures):
        try:
            target_df = future.result()
            chunk_data.append(target_df)
        except Exception as exc:
            logger.error('generated an exception: %s' % exc)

    df = pd.concat(chunk_data).reset_index(drop=True)
    logger.info(f"*** {df.shape[0]} rows fetched ***")
    label_counts = Counter(label for labels in df[LABEL_COLUMN] for label in labels.split('|'))
    label_counts_filtered = [label for label, count in label_counts.items() if count < minumum_samples_per_target]
    if len(label_counts_filtered) > 0:
        logger.info(
            f"*** remove {label_counts_filtered} from dataframe as it does not have enough samples to train ***")
        for label in label_counts_filtered:
            logger.info(f"*** {label} have {label_counts[label]} samples")
            df[LABEL_COLUMN] = df[LABEL_COLUMN].str.replace(fr"(\||^){label}(\||$)", "")

    df = df[df[LABEL_COLUMN].agg(len) > 0]
    df_first_200k = df[:LIMIT_200K]
    df_first_200k = shuffle(df_first_200k)
    logger.info(f"*** Final data to train with is {df_first_200k.shape[0]} after pruning ***")
    data = df_first_200k.to_csv(encoding=ENCODING , index=False).encode(ENCODING)
    delete_folder(language_bucket_location, client)
    rectified_data_path = os.path.join(language_bucket_location, f"data_{datetime.now().strftime('%Y-%m-%d')}.csv")
    store_or_update_csv(rectified_data_path, data, client)
    logger.info(f"*** Rectified Job Complete and Stored data in {rectified_data_path} ***")


if __name__ == '__main__':
    rectify()
    #
    # target_sum = 2_00_000
    # df = pd.read_csv("data.csv")
    # df = df.dropna(subset=["cat"])
    # df = df[df["count"] > 20]
    #
    # df_greater_than_100 = df[df["count"] > 100]
    # df_100_or_less = df[df["count"] <= 100]
    #
    # df = df.sort_values(by="count", ascending=False)
    #
    # total_sum = df["count"].sum()
    #
    # reduction_ratio = target_sum / total_sum
    #
    # sampled_categories = {}
    #
    # for _, row in df_greater_than_100.iterrows():
    #     category = row["cat"]
    #     count = row["count"]
    #     reduced_count = int(count * reduction_ratio)
    #     sampled_categories[category] = reduced_count
    #
    # sampled_categories.update(dict(df_100_or_less[["cat", "count"]].values))
    #
    # print(sampled_categories)
