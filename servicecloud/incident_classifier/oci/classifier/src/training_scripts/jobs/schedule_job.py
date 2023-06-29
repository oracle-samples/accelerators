################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:26 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: ec86ec9bc06a533669a68394bcd49dc2a7714a18 $
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

logging.basicConfig(level=logging.INFO, format='[%(levelname)s] %(asctime)s %(name)s:%(lineno)d - %(message)s')
logger = logging.getLogger(os.path.basename(__file__))

base_dir = dirname(dirname(dirname(abspath(__file__))))
sys.path.append(base_dir)

logger.info(f"SYS: {base_dir}")

from training_scripts.constants import MODEL_TRAINING_INTERVAL, INGESTION_JOB_REPEAT_INTERVAL_IN_HOUR, \
    JOB_NAME, INGESTION_JOB, BUILD_MODEL_JOB
from training_scripts.utils import set_config


def job(script_name):
    src_dir = dirname(abspath(__file__))

    command = f"python {src_dir}/{script_name}"
    logger.info(f"Command: {command}")
    process = subprocess.Popen(command, shell=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
    for stdout_line in iter(process.stdout.readline, b''):
        logging.info('%r', stdout_line)
    process.stdout.close()
    return_code = process.wait()
    if return_code:
        raise subprocess.CalledProcessError(return_code, cmd)
    return process.returncode


def schedule_ingestion_job():
    logger.info("*** Executing Ingestion Job ***")
    report_name = "get_report.py"
    job(report_name)
    job_with_param = functools.partial(job, report_name)
    schedule.every(int(os.getenv(INGESTION_JOB_REPEAT_INTERVAL_IN_HOUR, 0))).hours.do(job_with_param)
    while True:
        time.sleep(0.005)
        schedule.run_pending()
        time.sleep(60)


def schedule_model_building():
    logger.info("*** Executing Model Training Job ***")
    report_name = "model_artifact.py"
    job(report_name)
    job_with_param = functools.partial(job, report_name)
    schedule.every(int(os.getenv(MODEL_TRAINING_INTERVAL, 0))).hours.do(job_with_param)
    while True:
        time.sleep(0.005)
        schedule.run_pending()
        time.sleep(60)


if __name__ == '__main__':
    set_config()
    jobName = os.getenv(JOB_NAME)
    if jobName is None:
        raise AttributeError("Please provide job name")
    logger.info(f"*** JOB: {jobName} ***")
    if jobName == INGESTION_JOB:
        schedule_ingestion_job()
    elif jobName == BUILD_MODEL_JOB:
        schedule_model_building()
    else:
        raise FileNotFoundError("Please provide appropriate job name")
