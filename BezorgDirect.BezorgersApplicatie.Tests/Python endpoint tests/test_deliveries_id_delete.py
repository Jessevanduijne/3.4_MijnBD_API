import requests
import json
from cerberus import Validator

def validateOutput () : 
    url = "https://virtserver.swaggerhub.com/inholland1/Bezorg.direct/1.2.2/deliveries/7d8a4be5-52b9-4be2-943c-8f5826483730"
    r = requests.delete(url)
    assert r.status_code == 200, "De status code is geen 200!"

validateOutput()