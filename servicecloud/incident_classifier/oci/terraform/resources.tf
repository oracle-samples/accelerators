################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:57 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: cf74866bf9a20e7f56ee0e158867a1d9792ba4c5 $
################################################################################################
#  File: resources.tf
################################################################################################
#################################################
# General OCI Resources Required for Accelerator
#################################################

/*
 * Create specific compartment where we will create Incident Classifier related resources
 */
resource "oci_identity_compartment" "tf_compartment" {
    # Required
    compartment_id = var.tenancy_ocid
    description = "Compartment for Incident Classifier Resources."
    name = local.compartment-name
    provisioner "local-exec" {
        when    = destroy
        command = "oci iam compartment delete -c ${self.id} --force"
    }
}

resource "oci_identity_user" "tf_accelerator_user" {
    #Required
    compartment_id = var.tenancy_ocid
    description = "User for Accelerator"
    name = "accelerator_user_${oci_identity_compartment.tf_compartment.name}"
}

resource "oci_identity_user_capabilities_management" "test_user_capabilities_management" {
    #Required
    user_id = oci_identity_user.tf_accelerator_user.id

    #Optional
    can_use_api_keys             = "true"
    can_use_auth_tokens          = "true"
    can_use_console_password     = "true"
    can_use_customer_secret_keys = "true"
    can_use_smtp_credentials     = "true"
}

/*
 * Group for Incident Classifier
 */
resource "oci_identity_group" "tf_group" {
    compartment_id = var.tenancy_ocid
    description = "Group for Accelerator Incident Classifier"
    name = local.group-name
}

resource "oci_identity_user_group_membership" "tf_user_group_membership" {
    #Required
    group_id = oci_identity_group.tf_group.id
    user_id = oci_identity_user.tf_accelerator_user.id
}

/*
 * Dynamic Group with matching rules
 */
resource "oci_identity_dynamic_group" "tf_dynamic_group" {
    compartment_id = var.tenancy_ocid
    description = "Dynamic Group for Incident Classification"
    matching_rule = local.matching-rules
    name = local.dynamic-group-name
}

resource "oci_identity_policy" "tf_policy" {
    #Required
    compartment_id = resource.oci_identity_compartment.tf_compartment.id
    description = "Policy for Incident Classifier"
    name = local.policy-name
    statements = local.policy_statements
}

data "oci_kms_vault" "tf_vault" {
    #Required
    vault_id = var.vault_id
}

resource "oci_kms_key" "tf_key" {
    #Required
    compartment_id = oci_identity_compartment.tf_compartment.id
    display_name = "b2c_private_key"
    key_shape {
        algorithm = "AES"
        length = 24
    }
    management_endpoint = data.oci_kms_vault.tf_vault.management_endpoint
}

resource "oci_vault_secret" "tf_secret" {
    compartment_id = oci_identity_compartment.tf_compartment.id
    description = "For Ingesting the data from B2C Site"
    secret_content {
        content_type = "BASE64"
        content = var.authorization_type == "BASIC" ? var.b2c_auth : var.cx_rest_api_key
        name = "INGESTION_SECRET_${local.timestamp_in_numeric}"
    }
    secret_name = local.secret-name
    vault_id = data.oci_kms_vault.tf_vault.id
    key_id = oci_kms_key.tf_key.id
}
