import json
import re
import requests
from cerberus import Validator

#Reading Jason file
def validateInput() : 
    file = open('C:\\Users\\Mats\\Documents\\School\\Jaar 4\\Python\\test_deliverer_login.json', 'r')
    jsonInput = file.read()
    request_input = json.loads(jsonInput)

    jsonSchema = {
    "emailAddress" : {"type" : "string"}, 
    "password" : {"type" : "string"}
    }

    v = Validator(jsonSchema)
    assert v.validate(request_input)
    return request_input
def validateOutput(request_input) : 
    url = "http://localhost:7071/api/login"
    r= requests.post(url, json =request_input)
    j = r.json()

    #Validate the response
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
    assert v.validate(j), "De auth token is niet correct!"

#Request Api
request_input = validateInput()
validateOutput(request_input)