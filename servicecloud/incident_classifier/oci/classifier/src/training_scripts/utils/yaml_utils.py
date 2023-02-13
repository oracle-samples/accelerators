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
#  SHA1: $Id: ac83d48aa3189901167e14ab47fb23c84ae82317 $
################################################################################################
#  File: yaml_utils.py
################################################################################################
import yaml, re, os

ENV_PATTERN = re.compile(r".*?\${(.*?)}.*?")


def env_constructor(loader, node):
    value = loader.construct_scalar(node)
    for group in ENV_PATTERN.findall(value):
        if os.environ.get(group) is None:
            os.environ.setdefault(group, "")
        value = value.replace(f"${{{group}}}", os.environ.get(group))
    return value


def parse_yaml(filepath):
    yaml.add_implicit_resolver("!pathex", ENV_PATTERN)
    yaml.add_constructor("!pathex", env_constructor)

    with open(filepath, 'r') as file:
        data = yaml.load(file.read(), Loader=yaml.FullLoader)
        print(data)

    with open(filepath, 'w') as file:
        yaml.dump(data, file)
