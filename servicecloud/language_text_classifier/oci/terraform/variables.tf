
################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2024, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 24B (February 2024) 
#  date: Thu Feb 15 09:26:02 IST 2024
 
#  revision: rnw-24-02-initial
#  SHA1: $Id: f0f03036bacc8601d93acb812b3099fd1c2e9516 $
################################################################################################
#  File: variables.tf
################################################################################################
####################################
# DataScience related resources
####################################

variable "region" {
}

variable "tenancy_ocid" {
}

variable "compartment" {
    type        = string
    default = "accelerator_incident_classifier"
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
    default = "accelerator_dynamic_groupgst"
    description = "Please provide dynamic group name."
}

# VCN related CIDR_BLOCK config
variable "public_subnet_cidr_block" {
    type        = string
    default = "10.0.0.0/24"
}

variable "private_subnet_cidr_block" {
    type        = string
    default = "10.0.1.0/24"
}

# Language Service related variables
variable "model_name" {
    type        = string
    default = "generic_model_endpoint"
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

variable "ingestion_job_schedule_interval" {
    type        = number
    default     = 1
    description = "Please provide ingestion job schedule interval in hours."
}

variable "training_job_schedule_interval" {
    type        = number
    default     = 24
    description = "Please provide training job schedule interval in hours."
}

variable "feature_columns" {
    type        = string
    default     = "[\"Subject\", \"Text\"]"
    description = "Please provide feature columns which needs to be used for training the language model"
}

variable "target_columns" {
    type        = string
    default     = "[\"Category ID\"]"
    description = "Please provide target column which needs to be predicted from language model"
}

variable "min_samples_per_target" {
    type        = number
    default     = 20
    description = "Please provide minimum samples to use per category while training"
}

variable "authorization_type" {
  type        = string
  description = "The type of authorization needs to use while ingesting the data. (BASIC | OAUTH)"

  validation {
    condition     = var.authorization_type == "BASIC" || var.authorization_type == "OAUTH"
    error_message = "The authorization_type value should be either \"BASIC\" or \"OAUTH\"."
  }
}

variable "b2c_auth" {
  default = ""
  description = "Please enter your cx b2c site auth: Ex: c2hlbGJ5LnRlc3Q6UGFzc3dvcmQx"
  sensitive   = true
}


variable "oauth_user" {
  default = ""
  description = "Please provide your OAUTH User"
}

variable "oauth_entity" {
  default = ""
  description = "Please provide your OAUTH entity"
}

variable "oauth_path" {
  default = ""
  description = "Please provide your OAUTH path"
}

variable "cx_rest_api_key" {
  default = ""
  description = "Please provide your Base64 encoded CX API Key"
  sensitive   = true
}

variable "num_of_days_data_to_fetch" {
    type        = number
    default     = 365
    description = "Please enter number of days data neeed to be fetch. :Default is 365 days"
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

variable "user_ocid" {
  type        = string
  description = "Please provide your Base64 encoded OCI user ocid"
  sensitive   = true
}

variable "user_auth_token" {
  default = ""
  description = "Please provide your Base64 encoded user auth token of oci"
  sensitive   = true
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
  policy-name = "accelerator_incident_classifier_policy"
  vcn-name = "accelerator_incident_classifier_vcn"
  project-name = "b2c_incident_classifier"
  application_display_name = "accelerator_fn"
  log-group-name = "accelerator_incident_classifier_log_group"
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
