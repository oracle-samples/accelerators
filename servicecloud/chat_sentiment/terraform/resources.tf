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
#  SHA1: $Id: 5e38285e556f0c6277cfc99fb001e7074dbdcb31 $
################################################################################################
#  File: resources.tf
################################################################################################

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

/*
 * Group for Incident Classifier
 */
resource "oci_identity_group" "tf_group" {
    compartment_id = var.tenancy_ocid
    description = "Group for Accelerator Incident Classifier"
    name = local.group-name
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
        content = var.authorization_authtype == "BASIC" ? var.authorization_basic_b2c_auth : var.authorization_oauth_cx_rest_api_key
        name = "INGESTION_SECRET_${local.timestamp_in_numeric}"
    }
    secret_name = local.secret-name
    vault_id = data.oci_kms_vault.tf_vault.id
    key_id = oci_kms_key.tf_key.id
}
