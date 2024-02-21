#!/usr/bin/env bash

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

cd ${WORKSPACE}

echo "*** ${PWD} ***" #"*** /home/datascience/decompressed_artifact/classifier/src/training_scripts ***"

echo "*** Creating virtual enviornment ****"
/opt/python3.8/bin/python3.8 -m venv .env
source .env/bin/activate

pip install --upgrade pip

pip install -r requirements.txt

echo "${WORKSPACE}"
python "${WORKSPACE}"/jobs/schedule_job.py
echo "!!! Environment Created !!!"
