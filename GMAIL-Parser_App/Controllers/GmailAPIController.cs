using GMAIL_Parser_App.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml;

namespace GMAIL_Parser_App.Controllers
{
    public class GmailAPIController : Controller
    {
        // GET: GmailAPI
        public ActionResult Index()
        {
            return View();
        }

        #region GetCode from URL
        public ActionResult Code(string state, string code, string scope)
        {
            string GoogleWebAppClientID = WebConfigurationManager.AppSettings["GoogleWebAppClientID"];
            string GoogleWebAppClientSecret = WebConfigurationManager.AppSettings["GoogleWebAppClientSecret"];
            string RedirectUrl = WebConfigurationManager.AppSettings["RedirectUrl"];
            string Token = CreateOAuthTokenForGmail(code, GoogleWebAppClientID, GoogleWebAppClientSecret, RedirectUrl);
            Session["Token"] = Token;
            return RedirectToAction("DisplayEmail");
        }
        public string CreateOAuthTokenForGmail(string code, string GoogleWebAppClientID, string GoogleWebAppClientSecret, string RedirectUrl)
        {
            RequestParameters requestParameters = new RequestParameters()
            {
                code = code,
                client_id = WebConfigurationManager.AppSettings["GoogleWebAppClientID"],
                client_secret = WebConfigurationManager.AppSettings["GoogleWebAppClientSecret"],
                redirect_uri = WebConfigurationManager.AppSettings["RedirectUrl"],
                grant_type = "authorization_code"
            };
            string inputJson = JsonConvert.SerializeObject(requestParameters);
            string requestURI = "token";
            string ResponseString = "";
            HttpResponseMessage response;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://oauth2.googleapis.com");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                StringContent content = new StringContent(inputJson, Encoding.UTF8, "application/json");
                response = client.PostAsync(requestURI,content).Result;

                if (response.IsSuccessStatusCode)
                {
                    ResponseString = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result).ToString();
                    var result = JsonConvert.DeserializeObject<OAuthTokenViewModel>(ResponseString);
                    ResponseString = result.Access_token.ToString();
                }
            }
            return ResponseString;
        }
        #endregion

        #region Get Mail data for Mining
        public async Task<ActionResult> DisplayEmail()
        {
            HttpClient client = new HttpClient();
            Root rootObj = new Root();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: Session["Token"].ToString());
            HttpResponseMessage responseMessage = await client.GetAsync("https://mail.google.com/mail/feed/atom");
            if (responseMessage.IsSuccessStatusCode)
            {
                var data = responseMessage.Content;
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(@responseData);
                var json = JsonConvert.SerializeXmlNode(doc);
                rootObj = JsonConvert.DeserializeObject<Root>(json);
            }
            return View(rootObj);
        }
        #endregion
    }
}