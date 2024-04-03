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
    public class GroupController : Controller
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
        // GET: Group/List
        // Objective: a webpage that lists the groups of volunteers in our system
        public ActionResult List()
        {
            // get group data through an Http request
            // GET {resource}/api/groupdata/listgroups
            //https://localhost:44380/api/groupdata/listgroups
            // use Http client to access the information

            HttpClient client = new HttpClient();
            //set the url
            string url = "https://localhost:44380/api/groupdata/listgroups";
            HttpResponseMessage response = client.GetAsync(url).Result;
            List<GroupDto> Groups = response.Content.ReadAsAsync<List<GroupDto>>().Result;

            // Views/Volunteers/List.cshtml
            return View(Groups);
        }

        // GET: Group/Details/5
        public ActionResult Details(int id)
        {
            // get a group data through an Http request
            // GET {resource}/api/groupdata/findgroup
            //https://localhost:44380/api/groupdata/findgroup/{id}
            // use Http client to access the information
            HttpClient client = new HttpClient() { };
            DetailsGroup ViewModel = new DetailsGroup();
            //set the url
            string url = "https://localhost:44380/api//groupdata/findgroup/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;


            Debug.WriteLine("the response code is:");
            Debug.WriteLine(response.StatusCode);

            GroupDto SelectedGroup = response.Content.ReadAsAsync<GroupDto>().Result;
            Debug.WriteLine("Received volunteers: ");
            Debug.WriteLine(SelectedGroup.GroupName);
            ViewModel.SelectedGroup = SelectedGroup;



            // showcase information about volunteers related to this group
            // send a request to gather information about volunteers related to a particular group id

            url = "https://localhost:44380/api/volunteerdata/listvolunteersforgroups/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<VolunteerDto> RelatedVolunteers = response.Content.ReadAsAsync<IEnumerable<VolunteerDto>>().Result;
            ViewModel.RelatedVolunteers = RelatedVolunteers;


            // show all activities were joined in by this group
            url = "https://localhost:44380/api/activitydata/listactivitiesforgroup/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<ActivityDto> JoinedActivities = response.Content.ReadAsAsync<IEnumerable<ActivityDto>>().Result;
            ViewModel.JoinedActivities = JoinedActivities;

            // show all events were joined in by this group
            url = "https://localhost:44380/api/eventdata/listeventsforgroup/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<EventDto> JoinedEvents = response.Content.ReadAsAsync<IEnumerable<EventDto>>().Result;
            ViewModel.JoinedEvents = JoinedEvents;

            return View(ViewModel);
        }
        public ActionResult Error()
        {

            return View();
        }
        // GET: Group/New
        public ActionResult New()
        {
            return View();
        }


        // POST: Group/Create
        [HttpPost]
        public ActionResult Create(Group Group)
        {
            Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(Group.GroupName);
            //objective: add a new Group into our system using the API
            //curl -H "Content-Type:application/json" -d @group.json https://localhost:44380/api/groupdata/addgroup 

            HttpClient client = new HttpClient() { };
            string url = "https://localhost:44380/api/groupdata/addgroup";


            string jsonpayload = jss.Serialize(Group);
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

        // GET: Group/Edit/5
        public ActionResult Edit(int id)
        {
            HttpClient client = new HttpClient() { };
            string url = "https://localhost:44380/api/groupdata/findgroup/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            GroupDto selectedGroup = response.Content.ReadAsAsync<GroupDto>().Result;
            return View(selectedGroup);
        }

        // POST: Group/Edit/5
        [HttpPost]
        public ActionResult Update(int id, Group Group)
        {
            HttpClient client = new HttpClient() { };
            string url = "https://localhost:44380/api/groupdata/updategroup/" + id;
            string jsonpayload = jss.Serialize(Group);
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

        // GET: Group/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            HttpClient client = new HttpClient() { };
            string url = "https://localhost:44380/api/groupdata/findgroup/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            GroupDto selectedGroup = response.Content.ReadAsAsync<GroupDto>().Result;
            return View(selectedGroup);
        }

        // POST: Group/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            HttpClient client = new HttpClient() { };
            string url = "https://localhost:44380/api/groupdata/deletegroup/" + id;
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
    }
}