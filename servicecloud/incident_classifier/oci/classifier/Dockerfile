################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 12:41:50 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 84d8475d993772f2b10e01b7f10ada7a5358afa2 $
################################################################################################
#  File: docker-compose.yml
################################################################################################

FROM fnproject/python:3.8-dev as build-stage

WORKDIR /function
ADD requirements.txt /function/
ADD src /function/
#dependency for fn
RUN pip3 install --target /python/  fdk==0.1.48

#dependencies for other functionalities are defined in requirements.txt
RUN pip3 install --target /python/ --no-cache --no-cache-dir -r requirements.txt &&\
      rm -fr ~/.cache/pip /tmp* requirements.txt func.yaml Dockerfile .venv &&\
      chmod -R o+r /python

WORKDIR /python
RUN pip3 install --target /python/ https://github.com/explosion/spacy-models/releases/download/en_core_web_lg-3.4.1/en_core_web_lg-3.4.1.tar.gz

ADD . /function/
RUN rm -fr /function/.pip_cache

FROM fnproject/python:3.8

#Containers must run as a non-privileged account
RUN adduser fnuser

WORKDIR /function
RUN microdnf upgrade && microdnf clean all

COPY --from=build-stage /python /python
COPY --from=build-stage /function /function
RUN chmod -R o+r /function
USER fnuser

ENV PYTHONPATH="${PYTHONPATH}:/function:/python"
ENTRYPOINT ["/python/bin/fdk", "/function/func.py", "handler"]