<?php

/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + OFSC Reference Integration
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (Feb 2015)
 *  OFSC release: 15.2 (Feb 2015)
 *  reference: 150622-000130
 *  date: Thu Sep  3 23:14:07 PDT 2015

 *  revision: rnw-15-11-fixes-release-03
 *  SHA1: $Id: a83a544ddc824f8195ecc0e83aa258023e974539 $
 * *********************************************************************************************
 *  File: workorder.php
 * ****************************************************************************************** */ 

use RightNow\Connect\v1_2 as RNCPHP;

class WorkOrder
{
    private $username;
	private $password;
	private $workOrderId;
	private $contact;
	private $contactPhone;
	private $contactEmail;
    private $contactMobilePhone;
	private $workOrderTimeSlot;
	private $workOrderDate;
	private $workOrderStatus;
	private $endTime;
	private $startEndTime;
	private $eta;
	private $externalId;
	private $deliveryWindowStart;
	private $deliveryWindowEnd;
	private $resource;
	private $travelTime;
	private $fieldServiceNote;
	private $duration;
	private $msg_id;
	private $responseDescription;
	private $responseStatus;
	
	public function getUsername() {
		return $this->username;
	}

	public function setUsername($username) {
		$this->username = $username;
	}

	public function getPassword() {
		return $this->password;
	}

	public function setPassword($password) {
		$this->password = $password;
	}

	public function getWorkOrderId() {
		return $this->workOrderId;
	}

	public function setWorkOrderId($workOrderId) {
		$this->workOrderId = $workOrderId;
	}

	public function getContact() {
		return $this->contact;
	}

	public function setContact($contact) {
		$this->contact = $contact;
	}

	public function getContactPhone() {
		return $this->contactPhone;
	}

	public function setContactPhone($contactPhone) {
        $this->contactPhone = empty($contactPhone) ? null : $contactPhone;
	}

	public function getContactEmail() {
		return $this->contactEmail;
	}

	public function setContactEmail($contactEmail) {
		$this->contactEmail = empty($contactEmail) ? null : $contactEmail;
	}

	public function getContactMobilePhone() {
		return $this->contactMobilePhone;
	}

	public function setContactMobilePhone($contactMobilePhone) {
		$this->contactMobilePhone = empty($contactMobilePhone) ? null : $contactMobilePhone;
	}


	public function getWorkOrderTimeSlot() {
		return $this->workOrderTimeSlot;
	}

	public function setWorkOrderTimeSlot($workOrderTimeSlot) {
		$this->workOrderTimeSlot = empty($workOrderTimeSlot) ? null : $workOrderTimeSlot;
	}

	public function getWorkOrderDate() {
		return $this->workOrderDate;
	}

	public function setWorkOrderDate($workOrderDate) { 
		$this->workOrderDate = empty($workOrderDate) ? null : $workOrderDate;
	}

	public function getWorkOrderStatus() {
		return $this->workOrderStatus;
	}

	public function setWorkOrderStatus($workOrderStatus) {
		$this->workOrderStatus = empty($workOrderStatus) ? null : $workOrderStatus;
	}

	public function getEndTime() {
		return $this->endTime;
	}

	public function setEndTime($endTime) {
		$this->endTime = empty($endTime) ? null : $endTime;
	}

	public function getStartEndTime() {
		return $this->startEndTime;
	}

	public function setStartEndTime($startEndTime) {
		$this->startEndTime = empty($startEndTime) ? null : $startEndTime;
	}

	public function getEta() {
		return $this->eta;
	}

	public function setEta($eta) {
		$this->eta = empty($eta) ? null : $eta;
	}

	public function getExternalId() {
		return $this->externalId;
	}

	public function setExternalId($externalId) {
	    $this->externalId = empty($externalId) ? null : intval($externalId);
	}

	public function getDeliveryWindowStart() {
		return $this->deliveryWindowStart;
	}

	public function setDeliveryWindowStart($deliveryWindowStart) {
		$this->deliveryWindowStart = empty($deliveryWindowStart) ? null : $deliveryWindowStart;
	}

	public function getDeliveryWindowEnd() {
		return $this->deliveryWindowEnd;
	}

	public function setDeliveryWindowEnd($deliveryWindowEnd) {
		$this->deliveryWindowEnd = empty($deliveryWindowEnd) ? null : $deliveryWindowEnd;
	}

	public function getResource() {
		return $this->resource;
	}

	public function setResource($resource) {
		$this->resource = empty($resource) ? null : $resource;
	}

	public function getTravelTime() {
		return $this->travelTime;
	}

	public function setTravelTime($travelTime) {
		$this->travelTime = empty($travelTime) ? 0 : intval($travelTime);
	}

	public function getFieldServiceNote() {
		return $this->fieldServiceNote;
	}

	public function setFieldServiceNote($fieldServiceNote) {
		$this->fieldServiceNote = empty($fieldServiceNote) ? null : $fieldServiceNote;
	}

	public function getDuration() {
		return $this->duration;
	}

