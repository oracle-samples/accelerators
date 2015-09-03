`ItemListVirtualTable` has been added as part of the feature ref 141204-000179. 
It implements a report which gets a list of EBS items by a serial number, for a given owner.

Steps required to deploy this report are given below:

- Follow the steps described in the guide to deploy the `EBSVirtualTableReportTablesAddin` addin.

- Add the following web service end point to the configuration settings. Please refer to the deployment guide for detailed instructions on how to modify configuration settings.

```
          "item_list": {
            "read": {
              "service_name": "GET_INSTANCES_BY_ITEM_SERIAL",
              "soap_action": "http://ebs.example.com:8000/webservices/SOAProvider/plsql/cs_servicerequest_pub/",
              "relative_path": "webservices/SOAProvider/plsql/cs_servicerequest_pub/"
            }
          },
          "entitlement_list": {
            "read": {
              "service_name": "GET_CONTRACTS__1",
              "soap_action": "http://ebs.example.com:8000/webservices/SOAProvider/plsql/oks_entitlements_pub/",
              "relative_path": "webservices/SOAProvider/plsql/oks_entitlements_pub/"
            }
          }

```

- Create a report called `DetailedItemListVirtualTableReport` under Public Reports > Accelerator.

- Add columns from `EBS Item Table` to the above report. You must add the column `INSTANCE_ID` to your report.

- Save the report.

- Create a report called `DetailedEntitlementListVirtualTableReport` under Public Reports > Accelerator.

- Add columns from `EBS Entitlement Table` to the above report.

- Create a filter called `HiddenInstanceId` of type Integer on the HiddenInstanceId column. Make it selectable and required at runtime.

- Save the report.

- Link the `INSTANCE_ID` column of `DetailedItemListVirtualTableReport` to the `HiddenInstanceId` filter of the `DetailedEntitlementListVirtualTableReport` and choose to open the results in a split window below the existing report.

- Save all report definitions.

- Add `DetailedItemListVirtualTableReport` to the Incident Workspace under new tab called `Item`.

- Open an incident and populate the following fields 
	- EBS Serial Number 
	- Contact (You must select a valid EBS contact)

- Save the incident

- Click on the tab called `Item`

- You should be able to see EBS items owned by the contact's organization, for the given serial number.