################################################################################################
#  This file is part of the Oracle B2C Service Accelerator Reference Integration set published
#  by Oracle B2C Service licensed under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23C (August 2023) 
#  date: Tue Aug 22 11:57:50 IST 2023
 
#  revision: RNW-23C
#  SHA1: $Id: d3e0028f4bae128d300f9ff6caf7dac60590101b $
################################################################################################
#  File: emotion_datascience.tf
################################################################################################
####################################
# DataScience related resources
####################################

/*
* Create Language Classification Project Id in project.txt
*/
resource null_resource "create_lang_emotions_project" {
  triggers  =  { always_run = "${timestamp()}" }
  provisioner "local-exec" {
    command = "sh ${path.module}/scripts/retrieve_project.sh ${local.emotion-project-name} ${resource.oci_identity_compartment.tf_compartment.id} ${path.module} emotions.txt"
  }
}

data local_file "lang_emotions_project_id" {
    depends_on = [null_resource.create_lang_emotions_project]
    filename = "${path.module}/emotions.txt"
}


/*
* Create Emotion Ingestion One Time Job
*/
resource oci_datascience_job "tf_emotion_ingestion_job_one_time" {
  depends_on = [null_resource.create_lang_emotions_project, null_resource.save_job_artifact_size, oci_datascience_project.tf_project, oci_objectstorage_bucket.tf_bucket]
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  delete_related_job_runs = "true"
  #description = <<Optional value not found in discovery>>
  display_name = "One Time Emotion Ingestion Job"
  freeform_tags = {
  }
  job_artifact = "${path.module}/../src.zip"
  artifact_content_disposition = "attachment; filename=src.zip"
  artifact_content_length = "${data.local_file.read_job_artifact_size.content}"
  job_configuration_details {
    environment_variables = {
      "JOB_RUN_ENTRYPOINT"                              = "./jobs/common/run_job.sh"
      "JOB_NAME"                                        = "emotion_ingestion_job"
      "MAIN_SOURCE_BUCKET_LOCATION"                     = "oci://${var.bucket_url}/emotion/data/"
      "CX_DOMAIN"                                       = var.domain
      "AUTH_TYPE"                                       = var.authorization_authtype
      "INITIAL_JOB_RUN"                                 = 1
      "INGESTION_JOB_INITIAL_DATA_FETCH_DAYS"           = var.emotion_num_of_days_data_to_fetch
      "EMOTION_INGESTION_JOB_REPEAT_INTERVAL_IN_DAYS"   = var.emotion_ingestion_job_schedule_interval
      "INGESTION_SECRET_ID"                             = oci_vault_secret.tf_secret.id
      "JWT_TIME_EXPIRY_IN_MINUTE"                       = 2
      "OAUTH_USER"                                      = var.authorization_oauth_user
      "OAUTH_ENTITY"                                    = var.authorization_oauth_entity
      "OAUTH_PATH"                                      = var.authorization_oauth_path
      "EMOTION_THRESHOLD"                               = var.emotion_threshold
      "ADD_ON_PERCENTAGE_FOR_POSITIVE_SAMPLE"           = var.emotion_percentage_for_positive_samples
      "ADD_ON_PERCENTAGE_FOR_NEUTRAL_SAMPLE"            = var.emotion_percentage_for_neutral_samples
      "COMPARTMENT_ID"                                  = resource.oci_identity_compartment.tf_compartment.id
    }
    job_type = "DEFAULT"
  }
  job_infrastructure_configuration_details {
    block_storage_size_in_gbs = "50"
    job_infrastructure_type   = "STANDALONE"
    shape_name = var.ingestion_job_shape
    subnet_id  = oci_core_subnet.tf_private_subnet.id
  }
  job_log_configuration_details {
    enable_auto_log_creation = "true"
    enable_logging           = "true"
    log_group_id             = oci_logging_log_group.tf_log_group.id
  }
  project_id = oci_datascience_project.tf_project.id
  provisioner "local-exec" {
    when    = destroy
    command = "sh ./scripts/destroy_jobs.sh ${self.compartment_id}"
    working_dir = path.module
  }
}

resource oci_datascience_job_run "tf_emotion_ingestion_one_time_job_run" {
  #Required
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  job_id = oci_datascience_job.tf_emotion_ingestion_job_one_time.id
  project_id = oci_datascience_project.tf_project.id
  asynchronous   = false
  job_configuration_override_details {
    job_type = "DEFAULT"
  }
  timeouts {
    create = "90h"
  }
}

