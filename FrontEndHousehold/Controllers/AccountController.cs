using FrontEndHousehold.Models.Domain;
using FrontEndHousehold.Models.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace FrontEndHousehold.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            var email = model.Email;
            var password = model.Password;
            var confirmPassword = model.ConfirmPassword;

            var url = "http://localhost:62357/api/Account/Register";

            var httpClient = new HttpClient();

            var parameters = new List<KeyValuePair<string, string>>();

            parameters
                .Add(new KeyValuePair<string, string>("Email", email));
            parameters
                .Add(new KeyValuePair<string, string>("Password", password));
            parameters
                .Add(new KeyValuePair<string, string>("ConfirmPassword", confirmPassword));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url, encodedParameters).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("Login");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ErrorViewModel>(data);

                return View();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return View("Error");
            }

            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            var url = "http://localhost:62357/token";

            var userName = model.Email;
            var password = model.Password;
            var grantType = "password";

            var httpClient = new HttpClient();

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("username", userName));
            parameters.Add(new KeyValuePair<string, string>("password", password));
            parameters.Add(new KeyValuePair<string, string>("grant_type", grantType));

            var encodedValues = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url, encodedValues).Result;

            var data = response.Content.ReadAsStringAsync().Result;

            var result = JsonConvert.DeserializeObject<LoginData>(data);

            var cookie = new HttpCookie("HouseholdCookie",
                result.AccessToken);

            Response.Cookies.Add(cookie);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}