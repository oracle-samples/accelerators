
################################################################################################
#  $ACCELERATOR_HEADER_PLACE_HOLDER$
#  SHA1: $Id:  $
################################################################################################
#  File: $ACCELERATOR_HEADER_FILE_NAME_PLACE_HOLDER$
################################################################################################
####################################
# DataScience related resources
####################################

import csv
import random
from sklearn.datasets import fetch_20newsgroups

"""
import ssl

# Set SSL certificates path if getting unverified SSL error
ssl._create_default_https_context = ssl._create_unverified_context
"""

# Set the number of samples and the desired label distribution
num_samples = 200
label_distribution = [1, 0] * (num_samples // 2)

# Shuffle the label distribution
random.shuffle(label_distribution)

# Load the 20 Newsgroups dataset
newsgroups_dataset = fetch_20newsgroups(subset='all', shuffle=True, random_state=42)

# Get random samples from the dataset
random_indices = random.sample(range(len(newsgroups_dataset.data)), num_samples)
random_samples = [newsgroups_dataset.data[i] for i in random_indices]
random_labels = [label_distribution[i % len(label_distribution)] for i in range(num_samples)]

# Create the CSV file
csv_file = "data.csv"

with open(csv_file, mode='w', newline='', encoding='utf-8') as file:
    writer = csv.writer(file)
    writer.writerow(["text", "labels"])  # Write the header
    writer.writerows(zip(random_samples, random_labels))  # Write the rows

print(f"CSV file '{csv_file}' created successfully!")
