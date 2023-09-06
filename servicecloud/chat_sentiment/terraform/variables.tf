################################################################################################
#  This file is part of the Oracle B2C Service Accelerator Reference Integration set published
#  by Oracle B2C Service licensed under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23C (August 2023) 
#  date: Tue Aug 22 11:57:51 IST 2023
 
#  revision: RNW-23C
#  SHA1: $Id: 488218b36ea62ec647dbdf1e107fd29b67306481 $
################################################################################################
#  File: variables.tf
################################################################################################

variable "region" {
}

variable "tenancy_ocid" {
}

variable "compartment" {
    type        = string
    default = "sentiment_accelerator"
    description = "Please provide compartment name"
    validation {
        condition     = can(regex("^[a-z0-9_]+$", var.compartment))
        error_message = "Variable must contain only lowercase letters, numerical values from 0-9, and underscores."
    }
}

variable "group" {
    type        = string
    default = "accelerator_group"
    description = "Please provide group name. Make sure this name is unique across the root compartment"
}

variable "dynamic_group" {
    type        = string
    default = "accelerator_dynamic_group"
    description = "Please provide dynamic group name."
}

# VCN related CIDR_BLOCK config
variable "network_public_subnet_cidr_block" {
    type        = string
    default = "10.0.0.0/24"
}

variable "network_private_subnet_cidr_block" {
    type        = string
    default = "10.0.1.0/24"
}

# Language Service related variables
variable "supervisor_ask_model_name" {
    type        = string
    default = "supervisor_ask_model_endpoint"
    description = "Please provide language model name"
}

variable "emotions_model_name" {
    type        = string
    default = "emotion_model_endpoint"
    description = "Please provide language model name"
}

variable "num_inference_unit" {
    type        = number
    default     = 1
    description = "Please provide number of inference unit you want to attach for serving the model"
}

# Data Science related variables
variable "ingestion_job_shape" {
    type        = string
    default = "VM.Standard2.1"
    description = "Above compute size VM would be used to ingest the data into object storage"
}

variable "supervisor_ask_ingestion_job_schedule_interval" {
    type        = number
    default     = 1
    description = "Please provide supervisor ask ingestion job schedule interval in days."
}


variable "emotion_ingestion_job_schedule_interval" {
    type        = number
    default     = 1
    description = "Please provide emotion ingestion job schedule interval in days."
}

variable "emotion_threshold" {
    type        = number
    default     = 70
    description = "Please provide emotion threshold to consider for prediction."
}

variable "emotion_percentage_for_positive_samples" {
    type        = number
    default     = 10
    description = "Please provide percentage contribution for positive samples."
}

variable "emotion_percentage_for_neutral_samples" {
    type        = number
    default     = 10
    description = "Please provide percentage contribution for neutral samples."
}

variable "emotions_inactive_chat_status_update_interval" {
    type        = number
    default     = 1
    description = "Please provide chat status update job schedule interval in minutes."
}

variable "supervisor_ask_training_job_schedule_interval" {
    type        = number
    default     = 1
    description = "Please provide training job schedule interval in days."
}

variable "emotions_training_job_schedule_interval" {
    type        = number
    default     = 7
    description = "Please provide training job schedule interval in days."
}

variable "authorization_authtype" {
  type        = string
  description = "The type of authorization needs to use while ingesting the data. (BASIC | OAUTH)"

  validation {
    condition     = var.authorization_authtype == "BASIC" || var.authorization_authtype == "OAUTH"
    error_message = "The authorization_authtype value should be either \"BASIC\" or \"OAUTH\"."
  }
}

variable "authorization_basic_b2c_auth" {
  default = ""
  description = "Please enter your cx b2c site auth: Ex: c2hlbGJ5LnRlc3Q6UGFzc3dvcmQx"
  sensitive   = true
}


variable "authorization_oauth_user" {
  default = ""
  description = "Please provide your OAUTH User"
}

variable "authorization_oauth_entity" {
  default = ""
  description = "Please provide your OAUTH entity"
}

variable "authorization_oauth_path" {
  default = ""
  description = "Please provide your OAUTH path"
}

variable "authorization_oauth_cx_rest_api_key" {
  default = ""
  description = "Please provide your Base64 encoded CX API Key"
  sensitive   = true
}

variable "emotion_num_of_days_data_to_fetch" {
    type        = number
    default     = 365
    description = "Please enter number of days data need to be fetch for emotions. :Default is 365 days"
}

variable "domain" {
    type        = string
    description = "Please enter your cx b2c site domain: Ex: qa--22cga1.custhelp.com"
}

variable "bucket_url" {
    type        = string
    description = "Please provide bucket name"
}

