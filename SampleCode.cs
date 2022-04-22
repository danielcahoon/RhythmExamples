using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;

namespace SEEE_TEST.Controllers
{
    public class UniversityInventoryController : Controller
    {
        [HttpPost]
        public ActionResult Index(string courseId, Models.UniversityModel.SEEECourseModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            
            using (var client = new HttpClient())
            {
                Models.UniversityModel account = new Models.UniversityModel();
                var path = "/custom_field_values/0/string_value";

                // GET contact record for contactId
                client.DefaultRequestHeaders.Add("Authorization", "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6IjVzT1I2MVBMdENFLUxuT1JIQTRxbyJ9.eyJodHRwOi8vcmh5dGhtc29mdHdhcmUuY29tL2N1c3RvbWVyX2lkIjoiaW5jb3NlLm9yZyIsImh0dHA6Ly9yaHl0aG1zb2Z0d2FyZS5jb20vdGVuYW50X2lkIjoiaW5jb3NlLm9yZyIsImlzcyI6Imh0dHBzOi8vaW5jb3NlLW9yZy51cy5hdXRoMC5jb20vIiwic3ViIjoiY1FGaEtSaVJocDJhZ0VERFA3WXpmbW1sZlBBdUR3Z0hAY2xpZW50cyIsImF1ZCI6Imh0dHBzOi8vYXBpLnJoeXRobXNvZnR3YXJlLmNvbSIsImlhdCI6MTY1MDU1NjY1NSwiZXhwIjoxNjUwNjQzMDU1LCJhenAiOiJjUUZoS1JpUmhwMmFnRUREUDdZemZtbWxmUEF1RHdnSCIsImd0eSI6ImNsaWVudC1jcmVkZW50aWFscyJ9.p-oCk8FAqu0Qo9vtwe0rnD6cM3pDkP_OglanNyFu_xNF3xaEQyp_7rS9gAEOBqN6zgMX9MB1fx352EGzJiVcvBSOQZtEpVasU7gGiU4SvLkeAmdVvP0P8w7-hgUhtKd-8As55x-FMGlaeERoA1qccanxxHsL1ztei5bVseGIrT1q3zGbZhHEtTnzh4ttf5ezXGF7J8_5pKJSWWrjzlpSFaRu43GaAhlN2rrOUyc80CllX9vhjO-3-oRu7lz7OxbIYsPIZD09CnFKaD_wFdcJfw0uhfuyCyLdcMzJmSdGwsM9sUvGPWHwKO5nFcI0UZGqa4wo0tCSsoXummQyr4bJVA");
                var TenantId = "incose.org";
                var Id = "303353";
                var getReq = client.GetAsync("https://rolodex.api.rhythmsoftware.com/contacts/" + TenantId + "/" + Id + "?fields=custom_field_values").Result;
                var contact = getReq.Content.ReadAsStringAsync().Result;
                var cfv = JsonConvert.DeserializeObject<Models.UniversityModel.CustomFields>(contact);
                account.CourseDirectory = JsonConvert.DeserializeObject<Dictionary<string, Models.UniversityModel.SEEECourseModel>>(cfv.CustomFieldValues[2].StringValue);
                model.UpdateCompetenciesKey(courseId, model.CourseId);
                if (model.CourseId != courseId)
                {
                    account.CourseDirectory.Remove(courseId);
                    account.CourseDirectory.Add(model.CourseId, model);
                } else
                {
                    account.CourseDirectory[model.CourseId] = model;
                }
                account.SelectedProgram.CourseList.Append(new Models.UniversityModel.SEEECourseKey() { CourseId = model.CourseId });
                string jsonString = "[{ 'op': 'replace', 'path': '" + path + "', 'value': '" + JsonConvert.SerializeObject(account) + "'}]";
                var postData = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var postReq = new HttpRequestMessage(new HttpMethod("PATCH"), "https://rolodex.api.rhythmsoftware.com/contacts/" + TenantId + "/" + Id);
                postReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6IjVzT1I2MVBMdENFLUxuT1JIQTRxbyJ9.eyJodHRwOi8vcmh5dGhtc29mdHdhcmUuY29tL2N1c3RvbWVyX2lkIjoiaW5jb3NlLm9yZyIsImh0dHA6Ly9yaHl0aG1zb2Z0d2FyZS5jb20vdGVuYW50X2lkIjoiaW5jb3NlLm9yZyIsImlzcyI6Imh0dHBzOi8vaW5jb3NlLW9yZy51cy5hdXRoMC5jb20vIiwic3ViIjoiY1FGaEtSaVJocDJhZ0VERFA3WXpmbW1sZlBBdUR3Z0hAY2xpZW50cyIsImF1ZCI6Imh0dHBzOi8vYXBpLnJoeXRobXNvZnR3YXJlLmNvbSIsImlhdCI6MTY1MDU1NjY1NSwiZXhwIjoxNjUwNjQzMDU1LCJhenAiOiJjUUZoS1JpUmhwMmFnRUREUDdZemZtbWxmUEF1RHdnSCIsImd0eSI6ImNsaWVudC1jcmVkZW50aWFscyJ9.p-oCk8FAqu0Qo9vtwe0rnD6cM3pDkP_OglanNyFu_xNF3xaEQyp_7rS9gAEOBqN6zgMX9MB1fx352EGzJiVcvBSOQZtEpVasU7gGiU4SvLkeAmdVvP0P8w7-hgUhtKd-8As55x-FMGlaeERoA1qccanxxHsL1ztei5bVseGIrT1q3zGbZhHEtTnzh4ttf5ezXGF7J8_5pKJSWWrjzlpSFaRu43GaAhlN2rrOUyc80CllX9vhjO-3-oRu7lz7OxbIYsPIZD09CnFKaD_wFdcJfw0uhfuyCyLdcMzJmSdGwsM9sUvGPWHwKO5nFcI0UZGqa4wo0tCSsoXummQyr4bJVA");
                postReq.Content = postData;
                try
                {
                    _ = client.SendAsync(request: postReq).Result;
                    return RedirectToAction("Index", "UniversityVisualization");
                } catch
                {
                    return RedirectToAction("Index");
                }
            }
        }
    }
}