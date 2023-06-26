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
#  SHA1: $Id: b38ec3900b00082091f13bd97c2b3d7932dc5055 $
################################################################################################
#  File: outputs.tf
################################################################################################
#output "images" {
#  value = data.oci_artifacts_container_images.predictor
#}

data "oci_apigateway_gateway" "api_gateway" {
  gateway_id = oci_apigateway_gateway.accelerator_gateway.id
}

output "api_gateway" {
  value = {
    ips      = data.oci_apigateway_gateway.api_gateway.ip_addresses
    hostname = data.oci_apigateway_gateway.api_gateway.hostname
  }
}