variable "vault_id" {
    type        = string
    description = "Please provide vault ocid in which secret would be stored."
}


/*
 * Local Variables for resources
 */
locals {
  timestamp = "${timestamp()}"
  timestamp_in_numeric = "${replace("${local.timestamp}", "/[-| |T|Z|:]/", "")}"
  compartment-name = var.compartment
  group-name = var.group
  dynamic-group-name = var.dynamic_group
  policy-name = "accelerator_sentiment_policy"
  vcn-name = "accelerator_sentiment_vcn"
  project-name = "sentiment_accelerator"
  emotion-project-name = "emotions_accelerator"
  supervisor-project-name = "supervisor_accelerator"
  application_display_name = "accelerator_fn"
  log-group-name = "accelerator_sentiment_log_group"
  services = ["datasciencenotebooksession", "datasciencejobrun", "datasciencemodeldeployment", "fnfunc", "ailanguagemodel"]
  service-names    = formatlist(" all { resource.type='%s', resource.compartment.id='%s' }", local.services, resource.oci_identity_compartment.tf_compartment.id)
  matching-rules      = "any {${join(",",local.service-names)}}"
  repo-name = "accelerator_functions"
  secret-name = "secret_${local.timestamp_in_numeric}"
  oci-secret-name = "ocisecret_${local.timestamp_in_numeric}"
  oci-auth-secret-name = "ocisecretauth_${local.timestamp_in_numeric}"
  policy_statements = [ format("Allow service datascience to use virtual-network-family in compartment %s", resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow service faas to use apm-domains in compartment %s", resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow service faas to read repos in compartment %s where request.operation='ListContainerImageSignatures'", resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow service faas to {KEY_READ} in compartment %s where request.operation='GetKeyVersion'", resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow service faas to {KEY_VERIFY} in compartment %s where request.operation='Verify'", resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow group %s to read metrics in compartment %s", resource.oci_identity_group.tf_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow group %s to manage objects in compartment %s", resource.oci_identity_group.tf_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow group %s to manage log-groups in compartment %s", resource.oci_identity_group.tf_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow group %s to use log-content in compartment %s", resource.oci_identity_group.tf_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow group %s to use virtual-network-family in compartment %s", resource.oci_identity_group.tf_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow group %s to use object-family in compartment %s", resource.oci_identity_group.tf_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow group %s to manage repos in compartment %s", resource.oci_identity_group.tf_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow group %s to manage api-gateway-family in compartment %s", resource.oci_identity_group.tf_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow group %s to manage logging-family in compartment %s", resource.oci_identity_group.tf_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow group %s to read objectstorage-namespaces in compartment %s", resource.oci_identity_group.tf_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow group %s to use cloud-shell in compartment %s", resource.oci_identity_group.tf_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow group %s to use apm-domains in compartment %s", resource.oci_identity_group.tf_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow group %s to use keys in compartment %s", resource.oci_identity_group.tf_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow group %s to read virtual-network-family in compartment %s", resource.oci_identity_group.tf_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow group %s to use ai-service-language-family in compartment %s", resource.oci_identity_group.tf_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow dynamic-group %s to use log-content in compartment %s", resource.oci_identity_dynamic_group.tf_dynamic_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow dynamic-group %s to read virtual-network-family in compartment %s", resource.oci_identity_dynamic_group.tf_dynamic_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow dynamic-group %s to use object-family in compartment %s", resource.oci_identity_dynamic_group.tf_dynamic_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow dynamic-group %s to manage data-science-family in compartment %s", resource.oci_identity_dynamic_group.tf_dynamic_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow dynamic-group %s to inspect vcns in compartment %s", resource.oci_identity_dynamic_group.tf_dynamic_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow dynamic-group %s to manage objects in compartment %s", resource.oci_identity_dynamic_group.tf_dynamic_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow dynamic-group %s to manage all-resources in compartment %s", resource.oci_identity_dynamic_group.tf_dynamic_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow dynamic-group %s to manage functions-family in compartment %s", resource.oci_identity_dynamic_group.tf_dynamic_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow dynamic-group %s to use log-groups in compartment %s", resource.oci_identity_dynamic_group.tf_dynamic_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("Allow dynamic-group %s to use ai-service-language-family in compartment %s", resource.oci_identity_dynamic_group.tf_dynamic_group.name, resource.oci_identity_compartment.tf_compartment.name),
                        format("ALLOW any-user to use functions-family in compartment %s where ALL {request.principal.type= 'ApiGateway', request.resource.compartment.id = '%s'}", resource.oci_identity_compartment.tf_compartment.name, resource.oci_identity_compartment.tf_compartment.id)]
}
