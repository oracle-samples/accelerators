<?php
/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 

 *  http://oss.oracle.com/licenses/upl
 *  Copyright (c) 2023, Oracle and/or its affiliates.
 ***********************************************************************************************
 *  Accelerator Package: Incident Text Based Classification
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html

 *  OSvC release: 23A (February 2023) 
 *  date: Fri Feb 10 15:54:08 IST 2023

 *  revision: rnw-23-02-initial
 *  SHA1: $Id: 9ca64de8547d34a6772f1a002abdc856fcd8b211 $
 *  CPMObjectEventHandler: AIProductIncidentHandler
 *  Package: RN
 *  Objects: Incident
 *  Actions: Create, Update
 *  Version: 1.4
 * *********************************************************************************************
 *  File: cpm_generic_with_text_classifier.php
 * ****************************************************************************************** */


use \RightNow\CPM\v1 as RNCPM;
use \RightNow\Connect\v1_4 as RNCPHP;

const INQUIRY = "inquiry";
const PRODUCT = "product";
const CATEGORY = "category";
const DISPOSITION = "disposition";
const CONFIDENCE_SCORE = "confidenceScore";
const PREDICTION = "prediction";
const CONFIDENCE_SCORE_DECIMALS = 2;

//Configuring model as a function at API gateway
const B2C_SITE_AUTH="B2C_SITE_AUTH";
const API_GATEWAY_URL="API_GATEWAY_URL";

// Set the confidence score to 1 if dont want the prediction to work
const SERVICE_ITEMS = array(PRODUCT, CATEGORY, DISPOSITION);
const SERVICE_ITEMS_FIELDS_TABLES = array(PRODUCT => 'ServiceProduct', CATEGORY => 'ServiceCategory', DISPOSITION=> 'ServiceDisposition');

const LOG_FORMAT= "Y-m-d H:i:s";
const DEBUG = "DEBUG", INFO = "INFO", WARN = "WARN", ERROR = "ERROR";
const LOG_LEVEL = [DEBUG, INFO, WARN, ERROR];
$CURRENT_LOG_LEVEL = INFO;

/**
 * CPM to use the incident classification project
 */
class AIProductIncidentHandler implements RNCPM\ObjectEventHandler
{
    /**
     * Parse the prediction data if the prediction is enabled
     */
    public static function parsePredictedData($decodedResult) {
        self::log("Inside parsePredictedData.", DEBUG);

        $return = array();
        foreach(SERVICE_ITEMS as $item) {
            $return[$item] = array(CONFIDENCE_SCORE => 0, PREDICTION => 0);
            if (isset($decodedResult[PREDICTION][$item])
                && isset($decodedResult[PREDICTION][$item][CONFIDENCE_SCORE])
                && isset($decodedResult[PREDICTION][$item][PREDICTION])) {
                    $return[$item][CONFIDENCE_SCORE] = $decodedResult[PREDICTION][$item][CONFIDENCE_SCORE];
                    $return[$item][PREDICTION] = $decodedResult[PREDICTION][$item][PREDICTION];
            }
        }
        self::log("Succesfully completed parsePredictedData.", DEBUG);
        return $return;
    }

    /**
     * Send a post request to the OCI service and return the product id
     *
     * @param string $text
     * @param array $items
     * @param string $apiGatewayUrl api gateway url
     * @param String $basicAuth basic auth for api gateway
     * @return array
     */
    public static function aiPredict($text, $items, $apiGatewayUrl, $basicAuth)
    {
        $return = array();
        $obj = (object) array('jsonData' => array(
            INQUIRY => $text,
            PRODUCT => 0,
            CATEGORY => 0,
            DISPOSITION => 0
        ));
        $headers = array("Authorization: Basic ".$basicAuth,
						 "Content-Type: application/json");
        if ($result = self::callDSOCIModel("POST", $apiGatewayUrl ,true, $obj, $headers)){
            $decodedResult = json_decode($result, true);
            if (isset($decodedResult[PREDICTION])) {
			    $return = self::parsePredictedData($decodedResult);
                self::log("Parsed Prediction response: ".json_encode($decodedResult[PREDICTION]), DEBUG);
			} elseif(isset($decodedResult['error'])) {
				self::log("ERROR invoking text model ".json_encode($decodedResult), ERROR);
			}

        }

        return $return;
    }

