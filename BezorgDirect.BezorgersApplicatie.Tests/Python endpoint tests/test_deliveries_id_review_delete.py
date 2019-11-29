import requests
import json
from cerberus import Validator

def validateOutput() : 
    url = "http://localhost:7071/api/Deliveries/DA44020E-E516-47E1-9DC5-032F196D7746/Feedback"
    header = {
    "Authorization" : "Bearer GERG15TRBrgd24EFhgrvfTHG34RHtrntyj1n65yj2"
    }
    r = requests.delete(url, headers= header)
    assert r.status_code == 200, "De status code is geen 200!"

validateOutput()