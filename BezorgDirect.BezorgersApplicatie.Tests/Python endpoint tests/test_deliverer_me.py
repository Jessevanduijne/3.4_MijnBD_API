import json
import requests
from cerberus import Validator

def validateOutput() : 
    url = "http://localhost:7071/api/me"
    header = {
           'Authorization': 'Bearer 997a139c203f4c2183cef6babc3ce469'}
    r= requests.get(url, headers=header)
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
    assert v.validate(j)
    assert r.status_code == 200, "De status code is geen 200!"

validateOutput()
#Request Api
# val = Validator(schema)
# print(val.validate(j))
# print(val.errors)
# #Validate the response
# assert r.status_code == 200, "De status code is geen 200!"