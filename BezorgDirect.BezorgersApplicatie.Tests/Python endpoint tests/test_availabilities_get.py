import requests
import json
from cerberus import Validator

def makeJsonObjectReadable(j) :
    tempdata = json.dumps(j) 
    tempdata = "{\"rows\" : " + tempdata
    tempdata = tempdata + "}"
    return json.loads(tempdata)

def validateOutput () : 
    headers = {
           'Authorization': 'Bearer 997a139c203f4c2183cef6babc3ce469'}
    url = "http://localhost:7071/api/Availabilities"
    r = requests.get(url, headers=headers)
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

validateOutput()