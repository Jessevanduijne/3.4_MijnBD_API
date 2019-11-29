import json
import requests
from cerberus import Validator

def validateInput () : 
    file = open('C:\\Users\\Mats\\Documents\\School\\Jaar 4\\Python\\test_deliverer_me_put.json', 'r')
    jsonInput = file.read()
    request_input = json.loads(jsonInput)

    schema = {
    "id" : {"type" : "string"}, 
    "emailAddress" : {"type" : "string"}, 
    "dateOfBirth" : {"type" : "string"}, 
    "fare" : {"type" : "number"}, 
    "home" : {"type" : "dict", "schema" : {
        "id" : {"type" : "string"}, 
        "address" : {"type" : "string"}, 
        "isWarehouse" : {"type" : "boolean"}, 
        "latitude" : {"type" : "number"}, 
        "longitude" : {"type" : "number"}, 
        "place" : {"type" : "string"}, 
        "postalCode" : {"type" : "string"}
    }}, 
    "phoneNumber" : {"type" : "string"}, 
    "range" : {"type" : "number"}, 
    "vehicle" : {"type" : "integer"}
    }
    v = Validator(schema)
    assert v.validate(request_input)
    return request_input
def validateOutput (request_input) : 
    header = {
    'Authorization': 'Bearer 997a139c203f4c2183cef6babc3ce469'}
    
    url = "http://localhost:7071/api/me"
    r= requests.put(url, json = request_input, headers=header)
    assert r.status_code == 200, "De statuscode is geen 200!"

request_input = validateInput()
validateOutput(request_input)