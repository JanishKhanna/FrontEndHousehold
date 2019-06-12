using FrontEndHousehold.Models.Domain;
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

            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var categories = JsonConvert.DeserializeObject<List<Category>>(data);

                var viewModel = categories.Select(p => new ViewCategoryViewModel(p)).ToList();

                return View(viewModel);
            }
            else
            {
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View();
            }
        }
    }
}