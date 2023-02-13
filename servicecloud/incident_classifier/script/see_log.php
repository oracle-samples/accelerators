<!--
/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
 *  http://oss.oracle.com/licenses/upl
 *  Copyright (c) 2023, Oracle and/or its affiliates.
 ***********************************************************************************************
 *  Accelerator Package: Incident Text Based Classification
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 23A (February 2023) 
 *  date: Wed Feb  1 01:32:11 IST 2023
 
 *  revision: rnw-23-02-initial
 *  SHA1: $Id: f5d33fb478d96c7f9d9eb595b60138aed3d80c95 $
 * *********************************************************************************************
 *  File: see_log.php
 * ****************************************************************************************** */
-->
<?php
use \RightNow\CPM\v1 as RNCPM;
use \RightNow\Connect\v1_4 as RNCPHP;
$username = $_POST['username'];
$password = $_POST['password'];
if (!defined('DOCROOT')) {
    $docroot = get_cfg_var('doc_root');
    define('DOCROOT', $docroot);
}
require_once(DOCROOT . '/include/services/AgentAuthenticator.phph');
$account = AgentAuthenticator::authenticateCredentials($username, $password);

if ($account['acct_id'] > 0) {
       
    $filename = '';
    if ($_GET['filename']) {
        $filename = '/tmp/' . $_GET['filename'];
    } else {
        foreach (glob('/tmp/*.*') as $file) {
            if (strpos($file, 'cpm_prediction_') !== false) {
                $filename = $file;
            }
        }
    }
    if($filename != '')  {
        $lines = file($filename);
        $count = 0;
        foreach ($lines as $line) {
            $count += 1;
            echo $line . "<br>";
        }
    }
}

?>
