# Deploying Sentiment Accelerator with OCI Stack Resource Manager

The Sentiment Accelerator application can be deployed using OCI Stack Resource Manager. This allows for the automation of resource provisioning as defined in the Terraform configuration. Follow the steps below to package your Terraform configuration and upload it to OCI Stack Resource Manager for deployment.

## Step 1: Zip the Required Folders

1. From the root of your project directory, select the `terraform` and `jobs` folders. Create a zip file containing these folders and any other necessary files:

   ```bash
   zip -r sentiment_accelerator.zip terraform jobs
   ```
2. This zip file will include your Terraform configuration (terraform folder) and any job scripts or resources (jobs folder) required for the Sentiment Accelerator application.

## Step 2: Upload to OCI Stack Resource Manager
1. Sign in to your Oracle Cloud Infrastructure Console. 
2. Navigate to "Stacks" under "Resource Manager" in the navigation menu Or Search for "Stack". 
3. Click on the "Create Stack" button. 
4. Select the "From Local File" option and upload the sentiment_accelerator.zip file you created earlier. 
5. Provide a name and description for your stack. 
6. Configure the stack settings, such as the compartment where resources will be created, tags, and other optional settings. 
7. Review the stack details and click "Apply" to create the required resorces.

---

# Terraform Configuration for Sentiment Accelerator

This Terraform configuration sets up various resources for the Sentiment Accelerator application. The Sentiment Accelerator utilizes language models and data science techniques to analyze and predict emotions from text data. The following sections describe the variables and resources defined in this Terraform configuration.

## Variables

Before running the Terraform configuration, you need to provide values for the following variables in the `variables.tf` file:

### General Configuration

- `region`: The Oracle Cloud Infrastructure region where resources will be created.
- `tenancy_ocid`: The OCID of your tenancy.

### Compartment Configuration

- `compartment`: The name of the compartment in which resources will be created. It should contain only lowercase letters, numerical values from 0-9, and underscores.
- `group`: The name of the group for the Sentiment Accelerator application. It should be unique across the root compartment.
- `dynamic_group`: The name of the dynamic group for the Sentiment Accelerator application.

### Network Configuration

- `network_public_subnet_cidr_block`: The CIDR block for the public subnet of the Virtual Cloud Network (VCN).
- `network_private_subnet_cidr_block`: The CIDR block for the private subnet of the VCN.

### Language Service Configuration

- `supervisor_ask_model_name`: The name of the language model used for supervisor asks.
- `emotions_model_name`: The name of the language model used for analyzing emotions.
- `num_inference_unit`: The number of inference units attached for serving the language models.
- `emotion_num_of_days_data_to_fetch`: The number of days' worth of data to fetch for emotions analysis.

### Data Science Configuration

- `ingestion_job_shape`: The compute shape used for ingesting data into object storage.
- `supervisor_ask_ingestion_job_schedule_interval`: The schedule interval for the supervisor ask ingestion job in days.
- `emotion_ingestion_job_schedule_interval`: The schedule interval for the emotion ingestion job in days.
- `emotion_threshold`: The emotion threshold used for prediction.
- `emotion_percentage_for_positive_samples`: The percentage contribution for positive samples.
- `emotion_percentage_for_neutral_samples`: The percentage contribution for neutral samples.
- `emotions_inactive_chat_status_update_interval`: The schedule interval for chat status update job in days.
- `supervisor_ask_training_job_schedule_interval`: The schedule interval for supervisor ask training job in days.
- `emotions_training_job_schedule_interval`: The schedule interval for emotions training job in days.

### Authorization Configuration

- `authorization_authtype`: The type of authorization used for data ingestion (BASIC | OAUTH).
- `authorization_basic_b2c_auth`: The basic authorization token for B2C authentication.
- `authorization_oauth_user`: The OAUTH user for authentication.
- `authorization_oauth_entity`: The OAUTH entity for authentication.
- `authorization_oauth_path`: The OAUTH path for authentication.
- `authorization_oauth_cx_rest_api_key`: The Base64 encoded CX API Key for authentication.

### Common Data Configuration

- `domain`: The domain of your CX B2C site.
- `bucket_url`: The URL of the bucket where data will be stored.
- `vault_id`: The OCID of the vault where secrets will be stored.

## Resources

The Terraform configuration defines several resources for setting up the Sentiment Accelerator application:

- Virtual Cloud Network (VCN) with public and private subnets.
- Language model endpoints for supervisor asks and emotion analysis.
- Data ingestion jobs for supervisor asks and emotions analysis.
- Data science training jobs for supervisor asks and emotions analysis.
- Authorization configurations for data ingestion.
- Configuration parameters for emotions analysis and prediction.


## Note

- Ensure you have the necessary permissions and authentication to create resources in your Oracle Cloud Infrastructure tenancy.
- Review and customize the configuration as needed before applying.

Feel free to reach out for any assistance or clarification on using this Terraform configuration.

---


“This accelerator uses EmoRoBERTa, a third party model provided under the terms of an open source license without warranty of any kind. Please refer to the Third Party Notices.txt in the code repository. By proceeding to configure this accelerator, you confirm that you understand and agree to the terms and the risks associated with the third-party model.

 

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,

OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.”

---
