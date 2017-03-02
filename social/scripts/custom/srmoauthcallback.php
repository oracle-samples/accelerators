#!/usr/bin/php
<?php
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

//Variable that will be used to display errors to the user.
$error;

//Run agent authenticator on the session ID that was passed.
require_once DOCROOT . '/include/services/AgentAuthenticator.phph';
$account = AgentAuthenticator::authenticateSessionID($_GET['session_id']);

//Include the SRM controller class
require_once DOCROOT . "/custom/oauthcontroller.phph";
use Oracle\Accelerator\SRM as SRM;

//Validate that SRM passed an auth token in the URL and proceed.
if (is_null($error) && isset($_GET['code']))
{
    try {
        $controller = new SRM\OAuthController();
        $tokenData = $controller->requestAccessToken($_GET['code'], $account['acct_id']);

        if ($tokenData == false)
        {
            $error = "Unable to parse response from SRM. Error getting token. Please restart process.";
        }
    }
    catch (\Exception $e)
    {
        $error = $e->getMessage();
    }
}
else
{
    $error = "Auth token and token endpoint not returned from SRM. Please restart process.";
}
?>

<html>
	<head>
		<title>SRM OAuth Initiation Process</title>
	</head>
	<?php if (!empty($tokenData)): ?>
	<script type="text/javascript">
		if(window.external != null){
			var co = window.external.GetCustomObject("Accelerator", "OAuthTokens");
			if(co != null){
				setCustomObjectField("Accelerator", "OAuthTokens", "LastAccessToken", "<?=$tokenData->access_token;?>");
				setCustomObjectField("Accelerator", "OAuthTokens", "RefreshToken", "<?=$tokenData->refresh_token;?>");
				setCustomObjectField("Accelerator", "OAuthTokens", "LastTimeString", "<?=$tokenData->last_time_string;?>");
				setCustomObjectField("Accelerator", "OAuthTokens", "ExpiresIn", "<?=$tokenData->expires_in;?>");
			}
		}
	</script>
	<?php endif;?>
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
		.error{
			display: block;
			color: red;
			font-size: 12px;
			padding: 5px;
		}
		#content_wrapper{
			position: relative;
			display: block;
			border-radius: 15px;
			padding: 20px;
			background: #fff;
		}
	</style>
	<body>
		<header>
			<span>Oracle Service Cloud</span>
		</header>
		<main>
		<h2>SRM OAuth Configuration Tool</h2>
		<div class="error"><?=$error;?></div>
		<div id="content_wrapper">
			<?php if (empty($error)): ?>
				<p>SRM token was successfully configured. Please log out of Service Cloud and log back in for these settings to take place.</p>
				<p>You may close this window.</p>
			<?php else: ?>
				<p>There was an error configuring SRM. Please restart the process.</p>
			<?php endif;?>
		</div>
		</main>
	</body>
</html>