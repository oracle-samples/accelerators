
################################################################################################
#  $ACCELERATOR_HEADER_PLACE_HOLDER$
#  SHA1: $Id:  $
################################################################################################
#  File: $ACCELERATOR_HEADER_FILE_NAME_PLACE_HOLDER$
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