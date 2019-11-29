import requests
import json
from cerberus import Validator

def makeJsonObjectReadable(j) :
    tempdata = json.dumps(j) 
    tempdata = "{\"rows\" : " + tempdata
    tempdata = tempdata + "}"
    return json.loads(tempdata)

def validateInput() : 
    file = open('C:\\Users\\Mats\\Documents\\School\\Jaar 4\\Python\\test_deliveries_id_review_post.json', 'r')
    jsonInput = file.read()
    jsonInput = json.loads(jsonInput)
    request_input = makeJsonObjectReadable(jsonInput)
    schema = {
        "rows" : {"type" : "list", "schema" : {
            "type" : "dict", "schema" : {
                "category" : {"type" : "integer"}, 
                "rating" : {"type" : "integer"}
            }
        }}
    }
    v = Validator(schema)
    assert v.validate(request_input)
    return jsonInput

def validateOutput (request_input) : 
    url = "http://localhost:7071/api/Deliveries/DA44020E-E516-47E1-9DC5-032F196D7746/Feedback"
    header = {
    "Authorization" : "Bearer GERG15TRBrgd24EFhgrvfTHG34RHtrntyj1n65yj2"
    }
    r = requests.post(url, json = request_input, headers=header)
    j = r.json()
    j = makeJsonObjectReadable(j)

    schema = {
        "rows" : {"type" : "list", "schema" : {
            "type" : "dict", "schema" : {
                "Id" : {"type" : "string"}, 
                "DelivererId" : {"type" : "string"}, 
                "DeliveryId" : {"type" : "string"}, 
                "Category" : {"type" : "integer"}, 
                "Rating" : {"type" : "integer"}, 
                "CategoryDisplayName" : {"type" : "string"}
            }
        }}
    }

    v = Validator(schema)
    assert v.validate(j)

request_input = validateInput()

validateOutput(request_input)