    /**
     * Get the text from the first thread item from an incident object
     *
     * @param RNCPHP\Incident $incident
     *
     * @return string
     */
    protected static function getFirstThreadText($incident) {
        $text = "";
        $firstThread;
		self::log("Retrieving text from first thread item.", DEBUG);
		foreach($incident->Threads as $item) {
			if ((empty($firstThread)) || ($firstThread->ID > $item->ID)) {
			 $firstThread = $item;
			}
		}
		if (!empty($firstThread)) {
            self::log("First thread found ".json_encode($firstThread->ID), DEBUG);
            $text = $firstThread->Text;
        }
		self::log("Successfully retrieved text from first thread.", DEBUG);
        return $text;
    }

    /**
     * Get the type id from the incident object. Null when none selected.
     *
     * @param RNCPHP\Incident $incident
     * @param $Type {Product, category, Disposition}
     *
     * @return int
     */
    protected static function getIdFromIncident($incident, $type) {
        return isset($incident->$type) ? $incident->$type->ID : null;
    }

    /**
     * Get Product, category, disposition from db
     * @param $result prediction response
     */
    protected static function getServiceItemDetails($result) {
        $serviceItem = array();

        foreach (SERVICE_ITEMS as $item) {
            $queryResult = RNCPHP\ROQL::queryObject("SELECT ".SERVICE_ITEMS_FIELDS_TABLES[$item]." FROM ".SERVICE_ITEMS_FIELDS_TABLES[$item]." WHERE ID = ".$result[$item][PREDICTION])->next();
            $serviceItem[$item] = $queryResult->next();
        }

        return $serviceItem;
    }

    /**
     * create the request and call oci api gateway to get the prediction
     *
     * @param $method type of http request
     * @param $url gateway url
     * @param $body data for request
     * @param $headers headers used
     *
     * @return $response
     */
    public static function callDSOCIModel($method, $url, $data = false, $body, $headers)
    {
        self::log("Invoking prediction endpoint.", INFO);

        if (!function_exists("\curl_init"))
        {
            // This only works on a asynchronous cpm
            \load_curl();
        }
        $curl = \curl_init();

        switch ($method)
        {
            case "POST":
                \curl_setopt($curl, CURLOPT_POST, 1);
                if ($data)
                    \curl_setopt($curl, CURLOPT_POSTFIELDS, json_encode($body));
                break;
            case "PUT":
                \curl_setopt($curl, CURLOPT_PUT, 1);
                break;
            default:
                if ($data)
                    $url = sprintf("%s?%s", $url, http_build_query($data));
        }
        \curl_setopt($curl, CURLOPT_URL, $url);
        \curl_setopt($curl, CURLOPT_RETURNTRANSFER, 1);
        \curl_setopt($curl, CURLOPT_SSL_VERIFYPEER, 0);
        \curl_setopt($curl, CURLOPT_HTTPHEADER, $headers);

		$response = \curl_exec($curl);
        if (\curl_error ( $curl )) {
            self::log("Error invoking predict endpoint: ".\curl_error( $curl ), ERROR);
        }else{
          $resp_code=\curl_getinfo($curl, CURLINFO_HTTP_CODE);
          self::log( "Predict response status:".$resp_code." response body: ".$response, INFO);
        }
        \curl_close($curl);
        return $response;
    }


