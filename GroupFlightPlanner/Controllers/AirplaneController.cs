using GroupFlightPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Diagnostics;
using GroupFlightPlanner.Models.ViewModels;

namespace GroupFlightPlanner.Controllers
{
    public class AirplaneController : Controller
    {

        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static AirplaneController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };

            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44380/api/");
        }


        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// For proper WebAPI authentication, you can send a post request with login credentials to the WebAPI and log the access token from the response. The controller already knows this token, so we're just passing it up the chain.
        /// 
        /// Here is a descriptive article which walks through the process of setting up authorization/authentication directly.
        /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api
        /// </summary>
        private void GetApplicationCookie()
        {
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


        /// <summary>
        /// 1. GET: Airplane/List
        /// This GET method is responsible for making the call to the airplane API, in which it will collect the list of airplanes and provide the collected information to the View.
        /// This code is responsible for utilizing the client.BaseAddress and calling the ListAirplanes method
        /// Go to  -> /Views/Airplane/List.cshtml
        /// 
        /// 2. GET: Airplane/List?AirplaneSearch=airbus
        /// this will happen when the user provides a searck key in the form, this will display all the Airplanes that contains the AirplaneSearch = airbus
        /// Go to  -> /Views/Airplane/List.cshtml
        /// </summary>
        /// <param name="AirplaneSearch">This parameter is String type and it will holde the value given ny the user, in case that the user doesn not provide a value it will be null and it will return all the list of Airplanes in the system
        /// If the user provide a value. it will provide to the view all the Airplanes that contains the AirplaneSearch parameter</param>
        /// <returns>
        /// Returns the List View, which will display a list of the airplanes in the system. Each of the airplanes in the database will be of the datatype AirplaneDto.
        /// 
        /// Additionally, if the AirplaneSearch != null it will display all the Airplanes that contains the AirplaneSearch
        /// </returns>
        public ActionResult List(string AirplaneSearch = null)
        {
            //communicate with the airplane data api to retrieve a list of airplanes
            //curl https://localhost:44380/api/AirplaneData/ListAirplanes

            AirplaneList ViewModel = new AirplaneList();
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin")) ViewModel.IsAdmin = true;
            else ViewModel.IsAdmin = false;

            string url = "AirplaneData/ListAirplanes/" + AirplaneSearch;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine(url);

            IEnumerable<AirplaneDto> airplanes = response.Content.ReadAsAsync<IEnumerable<AirplaneDto>>().Result;

            ViewModel.Airplanes = airplanes;

            return View(ViewModel);
        }

        /// <summary>
        /// GET: Airplane/Details/{id}
        /// This GET method will be responsible for calling the FindAirplane method from the airplane API, as well as the Related method in order to show the flighst of a particular airplane FlightData/ListFlightsForAirplane/ + id.
        /// This method will return a ViewModel that holds the SelectedAirplane and RelatedFlights (Related method)
        /// RelatedFlights => List of flights for a specific Airplane
        /// Go to  -> /Views/Airplane/Details.cshtml
        /// </summary>
        /// <param name="id">This is an int datatype parameter of the airplane you want to find.</param>
        /// <returns>
        /// It will return a ViewModel of type DetailsAirplane this viewmodel will allow the view to acces to the SelectedAirplane (airplane found by the ID given in the URL) and RelatedFlights
        /// </returns>
        public ActionResult Details(int id)
        {
            //communicate with the airplane data api to retrieve one airplane
            //curl https://localhost:44380/api/AirplaneData/FindAirplane/{id}

            //instance of ViewModel
            DetailsAirplane ViewModel = new DetailsAirplane();

            if (User.Identity.IsAuthenticated && User.IsInRole("Admin")) ViewModel.IsAdmin = true;
            else ViewModel.IsAdmin = false;

            string url = "AirplaneData/FindAirplane/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            AirplaneDto SelectedAirplane = response.Content.ReadAsAsync<AirplaneDto>().Result;
            ViewModel.SelectedAirplane = SelectedAirplane;


            //showcase information about Flights related to this Airplane -> ListFlightsForAirplane
            //send a request to gather information about Flights related to a particular ID 
            url = "FlightData/ListFlightsForAirplane/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<FlightDto> RelatedFlights = response.Content.ReadAsAsync<IEnumerable<FlightDto>>().Result;
            ViewModel.RelatedFlights = RelatedFlights;

            return View(ViewModel);
        }

        /// <summary>
        /// GET: Airplane/New
        /// GET method to add a new airplane to the system, responsible for providing the view of the form for inserting a new airplane.
        /// Go to  -> /Views/Airplane/New.cshtml
        /// </summary>
        /// <returns>
        /// Returns the view of the form so that the user can insert a new airplane.
        /// </returns>
        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            return View();
        }


        /// <summary>
        /// POST: Airplane/Create
        /// This POST method will be in charge of receiving the information sent by the new form, once the information is received 
        /// the method will be in charge of processing the conversion of the Airplane object to json in order to be sent in the body of the HTTP REQUEST
        /// Additionally, it is indicated that its content is of type json in the request header. Once this is done, 
        /// a POST request will be sent to the specified Uri as an asynchronous operation. If its IsSuccessStatusCode is true, 
        /// it will redirect the user to the list airplanes page, otherwise it will indicate to the user that there is an error.
        /// Go to (if success) -> /Views/Airplane/List.cshtml
        /// Go to (if not success) -> /Views/Airplane/Error.cshtml
        /// </summary>
        /// <param name="airplane">This parameter represents the object received by the form for creating a new airplane.</param>
        /// <returns>
        /// Returns the user to either the List View of the airplanes or the Error View, depending on the response StatusCode
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Airplane airplane)
        {
            GetApplicationCookie();//get token credentials
            //objective: add a new airplane into the system using the API
            //curl -d @airplane.json -H "Content-type: application/json" https://localhost:44380/api/AirplaneData/AddAirplane 
            string url = "AirplaneData/AddAirplane";

            string jsonpayload = jss.Serialize(airplane);

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

        /// <summary>
        /// GET: Airplane/Edit/{id}
        /// This GET method is in charge of collecting the information of the airplane and send it to the view to prepoluate the form with the airplane information that is requested by its id, 
        /// for this the api/AirplaneData/FindAirplane/{id} is used. Once the call to the API is made, the information collected of the datatype AirplaneDto will be sent to the view. 
        /// In this way the form will be populated with the information of the airplane
        /// Go to  -> /Views/Airplane/Edit.cshtml
        /// </summary>
        /// <param name="id">This is an int datatype parameter of the AirplaneId providaded by the url that will be displayed in the form in order to make an update</param>
        /// <returns>
        /// Returns the view with the form filled with the information of the airplane to update
        /// </returns>
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            string url = "AirplaneData/FindAirplane/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            AirplaneDto selectedairplane = response.Content.ReadAsAsync<AirplaneDto>().Result;

            return View(selectedairplane);
        }

        /// <summary>
        /// POST: Airplane/Update/{id}
        /// This POST method is responsible for making the call to the UpdateAirplane method of the airplane api. The information collected by the form will be sent in the body of the request
        /// Go to after updating (if success) -> /Views/Airplane/Details/{id}.cshtml
        /// Go to (if not success) -> /Views/Airplane/Error.cshtml
        /// </summary>
        /// <param name="id">This is the parameter provided by the url that identifies the AirplaneId that is going to be updated</param>
        /// <param name="airplane">The airplane object, this parameter holds the new data, this new data will be sent as a body to the UpdateAirplane method of the Airplane API</param>
        /// <returns>
        /// If the update is satisfactory the user will be redirected to the airplane list, otherwise it will be sent to the error page
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id, Airplane airplane)
        {
            GetApplicationCookie();//get token credentials
            //serialize into JSON
            //Send the request to the API
            string url = "AirplaneData/UpdateAirplane/" + id;

            string jsonpayload = jss.Serialize(airplane);
            Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details/" + id);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// GET: Airplane/DeleteConfirm/{id}
        /// This is a GET method that is responsible for finding the information of the airplane to delete, this is done through its airplane id which is provided by the id of the url
        /// Go to  -> /Views/Airplane/DeleteConfirm.cshtml
        /// </summary>
        /// <param name="id">This is an int datatype parameter of the airplane that will be displayed in DeleteConfirm View in order to delete it</param>
        /// <returns>
        /// Returns a view that provides information about the airplane to delete, this is through the selectedairplane that was found by the supplied id
        /// </returns>
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "AirplaneData/FindAirplane/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            AirplaneDto selectedairplane = response.Content.ReadAsAsync<AirplaneDto>().Result;
            return View(selectedairplane);
        }

        /// <summary>
        /// POST: Airplane/Delete/{id}
        /// This POST method is responsible for making the request to api/AirplaneData/DeleteAirplane to be able to delete the indicated airplane from the database. 
        /// If the IsSuccessStatusCode is true it will send the user to the list of airplanes, otherwise it will send the user to the Error page
        /// Go to (if success) -> /Views/Airplane/List.cshtml
        /// Go to (if not success) -> /Views/Airplane/Error.cshtml
        /// </summary>
        /// <param name="id">This id indicates the AirplaneId that will be used to determine the airplane that will be deleted</param>
        /// <returns>
        /// If the deletion is completed and no error occurs the user will be directed to the list of airplanes which will not show the recently deleted airplane. 
        /// If the IsSuccessStatusCode is false, this will indicate that the record was not deleted and the user will be directed to the View Error 
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();//get token credentials
            string url = "AirplaneData/DeleteAirplane/" + id;
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

        /// <summary>
        /// This Get method is responsible for returning the view when an error occurs, such as not finding the ID in the system or some of the operations that did not work
        /// Go to -> /Views/Airplane/Error.cshtml
        /// </summary>
        /// <returns>
        /// Return the Error View
        /// </returns>
        public ActionResult Error()
        {
            return View();
        }
    }
}