	public function setDuration($duration) {
		$this->duration = empty($duration) ? 0 : intval($duration);
	}
	
	public function getMsgId() {
		return $this->msg_id;
	}

	public function setMsgId($msgId) {
		$this->msg_id = $msgId;
	}
	
	public function getResponseDescription() {
		return $this->responseDescription;
	}

	public function setResponseDescription($responseDescription) {
		$this->responseDescription = $responseDescription;
	}
	
	public function getResponseStatus() {
		return $this->responseStatus;
	}

	public function setResponseStatus($responseStatus) {
		$this->responseStatus = $responseStatus;
	}
	
	public function updateWorkOrder() {
	    require_once(OFSC_ROOT . 'toalogservice.php');
		$log = ToaLogService::GetLog();
			
		try {
		    //Input validation is required to prevent malicious data being processed and unexpected data being returned. 
			//Sanitizing the WorkOrderId in order to avoid SQL injection attack.
		    $wkOrderId = $this->sanitizeWorkOrderId();
			$result = null;
			
			if($wkOrderId != null) {
			    //Caution: All data that is part of WHERE clause must be sanitized to avoid SQL injection attack.
				$result = RNCPHP\ROQL::queryObject( "SELECT TOA.Work_Order FROM TOA.Work_Order where TOA.Work_Order.ID = ".$wkOrderId)->next();
			}
			else {
			    if($log != null){
				    $log->debug("Invalid WorkOrder Id: " . $this->getWorkOrderId());
			    }
			}
		    
			if($result != null && ($workOrder = $result->next()) != null) {
			    $workOrder->Contact_Phone = $this->getContactPhone();
                $workOrder->Contact_Email = $this->getContactEmail();
                $workOrder->Contact_Mobile_Phone = $this->getContactMobilePhone();
                $workOrder->WO_Time_Slot = $this->getWorkOrderTimeSlot();
				$workOrder->WO_Status = $this->getWorkOrderStatus();
                $workOrder->End_Time = $this->getEndTime();
                $workOrder->Start_End_Time = $this->getStartEndTime();
				$workOrder->ETA = $this->getEta();
	            $workOrder->Delivery_Window_Start = $this->getDeliveryWindowStart();
	            $workOrder->Delivery_Window_End = $this->getDeliveryWindowEnd();
	            $workOrder->Resource = $this->getResource();
	            $workOrder->Travel_Time = $this->getTravelTime();
				$workOrder->Field_Service_Note = $this->getFieldServiceNote();
                $workOrder->Duration = $this->getDuration();
				
				$updateFailed = false;
				if($this->getWorkOrderDate() != null){
				    try {
					    $workOrderDate = new DateTime($this->getWorkOrderDate());
						$workOrder->WO_Date = $workOrderDate->getTimestamp();
					} catch (Exception $e) {
					    $updateFailed = true;
					    $this->setResponseStatus('failed');
			            $this->setResponseDescription('Unable to update WorkOrder');
					    if($log != null){
				            $log->debug("Invalid WorkOrder Date : " . $this->getWorkOrderDate());
			            }   
					}
				}
				else {
				    $updateFailed = true;
				    $this->setResponseStatus('failed');
			        $this->setResponseDescription('Unable to update WorkOrder');
				    if($log != null){
				        $log->debug("WorkOrder Date is empty or null.");
			        }
				}
				
				if($this->getExternalId() != null) {
				    $workOrder->External_ID = $this->getExternalId();   
				}
				else {
				    $updateFailed = true;
					$this->setResponseStatus('failed');
			        $this->setResponseDescription('Unable to update WorkOrder');
				    if($log != null){
				        $log->debug("External Id is null or empty.");
			        }
				}
				if(!$updateFailed) {
				    $workOrder->save();
                    $this->setResponseStatus('sent');
			        $this->setResponseDescription('WorkOrder updated');
				}
			}
			else {
			    $this->setResponseStatus('failed');
			    $this->setResponseDescription('Unable to update WorkOrder');
				if($log != null){
				    $log->debug("Unable to update WorkOrder Id: " . $this->getWorkOrderId());
			    }
			}
		} catch (RNCPHP\ConnectAPIError $err) {
            $this->setResponseStatus('failed');
			$this->setResponseDescription('Unable to update WorkOrder');
			
			if($log != null){
				$log->debug("Unable to update WorkOrder Id: " . $this->getWorkOrderId() . "  Error: " .$err->getMessage());
			}
        }
	}
	
	/**
	 * Input validation is required to prevent malicious data being processed and unexpected data being returned. Sanitizing the WorkOrderId in order to avoid SQL injection attack
	 * @return null or WorkOrderId
	 */
    private function sanitizeWorkOrderId() {
        $wkOrderId = trim($this->getWorkOrderId());
	
	    if(strlen($wkOrderId) > 0 && (preg_match('/^[1-9][0-9]*$/', $wkOrderId) === 1)) {
	        return intval($wkOrderId);
        }
	
	    return null;
    }
}
?>
