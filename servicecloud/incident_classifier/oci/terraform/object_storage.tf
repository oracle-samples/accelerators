################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:56 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 0b4e9e63da6e732e7f4b1d675ca7ece300159361 $
################################################################################################
#  File: object_storage.tf
################################################################################################
####################################
# Push Conda Object to the Bucket
####################################

data oci_objectstorage_namespace "tf_namespace" {
  compartment_id = oci_identity_compartment.tf_compartment.id
}

resource oci_objectstorage_bucket "tf_bucket" {
  access_type    = "NoPublicAccess"
  auto_tiering   = "Disabled"
  compartment_id = oci_identity_compartment.tf_compartment.id
  name                  = var.bucket_name
  namespace             = data.oci_objectstorage_namespace.tf_namespace.namespace
  object_events_enabled = "false"
  storage_tier          = "Standard"
  versioning            = "Disabled"
}
