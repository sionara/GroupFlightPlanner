using GroupFlightPlanner.Models.ViewModels;
using GroupFlightPlanner.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace GroupFlightPlanner.Controllers
{
    public class VolunteerController : Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// For proper WebAPI authentication, you can send a post request with login credentials to the WebAPI and log the access token from the response. The controller already knows this token, so we're just passing it up the chain.
        /// 
        /// Here is a descriptive article which walks through the process of setting up authorization/authentication directly.
        /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api
        /// </summary>
        private void GetApplicationCookie()
        {
            HttpClient client = new HttpClient() { };
            string token = "";
            //HTTP client is set up to be reused, otherwise it will exhaust server resources.
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }
        // GET: Volunteer/List
        // Objective: a webpage that lists the bikes in our system
        public ActionResult List(string SearchKey = null)
        {
            // get volunteer data through an Http request
            // GET {resource}/api/volunteerdata/listvolunteers
            //https://localhost:44380/api/volunteerdata/listvolunteers
            // use Http client to access the information

            HttpClient client = new HttpClient() { };
            //set the url
            string url = "https://localhost:44380/api/volunteerdata/listvolunteers";
            HttpResponseMessage response = client.GetAsync(url).Result;
            List<VolunteerDto> Volunteers = response.Content.ReadAsAsync<List<VolunteerDto>>().Result;

            foreach (VolunteerDto volunteer in Volunteers)
            {
                Debug.WriteLine("Received volunteers: " + volunteer.FirstName);
            }

            if (SearchKey != null)
            {
                var searchData = Volunteers.Where(v => v.FirstName.Contains(SearchKey)).ToList();
                if (searchData.Count == 0)
                {
                    ViewBag.Msg = "No result";
                    return View();

                }
                else
                {
                    return View(searchData);
                }
            }

            // Views/Volunteers/List.cshtml
            return View(Volunteers);
        }

        // GET: api/Volunteer/Details/5
        public ActionResult Details(int id)
        {
            // get a volunteer data through an Http request
            // GET {resource}/api/volunteerdata/findvolunteer
            //https://localhost:44380/api/volunteerdata/findvolunteer/{id}
            // use Http client to access the information

            HttpClient client = new HttpClient() { };
            //set the url
            string url = "https://localhost:44380/api/volunteerdata/findvolunteer/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;


            Debug.WriteLine("the response code is:");
            Debug.WriteLine(response.StatusCode);

            VolunteerDto selectedVolunteer = response.Content.ReadAsAsync<VolunteerDto>().Result;
            Debug.WriteLine("Received volunteers: ");
            Debug.WriteLine(selectedVolunteer.FirstName);

            return View(selectedVolunteer);
        }

        public ActionResult Error()
        {

            return View();
        }

        // GET: Volunteer/New
        [Authorize]
        public ActionResult New()
        {
            //information about all groups in the system.
            //GET api/groupdata/listgroups
            HttpClient client = new HttpClient() { };

            string url = "https://localhost:44380/api/groupdata/listgroups";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<GroupDto> GroupOptions = response.Content.ReadAsAsync<IEnumerable<GroupDto>>().Result;

            return View(GroupOptions);
        }

        // POST: Volunteer/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Volunteer Volunteer)
        {
            GetApplicationCookie();//get token credentials
            Debug.WriteLine("the json payload is :");
            Debug.WriteLine(Volunteer.FirstName);
            //objective: add a new volunteer into our system using the API
            //curl -H "Content-Type:application/json" -d @volunteer.json https://localhost:44380/api/volunteerdata/addvolunteer
            HttpClient client = new HttpClient() { };
            string url = "https://localhost:44380/api/volunteerdata/addvolunteer";


            string jsonpayload = jss.Serialize(Volunteer);
            Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Volunteer/Edit/5
        //[Authorize]
        public ActionResult Edit(int id)
        {
            UpdateVolunteer ViewModel = new UpdateVolunteer();

            //the existing volunteer information
            string url = "https://localhost:44380/api/volunteerdata/findvolunteer/" + id;

            HttpClient client = new HttpClient() { };

            HttpResponseMessage response = client.GetAsync(url).Result;

            VolunteerDto SelectedVolunteer = response.Content.ReadAsAsync<VolunteerDto>().Result;
            ViewModel.SelectedVolunteer = SelectedVolunteer;

            // all groups to choose from when updating this volunteer
            //the existing volunteer information

            url = "https://localhost:44380/api/groupdata/listgroups/";

            response = client.GetAsync(url).Result;

            IEnumerable<GroupDto> GroupOptions = response.Content.ReadAsAsync<IEnumerable<GroupDto>>().Result;

            ViewModel.GroupOptions = GroupOptions;

            return View(ViewModel);
        }

        // POST: Volunteer/Update/5
        [HttpPost]
        //[Authorize]
        public ActionResult Update(int id, Volunteer Volunteer)
        {
            GetApplicationCookie();//get token credentials   
            string url = "https://localhost:44380/api/volunteerdata/updatevolunteer/" + id;
            HttpClient client = new HttpClient() { };
            string jsonpayload = jss.Serialize(Volunteer);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Volunteer/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            // get a volunteer data through an Http request
            // GET {resource}/api/volunteerdata/findvolunteer
            //https://localhost:44380/api/volunteerdata/findvolunteer/{id}
            // use Http client to access the information

            HttpClient client = new HttpClient() { };
            string url = "https://localhost:44380/api/volunteerdata/findvolunteer/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            VolunteerDto selectedVolunteer = response.Content.ReadAsAsync<VolunteerDto>().Result;
            return View(selectedVolunteer);
        }

        // POST: Volunteer/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            HttpClient client = new HttpClient() { };

            string url = "https://localhost:44380/api/volunteerdata/deletevolunteer/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Volunteer/Delete/5
        /*[HttpPost]
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
        }*/
    }
}