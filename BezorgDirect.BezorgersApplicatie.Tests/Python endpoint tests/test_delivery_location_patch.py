import requests
import json
from cerberus import Validator

def validateInput() : 
    file = open('C:\\Users\\Mats\\Documents\\School\\Jaar 4\\Python\\test_delivery_location_patch.json', 'r')
    jsonInput = file.read()
    request_input = json.loads(jsonInput)
    schema = {
        "latitude" : {"type" : "number"}, 
        "longitude" : {"type" : "number"}
    }
    v = Validator(schema)
    assert v.validate(request_input)
    return request_input
def validateOutput(request_input) : 
    url = "https://virtserver.swaggerhub.com/inholland1/Bezorg.direct/1.2.2/delivery/location"
    r = requests.patch(url, request_input)
    assert r.status_code == 200, "The status code is not 200!"

request_input = validateInput()
validateOutput(request_input)