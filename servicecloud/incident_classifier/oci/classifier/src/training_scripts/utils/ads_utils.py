################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 12:41:59 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: ee1d34142f1e95931a43a9e8d73f2e8b5e7e9818 $
################################################################################################
#  File: ads_utils.py
################################################################################################
import os
import tempfile

import pickle
from ads.common.model_artifact import ModelArtifact


def save_generic_model(model: ModelArtifact, path: str):
    # Serialize the templates
    with open(os.path.join(path, "templates.pkl"), "wb") as outfile:
        pickle.dump(model, outfile, protocol=pickle.HIGHEST_PROTOCOL)


def generate_artifact_path():
    artifact_path = tempfile.mkdtemp()
    model_artifact_path = os.path.join(artifact_path, "model_artifacts")
    if not os.path.exists(model_artifact_path):
        os.mkdir(model_artifact_path)
    return model_artifact_path


def list_model_template_artifacts(path: str):
    # List the template files
    for file in os.listdir(path):
        if os.path.isdir(os.path.join(path, file)):
            for file2 in os.listdir(os.path.join(path, file)):
                print(os.path.join(file, file2))
        else:
            print(file)
