################################################################################################
#  This file is part of the Oracle B2C Service Accelerator Reference Integration set published
#  by Oracle B2C Service licensed under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23C (August 2023) 
#  date: Tue Aug 22 11:57:49 IST 2023
 
#  revision: RNW-23C
#  SHA1: $Id: 1a6a22539b58b7f2c5e306ba9d6452cc1adcf448 $
################################################################################################
#  File: test_preprocess_data.py
################################################################################################
import unittest
from unittest.mock import patch
from chat_ingest_src.preprocess_data import remove_html_single, HTML_PARSER


class TestRemoveHTMLSingle(unittest.TestCase):

    def test_remove_html_single_no_html(self):
        input_text = "This is a test without any HTML."
        output_text = remove_html_single(input_text)
        self.assertEqual(output_text, input_text)

    def test_remove_html_single_with_html(self):
        input_text = "<p>This is <b>bold</b> <i>italic</i> text.</p>"
        expected_output = "This is bold italic text."
        output_text = remove_html_single(input_text)
        self.assertEqual(output_text, expected_output)

    @patch('chat_ingest_src.preprocess_data.HTML_CLEANER.clean_html')
    @patch('chat_ingest_src.preprocess_data.etree.fromstring')
    def test_remove_html_single_error_handling(self, mock_fromstring, mock_clean_html):
        mock_fromstring.return_value = None
        mock_clean_html.side_effect = ValueError("Some parsing error")

        input_text = "<p>This is some text.</p>"
        output_text = remove_html_single(input_text)

        mock_fromstring.assert_called_once_with(input_text, HTML_PARSER)
        self.assertEqual(output_text, input_text)


if __name__ == '__main__':
    unittest.main()
