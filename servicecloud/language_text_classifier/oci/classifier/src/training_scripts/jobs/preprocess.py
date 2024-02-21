
################################################################################################
#  $ACCELERATOR_HEADER_PLACE_HOLDER$
#  SHA1: $Id:  $
################################################################################################
#  File: $ACCELERATOR_HEADER_FILE_NAME_PLACE_HOLDER$
################################################################################################
####################################
# DataScience related resources
####################################

import re
from io import StringIO

from bs4 import BeautifulSoup

from constants import LANGUAGE_SERVICE_CHAR_LIMIT


def clean_text(text):
    soup = BeautifulSoup(StringIO(text), "html.parser")
    text = soup.get_text()
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

    text = re.sub(r'\S+\.com', '[DOMAIN]', text)  # URL like example.com

    text = text.replace("[PHONE]", " ")
    text = text.replace("[DATE]", " ")
    text = text.replace("[TIME]", " ")
    text = text.replace("[EMAIL]", " ")
    text = text.replace("[URI]", " ")
    text = text.replace("[AMOUNT]", " ")
    text = text.replace("[DOMAIN]", " ")

    text = text[:LANGUAGE_SERVICE_CHAR_LIMIT]  # language service limit
    return text.lower()
