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
#  SHA1: $Id: 7260b65664520609da88740e62469691b12451b3 $
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
  access_type           = "NoPublicAccess"
  auto_tiering          = "Disabled"
  compartment_id        = oci_identity_compartment.tf_compartment.id
  name                  = var.bucket_url
  namespace             = data.oci_objectstorage_namespace.tf_namespace.namespace
  object_events_enabled = "false"
  storage_tier          = "Standard"
  versioning            = "Disabled"
}