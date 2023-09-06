<?php
/* * *******************************************************************************************
 *  This file is part of the Oracle B2C Service Accelerator Reference Integration set published
 *  by Oracle B2C Service licensed under the Universal Permissive License (UPL), Version 1.0 as shown at 
 *  http://oss.oracle.com/licenses/upl
 *  Copyright (c) 2023, Oracle and/or its affiliates.
 ***********************************************************************************************
 *  Accelerator Package: Incident Text Based Classification
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 23C (August 2023) 
 *  date: Tue Aug 22 11:57:47 IST 2023
 
 *  revision: RNW-23C
 *  SHA1: $Id: 695f2e796f645ec9d004cbedd96bb6885ea45f75 $
 * CPMObjectEventHandler: ChatEscalationHandler
 * Package: RN
 * Objects: AIML$ChatAIPredictionInfo
 * Actions: Create, Update
 * Version: 1.4
 * *********************************************************************************************
 *  File: chatEscalationHandler.php
 * ****************************************************************************************** */

use \RightNow\CPM\v1 as RNCPM;
use \RightNow\Connect\v1_4 as RNCPHP;

const CHAT_ROLE = 1;
const EMOTION_CONF = 4;
const IS_MANAGER_ASK = 5;
const IS_MANAGER_ASK_CONF = 6;
const CREATED_TIME = 10;
const CHAT_TEXT = 2;
const ID = 7;
const EMOTION = 3;
const LOG_FORMAT = "Y-m-d H:i:s";
const DEBUG = "DEBUG", INFO = "INFO", WARN = "WARN", ERROR = "ERROR";
const LOG_LEVEL = [DEBUG, INFO, WARN, ERROR];
$CURRENT_LOG_LEVEL = DEBUG;

/**
 * CPM to use the AIML$ChatAIPredictionInfo and evaluate chat
 */
class ChatEscalationHandler implements RNCPM\ObjectEventHandler
{



