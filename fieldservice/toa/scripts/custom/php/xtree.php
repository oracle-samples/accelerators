<?php
/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the MIT license (MIT) included in the original distribution.
 *  Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + OFSC Reference Integration
 *  link: http://www-content.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.2 (Feb 2015)
 *  OFSC release: 15.2 (Feb 2015)
 *  reference: 150622-000130
 *  date: Mon Aug 24 09:01:23 PDT 2015

 *  revision: rnw-15-11-fixes-release-01
*  SHA1: $Id: bc67de8c21afe05d244eaafc3d1aa763bdf64ab1 $
* *********************************************************************************************
*  File: xtree.php
* *********************************************************************************************
 * Example:
 * $xml = '<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"><soapenv:Header></soapenv:Header><soapenv:Body><n0:QueryCSVResponse xmlns:n0="urn:messages.ws.rightnow.com/v1_2" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"><n0:CSVTableSet><n0:CSVTables><n0:CSVTable><n0:Name>MS.SiteConfiguration</n0:Name><n0:Columns>Name,ID,OtherValue</n0:Columns><n0:Rows><n0:Row>Jonathan,1,blue</n0:Row><n0:Row>Hilgeman,2,red</n0:Row></n0:Rows></n0:CSVTable></n0:CSVTables></n0:CSVTableSet></n0:QueryCSVResponse></soapenv:Body></soapenv:Envelope>';
 *
 */

class xtree
{
	public $xtree = "";
	private $xmlRaw = "";
	private $parser;
	private $openTags = array();
	private $currentDepth = 0;
	private $objects = array();
	private $currentObject = null;
	private $stripNamespaces = false;
	private $autoTrim = false;

	private $fp = null;

	public function __construct($config = array())
	{ 
        $defaults = array(
            'xmlRaw' => '',
            'stripNamespaces' => false,
            'autoTrim' => true,
            );
        $config = array_merge($defaults, $config);

		$this->xmlRaw = trim($config['xmlRaw']);
		$this->stripNamespaces = $config['stripNamespaces'];
		$this->autoTrim = $config['autoTrim'];
		$this->parse();
	}

	private function parse()
	{
		$keepTrying = true;
		$xmlRaw = utf8_encode($this->xmlRaw);
		$lastErrorPosition = 0;
		$badString = "";
		$lastBadString = "";
  
		while($keepTrying)
		{
			// Reset
			$this->objects                    = null;
			$this->currentObject              = null;
			$this->openTags                   = null;
			$this->xmlRaw                     = null;
			$this->xtree                    = null;
			$this->rootTag                    = null;
  
			// Initialize parser
			$this->parser = xml_parser_create();
			xml_set_object($this->parser, $this);
			xml_parser_set_option($this->parser, XML_OPTION_CASE_FOLDING, 0);
			xml_parser_set_option($this->parser, XML_OPTION_SKIP_TAGSTART, 0);
			xml_parser_set_option($this->parser, XML_OPTION_SKIP_WHITE, 1);
			xml_parser_set_option($this->parser, XML_OPTION_TARGET_ENCODING, "UTF-8");
			xml_set_element_handler($this->parser, "tag_open", "tag_close");
			xml_set_character_data_handler($this->parser, "cdata"); // Parse!
			xml_parse($this->parser, $xmlRaw);
  
			$errCode = xml_get_error_code($this->parser);
			if($errCode > 0)
			{
					// Get current error position and make sure we're not stuck in an infinite loop
					$errpos = xml_get_current_byte_index($this->parser);
					$errMessage = "XML Parsing Error: " . xml_error_string($errCode) . " at offset " . $errpos;
					if(file_exists("/tmp") && is_writeable("/tmp")) { file_put_contents("/tmp/".time()."_{$errCode}_xtree_parsefailure.xml",$errMessage . "\n\n" . $this->xmlRaw); }
  
					if(($errCode == 9) || ($errCode == 14))
					{
						// Check the area for bad / encoded entities
						$badString = substr($xmlRaw, $errpos-7, 20);
						if(preg_match("/(&#\d+;)/",$badString,$matches))
						{
							// Remove entitiy, splice in fixed string, and allow this to re-run
							$fixString = str_replace($matches[1],"",$badString);
							$xmlRaw = substr($xmlRaw, 0, $errpos-7) . $fixString . substr($xmlRaw, $errpos+13);
						}
						else
						{
							$errMessage = "XML Parsing Error: " . xml_error_string($errCode) . ", String: $badString!";
							if(file_exists("/tmp") && is_writeable("/tmp")) { file_put_contents("/tmp/".time()."_{$errCode}_xtree_parsefailure.xml",$errMessage . "\n\n" . $this->xmlRaw); }
							throw new Exception($errMessage);
							$keepTrying = false;
						}
				}
				else
				{
					$badString = substr($xmlRaw, $errpos-7, 20);
					$errMessage = "XML Parsing Error: Code {$errCode}, Message: " . xml_error_string($errCode) . "! Position: {$errpos}, String: {$badString}";
					if(file_exists("/tmp") && is_writeable("/tmp")) { file_put_contents("/tmp/".time()."_{$errCode}_xtree_parsefailure.xml",$errMessage . "\n\n" . $this->xmlRaw); }
					throw new Exception($errMessage);
					$keepTrying = false;
				}
			}
			else
			{
				$keepTrying = false;
			}

			if($keepTrying)
			{
				if(($errpos == $lastErrorPosition) && ($badString == $lastBadString))
				{
					$errMessage = "XML Parsing Error at position $lastErrorPosition: " . xml_error_string($errCode) . " and could not automatically fix!";
					if(file_exists("/tmp") && is_writeable("/tmp")) { file_put_contents("/tmp/".time()."_{$errCode}_xtree_parsefailure.xml",$errMessage . "\n\n" . $this->xmlRaw); }
					throw new Exception($errMessage);
				}
				$lastErrorPosition = $errpos;
				$lastBadString = $badString;
			}
			xml_parser_free($this->parser);
		}

		// Store tree
		$rootNode            = new XMLNode("");
		$rootTag             = $this->rootTag;
		if(is_null($this->objects)) { $this->objects = array(); }
        $rootNode->$rootTag  = $this->objects[0];

		// Cleanup
		$this->objects       = null;
		$this->currentObject = null;
		$this->openTags      = null;
		$this->rootTag       = null;
		$this->xmlRaw        = null;
		$this->xtree       = $rootNode;
	}
  
