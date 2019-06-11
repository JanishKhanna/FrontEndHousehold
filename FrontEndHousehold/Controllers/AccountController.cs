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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

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
                TempData["Message"] = "Your account has been created successfully!";
                return RedirectToAction("Login");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var errors = JsonConvert.DeserializeObject<ErrorViewModel>(data);

                foreach (var key in errors.ModelState)
                {
                    foreach (var error in key.Value)
                    {
                        ModelState.AddModelError(key.Key, error);
                    }
                }

                return View(model);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return View("Sorry. An unexpected error has occured. Please try again later.");
            }

            return View("Error");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

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

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<LoginData>(data);

                var cookie = new HttpCookie("HouseholdCookie",
                    result.AccessToken);

                Response.Cookies.Add(cookie);

                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var error = JsonConvert.DeserializeObject<LoginError>(data);

                ModelState.AddModelError("", error.ErrorDescription);

                return View(model);
            }
            else
            {
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction("Login");
            }

            var token = cookie.Value;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var oldPassword = model.OldPassword;
            var newPassword = model.NewPassword;
            var confirmPassword = model.ConfirmPassword;

            var url = "http://localhost:62357/api/Account/ChangePassword";

            var httpClient = new HttpClient();

            var parameters = new List<KeyValuePair<string, string>>();

            parameters
                .Add(new KeyValuePair<string, string>("OldPassword", oldPassword));
            parameters
                .Add(new KeyValuePair<string, string>("NewPassword", newPassword));
            parameters
                .Add(new KeyValuePair<string, string>("ConfirmPassword", confirmPassword));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url, encodedParameters).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ViewBag.Message = "Password has been changed succesfully";
                return View();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                var errors = JsonConvert.DeserializeObject<ErrorViewModel>(data);

                foreach (var key in errors.ModelState)
                {
                    foreach (var error in key.Value)
                    {
                        ModelState.AddModelError(key.Key, error);
                    }
                }

                return View(model);
            }
            else
            {
                ModelState.AddModelError("", "Sorry, An unexpected error has occured. Please try again later");
                return View(model);
            }
        }
    }
}
