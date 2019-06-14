using FrontEndHousehold.Models.Domain;
using FrontEndHousehold.Models.ViewModel;
using FrontEndHousehold.Models.ViewModel.Transactions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace FrontEndHousehold.Controllers
{
    public class TransactionController : Controller
    {
        // GET: Transaction
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ViewTransactions(int id)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/transaction/list-of-transactions/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var response = httpClient.GetAsync(url).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var transactions = JsonConvert.DeserializeObject<List<Transaction>>(data);
                ViewBag.id = id;
                var viewModel = transactions.Select(p => new ViewTransactionViewModel(p)).ToList();

                return View(viewModel);
            }
            else
            {
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View();
            }
        }

        [HttpGet]
        public ActionResult CreateTransaction(int id)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            var token = cookie.Value;
            var url = $"http://localhost:62357/api/category-management/get-categories/{id}";
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var response = httpClient.GetAsync(url).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var categories = JsonConvert.DeserializeObject<List<Category>>(data);
                ViewBag.id = id;
                var viewModel = new CreateTransactionViewModel()
                {
                    Categories = new SelectList(categories, nameof(Category.CategoryId), nameof(Category.Name))
                };

                return View(viewModel);
            }
            else
            {
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult CreateTransaction(int id, CreateTransactionViewModel model)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(nameof(model), "Invalid Form Data");
                return View(model);
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/transaction/create-transaction/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("Title", model.Title));
            parameters.Add(new KeyValuePair<string, string>("Description", model.Description));
            parameters.Add(new KeyValuePair<string, string>("DateOfTransaction", model.DateOfTransaction.ToString()));
            parameters.Add(new KeyValuePair<string, string>("Amount", model.Amount.ToString()));
            parameters.Add(new KeyValuePair<string, string>("CategoryId", model.CategoryId.ToString()));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url, encodedParameters).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<Transaction>(data);
                return RedirectToAction("ViewTransactions", new { id = result.BankAccountId });
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
        public ActionResult EditTransaction(int id)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/transaction/transaction-by-id/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var response = httpClient.GetAsync(url).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<Transaction>(data);

                if (!result.IsOwner)
                {
                    return RedirectToAction(nameof(HouseholdController.Index), "Household");
                }

                var viewModel = new EditTransactionViewModel()
                {
                    Title = result.Title,
                    Description = result.Description,
                    DateOfTransaction = result.DateOfTransaction,
                    Amount = result.Amount,
                };

                return View(viewModel);
            }
            else
            {
                return RedirectToAction("ViewBankAccounts");
            }
        }

        [HttpPost]
        public ActionResult EditTransaction(int id, EditTransactionViewModel model)
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

            var url = $"http://localhost:62357/api/transaction/edit-transaction/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("Title", model.Title));
            parameters.Add(new KeyValuePair<string, string>("Description", model.Description));
            parameters.Add(new KeyValuePair<string, string>("DateOfTransaction", model.DateOfTransaction.ToString()));
            parameters.Add(new KeyValuePair<string, string>("Amount", model.Amount.ToString()));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var response = httpClient.PutAsync(url, encodedParameters).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<Transaction>(data);
                return RedirectToAction("ViewTransactions", new { id = result.BankAccountId });
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
                return RedirectToAction("ViewTransactions");
            }
            else
            {
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult DeleteTransaction(int id)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/transaction/delete-transaction/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var response = httpClient.DeleteAsync(url).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
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
                TempData["Message"] = "It looks like this Transaction was deleted";
                return RedirectToAction(nameof(HouseholdController.ViewHousehold), "Household");
            }
            else
            {
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View();
            }
        }

        [HttpGet]
        public ActionResult Void(int id)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/transaction/void-transaction/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var response = httpClient.PutAsync(url, null).Result;

            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction(nameof(HouseholdController.ViewHousehold), "Household");
            }
            else
            {
                return View("Error");
            }

        }
    }
}