	// Handler for opening tags: <tag>
	function tag_open($parser, $tag, $attributes)
	{
		// Determine current depth
		$this->currentDepth++;

		// Get tag name
		if ($this->stripNamespaces) {
				$pieces = explode(":", $tag, 2);
				$tag = array_pop($pieces);
		} else {
				$tag = str_replace(":", "__", $tag);
		}
  
		if($this->rootTag === null)
		{
				$this->rootTag = $tag;
		}
      
		// Create new XMLNode object
		$data = array();
		if (count($attributes)) {
				$data["attributes"] = $attributes;
		}
		$object = new XMLNode($data);
  
		// Store current object so that the data handler can add it
		$this->currentObject = $object;
  
		// Add to parent object
		if (isset($this->openTags[($this->currentDepth - 1)])) {
				$parentObject = $this->openTags[($this->currentDepth - 1)];
				if (!isset($parentObject->$tag))
				{
					// First object of that tag in this area
					$parentObject->$tag = $object;
				}
				else 
				{
					// Multiple objects
					if (!is_array($parentObject->$tag))
					{
						// Convert to an array of objects if necessary
						$prevObject = $parentObject->$tag;
						$parentObject->$tag = array( $prevObject );
					}
					
					// Append new object
					array_push($parentObject->$tag,$object);
				}
		}
  
		// Store references to object
		$this->openTags[$this->currentDepth] = $object;
		$this->objects[]                     = $object;
  
	}
  
	// Push data into XMLNode object
	function cdata($parser, $cdata)
	{
		if (!isset($this->currentObject->_["value"]))
		{
			$this->currentObject->_["value"] = "";
		}
		$this->currentObject->_["value"] .= ($this->autoTrim ? trim($cdata) : $cdata);
	}
  
	// Handler for closing tags: </tag>
	function tag_close($parser, $tag)
	{
		$depthTag = $this->currentDepth . "__" . $tag;
		if (!isset($this->openTags[$this->currentDepth]))
		{
			echo "Can't close $depthTag!<br>\n";
		}
		else
		{
			unset($this->openTags[$this->currentDepth]);
		}
		$this->currentDepth--;
	}
  
	// Cleanup
	public function __destruct()
	{
		$properties = get_object_vars($this);
		foreach ($properties as $var => $val)
		{
			unset($this->$var);
		}
		unset($properties);
	}
}

// Simple storage class for each node
class XMLNode
{
		public $_ = array();

		public function __construct($data)
		{
			$this->_ = $data;
		}

		// Cleanup
		public function __destruct()
		{
			unset($this->_);
		}
}

?>