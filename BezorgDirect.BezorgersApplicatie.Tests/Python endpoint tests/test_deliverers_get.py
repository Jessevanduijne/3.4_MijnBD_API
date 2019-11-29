import requests
import json
from cerberus import Validator

def makeJsonObjectReadable(j) :
    tempdata = json.dumps(j) 
    tempdata = "{\"rows\" : " + tempdata
    tempdata = tempdata + "}"
    return json.loads(tempdata)

def validateOutput () : 
    url = "http://localhost:7071/api/Deliverers/admin"
    header = {
        "Authorization" : "Bearer GERG15TRBrgd24EFhgrvfTHG34RHtrntyj1n65yj2"
    }
    r = requests.get(url, headers=header)
    jason = r.json()
    j = makeJsonObjectReadable(jason)
    schema = {
        "rows" : {"type" : "list", "schema" : {
            "type" : "dict", "schema" : {
                "Id" : {"type" : "string"}, 
                "EmailAddress" : {"type" : "string"}, 
                "Token" : {"type" : "string"}, 
                "DateOfBirth" : {"type" : "string"}, 
                "Fare" : {"type" : "number"}, 
                "Home" : {"type" : "dict", "schema" : {
                    "Id" : {"type" : "string"}, 
                    "Address" : {"type" : "string"}, 
                    "latitude" : {"type" : "number"}, 
                    "longitude" : {"type" : "number"}, 
                    "place" : {"type" : "string"}, 
                    "postalCode" : {"type" : "string"}
                }}, 
                "phoneNumber" : {"type" : "string"}, 
                "range" : {"type" : "number"}, 
                "vehicle" : {"type" : "integer"}, 
                "TotalEarnings" : {"nullable" : True, "type" : "number"}
            }
        }}
    }
    v = Validator(schema)
    v.validate(j)
    print(v.errors)
    assert r.status_code == 200

validateOutput()