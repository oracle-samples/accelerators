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
#  SHA1: $Id: 1bc309c1bb7b4fa13535c1e2770ed999d1942f0d $
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
