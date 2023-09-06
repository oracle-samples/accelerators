################################################################################################
#  This file is part of the Oracle B2C Service Accelerator Reference Integration set published
#  by Oracle B2C Service licensed under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23C (August 2023) 
#  date: Tue Aug 22 11:57:47 IST 2023
 
#  revision: RNW-23C
#  SHA1: $Id: 84f2d1829943f6e2c7612e9aa39799ad38162ed2 $
################################################################################################
#  File: preprocess_data.py
################################################################################################

import logging

from lxml import etree
from lxml.html import HTMLParser
from lxml.html.clean import Cleaner

from common.constants import LOGGING_FORMAT

HTML_PARSER = HTMLParser()
HTML_CLEANER = Cleaner(allow_tags=[])

LOGGING_LEVEL = logging.INFO
logging.basicConfig(level=LOGGING_LEVEL, format=LOGGING_FORMAT)
logger = logging.getLogger()


def remove_html_single(text: str) -> str:
    """Removes html info from a single text string.

    Args:
        text (str): input text

    Returns:
        str: output text with no html
    """
    value = etree.fromstring(text, HTML_PARSER)
    if value is not None:
        try:
            text = HTML_CLEANER.clean_html(value).text_content()
            text = text.strip()
            text = " ".join(text.split())
        except ValueError as e:
            logger.error(f'Some issues parsing document: "{text}" \n"{str(e)}"')
    return text
