################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 24A (March 2024) 
#  date: Fri March 22 11:08:27 IST 2024
 
#  revision: rnw-24-03-initial
#  SHA1: $Id: bb5648f30c911a43c6846facc28ee344673848e1 $
################################################################################################
#  File: datascience.tf
################################################################################################
####################################
# DataScience related resources
####################################

resource null_resource "save_job_artifact_size" {
  triggers  =  { always_run = "${timestamp()}" }
  provisioner "local-exec" {
    command     = <<-EOT
      zip -r ${path.module}/src.zip ${path.module}/src/*
      wc -c ${path.module}/src.zip | awk '{print $1}' | tail -1 | tr -d '\n' >> ${path.module}/artifact_size.txt
    EOT
    working_dir = "${abspath(path.root)}/./../classifier"
  }
}

data local_file "read_job_artifact_size" {
    depends_on = [null_resource.save_job_artifact_size]
    filename = "${abspath(path.root)}/./../classifier/artifact_size.txt"
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
        command = "sh ./scripts/destroy_models.sh ${self.compartment_id}"
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
* Publish Conda Job
*/
resource oci_datascience_job "tf_publish_conda" {
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  delete_related_job_runs = "true"
  display_name = "Run Conda Selenium Job"
  freeform_tags = {
  }
  job_artifact = "${path.module}/../classifier/src.zip"
  artifact_content_disposition = "attachment; filename=src.zip"
  artifact_content_length = "${data.local_file.read_job_artifact_size.content}"
  job_configuration_details {
    environment_variables = {
      "JOB_RUN_ENTRYPOINT"           = "./src/training_scripts/jobs/run_selenium.sh"
      "RAW_GIT_ENV_URL"              = "${var.environment_yaml_path}"
      "OCI_USER_ID"                  = "${var.user_ocid}"
      "OCI_FINGERPRINT"              = "${var.user_fingerprint}"
      "OCI_REGION"                   = "${var.region}"
      "OCI_TENANCY_ID"               = "${var.tenancy_ocid}"
      "OCI_PRIVATE_KEY"              = oci_vault_secret.tf_oci_private_secret.id
      "BUCKET_NAME"                  = "${var.bucket_name}"
      "TENANCY"                      = data.oci_identity_tenancy.tf_tenancy.name
      "NAMESPACE"                    = data.oci_objectstorage_namespace.tf_namespace.namespace
    }
    job_type = "DEFAULT"
  }
  job_infrastructure_configuration_details {
    block_storage_size_in_gbs = "50"
    job_infrastructure_type   = "STANDALONE"
    shape_name = "VM.Standard3.Flex"
    job_shape_config_details {
        memory_in_gbs = 32
        ocpus = 1
    }
    subnet_id  = oci_core_subnet.tf_private_subnet.id
  }
  job_log_configuration_details {
    enable_auto_log_creation = "true"
    enable_logging           = "true"
    log_group_id             = oci_logging_log_group.tf_log_group.id
  }
  project_id = oci_datascience_project.tf_project.id
  depends_on = [oci_objectstorage_bucket.tf_bucket]
  provisioner "local-exec" {
    when    = destroy
    command = "sh ./scripts/destroy_jobs.sh ${self.compartment_id}"
    working_dir = path.module
  }
}

resource oci_datascience_job_run "tf_publish_conda_job_run" {
  #Required
  depends_on = [oci_objectstorage_bucket.tf_bucket]
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  job_id = oci_datascience_job.tf_publish_conda.id
  project_id = oci_datascience_project.tf_project.id
  asynchronous   = false
  job_configuration_override_details {
    job_type = "DEFAULT"
  }

  timeouts {
    create = "2.5h"
  }
  provisioner "local-exec" {
    when    = destroy
    command = "oci data-science job-run cancel --job-run-id ${self.id}"
    on_failure = continue
  }
}

/*
* Create Ingestion Job
*/
resource oci_datascience_job "tf_one_time_ingestion_job" {
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  delete_related_job_runs = "true"
  #description = <<Optional value not found in discovery>>
  display_name = "Ingestion Job"
  freeform_tags = {
  }
  job_artifact = "${path.module}/../classifier/src.zip"
  artifact_content_disposition = "attachment; filename=src.zip"
  artifact_content_length = "${data.local_file.read_job_artifact_size.content}"
  job_configuration_details {
    environment_variables = {
      "BUCKET_URL"                   = "oci://${var.bucket_name}/ai4service/data/autoclassif/"
      "JOB_RUN_ENTRYPOINT"           = "./src/training_scripts/jobs/get_report.py"
      "JOB_NAME"                     = "ingestion_job"
      "CX_DOMAIN"                    = var.domain
      "AUTH_TYPE"                    = var.authorization_type
      "INGESTION_SECRET_ID"          = oci_vault_secret.tf_secret.id
      "OAUTH_USER"                   = var.oauth_user
      "OAUTH_ENTITY"                 = var.oauth_entity
      "OAUTH_PATH"                   = var.oauth_path
      "CONDA_ENV_BUCKET"             = var.bucket_name
      "CONDA_ENV_NAMESPACE"          = data.oci_objectstorage_namespace.tf_namespace.namespace
      "CONDA_ENV_OBJECT_NAME"        = "conda_environments/cpu/b2c_env/1.0/b2c_env_slug"
      "CONDA_ENV_REGION"             = "${var.region}"
      "CONDA_ENV_TYPE"               = "published"
      "compartment_id"               = resource.oci_identity_compartment.tf_compartment.id
      "subnet_id"                    = oci_core_subnet.tf_private_subnet.id
      "project_id"                   = oci_datascience_project.tf_project.id
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
  depends_on = [oci_vault_secret.tf_secret, oci_datascience_job.tf_publish_conda, oci_datascience_job_run.tf_publish_conda_job_run]
  provisioner "local-exec" {
    when    = destroy
    command = "sh ./scripts/destroy_jobs.sh ${self.compartment_id}"
    working_dir = path.module
  }
}

resource oci_datascience_job_run "tf_one_time_ingestion_job_run" {
  #Required
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  job_id = oci_datascience_job.tf_one_time_ingestion_job.id
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
* Scheduled Ingestion Job
*/
resource oci_datascience_job "tf_schedule_ingestion_job" {
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  delete_related_job_runs = "true"
  #description = <<Optional value not found in discovery>>
  display_name = "Scheduled Ingestion Job"
  freeform_tags = {
  }
  job_artifact = "${path.module}/../classifier/src.zip"
  artifact_content_disposition = "attachment; filename=src.zip"
  artifact_content_length = "${data.local_file.read_job_artifact_size.content}"
  job_configuration_details {
    #command_line_arguments = <<Optional value not found in discovery>>
    environment_variables = {
      "BUCKET_URL"                   = "oci://${var.bucket_name}/ai4service/data/autoclassif/"
      "JOB_RUN_ENTRYPOINT"           = "./src/training_scripts/jobs/schedule_job.py"
      "JOB_NAME"                     = "ingestion_job"
      "CX_DOMAIN"                    = var.domain
      "AUTH_TYPE"                    = var.authorization_type
      "INGESTION_SECRET_ID"          = oci_vault_secret.tf_secret.id
      "OAUTH_USER"                   = var.oauth_user
      "OAUTH_ENTITY"                 = var.oauth_entity
      "OAUTH_PATH"                   = var.oauth_path
      "CONDA_ENV_BUCKET"             = var.bucket_name
      "CONDA_ENV_NAMESPACE"          = data.oci_objectstorage_namespace.tf_namespace.namespace
      "CONDA_ENV_OBJECT_NAME"        = "conda_environments/cpu/b2c_env/1.0/b2c_env_slug"
      "CONDA_ENV_REGION"             = "${var.region}"
      "CONDA_ENV_TYPE"               = "published"
      "compartment_id"               = resource.oci_identity_compartment.tf_compartment.id
      "subnet_id"                    = oci_core_subnet.tf_private_subnet.id
      "project_id"                   = oci_datascience_project.tf_project.id
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
  depends_on = [oci_datascience_job_run.tf_one_time_ingestion_job_run, oci_vault_secret.tf_secret]
  provisioner "local-exec" {
    when    = destroy
    command = "sh ./scripts/destroy_jobs.sh ${self.compartment_id}"
    working_dir = path.module
  }
}

resource "oci_datascience_job_run" "tf_schedule_ingestion_job_run" {
  #Required
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  job_id = oci_datascience_job.tf_schedule_ingestion_job.id
  project_id = oci_datascience_project.tf_project.id
  asynchronous   = true
  job_configuration_override_details {
    job_type = "DEFAULT"
  }
  depends_on = [oci_datascience_job_run.tf_one_time_ingestion_job_run]
  provisioner "local-exec" {
    when    = destroy
    command = "oci data-science job-run cancel --job-run-id ${self.id}"
    on_failure = continue
  }
}

/*
* Build Model through Job
*/
resource oci_datascience_job "tf_schedule_build_model_job" {
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  delete_related_job_runs = "true"
  #description = <<Optional value not found in discovery>>
  display_name = "Scheduled Build Model Job"
  freeform_tags = {
  }
  job_artifact = "${path.module}/../classifier/src.zip"
  artifact_content_disposition = "attachment; filename=src.zip"
  artifact_content_length = "${data.local_file.read_job_artifact_size.content}"
  job_configuration_details {
    #command_line_arguments = <<Optional value not found in discovery>>
    environment_variables = {
      "BUCKET_URL"                   = "oci://${var.bucket_name}/ai4service/data/autoclassif/"
      "JOB_RUN_ENTRYPOINT"           = "./src/training_scripts/jobs/schedule_job.py"
      "JOB_NAME"                     = "build_model"
      "CONDA_ENV_BUCKET"             = var.bucket_name
      "CONDA_ENV_NAMESPACE"          = data.oci_objectstorage_namespace.tf_namespace.namespace
      "CONDA_ENV_OBJECT_NAME"        = "conda_environments/cpu/b2c_env/1.0/b2c_env_slug"
      "CONDA_ENV_REGION"             = "${var.region}"
      "CONDA_ENV_TYPE"               = "published"
      "compartment_id"               = resource.oci_identity_compartment.tf_compartment.id
      "subnet_id"                    = oci_core_subnet.tf_private_subnet.id
      "project_id"                   = oci_datascience_project.tf_project.id
    }
    job_type = "DEFAULT"
  }
  job_infrastructure_configuration_details {
    block_storage_size_in_gbs = "50"
    job_infrastructure_type   = "STANDALONE"
    shape_name = var.build_model_shape
    subnet_id  = oci_core_subnet.tf_private_subnet.id
  }
  job_log_configuration_details {
    enable_auto_log_creation = "true"
    enable_logging           = "true"
    log_group_id             = oci_logging_log_group.tf_log_group.id
  }
  project_id = oci_datascience_project.tf_project.id
  depends_on = [oci_datascience_job_run.tf_one_time_ingestion_job_run]
  provisioner "local-exec" {
    when    = destroy
    command = "sh ./scripts/destroy_jobs.sh ${self.compartment_id}"
    working_dir = path.module
  }
}

resource "oci_datascience_job_run" "tf_schedule_build_model_job_run" {
  #Required
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  job_id = oci_datascience_job.tf_schedule_build_model_job.id
  project_id = oci_datascience_project.tf_project.id
  asynchronous   = true
  job_configuration_override_details {
    job_type = "DEFAULT"
  }
  depends_on = [oci_datascience_job_run.tf_one_time_ingestion_job_run]
  provisioner "local-exec" {
    when    = destroy
    command = "oci data-science job-run cancel --job-run-id ${self.id}"
    on_failure = continue
  }
}

resource null_resource "check_for_deployed_model" {
  depends_on = [oci_datascience_job_run.tf_schedule_build_model_job_run]
  triggers  =  { always_run = "${timestamp()}" }
  provisioner "local-exec" {
    command = "sh ${path.module}/download_model.sh ${resource.oci_identity_compartment.tf_compartment.id} ${path.module}"
  }
}

data local_file "model_id" {
    depends_on = [null_resource.check_for_deployed_model]
    filename = "${path.module}/model.txt"
}
