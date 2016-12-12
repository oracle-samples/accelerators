/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: Mobile Agent App Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 16.8 (August 2016)
 *  MAF release: 2.3
 *  reference: 151217-000185
 *  date: Tue Aug 23 16:35:47 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: 02906d979adccdc509cce4216fb1ef67cea67bd4 $
 * *********************************************************************************************
 *  File: LifeCycleListenerImpl.java
 * *********************************************************************************************/

package application;

import SQLite.JDBCDataSource;
import database.ConnectionFactory;
import java.io.InputStream;
import java.sql.Connection;
import java.sql.DatabaseMetaData;
import java.sql.ResultSet;
import java.sql.Statement;

import java.util.logging.Level;

import javax.el.ValueExpression;

import oracle.adfmf.application.LifeCycleListener;
import oracle.adfmf.application.PushNotificationConfig;
import oracle.adfmf.framework.api.AdfmfJavaUtilities;
import oracle.adfmf.framework.event.EventSource;
import oracle.adfmf.framework.event.EventSourceFactory;
import oracle.adfmf.util.Utility;

/**
 * The application life cycle listener provides the basic structure for developers needing
 * to include their own functionality during the different stages of the application life
 * cycle.  Please note that there is <b>no user</b> associated with any of the following
 * methods.
 *
 * Common examples of functionality that might be added:
 *
 * start:
 *   1. determine if the application has updates
 *   2. determine if there already exists a local application database image
 *   3. setup any one time state for the application
 *
 * activate:
 *   1. read any application cache stores and re-populate state (if needed)
 *   2. establish/re-establish any database connections and cursors
 *   3. process any pending web-service requests
 *   4. obtain any required resources
 *
 * deactivate:
 *   1. write any restorable state to an application cache store
 *   2. close any database cursors and connections
 *   3. defer any pending web-service requests
 *   4. release held resources
 *
 * stop:
 *   1. logoff any remote services
 *
 * NOTE:
 * 1. In order for the system to recognize an application life cycle listener
 *    it must be registered in the maf-application.xml file.
 * 2. Application assemblers must implement this interface if they would like to
 *    receive notification of application start, hibernation, and application resume.
 *    If a secure web service is needed, you will need to do this from your 'default'
 *    feature where you will have an associated user.
 *
 * @see oracle.adfmf.application.LifeCycleListener
 */
public class LifeCycleListenerImpl implements LifeCycleListener, PushNotificationConfig
{
  private String[] cachedTables = {"Product", "ProductCatalogue", "Category"};
    
  public LifeCycleListenerImpl()
  {
  }

  /**
   * The start method will be called at the start of the application.
   * 
   * NOTE:
   * 1. This is a <b>blocking</b> call and will freeze the user interface
   *    while the method is being executed.  If you have any longer running
   *    items you should create a background thread and do the work there.
   * 2. Only the application controller's classes will be available in this
   *    method.
   * 3. At this stage, only an anonymous user is associated with the application
   *    so do not attempt to call any secure web services in this method.
   */
  public void start()
  {
      System.out.println("start application ... ");
      
      AdfmfJavaUtilities.setELValue("#{applicationScope.QUERY_URL_CONNECTION}", "IncidentConnection");
      AdfmfJavaUtilities.setELValue("#{applicationScope.QUERY_RESULTS_URI}", "queryResults");
      AdfmfJavaUtilities.setELValue("#{applicationScope.QUERY_REPORT_URI}", "analyticsReportResults");
      AdfmfJavaUtilities.setELValue("#{applicationScope.QUERY_LOV_URI}", "namedIDs");
      
      AdfmfJavaUtilities.setELValue("#{applicationScope.CFG_SERVICE_CONN_NAME}", "IncidentConnection");
      AdfmfJavaUtilities.setELValue("#{applicationScope.CFG_LOGIN_CONN_NAME}", "LoginServer");
      AdfmfJavaUtilities.setELValue("#{applicationScope.CFG_REST_URI}", "/services/rest/connect/v1.3");

      checkPreferenceServerSetting();   
      
            
      try {
          initializeDatabaseFromScript();
          cleanLocalCache();
      } catch (Exception ex) {
          Utility.ApplicationLogger.severe(ex.getMessage());
          throw new RuntimeException(ex);
      }  
  }