    public static function callRestAPI($method, $url, $data = false, $body, $headers, $apiTimeout, &$predictionLog = null)
    {
        self::log("Invoking rest endpoint." + $url, INFO);

        if (!function_exists("\curl_init")) {
            // This only works on a asynchronous cpm
            \load_curl();
        }
        $curl = \curl_init();

        switch ($method) {
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
        \curl_setopt($curl, CURLOPT_TIMEOUT, $apiTimeout);
        \curl_setopt($curl, CURLOPT_HTTPHEADER, $headers);

        $response = \curl_exec($curl);
        if (\curl_error($curl)) {
            $errorMsg = "Error invoking rest endpoint: " . \curl_error($curl);
            self::log($errorMsg, ERROR);
        } else {
            $resp_code = \curl_getinfo($curl, CURLINFO_HTTP_CODE);
            self::log("REST API call with response status:" . $resp_code . " response body: " . $response, DEBUG);
        }
        \curl_close($curl);
        return $response;
    }

    public static function callReportToGetChats($basicAuth, $url, $apiTimeout, $chatId)
    {
        $count = 0;
        $decodedResult = null;
        $filter = (object) array('name' => 'Parent Id', 'values' => [strval($chatId)]);
        $obj = (object) array('lookupName' => 'ChatForCurrentAgent', 'filters' => [$filter]);
        $headers = array(
            "Authorization: Basic " . $basicAuth,
            "OSvC-CREST-Application-Context: cpm report call",
            "Content-Type: application/json"
        );
        self::log("Before report call: ");
        if ($result = self::callRestAPI("POST", $url, true, $obj, $headers, $apiTimeout)) {
            self::log("After report call: ");
            $decodedResult = json_decode($result, true);
            if (isset($decodedResult['count'])) {
                $count = intval($decodedResult['count']);
                self::log("Parsed callReportToGetChats response: " . json_encode($decodedResult['count']), DEBUG);
            } elseif (isset($decodedResult['error'])) {
                self::log("ERROR invoking callReportToGetChats " . json_encode($decodedResult), ERROR);
            }
        }
        return $decodedResult;
    }

    /**
     * Apply the cpm
     *
     * @param string $runMode RNCPM\RunModeLive RNCPM\RunModeTestObject RNCPM\RunModeTestHarness
     * @param string $action RNCPM\ActionCreate RNCPM\ActionUpdate RNCPM\ActionDestroy
     * @param RNCPHP\AIML\ChatAIPredictionInfo $ChatAIPredictionInfo Any Connect for PHP object that is available to CPM
     * @param int $cycle Recursive depth of the event
     *
     * @return void
     */
    public static function apply($runMode, $action, $conversation, $cycle)
    {

        // Only run CPM on the first cycle
        if ($cycle !== 0)
            return;

        if ($action == RNCPM\ActionCreate) {
            try {
                    $logConfig= RNCPHP\Configuration::fetch("CUSTOM_CFG_LOG_CONFIG")->Value;

                    if (in_array(strtoupper($logConfig), LOG_LEVEL)) {
                        $GLOBALS['CURRENT_LOG_LEVEL'] = strtoupper($logConfig);
                    }

                    self::log("Conversation id  {$conversation->ID} being created", DEBUG);
                    self::log("Chat id  {$conversation->ChatAIResultSummary->ChatId} ", INFO);
                    self::log("Chat text {$conversation->ChatText} ", DEBUG);


                    self::log( "Log Level configured is " . $GLOBALS['CURRENT_LOG_LEVEL'], DEBUG);

                    $configResponse = json_decode(RNCPHP\Configuration::fetch("CUSTOM_CFG_EMOTION")->Value, true);
                    $minMessageToSkip = $configResponse['INITIAL_MESSAGES_TO_SKIP'];
                    $minEmotionConfidence = $configResponse['MIN_EMOTION_CONFIG'];
                    $minSupervisorConfidence = $configResponse['MIN_REQUEST_MANAGER_CONFIG'];
                    $negativeLabel =  array_key_exists('NEGATIVE_LOOKUPNAME', $configResponse)? $configResponse['NEGATIVE_LOOKUPNAME']: 'negative';
                    $apiTimeout = $configResponse['API_TIME_OUT'];
                    $endUserNegativeCount = 0;
                    $endUserNegativeAfterAgentResponse = 0;
                    $agentNegativeCount = 0;
                    $have_alert = false;
                    $row_num = 0;
                    $first_negative_index = 0;
                    $start_negative_time = 0;
                    $last_negative_time = 0;
                    $next_agent_chat = false;
                    $agent_response_between = false;
                    $agent_response_for_negative = false;
                    $is_manager_ask = false;
                    $manager_ask_event_count = 0;
                    $chat_text_length_after_negative_chat = 0;
                    $chatAIAnalysisResult = $conversation->ChatAIResultSummary;

                    $response = self::callReportToGetChats($configResponse['B2C_BASIC_AUTH'], $configResponse['CHAT_DETAILS_REPORT_URL'], $apiTimeout, $conversation->ChatAIResultSummary->ID);

                    $queryResult = RNCPHP\ROQL::queryObject(
                        "SELECT AIML.ChatAIResultSummary FROM AIML.ChatAIResultSummary WHERE ID = " . $conversation->ChatAIResultSummary->ID . " order by ID DESC"
                    )
                        ->next();
                    $chatAIAnalysisResult = $queryResult->next();
                    if ($chatAIAnalysisResult) {
                        self::log("chat already present in parent table  {$chatAIAnalysisResult->ChatId}", DEBUG);
                        if (!$chatAIAnalysisResult->IsActive) {
                            self::log("chat already present as inactive  {$chatAIAnalysisResult->ChatId}", DEBUG);
                            return;
                        }
                    } else {
                        $chatAIAnalysisResult = new RNCPHP\AIML\ChatAIAnalysisResult();
                    }
                    if ($conversation->ChatText == 'Engagement has been concluded.') {
                        $chatAIAnalysisResult->IsActive = false;
                    } else {
                        $chatAIAnalysisResult->IsActive = true;
                    }

                    foreach ($response['rows'] as $index => $chat) {
                        self::log("row id  {$row_num} being processed text {$chat[CHAT_TEXT]}", DEBUG);
                        $chatRole = $chat[CHAT_ROLE];
                        $EmotionConfidence = (float) $chat[EMOTION_CONF];
                        $IsMangerAskMessage = $chat[IS_MANAGER_ASK];
                        $ManagerAskMessageConfidence = (float) $chat[IS_MANAGER_ASK_CONF];
                        $current_row_is_manager_ask = false;
                        $id = $chat[ID];
                        if ($row_num >= $minMessageToSkip) {
                            if ($chatRole == 'END_USER') {
                                $current_row_is_manager_ask = ('Yes' == $IsMangerAskMessage && $ManagerAskMessageConfidence > $minSupervisorConfidence) ? true : false;
                                if ($current_row_is_manager_ask) {
                                    self::log("row id  {$row_num} found manager ask increment count", DEBUG);
                                    $manager_ask_event_count = $manager_ask_event_count + 1;
                                    if (!$is_manager_ask) {
                                        $is_manager_ask = true;
                                        self::log("row id  {$row_num} chat id {$chatAIAnalysisResult->ChatId} found manager ask ManagerAskMessageConfidence : {$ManagerAskMessageConfidence} - minSupervisorConfidence : {$minSupervisorConfidence}", INFO);
                                    }

                                }

                                if ($first_negative_index > 0) {
                                    $chat_text_length_after_negative_chat = $chat_text_length_after_negative_chat + strlen($chat[CHAT_TEXT]);
                                    if ($configResponse['MAX_NEGATIVE_CHAT_COUNT'] <= $endUserNegativeCount && $last_negative_time > 0) {
                                        self::log("ALERT  {$row_num} inside generic max count", DEBUG);
                                        $latest_chat_time = strtotime(str_replace("'", "", $chat[CREATED_TIME]));
                                        $negativeChatTimeDiff = $latest_chat_time - $last_negative_time;
                                        self::log("ALERT  {$row_num} inside generic max negativeChatTimeDiff " . strval($negativeChatTimeDiff), DEBUG);
                                        $first_last_message_gap_with_no_agent_response = $negativeChatTimeDiff > $configResponse['MAX_TIME_GAP_FOR_AGENT_RESPONSE'];
                                        self::log("ALERT:  first_last_message_gap_with_no_agent_response {$first_last_message_gap_with_no_agent_response} next_agent_chat {$next_agent_chat}", DEBUG);
                                        if ($first_last_message_gap_with_no_agent_response && !$next_agent_chat) {
                                            $have_alert = true;
                                            self::log("Alert flag 1: chat id {$chatAIAnalysisResult->ChatId} first_last_message_gap_with_no_agent_response {$first_last_message_gap_with_no_agent_response} ", INFO);
                                        }
                                    }
                                }
                            }

                            $emotion = $chat[EMOTION];


                            if ($chatRole == 'AGENT' && $first_negative_index != 0) {
                                $next_agent_chat = true;
                                self::log("row id  {$row_num} next agent chat found", DEBUG);
                            }

                            if (strtolower($emotion) === $negativeLabel && $EmotionConfidence > $minEmotionConfidence) {
                                self::log("row id  {$row_num} negative chat found", DEBUG);
                                if ($chatRole == 'END_USER') {
                                    $endUserNegativeCount = $endUserNegativeCount + 1;
                                    if ($start_negative_time == 0) {
                                        $start_negative_time = strtotime(str_replace("'", "", $chat[CREATED_TIME]));
                                        $last_negative_time = $start_negative_time;
                                        self::log(" ALERT  {$row_num} first negative message time " . strval($start_negative_time), DEBUG);
                                    } else {
                                        if ($next_agent_chat) {
                                            $agent_response_between = true;
                                        }
                                        $last_negative_time = strtotime(str_replace("'", "", $chat[CREATED_TIME]));
                                        self::log(" ALERT  {$row_num} last negative message time " . strval($last_negative_time), DEBUG);
                                    }
                                    if ($configResponse['MAX_NEGATIVE_CHAT_COUNT'] <= $endUserNegativeCount) {
                                        self::log(" ALERT  {$row_num} inside max count last_negative_time" . strval($last_negative_time), DEBUG);
                                        $negativeChatTimeDiff = $last_negative_time - $start_negative_time;
                                        self::log(" ALERT  {$row_num} inside max count negativeChatTimeDiff " . strval($negativeChatTimeDiff), DEBUG);
                                        $first_last_time_gap_reached = $negativeChatTimeDiff > $configResponse['MAX_TIME_GAP_AGENT_RESPONSE_FOR_NEGATIVE'];
                                        $length_limit_reached = $chat_text_length_after_negative_chat > $configResponse['MAX_TEXT_LENGTH'];
                                        self::log(" ALERT  {$row_num} inside max count chat_text_length_after_negative_chat" . strval($chat_text_length_after_negative_chat), DEBUG);
                                        self::log(" ALERT: agent_response_between {$agent_response_between} first_last_time_gap_reached {$first_last_time_gap_reached} length_limit_reached {$length_limit_reached} ", DEBUG);
                                        if ($agent_response_between || $first_last_time_gap_reached || $length_limit_reached) {
                                            $have_alert = true;
                                            self::log(" Alert flag 2: chat id {$chatAIAnalysisResult->ChatId}  agent_response_between {$agent_response_between} first_last_time_gap_reached {$first_last_time_gap_reached} length_limit_reached {$length_limit_reached} ", INFO);
                                        }
                                    }
                                    if ($first_negative_index == 0) {
                                        self::log("row id  {$row_num} first negative chat found", DEBUG);
                                        $first_negative_index = $row_num;
                                    }
                                } else if ($chatRole == 'AGENT') {
                                    $agentNegativeCount = $agentNegativeCount + 1;
                                }
                            }
                        }
                        $row_num = $row_num + 1;
                    }
                    self::log("chat id {$chatAIAnalysisResult->ChatId} evaluation done is_manager_ask {$is_manager_ask} being manager_ask_event_count {$manager_ask_event_count} have_alert {$have_alert}  ", INFO);


                    $chatAIAnalysisResult->NegativeMessageCount = $endUserNegativeCount;
                    $chatAIAnalysisResult->IsEndUserInNegativeEmotion = $have_alert;
                    $chatAIAnalysisResult->RequestManagerIntervene = $is_manager_ask;
                    $chatAIAnalysisResult->RequestManagerInterveneCount = $manager_ask_event_count;

                    $chatAIAnalysisResult->save();
                    self::log("chat id  {$conversation->ChatAIResultSummary->ChatId} being saved", INFO);
            } catch (Exception $err) {
                self::log("Error creating Incident {$conversation->ChatAIResultSummary->ChatId}." . $err->getMessage(), ERROR);
            }
        }
    }


    /**
     * Log a line to file in tmp folder
     *
     * @param String $logText
     * @return void
     */
    protected static function log($logText, $logLevel = DEBUG)
    {

        if (array_search($GLOBALS['CURRENT_LOG_LEVEL'], LOG_LEVEL) <= array_search($logLevel, LOG_LEVEL)) {
            $fp = fopen("/tmp/cpm_chat_emotion_" . date("Ymd") . ".log", "a+");
            fwrite($fp, date(LOG_FORMAT) . " $logLevel $logText\n");
            fclose($fp);
        }
    }
}

/**
 * Unit testing class
 */
class ChatEscalationHandler_TestHarness implements RNCPM\ObjectEventHandler_TestHarness
{
    static $chatId = null;
    static $chatId2 = null;

