import requests
import json
from cerberus import Validator

def makeJsonObjectReadable(j) :
    tempdata = json.dumps(j) 
    tempdata = "{\"rows\" : " + tempdata
    tempdata = tempdata + "}"
    return json.loads(tempdata)

def validateInput() : 
    file = open('C:\\Users\\Mats\\Documents\\School\\Jaar 4\\Python\\test_availabilities_put.json', 'r')
    jsonInput = file.read()
    originalJson = json.loads(jsonInput)
    request_input = makeJsonObjectReadable(originalJson)
    schema = {
            "rows" : {"type" : "list", "schema" : {
            "type" : "dict", "schema" : {
                "id" : {"type" : "string"},
                "date" : {"type" : "string"}, 
                "delivererId" : {"type" : "string"},
                "endTime" : {"type" : "string"}, 
                "startTime" : {"type" : "string"}
            }
        }}
    }
    v = Validator(schema)
    assert v.validate(request_input)
    return originalJson

def validateOutput(request_input) :
    url = "http://localhost:7071/api/Availabilities"
    header = {
        "Authorization" : "Bearer 997a139c203f4c2183cef6babc3ce469"
    }
    r = requests.put(url, json = request_input, headers=header)
    j = r.json()
    j = makeJsonObjectReadable(j)
    schema = {
        "rows" : {"type" : "list", "schema" : {
            "type" : "dict", "schema" : {
                "Id" : {"type" : "string"}, 
                "Date" : {"type" : "string"}, 
                "DelivererId" : {"type" : "string"}, 
                "EndTime" : {"type" : "string"}, 
                "StartTime" : {"type" : "string"}
            }
        }}
    }
    v = Validator(schema)
    assert v.validate(j)
    assert r.status_code == 200, "De statuscode is geen 200!!"

request_input = validateInput()

validateOutput(request_input)