/*
* Create Scheduled Emotion Ingestion Job
*/
resource oci_datascience_job "tf_emotion_ingestion_job" {
  depends_on = [oci_datascience_job_run.tf_emotion_ingestion_one_time_job_run, oci_datascience_job_run.tf_rectify_emotions_one_time_job_run]
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  delete_related_job_runs = "true"
  #description = <<Optional value not found in discovery>>
  display_name = "Scheduled Emotion Ingestion Job"
  freeform_tags = {
  }
  job_artifact = "${path.module}/../src.zip"
  artifact_content_disposition = "attachment; filename=src.zip"
  artifact_content_length = "${data.local_file.read_job_artifact_size.content}"
  job_configuration_details {
    environment_variables = {
      "JOB_RUN_ENTRYPOINT"                              = "./jobs/common/run_job.sh"
      "JOB_NAME"                                        = "emotion_ingestion_job"
      "MAIN_SOURCE_BUCKET_LOCATION"                     = "oci://${var.bucket_url}/emotion/data/"
      "CX_DOMAIN"                                       = var.domain
      "AUTH_TYPE"                                       = var.authorization_authtype
      "INGESTION_JOB_INITIAL_DATA_FETCH_DAYS"           = var.emotion_num_of_days_data_to_fetch
      "EMOTION_INGESTION_JOB_REPEAT_INTERVAL_IN_DAYS"   = var.emotion_ingestion_job_schedule_interval
      "INGESTION_SECRET_ID"                             = oci_vault_secret.tf_secret.id
      "JWT_TIME_EXPIRY_IN_MINUTE"                       = 2
      "OAUTH_USER"                                      = var.authorization_oauth_user
      "OAUTH_ENTITY"                                    = var.authorization_oauth_entity
      "OAUTH_PATH"                                      = var.authorization_oauth_path
      "EMOTION_THRESHOLD"                               = var.emotion_threshold
      "ADD_ON_PERCENTAGE_FOR_POSITIVE_SAMPLE"           = var.emotion_percentage_for_positive_samples
      "ADD_ON_PERCENTAGE_FOR_NEUTRAL_SAMPLE"            = var.emotion_percentage_for_neutral_samples
      "COMPARTMENT_ID"                                  = resource.oci_identity_compartment.tf_compartment.id
    }
    job_type = "DEFAULT"
  }
  job_infrastructure_configuration_details {
    block_storage_size_in_gbs = "50"
    job_infrastructure_type   = "STANDALONE"
    shape_name = var.ingestion_job_shape
    subnet_id  = oci_core_subnet.tf_private_subnet.id
  }
  job_log_configuration_details {
    enable_auto_log_creation = "true"
    enable_logging           = "true"
    log_group_id             = oci_logging_log_group.tf_log_group.id
  }
  project_id = oci_datascience_project.tf_project.id
  provisioner "local-exec" {
    when    = destroy
    command = "sh ./scripts/destroy_jobs.sh ${self.compartment_id}"
    working_dir = path.module
  }
}

resource oci_datascience_job_run "tf_emotion_ingestion_job_run" {
  #Required
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  job_id = oci_datascience_job.tf_emotion_ingestion_job.id
  project_id = oci_datascience_project.tf_project.id
  asynchronous   = true
  job_configuration_override_details {
    job_type = "DEFAULT"
  }
  timeouts {
    create = "90h"
  }
}

/*
* Create Rectify Emotions One Time Job
*/
resource oci_datascience_job "tf_rectify_emotions_one_time_job" {
  depends_on = [oci_datascience_job_run.tf_emotion_ingestion_one_time_job_run]
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  delete_related_job_runs = "true"
  #description = <<Optional value not found in discovery>>
  display_name = "One Time Rectify Emotions Job"
  freeform_tags = {
  }
  job_artifact = "${path.module}/../src.zip"
  artifact_content_disposition = "attachment; filename=src.zip"
  artifact_content_length = "${data.local_file.read_job_artifact_size.content}"
  job_configuration_details {
    environment_variables = {
      "JOB_RUN_ENTRYPOINT"                              = "./jobs/common/run_job.sh"
      "JOB_NAME"                                        = "rectify_emotions_job"
      "MAIN_SOURCE_BUCKET_LOCATION"                     = "oci://${var.bucket_url}/emotion/data/"
      "LANGUAGE_BUCKET_LOCATION"                        = "oci://${var.bucket_url}/emotion/language/"
      "RECTIFY_EMOTIONS_JOB_REPEAT_INTERVAL_IN_DAYS"    = var.emotion_ingestion_job_schedule_interval
      "INITIAL_JOB_RUN"                                 = 1
      "EMOTION_THRESHOLD"                               = var.emotion_threshold
      "ADD_ON_PERCENTAGE_FOR_POSITIVE_SAMPLE"           = var.emotion_percentage_for_positive_samples
      "ADD_ON_PERCENTAGE_FOR_NEUTRAL_SAMPLE"            = var.emotion_percentage_for_neutral_samples
      "COMPARTMENT_ID"                                  = resource.oci_identity_compartment.tf_compartment.id
    }
    job_type = "DEFAULT"
  }
  job_infrastructure_configuration_details {
    block_storage_size_in_gbs = "50"
    job_infrastructure_type   = "STANDALONE"
    shape_name = var.ingestion_job_shape
    subnet_id  = oci_core_subnet.tf_private_subnet.id
  }
  job_log_configuration_details {
    enable_auto_log_creation = "true"
    enable_logging           = "true"
    log_group_id             = oci_logging_log_group.tf_log_group.id
  }
  project_id = oci_datascience_project.tf_project.id
  provisioner "local-exec" {
    when    = destroy
    command = "sh ./scripts/destroy_jobs.sh ${self.compartment_id}"
    working_dir = path.module
  }
}

