<?xml version='1.0' encoding='UTF-8'?>
<wsdl:definitions name="OutboundWSImplService" targetNamespace="http://www.oracle.com/" xmlns:ns1="urn:toatech:agent" xmlns:ns2="http://schemas.xmlsoap.org/soap/http" xmlns:soap="http://schemas.xmlsoap.org/wsdl/soap/" xmlns:tns="http://www.oracle.com/" xmlns:wsdl="http://schemas.xmlsoap.org/wsdl/" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
	<wsdl:types>
		<xs:schema elementFormDefault="qualified" targetNamespace="urn:toatech:agent" version="1.0" xmlns:tns="urn:toatech:agent" xmlns:xs="http://www.w3.org/2001/XMLSchema">
			<xs:element name="send_message">
				<xs:complexType>
					<xs:sequence>
						<xs:element minOccurs="0" name="user" type="tns:user_t"/>
						<xs:element minOccurs="0" name="messages" type="tns:ArrayOfMessage_t"/>
					</xs:sequence>
				</xs:complexType>
			</xs:element>
			<xs:element name="send_message_response">
				<xs:complexType>
					<xs:sequence>
						<xs:element maxOccurs="unbounded" minOccurs="0" name="message_response" type="tns:message_response_t"/>
					</xs:sequence>
				</xs:complexType>
			</xs:element>
			<xs:complexType name="user_t">
				<xs:sequence>
					<xs:element minOccurs="0" name="now" type="xs:string"/>
					<xs:element minOccurs="0" name="login" type="xs:string"/>
					<xs:element minOccurs="0" name="company" type="xs:string"/>
					<xs:element minOccurs="0" name="auth_string" type="xs:string"/>
				</xs:sequence>
			</xs:complexType>
			<xs:complexType name="ArrayOfMessage_t">
				<xs:sequence>
					<xs:element maxOccurs="unbounded" minOccurs="0" name="message" nillable="true" type="tns:message_t"/>
				</xs:sequence>
			</xs:complexType>
			<xs:complexType name="message_t">
				<xs:sequence>
					<xs:element minOccurs="1" maxOccurs="1" nillable="false" name="message_id" type="xs:string"/>
					<xs:element minOccurs="1" maxOccurs="1" nillable="false" name="body" type="xs:string"/>
				</xs:sequence>
			</xs:complexType>
			<xs:complexType name="message_response_t">
				<xs:sequence>
					<xs:element minOccurs="1" maxOccurs="1" nillable="false" name="message_id" type="xs:string"/>
					<xs:element minOccurs="0" name="fault_attempt" type="xs:string"/>
					<xs:element minOccurs="1" maxOccurs="1" nillable="false" name="status" type="xs:string"/>
					<xs:element minOccurs="1" maxOccurs="1" nillable="false" name="description" type="xs:string"/>
					<xs:element minOccurs="0" name="data" type="xs:string"/>
					<xs:element minOccurs="0" name="external_id" type="xs:string"/>
					<xs:element minOccurs="0" name="duration" type="xs:string"/>
					<xs:element minOccurs="0" name="sent" type="xs:string"/>
				</xs:sequence>
			</xs:complexType>
			<xs:complexType name="result_t">
				<xs:sequence>
					<xs:element minOccurs="0" name="code" type="xs:string"/>
					<xs:element minOccurs="0" name="desc" type="xs:string"/>
				</xs:sequence>
			</xs:complexType>
		</xs:schema>
	</wsdl:types>
	<wsdl:message name="send_message">
		<wsdl:part element="ns1:send_message" name="parameters">
		</wsdl:part>
	</wsdl:message>
	<wsdl:message name="send_messageResponse">
		<wsdl:part element="ns1:send_message_response" name="parameters">
		</wsdl:part>
	</wsdl:message>
	<wsdl:portType name="OutboundSoap">
		<wsdl:operation name="send_message">
			<wsdl:input message="tns:send_message" name="send_message">
			</wsdl:input>
			<wsdl:output message="tns:send_messageResponse" name="send_messageResponse">
			</wsdl:output>
		</wsdl:operation>
	</wsdl:portType>
	<wsdl:binding name="OutboundWSImplServiceSoapBinding" type="tns:OutboundSoap">
		<soap:binding style="document" transport="http://schemas.xmlsoap.org/soap/http"/>
		<wsdl:operation name="send_message">
			<soap:operation soapAction="field_service/send_message" style="document"/>
			<wsdl:input name="send_message">
				<soap:body use="literal"/>
			</wsdl:input>
			<wsdl:output name="send_messageResponse">
				<soap:body use="literal"/>
			</wsdl:output>
		</wsdl:operation>
	</wsdl:binding>
	<wsdl:service name="OutboundWSImplService">
		<wsdl:port binding="tns:OutboundWSImplServiceSoapBinding" name="OutboundSoapPort">
			<soap:address location="__REPLACE_SOAP_URL__"/>
		</wsdl:port>
	</wsdl:service>
</wsdl:definitions>