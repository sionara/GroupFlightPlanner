using GroupFlightPlanner.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using GroupFlightPlanner.Models.ViewModels;

namespace GroupFlightPlanner.Controllers
{
    public class AirlineController : Controller
    {

        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static AirlineController()
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
        /// 1. GET: Airline/List
        /// This GET method is responsible for making the call to the airline API, in which it will collect the list of airlines and provide the collected information to the View.
        /// This code is responsible for utilizing the client.BaseAddress and calling the ListAirlines method
        /// Go to  -> /Views/Airline/List.cshtml
        /// 
        /// 2. GET: Airline/List?AirlineSearch=can
        /// this will happen when the user provides a searck key in the form, this will display all the Airlines that contains the AirlineSearch = can
        /// Go to  -> /Views/Airline/List.cshtml
        /// </summary>
        /// <param name="AirlineSearch">This parameter is type string and it's function is to search for a specific Airline, if it is not given
        /// it will take the value of null and it will show all the Airline sin the system. If the user provides a string, this controller will provide the Airlines
        /// that contains the string given</param>
        /// <returns>
        /// Returns the List View, which will display a list of the airlines in the system. Each of the airlines in the database will be of the datatype AirlineDto.
        /// 
        /// Additionally, if the AirlineSearch != null it will display all the Airlines that contains the AirlineSearch
        /// </returns>
        public ActionResult List(string AirlineSearch = null)
        {
            //communicate with the airline data api to retrieve a list of airlines
            //curl https://localhost:44380/api/AirlineData/ListAirlines

            AirlineList ViewModel = new AirlineList();
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin")) ViewModel.IsAdmin = true;
            else ViewModel.IsAdmin = false;

            string url = "AirlineData/ListAirlines/" + AirlineSearch;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine(url);

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<AirlineDto> airlines = response.Content.ReadAsAsync<IEnumerable<AirlineDto>>().Result;
            //Debug.WriteLine("Number of airlines received : ");
            //Debug.WriteLine(airlines.Count());

            ViewModel.Airlines = airlines;

            return View(ViewModel);
        }

        /// <summary>
        /// GET: Airline/Details/{id}
        /// This GET method will be responsible for calling the FindAirline method from the airline,the FlightData/ListFlightsForAirline/ + id and the FlightData/ListPlanesForAirline/ + id.
        /// Because this time the method will return a ViewModels that holds the SelectedAirline, RelatedFlights (Related method) and RelatedAirplanes (Related method)
        /// RelatedFlights => List of flights for airline
        /// RelatedAirplanes => List of airplanes that a Airline has
        /// Go to  -> /Views/Airline/Details.cshtml
        /// </summary>
        /// <param name="id">This is an int datatype parameter of the airline you want to find.</param>
        /// <returns>
        /// It will return a ViewModel of type DetailsAirline this viewmodel will allow the view to acces to the SelectedAirline (airline found by the ID given in the URL), RelatedFlights and RelatedAirplanes
        /// </returns>
        public ActionResult Details(int id)
        {
            //communicate with the airline data api to retrieve one airline
            //curl https://localhost:44380/api/AirlineData/FindAirline/{id}

            //instance of ViewModel
            DetailsAirline ViewModel = new DetailsAirline();

            if (User.Identity.IsAuthenticated && User.IsInRole("Admin")) ViewModel.IsAdmin = true;
            else ViewModel.IsAdmin = false;

            string url = "AirlineData/FindAirline/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            AirlineDto SelectedAirline = response.Content.ReadAsAsync<AirlineDto>().Result;
            ViewModel.SelectedAirline = SelectedAirline;
            //Debug.WriteLine("airline received : ");
            //Debug.WriteLine(selectedairline.AirlineName);

            //showcase information about Flights related to this Airline -> ListFlightsForAirline
            //send a request to gather information about Flights related to a particular ID 
            url = "FlightData/ListFlightsForAirline/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<FlightDto> RelatedFlights = response.Content.ReadAsAsync<IEnumerable<FlightDto>>().Result;
            ViewModel.RelatedFlights = RelatedFlights;

            /*showcase information about Airplanes related to this Airline -> ListPlanesForAirline
            send a request to gather information about Flights related to a particular ID 
            The flights will hold information about the AirplaneModel and RegistrationNum
            Even though there will be repeated airplanes, only the first one is selected, this was done by
            
            List<FlightDto> FlightsDtosUnique = FlightsDtos
            .GroupBy(flight => flight.RegistrationNum)
            .Select(group => group.First())
            .ToList();

            This is through LINQ that grouped the airplanes through the RegistrationNum and then only selected the 
            first element of the group, thus allowing not to have duplicate airplanes*/
            url = "FlightData/ListPlanesForAirline/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<FlightDto> RelatedAirplanes = response.Content.ReadAsAsync<IEnumerable<FlightDto>>().Result;
            ViewModel.RelatedAirplanes = RelatedAirplanes;

            return View(ViewModel);
        }

        /// <summary>
        /// GET: Airline/New
        /// GET method to add a new airline to the system, responsible for providing the view of the form for inserting a new airline.
        /// Go to  -> /Views/Airline/New.cshtml
        /// </summary>
        /// <returns>
        /// Returns the view of the form so that the user can insert a new airline.
        /// </returns>
        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            return View();
        }


