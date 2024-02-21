
################################################################################################
#  $ACCELERATOR_HEADER_PLACE_HOLDER$
#  SHA1: $Id:  $
################################################################################################
#  File: $ACCELERATOR_HEADER_FILE_NAME_PLACE_HOLDER$
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