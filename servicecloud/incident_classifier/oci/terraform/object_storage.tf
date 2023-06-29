################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:28 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: aeaeaf8ebb75df3d4949c51ec65809a4b3254dcb $
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
