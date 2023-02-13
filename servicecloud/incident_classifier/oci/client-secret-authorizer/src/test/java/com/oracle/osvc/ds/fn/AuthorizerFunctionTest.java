/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
 *  http://oss.oracle.com/licenses/upl
 *  Copyright (c) 2023, Oracle and/or its affiliates.
 ***********************************************************************************************
 *  Accelerator Package: Incident Text Based Classification
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 23A (February 2023) 
 *  date: Tue Jan 31 12:41:34 IST 2023
 
 *  revision: rnw-23-02-initial
 *  SHA1: $Id: e67599120975313a654c14bfe88c2b5a84914b75 $
 * *********************************************************************************************
 *  File: AuthorizerFunctionTest.java
 * ****************************************************************************************** */

package com.oracle.osvc.ds.fn;

import com.fnproject.fn.api.RuntimeContext;
import com.fnproject.fn.runtime.FunctionRuntimeContext;
import com.fnproject.fn.testing.FnResult;
import com.fnproject.fn.testing.FnTestingRule;
import com.oracle.osvc.ds.constants.AuthorizerConstants;
import com.oracle.osvc.ds.model.AuthorizerInput;
import com.oracle.osvc.ds.model.AuthorizerOutput;
import com.oracle.osvc.ds.utils.ConnectionUtil;
import org.apache.http.HttpHeaders;
import org.apache.http.HttpStatus;
import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.mockito.ArgumentMatchers;
import org.mockito.Mock;
import org.mockito.Mockito;

import java.io.IOException;
import java.net.HttpURLConnection;
import java.util.HashMap;
import java.util.Map;

import static org.junit.Assert.assertNull;
import static org.junit.Assert.assertTrue;
import static junit.framework.TestCase.assertEquals;
import static junit.framework.TestCase.assertNotNull;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.when;

public class AuthorizerFunctionTest {

    private static final String DOMAIN = "testdomain.com";
    @Rule
    public final FnTestingRule testing = FnTestingRule.createDefault();

    AuthorizerFunction fn;

    @Mock
    HttpURLConnection con;

    @Before
    public void setup(){
        fn = new AuthorizerFunction();
        con = mock(HttpURLConnection.class);
    }
    @Test
    public void noAuthPassedTest() {

        testing.givenEvent().enqueue().setConfig(AuthorizerConstants.DOMAIN_CONFIG, DOMAIN);
        testing.thenRun(AuthorizerFunction.class, "handleRequest");
        assertNotNull(testing.getStdErrAsString());

    }

    @Test
    public void invalidAuthTypeTest() {

        String invalidInput = "{\"data\": {\"Authorization\":\"Bearer testAuth\"}}";
        testing.givenEvent().withBody(invalidInput).enqueue().setConfig(AuthorizerConstants.DOMAIN_CONFIG, DOMAIN);
        testing.thenRun(AuthorizerFunction.class, "handleRequest");

        FnResult result = testing.getOnlyResult();
        assertEquals("{\"active\":false,\"wwwAuthenticate\":\"Basic realm= " + DOMAIN + "\"}", result.getBodyAsString());
    }

    @Test
    public void validTest() throws IOException {

        Mockito.mockStatic(ConnectionUtil.class);
        when(ConnectionUtil.getConnection(ArgumentMatchers.anyString())).thenReturn(con);
        when(con.getResponseCode()).thenReturn(HttpStatus.SC_OK);

        AuthorizerInput input = new AuthorizerInput();
        Map<String,String> map = new HashMap<>();
        map.put(HttpHeaders.AUTHORIZATION, "Basic xxxx");
        input.setData(map);

        map.put(AuthorizerConstants.DOMAIN_CONFIG, DOMAIN);
        RuntimeContext runtimeContext = new FunctionRuntimeContext(null, map);

        AuthorizerOutput result = fn.handleRequest(input,runtimeContext);
        assertTrue("Successful verification should return active true", result.getActive());
        assertNull("Successful verification need not return any info in www-auth header", result.getWwwAuthenticate());

    }

    @Test
    public void noDomainConfiguredTest() {

        testing.givenEvent().withBody(getValidInput()).enqueue();
        testing.thenRun(AuthorizerFunction.class, "handleRequest");
        assertNotNull(testing.getStdErrAsString());

    }

    private String getValidInput(){
        return "{\"data\": {\"Authorization\": \"Basic dGVzdHVzZXI6dGVzdHBhc3N3b3Jk\"}}";
    }
}