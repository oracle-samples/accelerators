#!/usr/bin/php
<?php

/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2014, 2015, 2016, 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + SRM Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17.2 (February 2017) 
 *  reference: 160628-000117
 *  date: Fri Feb 10 19:47:44 PST 2017
 
 *  revision: rnw-17-2-fixes-release-2
 *  SHA1: $Id: df36e6c1cf617a66587eb758cb7163047cdba738 $
 * *********************************************************************************************
 *  File: srmoauthinit.php
 * *********************************************************************************************/

if (!isset($_SERVER['RNT_SSL']))
{
    die("You must submit this request via HTTPS");
}

//Set the docroot dir for OSvC for later file includes
if (!defined('DOCROOT'))
{
    $docroot = get_cfg_var('doc_root');
    define('DOCROOT', $docroot);
}

//Include the SRM controller class
require_once DOCROOT . "/custom/oauthcontroller.phph";
use Oracle\Accelerator\SRM as SRM;

//We will get the agent hash from the URL param, if set.
//Otherwise, die as this information is required for all requests.
$session_id = !empty($_GET['session_id']) ? $_GET['session_id'] : -1;

//Run agent authenticator on the session ID that was passed.
require_once DOCROOT . '/include/services/AgentAuthenticator.phph';
$account = AgentAuthenticator::authenticateSessionID($session_id);
$isAdmin = SRM\OAuthController::validateAgentProfile($account['acct_id']);

//Construct the SRM callback URL that users will be sent to once they authenticate
//This passes the session ID to ensure that the same person that leaves OSvC comes back from SRM.
$callbackUri = SRM\OAuthController::getCallbackUrl($_SERVER, $_GET['session_id']);

//Check to see if configs have already been set on the server.
$controller = new SRM\OAuthController();
$savedVars = $controller->getOauthConfigsFromServer();

//If this account is not an admin, yet the configs are already set, then forward the user to SRM
//for validation for his/her access token.
if (!$isAdmin && !empty($savedVars))
{
    $url = SRM\OAuthController::prepareRedirectForOauth(
    	$savedVars->ClientID, 
    	$savedVars->ClientSecret, 
    	$savedVars->AuthTokenURL, 
    	$savedVars->AccessTokenURL, 
    	$savedVars->Scope, 
    	'code', 
    	$callbackUri, 
    	false
    	);

    //Redirect to SRM for Auth
    header("Location: " . $url);
}
//If the configs are not set, and this user is not an admin, then require an admin to setup configs.
elseif (!$isAdmin)
{
    die("You must be an administrator to use this function.");
}

//Variable that will be used to display errors to the user.
$error;
$authEndpoint = "";
$tokenEndpoint = "";
$clientSecret = "";
$clientId = "";
$scope = "";

//Parse POST to see if the user has filled out the form.
if (!empty($_POST['authorization_endpoint']) && !empty($_POST['token_endpoint']) && !empty($_POST['client_secret']) && !empty($_POST['client_id']))
{
    $url = SRM\OAuthController::prepareRedirectForOauth($_POST['client_id'], $_POST['client_secret'], $_POST['authorization_endpoint'], $_POST['token_endpoint'], $_POST['scope'], $_POST['response_type'], $_POST['redirect_uri'], true);

    //Redirect to SRM for Auth
    header("Location: " . $url);
}
//If the form is partially filled, then force them to fill everything
elseif (!empty($_POST))
{
    $error = "You must enter all required data.";
    $authEndpoint = $_POST['authorization_endpoint'];
    $tokenEndpoint = $_POST['token_endpoint'];
    $clientSecret = $_POST['client_secret'];
    $clientId = $_POST['client_id'];
    $scope = $_POST['client_id'];
}
//If this is a new request, then see if there is existing data to pre-populate the fields
else
{
    if (!empty($savedVars))
    {
        $authEndpoint = $savedVars->AuthTokenURL;
        $tokenEndpoint = $savedVars->AccessTokenURL;
        $clientSecret = $savedVars->ClientSecret;
        $clientId = $savedVars->ClientID;
        $scope = $savedVars->Scope;
    }
}
?>

