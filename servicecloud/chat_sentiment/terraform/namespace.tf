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
#  SHA1: $Id: 444d80a5c954d4004809c680e837aebe9ad20b9d $
################################################################################################
#  File: namespace.tf
################################################################################################
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
