#!/usr/bin/env bash
################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:54 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 6ebcf0dc4197aa477f123cd1e8b5d533d6daa8c7 $
################################################################################################
#  File: run_selenium.sh
################################################################################################
# *** messages for status
# !!! messages for success
# ### messages for failure

set -eo pipefail

WORKSPACE=$(dirname "$(readlink -f "$0")" )/..

echo "*** Installing selenium relared linux dependencies ***"
sudo yum -y install tk-devel gdbm-devel
sudo yum -y install openssl-devel
sudo yum -y install libffi-devel

echo "*** Downloading & Installing Python-3.8.0 ***"
mkdir python
cd python || exit
wget https://www.python.org/ftp/python/3.8.0/Python-3.8.0.tgz
tar xvzf Python-3.8.0.tgz
cd Python-3.8.0 || exit
./configure --prefix=/opt/python3.8
make
sudo make altinstall

echo "*** Creating virtual enviornment ****"
/opt/python3.8/bin/python3.8 -m venv .env
source .env/bin/activate

echo "*** Downloading & Installing Selenium releated dependencies ***"
pip install --upgrade pip
pip install selenium==4.6.0
wget https://dl.google.com/linux/direct/google-chrome-stable_current_x86_64.rpm
sudo yum -y install ./google-chrome-stable_current_*.rpm
CHROME_VERSION=$(/usr/bin/google-chrome --version | awk '{print $3}')
echo "CHROME_VERSION : ${CHROME_VERSION}"
CHROME_DRIVER_WEB_PATH=$(curl https://chromedriver.storage.googleapis.com/ | grep -o "<Key>${CHROME_VERSION%\.*}.*</Key>" | awk -F">" '{print $2}' | awk -F"<" '{print $1}')
wget https://chromedriver.storage.googleapis.com/"${CHROME_DRIVER_WEB_PATH}"
FILE_PATH=$(echo "${CHROME_DRIVER_WEB_PATH}" | awk -F'/' '{print $2}')
unzip "$FILE_PATH"


echo "${WORKSPACE}";
python "${WORKSPACE}"/jobs/run_notebook.py
echo "!!! Conda Environment Created !!!"
