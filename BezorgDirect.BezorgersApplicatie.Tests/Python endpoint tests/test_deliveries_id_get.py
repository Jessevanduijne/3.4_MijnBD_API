import requests
import json
from cerberus import Validator

def validateOutput() : 
    url = "http://localhost:7071/api/Deliveries/DA44020E-E516-47E1-9DC5-032F196D7746"
    header = {
        "Authorization" : "Bearer GERG15TRBrgd24EFhgrvfTHG34RHtrntyj1n65yj2"
    }
    r = requests.get(url, headers=header)
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

validateOutput()