  /**
   * The stop method will be called at the termination of the application.
   * 
   * NOTE:
   * 1. Depending on how the application is being shutdown, this method may
   *    or may not be called. For example, if a user kills the Application from
   *    the iOS multitask UI then stop will not be called.  Because of this, each
   *    feature should save off their state in the deactivate handler.
   * 2. Only the application controller's classes will be available in this
   *    method.
   * 3. At this stage, only an anonymous user is associated with the application
   *    so do not attempt to call any secure web services in this method.
   */
  public void stop()
  {
    // Add code here...
  }

  /**
   * The activate method will be called when the application is started (post
   * the start method) and when an application is resumed by the operating
   * system.  If the application supports checkpointing, this is a place where
   * the application should read the checkpoint information and resume the process.
   * 
   * NOTE:
   * 1. This is a <b>blocking</b> call and will freeze the user interface
   *    while the method is being executed.  If you have any longer running
   *    items you should create a background thread and do the work there.
   * 2. Only the application controller's classes will be available in this
   *    method.
   * 3. At this stage, only an anonymous user is associated with the application
   *    so do not attempt to call any secure web services in this method.
   * 4. Once an application is activated, the visible feature's activate lifecycle
   *    method will be executed (if configured) post this method being called.
   */
  public void activate()
  {
    // Add code here...
  }

  /**
   * The deactivate method will be called as part of the application shutdown
   * process or when the application is being deactivated/hibernated by the
   * operating system.  This is the place where application developers would
   * write application checkpoint information in either a database or a "device
   * only" file so if the application is terminated while in the background
   * the application can resume the process when the application is reactivated.
   * 
   * NOTE:
   * 1. This is a <b>blocking</b> call and will freeze the user interface
   *    while the method is being executed.  If you have any longer running
   *    items you should create a background thread and do the work there.
   * 2. Only the application controller's classes will be available in this
   *    method.
   * 3. At this stage, only an anonymous user is associated with the application
   *    so do not attempt to call any secure web services in this method.
   * 4. When an application is being deactivated, the visible feature's
   *    deactivate lifecycle method will be executed (if configured) prior to
   *    this method being called.
   */
  public void deactivate()
  {
    // Add code here...
  }
  
    private void initializeDatabaseFromScript() throws Exception {
          InputStream scriptStream = null;
          Connection conn = null;
          try {

              // Retrieve the path to the local database file
              Utility.ApplicationLogger.severe("Initializing DB...");
              String docRoot = AdfmfJavaUtilities.getDirectoryPathRoot(AdfmfJavaUtilities.ApplicationDirectory);
              Utility.ApplicationLogger.severe(docRoot);
              String dbName = docRoot + "/sm.db";
              Utility.ApplicationLogger.severe(dbName);
              Utility.ApplicationLogger.logp(Level.WARNING, this.getClass().getName(), "TEST", dbName);
              // Check if the db file exists or not
              //File dbFile = new File(dbName);
              //if (dbFile.exists())
              //    return;
              
              // If it doesn't exist then we create it - make sure to turn off autocommit - db file created when opened, if it doesn't exist
              conn = new JDBCDataSource("jdbc:sqlite:" + dbName).getConnection();
              Utility.ApplicationLogger.severe("Initializing connection...");
              conn.setAutoCommit(false);
              Utility.ApplicationLogger.severe("Initializing commit...");
              //exception handeling
          } finally {
              if (conn != null) {
                  conn.commit();
                  conn.close();
              }
          }
    }  
    
