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
#  SHA1: $Id: 2b650fe3c5b8e510b1e1ca11491d9eeebd8c1715 $
################################################################################################
#  File: gateway.tf
################################################################################################
## This configuration was generated by terraform-provider-oci
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
    request_policies {
      authentication {
        cache_key = [
          "Authorization",
        ]
        function_id                 = oci_functions_function.tf_fn_auth.id
        is_anonymous_access_allowed = "false"
        parameters = {
          "Authorization" = "request.headers[Authorization]"
        }
        type = "CUSTOM_AUTHENTICATION"
        validation_failure_policy {
          type = "MODIFY_RESPONSE"
        }
      }
      mutual_tls {
        allowed_sans = [
        ]
        is_verified_certificate_required = "false"
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
      request_policies {
        authorization {
          type = "AUTHENTICATION_ONLY"
        }
      }
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

