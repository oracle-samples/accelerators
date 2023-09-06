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
#  SHA1: $Id: 9b7169f2fc713c56e1b889a4164f62f360849076 $
################################################################################################
#  File: manager_ask_datascience.tf
################################################################################################
####################################
# DataScience related resources
####################################

resource null_resource "save_job_artifact_size" {
  triggers  =  { always_run = "${timestamp()}" }
  provisioner "local-exec" {
    command     = <<-EOT
      zip -r ${path.module}/src.zip ${path.module}/jobs/*
      wc -c ${path.module}/src.zip | awk '{print $1}' | tail -1 | tr -d '\n' >> ${path.module}/artifact_size.txt
    EOT
    working_dir = "${abspath(path.root)}/./.."
  }
}

data local_file "read_job_artifact_size" {
    depends_on = [null_resource.save_job_artifact_size]
    filename = "${abspath(path.root)}/./../artifact_size.txt"
}

/*
* Get All DataScience project with filter of display name
*/
data "oci_datascience_projects" "tf_ds_projects" {
    #Required
    compartment_id = resource.oci_identity_compartment.tf_compartment.id

    display_name = local.project-name
    state = "ACTIVE"
}

data "oci_datascience_notebook_session_shapes" "tf_notebook_session_shapes" {
  compartment_id = var.tenancy_ocid
}

/*
* DataScience Project Configuration
*/
resource "oci_datascience_project" "tf_project" {
    #Required
    display_name = local.project-name
    compartment_id = resource.oci_identity_compartment.tf_compartment.id
    provisioner "local-exec" {
        when    = destroy
        command = "sh ./scripts/destroy_language_resources.sh ${self.compartment_id}"
        working_dir = path.module
    }
}

/*
* Create Group Log
*/
resource "oci_logging_log_group" "tf_log_group" {
    #Required
    compartment_id = resource.oci_identity_compartment.tf_compartment.id
    display_name = local.log-group-name

    provisioner "local-exec" {
        when    = destroy
        command = "sh ./scripts/destroy_logs.sh ${self.id}"
        working_dir = path.module
    }
}


data "oci_identity_tenancy" "tf_tenancy" {
    tenancy_id = var.tenancy_ocid
}


/*
* Create Language Classification Project Id in project.txt
*/
resource null_resource "create_lang_project" {
  triggers  =  { always_run = "${timestamp()}" }
  provisioner "local-exec" {
    command = "sh ${path.module}/scripts/retrieve_project.sh ${local.supervisor-project-name} ${resource.oci_identity_compartment.tf_compartment.id} ${path.module} project.txt"
  }
}

data local_file "lang_project_id" {
    depends_on = [null_resource.create_lang_project]
    filename = "${path.module}/project.txt"
}

/*
* Create Supervisor Ask Ingestion One Time Job
*/
resource oci_datascience_job "tf_ingestion_job_one_time" {
  depends_on = [null_resource.create_lang_project, null_resource.save_job_artifact_size, oci_datascience_project.tf_project, oci_objectstorage_bucket.tf_bucket]
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  delete_related_job_runs = "true"
  #description = <<Optional value not found in discovery>>
  display_name = "One Time Supervisor Ask Ingestion Job"
  freeform_tags = {
  }
  job_artifact = "${path.module}/../src.zip"
  artifact_content_disposition = "attachment; filename=src.zip"
  artifact_content_length = "${data.local_file.read_job_artifact_size.content}"
  job_configuration_details {
    environment_variables = {
      "JOB_RUN_ENTRYPOINT"                      = "./jobs/common/run_job.sh"
      "JOB_NAME"                                = "ingestion_job"
      "MAIN_SOURCE_BUCKET_LOCATION"             = "oci://${var.bucket_url}/supervisor/data/"
      "CX_DOMAIN"                               = var.domain
      "AUTH_TYPE"                               = var.authorization_authtype
      "INGESTION_JOB_REPEAT_INTERVAL_IN_DAYS"   = var.supervisor_ask_ingestion_job_schedule_interval
      "INGESTION_SECRET_ID"                     = oci_vault_secret.tf_secret.id
      "JWT_TIME_EXPIRY_IN_MINUTE"               = 2
      "INITIAL_JOB_RUN"                         = 1
      "OAUTH_USER"                              = var.authorization_oauth_user
      "OAUTH_ENTITY"                            = var.authorization_oauth_entity
      "OAUTH_PATH"                              = var.authorization_oauth_path
      "COMPARTMENT_ID"                          = resource.oci_identity_compartment.tf_compartment.id
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

resource oci_datascience_job_run "tf_ingestion_one_time_job_run" {
  #Required
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  job_id = oci_datascience_job.tf_ingestion_job_one_time.id
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
* Create Supervisor Ask Ingestion Job
*/
resource oci_datascience_job "tf_ingestion_job" {
  depends_on = [oci_datascience_job_run.tf_ingestion_one_time_job_run, oci_datascience_job_run.tf_rectify_one_time_job_run]
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  delete_related_job_runs = "true"
  #description = <<Optional value not found in discovery>>
  display_name = "Scheduled Supervisor Ask Ingestion Job"
  freeform_tags = {
  }
  job_artifact = "${path.module}/../src.zip"
  artifact_content_disposition = "attachment; filename=src.zip"
  artifact_content_length = "${data.local_file.read_job_artifact_size.content}"
  job_configuration_details {
    environment_variables = {
      "JOB_RUN_ENTRYPOINT"                      = "./jobs/common/run_job.sh"
      "JOB_NAME"                                = "ingestion_job"
      "MAIN_SOURCE_BUCKET_LOCATION"             = "oci://${var.bucket_url}/supervisor/data/"
      "CX_DOMAIN"                               = var.domain
      "AUTH_TYPE"                               = var.authorization_authtype
      "INGESTION_JOB_REPEAT_INTERVAL_IN_DAYS"   = var.supervisor_ask_ingestion_job_schedule_interval
      "INGESTION_SECRET_ID"                     = oci_vault_secret.tf_secret.id
      "JWT_TIME_EXPIRY_IN_MINUTE"               = 2
      "OAUTH_USER"                              = var.authorization_oauth_user
      "OAUTH_ENTITY"                            = var.authorization_oauth_entity
      "OAUTH_PATH"                              = var.authorization_oauth_path
      "COMPARTMENT_ID"                          = resource.oci_identity_compartment.tf_compartment.id
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

resource oci_datascience_job_run "tf_ingestion_job_run" {
  #Required
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  job_id = oci_datascience_job.tf_ingestion_job.id
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
* Create Rectify Supervisor Ask One Time Job
*/
resource oci_datascience_job "tf_rectify_one_time_job" {
  depends_on = [oci_datascience_job_run.tf_ingestion_one_time_job_run]
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  delete_related_job_runs = "true"
  #description = <<Optional value not found in discovery>>
  display_name = "One Time Rectify Supervisor Ask Job"
  freeform_tags = {
  }
  job_artifact = "${path.module}/../src.zip"
  artifact_content_disposition = "attachment; filename=src.zip"
  artifact_content_length = "${data.local_file.read_job_artifact_size.content}"
  job_configuration_details {
    environment_variables = {
      "JOB_RUN_ENTRYPOINT"                      = "./jobs/common/run_job.sh"
      "JOB_NAME"                                = "rectify_job"
      "MAIN_SOURCE_BUCKET_LOCATION"             = "oci://${var.bucket_url}/supervisor/data/"
      "MINIMUM_SAMPLES_PER_TARGET"              = 20
      "LANGUAGE_BUCKET_LOCATION"                = "oci://${var.bucket_url}/supervisor/language/"
      "RECTIFY_JOB_REPEAT_INTERVAL_IN_DAYS"     = var.supervisor_ask_ingestion_job_schedule_interval
      "FEATURE_COLUMNS"                         = "[\"text\"]"
      "TARGET_COLUMNS"                          = "[\"labels\"]"
      "INITIAL_JOB_RUN"                         = 1
      "COMPARTMENT_ID"                          = resource.oci_identity_compartment.tf_compartment.id
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

resource oci_datascience_job_run "tf_rectify_one_time_job_run" {
  #Required
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  job_id = oci_datascience_job.tf_rectify_one_time_job.id
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
* Create Rectify Supervisor Ask Job
*/
resource oci_datascience_job "tf_rectify_job" {
  depends_on = [oci_datascience_job_run.tf_rectify_one_time_job_run]
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  delete_related_job_runs = "true"
  #description = <<Optional value not found in discovery>>
  display_name = "Scheduled Rectify Supervisor Ask Job"
  freeform_tags = {
  }
  job_artifact = "${path.module}/../src.zip"
  artifact_content_disposition = "attachment; filename=src.zip"
  artifact_content_length = "${data.local_file.read_job_artifact_size.content}"
  job_configuration_details {
    environment_variables = {
      "JOB_RUN_ENTRYPOINT"                      = "./jobs/common/run_job.sh"
      "JOB_NAME"                                = "rectify_job"
      "MAIN_SOURCE_BUCKET_LOCATION"             = "oci://${var.bucket_url}/supervisor/data/"
      "MINIMUM_SAMPLES_PER_TARGET"              = 20
      "LANGUAGE_BUCKET_LOCATION"                = "oci://${var.bucket_url}/supervisor/language/"
      "RECTIFY_JOB_REPEAT_INTERVAL_IN_DAYS"     = var.supervisor_ask_ingestion_job_schedule_interval
      "FEATURE_COLUMNS"                         = "[\"text\"]"
      "TARGET_COLUMNS"                          = "[\"labels\"]"
      "COMPARTMENT_ID"                          = resource.oci_identity_compartment.tf_compartment.id
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

resource oci_datascience_job_run "tf_rectify_job_run" {
  #Required
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  job_id = oci_datascience_job.tf_rectify_job.id
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
* Create Supervisor Ask Language Classification Job
*/
resource oci_datascience_job "tf_lang_classifier_job" {
  depends_on = [oci_datascience_job_run.tf_rectify_one_time_job_run]
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  delete_related_job_runs = "true"
  #description = <<Optional value not found in discovery>>
  display_name = "Supervisor Ask Language Classification Job"
  freeform_tags = {
  }
  job_artifact = "${path.module}/../src.zip"
  artifact_content_disposition = "attachment; filename=src.zip"
  artifact_content_length = "${data.local_file.read_job_artifact_size.content}"
  job_configuration_details {
    environment_variables = {
      "JOB_RUN_ENTRYPOINT"           = "./jobs/common/run_job.sh"
      "JOB_NAME"                     = "build_model"
      "LANGUAGE_BUCKET_LOCATION"     = "oci://${var.bucket_url}/supervisor/language/"
      "PROJECT_NAME"                 = local.project-name
      "MODEL_TRAINING_INTERVAL"      = var.supervisor_ask_training_job_schedule_interval
      "MODEL_ENDPOINT_NAME"          = var.supervisor_ask_model_name
      "PROJECT_ID"                   = data.local_file.lang_project_id.content
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

resource oci_datascience_job_run "tf_lang_classifier_job_run" {
  #Required
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  job_id = oci_datascience_job.tf_lang_classifier_job.id
  project_id = oci_datascience_project.tf_project.id
  asynchronous   = true
  job_configuration_override_details {
    job_type = "DEFAULT"
  }
  timeouts {
    create = "90h"
  }
}