    private void cleanLocalCache() throws Exception {
        Connection conn = null;
        try {
            conn = ConnectionFactory.getConnection();
            DatabaseMetaData dbm = conn.getMetaData();
            for (String objectName : cachedTables) {
                // check if $objectName table is there
                ResultSet tables = dbm.getTables(null, null, objectName, null);
                if (tables.next()){
                    Utility.ApplicationLogger.severe("delete local cache: " + objectName);
                    deleteTable(conn, objectName);
                }
            }
        } catch (Exception ex) {
            Utility.ApplicationLogger.severe(ex.getMessage());
            ex.printStackTrace();
            throw new RuntimeException(ex);
        }finally {
            if (conn != null) {
                conn.commit();
            }
        }
    }
    
    private void deleteTable(Connection conn, String objectName){
        try {
            Statement deleteTblStmt = conn.createStatement();
            String deleteTableSQL = "DROP TABLE " + objectName;
            Utility.ApplicationLogger.severe(deleteTableSQL);
            deleteTblStmt.executeQuery(deleteTableSQL);
            
        } catch (Exception ex) {
            Utility.ApplicationLogger.severe(ex.getMessage());
            ex.printStackTrace();
            throw new RuntimeException(ex);
        }
    }
    
    private String getScopedConfigString(String ConfigKey) {
        ValueExpression ve1 = AdfmfJavaUtilities.getValueExpression(ConfigKey, String.class);
        return (String)ve1.getValue(AdfmfJavaUtilities.getELContext());
    }
    
    private void checkPreferenceServerSetting() {
        String CFG_SERVICE_CONN_NAME = getScopedConfigString("#{applicationScope.CFG_SERVICE_CONN_NAME}");
        String CFG_LOGIN_CONN_NAME = getScopedConfigString("#{applicationScope.CFG_LOGIN_CONN_NAME}");
        String CFG_LOGIN = getScopedConfigString("#{applicationScope.CFG_REST_URI}");
        String CFG_LOGOUT = getScopedConfigString("#{applicationScope.CFG_REST_URI}");
        String CFG_SERVICE = getScopedConfigString("#{applicationScope.CFG_REST_URI}");

        String serverURL = getScopedConfigString("#{preferenceScope.application.config.server_url}");
        System.out.println("Configured URL from preferences: " + serverURL);
        
        if (!Utility.isEmpty(serverURL)) {
            AdfmfJavaUtilities.clearSecurityConfigOverrides(CFG_SERVICE_CONN_NAME);
            AdfmfJavaUtilities.clearSecurityConfigOverrides(CFG_LOGIN_CONN_NAME);
            AdfmfJavaUtilities.overrideConnectionProperty(CFG_LOGIN_CONN_NAME, "login", "url", serverURL + CFG_LOGIN);
            AdfmfJavaUtilities.overrideConnectionProperty(CFG_LOGIN_CONN_NAME, "logout", "url", serverURL + CFG_LOGOUT);
            AdfmfJavaUtilities.overrideConnectionProperty(CFG_SERVICE_CONN_NAME, "urlconnection", "url", serverURL + CFG_SERVICE);
            AdfmfJavaUtilities.addWhiteListEntry(AdfmfJavaUtilities.getFeatureId(), serverURL, false);
            AdfmfJavaUtilities.updateApplicationInformation(false);
        }
    }
    
    public long getNotificationStyle() {
        // Allow for alerts and badging and sounds
        return PushNotificationConfig.NOTIFICATION_STYLE_ALERT | PushNotificationConfig.NOTIFICATION_STYLE_BADGE | PushNotificationConfig.NOTIFICATION_STYLE_SOUND;
    }

    public String getSourceAuthorizationId() {
        // Return the GCM sender id
        //return (String)AdfmfJavaUtilities.evaluateELExpression("#{applicationScope.configuration.gcmSenderId}");
        return (String)AdfmfJavaUtilities.evaluateELExpression("#{preferenceScope.application.config.gcm_senderId}");
    }
}
