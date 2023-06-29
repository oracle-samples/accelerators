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