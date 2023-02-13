################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:45 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 5c7868eecf11690fb703254a186dd2526d31d610 $
################################################################################################
#  File: check_endpoint.py
################################################################################################

# The OCI SDK must be installed for this example to function properly.
# Installation instructions can be found here: https://docs.oracle.com/en-us/iaas/Content/API/SDKDocs/pythonsdk.htm
import requests
import oci
from oci.signer import Signer

config = oci.config.from_file("~/.oci/config",
                              profile_name="DEFAULT")  # replace with the location of your oci config file


def main(endpoint):
    auth = Signer(
        tenancy=config['tenancy'],
        user=config['user'],
        fingerprint=config['fingerprint'],
        private_key_file_location=config['key_file'],
        pass_phrase=config['pass_phrase'])

    body = {"jsonData": {
        "inquiry": "My iPhone is not working",
        "product": 0,
        "category": 0,
        "disposition": 0
    }}

    print(body)
    response = requests.post(endpoint, json=body, auth=auth)
    print(response.json())


if __name__ == '__main__':
    main("pass model deployment endpoint here..")
