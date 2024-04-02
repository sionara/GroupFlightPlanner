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
    public class ActivityController : Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
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

        // GET: Activity/List
        // Objective: a webpage that lists the Activities in our system
        [Authorize]
        public ActionResult List()
        {

            // get Activity data through an Http request
            // GET {resource}/api/activitydata/listactivities
            //https://localhost:44380/api/activitydata/listactivities

            // we need to prove who we are to access the activities resourse
            GetApplicationCookie();

            // use Http client to access the information

            HttpClient client = new HttpClient() { };
            //set the url
            // we will borrow the token from this request
            string url = "https://localhost:44380/api/activitydata/listactivities";
            HttpResponseMessage response = client.GetAsync(url).Result;
            List<ActivityDto> Activities = response.Content.ReadAsAsync<List<ActivityDto>>().Result;

            // Views/Volunteers/List.cshtml
            return View(Activities);
        }

        // GET: Activity/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {

            DetailsActivity ViewModel = new DetailsActivity();

            //objective: communicate with our activities data api to retrieve one activity
            //curl https://localhost:44380/api/activitydata/findactivity/{id}

            HttpClient client = new HttpClient() { };
            GetApplicationCookie();
            string url = "https://localhost:44380/api/activitydata/findactivity/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            ActivityDto SelectedActivity = response.Content.ReadAsAsync<ActivityDto>().Result;
            Debug.WriteLine("activity received : ");
            Debug.WriteLine(SelectedActivity.ActivityName);

            ViewModel.SelectedActivity = SelectedActivity;
            //show associated groups with this activity
            url = "https://localhost:44380/api/groupdata/listgroupsforactivity/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<GroupDto> ResponsibleGroups = response.Content.ReadAsAsync<IEnumerable<GroupDto>>().Result;
            ViewModel.ResponsibleGroups = ResponsibleGroups;


            url = "https://localhost:44380/api/groupdata/listgroupsnotjoininactivity/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<GroupDto> AvailableGroups = response.Content.ReadAsAsync<IEnumerable<GroupDto>>().Result;

            ViewModel.AvailableGroups = AvailableGroups;


            return View(ViewModel);
        }

        //POST: Activity/Associate/{activityid}
        [HttpPost]
        [Authorize]
        public ActionResult Associate(int id, int GroupId)
        {
            GetApplicationCookie();//get token credentials
            Debug.WriteLine("Attempting to associate activity :" + id + " with group " + GroupId);

            HttpClient client = new HttpClient() { };

            //call our api to associate animal with keeper
            string url = "https://localhost:44380/api/activitydata/associateactivitywithgroup/" + id + "/" + GroupId;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }


        //Get: Activity/UnAssociate/{id}?GroupId={groupId}
        [HttpGet]
        [Authorize]
        public ActionResult UnAssociate(int id, int GroupId)
        {
            GetApplicationCookie();//get token credentials
            Debug.WriteLine("Attempting to unassociate activity :" + id + " with group: " + GroupId);

            HttpClient client = new HttpClient() { };

            //call our api to associate activity with group
            string url = "https://localhost:44380/api/activitydata/unassociateactivitywithgroup/" + id + "/" + GroupId;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }


        public ActionResult Error()
        {

            return View();
        }

        // GET: Activity/New
        //[Authorize]
        public ActionResult New()
        {

            return View();
        }

        // POST: Activity/Create
        [HttpPost]
        //[Authorize]
        public ActionResult Create(Activity activity)
        {
            GetApplicationCookie();//get token credentials
            Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(activity.ActivityName);
            //objective: add a new activity into our system using the API
            //curl -H "Content-Type:application/json" -d @activity.json https://localhost:44380/api/activitydata/addactivity 
            string url = "https://localhost:44380/api/activitydata/addactivity";


            string jsonpayload = jss.Serialize(activity);
            Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpClient client = new HttpClient() { };

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
        // GET: Activity/Edit/5
        public ActionResult Edit(int id)
        {
            HttpClient client = new HttpClient() { };
            string url = "https://localhost:44380/api/activitydata/findactivity/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ActivityDto selectedActivity = response.Content.ReadAsAsync<ActivityDto>().Result;
            return View(selectedActivity);
        }

        // POST: Activity/Edit/5
        [HttpPost]
        public ActionResult Update(int id, Activity Activity)
        {
            HttpClient client = new HttpClient() { };
            string url = "https://localhost:44380/api/activitydata/updateactivity/" + id;
            string jsonpayload = jss.Serialize(Activity);
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
        // GET: Activity/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            HttpClient client = new HttpClient() { };
            string url = "https://localhost:44380/api/activitydata/findactivity/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ActivityDto selectedActivity = response.Content.ReadAsAsync<ActivityDto>().Result;
            return View(selectedActivity);
        }

        // POST: Activity/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            HttpClient client = new HttpClient() { };
            string url = "https://localhost:44380/api/activitydata/deleteactivity/" + id;
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