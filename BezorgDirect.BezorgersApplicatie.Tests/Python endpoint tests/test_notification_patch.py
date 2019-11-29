import requests
import json
from cerberus import Validator

def validateInput() : 
    file = open('C:\\Users\\Mats\\Documents\\School\\Jaar 4\\Python\\test_notification_patch.json', 'r')
    jsonInput = file.read()
    request_input = json.loads(jsonInput)
    schema = {
        "accept" : {"type" : "boolean"}
    }
    v = Validator(schema)
    assert v.validate(request_input)
    return request_input

def validateOutput(request_input) : 
    url = "http://localhost:7071/api/notifications/D7FC12B6-9C9B-4DA3-B026-054626B73940"
    r = requests.patch(url, request_input)
    assert r.status_code == 200, "De status code is geen 200!"

request_input = validateInput()
validateOutput(request_input)