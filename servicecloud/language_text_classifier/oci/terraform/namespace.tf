
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
#  SHA1: $Id: c0f211b33ba8eef264d83174fbab1e674842f05a $
################################################################################################
#  File: namespace.tf
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
