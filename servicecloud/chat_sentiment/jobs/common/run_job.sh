#!/usr/bin/env bash
################################################################################################
#  This file is part of the Oracle B2C Service Accelerator Reference Integration set published
#  by Oracle B2C Service licensed under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23C (August 2023) 
#  date: Tue Aug 22 11:57:48 IST 2023
 
#  revision: RNW-23C
#  SHA1: $Id: 7481a21b8df51702ac90c66d740ef2daf6ae0736 $
################################################################################################
#  File: run_job.sh
################################################################################################

set -eo pipefail

WORKSPACE=$(dirname "$(readlink -f "$0")")/..

sudo yum -y install tk-devel gdbm-devel
sudo yum -y install openssl-devel
sudo yum -y install libffi-devel
sudo yum -y install bzip2-devel

echo "*** Downloading & Installing Python-3.8.0 ***"
mkdir python
cd python || exit
wget https://www.python.org/ftp/python/3.8.0/Python-3.8.0.tgz
tar xvzf Python-3.8.0.tgz
cd Python-3.8.0 || exit
./configure --prefix=/opt/python3.8
make
sudo make altinstall

echo "*** ${PWD} ***" #"*** /home/datascience/python/Python-3.8.0 ***"

cd "${WORKSPACE}"

echo "*** ${PWD} ***" #"*** /home/datascience/decompressed_artifact/jobs ***"

echo "*** Creating virtual enviornment ****"
/opt/python3.8/bin/python3.8 -m venv .env
source .env/bin/activate

pip install --upgrade pip

pip install -r ./chat_ask_for_manager/requirements.txt
pip install -r ./chat_ingest_src/requirements.txt
pip install -r ./chat_model_train_src/requirements.txt
pip install -r ./chat_status_update_src/requirements.txt
echo "!!! Environment Created !!!"

export PYTHONPATH="${PYTHONPATH}:${PWD}"

echo "${WORKSPACE}"
python "${WORKSPACE}"/common/schedule_job.py
echo "!!! Job Complete !!!"
