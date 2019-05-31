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
    public class HouseholdController : Controller
    {
        // GET: Household
        public ActionResult Index()
        {
            return View();
        }

        // GET: Household/Details/5
        public ActionResult ViewHousehold()
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/household-management/get-all-households";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var data = httpClient.GetStringAsync(url).Result;

            var result = JsonConvert.DeserializeObject<List<Household>>(data);

            var viewModel = result.Select(p => new ViewHouseholdViewModel()
            {
                Name = p.Name,
                Description = p.Description,
                DateCreated = p.DateCreated
            }).ToList();

            return View(viewModel);
        }

        [HttpGet]        
        public ActionResult CreateHousehold()
        {
            return View();
        }

        
       [HttpPost]
        public ActionResult CreateHousehold(CreateHouseholdViewModel model)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/household-management/create";            

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var name = model.Name;
            var description = model.Description;
            var dateCreated = DateTime.Now;

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("Name", name));
            parameters.Add(new KeyValuePair<string, string>("Description", description));
            //parameters.Add(new ConsoleSpecialKey("DateCreated", dateCreated));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url, encodedParameters).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<Household>(data);
                return View("ViewHousehold");
            }  
            else if(response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ErrorViewModel>(data);

                var error = result.ModelState;

                return View(error);
            }

            return View(nameof(HomeController.Index), "Home");

        }

        // GET: Household/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }



        // POST: Household/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Household/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Household/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