resource oci_datascience_job_run "tf_rectify_emotions_one_time_job_run" {
  #Required
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  job_id = oci_datascience_job.tf_rectify_emotions_one_time_job.id
  project_id = oci_datascience_project.tf_project.id
  asynchronous   = false
  job_configuration_override_details {
    job_type = "DEFAULT"
  }
  timeouts {
    create = "90h"
  }
}

/*
* Create Scheduled Rectify Emotions Job
*/
resource oci_datascience_job "tf_rectify_emotions_job" {
  depends_on = [oci_datascience_job_run.tf_rectify_emotions_one_time_job_run]
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  delete_related_job_runs = "true"
  #description = <<Optional value not found in discovery>>
  display_name = "Scheduled Rectify Emotions Job"
  freeform_tags = {
  }
  job_artifact = "${path.module}/../src.zip"
  artifact_content_disposition = "attachment; filename=src.zip"
  artifact_content_length = "${data.local_file.read_job_artifact_size.content}"
  job_configuration_details {
    environment_variables = {
      "JOB_RUN_ENTRYPOINT"                              = "./jobs/common/run_job.sh"
      "JOB_NAME"                                        = "rectify_emotions_job"
      "MAIN_SOURCE_BUCKET_LOCATION"                     = "oci://${var.bucket_url}/emotion/data/"
      "LANGUAGE_BUCKET_LOCATION"                        = "oci://${var.bucket_url}/emotion/language/"
      "RECTIFY_EMOTIONS_JOB_REPEAT_INTERVAL_IN_DAYS"    = var.emotion_ingestion_job_schedule_interval
      "EMOTION_THRESHOLD"                               = var.emotion_threshold
      "ADD_ON_PERCENTAGE_FOR_POSITIVE_SAMPLE"           = var.emotion_percentage_for_positive_samples
      "ADD_ON_PERCENTAGE_FOR_NEUTRAL_SAMPLE"            = var.emotion_percentage_for_neutral_samples
      "COMPARTMENT_ID"                                  = resource.oci_identity_compartment.tf_compartment.id
    }
    job_type = "DEFAULT"
  }
  job_infrastructure_configuration_details {
    block_storage_size_in_gbs = "50"
    job_infrastructure_type   = "STANDALONE"
    shape_name = var.ingestion_job_shape
    subnet_id  = oci_core_subnet.tf_private_subnet.id
  }
  job_log_configuration_details {
    enable_auto_log_creation = "true"
    enable_logging           = "true"
    log_group_id             = oci_logging_log_group.tf_log_group.id
  }
  project_id = oci_datascience_project.tf_project.id
  provisioner "local-exec" {
    when    = destroy
    command = "sh ./scripts/destroy_jobs.sh ${self.compartment_id}"
    working_dir = path.module
  }
}

resource oci_datascience_job_run "tf_rectify_emotions_job_run" {
  #Required
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  job_id = oci_datascience_job.tf_rectify_emotions_job.id
  project_id = oci_datascience_project.tf_project.id
  asynchronous   = true
  job_configuration_override_details {
    job_type = "DEFAULT"
  }
  timeouts {
    create = "90h"
  }
}

