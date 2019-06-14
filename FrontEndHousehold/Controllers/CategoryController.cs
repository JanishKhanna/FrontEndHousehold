using FrontEndHousehold.Models.Domain;
using FrontEndHousehold.Models.ViewModel;
using FrontEndHousehold.Models.ViewModel.Categories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace FrontEndHousehold.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ViewCategories(int id)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/category-management/list-of-categories/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var response = httpClient.GetAsync(url).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var categories = JsonConvert.DeserializeObject<List<Category>>(data);
                ViewBag.id = id;
                var viewModel = categories.Select(p => new ViewCategoryViewModel(p)
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
        public ActionResult CreateCategory(int id)
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
        public ActionResult CreateCategory(int id, CreateEditCategoryViewModel model)
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

            var url = $"http://localhost:62357/api/category-management/create-category/{id}";

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
                var result = JsonConvert.DeserializeObject<Category>(data);
                return RedirectToAction("ViewCategories", new { id = result.HouseholdId });
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
        public ActionResult EditCategory(int id)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/category-management/get-by-id/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var response = httpClient.GetAsync(url).Result;
            var data = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<Category>(data);

                if (!result.IsOwner)
                {
                    return RedirectToAction(nameof(HouseholdController.Index), "Household");
                }

                var viewModel = new CreateEditCategoryViewModel()
                {
                    Name = result.Name,
                    Description = result.Description
                };

                return View(viewModel);
            }
            else
            {
                return RedirectToAction(nameof(HouseholdController.ViewHousehold), "Household");
            }
        }

        [HttpPost]
        public ActionResult EditCategory(int id, CreateEditCategoryViewModel model)
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

            var url = $"http://localhost:62357/api/category-management/edit-category/{id}";

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
                var result = JsonConvert.DeserializeObject<Category>(data);
                return RedirectToAction("ViewCategories", new { id = result.HouseholdId });
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
                return View("Error");
            }
            else
            {
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult DeleteCategory(int id)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/category-management/delete-category/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var response = httpClient.DeleteAsync(url).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //var data = response.Content.ReadAsStringAsync().Result;
                //var result = JsonConvert.DeserializeObject<Category>(data);
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
                TempData["Message"] = "It looks like this Category was deleted";
                return RedirectToAction(nameof(HouseholdController.ViewHousehold), "Household");
            }
            else
            {
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View();
            }
        }
    }
}