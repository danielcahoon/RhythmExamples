# This code will get a the custom field values for a contact from Rhythm that has an id of 303353
# then prints out all the values that were received back from Rhythm.

import requests
import json

# gets the custom_field_values for a contact
def getContact(id="303353", authToken="eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6IjVzT1I2MVBMdENFLUxuT1JIQTRxbyJ9.eyJodHRwOi8vcmh5dGhtc29mdHdhcmUuY29tL2N1c3RvbWVyX2lkIjoiaW5jb3NlLm9yZyIsImh0dHA6Ly9yaHl0aG1zb2Z0d2FyZS5jb20vdGVuYW50X2lkIjoiaW5jb3NlLm9yZyIsImlzcyI6Imh0dHBzOi8vaW5jb3NlLW9yZy51cy5hdXRoMC5jb20vIiwic3ViIjoiY1FGaEtSaVJocDJhZ0VERFA3WXpmbW1sZlBBdUR3Z0hAY2xpZW50cyIsImF1ZCI6Imh0dHBzOi8vYXBpLnJoeXRobXNvZnR3YXJlLmNvbSIsImlhdCI6MTY1MDU1NjY1NSwiZXhwIjoxNjUwNjQzMDU1LCJhenAiOiJjUUZoS1JpUmhwMmFnRUREUDdZemZtbWxmUEF1RHdnSCIsImd0eSI6ImNsaWVudC1jcmVkZW50aWFscyJ9.p-oCk8FAqu0Qo9vtwe0rnD6cM3pDkP_OglanNyFu_xNF3xaEQyp_7rS9gAEOBqN6zgMX9MB1fx352EGzJiVcvBSOQZtEpVasU7gGiU4SvLkeAmdVvP0P8w7-hgUhtKd-8As55x-FMGlaeERoA1qccanxxHsL1ztei5bVseGIrT1q3zGbZhHEtTnzh4ttf5ezXGF7J8_5pKJSWWrjzlpSFaRu43GaAhlN2rrOUyc80CllX9vhjO-3-oRu7lz7OxbIYsPIZD09CnFKaD_wFdcJfw0uhfuyCyLdcMzJmSdGwsM9sUvGPWHwKO5nFcI0UZGqa4wo0tCSsoXummQyr4bJVA"):
    tenant_id = "incose.org"
    url = "https://rolodex.api.rhythmsoftware.com/contacts/" + tenant_id + "/" + id

    query = {
    "fields": "custom_field_values",
    }

    headers = {"Authorization": authToken}

    response = requests.get(url, headers=headers, params=query)

    data = response.json()
    print(data)
    return data


# Each custom field value I store JSON in 'string_value' which can later be  
# deserialized into an object.
#
# {
#     "custom_field_values" : [
#         {
#             "custom_field_id" : "rolodex-contacts__seee_competency_questionnaire_results__c",
#             "string_value": '{ ... }'
#         },
#         { ... },
#         { ... }
#     ]
# }
custom_field_values = getContact()
i = 0
for custom_field in custom_field_values["custom_field_values"]:
    custom_field_id = custom_field["custom_field_id"]
    string_value = custom_field["string_value"]
    print(custom_field_id)
    print(string_value)
    print("index = ", i)
    i += 1