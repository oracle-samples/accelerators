################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:18 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 26bf42f685ea628e4aaa20212d9874bc8ba1ec4b $
################################################################################################
#  File: constants.py
################################################################################################
"""Constants used in package ds_cx_ai4service_automated_classification."""
import datetime
from re import compile, IGNORECASE, UNICODE

# Config paths
APP_DIR = 'ds_cx_ai4service_automated_classification'
CONFIG_DIR = 'config'
CONFIG_MODULE_NAME = f'{APP_DIR}.{CONFIG_DIR}'
CONFIG_FILE = 'app_config.ini'

# Columns
# WARNING: All columns MUST be lowercase, we convert
# the dataframes' columns to lowercase after the ingestion.
INQUIRY_COLUMN = "inquiry"
INCIDENT_COLUMN = "incident"
PRODUCT_COLUMN = "product"
CATEGORY_COLUMN = "category"
DISPOSITION_COLUMN = "disposition"
ID_COLUMN = "id"
PRODUCT_ID_COLUMN = "product id"
CATEGORY_ID_COLUMN = "category id"
DISPOSITION_ID_COLUMN = "disposition id"
DATE_COLUMN = "date created"
DATA_COLUMN = "data"
SUBJECT_COLUMN = "subject"
TEXT_COLUMN = "text"
INITIAL_PRODUCT_ID_COLUMN = "initial product"
INITIAL_CATEGORY_ID_COLUMN = "initial category"
INITIAL_DISPOSITION_ID_COLUMN = "initial disposition"
NAME_COLUMN = "name"
LVL_1 = "level 1"
TARGET_COLUMNS_TRAIN = [PRODUCT_ID_COLUMN, CATEGORY_ID_COLUMN, DISPOSITION_ID_COLUMN]
FEATURE_COLUMNS_TRAIN = [DATA_COLUMN, INITIAL_PRODUCT_ID_COLUMN, INITIAL_CATEGORY_ID_COLUMN,
                         INITIAL_DISPOSITION_ID_COLUMN]
FEATURE_COLUMNS_SERVE = [INQUIRY_COLUMN, PRODUCT_COLUMN, CATEGORY_COLUMN, DISPOSITION_COLUMN]
INCIDENTS_COLUMNS = [DATA_COLUMN, PRODUCT_COLUMN, PRODUCT_ID_COLUMN, INITIAL_PRODUCT_ID_COLUMN,
                     CATEGORY_ID_COLUMN, INITIAL_CATEGORY_ID_COLUMN, DISPOSITION_ID_COLUMN,
                     INITIAL_DISPOSITION_ID_COLUMN]
HIERARCHY_COLUMNS = [ID_COLUMN, NAME_COLUMN, LVL_1, "level 2", "level 3", "level 4", "level 5", "level 6"]
PRODUCT_FEATURE_COLUMNS = [DATA_COLUMN, INITIAL_PRODUCT_ID_COLUMN]
CATEGORY_FEATURE_COLUMNS = [DATA_COLUMN, INITIAL_CATEGORY_ID_COLUMN]
DISPOSITION_FEATURE_COLUMNS = [DATA_COLUMN, INITIAL_DISPOSITION_ID_COLUMN]

# Build Model Params
INITIAL_ID_COLUMN = 'initial_id_column'
FEATURE_COLUMNS = 'feature_columns'
RESPONSE_KEY = 'response_key'
BUILD_MODEL_PARAMS = {
    PRODUCT_ID_COLUMN: {
        INITIAL_ID_COLUMN: INITIAL_PRODUCT_ID_COLUMN,
        FEATURE_COLUMNS: PRODUCT_FEATURE_COLUMNS,
        RESPONSE_KEY: PRODUCT_COLUMN
    },
    CATEGORY_ID_COLUMN: {
        INITIAL_ID_COLUMN: INITIAL_CATEGORY_ID_COLUMN,
        FEATURE_COLUMNS: CATEGORY_FEATURE_COLUMNS,
        RESPONSE_KEY: CATEGORY_COLUMN
    },
    DISPOSITION_ID_COLUMN: {
        INITIAL_ID_COLUMN: INITIAL_DISPOSITION_ID_COLUMN,
        FEATURE_COLUMNS: DISPOSITION_FEATURE_COLUMNS,
        RESPONSE_KEY: DISPOSITION_COLUMN
    }
}

# Constants
RANDOM_SEED = 42
TRANSFORMER_TOP_N_WORDS = 6000
SVD_COMPONENTS = 200
TEST_RATIO = 0.2
PRUNE_MIN_COUNT = 20
ACCURACY_METRIC = "AccuracyMetric"

# Model ID (for templates metrics) or Data ID (for data metrics) to be used as an identifier
MODEL_ID = "auto_classif"
DATA_ID = "auto_classif"

# Data metric alert thresholds
SPARSITY_DEFAULT_THRESHOLD = 0.8
CARDINALITY_DEFAULT_THRESHOLD = 1

# Model metric default alert threshold
MODEL_METRIC_DEFAULT_THRESHOLD = 0.5

# minimum threshold to group two nodes
GDFS_THRESHOLD = 0.1