    static $parent = null;


    /**
     * Create an ChatAIPredictionInfo
     *
     * @param boolean $productFilledIn
     * @param boolean $closed
     */
    public static function createChat($parent, $threadText, $productFilledIn = false, $closed = false)
    {
		$queryResult = RNCPHP\ROQL::queryObject(
			"select AIML.Emotion from AIML.Emotion where lookupname='Negative'")
			->next();
		$negativeEmotion = $queryResult->next();
        $chat = new RNCPHP\AIML\ChatAIPredictionInfo();
        $chat->ChatText = $threadText;
        $chat->ChatAIResultSummary = $parent;
        $chat->Emotion = $negativeEmotion;
        $chat->EmotionConf = 90.22;
        $chat->RequestManagerIntervene = 1;
        $chat->RequestManagerInterveneConf = 80.00;
        $chat->ChatRole = 1;
        $chat->save();
        return $chat;
    }

    /**
     * Setup of the tests
     */
    public static function setup()
    {
        $parentChat = new RNCPHP\AIML\ChatAIResultSummary();
        $parentChat->ChatId = 14473328;
        $parentChat->IsActive = 1;
        $parentChat->MinEmotionConf = 0.00;
        $parentChat->MinRequestManagerInterveneConf = 0.00;
        $parentChat->save();
        self::$parent = $parentChat->ID;
        self::$chatId = self::createChat(self::$parent, "I am not happy")->ID;
        self::$chatId2 = self::createChat(self::$parent, "I want to talk to Supervisor", true, true)->ID;
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
        $chatId = $objectType::fetch(self::$chatId);
        $chatId2 = $objectType::fetch(self::$chatId2);

        return array($chatId, $chatId2);
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
            if ($object->ID)
                echo "chat text: " . $object->ChatText . "\n";
        } else if ($action == RNCPM\ActionUpdate) {
            echo "** Action update **\n";
        }
        echo 'Action: ' . $action . "\n";
        echo 'Emotion: ' . strval($object->Emotion->ID) . "\n";
        echo 'IsMangerAskMessage: ' . $object->RequestManagerIntervene . "\n";


        return true;
    }

    /**
     * Cleanup of the tests
     */
    public static function cleanup()
    {
        if (self::$chatId) {
            $row = RNCPHP\AIML\ChatAIPredictionInfo::fetch(self::$chatId);
            $row->destroy();
            self::$chatId = null;
        }
        if (self::$chatId2) {
            $row = RNCPHP\AIML\ChatAIPredictionInfo::fetch(self::$chatId2);
            $row->destroy();
            self::$chatId2 = null;
        }
        if (self::$parent) {
            $row = RNCPHP\AIML\ChatAIResultSummary::fetch(self::$parent);
            $row->destroy();
            self::$parent = null;
        }
    }
}
?>