    /**
     * Apply the cpm
     *
     * @param string $runMode RNCPM\RunModeLive RNCPM\RunModeTestObject RNCPM\RunModeTestHarness
     * @param string $action RNCPM\ActionCreate RNCPM\ActionUpdate RNCPM\ActionDestroy
     * @param RNCPHP\Incident $incident Any Connect for PHP object that is available to CPM
     * @param int $cycle Recursive depth of the event
     *
     * @return void
     */
    public static function apply($runMode, $action, $incident, $cycle)
    {
        // Only run CPM on the first cycle
        if ($cycle !== 0)
            return;


        $logConfig= RNCPHP\Configuration::fetch("CUSTOM_CFG_LOG_CONFIG")->Value;

        if (in_array(strtoupper($logConfig), LOG_LEVEL)) {
            $GLOBALS['CURRENT_LOG_LEVEL'] = strtoupper($logConfig);
        }

        self::log("Initiating Prediction flow for incident {$incident -> ID}.", INFO);
        self::log( "Log Level configured is " . $GLOBALS['CURRENT_LOG_LEVEL'], INFO);

		$queryResult = array();
		$configResponse= json_decode(RNCPHP\Configuration::fetch("CUSTOM_CFG_CPM_CONFIG")->Value, true);

        $createdIds = array(
            PRODUCT => self::getIdFromIncident($incident, ucfirst(PRODUCT)),
            CATEGORY => self::getIdFromIncident($incident, ucfirst(CATEGORY)),
            DISPOSITION => self::getIdFromIncident($incident, ucfirst(DISPOSITION)),
        );

		$textExclusionArray = array();
		self::log("config value exclusionList: ".$configResponse['TEXT_EXCLUSION_LIST'], DEBUG);
		if(!is_null($configResponse['TEXT_EXCLUSION_LIST'])){
		    $textExclusionList = $configResponse['TEXT_EXCLUSION_LIST'];
		    $textExclusionArray = split(":", $textExclusionList);
		}

        // This is where we want to set the product to the machine learning outcome.
        // Also here we want to store the original product to our DB for statistics
        if ($action == RNCPM\ActionCreate) {
            self::log("Incident {$incident->ID} being created", INFO);
           try{
                $predictionLog = new RNCPHP\Prediction\PredictionLog();
                $predictionLog->IncidentId = $incident->ID;
                $predictionLog->CreatedProductId = $createdIds[PRODUCT];
                $predictionLog->CreatedProduct = $createdIds[PRODUCT];
                $predictionLog->CreatedCategoryId = $createdIds[CATEGORY];
                $predictionLog->CreatedCategory = $createdIds[CATEGORY];
                $predictionLog->CreatedDispositionId = $createdIds[DISPOSITION];
                $predictionLog->CreatedDisposition = $createdIds[DISPOSITION];
                $predictionLog->MinConfidenceScoreProduct = $configResponse['PRODUCT_MIN_CONFIDENCE_SCORE'];
                $predictionLog->MinConfidenceScoreCategory = $configResponse['CATEGORY_ITEMS_MIN_CONFIDENCE_SCORE'];
                $predictionLog->MinConfidenceScoreDisposition = $configResponse['DISPOSITION_ITEMS_MIN_CONFIDENCE_SCORE'];
                $predictionLog->Source = 2;

                $text = self::getFirstThreadText($incident);
                $predictText = $incident->Subject . ($text ? ' ' . $text : "");

                foreach ($textExclusionArray as $index => $exclusionName) {
					$filterValue = $exclusionName;
					if (stripos($exclusionName, '/') !== 0) {
					 $filterValue = "/".$exclusionName."/";
					}
					self::log("In loop for exclusionName: {$filterValue}", DEBUG);
                    if(!empty($predictText) && !empty($filterValue) && preg_match($filterValue, strtolower($predictText))){
                        self::log("Return : found exclusionName: {$filterValue}", DEBUG);
                        return;
                    }
                }

                if(is_null($createdIds[PRODUCT]) || is_null($createdIds[CATEGORY])
                || is_null($createdIds[DISPOSITION])) {
                    $incidentIntentDetail = new RNCPHP\Prediction\IncidentIntentDetail();
                    $incidentIntentDetail->AutoMLTriggered = 1;
                    self::log("Confidence score min threshold for prod = {$configResponse['PRODUCT_MIN_CONFIDENCE_SCORE']}, cat = {$configResponse['CATEGORY_ITEMS_MIN_CONFIDENCE_SCORE']}, disp = {$configResponse['DISPOSITION_ITEMS_MIN_CONFIDENCE_SCORE']}", INFO);
                    $start_time = microtime(true);

                    if ($result = self::aiPredict($predictText, $createdIds, $configResponse[API_GATEWAY_URL], $configResponse[B2C_SITE_AUTH])) {
                        $end_time = microtime(true);
                        $serviceItem = self::getServiceItemDetails($result);

                        if (isset($serviceItem[PRODUCT]->ID) && $serviceItem[PRODUCT]->ID
                            && $configResponse['PRODUCT_MIN_CONFIDENCE_SCORE'] < $result[PRODUCT][CONFIDENCE_SCORE]
                            && is_null($createdIds[PRODUCT])) {
                                $incident->Product = $serviceItem[PRODUCT];
                                $incidentIntentDetail->PredictedProduct = $serviceItem[PRODUCT];
                                self::log("Product updated with predicted value: {$result[PRODUCT][CONFIDENCE_SCORE]}", INFO);
                        } elseif (!is_null($createdIds[PRODUCT]) && ($createdIds[PRODUCT] == $serviceItem[PRODUCT]->ID)) {
                                self::log("Predicted product same as initial product => Skipping product update.", INFO);
                        }
                        if (isset($serviceItem[CATEGORY]->ID) && $serviceItem[CATEGORY]->ID
                        && $configResponse['CATEGORY_ITEMS_MIN_CONFIDENCE_SCORE'] < $result[CATEGORY][CONFIDENCE_SCORE]
                        && is_null($createdIds[CATEGORY])) {
                            $incident->Category = $serviceItem[CATEGORY];
                            $incidentIntentDetail->PredictedCategory = $serviceItem[CATEGORY];
                            self::log("Category updated with predicted value: {$result[CATEGORY][CONFIDENCE_SCORE]}", INFO);
                        } elseif (!is_null($createdIds[CATEGORY]) && ($createdIds[CATEGORY] == $serviceItem[CATEGORY]->ID)) {
                            self::log("Predicted category same as initial category => Skipping category update.",INFO);
                        }
                        if (isset($serviceItem[DISPOSITION]->ID) && $serviceItem[DISPOSITION]->ID
                            && $configResponse['DISPOSITION_ITEMS_MIN_CONFIDENCE_SCORE'] < $result[DISPOSITION][CONFIDENCE_SCORE]
                            && is_null($createdIds[DISPOSITION])) {
                            $incident->Disposition = $serviceItem[DISPOSITION];
                            $incidentIntentDetail->PredictedDisposition = $serviceItem[DISPOSITION];
                            self::log("Disposition updated with predicted value: {$result[DISPOSITION][CONFIDENCE_SCORE]}", INFO);
                        }

                        $execution_time = ($end_time - $start_time);
                        $predictionLog->ModelApiExTime = number_format(($execution_time), 2);

                        $incident->CustomFields->Prediction->IncidentIntentDetail=$incidentIntentDetail;
                        $incidentIntentDetail->save();


                        // To not crash when predicted ID is not found
                        if (isset($serviceItem[PRODUCT])) {
                            $predictionLog->PredictedProductId = $serviceItem[PRODUCT]->ID;
                            $predictionLog->PredictedProduct = $serviceItem[PRODUCT]->ID;
                            $predictionLog->ConfidenceScoreProduct = self::confNumber($result[PRODUCT][CONFIDENCE_SCORE]);
                        }
                        if (isset($serviceItem[CATEGORY])) {
                            $predictionLog->PredictedCategoryId = $serviceItem[CATEGORY]->ID;
                            $predictionLog->PredictedCategory = $serviceItem[CATEGORY]->ID;
                            $predictionLog->ConfidenceScoreCategory = self::confNumber($result[CATEGORY][CONFIDENCE_SCORE]);
                        }
                        if (isset($serviceItem[DISPOSITION])) {
                            $predictionLog->PredictedDispositionId = $serviceItem[DISPOSITION]->ID;
                            $predictionLog->PredictedDisposition = $serviceItem[DISPOSITION]->ID;
                            $predictionLog->ConfidenceScoreDisposition = self::confNumber($result[DISPOSITION][CONFIDENCE_SCORE]);
                        }
                         $predictionLog->save();
                        self::log("Prediction record created for {$incident->ID}.", INFO);
                    }
                    $incident->save(RNCPHP\RNObject::SuppressAll);
                    self::log("Incident {$incident->ID} updated.", INFO);

                } else {
					self::log("{$incident->ID} have all fields set", INFO);
				}

        } catch (Exception $err ){
			self::log("Error creating Incident {$incident->ID}.".$err->getMessage(), ERROR);
        }
        }
    }

