using FrontEndHousehold.Models.Domain;
using FrontEndHousehold.Models.ViewModel;
using FrontEndHousehold.Models.ViewModel.BankAccounts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace FrontEndHousehold.Controllers
{
    public class BankAccountController : Controller
    {
        // GET: BankAccount
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ViewBankAccounts(int id)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/bank-account/list-of-accounts/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var response = httpClient.GetAsync(url).Result;


            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var accounts = JsonConvert.DeserializeObject<List<BankAccount>>(data);
                ViewBag.id = id;
                var viewModel = accounts.Select(p => new ViewBankAccountViewModel(p)
                {
                    IsOwner = p.IsOwner
                }).ToList();

                return View(viewModel);
            }
            else
            {
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View();
            }
        }

        [HttpGet]
        public ActionResult CreateAccount(int id)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            ViewBag.id = id;
            return View();
        }

        [HttpPost]
        public ActionResult CreateAccount(int id, CreateEditBankAccountViewModel model)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(nameof(model), "Invalid Form Data");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/bank-account/create-bankaccount/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("Name", model.Name));
            parameters.Add(new KeyValuePair<string, string>("Description", model.Description));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url, encodedParameters).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<BankAccount>(data);
                return RedirectToAction("ViewBankAccounts", new { id = result.HouseholdId});
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ErrorViewModel>(data);

                foreach (var key in result.ModelState)
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
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult EditAccount(int id)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/bank-account/account-by-id/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var response = httpClient.GetAsync(url).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<BankAccount>(data);

                if (!result.IsOwner)
                {
                    return RedirectToAction(nameof(HouseholdController.Index), "Household");
                }

                var viewModel = new CreateEditBankAccountViewModel()
                {
                    Name = result.Name,
                    Description = result.Description
                };

                return View(viewModel);
            }
            else
            {
                return RedirectToAction("ViewBankAccounts");
            }
        }

        [HttpPost]
        public ActionResult EditAccount(int id, CreateEditBankAccountViewModel model)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/bank-account/edit-account/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("Name", model.Name));
            parameters.Add(new KeyValuePair<string, string>("Description", model.Description));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var response = httpClient.PutAsync(url, encodedParameters).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<BankAccount>(data);
                return RedirectToAction("ViewBankAccounts", new { id = result.HouseholdId });
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ErrorViewModel>(data);

                foreach (var key in result.ModelState)
                {
                    foreach (var error in key.Value)
                    {
                        ModelState.AddModelError(key.Key, error);
                    }
                }

                return View(model);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return RedirectToAction("ViewBankAccounts");
            }
            else
            {
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult DeleteAccount(int id)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/bank-account/delete-account/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var response = httpClient.DeleteAsync(url).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //var data = response.Content.ReadAsStringAsync().Result;
                //var result = JsonConvert.DeserializeObject<BankAccount>(data);
                return RedirectToAction(nameof(HouseholdController.ViewHousehold), "Household");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ErrorViewModel>(data);

                foreach (var key in result.ModelState)
                {
                    foreach (var error in key.Value)
                    {
                        ModelState.AddModelError(key.Key, error);
                    }
                }

                return View();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData["Message"] = "It looks like this household was deleted";
                return RedirectToAction("ViewBankAccounts");
            }
            else
            {
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View();
            }
        }
    }
}