/*
* Create Emotions Language Classification Job
*/
resource oci_datascience_job "tf_lang_emotions_classifier_job" {
  depends_on = [oci_datascience_job_run.tf_rectify_emotions_one_time_job_run]
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  delete_related_job_runs = "true"
  #description = <<Optional value not found in discovery>>
  display_name = "Emotions Classification Job"
  freeform_tags = {
  }
  job_artifact = "${path.module}/../src.zip"
  artifact_content_disposition = "attachment; filename=src.zip"
  artifact_content_length = "${data.local_file.read_job_artifact_size.content}"
  job_configuration_details {
    environment_variables = {
      "JOB_RUN_ENTRYPOINT"           = "./jobs/common/run_job.sh"
      "JOB_NAME"                     = "build_model"
      "LANGUAGE_BUCKET_LOCATION"     = "oci://${var.bucket_url}/emotion/language/"
      "PROJECT_NAME"                 = local.emotion-project-name
      "MODEL_TRAINING_INTERVAL"      = var.emotions_training_job_schedule_interval
      "MODEL_ENDPOINT_NAME"          = var.emotions_model_name
      "PROJECT_ID"                   = data.local_file.lang_emotions_project_id.content
      "INFERENCE_UNIT"               = var.num_inference_unit
      "COMPARTMENT_ID"               = resource.oci_identity_compartment.tf_compartment.id
    }
    job_type = "DEFAULT"
  }
  job_infrastructure_configuration_details {
    block_storage_size_in_gbs = "50"
    job_infrastructure_type   = "STANDALONE"
    shape_name = var.ingestion_job_shape
    subnet_id  = oci_core_subnet.tf_private_subnet.id
  }
  job_log_configuration_details {
    enable_auto_log_creation = "true"
    enable_logging           = "true"
    log_group_id             = oci_logging_log_group.tf_log_group.id
  }
  project_id = oci_datascience_project.tf_project.id
  provisioner "local-exec" {
    when    = destroy
    command = "sh ./scripts/destroy_jobs.sh ${self.compartment_id}"
    working_dir = path.module
  }
}

resource oci_datascience_job_run "tf_lang_emotions_classifier_job_run" {
  #Required
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  job_id = oci_datascience_job.tf_lang_emotions_classifier_job.id
  project_id = oci_datascience_project.tf_project.id
  asynchronous   = true
  job_configuration_override_details {
    job_type = "DEFAULT"
  }
  timeouts {
    create = "90h"
  }
}


/*
* Create Chat Status Update Job
*/
resource oci_datascience_job "tf_status_update_job" {
  depends_on = [oci_datascience_job_run.tf_rectify_emotions_one_time_job_run]
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  delete_related_job_runs = "true"
  #description = <<Optional value not found in discovery>>
  display_name = "Chat Inactive Status Update Job"
  freeform_tags = {
  }
  job_artifact = "${path.module}/../src.zip"
  artifact_content_disposition = "attachment; filename=src.zip"
  artifact_content_length = "${data.local_file.read_job_artifact_size.content}"
  job_configuration_details {
    environment_variables = {
      "JOB_RUN_ENTRYPOINT"                              = "./jobs/common/run_job.sh"
      "JOB_NAME"                                        = "chat_status_update"
      "CX_DOMAIN"                                       = var.domain
      "AUTH_TYPE"                                       = var.authorization_authtype
      "INGESTION_SECRET_ID"                             = oci_vault_secret.tf_secret.id
      "JWT_TIME_EXPIRY_IN_MINUTE"                       = 2
      "OAUTH_USER"                                      = var.authorization_oauth_user
      "OAUTH_ENTITY"                                    = var.authorization_oauth_entity
      "OAUTH_PATH"                                      = var.authorization_oauth_path
      "JOB_INTERVAL_IN_MIN"                             = var.emotions_inactive_chat_status_update_interval
      "COMPARTMENT_ID"                                  = resource.oci_identity_compartment.tf_compartment.id
    }
    job_type = "DEFAULT"
  }
  job_infrastructure_configuration_details {
    block_storage_size_in_gbs = "50"
    job_infrastructure_type   = "STANDALONE"
    shape_name = var.ingestion_job_shape
    subnet_id  = oci_core_subnet.tf_private_subnet.id
  }
  job_log_configuration_details {
    enable_auto_log_creation = "true"
    enable_logging           = "true"
    log_group_id             = oci_logging_log_group.tf_log_group.id
  }
  project_id = oci_datascience_project.tf_project.id
  provisioner "local-exec" {
    when    = destroy
    command = "sh ./scripts/destroy_jobs.sh ${self.compartment_id}"
    working_dir = path.module
  }
}

resource oci_datascience_job_run "tf_status_update_job_run" {
  #Required
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  job_id = oci_datascience_job.tf_status_update_job.id
  project_id = oci_datascience_project.tf_project.id
  asynchronous   = true
  job_configuration_override_details {
    job_type = "DEFAULT"
  }
  timeouts {
    create = "90h"
  }
}
