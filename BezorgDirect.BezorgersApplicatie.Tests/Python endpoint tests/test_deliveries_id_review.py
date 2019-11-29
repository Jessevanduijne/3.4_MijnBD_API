import requests
import json
from cerberus import Validator
def makeJsonObjectReadable(j) :
    tempdata = json.dumps(j) 
    tempdata = "{\"rows\" : " + tempdata
    tempdata = tempdata + "}"
    return json.loads(tempdata)

def validateOutput() : 
    url = "http://localhost:7071/api/Deliveries/DA44020E-E516-47E1-9DC5-032F196D7746/Feedback"
    header = {
        "Authorization" : "Bearer GERG15TRBrgd24EFhgrvfTHG34RHtrntyj1n65yj2"
    }
    r = requests.get(url, headers=header)
    j = r.json()
    j = makeJsonObjectReadable(j)
    schema = {
        "rows" : {"type" : "list", "schema" : {
            "type" : "dict", "schema" : {
                "Id" : {"type" : "string"}, 
                "DeliveryId" : {"type" : "string"}, 
                "DelivererId" : {"type" : "string"}, 
                "Category" : {"type" : "integer"}, 
                "CategoryDisplayName" : {"type" : "string"}, 
                "Rating" : {"type" : "integer"}
            }
        }}
    }
    v = Validator(schema)
    assert v.validate(j)

validateOutput()