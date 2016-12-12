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
 *  date: Tue Aug 23 16:35:59 PDT 2016

 *  revision: rnw-16-8-fixes-release-01
 *  SHA1: $Id: ac2423e7abd1e2ae6ff19f38b389472ae23669be $
 * *********************************************************************************************
 *  File: ReportItem.java
 * *********************************************************************************************/

package report;

import util.Util;

public class ReportItem {

    private String id = null;
    private String attr1 = null;
    private String attr2 = null;  // is date for Incidents
    private String attr3 = null;
    private String attr4 = null;
    private String attr5 = null;
    private String attr6 = null;
    private String attr7 = null;
    private String attr8 = null;
    private String attr9 = null;
    private String attr10 = null;
    
    public ReportItem() {
        super();
    }
    
    public void setAttr(int index, String value){
        
        // escape html tag
        //if(value != null)
        //    value = value.replaceAll("\\<[^>]*>","");
        
        if (index == 0 )
            setId(value);
        else {            
            value = Util.attributeToDateString("attr"+index, value);
            
            switch(index){
            case 1:
                setAttr1(value); break;
            case 2:
                setAttr2(value); break;
            case 3:
                setAttr3(value); break;
            case 4:
                setAttr4(value); break;
            case 5:
                setAttr5(value); break;
            case 6:
                setAttr6(value); break;
            case 7:
                setAttr7(value); break;
            case 8:
                setAttr8(value); break;
            case 9:
                setAttr9(value); break;
            case 10:
                setAttr10(value); break;
            }
        }
    }


    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getAttr1() {
        return attr1;
    }

    public void setAttr1(String attr1) {
        this.attr1 = attr1;
    }

    public String getAttr2() {
        return attr2;
    }

    public void setAttr2(String attr2) {

        this.attr2 = attr2;
    }

    public String getAttr3() {
        return attr3;
    }

    public void setAttr3(String attr3) {
        this.attr3 = attr3;
    }

    public String getAttr4() {
        return attr4;
    }

    public void setAttr4(String attr4) {
        this.attr4 = attr4;
    }

    public String getAttr5() {
        return attr5;
    }

    public void setAttr5(String attr5) {
        this.attr5 = attr5;
    }

    public String getAttr6() {
        return attr6;
    }

    public void setAttr6(String attr6) {
        this.attr6 = attr6;
    }

    public String getAttr7() {
        return attr7;
    }

    public void setAttr7(String attr7) {
        this.attr7 = attr7;
    }

    public void setAttr8(String attr8) {
        this.attr8 = attr8;
    }

    public String getAttr8() {
        return attr8;
    }

    public void setAttr9(String attr9) {
        this.attr9 = attr9;
    }

    public String getAttr9() {
        return attr9;
    }

    public void setAttr10(String attr10) {
        this.attr10 = attr10;
    }

    public String getAttr10() {
        return attr10;
    }
}

