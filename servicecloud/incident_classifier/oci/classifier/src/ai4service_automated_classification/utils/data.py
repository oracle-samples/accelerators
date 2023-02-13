################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:51 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 976e0d01eefe8cf2f1f54a94818e85c1bd598fe2 $
################################################################################################
#  File: data.py
################################################################################################
import os
from typing import List

import numpy as np
from unicodedata import normalize

from lxml import etree
from lxml.html import HTMLParser
from lxml.html.clean import Cleaner
from spacy.language import Language

from ai4service_automated_classification.constants import DAYS_F_RGX, DAYS_S_RGX, MONTHS_F_RGX, MONTHS_S_RGX, \
    NUMBERS_REGEX, PHONE_REGEX, EMAIL_REGEX, URL_REGEX, TOKEN_RGX, CLEANUP_SCOPE, STOP_WORDS

HTML_PARSER = HTMLParser()
HTML_CLEANER = Cleaner(allow_tags=[])
URL_TOK = "[URL]"
EMAIL_TOK = "[EMAIL]"
PHONE_TOK = "[PHONE]"
NUMBER_TOK = "[NUMBER]"
DATE_TOK = "[DATE]"

CLEANED_TOKS = [URL_TOK, EMAIL_TOK, PHONE_TOK, NUMBER_TOK, DATE_TOK]


def remove_html_single(text: str) -> str:
    """Removes html info from a single text string.

    Args:
        text (str): input text

    Returns:
        str: output text with no html
    """
    if "<" in text:
        value = etree.fromstring(text, HTML_PARSER)
        if value is not None:
            try:
                text = HTML_CLEANER.clean_html(value).text_content()
            except ValueError as e:
                print(f'Some issues parsing document: "{text}" \n"{str(e)}"')
    return text


def tokenize_regex(text: str) -> List:
    """Destructive tokenizer that returns alphanumeric words.

    Args:
        text (str): input text

    Returns:
        List: list of words
    """
    return TOKEN_RGX.findall(text)


def clean_text(text: str, to_remove: List[str] = None) -> str:
    """Cleans a text from unwanted strings.

    Args:
        text (str): input text
        to_remove (list, optional): List of unwanted strings.
        Defaults to [" ", ""] since those were ecountered often.

    Returns:
        str: text without unwanted strings
    """
    if to_remove is None:
        to_remove = [" ", ""]
    for removable in to_remove:
        text = text.replace(removable, " ")
    return text


def clean_urls(text: str, placeholder: str = URL_TOK) -> str:
    """Search and replace URLs

    Args:
        text (str): input text
        placeholder (str, optional): thing to replace the URL with. Defaults to URL_TOK.

    Returns:
        str: input text with URLs replaced by placeholder
    """
    return URL_REGEX.sub(placeholder, text)


def clean_emails(text: str, placeholder: str = EMAIL_TOK) -> str:
    """Search and replace emails

    Args:
        text (str): input text
        placeholder (str, optional): thing to replace the EMAIL with.
        Defaults to EMAIL_TOK.

    Returns:
        str: input text with email addresses replaced by placeholder
    """
    return EMAIL_REGEX.sub(placeholder, text)


def clean_phone_numbers(text: str, placeholder: str = PHONE_TOK) -> str:
    """Search and replace phone numbers

    Args:
        text (str): input text
        placeholder (str, optional): thing to replace the PHONE with.
        Defaults to PHONE_TOK.

    Returns:
        str: input text with phone numbers replaced by placeholder
    """
    return PHONE_REGEX.sub(placeholder, text)


def clean_numbers(text: str, placeholder: str = NUMBER_TOK) -> str:
    """Search and replace numbers

    Args:
        text (str): input text
        placeholder (str, optional): thing to replace the NUMBER with.
        Defaults to NUMBER_TOK.

    Returns:
        str: input text with numbers replaced by placeholder
    """
    return NUMBERS_REGEX.sub(placeholder, text)


def clean_dates(text: str, placeholder: str = DATE_TOK) -> str:
    """Search and replace days and months based on global locale.

    Args:
        text (str): input text
        placeholder (str, optional): thing to replace the DATE with.
        Defaults to DATE_TOK.

    Returns:
        str: input text with days and months replaced by placeholder
    """
    text = DAYS_F_RGX.sub(placeholder, text)
    text = DAYS_S_RGX.sub(placeholder, text)
    text = MONTHS_F_RGX.sub(placeholder, text)
    return MONTHS_S_RGX.sub(placeholder, text)


def text_clean(text: str, nlp: Language) -> str:
    """String cleaning function, see return value.

    Args:
        text (str): Input string
        nlp: Spacy's pre-trained language model. For ex: nlp = spacy.load("en_core_web_lg")

    Returns:
        str: Removes HTML and replaces URLs, emails, numbers and phone numbers with constant keywords

    """


    cleanup_scope: List = os.getenv(CLEANUP_SCOPE, "html").lower().replace(" ", "").split(",")
    stop_words: List = os.getenv(STOP_WORDS, "").lower().split(",")
    text = clean_text(text.lower())

    if "html" in cleanup_scope:
        text = remove_html_single(text)

    text = normalize('NFKD', text)

    if "urls" in cleanup_scope:
        text = clean_urls(text)
        text = text.replace(URL_TOK, '')

    if "emails" in cleanup_scope:
        text = clean_emails(text)
        text = text.replace(EMAIL_TOK, '')

    if "numbers" in cleanup_scope:
        text = clean_numbers(text)
        text = text.replace(NUMBER_TOK, '')

    if "phone" in cleanup_scope:
        text = clean_phone_numbers(text)
        text = text.replace(PHONE_TOK, '')

    if "date" in cleanup_scope:
        text = clean_dates(text)
        text = text.replace(DATE_TOK, '')

    if not nlp is None:
        text = text.replace("\n", " ")
        doc = nlp(text)
        if "person" in cleanup_scope:
            for ent in doc.ents:
                if ent.label_ in ['PERSON', 'DATE']:
                    text = text.replace(ent.text, " ")

    if len(stop_words) > 1:
        for word in stop_words:
            text = text.replace(word, " ")

    if not nlp is None:
        nlp.vocab["not"].is_stop = False
        text = " ".join(token.lemma_ for token in nlp(text) if
                        not token.is_punct
                        and not token.is_currency
                        and not token.is_digit
                        and not token.is_punct
                        and not token.is_oov
                        and not token.is_space
                        and not token.is_stop
                        and not token.like_num
                        and not token.pos_ == "PROPN")

    if len(text) <= 1:
        text = np.nan

    return text
