
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
#  SHA1: $Id: 665a2ff382fff9c9566d66cd881d54a2e5fac5cf $
################################################################################################
#  File: outputs.tf
################################################################################################
####################################
# DataScience related resources
####################################

data "oci_apigateway_gateway" "api_gateway" {
  gateway_id = oci_apigateway_gateway.accelerator_gateway.id
}

output "api_gateway" {
  value = {
    ips      = data.oci_apigateway_gateway.api_gateway.ip_addresses
    hostname = data.oci_apigateway_gateway.api_gateway.hostname
  }
}