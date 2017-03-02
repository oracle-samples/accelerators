<?php
/**
 * This script is the primary script that brokers requests between an SRM add-in and CX or SRM.
 */
//We have increased error reporting.  This should be decreased in production.
error_reporting(E_ERROR | E_WARNING | E_PARSE);

//We simply fail if this is not HTTPS. HTTP is unaccceptable.
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

use Oracle\Accelerator\SRM as SRM;

//Include the SRM controller class
require_once DOCROOT . "/custom/oauthcontroller.phph";

//Include the SCLog class for Logging
require_once DOCROOT . "/custom/sclog.php";
$log = new scTools\SCLog();

//We will get the agent session ID
$session_id = !empty($_GET['session_id']) ? $_GET['session_id'] : -1;

//Run agent authenticator on the session ID that was passed.
//Agent authenticator blocks moving forward if the session id does not validate
require_once DOCROOT . '/include/services/AgentAuthenticator.phph';
$account = AgentAuthenticator::authenticateSessionID($session_id);

//Extract and validate informtion from the posted data
//All data must be present in the request to proceed.
if (!empty($account['acct_id']) && !empty($_POST['f']) && !empty($_POST['process_id']) && !empty($_POST['request_time']))
{
    $function = $_POST['f'];
    $request_time = $_POST['request_time'];
    $process_id = $_POST['process_id'];

    //If our validate request method fails, then stop the process
    if (SRM\OAuthController::validate_request($account['acct_id'], $process_id, $request_time))
    {
        //Call the function from our OAuthController Class that is requested via the URL.
        $oauthController = new SRM\OAuthController();
        if ($function == "refresh_access_token" || $function == "get_token")
        {
            $oauthController->$function($account['acct_id']);
        }
        else if(!empty($function) && method_exists($oauthController, $function)){
            $oauthController->$function();
        }
        else
        {
            $log->error("Function Error", "Invalid function requested");
            echo "Invalid function request";
        }
    }
    else
    {
        $log->error("Account Validation Error", "Account that submitted this request does not have a valid request time in oauth_valid_until or process id in oauth_process_id.");
        echo "Invalid information provided. This action has been logged.";
    }
}
else
{
    $log->error("Data Parse Error", "Required data not provided in request.  Post Contents: " . print_r($_POST, true));
    echo "Data not provided in request";
}
