################################################################################################
#  This file is part of the Oracle B2C Service Accelerator Reference Integration set published
#  by Oracle B2C Service licensed under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23C (August 2023) 
#  date: Tue Aug 22 11:57:49 IST 2023
 
#  revision: RNW-23C
#  SHA1: $Id: 44749097c3dee74df73391a9328d8d6ae26a55fb $
################################################################################################
#  File: test_schedule_job.py
################################################################################################
import unittest
from unittest.mock import patch

from common.constants import INGESTION_JOB, EMOTION_INGESTION_JOB, RECTIFY_JOB, RECTIFY_EMOTIONS_JOB, BUILD_MODEL_JOB, \
    CHAT_STATUS_UPDATE
from common.schedule_job import run_command, run_job, isScheduled


class TestScheduleJob(unittest.TestCase):

    @patch('subprocess.Popen')
    def test_run_command(self, mock_popen):
        mock_process = mock_popen.return_value
        mock_process.stdout.readline.side_effect = [b'Output Line 1\n', b'Output Line 2\n', b'']
        mock_process.wait.return_value = 0
        mock_process.returncode = 0
        command = 'echo "Hello, World!"'
        result = run_command(command)

        mock_popen.assert_called_once_with(command, shell=True, stdout=-1, stderr=-2)
        self.assertEqual(result, 0)

    @patch("common.schedule_job.os.getenv", side_effect=lambda key, default: {
        "INITIAL_JOB_RUN": "0"
    }[key])
    def test_isScheduled(self, mock_os_getenv):
        result = isScheduled()
        self.assertTrue(result)

    @patch("common.schedule_job.schedule_and_run_job")
    @patch("common.schedule_job.job")
    @patch("common.schedule_job.isScheduled", return_value=False)
    @patch("common.schedule_job.os.getenv", side_effect=lambda key, default: {
        "INGESTION_JOB_REPEAT_INTERVAL_IN_DAYS": "1",
        "EMOTION_INGESTION_JOB_REPEAT_INTERVAL_IN_DAYS": "1",
        "RECTIFY_JOB_REPEAT_INTERVAL_IN_DAYS": "1",
        "RECTIFY_EMOTIONS_JOB_REPEAT_INTERVAL_IN_DAYS": "1",
        "MODEL_TRAINING_INTERVAL": "1"
    }.get(key, default))
    @patch("common.schedule_job.logger")
    def test_run_job(self, mock_logger, mock_os_getenv, mock_is_scheduled, mock_job, mock_schedule_and_run_job):
        job_mapping = {
            INGESTION_JOB: "../chat_ask_for_manager/ingest_data.py",
            EMOTION_INGESTION_JOB: "../chat_ingest_src/populate_sentiments.py",
            RECTIFY_JOB: "../chat_ask_for_manager/rectify_data.py",
            RECTIFY_EMOTIONS_JOB: "../chat_ingest_src/rectify_emotions.py",
            BUILD_MODEL_JOB: "../chat_model_train_src/main.py",
            CHAT_STATUS_UPDATE: "../chat_status_update_src/mark_completed_chat_to_inactive.py"
        }
        repeat_interval_mapping = {
            INGESTION_JOB: "INGESTION_JOB_REPEAT_INTERVAL_IN_DAYS",
            EMOTION_INGESTION_JOB: "EMOTION_INGESTION_JOB_REPEAT_INTERVAL_IN_DAYS",
            RECTIFY_JOB: "RECTIFY_JOB_REPEAT_INTERVAL_IN_DAYS",
            RECTIFY_EMOTIONS_JOB: "RECTIFY_EMOTIONS_JOB_REPEAT_INTERVAL_IN_DAYS",
            BUILD_MODEL_JOB: "MODEL_TRAINING_INTERVAL",
            CHAT_STATUS_UPDATE: "RECTIFY_EMOTIONS_JOB_REPEAT_INTERVAL_IN_DAYS"
        }

        for job_name, expected_script in job_mapping.items():
            with self.subTest(job_name=job_name):
                mock_logger.reset_mock()
                mock_is_scheduled.reset_mock()
                mock_job.reset_mock()
                mock_schedule_and_run_job.reset_mock()

                run_job(job_name)

                if job_name == BUILD_MODEL_JOB:
                    mock_logger.info.assert_called_with(f"*** Executing Model Training Job ***")
                    mock_is_scheduled.assert_called_once()
                    mock_job.assert_called_once_with("../chat_model_train_src/main.py")
                else:
                    mock_logger.info.assert_called_with(f"*** JOB: {job_name} ***")
                    mock_schedule_and_run_job.assert_called_with(expected_script, repeat_interval_mapping[job_name])
                    mock_job.assert_not_called()
