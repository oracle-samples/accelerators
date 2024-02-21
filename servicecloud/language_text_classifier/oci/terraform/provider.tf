
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
#  SHA1: $Id: 46147e14072ec803fabb8b18b103fecbc9c5ff92 $
################################################################################################
#  File: provider.tf
################################################################################################
####################################
# DataScience related resources
####################################

####################################
# OCI Authentication
####################################

provider "oci" {
   auth = "InstancePrincipal"
   region = "${var.region}"
}
