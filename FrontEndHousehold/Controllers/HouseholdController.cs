using FrontEndHousehold.Models.Domain;
using FrontEndHousehold.Models.ViewModel;
using FrontEndHousehold.Models.ViewModel.Households;
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
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/household-management/get-all-households";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var response = httpClient.GetAsync(url).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var households = JsonConvert.DeserializeObject<List<Household>>(data);

                var viewModel = households.Select(p => new ViewHouseholdViewModel()
                {
                    HouseholdId = p.HouseholdId,
                    Name = p.Name,
                    Description = p.Description,
                    DateCreated = p.DateCreated,
                    DateUpdated = p.DateUpdated,
                    NumberOfUsers = p.NumberOfUsers,
                    IsOwner = p.IsOwner,
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
        public ActionResult CreateHousehold()
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            return View();
        }

        [HttpPost]
        public ActionResult CreateHousehold(CreateHouseholdViewModel model)
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

            var url = $"http://localhost:62357/api/household-management/create";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var name = model.Name;
            var description = model.Description;

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("Name", name));
            parameters.Add(new KeyValuePair<string, string>("Description", description));

            var encodedParameters = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url, encodedParameters).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return RedirectToAction("ViewHousehold");
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
        [Route("EditHousehold/{id:int}")]
        public ActionResult EditHousehold(int id)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/household-management/by-id/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var response = httpClient.GetAsync(url).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<Household>(data);

                if (!result.IsOwner)
                {
                    return RedirectToAction("Index");
                }

                var viewModel = new EditHouseholdViewModel()
                {
                    Name = result.Name,
                    Description = result.Description
                };

                return View(viewModel);
            }
            else
            {
                return RedirectToAction("ViewHousehold");
            }
        }

        [HttpPost]
        [Route("EditHousehold/{id:int}")]
        public ActionResult EditHousehold(int id, EditHouseholdViewModel model)
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
                return RedirectToAction("ViewHousehold");
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
                return RedirectToAction("ViewHousehold");
            }
            else
            {
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View(model);
            }
        }

        //[HttpGet]
        //[Route("ViewUsers/{id}")]
        //public ActionResult ViewUsers(int id)
        //{
        //    var cookie = Request.Cookies["HouseholdCookie"];

        //    if (cookie == null)
        //    {
        //        return RedirectToAction(nameof(AccountController.Login), "Account");
        //    }

        //    var token = cookie.Value;

        //    var url = $"http://localhost:62357/api/household-management/by-id/{id}";

        //    var httpClient = new HttpClient();
        //    httpClient.DefaultRequestHeaders.Add("Authorization",
        //        $"Bearer {token}");

        //    var response = httpClient.GetAsync(url).Result;

        //    if(response.StatusCode == System.Net.HttpStatusCode.OK)
        //    {
        //        var data = response.Content.ReadAsStringAsync().Result;
        //        var result = JsonConvert.DeserializeObject<Household>(data);

        //        if (!result.IsOwner)
        //        {
        //            return RedirectToAction("ViewHousehold");
        //        }

        //        return View();
        //    }
        //    //var response = data.Content.ReadAsStringAsync().Result;

        //    //var result = JsonConvert.DeserializeObject<List<InviteUserViewModel>>(response);

        //    var viewModel = result.Select(p => new InviteUserViewModel()
        //    {
        //        Email = p?.Email
        //    }).ToList();

        //    return View(viewModel);
        //}

        [HttpGet]
        [Route("InviteUsers/{id}")]
        public ActionResult InviteUsers(int id)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/household-management/by-id/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var response = httpClient.GetAsync(url).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<Household>(data);

                if (!result.IsOwner)
                {
                    return RedirectToAction("ViewHousehold");
                }
            }

            return View();
        }

        [HttpPost]
        [Route("InviteUsers/{id}")]
        public ActionResult InviteUsers(int id, InviteUserViewModel model)
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
                return RedirectToAction("ViewHousehold");
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
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Sorry, An unexpected error has occured. Please try again later");
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Join()
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/household-management/get-invites";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var response = httpClient.GetAsync(url).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<List<InviteViewModel>>(data);

                return View(result);
            }
            else
            {
                return RedirectToAction("ViewHousehold");
            }
        }

        [HttpPost]
        [Route("Join/{id}")]
        public ActionResult Join(int id)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/household-management/join/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var response = httpClient.PostAsync(url, null).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("ViewHousehold");
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
                return RedirectToAction("ViewHousehold");
            }
            else
            {
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View();
            }
        }

        [HttpPost]
        [Route("LeaveHousehold/{id}")]
        public ActionResult LeaveHousehold(int id)
        {
            var cookie = Request.Cookies["HouseholdCookie"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            var token = cookie.Value;

            var url = $"http://localhost:62357/api/household-management/delete/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {token}");

            var response = httpClient.DeleteAsync(url).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("ViewHousehold");
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
                return RedirectToAction("ViewHousehold");
            }
            else
            {
                ModelState.AddModelError("", "Sorry. An unexpected error has occured. Please try again later");
                return View();
            }
        }
    }
}