    protected static function confNumber($value) {
        return number_format($value, CONFIDENCE_SCORE_DECIMALS);
    }

    /**
     * Log a line to file in tmp folder
     *
     * @param String $logText
     * @return void
     */
    protected static function log($logText, $logLevel=DEBUG) {

        if(array_search($GLOBALS['CURRENT_LOG_LEVEL'], LOG_LEVEL) <= array_search($logLevel, LOG_LEVEL)){
            $fp = fopen("/tmp/cpm_prediction_" . date("Ymd") .".log", "a+");
            fwrite($fp, date(LOG_FORMAT) . " $logLevel $logText\n");
            fclose($fp);
        }
    }
}

/**
 * Unit testing class
 */
class AIProductIncidentHandler_TestHarness implements RNCPM\ObjectEventHandler_TestHarness
{
    static $incidentId = null;
    static $incidentId2 = null;

    protected static function getServiceItemDetails()
    {
        $serviceProduct = array();
        foreach (SERVICE_ITEMS as $item)
        {
            $queryResult = RNCPHP\ROQL::queryObject("SELECT ".SERVICE_ITEMS_FIELDS_TABLES[$item]." FROM ".SERVICE_ITEMS_FIELDS_TABLES[$item])->next();
            $serviceProduct[$item] = $queryResult->next();
        }
        return $serviceProduct;
    }

