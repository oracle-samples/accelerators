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
#  SHA1: $Id: 63d31ddce1c5d6ce9ebf7905a916525d61135a3f $
################################################################################################
#  File: test_populate_sentiments.py
################################################################################################
import unittest
from unittest.mock import patch, Mock

from chat_ingest_src import populate_sentiments
import pandas as pd

from chat_ingest_src.populate_sentiments import load_model, split_set_into_equal_sizes


class TestProcessChatEmotion(unittest.TestCase):

    @patch("chat_ingest_src.populate_sentiments.pipeline")
    @patch("chat_ingest_src.populate_sentiments.os.getenv")
    def test_load_model(self, mock_os_getenv, mock_pipeline):
        # Mock environment variable and pipeline
        mock_os_getenv.side_effect = lambda key, default: {
            "EMOTION_MODEL": "arpanghoshal/EmoRoBERTa"
        }.get(key, default)

        mock_emotion = Mock()
        mock_pipeline.return_value = mock_emotion

        # Call the function
        result = load_model()

        # Assertions
        mock_os_getenv.assert_called_once_with("EMOTION_MODEL", "arpanghoshal/EmoRoBERTa")
        mock_pipeline.assert_called_once_with('sentiment-analysis', model="arpanghoshal/EmoRoBERTa")
        self.assertEqual(result, mock_emotion)

    def test_split_set_into_equal_sizes(self):
        input_set = {1, 2, 3, 4, 5, 6, 7, 8, 9}
        subset_size = 3

        expected_subsets = [
            {1, 2, 3},
            {4, 5, 6},
            {7, 8, 9}
        ]

        subsets = split_set_into_equal_sizes(input_set, subset_size)
        self.assertEqual(len(subsets), len(expected_subsets))

        for subset, expected_subset in zip(subsets, expected_subsets):
            self.assertEqual(subset, expected_subset)

    def test_split_set_into_equal_sizes_odd_subset(self):
        input_set = {1, 2, 3, 4, 5, 6, 7}
        subset_size = 4

        expected_subsets = [
            {1, 2, 3, 4},
            {5, 6, 7}
        ]

        subsets = split_set_into_equal_sizes(input_set, subset_size)
        self.assertEqual(len(subsets), len(expected_subsets))

        for subset, expected_subset in zip(subsets, expected_subsets):
            self.assertEqual(subset, expected_subset)

    @patch('chat_ingest_src.populate_sentiments.load_model')
    def test_predict(self, mock_load_model):
        input_data = ['text1', 'text2']
        chat_id = 'chat_id'
        model_mock = Mock()
        model_mock.predict.return_value = [{'label': 'positive', 'score': 0.85}, {'label': 'negative', 'score': 0.75}]
        mock_load_model.return_value = model_mock

        emotion_labels, input_texts, result_chat_id = populate_sentiments.predict(input_data, chat_id, model_mock)
        self.assertEqual(emotion_labels, [{'label': 'positive', 'score': 0.85}, {'label': 'negative', 'score': 0.75}])
        self.assertEqual(input_texts, input_data)
        self.assertEqual(result_chat_id, chat_id)

    @patch('chat_ingest_src.populate_sentiments.process_chat')
    @patch('chat_ingest_src.populate_sentiments.load_model')
    def test_process_sentiments(self, mock_load_model, mock_process_chat):
        columns = ['Chat ID', 'text', 'other_column']
        rows = [
            ['chat1', 'text1', 'val1'],
            ['chat1', 'text2', 'val2'],
            ['chat2', 'text3', 'val3']
        ]
        df_mock = Mock()
        chat_result_mock = [
            {'Chat ID': 'chat1', 'text': 'text1', 'labels': 'positive', 'emotion': 'happy', 'score': 80},
            {'Chat ID': 'chat1', 'text': 'text2', 'labels': 'negative', 'emotion': 'sad', 'score': 70}
        ]
        mock_load_model.return_value = Mock()
        mock_process_chat.return_value = chat_result_mock

        labeled_chat = populate_sentiments.process_sentiments(columns, rows)
        self.assertEqual(labeled_chat.loc[0].to_dict(), chat_result_mock[0])
        self.assertEqual(labeled_chat.loc[1].to_dict(), chat_result_mock[1])

    @patch('chat_ingest_src.populate_sentiments.get_object_storage_client')
    @patch('chat_ingest_src.populate_sentiments.store_or_update_csv')
    def test_upload_csv_file(self, mock_store_or_update_csv, mock_get_object_storage_client):
        ingest_bucket_location = 'bucket_location'
        filename = 'test_file.csv'
        labeled_chat = pd.DataFrame({'Chat ID': ['chat1'], 'text': ['text1'],
                                     'labels': ['positive'], 'emotion': ['happy'], 'score': [80]})
        object_storage_client_mock = Mock()
        populate_sentiments.get_object_storage_client.return_value = object_storage_client_mock
        populate_sentiments.upload_csv_file(ingest_bucket_location, filename, labeled_chat)
        mock_store_or_update_csv.assert_called_once_with(
            f"{ingest_bucket_location}/{filename}",
            labeled_chat.to_csv(encoding='utf-8'),
            object_storage_client_mock
        )

    def test_label2id(self):
        self.assertEqual(populate_sentiments.label2id('neutral'), populate_sentiments.NEUTRAL_CHAT)
        self.assertEqual(populate_sentiments.label2id('positive'), populate_sentiments.POSITIVE_CHAT)
        self.assertEqual(populate_sentiments.label2id('negative'), populate_sentiments.NEGATIVE_CHAT)

    def test_process_feedback_sentiments(self):
        columns = ['Chat ID', 'text', 'labels', 'emotion', 'score']
        rows = [
            ['chat1', 'text1', 'neutral', 'happy', 80],
            ['chat1', 'text2', 'positive', 'joyful', 90]
        ]
        expected_result = [
            ['chat1', 'text1', populate_sentiments.NEUTRAL_CHAT, 'happy', 80],
            ['chat1', 'text2', populate_sentiments.POSITIVE_CHAT, 'joyful', 90]
        ]
        result_df = populate_sentiments.process_feedback_sentiments(columns, rows)
        self.assertEqual(result_df.values.tolist(), expected_result)


if __name__ == '__main__':
    unittest.main()
