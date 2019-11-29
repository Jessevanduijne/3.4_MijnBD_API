import json
import requests
from cerberus import Validator
def logoutCheck (url) : 
    header = {
        "Authorization" : "Bearer 839b7f2f47af450a8bcff7f0948bfe93"
    }
    r = requests.post(url, headers = header)
    #Validate response
    assert r.status_code == 200, "De status code is geen 200!"

#Request Api
url = "http://localhost:7071/api/logout"
logoutCheck(url)