<html>
	<head>
		<title>SRM OAuth Initiation Process</title>
	</head>
	<style>
		body{
			background: #ececec;
			padding: 0;
			margin: 0;
		}
		header{
			padding: 5px 20px;
			background: black;
			color: white;
			font-family: Arial, sans-serif;
			font-size: 18px;
			font-weight: bold;
			letter-spacing: 1px;
			color: #ffffff;
			text-transform: uppercase;
			top: 0;
			width: 100%;
			position: relative;
		}
		main{
			margin: 20px;
			font-family: Arial, sans-serif;
		}
		main h2{
			font-weight: 300;
			font-size: 28px;
			text-transform: uppercase;
			color: #4597c7;
			text-shadow: #ffffff 0 1px 0;
			letter-spacing: 1px;
			margin: 20px 0;
		}
		#form_wrapper{
			position: relative;
			display: block;
			border-radius: 15px;
			padding: 20px;
			background: #fff;
		}
		form label{
			display: block;
			font-weight: bold;
			text-transform: uppercase;
			font-size: 14px;
		}
		form input, form select{
			display: block;
			width: 100%;
			font-size: 14px;
			outline: aqua;
			border: 1px solid #eee;
			padding: 3px;
			color: #333;
		}
		form input[type=submit]{
			width: 200px;
			color: #fff;
			background: #333;
			border-radius: 5px;
			margin: auto;
		}
		form input[type=submit]:hover{
			background: #999;
			cursor: hand;
		}
		.note{
			font-size: 11px;
		}
		.error{
			display: block;
			color: red;
			font-size: 12px;
			padding: 5px;
		}
	</style>
	<body>
		<header>
			<span>Oracle Service Cloud</span>
		</header>
		<main>
			<h2>SRM OAuth Configuration Tool</h2>
			<div class="error" id="error"><?=$error;?></div>
			<div id="form_wrapper">
				<form id="setup_form" action="<?="https://$_SERVER[HTTP_HOST]$_SERVER[REQUEST_URI]";?>" method="post">
					<label for="">Authorization Endpoint:</label><input name="authorization_endpoint" value="<?=$authEndpoint?>" /><br/>
					<label for="">Token Endpoint:</label><input name="token_endpoint" value="<?=$tokenEndpoint?>"/><br/>
					<label for="">Client ID:</label><input name="client_id" value="<?=$clientId?>"/><br/>
					<label for="">Client Secret:</label><input name="client_secret" value="<?=$clientSecret?>"/><br/>
					<label for="">Scope:</label>
					<select name="scope" value="<?=$scope?>">
						<option value="engage">Engage</option>
					</select>
					<br/>
					<input name="session_id" value="<?=$_GET['session_id'];?>" style="display:none;" />
					<input name="response_type" value="code" style="display:none;" />
					<input name="access_type" value="offline" style="display:none;" />
					<input name="redirect_uri" value="<?=$callbackUri?>" style="display:none;" />
					<input id="submit" type="submit" value="Submit"/>
				</form>
				<div class="note">
					<p>By clicking submit, you will be redirected to SRM where you will login with admin credentials that have access to the SRM API. Once you have logged in, you will be redirected back to Oracle Service Cloud and notified if this process failed or succeeded.</p>
				</div>
			</div>
		</main>
	</body>
	<script type="text/javascript">
		if(window.external != null){
			var co = window.external.GetCustomObject("Accelerator", "OAuthTokens");
			if(co != null && co.Id == null){
				var error = document.getElementById("error");
				error.innerHTML = "Do not use this process on a new workspace record. Instead, copy the following link into your web browser and close this workspace.<br><br>" + window.location.href;
			}
		}
	</script>
</html>