// contactId is the id of the contact you are trying to get or patch
const contactId = "303353";
// authToken is a generated machine to machine token that is only valid for 24 hours (This one has already expired)
const authToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6IjVzT1I2MVBMdENFLUxuT1JIQTRxbyJ9.eyJodHRwOi8vcmh5dGhtc29mdHdhcmUuY29tL2N1c3RvbWVyX2lkIjoiaW5jb3NlLm9yZyIsImh0dHA6Ly9yaHl0aG1zb2Z0d2FyZS5jb20vdGVuYW50X2lkIjoiaW5jb3NlLm9yZyIsImlzcyI6Imh0dHBzOi8vaW5jb3NlLW9yZy51cy5hdXRoMC5jb20vIiwic3ViIjoiY1FGaEtSaVJocDJhZ0VERFA3WXpmbW1sZlBBdUR3Z0hAY2xpZW50cyIsImF1ZCI6Imh0dHBzOi8vYXBpLnJoeXRobXNvZnR3YXJlLmNvbSIsImlhdCI6MTY1MDM4NTgzNCwiZXhwIjoxNjUwNDcyMjM0LCJhenAiOiJjUUZoS1JpUmhwMmFnRUREUDdZemZtbWxmUEF1RHdnSCIsImd0eSI6ImNsaWVudC1jcmVkZW50aWFscyJ9.IRf32S97cwZxvYTRUP8K1GNoZ1J5Zttrordp9DGETaFpbuSl3M6J-JjyE-WbryTq4F8cDtLMBvxJYJrgpzvQ7Yb630Q5FvzbdPBHCkhCy2_6taRcqrDRLbtOfesnoeu1t0iNhMScAgQKb6u4rqyou8eR0KhEpUhEV19s5PPnFGnxTAnO3IsLOeaM5ZIXYLd7GZfjtbcjSoc7gJNVLwStFE8aekdkHDqAxw4_xFrrX027DOt51ZGXtAQts4mOdYWuKL-aQfaDSrKXTAiMvKrjHww_05px0SMh28I3GfHqeKeN8iCp4G0SCIsS41Qr2opveS4n7TR87GWRfOTomLIqfQ";
// tenantId is the id that is associated with our resources in Rhythm
const tenantId = "incose.org"

// If patchContact isn't defined yet, create the function to update the custom_field_values on the contact with the corresponding contactId
if (typeof patchContact == 'undefined') {
    patchContact = function (cfv, path="/custom_field_values", tenantId, contactId, authToken) {
        return new Promise(async (resolve, reject) => {
            const resp = await fetch(
                `https://rolodex.api.rhythmsoftware.com/contacts/${tenantId}/${contactId}`,
                {
                    method: 'PATCH',
                    headers: {
                        'Content-Type': 'application/json',
                        Authorization: `${authToken}`
                    },
                    body: JSON.stringify([
                        {
                            op: 'replace',
                            path: `${path}`,
                            value: `${cfv}`
                        }
                    ])
                }
            );

            const data = await resp.json();
            console.log(data);
            resolve(data);
        });
    }
}
if (typeof getContact == 'undefined') {
    getContact = function (query = "", tenantId = 'incose.org', contactId, authToken) {
        return new Promise(async (resolve, reject) => {
            const resp = await fetch(
                `https://rolodex.api.rhythmsoftware.com/contacts/${tenantId}/${contactId}?${query}`,
                {
                    method: 'GET',
                    headers: {
                        Authorization: `${authToken}`
                    }
                }
            );

            const data = await resp.text();
            console.log(data);
            resolve(data);
        });
    }
}

const query = new URLSearchParams({
    fields: 'first_name, last_name, email_address, home_address, facebook, notes, custom_field_values'
}).toString();

var customField = null;
var courseDirectory = null;
var courseDirectoryIndex = 0;
var programList = null;
var programListIndex = 0;
const customFieldPrefix = "rolodex-contacts__";
const dirId = "university_course_directory__c";
const listId = "university_program_list__c";
const fieldValueToDelete = "Anthropology";
getContact(query).then(
    (resp) => {
        if (resp) {
            customField = JSON.parse(resp)["custom_field_values"];
            for (let i = 0; i < customField.length; i++) {
                if (customField[i].custom_field_id === `${customFieldPrefix}${dirId}`) {
                    courseDirectoryIndex = i;
                } else if (customField[i].custom_field_id === `${customFieldPrefix}${listId}`) {
                    programListIndex = i;
                } else {
                    // Do nothing
                    continue;
                }
            }
            courseDirectory = JSON.parse(customField[courseDirectoryIndex].string_value);
            programList = JSON.parse(customField[programListIndex].string_value);
        }
        programList.forEach((program, index) => {
            if (program.programName === fieldValueToDelete) {
                if (!customField) {
                    return;
                }
                programList.splice(index, 1).toString();
                customField[programListIndex].string_value = programList;
                console.log(JSON.stringify(customField));
                patchContact(JSON.stringify(customField)).then((value) => {
                    //window.location.reload(true);
                });
            }
        });
    }
);

/*
 * "ProgramList": [
    {
      "programName": "Computer Science",
      "specializationArea": "Artificial Intelligence",
      "programType": "Bachelor's of Science",
      "totalLectureHours": 45,
      "courseList": [
        {
          "id": "CSCI 1410"
        },
        {
          "id": "CSCI 1411"
        },
        {
          "id": "CSCI 1510"
        },
        {
          "id": "CSCI 1511"
        },
        {
          "id": "CSCI 2412"
        }
      ]
    }
  ]
 * 
 * 
 *"custom_field_values" : [
 *  {...},
 *  {
 *      "custom_field_id": "rolodex-contacts__university_program_list__c",
 *      "string_value" : '"ProgramList": [{ "programName": "Computer Science", "specializationArea": "Artificial Intelligence", "programType": "Bachelor's of Science", "totalLectureHours": 45, "courseList": [{ "id": "CSCI 1410" }, { "id": "CSCI 1411" }, { "id": "CSCI 1510" }, { "id": "CSCI 1511" }, { "id": "CSCI 2412" }]}]'
 *  },
 *  {...}
 * ]
 * 
 * 
 * /