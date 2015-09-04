/**
Purpose :
This page is modified to show the list of workorders associated with the incident/questions in the questions page.

Objective of the document:
Steps to successfully configure/set the Customer Portal to show the list of workorders in the questions details page.
**/

Steps:
1. Import the report with name "CP Incident Work Order" in cx. (This should be available in the report folder from where other reports for cx are expected to be imported).
2. Once the report is imported, edit the detail.php (available at <current location>/development/views/pages/account/questions/) and change the report id in the work order section of the page to the report id of the report imported in step 1.
3. Once edited and saved, navigate to the site's scripts/cp/customer/development/views/pages/account/questions/ folder and replace the existing detail.php file with the one edited in step 2.
4. Restart customer portal.

