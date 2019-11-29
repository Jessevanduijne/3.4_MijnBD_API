import json
import requests
from cerberus import Validator

def validateInput() : 
    file = open('C:\\Users\\Mats\\Documents\\School\\Jaar 4\\Python\\test_deliverer_register.json', 'r')
    jsonInput = file.read()
    request_input = json.loads(jsonInput)
    schema = {
        "emailAddress" : {"type" : "string"}, 
        "password" : {"type" : "string"}, 
        "dateOfBirth" : {"type" : "string"}, 
        "fare" : {"type" : "number"}, 
        "home" : {"type" : "dict", "schema" : {
            "address" : {"type" : "string"}, 
            "postalCode" : {"type" : "string"}, 
            "place" : {"type" : "string"}
        }},
        "phoneNumber" : {"type" : "string"}, 
        "range" : {"type" : "number"}, 
        "vehicle" : {"type" : "integer"} 
    }
    v = Validator(schema)
    assert v.validate(request_input)
    return request_input

def validateOutput(request_input) : 
    url = "http://localhost:7071/api/register"
    r= requests.post(url, json =request_input)
    j = r.json()
    schema = {
        "id" : {"type" : "string"}, 
        "emailAddress" : {"type" : "string"}, 
        "token" : {"type" : "string", "regex" : "\S*(\S*([a-zA-Z]\S*[0-9])|([0-9]\S*[a-zA-Z]))\S*"}, 
        "dateOfBirth" : {"type" : "string"}, 
        "fare" : {"type" : "number"}, 
        "home" : {"type" : "dict", "schema" : {
            "id" : {"type" : "string"}, 
            "address" : {"type" : "string"}, 
            "latitude" : {"type" : "number"}, 
            "longitude" : {"type" : "number"}, 
            "place" : {"type" : "string"}, 
            "postalCode" : {"type" : "string"}
        }}, 
        "phoneNumber" : {"type" : "string"}, 
        "range" : {"type" : "number"}, 
        "vehicle" : {"type" : "integer"},
        "totalEarned" : {"nullable" : True, "type" : "number"}
    }
    v = Validator(schema)
    assert r.status_code == 200, "De status code is geen 200!"
    assert v.validate(j)
    

request_input = validateInput()
validateOutput(request_input)
