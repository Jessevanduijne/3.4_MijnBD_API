import requests
from cerberus import Validator

r = requests.get("https://inhollandbackend.azurewebsites.net/api/articles")
j = r.json()

schema = {
   "Results":{
      "type":"list",
      "schema":{
         "type":"dict",
         "schema":{
            "Id":{
               "type":"integer"
            },
            "Related":{
                "type": "list",
                "schema": {
                    "type": "string"
                }
            },
            "Categories":{
                "type": "list",
                "schema": {
                    "type": "dict",
                    "schema": {
                        "Id": {"type": "integer"},
                        "Name": {"type": "string"}
                    }
                }
            },
            "Feed":{
               "type":"integer"
            },
            "Title":{
               "type":"string"
            },
            "Summary":{
               "type":"string"
            },
            "PublishDate":{
               "type":"string"
            },
            "Image":{
               "type":"string"
            },
            "Url":{
               "type":"string"
            },
            "IsLiked":{
               "type":"boolean"
            }
         }
      }
   },
   "NextId":{
      "type":"integer"
   }
}

v = Validator(schema)
print(v.validate(j)
# print(v.validate(j[1]))