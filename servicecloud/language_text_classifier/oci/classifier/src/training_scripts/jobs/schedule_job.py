
################################################################################################
#  $ACCELERATOR_HEADER_PLACE_HOLDER$
#  SHA1: $Id:  $
################################################################################################
#  File: $ACCELERATOR_HEADER_FILE_NAME_PLACE_HOLDER$
################################################################################################
####################################
# DataScience related resources
####################################

import logging
import os
import time
import schedule
import subprocess
import functools
import sys
from os.path import dirname, abspath

logging.basicConfig(level=logging.INFO, format='[%(levelname)s] %(asctime)s %(name)s:%(lineno)d - %(message)s')
logger = logging.getLogger(os.path.basename(__file__))

base_dir = dirname(dirname(dirname(abspath(__file__))))
sys.path.append(base_dir)

logger.info(f"SYS: {base_dir}")

from training_scripts.jobs.constants import MODEL_TRAINING_INTERVAL, INGESTION_JOB_REPEAT_INTERVAL_IN_HOUR, \
    JOB_NAME, INGESTION_JOB, BUILD_MODEL_JOB, RECTIFY_JOB, RECTIFY_JOB_REPEAT_INTERVAL_IN_HOUR, INITIAL_JOB_RUN


def job(script_name):
    src_dir = dirname(abspath(__file__))
    logger.info(os.getcwd())
    command = f"python {src_dir}/{script_name}"
    logger.info(f"Command: {command}")
    process = subprocess.Popen(command, shell=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
    for stdout_line in iter(process.stdout.readline, b''):
        logger.info(f"{stdout_line}")
    process.stdout.close()
    return_code = process.wait()
    logger.info(f'Return Code: {return_code}')
    if return_code:
        raise subprocess.CalledProcessError(return_code, command)
    return process.returncode


def isScheduled():
    flag = int(os.getenv(INITIAL_JOB_RUN, 0))
    logger.info(f"flag: {flag}")
    return flag == 0


def schedule_job(job_with_param, interval):
    schedule.every(int(os.getenv(interval, 0))).hours.do(job_with_param)
    while True:
        time.sleep(0.005)
        schedule.run_pending()
        time.sleep(60)


def schedule_ingestion_job():
    logger.info("*** Executing Ingestion Job ***")
    report_name = "ingest_data.py"
    job(report_name)
    if isScheduled():
        job_with_param = functools.partial(job, report_name)
        schedule_job(job_with_param, INGESTION_JOB_REPEAT_INTERVAL_IN_HOUR)


def schedule_rectifying_job():
    logger.info("*** Executing Rectifying Job ***")
    report_name = "rectify_data.py"
    job(report_name)
    if isScheduled():
        job_with_param = functools.partial(job, report_name)
        schedule_job(job_with_param, RECTIFY_JOB_REPEAT_INTERVAL_IN_HOUR)


def schedule_model_building():
    logger.info("*** Executing Model Training Job ***")
    report_name = "main.py"
    job(report_name)
    if isScheduled():
        report_name = "main.py --is_first_time=False"
        job_with_param = functools.partial(job, report_name)
        schedule_job(job_with_param, MODEL_TRAINING_INTERVAL)


if __name__ == '__main__':
    jobName = os.getenv(JOB_NAME)
    if jobName is None:
        raise AttributeError("Please provide job name")
    logger.info(f"*** JOB: {jobName} ***")
    if jobName == INGESTION_JOB:
        schedule_ingestion_job()
    elif jobName == RECTIFY_JOB:
        schedule_rectifying_job()
    elif jobName == BUILD_MODEL_JOB:
        schedule_model_building()
    else:
        raise FileNotFoundError("Please provide appropriate job name")
