# General Text Classification Terraform Configuration

This Terraform configuration file contains the following variables for configuring a general text classification system:

## Region

- Variable Name: `region`
- Description: Please provide the region where the resources will be provisioned.

## Tenancy OCID

- Variable Name: `tenancy_ocid`
- Description: Please provide the OCID of the tenancy.

## Compartment

- Variable Name: `compartment`
- Type: `string`
- Default Value: `"accelerator_incident_classifier"`
- Description: Please provide the compartment name.
- Validation:
  - Condition: `can(regex("^[a-z0-9_]+$", var.compartment))`
  - Error Message: Variable must contain only lowercase letters, numerical values from 0-9, and underscores.

## Group

- Variable Name: `group`
- Type: `string`
- Default Value: `"accelerator_group"`
- Description: Please provide the group name. Make sure this name is unique across the root compartment.

## Dynamic Group

- Variable Name: `dynamic_group`
- Type: `string`
- Default Value: `"accelerator_dynamic_groupgst"`
- Description: Please provide the dynamic group name.

## VCN Related CIDR_BLOCK Config

### Public Subnet CIDR Block

- Variable Name: `network_public_subnet_cidr_block`
- Type: `string`
- Default Value: `"10.0.0.0/24"`

### Private Subnet CIDR Block

- Variable Name: `network_private_subnet_cidr_block`
- Type: `string`
- Default Value: `"10.0.1.0/24"`

## Language Service Related Variables

### Model Name

- Variable Name: `supervisor_ask_model_name`
- Type: `string`
- Default Value: `"generic_model_endpoint"`
- Description: Please provide the language model name.

### Number of Inference Unit

- Variable Name: `num_inference_unit`
- Type: `number`
- Default Value: `1`
- Description: Please provide the number of inference units you want to attach for serving the model.

## Data Science Related Variables

### Ingestion Job Shape

- Variable Name: `ingestion_job_shape`
- Type: `string`
- Default Value: `"VM.Standard2.1"`
- Description: The compute size VM that would be used to ingest the data into object storage.

### Ingestion Job Schedule Interval

- Variable Name: `supervisor_ask_ingestion_job_schedule_interval`
- Type: `number`
- Default Value: `1`
- Description: Please provide the ingestion job schedule interval in hours.

### Training Job Schedule Interval

- Variable Name: `supervisor_ask_training_job_schedule_interval`
- Type: `number`
- Default Value: `24`
- Description: Please provide the training job schedule interval in hours.


## Authorization Type

- Variable Name: `authorization_authtype`
- Type: `string`
- Description: The type of authorization to use while ingesting the data. (BASIC | OAUTH)
- Validation:
  - Condition: `var.authorization_authtype == "BASIC" || var.authorization_authtype == "OAUTH"`
  - Error Message: The `authorization_authtype` value should be either "BASIC" or "OAUTH".

## B2C Authentication

- Variable Name: `authorization_basic_b2c_auth`
- Default Value: `""`
- Description: Please enter your CX B2C site auth. Example: `c2hlbGJ5LnRlc3Q6UGFzc3dvcmQx`.

## OAuth User

- Variable Name: `authorization_oauth_user`
- Default Value: `""`
- Description: Please provide your OAuth User.

## OAuth Entity

- Variable Name: `authorization_oauth_entity`
- Default Value: `""`
- Description: Please provide your OAuth entity.

## OAuth Path

- Variable Name: `authorization_oauth_path`
- Default Value: `""`
- Description: Please provide your OAuth path.

## CX REST API Key

- Variable Name: `authorization_oauth_cx_rest_api_key`
- Default Value: `""`
- Description: Please provide your Base64 encoded CX API Key.

## Number of Days Data to Fetch

- Variable Name: `emotion_num_of_days_data_to_fetch`
- Type: `string`
- Description: Please enter the number of days of data needed to be fetched. The default is 365 days.

## Domain

- Variable Name: `domain`
- Type: `string`
- Description: Please enter your CX B2C site domain. Example: `qa--22cga1.custhelp.com`.

## Bucket URL

- Variable Name: `bucket_url`
- Type: `string`
- Description: Please provide the bucket URL up to the folder that will be used to store the source data.

## Vault ID

- Variable Name: `vault_id`
- Type: `string`
- Description: Please provide the Vault OCID in which the secret would be stored.



Please modify the default values and descriptions according to your specific requirements.