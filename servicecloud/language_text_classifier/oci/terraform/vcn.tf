################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2024, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 24B (February 2024) 
#  date: Thu Feb 15 09:26:02 IST 2024
 
#  revision: rnw-24-02-initial
#  SHA1: $Id: 207268f4993b56472adb8273590e8260832bd3b8 $
################################################################################################
#  File: vcn.tf
################################################################################################
####################################
# DataScience related resources
####################################

resource "oci_core_vcn" "tf_vcn" {
    compartment_id = resource.oci_identity_compartment.tf_compartment.id
    display_name = local.vcn-name
    dns_label    = "cxaiservicevcn"
    cidr_blocks = [
        "10.0.0.0/16",
    ]
}


resource oci_core_nat_gateway "tf_nat_gateway_vcn" {
  block_traffic  = "false"
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  defined_tags = {
  }
  display_name = "NAT Gateway-cx-vcn"
  #public_ip_id = "ocid1.publicip.oc1.phx."
  vcn_id = oci_core_vcn.tf_vcn.id
}

resource oci_core_internet_gateway "tf_internet_gateway" {
    #Required
    compartment_id = resource.oci_identity_compartment.tf_compartment.id
    vcn_id = oci_core_vcn.tf_vcn.id

    #Optional
    enabled = "true"
    #route_table_id = oci_core_vcn.tf_vcn.default_route_table_id
}

resource "oci_core_default_route_table" "route_table" {
  manage_default_resource_id = oci_core_vcn.tf_vcn.default_route_table_id

  route_rules {
    destination       = "0.0.0.0/0"
    network_entity_id = oci_core_internet_gateway.tf_internet_gateway.id
  }
}

resource oci_core_default_security_list "security_list" {
  manage_default_resource_id = oci_core_vcn.tf_vcn.default_security_list_id

  egress_security_rules {
    protocol    = "6"
    //tcp
    destination = "0.0.0.0/0"
    stateless   = false

    tcp_options {
      min = 443
      max = 443
    }
  }

  #Allowing external traffic
  ingress_security_rules {
    protocol = "6"
    // tcp
    source    = "0.0.0.0/0"
    stateless = false

    tcp_options {
      min = 443
      max = 443
    }
  }
}

resource oci_core_subnet "tf_public_subnet" {
  cidr_block      = var.public_subnet_cidr_block
  compartment_id  = resource.oci_identity_compartment.tf_compartment.id
  dhcp_options_id = oci_core_vcn.tf_vcn.default_dhcp_options_id
  display_name    = "public-subnet"
  dns_label       = "sub09120456330"
  prohibit_internet_ingress  = "false"
  prohibit_public_ip_on_vnic = "false"
  route_table_id             = oci_core_vcn.tf_vcn.default_route_table_id
  security_list_ids = [
    oci_core_vcn.tf_vcn.default_security_list_id,
  ]
  vcn_id = oci_core_vcn.tf_vcn.id
}

resource oci_core_route_table tf_route_table {
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  defined_tags = {
  }
  display_name = "Route Table for Private Subnet"
  route_rules {
    destination       = "0.0.0.0/0"
    destination_type  = "CIDR_BLOCK"
    network_entity_id = oci_core_nat_gateway.tf_nat_gateway_vcn.id
  }
  vcn_id = oci_core_vcn.tf_vcn.id
}


resource oci_core_security_list "security_list_for_private_subnet_vcn" {
  compartment_id = resource.oci_identity_compartment.tf_compartment.id
  display_name = "Security List for Private Subnet"
  egress_security_rules {
    destination      = "0.0.0.0/0"
    destination_type = "CIDR_BLOCK"
    protocol  = "6"
    //tcp
    stateless = "false"

    tcp_options {
      min = 443
      max = 443
    }
  }
  egress_security_rules {
    destination      = "0.0.0.0/0"
    destination_type = "CIDR_BLOCK"
    protocol  = "6"
    //tcp
    stateless = "false"

    tcp_options {
      min = 80
      max = 80
    }
  }
  vcn_id = resource.oci_core_vcn.tf_vcn.id
}

resource oci_core_subnet "tf_private_subnet" {
  cidr_block      = var.private_subnet_cidr_block
  compartment_id  = resource.oci_identity_compartment.tf_compartment.id
  dhcp_options_id = oci_core_vcn.tf_vcn.default_dhcp_options_id
  display_name = "private-subnet"
  dns_label       = "sub09120456331"
  ipv6cidr_blocks = [
  ]
  prohibit_internet_ingress  = "true"
  prohibit_public_ip_on_vnic = "true"
  route_table_id             = oci_core_route_table.tf_route_table.id
  security_list_ids = [
    oci_core_security_list.security_list_for_private_subnet_vcn.id,
  ]
  vcn_id = resource.oci_core_vcn.tf_vcn.id
}
