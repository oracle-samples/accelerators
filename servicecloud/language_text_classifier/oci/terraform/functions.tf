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
#  SHA1: $Id: c16be307e375910fd4301dfe08ae62fa89403a1b $
################################################################################################
#  File: functions.tf
################################################################################################
####################################
# DataScience related resources
####################################

resource null_resource "save_region_code" {
  triggers  =  { always_run = "${timestamp()}" }
  provisioner "local-exec" {
    command = "sh get_region_code.sh ${var.region} ${path.module}/region.txt"
  }
}

data "local_file" "region_code" {
  depends_on = [null_resource.save_region_code]
  filename = "${path.module}/region.txt"
}

data "oci_artifacts_container_repositories" "tf_container_repositories" {
    compartment_id = resource.oci_identity_compartment.tf_compartment.id
    display_name = "${local.repo-name}_${oci_identity_compartment.tf_compartment.name}"
}

resource "oci_artifacts_container_repository" "tf_container_repository" {
    compartment_id = resource.oci_identity_compartment.tf_compartment.id
    display_name = "${local.repo-name}_${oci_identity_compartment.tf_compartment.name}"
    is_immutable = "false"
}

resource "oci_functions_application" "tf_application" {
    depends_on = [null_resource.check_for_deployed_model]
    compartment_id = resource.oci_identity_compartment.tf_compartment.id
    display_name = local.application_display_name
    subnet_ids = [oci_core_subnet.tf_private_subnet.id]
    config = {
         ENDPOINT_OCID = data.local_file.endpoint_id.content
    }
}


locals {
  fnroot    = "${abspath(path.root)}/./../function"
  rawfndata = yamldecode(file("${local.fnroot}/func.yaml"))
  fndata = {
    name    = local.rawfndata.name
    version = local.rawfndata.version
    memory  = local.rawfndata.memory
    timeout = try(local.rawfndata.timeout, 30)
    image   = "${data.local_file.region_code.content}.ocir.io/${data.local_file.namespace.content}/${oci_artifacts_container_repository.tf_container_repository.display_name}:predictor"
  }
}

resource null_resource "save_auth_token" {
  count     = var.user_auth_token != "" ? 1 : 0
  triggers  =  { always_run = "${timestamp()}" }
  provisioner "local-exec" {
    command     = <<-EOT
      oci secrets secret-bundle get --secret-id ${oci_vault_secret.tf_oci_auth_token.id} | jq -r '.data."secret-bundle-content".content' | base64 -d | tr -d '\n' >> ${path.module}/token.txt
    EOT
  }
}

resource null_resource "create_auth_token_with_config" {
  count     = var.user_auth_token != "" ? 0 : 1
  depends_on = [null_resource.save_job_artifact_size]
  triggers  =  { always_run = "${timestamp()}" }
  provisioner "local-exec" {
    command     = <<-EOT
      oci iam auth-token delete --force --user-id ${var.user_ocid} --auth-token-id $(oci iam auth-token list --user-id ${var.user_ocid} | jq -r '.data[0].id') 2> /dev/null
      oci iam auth-token create --description "registry" --user-id ${var.user_ocid} | jq -r '.data.token' | tail -1 | tr -d '\n' >> ${path.module}/token.txt
    EOT
  }
}

resource null_resource "login2ocir_with_config" {
  depends_on = [null_resource.create_auth_token_with_config, null_resource.save_auth_token]
  triggers = {
    always_run = "${timestamp()}"
  }
  provisioner "local-exec" {
    command     = <<-EOT
      sleep 20
      cat ${path.module}/token.txt | docker login --username '${data.local_file.namespace.content}/${data.oci_identity_user.tf_user.name}' ${data.local_file.region_code.content}.ocir.io --password-stdin
    EOT
  }
}


resource null_resource "build_function" {
  depends_on = [null_resource.check_for_deployed_model, null_resource.login2ocir_with_config, oci_artifacts_container_repository.tf_container_repository]
  triggers = {
    fnversion = local.fndata.version
  }
  provisioner "local-exec" {
    command = "cat ${path.module}/model.txt"
  }
  provisioner "local-exec" {
    command = "docker build . -t ${data.local_file.region_code.content}.ocir.io/${data.local_file.namespace.content}/${oci_artifacts_container_repository.tf_container_repository.display_name}:predictor"
    working_dir = local.fnroot
  }
}

resource null_resource "deploy_function" {
  depends_on = [null_resource.check_for_deployed_model, null_resource.login2ocir_with_config, null_resource.build_function]
  triggers = {
    fnversion = local.fndata.version
  }
  provisioner "local-exec" {
    command     = "docker push ${data.local_file.region_code.content}.ocir.io/${data.local_file.namespace.content}/${oci_artifacts_container_repository.tf_container_repository.display_name}:predictor"
    working_dir = local.fnroot
  }
}


# Query OCIR to get the images digest
data oci_artifacts_container_images "predictor" {
  depends_on = [null_resource.deploy_function]
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  repository_id = oci_artifacts_container_repository.tf_container_repository.id
  repository_name = oci_artifacts_container_repository.tf_container_repository.display_name
  display_name = "${oci_artifacts_container_repository.tf_container_repository.display_name}:predictor"
  #Ex: name would be"${var.ocir_ns}/${var.application_name}/${each.key}:${each.value.version}"
}

resource "oci_functions_function" "tf_fn_predictor" {
  depends_on = [null_resource.deploy_function]
  application_id            = oci_functions_application.tf_application.id
  # No name suffix is added to keep compatibility with the Fn CLI
  display_name              = local.fndata.name
  image                     = local.fndata.image
  memory_in_mbs             = local.fndata.memory
  timeout_in_seconds        = local.fndata.timeout
  image_digest              = data.oci_artifacts_container_images.predictor.container_image_collection[0].items[0].digest
}
