### Setup Resources for Incident Classifier through Terraform CLI

#### Prerequesites
- Compress src folder under classifier folder.
- Download condapackage and move it to terraform folder.


#### Step 1:
- Please install terraform from [here](https://learn.hashicorp.com/tutorials/terraform/install-cli)
#### Step 2:
- Change the current directory to [terrafom](../terraform) folder
#### Step 3:
- Init terraform by executing ```terraform init``` from terminal
#### Step 4:
- destroy previously created [If exists] terraform by executing ```terraform apply -destory``` from terminal
#### Step 5:
- create terraform plan by ```terraform plan -out tf.plan``` from terminal
#### Step 6:
- Apply to terraform plan by ```terraform apply "tf.plan"``` from terminal
    

### Setup Resources for Incident Classifier through Oracle Stack

##### Step 1: Go to Resource Manager and Select Stack
##### Step 2: Click on Create Stack and Upload .zip file (incident-classifier codebase) there (contails only .tf files)
![Step 2](./screenshots/0_Step_2_Click_on_Create_Stack_and_Upload_zip_file.png)
##### Step 3: Please provide required configuration details. Such as compartment to be, group name to be etc.
![Step 3](./screenshots/1_enter_configuration_whcih_required.png)
##### Step 4: Verify given configuration details and click create.
![Step 4](./screenshots/2_verify_configuration.png)
##### Step 5: Click on "Plan"
![Step 5](./screenshots/3_plan_the_stack.png)
##### Step 6: Verify that what resources are going to be created.
![Step 6](./screenshots/4_verify_resource_created.png)
##### Step 7: Apply the created plan
![Step 7](./screenshots/5_apply_the_stack.png)
##### Step 8: Check logs to check what resources are created
![Step 8](./screenshots/6_check_logs.png)
##### Step 9: If you want to rollback then destroy the stack by clicking "Destroy". It will destroy created resources.
![Step 9](./screenshots/7_destroy_stack.png)