    /**
     * Create an incident
     *
     * @param String $subject
     * @param String $threadText
     * @param boolean $productFilledIn
     * @param boolean $closed
     * @return RNCPHP\Incident
     */
    public static function createIncident($subject, $threadText, $productFilledIn = false, $closed = false)
    {
        $incident = new RNCPHP\Incident();

        $incident->Subject = $subject;
        //@TODO update the contact id to test contact of the site
        $queryResult = RNCPHP\ROQL::queryObject("SELECT Contact FROM Contact WHERE Contact.id = 10")->next();
        $incident->PrimaryContact = $queryResult->next();

        $incident->Threads = new RNCPHP\ThreadArray();
        $incident->Threads[0] = new RNCPHP\Thread();
        $incident->Threads[0]->EntryType = new RNCPHP\NamedIDOptList();
        $incident->Threads[0]->EntryType->ID = 3; // Used the ID here. See the Thread object for definition
        $incident->Threads[0]->Text = "oracle";

        if ($productFilledIn)
        {
            $queryResult = self::getServiceItemDetails();
            $incident->Product = $queryResult[PRODUCT];
            $incident->Category = $queryResult[CATEGORY];
            $incident->Disposition = $queryResult[DISPOSITION];
        }
        $incident->save();

        if ($closed)
        {
            $incident->StatusWithType = new RNCPHP\StatusWithType();
            $incident->StatusWithType->Status = new RNCPHP\NamedIDOptList();
            $incident->StatusWithType->Status->ID = 2;// for closed = solved
            $incident->save();
        }

        return $incident;
    }

