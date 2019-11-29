import requests
import json
from cerberus import Validator

def validateOutput() : 
    url = "http://localhost:7071/api/Delivery"
    header = {
   'Authorization': 'Bearer 997a139c203f4c2183cef6babc3ce469'}
    r = requests.get(url, headers=header)
    if (r.status_code == 200) :
        j = r.json()
        schema = {
        "Id" : {"type" : "string"}, 
        "CurrentId" : {"type" : "dict", "schema" : {
            "Id" : {"type" : "string"}, 
            "Address" : {"type" : "string"}, 
            "IsWarehouse" : {"type" : "boolean"}, 
            "Latitude" : {"type" : "number"}, 
            "Longitude" : {"type" : "number"}, 
            "Place" : {"type" : "string"}, 
            "PostalCode" : {"type" : "string"}
        }}, 
        "CustomerDistanceInKilometers" : {"type" : "number"}, 
        "CustomerETA" : {"type" : "string"}, 
        "CustomerId" : {"type" : "dict", "schema" : {
            "Id" : {"type" : "string"}, 
            "Address" : {"type" : "string"}, 
            "IsWarehouse" : {"type" : "boolean"}, 
            "Latitude" : {"type" : "number"}, 
            "Longitude" : {"type" : "number"}, 
            "Place" : {"type" : "string"}, 
            "PostalCode" : {"type" : "string"}
        }}, 
        "CustomerPhoneNumber" : {"type" : "string"}, 
        "DeliveredAt" : {"type" : "string"}, 
        "DelivererId" : {"type" : "string"}, 
        "DueDate" : {"type" : "string"}, 
        "PaymentMethod" : {"type" : "integer"}, 
        "Price" : {"type" : "number"}, 
        "StartedAtId" : {"type" : "dict", "schema" : {
            "Id" : {"type" : "string"}, 
            "Address" : {"type" : "string"}, 
            "IsWarehouse" : {"type" : "boolean"}, 
            "Latitude" : {"type" : "number"}, 
            "Longitude" : {"type" : "number"}, 
            "Place" : {"type" : "string"}, 
            "PostalCode" : {"type" : "string"}
        }}, 
        "Status" : {"type" : "integer"}, 
        "Tip" : {"type" : "number"}, 
        "Vehicle" : {"type" : "integer"}, 
        "WarehouseDistanceInKilometers" : {"type" : "number"}, 
        "WarehouseETA" : {"type" : "string"}, 
        "WarehouseId" : {"type" : "dict", "schema" : {
            "Id" : {"type" : "string"}, 
            "Address" : {"type" : "string"}, 
            "IsWarehouse" : {"type" : "boolean"}, 
            "Latitude" : {"type" : "number"}, 
            "Longitude" : {"type" : "number"}, 
            "Place" : {"type" : "string"}, 
            "PostalCode" : {"type" : "string"}
        }}, 
        "WarehousePickUpAt" : {"type" : "string"}
        }
        v = Validator(schema)
        assert v.validate(j)
    assert r.status_code == 200|204

validateOutput()