# logging related format
LOGGING_FORMAT = '[%(levelname)s] %(asctime)s %(name)s:%(lineno)d - %(message)s'

# signatures to remove
CLEANUP_SCOPE = "cleanup_scope"
STOP_WORDS = "stop_words"

URL_TOK = "[URL]"
EMAIL_TOK = "[EMAIL]"
PHONE_TOK = "[PHONE]"
NUMBER_TOK = "[NUMBER]"
DATE_TOK = "[DATE]"

CLEANED_TOKS = [URL_TOK, EMAIL_TOK, PHONE_TOK, NUMBER_TOK, DATE_TOK]

# List of pre-compiled regex

TOKEN_RGX = compile(r'\b[^\d\W]+\b')

# taken hostname, domainname, tld from URL regex below
EMAIL_REGEX = compile(
    r"(?:^|(?<=[^\w@.)]))([\w+-](\.(?!\.))?)*?[\w+-](@|[(<{\[]at[)>}\]])(?:(?:[a-z\\u00a1-\\uffff0-9]-?)*[a-z\\u00a1-\\uffff0-9]+)(?:\.(?:[a-z\\u00a1-\\uffff0-9]-?)*[a-z\\u00a1-\\uffff0-9]+)*(?:\.(?:[a-z\\u00a1-\\uffff]{2,}))",
    flags=IGNORECASE | UNICODE,
)

# for more information: https://github.com/jfilter/clean-text/issues/10
PHONE_REGEX = compile(
    r"((?:^|(?<=[^\w)]))(((\+?[01])|(\+\d{2}))[ .-]?)?(\(?\d{3,4}\)?/?[ .-]?)?(\d{3}[ .-]?\d{4})(\s?(?:ext\.?|[#x-])\s?\d{2,6})?(?:$|(?=\W)))|\+?\d{4,5}[ .-/]\d{6,9}"
)

NUMBERS_REGEX = compile(
    r"(?:^|(?<=[^\w,.]))[+–-]?(([1-9]\d{0,2}(,\d{3})+(\.\d*)?)|([1-9]\d{0,2}([ .]\d{3})+(,\d*)?)|(\d*?[.,]\d+)|\d+)(?:$|(?=\b))"
)

URL_REGEX = compile(
    r"(?:^|(?<![\w\/\.]))"
    r"(?:(?:https?:\/\/|ftp:\/\/|www\d{0,3}\.))"
    r"(?:\S+(?::\S*)?@)?" r"(?:"
    # IP address exclusion
    # private & local networks
    r"(?!(?:10|127)(?:\.\d{1,3}){3})"
    r"(?!(?:169\.254|192\.168)(?:\.\d{1,3}){2})"
    r"(?!172\.(?:1[6-9]|2\d|3[0-1])(?:\.\d{1,3}){2})"
    # IP address dotted notation octets
    # excludes loopback network 0.0.0.0
    # excludes reserved space >= 224.0.0.0
    # excludes network & broadcast addresses
    # (first & last IP address of each class)
    r"(?:[1-9]\d?|1\d\d|2[01]\d|22[0-3])"
    r"(?:\.(?:1?\d{1,2}|2[0-4]\d|25[0-5])){2}"
    r"(?:\.(?:[1-9]\d?|1\d\d|2[0-4]\d|25[0-4]))"
    r"|"
    # host name
    r"(?:(?:[a-z\\u00a1-\\uffff0-9]-?)*[a-z\\u00a1-\\uffff0-9]+)"
    # domain name
    r"(?:\.(?:[a-z\\u00a1-\\uffff0-9]-?)*[a-z\\u00a1-\\uffff0-9]+)*"
    # TLD identifier
    r"(?:\.(?:[a-z\\u00a1-\\uffff]{2,}))" r"|" r"(?:(localhost))" r")"
    # port number
    r"(?::\d{2,5})?"
    # resource path
    r"(?:\/[^\)\]\}\s]*)?",
    flags=UNICODE | IGNORECASE,
)

QUOTES = [
    "«",
    "‹",
    "»",
    "›",
    "„",
    "“",
    "‟",
    "”",
    "❝",
    "❞",
    "❮",
    "❯",
    "〝",
    "〞",
    "〟",
    "＂",
    "‘",
    "‛",
    "’",
    "❛",
    "❜",
    "`",
    "´",
    "‘",
    "’"
]

QUOTE_REGEX = compile(r"[" + "".join(QUOTES) + "]")

DAYS_F_RGX = compile(r"|".join({datetime.date(2001, 1, i).strftime(r'\b%A\b') for i in range(1, 8)}),
                     flags=UNICODE | IGNORECASE)
DAYS_S_RGX = compile(r"|".join({datetime.date(2001, 1, i).strftime(r'\b%a\b') for i in range(1, 8)}),
                     flags=UNICODE | IGNORECASE)
MONTHS_F_RGX = compile(r"|".join({datetime.date(2001, i, 1).strftime(r'\b%B\b') for i in range(1, 8)}),
                       flags=UNICODE | IGNORECASE)
MONTHS_S_RGX = compile(r"|".join({datetime.date(2001, i, 1).strftime(r'\b%b\b') for i in range(1, 8)}),
                       flags=UNICODE | IGNORECASE)
