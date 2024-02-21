
################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2024, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 24B (February 2024) 
#  date: Thu Feb 15 09:26:01 IST 2024
 
#  revision: rnw-24-02-initial
#  SHA1: $Id: 5a229164ff2b4d2a36080405a79ab4d7e56ce229 $
################################################################################################
#  File: object_storage.tf
################################################################################################
####################################
# DataScience related resources
####################################

####################################
# Push Conda Object to the Bucket
####################################

data oci_objectstorage_namespace "tf_namespace" {
  compartment_id = oci_identity_compartment.tf_compartment.id
}

resource oci_objectstorage_bucket "tf_bucket" {
  access_type           = "NoPublicAccess"
  auto_tiering          = "Disabled"
  compartment_id        = oci_identity_compartment.tf_compartment.id
  name                  = var.bucket_url
  namespace             = data.oci_objectstorage_namespace.tf_namespace.namespace
  object_events_enabled = "false"
  storage_tier          = "Standard"
  versioning            = "Disabled"
}