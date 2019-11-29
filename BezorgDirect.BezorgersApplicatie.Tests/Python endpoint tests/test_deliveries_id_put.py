import requests
import json
from cerberus import Validator

def validateInput() : 
    file = open('C:\\Users\\Mats\\Documents\\School\\Jaar 4\\Python\\test_deliveries_id_put.json', 'r')
    jsonInput = file.read()
    request_input = json.loads(jsonInput)
    schema = {
        "Id" : {"type" : "string"}, 
        "DelivererId" : {"nullable" : True, "type" : "string"}, 
        "CustomerPhoneNumber" : {"type" : "string"}, 
        "DueDate" : {"type" : "string"}, 
        "Vehicle" : {"type" : "integer"}, 
        "VehicleDisplayName" : {"type" : "string"}, 
        "StartedAtId" : {"nullable" : True, "type" : "string"}, 
        "WarehouseId" : {"type" : "string"}, 
        "WarehousePickUpAt" : {"nullable" : True, "type" : "string"}, 
        "WarehouseETA" : {"nullable" : True, "type" : "string"}, 
        "WarehouseDistanceInKilometers" : {"nullable" : True, "type" : "number"},
        "CustomerDistanceInKilometers" : {"nullable" : True, "type" : "number"}, 
        "CustomerETA" : {"nullable" : True, "type" : "string"}, 
        "CustomerId" : {"type" : "string"},
        "CurrentId" : {"nullable" : True, "type" : "string"}, 
        "DeliveredAt" : {"nullable" : True, "type" : "string"},
        "Price" : {"type" : "number"}, 
        "Tip" : {"nullable" : True, "type" : "number"}, 
        "PaymentMethod" : {"type" : "integer"}, 
        "PaymentMethodDisplayName" : {"type" : "string"}, 
        "Status" : {"type" : "integer"}, 
        "StatusDisplayName" : {"type" : "string"}
    }
    v = Validator(schema)
    assert v.validate(request_input)
    return request_input
def validateOutput(request_input) : 
    header = {
    "Authorization" : "Bearer GERG15TRBrgd24EFhgrvfTHG34RHtrntyj1n65yj2"
    }
    url = "http://localhost:7071/api/Deliveries/DA44020E-E516-47E1-9DC5-032F196D7746"
    r = requests.put(url, json = request_input, headers = header)
    assert r.status_code == 200, "De statuscode is niet 200!!"


request_input = validateInput()
validateOutput(request_input)