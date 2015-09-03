# Description
* Contains Addins and extensions for CX Console
* EBS Item List [Report](cx/Case Management Accelerator Solution/RightNow.EBSVirtualReportTablesAddIns/ItemListVirtualTable.md)

# Prerequisites
- Open the "\rntapps\cx\Case Management Accelerator Solution\Case Management Accelerator Solution.sln"
- right click on RightNow.EBSVirtualReportTablesAddIns project, and Add Reference ...
- C:\Users\YourUser\AppData\Roaming\RightNow_Technologies\yourSite\numberfolder\AddInPipeline\AddInViews
- Build the solution
- RightNow.EBSVirtualReportTablesAddIns.dll and AcceleratorSharedServices.dll are produced in the bin\Debug folder 
as well as being copied to "%USERPROFILE%\RightNowDev\AddIns\RightNow.EBSVirtualReportTablesAddIns folder

### PSLog (on hold using Shared Library) ###
- to see the PSLog report, the user login Profile Permission for the custom object PSLog need to be checked (all)

# Getting Started
There are 2 ways for CX to pick up the dll


Dev Mode (for developer):
- Login to CX 
- Staff Management -> Profiles -> pick whatever your login user profile -> Add-Ins tab
- Check Developer Access


Add-In Manager
- Login to CX
- Configuration -> Site Configuration -> Add-In Manager
- New 
- select the RightNow.EBSVirtualReportTablesAddIns.zip which contains AcceleratorSharedServices.dll and RightNow.EBSVirtualReportTablesAddIns.dll from bin\Debug folder.
- click the Profile Access on top
- choose "whatever your login user profile" and check the Allowed Interfaces for checkbox


The Reports (create them in the following order)

"Detail" reports : SR and Incident details
SR Detail (SrDetailVirtualTableReport on 113h)
- Create a Standard Report
- choose Record (NOT Tabular)
- choose EBS SR Detail Table
- drag any columns there for your requirement

- *** Important *** to add the filter :
   Name: HiddenSRconcatIncident_ID (check both box) (SRData$SRDetailTable.HiddenSRconcatIncident_ID, equal, empty Value)


Make sure to use this report:
- Create your own report. 
- New Report -> Grid Report -> Data Dictionary -> All Tables -> EBS Service Request Table (SRData$SRlistTable)
- drag SR Number, Incident Ref, Subject (Summary), Date Created, Status, and HiddenSRconcatIncident_ID in the specified order
- Change each column Heading as needed
- right click on Created column and choose Sort Descending 
- LAST column has to be: HiddenSRconcatIncident_ID and set it to Hidden on the report
- click on Subject column, and select Report Linking ribbon
- In Report Linking Wizard, SRdetailVirtualTableReport
- choose Parent Column Value Use dropdown, and HiddenSRconcatIncident_ID Value dropdown
- choose last radio button: Display Report in a Split Window -> Bottom radio button

Incidents "incidentsByContact" report
- the purpose for this report is to combine it with the SrVirtualTableReport behind the scene
- is important the column order should be the same as SrVirtualTableReport because the data retrieved from Connect WS is being put to the virtual report code.
Do Not change the default columm heading for The Incidents "incidentsByContact" report, which are
"EBS_SR_NUM,Reference #,Date Created,Subject,Status,Incident ID"

- *** Important *** to add the filter :
   Name: Contact (check both box) (incidents.c_id, equal, empty Value)


Custom Contact Workspace
- Configuration -> Application Appearance -> Workspaces
- Make copy of Contact workspace, call it Custom Contact WS
- Open it, and add a tab: "SR Report"
- click Insert Control
- Drag the Report to the tab content
- Click the dragged report, and click Design menu
- click Report
- choose the SrVirtualTableReport 


# How to Test
- update any existing contact co_EBSContactPartyId to 4431
- click on the "Incident/SR Report" tab
- should see combined incident and EBS rows related to this EBS contact


Config Verb CUSTOM_CFG_EBS_Web_Service_Endpoint test
- incidentsByContactReportID is added to config verb
- endpoint, usr and pwd are also used, same as cp project
- incidentsByContactReportID is from report ID (AcId) from rNow_incidentsByContact

To find out cfContactPartyID : 
- Go to Configuration -> Database - Custom Fields -> Contact 
- mouse over custom field co_EBSContactPartyID, currently it 268

CONFIG VERB: CUSTOM_CFG_Accel_Ext_Integrations
against LIVE EBS:

{"rnt_host":"klau01-14801-sql-113h.dv.lan","integration":{"server_type":"EBS","ext_base_url":"http://rws3220164.us.oracle.com:8055/","username":"ebusiness","password":"welcome","cfContactPartyID":268,"incidentsByContactReportID":101036,"ext_services":{"service_request_list":{"read":{"service_name":"CS_SERVICEREQUEST_PUB","soap_action":"GET_SRINFO_BYCONTACT","relative_path":"webservices/SOAProvider/plsql/cs_servicerequest_pub/"}},"service_request_detail":{"read":{"service_name":"CS_SERVICEREQUEST_PUB","soap_action":"GET_SR_INFO","relative_path":"webservices/SOAProvider/plsql/cs_servicerequest_pub/"}
... (not a complete one, not sure the rest for LIVE EBS on others ws)
#### if invalid co_EBSContactPartyID is specified, no rows is returned #####
#### if wrong (meaning other custom field metadata ID), result is unpredictable #####
For example, there are many custom fields, so if admin put the wrong custom field id, 168, 
it can be custom field for customfieldProductId (for example), 
the code will get whatever actual data on the product id, and display the list like said productid 2,
then it will assume it is contactid 2.