        /// <summary>
        /// POST: Airline/Create
        /// This POST method will be in charge of receiving the information sent by the new form, once the information is received 
        /// the method will be in charge of processing the conversion of the Airline object to json in order to be sent in the body of the HTTP REQUEST
        /// Additionally, it is indicated that its content is of type json in the reques header. Once this is done, 
        /// a POST request will be sent to the specified Uri as an asynchronous operation. If its IsSuccessStatusCode is true, 
        /// it will redirect the user to the list page, otherwise it will indicate to the user that there is an error.
        /// Go to (if success) -> /Views/Airline/List.cshtml
        /// Go to (if not success) -> /Views/Airline/Error.cshtml
        /// </summary>
        /// <param name="airline">This parameter represents the object received by the form for creating a new airline.</param>
        /// <returns>
        /// Returns the user to either the List View or the Error View, depending on the response StatusCode
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Airline airline)
        {
            GetApplicationCookie();
            Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(airline.AirlineName);
            //objective: add a new airline into the system using the API
            //curl -d @airline.json -H "Content-type: application/json" https://localhost:44380/api/AirlineData/AddAirline 
            string url = "AirlineData/AddAirline";

            string jsonpayload = jss.Serialize(airline);

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
        /// GET: Airline/Edit/{id}
        /// This GET method is in charge of collecting and sending the informaction to the View which will have a form with the airline information that is requested by its id, 
        /// for this the api/AirlineData/FindAirline/{id} is used. Once the call to the API is made, the information collected of the datatype AirlineDto will be sent to the view. 
        /// In this way the form will be populated with the information of the airline
        /// Go to  -> /Views/Airline/Edit.cshtml
        /// </summary>
        /// <param name="id">This is an int datatype parameter of the AirlineId value providaded by the url that will be displayed in the form in order to make an update</param>
        /// <returns>
        /// Returns the view with the form filled with the information of the airline to update
        /// </returns>
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            string url = "AirlineData/FindAirline/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            AirlineDto selectedairline = response.Content.ReadAsAsync<AirlineDto>().Result;

            Debug.WriteLine(selectedairline.AirlineName);
            Debug.WriteLine(selectedairline.FoundingYear);

            Debug.WriteLine("code --> " + (int)(response.StatusCode));

            return View(selectedairline);
        }

        /// <summary>
        /// POST: Airline/Update/{id}
        /// This POST method is responsible for making the call to the UpdateAirline method of the airline api. The information collected by the form will be sent in the body of the request
        /// Go to (if success) -> /Views/Airline/Details/{id}.cshtml
        /// Go to (if not success) -> /Views/Airline/Error.cshtml
        /// </summary>
        /// <param name="id">This is the parameter provided by the url that identifies the AirlineId that is going to be modified</param>
        /// <param name="airline">The airline object, this parameter holds the new data, this new data will be sent as a body to the UpdateAirline method of the Airline API</param>
        /// <returns>
        /// If the update is satisfactory the user will be redirected to the airline list, otherwise it will be sent to the error page
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id, Airline airline)
        {
            GetApplicationCookie();
            //serialize into JSON
            //Send the request to the API
            string url = "AirlineData/UpdateAirline/" + id;

            string jsonpayload = jss.Serialize(airline);
            Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            Debug.WriteLine("code --> " + (int)(response.StatusCode));
            Debug.WriteLine("status --> " + response.IsSuccessStatusCode);

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
        /// GET: Airline/DeleteConfirm/{id}
        /// This is a GET method that is responsible for finding the information of the airline to delete, this is done through its airline id which is provided by the id of the url
        /// Go to  -> /Views/Airline/DeleteConfirm.cshtml
        /// </summary>
        /// <param name="id">This is an int datatype parameter of the airline that will be displayed in DeleteConfirm View in order to delete the record</param>
        /// <returns>
        /// Returns a view that provides information about the airline to delete, this is through the selectedairline that was found by the supplied id
        /// </returns>
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "AirlineData/FindAirline/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            AirlineDto selectedairline = response.Content.ReadAsAsync<AirlineDto>().Result;
            return View(selectedairline);
        }

        /// <summary>
        /// POST: Airline/Delete/{id}
        /// This POST method is responsible for making the request to api/AirlineData/DeleteAirline to be able to delete the indicated airline from the database. 
        /// If the IsSuccessStatusCode is true it will send the user to the list of airlines, otherwise it will send the user to the Error page
        /// Go to (if success) -> /Views/Airline/List.cshtml
        /// Go to (if not success) -> /Views/Airline/Error.cshtml
        /// </summary>
        /// <param name="id">This id indicates the AirlineId that will be used to determine the airline that will be deleted</param>
        /// <returns>
        /// If the deletion is completed and no error occurs the user will be directed to the list of airlines which will not show the recently deleted airline. 
        /// If the IsSuccessStatusCode is false, this will indicate that the record was not deleted and the user will be directed to the View Error 
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            string url = "AirlineData/DeleteAirline/" + id;
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
        /// Go to -> /Views/Airline/Error.cshtml
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