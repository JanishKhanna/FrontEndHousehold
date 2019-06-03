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
        
        [HttpGet]
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
                HouseholdId = p.HouseholdId,
                Name = p.Name,
                Description = p.Description,
                DateCreated = p.DateCreated,
                DateUpdated = p.DateUpdated
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
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(nameof(model), "Invalid Form Data");
            }

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

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url, encodedParameters).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<Household>(data);
                return View("ViewHousehold");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ErrorViewModel>(data);

                var error = result.ModelState;

                return View(error);
            }

            return View(nameof(HomeController.Index), "Home");

        }

        [HttpGet]
        [Route("EditHousehold/{id:int}")]
        public ActionResult EditHousehold(int id)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/household-management/by-id/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var data = httpClient.GetStringAsync(url).Result;

            var result = JsonConvert.DeserializeObject<Household>(data);

            var viewModel = new EditHouseholdViewModel()
            {
                Name = result.Name,
                Description = result.Description
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("EditHousehold/{id:int}")]
        public ActionResult EditHousehold(int id, EditHouseholdViewModel model)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/household-management/edit/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var name = model.Name;
            var description = model.Description;

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("Name", name));
            parameters.Add(new KeyValuePair<string, string>("Description", description));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var response = httpClient.PutAsync(url, encodedParameters).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<Household>(data);
                return View("Index");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ErrorViewModel>(data);

                var error = result.ModelState;

                return View(error);
            }
            else if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return View("Household Not Found");
            }

            return View(nameof(HouseholdController.Index), "Household");

        }

        [HttpGet]
        [Route("ViewUsers/{id}")]
        public ActionResult ViewUsers(int id)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/household-management/users-joined-to-household/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var data = httpClient.GetAsync(url).Result;
            var response = data.Content.ReadAsStringAsync().Result;

            var result = JsonConvert.DeserializeObject<List<UserViewModel>>(response);

            var viewModel = result.Select(p => new UserViewModel()
            {
                Email = p?.Email
            }).ToList();

            return View(viewModel);
        } 
        
        [HttpGet]
        [Route("InviteUsers/{id}")]
        public ActionResult InviteUsers(int id)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/household-management/by-id/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var data = httpClient.GetStringAsync(url).Result;

            var result = JsonConvert.DeserializeObject<Household>(data);            

            return View();
        }

        [HttpPost]
        [Route("InviteUsers/{id}")]
        public ActionResult InviteUsers(int id, UserViewModel model)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/household-management/invite/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var email = model.Email;

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("Email", email));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url, encodedParameters).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                //var result = JsonConvert.DeserializeObject<UserViewModel>(data);
                return View("Index");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ErrorViewModel>(data);

                var error = result.ModelState;

                return View(error);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return View("User Not Found");
            }

            return View(nameof(HouseholdController.Index), "Household");
        }

        [HttpPost]
        [Route("Join/{id}")]
        public ActionResult Join(int id, UserViewModel model)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/household-management/join/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var email = model.Email;

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("Email", email));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url, encodedParameters).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                return View("Index");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ErrorViewModel>(data);

                var error = result.ModelState;

                return View(error);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return View("User Not Found");
            }

            return View(nameof(HouseholdController.Index), "Household");
        }

        [HttpPost]
        [Route("LeaveHousehold/{id}")]
        public ActionResult LeaveHousehold(int id)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/household-management/delete/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var response = httpClient.DeleteAsync(url).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                return View("Index");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ErrorViewModel>(data);

                var error = result.ModelState;

                return View(error);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return View("User Not Found");
            }

            return View(nameof(HouseholdController.Index), "Household");
        }
    }
}
