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
#  SHA1: $Id: 4dda5697ff556feb4c7a6022759dffd7ad1164d6 $
################################################################################################
#  File: provider.tf
################################################################################################
####################################
# OCI Authentication
####################################

provider "oci" {
   auth = "InstancePrincipal"
   region = "${var.region}"
}

#provider "oci" {
#  tenancy_ocid = var.tenancy_ocid
#  config_file_profile= "DEFAULT"
#}