################################################################################################
#  This file is part of the Oracle B2C Service Accelerator Reference Integration set published
#  by Oracle B2C Service licensed under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23C (August 2023) 
#  date: Tue Aug 22 11:57:48 IST 2023
 
#  revision: RNW-23C
#  SHA1: $Id: a6701e1a04d196ef0701f94956d0df559be60a0c $
################################################################################################
#  File: schedule_job.py
################################################################################################

import logging
import os
import time
import schedule
import subprocess
import functools
import sys
from os.path import dirname, abspath
from common.constants import MODEL_TRAINING_INTERVAL, INGESTION_JOB_REPEAT_INTERVAL_IN_DAYS, \
    JOB_NAME, INGESTION_JOB, BUILD_MODEL_JOB, RECTIFY_JOB, RECTIFY_JOB_REPEAT_INTERVAL_IN_DAYS, INITIAL_JOB_RUN, \
    EMOTION_INGESTION_JOB, EMOTION_INGESTION_JOB_REPEAT_INTERVAL_IN_DAYS, RECTIFY_EMOTIONS_JOB, \
    RECTIFY_EMOTIONS_JOB_REPEAT_INTERVAL_IN_DAYS, CHAT_STATUS_UPDATE

logging.basicConfig(level=logging.INFO, format='[%(levelname)s] %(asctime)s %(name)s:%(lineno)d - %(message)s')
logger = logging.getLogger(os.path.basename(__file__))

base_dir = dirname(dirname(abspath(__file__)))
sys.path.append(base_dir)

logger.info(f"SYS: {base_dir}")


def job(script_name):
    src_dir = dirname(abspath(__file__))
    logger.info(os.getcwd())
    command = f"python {src_dir}/{script_name}"
    logger.info(f"Command: {command}")
    return run_command(command)


def run_command(command):
    process = subprocess.Popen(command, shell=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
    for stdout_line in iter(process.stdout.readline, b''):
        logger.info(stdout_line)
    process.stdout.close()
    return_code = process.wait()
    logger.info(f'Return Code: {return_code}')
    if return_code:
        raise subprocess.CalledProcessError(return_code, command)
    return process.returncode


def schedule_and_run_job(job_entry_point, repeat_interval):
    logger.info(f"*** Executing {job_entry_point} ***")
    job(job_entry_point)
    if isScheduled():
        job_with_param = functools.partial(job, job_entry_point)
        schedule.every(int(os.getenv(repeat_interval, 0))).days.do(job_with_param)
        while True:
            time.sleep(0.005)
            schedule.run_pending()
            time.sleep(60)


def isScheduled():
    flag = int(os.getenv(INITIAL_JOB_RUN, 0))
    logger.info(f"flag: {flag}")
    return flag == 0


def run_job(jobName):
    if jobName is None:
        raise AttributeError("Please provide job name")

    logger.info(f"*** JOB: {jobName} ***")

    if jobName == INGESTION_JOB:
        schedule_and_run_job("../chat_ask_for_manager/ingest_data.py", INGESTION_JOB_REPEAT_INTERVAL_IN_DAYS)
    elif jobName == EMOTION_INGESTION_JOB:
        schedule_and_run_job("../chat_ingest_src/populate_sentiments.py", EMOTION_INGESTION_JOB_REPEAT_INTERVAL_IN_DAYS)
    elif jobName == RECTIFY_JOB:
        schedule_and_run_job("../chat_ask_for_manager/rectify_data.py", RECTIFY_JOB_REPEAT_INTERVAL_IN_DAYS)
    elif jobName == RECTIFY_EMOTIONS_JOB:
        schedule_and_run_job("../chat_ingest_src/rectify_emotions.py", RECTIFY_EMOTIONS_JOB_REPEAT_INTERVAL_IN_DAYS)
    elif jobName == BUILD_MODEL_JOB:
        logger.info("*** Executing Model Training Job ***")
        job("../chat_model_train_src/main.py")
        if isScheduled():
            report_name = "../chat_model_train_src/main.py --is_first_time"
            job_with_param = functools.partial(job, report_name)
            schedule.every(int(os.getenv(MODEL_TRAINING_INTERVAL, 0))).days.do(job_with_param)
            while True:
                time.sleep(0.005)
                schedule.run_pending()
                time.sleep(60)
    elif jobName == CHAT_STATUS_UPDATE:
        schedule_and_run_job("../chat_status_update_src/mark_completed_chat_to_inactive.py",
                             RECTIFY_EMOTIONS_JOB_REPEAT_INTERVAL_IN_DAYS)
    else:
        raise FileNotFoundError("Please provide appropriate job name")


if __name__ == '__main__':
    jobName = os.getenv(JOB_NAME)
    run_job(jobName)
