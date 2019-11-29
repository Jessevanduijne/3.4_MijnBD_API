import requests
import json
from cerberus import Validator

def validateInput() : 
    file = open('C:\\Users\\Mats\\Documents\\School\\Jaar 4\\Python\\test_deliveries_post.json', 'r')
    jsonInput = file.read()
    request_input = json.loads(jsonInput)
    schema = {
        "CustomerPhoneNumber" : {"type" : "string"}, 
        "DueDate" : {"type" : "string"}, 
        "Vehicle" : {"type" : "integer"}, 
        "VehicleDisplayName" : {"type" : "string"}, 
        "StartedAtId" : {"nullable" : True, "type" : "string"}, 
        "WarehouseDistanceInKilometers" : {"nullable" : True, "type" : "number"}, 
        "WarehouseETA" : {"nullable" : True, "type" : "string"}, 
        "WarehouseId" : {"type" : "string"}, 
        "WarehousePickUpAt" : {"nullable" : True, "type" : "string"}, 
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
        "StatusDisplayName" : {"type" : "string"}, 
        "CustomerAddress" : {"type" : "string"}, 
        "WarehouseAddress" : {"type" : "string"}
    }
    v = Validator(schema)
    assert v.validate(request_input)
    return request_input

def validateOutput(request_input) : 
    url = "http://localhost:7071/api/Deliveries"
    header = {
        "Authorization" : "Bearer GERG15TRBrgd24EFhgrvfTHG34RHtrntyj1n65yj2"
    }
    r = requests.post(url, json =request_input, headers=header)
    j = r.json()
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
        "StatusDisplayName" : {"type" : "string"}, 
        "Warehouse" : {"type" : "dict", "schema" : {
            "Id" : {"type" : "string"}, 
            "Latitude" : {"type" : "number"}, 
            "Longitude" : {"type" : "number"}, 
            "Address" : {"type" : "string"}, 
            "PostalCode" : {"type" : "string"}, 
            "Place" : {"type" : "string"}
        }}, 
        "Customer" : {"type" : "dict", "schema": {
            "Id" : {"type" : "string"}, 
            "Latitude" : {"type" : "number"}, 
            "Longitude" : {"type" : "number"}, 
            "Address" : {"type" : "string"}, 
            "PostalCode" : {"type" : "string"}, 
            "Place" : {"type" : "string"}
        }},
        "Current" : {"nullable" : True, "type" : "dict", "schema" : {
            "Id" : {"type" : "string"}, 
            "Latitude" : {"type" : "number"}, 
            "Longitude" : {"type" : "number"}, 
            "Address" : {"type" : "string"}, 
            "PostalCode" : {"type" : "string"}, 
            "Place" : {"type" : "string"}
        }}
    }
    v = Validator(schema)
    assert v.validate(j)

request_input = validateInput()
validateOutput(request_input)