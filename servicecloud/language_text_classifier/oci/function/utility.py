
################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2024, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 24B (February 2024) 
#  date: Thu Feb 15 09:26:01 IST 2024
 
#  revision: rnw-24-02-initial
#  SHA1: $Id: 4e16c0430620ed087dac4ae7d7f9ef91115f0248 $
################################################################################################
#  File: utility.py
################################################################################################
####################################
# DataScience related resources
####################################

import html as ihtml
from bs4 import BeautifulSoup
import re


def clean_text(text):
    text = BeautifulSoup(ihtml.unescape(text), "lxml").text
    text = re.sub(r"http[s]?://\S+", "", text)
    text = re.sub(r"\s+", " ", text)
    return text


def clean(text):
    text = clean_text(text)
    text = re.sub(r'\+?\d{0,3}[-.\s]?\(?\d{3}\)?[-.\s]?\d{3,4}[-.\s]?\d{4}\b', '[PHONE]', text)  # removes phone numbers
    text = re.sub(r'\+?(\d[-.\s()]*){7,}\b', '[PHONE]', text)  # 000-000-0000 format us number
    text = re.sub(r'\+?\d{0,3}[-.\s]?\(?\d{3}\)?[-.\s]?\d{6,10}\b', '[PHONE]', text)  # indian phone numbers
    # removes dd-mm-yyyy, dd.mm.yyyy, day, abbreviated month name d, yyyy, dd/mm/yyyy hh:mm AM, and dd/mm/yyyy hh:mm PM
    text = re.sub(r'\b\d{1,2}[/-]\d{1,2}[/-]\d{4}(\s\d{1,2}:\d{2}\s(?:AM|PM))?\b|\b\w{3},\s\w{3}\s\d{1,2},\s\d{4}\b',
                  '[DATE]',
                  text)
    text = re.sub(r'\b\d{1,2}:\d{2}\s(?:AM|PM|am|pm)\b', '[TIME]', text)  # hh:mm am/AM/PM/pm

    text = re.sub(r"(?:^|(?<=[^\w@.)]))([\w+-](\.(?!\.))?)*?[\w+-](@|"
                  r"[(<{\[]at[)>}\]])(?:(?:[a-z\\u00a1-\\uffff0-9]-?)*[a-z\\u00a1-\\uffff0-9]+)(?:\.(?:[a-z\\u00a1-\\uffff0-9]-?)"
                  r"*[a-z\\u00a1-\\uffff0-9]+)*(?:\.(?:[a-z\\u00a1-\\uffff]{2,}))", "[EMAIL]", text)  # email

    text = re.sub(r'\$\d+(?:\.\d+)?', '[AMOUNT]', text)  # amount
    text = re.sub(r'\b[A-Z0-9]{2}[A-Z]-[A-Z0-9]{2}-[A-Z0-9]{2}[A-Z]\b', '[URI]',
                  text)  # uri like 98Z-MV-34V  "XXY-ZZ-WV", where X, Y, Z, and W are uppercase letters and digits:
    return text.lower()
