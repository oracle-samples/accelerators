/**
Purpose :
This document provides information on configuring the PHP Endpoint as required by outbound ETADirect API.

Objective of the document:
The document provides step by step process to configure customer site with outbound ETADirect SOAP Endpoint.
**/

Steps:
1. Please identify the root folder where the endpoint would be deployed. For our e.g. we have taken root folder(henceforth ROOT_FOLDER) as "cgi-bin/<interface_name>.cfg/scripts/custom/src". The 
2. The file 'ofsc_int_v1.php' is the entry point for SOAP Endpoint. It has to be placed in the ROOT_FOLDER.
3. The same ROOT_FOLDER must contain all other files.
4. The file 'toasoapcontroller.php' must be changed to specify the http endpoint. The URL has to be changed for variable CUSTOM_ROOT_URL. For our ROOT_FOLDER the URL would be "http://toa-int-endpoint.kl.pqr/cgi-bin/toa_int_endpoint.cfg/php/custom/".


Once this changes are done; and PHP scripts are deployed, then the endpoint is active.

Test an endpoint:
1. In browser put the URL http://toa-int-endpoint.kl.pqr/cgi-bin/toa_int_endpoint.cfg/php/custom/ofsc_int_v1.php?wsdl (as in our example). It should display the WSDL. The WSDL should also have the correct SOAP Endpoint specified as "http://toa-int-endpoint.kl.pqr/cgi-bin/toa_int_endpoint.cfg/php/custom/ofsc_int_v1.php".
