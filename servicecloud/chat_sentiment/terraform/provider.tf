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
#  SHA1: $Id: 7b6a9da2dc1c31480ca3dad4fb5f4f5a698af95d $
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
