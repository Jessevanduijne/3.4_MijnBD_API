import requests
import json
from cerberus import Validator

def makeJsonObjectReadable(j) :
    tempdata = json.dumps(j) 
    tempdata = "{\"rows\" : " + tempdata
    tempdata = tempdata + "}"
    return json.loads(tempdata)

def validateInput() : 
    file = open('C:\\Users\\Mats\\Documents\\School\\Jaar 4\\Python\\test_availabilities_post.json', 'r')
    jsonInput = file.read()
    jsonToApplication = json.loads(jsonInput)
    request_input = makeJsonObjectReadable(jsonToApplication)
    schema = {
        "rows" : {"type" : "list", "schema" : {
            "type" : "dict", "schema" : {
                "date" : {"type" : "string"}, 
                "endTime" : {"type" : "string"}, 
                "startTime" : {"type" : "string"}
            }
        }}
    }
    v = Validator(schema)
    assert v.validate(request_input)
    return jsonToApplication
def validateOutput(request_input) :
    url = "http://localhost:7071/api/Availabilities"
    header = {
        "Authorization" : "Bearer 997a139c203f4c2183cef6babc3ce469"
    }
    r = requests.post(url, json = request_input, headers= header)
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