    /**
     * Setup of the tests
     */
    public static function setup()
    {
        self::$incidentId = self::createIncident("My phone does vacuum clean properly", "oracle")->ID;
        self::$incidentId2 = self::createIncident("Secure My phone doesn't vacuum clean properly", "oracle", true, true)->ID;
    }

    /**
     * Get the object that we want to use for testing
     *
     * @param string $action RNCPM\ActionCreate RNCPM\ActionUpdate RNCPM\ActionDestroy
     * @param string $objectType the type of object we want to fetch (any connect for php object)
     *
     * @return void
     */
    public static function fetchObject($action, $objectType)
    {
        $incidentId = $objectType::fetch(self::$incidentId);
        $incidentId2 = $objectType::fetch(self::$incidentId2);

        return array($incidentId, $incidentId2);
    }

    /**
     * Print test results for the user to validate
     *
     * @param string $action RNCPM\ActionCreate RNCPM\ActionUpdate RNCPM\ActionDestroy
     * @param RNCPHP\Incident $object Any Connect for PHP object that is available to CPM
     *
     * @return boolean
     */
    public static function validate($action, $object)
    {
        if ($action == RNCPM\ActionCreate) {
            echo "** Action create **\n";
            if ($object->Product) echo "Product lookup name: ".$object->Product->LookupName."\n";
        } else if ($action == RNCPM\ActionUpdate) {
            echo "** Action update **\n";
        }
        echo 'Action: '.$action."\n";
        echo 'Subject: '.$object->Subject."\n";
        echo 'Thread text: '.$object->Threads[0]->Text."\n";
        echo 'Status ID: '.$object->StatusWithType->Status->ID."\n";
        echo 'Closed time: '.date(DATE_ATOM, $object->ClosedTime)."\n";

        $queryResult = RNCPHP\ROQL::queryObject(
            "SELECT Prediction.PredictionLog FROM Prediction.PredictionLog WHERE IncidentId = " . $object->ID . " order by ID DESC")
            ->next();
        $predictionLog = $queryResult->next();

        echo "PredictionLog";
        if ($predictionLog) {
            echo "\n id {$predictionLog->ID}\n CreatedTime: ".date(DATE_ATOM, $predictionLog->CreatedTime)
                ."\n IncidentId: {$predictionLog->IncidentId->ID}\n"
                ." CreatedProductId: {$predictionLog->CreatedProductId}\n"
                ." CreatedCategoryId: {$predictionLog->CreatedCategoryId}\n"
                ." CreatedDispositionId: {$predictionLog->CreatedDispositionId}\n"
                ." PredictedProductId: {$predictionLog->PredictedProductId}\n"
                ." PredictedCategoryId: {$predictionLog->PredictedCategoryId}\n"
                ." PredictedDispositionId: {$predictionLog->PredictedDispositionId}\n"
                ." ConfidenceScoreProduct: {$predictionLog->ConfidenceScoreProduct}\n"
                ." ConfidenceScoreCategory: {$predictionLog->ConfidenceScoreCategory}\n"
                ." ConfidenceScoreDisposition: {$predictionLog->ConfidenceScoreDisposition}\n"
                ." MinConfidenceScoreProduct: {$predictionLog->MinConfidenceScoreProduct}\n"
                ." MinConfidenceScoreCategory: {$predictionLog->MinConfidenceScoreCategory}\n"
                . " MinConfidenceScoreDisposition: {$predictionLog->MinConfidenceScoreDisposition}";
        } else echo " !!!!! not found !!!!!!";
        echo "\n############\n";

        return true;
    }

    /**
     * Cleanup of the tests
     */
    public static function cleanup()
    {
        if (self::$incidentId)
        {
            $incident = RNCPHP\Incident::fetch(self::$incidentId);
            $incident->destroy();
            self::$incidentId = null;
        }
        if (self::$incidentId2)
        {
            $incident = RNCPHP\Incident::fetch(self::$incidentId2);
            $incident->destroy();
            self::$incidentId2 = null;
        }
    }
}
?>