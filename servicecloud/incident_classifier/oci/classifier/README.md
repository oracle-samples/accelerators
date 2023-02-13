## Accelerator B2C Automated

### Overview

Accelerator B2C Automated is wrapper around Accelerated OCI DataScience library which intends to provide B2C Automated
Classification solution through OCI REST endpoint.

### Setup Project

Please refer the [Development Setup Guide](./DevelopmentSetup.md)

### [Prerequisites](https://otube.oracle.com/playlist/dedicated/0_f8f8whqc/0_75tgry5r)

1. [Create Project](https://cloud.oracle.com/data-science/projects)
2. [Create VCN and Subnets](https://cloud.oracle.com/networking/vcns)

3. [Create Dynamic Group with apply followinf rules](https://cloud.oracle.com/identity/domains/)
    - Rule 1
      ```
      all {resource.type='datasciencenotebooksession',resource.compartment.id='ocid1.tenancy.oc1..'}
      ```
    - Rule 2
      ```
      all {resource.type='datasciencejobrun',resource.compartment.id='ocid1.tenancy.oc1..'}
      ```
    - Rule 3
      ```
      all {resource.type='datasciencemodeldeployment',resource.compartment.id='ocid1.tenancy.oc1..'}
      ```

2. [Create Policies with following rules](https://cloud.oracle.com/identity/policies)
    ```
    Allow service datascience to use virtual-network-family in compartment {datascience}
    Allow group DataScienceGroup to read metrics in compartment {datascience}
    Allow group DataScienceGroup to manage data-science-family in compartment {datascience}
    Allow group DataScienceGroup to manage log-groups in compartment {datascience}
    Allow group DataScienceGroup to use log-content in compartment {datascience}
    Allow group DataScienceGroup to use virtual-network-family in compartment {datascience}
    Allow group DataScienceGroup to use object-family in compartment {datascience}
    Allow dynamic-group DataScienceDynamicGroup to use log-content in compartment {datascience}
    Allow dynamic-group DataScienceDynamicGroup to read virtual-network-family in compartment {datascience}
    Allow dynamic-group DataScienceDynamicGroup to manage data-science-family in compartment {datascience}
    Allow dynamic-group DataScienceDynamicGroup to use object-family in compartment {datascience}
    Allow dynamic-group DataScienceDynamicGroup to read repos in compartment {datascience}
    ```

#### Inorder to allow datascience jobs (specifically, subnets to use object storage). Add following Rules and Policies.

- In Policy

    ```
     Allow dynamic-group DataScienceDynamicGroup to inspect vcns in tenancy
    ```  
    ```
    Allow dynamic-group DataScienceDynamicGroup to manage objects in compartment datascience
    ```
    ```
    Allow dynamic-group DataScienceDynamicGroup to manage all-resources in compartment datascience
    ```

- In Dynamic Groups

    ```
    ALL {resource.type = 'fnfunc', resource.compartment.id = 'ocid1.compartment.oc1..'}
    ```

- Create Conda Enviornment Slug in order to allow model deployment to use the same python
  enviornment ([Documentation](https://blogs.oracle.com/ai-and-datascience/post/creating-a-new-conda-environment-from-scratch-in-oci-data-science#:~:text=The%20slug%20name%20is%20the,executed%20the%20same%20odsc%20commands.))


5. Add Configuration setting under Site configuration in agent console (.net application). This is required for 
   get report job to run.

 ```
key - CUSTOM_CFG_REPORT_LIST
value - 
    [{
 "reportName": "aia_incidents",
 "reportQuery": "USE REPORT; Select Incidents.id 'Incident ID', Incidents.referenceNumber 'Reference %23', Incidents.subject 'Subject', Threads.Text Text, Incidents.product 'Product ID', 0 'Initial Product', Incidents.category 'Category ID', 0 'Initial Category', Incidents.disposition 'Disposition ID', 0 'Initial Disposition', Incidents.closedTime, Threads.id 'Thread Id' from Incidents where Incidents.closedTime > 'START_DATE'  and Incidents.closedTime < 'END_DATE' and threads.entryType=3 and threads.displayOrder=1 and Incidents.channel=1 ORDER BY Incidents.closedTime"
}, {
 "reportName": "aia_disposition_hierarchy",
 "reportQuery": "USE REPORT; select Parent.ID 'LevelSecondLast',ID 'LevelLast',ID 'ID',LookupName 'Disposition Name',Parent.Level1 'Disposition Level 1' ,Parent.Level2 'Disposition Level 2',Parent.Level3 'Disposition Level 3',Parent.Level4 'Disposition Level 4','' 'Disposition Level 5','' 'Disposition Level 6' from ServiceDispositions order by ID asc"
}, {
 "reportName": "aia_product_hierarchy",
 "reportQuery": "USE REPORT; select Parent.ID 'LevelSecondLast',ID 'LevelLast',ID 'ID',LookupName 'Product Name',Parent.Level1 'Product Level 1' ,Parent.Level2 'Product Level 2',Parent.Level3 'Product Level 3',Parent.Level4 'Product Level 4','' 'Product Level 5','' 'Product Level 6' from ServiceProducts order by ID asc"
}, {
 "reportName": "aia_category_hierarchy",
 "reportQuery": "USE REPORT; select Parent.ID 'LevelSecondLast',ID 'LevelLast',ID 'ID',LookupName 'Category Name',Parent.Level1 'Category Level 1' ,Parent.Level2 'Category Level 2',Parent.Level3 'Category Level 3',Parent.Level4 'Category Level 4','' 'Category Level 5','' 'Category Level 6' from ServiceCategories order by ID asc"
}]

Key- CUSTOM_CFG_CPM_CONFIG
value -
{
 "B2C_SITE_AUTH": "XXXXX",
 "API_GATEWAY_URL": "XXXXX/b2c/predict",
 "VISON_API_URL": "XXXXX/image/analyse-image",
 "RIGHTNOW_ATTACHMENT_URL": "XXXXX/services/rest/connect/v1.4/incidents/%d/fileAttachments/%d/data",
 "PRODUCT_MIN_CONFIDENCE_SCORE": 0.31,
 "CATEGORY_ITEMS_MIN_CONFIDENCE_SCORE": 0.32,
 "DISPOSITION_ITEMS_MIN_CONFIDENCE_SCORE": 0.33,
 "CPM_PROXY_URL": "",
 "IS_VISION_ENABLED": true
}
```