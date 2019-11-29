import requests
import json
from cerberus import Validator

def validateOutput() : 
    url = "http://localhost:7071/api/Availabilities/e28f289f-89e1-411a-a932-08d761351604"
    header = {
        "Authorization" : "Bearer 997a139c203f4c2183cef6babc3ce469"
    }
    r = requests.delete(url, headers= header)
    assert r.status_code == 200, "De status code is geen 200!"

validateOutput()