import requests
import json
from cerberus import Validator

def validateOutput () : 
    url = "http://localhost:7071/api/notification"
    header = {
   'Authorization': 'Bearer 997a139c203f4c2183cef6babc3ce469'}
    r = requests.get(url, headers=header)
    if (r.status_code == 200) :
        j = r.json()
        schema = {
        "Id" : {"type" : "string"}, 
        "AcceptedAt" : {"type" : "string"}, 
        "CreatedAt" : {"type" : "string"}, 
        "DelivererId" : {"type" : "string"}, 
        "DeliveryId" : {"type" : "string"}, 
        "ExpiredAt" : {"type" : "string"}, 
        "RefusedAt" : {"type" : "string"}, 
        "Status" : {"type" : "integer"}
        }
        v = Validator(schema)
        assert v.validate(j)
    assert r.status_code == 200|204

validateOutput()