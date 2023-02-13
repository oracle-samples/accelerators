################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:48 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: f5a19476558814b377771f3be116026853b6a4ed $
################################################################################################
#  File: data_test_PTC.py
################################################################################################
import pandas as pd
import pytest
from sklearn.model_selection import (train_test_split)

from ai4service_automated_classification.constants import HIERARCHY_COLUMNS, INCIDENTS_COLUMNS, DATA_COLUMN, \
    PRODUCT_ID_COLUMN, CATEGORY_ID_COLUMN, DISPOSITION_ID_COLUMN
from ai4service_automated_classification.ml.Node import Node
from ai4service_automated_classification.ml.Transformer import Transformer
from ai4service_automated_classification.ml.util.data_util import build_filter_dict, \
    preprocess_incidents, preprocess_hierarchy


@pytest.fixture(scope="function")
def data_most_frequent_label_0():
    return [
        ("Replication of Organization ignores shipping address While replicating an Organization as a Sales Account, "
         "the shipping address is not considered if Billing Address is not present",
         "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Creation of Sales Account in OEC fails when City is longer than 60 characters Replicating an Organization as "
         "a Sales Account fails if the city in the Billing address (shipping address is ignored anyway)",
         "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Blank address element not allowed to over-write a non-blank address element while propagating Org "
         "update to OEC/CDM When the address of an organization is updated, the address of the corresponding Sales "
         "Account should also be updated.  The update happens, however it is being observed that a blank address "
         "element is not allowed to over-write a non-blank address element.",
         "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Smoketest - Integration Utilities Execution Failed Failed job URL : Unable to fetch error stack trace. "
         "Please refer the job execution log.",
         "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Smoketest - Integration Utilities Execution Failed For clarification of the Development Severity 0 Policy, "
         "please refer to the Development WIKI page",
         "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Background color for the Search button is not blue when the report is rendered initially \n\nCould I "
         "reproduce this in the previous release?",
         "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("CXScripts.AnswerEditor.esc.Links.AnswerTypeChanged and AnswerEditor.esc.Links.AnswerTypeNotChanged- "
         "troubleshoot and fix script failures \nFixed scripts - updated expect statements to include special "
         "responses tab\n\n\n",
         "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Contact creation ignores alternate e-mails if primary e-mail is unspecified While replicating the creation "
         "of a Contact, only the primary e-mail is considered",
         "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Contact update flow ignores primary e-mail if alternate e-mail is specified This defect is the exact inverse "
         "of 200101-000008",
         "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Background color for the Search button is not blue when the report is rendered initially",
         "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Escalate action relative time field value changes from var_date to Sunday after save Steps to reproduce:\n1. "
         "Login to BUI\n2. Create a rule with Escalation action", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Apply sla can be saved without selecting sla value", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Rolling back to Multi Datasource - For tracking InfoManger and Rest API application side.",
         "Techmail", 2, 0, 2, 0, 2, 0),
        ("I attached the configuration data supplied by Cloud Ops here.", "Techmail", 2, 0, 2, 0, 2, 0),
        ("CDM Integration - CDM merge is not working as expected on merging OSvC contact with another source "
         "system contact", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Integration API Execution Failed Failed job URL", "Techmail", 2, 0, 2, 0, 2, 0),
        ("BUI : Rule triggering not working for escalation level equals Unspecified condition",
         "Techmail", 2, 0, 2, 0, 2, 0),
        ("Smoketest - Fixes Console WPF Execution Failed", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Immediate right click on a report in the Report explorer is not showing the context menu dialog",
         "Techmail", 2, 0, 2, 0, 2, 0),
        ("User defined date time related automations failing", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Integration Atlas Site Clone Failed Failed ", "Techmail", 2, 0, 2, 0, 2, 0),
        ("UI - Relative field value set to Now when saved without any value", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Creation of Org with an existing B2B contact sometimes does not create relationship in OEC",
         "Techmail", 2, 0, 2, 0, 2, 0),
        ("Page numbers and count not updated after rule delete_files.", "Techmail", 2, 0, 2, 0, 2, 0),
        ("3 scrollbars appears with pagination", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Example with None Target", "NoneTarget", 1, None, 1, None, 1, None),
        ("Example with NoneNONE Target", "NoneTarget", 1, None, 1, None, 1, None),
        ("ExampleNONE2 with NoneNONE Target", "NoneTarget", 1, None, 1, None, 1, None),
        ("ExampleNONE3 with NoneNONE Target", "NoneTarget", None, None, 1, None, 1, None),
        ("ExampleNONE4 with NoneNONE Target", "NoneTarget", None, None, 1, None, 1, None),
        ("Example with NoneNONE Target", "NoneTarget", 1, None, None, None, 1, None),
        ("ExampleNONE2 with NoneNONE Target", "NoneTarget", 1, None, None, None, 1, None),
        ("Example with NoneNONENone Target", "NoneTarget", 1, None, 1, None, None, None),
        ("ExampleNONE2 with NoneNONENone Target", "NoneTarget", 1, None, 1, None, None, None),
        ("Example2 with None2 Target 2", "NoneTarget1", 2, None, 2, None, 2, None),
        ("Example2 with None2NONE Target 2", "NoneTarget1", 2, None, 2, None, 2, None),
        ("Example2NONE2 with None2NONE Target NONE 2", "NoneTarget1", 2, None, 2, None, 2, None),
        ("Example2NONE3 with None2NONE Target NONE 2", "NoneTarget1", None, None, 2, None, 2, None),
        ("Example2NONE4 with None2NONE Target NONE 2", "NoneTarget1", None, None, 2, None, 2, None),
        ("Example2222 with None2 Target 2", "NoneTarget1", 2, None, None, None, 2, None),
        ("Example2222 with None2NONE Target 2", "NoneTarget1", 2, None, 2, None, None, None),
        ("Example2222 with None 2 Target 2", "NoneTarget1", 2, None, None, None, 2, None),
        ("Example2222 with None 2 NONE Target 2", "NoneTarget1", 2, None, 2, None, None, None),
        # Extra samples to pass prune min count
        ("Additional sample0 for class one.", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Additional sample1 for class one.", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Additional sample2 for class one.", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Additional sample3 for class one.", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Additional sample4 for class one.", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Additional sample5 for class one.", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Additional sample6 for class one.", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Additional sample7 for class one.", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Additional sample8 for class one.", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Additional sample9 for class one.", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Additional sampl10 for class one.", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Additional sampl11 for class one.", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Additional sampl12 for class one.", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Additional sampl13 for class one.", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Additional sampl14 for class one.", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Additional sampl15 for class one.", "Internal Systems", 1, 0, 1, 0, 1, 0),
        ("Additional sample0 for class two.", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Additional sample1 for class two.", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Additional sample2 for class two.", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Additional sample3 for class two.", "Techmail", 2, 0, 2, 0, 2, 0),
        ("Additional sample4 for class two.", "Techmail", 2, 0, 2, 0, 2, 0),
    ]


@pytest.fixture(scope="function")
def dummy_data_PTC(data_most_frequent_label_0):
    data = pd.DataFrame(data_most_frequent_label_0, columns=INCIDENTS_COLUMNS)
    return data


@pytest.fixture(scope="function")
def dummy_hierarchy_PTC():
    product_hierarchy_data = [
        [1, "Internal Systems", 1, 0, 0, 0, 0, 0],
        [2, "Techmail", 1, 2, 0, 0, 0, 0]
    ]
    category_hierarchy_data = [
        [1, "Category Systems", 1, 0, 0, 0, 0, 0],
        [2, "Categor 2", 1, 2, 0, 0, 0, 0],
    ]
    disposition_hierarchy_data = [
        [1, "Disposition Systems", 1, 0, 0, 0, 0, 0],
        [2, "Dispositio 2", 1, 2, 0, 0, 0, 0],
    ]
    hierarchy = {
        PRODUCT_ID_COLUMN: pd.DataFrame(product_hierarchy_data, columns=HIERARCHY_COLUMNS),
        CATEGORY_ID_COLUMN: pd.DataFrame(category_hierarchy_data, columns=HIERARCHY_COLUMNS),
        DISPOSITION_ID_COLUMN: pd.DataFrame(disposition_hierarchy_data, columns=HIERARCHY_COLUMNS)
    }
    return hierarchy


@pytest.fixture(scope="function")
def dummy_category_hierarchy_PTC():
    hierarchy_cols = ["Category Id", "Category Name", "Category Level 1", "Category Level 2", "Category Level 3",
                      "Category Level 4", "Category Level 5", "Category Level 6"]
    hierarchy_data = [
        [1, "Category One", 1, 0, 0, 0, 0, 0],
        [2, "Category Two", 1, 2, 0, 0, 0, 0]
    ]
    category_hierarchy = pd.DataFrame(hierarchy_data, columns=hierarchy_cols)
    return category_hierarchy


@pytest.fixture(scope="function")
def dummy_disposition_hierarchy_PTC():
    hierarchy_cols = ["Disposition Id", "Disposition Name", "Disposition Level 1", "Disposition Level 2",
                      "Disposition Level 3", "Disposition Level 4", "Disposition Level 5", "Disposition Level 6"]
    hierarchy_data = [
        [1, "Disposition One", 1, 0, 0, 0, 0, 0],
        [2, "Disposition Two", 1, 2, 0, 0, 0, 0]
    ]
    disposition_hierarchy = pd.DataFrame(hierarchy_data, columns=hierarchy_cols)
    return disposition_hierarchy


@pytest.fixture(scope="function")
def processed_dummy_data_PTC(dummy_data_PTC):
    return preprocess_incidents(dummy_data_PTC)


@pytest.fixture(scope="function")
def processed_dummy_hierarchy_PTC(dummy_hierarchy_PTC):
    for key in dummy_hierarchy_PTC.keys():
        dummy_hierarchy_PTC[key] = preprocess_hierarchy(dummy_hierarchy_PTC[key])
    return dummy_hierarchy_PTC


# difference from previous data is addition of extra product in both product hierarchy and data
@pytest.fixture(scope="function")
def dummy_data_PTC_extra_product(data_most_frequent_label_0):
    data_most_frequent_label_0[-1] = ("3 scrollbars appears with pagination", "Techmail", 3, 0, 3, 0, 3, 0)
    data_most_frequent_label_0[-2] = ("Page numbers and count not updated after rule delete_files.",
                                      "Techmail", 3, 0, 3, 0, 3, 0)
    data_most_frequent_label_0[-3] = ("Creation of Org with an existing B2B contact sometimes does not create "
                                      "relationship in OEC", "Techmail", 3, 0, 3, 0, 3, 0)
    data_most_frequent_label_0[-4] = ("UI - Relative field value set to Now when saved without any value",
                                      "Techmail", 3, 0, 3, 0, 3, 0)
    data_most_frequent_label_0[-5] = ("Integration Atlas Site Clone Failed Failed ",
                                      "Techmail", 3, 0, 3, 0, 3, 0)
    data_most_frequent_label_0[-6] = ("User defined date time related automations failing",
                                      "Techmail", 3, 0, 3, 0, 3, 0)
    data = pd.DataFrame(data_most_frequent_label_0, columns=INCIDENTS_COLUMNS)

    product_hierarchy_data = [
        [1, "Internal Systems", 1, 0, 0, 0, 0, 0],
        [2, "Techmail", 1, 2, 0, 0, 0, 0],
        [3, "Email issue", 1, 2, 3, 0, 0, 0]
    ]
    category_hierarchy_data = [
        [1, "Category Systems", 1, 0, 0, 0, 0, 0],
        [2, "Categor 2", 1, 2, 0, 0, 0, 0],
        [3, "Categ 3", 1, 2, 3, 0, 0, 0]
    ]
    disposition_hierarchy_data = [
        [1, "Disposition Systems", 1, 0, 0, 0, 0, 0],
        [2, "Dispositio 2", 1, 2, 0, 0, 0, 0],
        [3, "Dispo issue", 1, 2, 3, 0, 0, 0]
    ]
    hierarchy = {
        PRODUCT_ID_COLUMN: pd.DataFrame(product_hierarchy_data, columns=HIERARCHY_COLUMNS),
        CATEGORY_ID_COLUMN: pd.DataFrame(category_hierarchy_data, columns=HIERARCHY_COLUMNS),
        DISPOSITION_ID_COLUMN: pd.DataFrame(disposition_hierarchy_data, columns=HIERARCHY_COLUMNS)
    }
    return data, hierarchy


@pytest.fixture(scope="function")  # need to see if scope in this case needs to be revisited
def filter_dict_PTC(processed_dummy_data_PTC, processed_dummy_hierarchy_PTC):
    filter_dict = build_filter_dict(processed_dummy_data_PTC[PRODUCT_ID_COLUMN],
                                    processed_dummy_hierarchy_PTC[PRODUCT_ID_COLUMN],
                                    PRODUCT_ID_COLUMN)
    return filter_dict


@pytest.fixture(scope="function")
def text_labels_PTC(dummy_data_PTC):
    dummy_data_PTC = preprocess_incidents(dummy_data_PTC)
    dummy_data_PTC = dummy_data_PTC[PRODUCT_ID_COLUMN]
    text = dummy_data_PTC[DATA_COLUMN]
    labels = dummy_data_PTC[PRODUCT_ID_COLUMN]
    return text, labels


@pytest.fixture(scope="function")
def train_test_split_fixture(text_labels_PTC):
    text, labels = text_labels_PTC
    X_train, X_test, y_train, y_test = train_test_split(text, labels, stratify=labels, test_size=0.1, random_state=1)
    return X_train, X_test, y_train, y_test


@pytest.fixture(scope="function")
def transformer_fixture(train_test_split_fixture):
    X_train, X_test, y_train, y_test = train_test_split_fixture
    featurizer = Transformer()
    X_trainf = featurizer.fit_transform(X_train)
    X_testf = featurizer.transform(X_test)
    return X_trainf, X_testf


@pytest.fixture(scope="function")
def Node_PTC():
    node_1 = Node(1, name="Internal Systems")
    node_2 = Node(2, name="Techmail")
    node_1.add_child(node_2)
    return node_1, node_2
