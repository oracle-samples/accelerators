
################################################################################################
#  $ACCELERATOR_HEADER_PLACE_HOLDER$
#  SHA1: $Id:  $
################################################################################################
#  File: $ACCELERATOR_HEADER_FILE_NAME_PLACE_HOLDER$
################################################################################################
####################################
# DataScience related resources
####################################

resource oci_apigateway_gateway "accelerator_gateway" {
  depends_on = [oci_functions_function.tf_fn_predictor]
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  display_name  = "accelerator_gateway"
  endpoint_type = "PUBLIC"
  network_security_group_ids = [
  ]
  response_cache_details {
    type = "NONE"
  }
  subnet_id = oci_core_subnet.tf_public_subnet.id
}


resource oci_apigateway_deployment "tf_deployment" {
  depends_on = [oci_functions_function.tf_fn_predictor]
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  display_name = "incident_classifier"
  gateway_id  = oci_apigateway_gateway.accelerator_gateway.id
  path_prefix = "/accelerator"
  specification {
    logging_policies {
      #access_log = <<Optional value not found in discovery>>
      execution_log {
        is_enabled = "true"
        log_level  = "INFO"
      }
    }
    routes {
      backend {
        function_id = oci_functions_function.tf_fn_predictor.id
        type = "ORACLE_FUNCTIONS_BACKEND"
      }
      logging_policies {
        #access_log = <<Optional value not found in discovery>>
        execution_log {
          #is_enabled = <<Optional value not found in discovery>>
          log_level = ""
        }
      }
      methods = [
        "POST",
      ]
      path = "/predict"
      response_policies {
      }
    }
  }
  provisioner "local-exec" {
    when    = destroy
    command = "oci os bucket delete -bn oci-logs._apigateway.${self.compartment_id} --force --empty"
    on_failure = continue
  }
}

