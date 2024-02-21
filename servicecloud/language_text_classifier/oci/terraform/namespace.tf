
################################################################################################
#  $ACCELERATOR_HEADER_PLACE_HOLDER$
#  SHA1: $Id:  $
################################################################################################
#  File: $ACCELERATOR_HEADER_FILE_NAME_PLACE_HOLDER$
################################################################################################
####################################
# DataScience related resources
####################################

/*
* Store Language Classification Model Id in model.txt
*/
resource null_resource "retrieve_namespace" {
  triggers  =  { always_run = "${timestamp()}" }
  provisioner "local-exec" {
    command = "sh ${path.module}/scripts/retrieve_namespace.sh ${resource.oci_identity_compartment.tf_compartment.id}"
  }
}

data local_file "namespace" {
    depends_on = [null_resource.retrieve_namespace]
    filename = "${path.module}/namespace.txt"
}
