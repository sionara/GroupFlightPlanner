using GroupFlightPlanner.Models;
using GroupFlightPlanner.Models.ViewModels;
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
    public class FlightController : Controller
    {

        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static FlightController()
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
        /// 1. GET: Flight/List
        /// This GET method is responsible for making the call to the flight API, in which it will collect the list of flights and provide the collected information to the View.
        /// This code is responsible for utilizing the client.BaseAddress and calling the ListFlights method
        /// Go to  -> /Views/Flight/List.cshtml
        /// 
        /// 2. GET: Flight/List?FlightSearch=UA
        /// this will happen when the user provides a searck key in the form, this will display all the Flights that contains the FlightSearch = UA
        /// Go to  -> /Views/Airline/List.cshtml
        /// </summary>
        /// <param name="FlightSearch">This parameter is type string and it's function is to search for a specific FLight, if it is not given
        /// it will take the value of null and it will show all the Flights in the system. If the user provides a string, this controller will provide the Flights
        /// that contains the string given</param>
        /// <returns>
        /// Returns the List View, which will display a list of the flights in the system. Each of the flights in the database will be of the datatype FlightDto.
        /// 
        /// Additionally, if the FlightSearch != null it will display all the Flights that contains the FlightSearch
        /// </returns>
        public ActionResult List(string FlightSearch = null)
        {
            //communicate with the flight data api to retrieve a list of flights
            //curl https://localhost:44380/api/FlightData/ListFlights

            string url = "FlightData/ListFlights/" + FlightSearch;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine(url);

            IEnumerable<FlightDto> flights = response.Content.ReadAsAsync<IEnumerable<FlightDto>>().Result;

            return View(flights);
        }

        /// <summary>
        /// GET: Flight/Details/{id}
        /// This GET method will be responsible for calling the FindFlight method from the flight API. Additionally, this method will be in charge to get the correct Time Flight (duration). Ddue to the time difference between de 
        /// Departure City and the Arrivale City the flight will not be congruent. In order to fix this, the logic should convert the Time Zone from on city to the Time Zone of the other city so that the dates of Departure and Arrivale will be on the same Time Zone 
        /// this was possible with the use of TimeZoneInfo.ConvertTimeBySystemTimeZoneId => This method converts a time from one time zone to another based on time zone identifiers. It accepts 3 parameters
        /// The date and time to convert (DateTime). The identifier of the source time zone (String). The identifier of the destination time zone (String). the time zone (type String) are provided by the user and it's stored in the DB
        /// Go to  -> /Views/Flight/Details.cshtml
        /// </summary>
        /// <param name="id">This is an int datatype parameter of the flight you want to find.</param>
        /// <returns>
        /// Returns a VIewModel DetailsFlight which holds a property SelectedFlight => the flight found by the ID given in the URL. This flight will be of the datatype FlightDto
        /// additionally, the ViewModel returns FlightDuration which its a string of the duration of the flights once the change to Time Zone is done as well as the mathematica operation to het the diference of hurs and minutes
        /// </returns>
        public ActionResult Details(int id)
        {
            //communicate with the flight data api to retrieve one flight
            //curl https://localhost:44380/api/FlightData/FindFlight/{id}

            //instance of ViewModel
            DetailsFlight ViewModel = new DetailsFlight();

            string url = "FlightData/FindFlight/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            FlightDto selectedflight = response.Content.ReadAsAsync<FlightDto>().Result;

            ViewModel.SelectedFlight = selectedflight;

            Debug.WriteLine("departure time: " + selectedflight.DepartureTime);
            Debug.WriteLine("arrival time: " + selectedflight.ArrivalTime);

            /* ---------- handle time difference to get flight duration ---------- */

            DateTime departureDate = DateTime.Parse(selectedflight.DepartureTime.ToString());
            DateTime arrivalDate = DateTime.Parse(selectedflight.ArrivalTime.ToString());

            try
            {

                DateTime convertDeparture = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(departureDate, selectedflight.TimeZoneFrom, selectedflight.TimeZoneTo);

                TimeSpan timeDifference = arrivalDate - convertDeparture;
                int Hours = (int)timeDifference.TotalHours;
                int Minutes = timeDifference.Minutes;

                Debug.WriteLine("duratiion" + Hours + ":" + Minutes);

                ViewModel.FlightDuration = Hours + "h " + Minutes + "min";
            }

            // in order to check if the time zone ID was not found on the local computer.
            catch (TimeZoneNotFoundException)
            {
                Debug.WriteLine("One of the time zone ID was not found on the local computer");
            }
            catch (InvalidTimeZoneException)
            {
                Debug.WriteLine("One of the time zone ID has been corrupted.");
            }

            /* ---------- handle time difference to get flight duration ---------- */

            //show all the locations under this flight

            url = "LocationData/ListLocationsForFlight/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<LocationDto> RelatedLocations = response.Content.ReadAsAsync<IEnumerable<LocationDto>>().Result;
            ViewModel.RelatedLocations = RelatedLocations;

            return View(ViewModel);
        }

        /// <summary>
        /// GET: Flight/New
        /// GET method to add a new flight to the system, responsible for providing the view of the form for inserting a new flight.
        /// Additionally it will provide the Airlines Options as well as the Airplanes Options so the user can select the desired airline and airplane on the drop downs
        /// This was possible through the call of the API to get the list of Airlines (api/AirlineData/ListAirlines) and the Airplanes (api/AirplaneData/ListAirplanes)
        /// Go to  -> /Views/Flight/New.cshtml
        /// </summary>
        /// <returns>
        /// Thi method provide the a ViewModel of type AddFlight, which holds the Airlines Options and Airplanes Options, so this properties will given to the VIew New
        /// </returns>
        [Authorize]
        public ActionResult New()
        {
            //information of all the airlines and airplanes in the system 

            //instance of ViewModel
            AddFlight ViewModel = new AddFlight();

            //1. GET api/AirlineData/ListAirlines
            string url = "AirlineData/ListAirlines";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<AirlineDto> AirlinesOptions = response.Content.ReadAsAsync<IEnumerable<AirlineDto>>().Result;
            ViewModel.AirlinesOptions = AirlinesOptions;

            //1. GET api/AirplaneData/ListAirplanes
            url = "AirplaneData/ListAirplanes";
            response = client.GetAsync(url).Result;
            IEnumerable<AirplaneDto> AirplanesOptions = response.Content.ReadAsAsync<IEnumerable<AirplaneDto>>().Result;
            ViewModel.AirplanesOptions = AirplanesOptions;

            //GET api/AirplaneData/ListAirplanes
            return View(ViewModel);
        }

        /// <summary>
        /// POST: Flight/Create
        /// This POST method will be in charge of receiving the information sent by the new form, once the information is received 
        /// the method will be in charge of processing the conversion of the Flight object to json in order to be sent in the body of the HTTP REQUEST
        /// Additionally, it is indicated that its content is of type json in the reques header. Once this is done, 
        /// A POST request will be sent to the specified Uri as an asynchronous operation. If its IsSuccessStatusCode is true, 
        /// it will redirect the user to the list Flights page, otherwise it will indicate to the user that there is an error.
        /// Go to (if new flight success) -> /Views/Flight/List.cshtml
        /// Go to (if not success) -> /Views/Flight/Error.cshtml
        /// </summary>
        /// <param name="flight">This parameter represents the object received by the form for creating a new flight.</param>
        /// <returns>
        /// Returns the user to either the List View or the Error View, depending on the response StatusCode
        /// </returns>
        [Authorize]
        [HttpPost]
        public ActionResult Create(Flight flight)
        {
            GetApplicationCookie();//get token credentials
            //objective: add a new flight into the system using the API
            //curl -d @flight.json -H "Content-type: application/json" https://localhost:44380/api/FlightData/AddFlight
            string url = "FlightData/AddFlight";

            string jsonpayload = jss.Serialize(flight);

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
        /// GET: Flight/Edit/{id}
        /// This GET method is in charge of collecting and sending the informaction to the View which will have a form with the flight information that is requested by its id, 
        /// for this the api/FlightData/FindFlight/{id} is used, additioally it will collect and send the information of the differnt Airlines and Airplanes in the sytem so the user can easly see the current vales
        /// and if desier, they will be able to change it throgh the use of the drop downs. The api used to get the list of Airlines is => AirlineData/ListAirlines and Airplanes => AirplaneData/ListAirplanes
        /// Once the call to this API API is made, the information collected of the datatype FlightDto, ListAirlines and ListAirplanes will be sent to the view by the ViewModel of type UpdateFlight. 
        /// In this way the form will be populated with the information of the flight
        /// Go to  -> /Views/Flight/Edit.cshtml
        /// </summary>
        /// <param name="id">This is an int datatype parameter of the FlightId value that will be displayed in the form in order to make an update</param>
        /// <returns>
        /// Returns a ViewModel of type UpdateFlight which holds the iformation of the flight to edit (ViewModel.SelectedFlight), the list of Airlines (ViewModel.AirlinesOptions) and the list of Airplanes (ViewModel.AirplanesOptions)
        /// </returns>
        [Authorize]
        public ActionResult Edit(int id)
        {
            //instance of ViewModel
            UpdateFlight ViewModel = new UpdateFlight();

            //1. The existing flight information
            string url = "FlightData/FindFlight/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            FlightDto SelectedFlight = response.Content.ReadAsAsync<FlightDto>().Result;
            ViewModel.SelectedFlight = SelectedFlight;

            //2. include all airlines to choose from when updating this flight
            url = "AirlineData/ListAirlines";
            response = client.GetAsync(url).Result;
            IEnumerable<AirlineDto> AirlinesOptions = response.Content.ReadAsAsync<IEnumerable<AirlineDto>>().Result;
            ViewModel.AirlinesOptions = AirlinesOptions;

            //3. include all airplanes to choose from when updating this flight
            url = "AirplaneData/ListAirplanes";
            response = client.GetAsync(url).Result;
            IEnumerable<AirplaneDto> AirplanesOptions = response.Content.ReadAsAsync<IEnumerable<AirplaneDto>>().Result;
            ViewModel.AirplanesOptions = AirplanesOptions;

            return View(ViewModel);
        }

        /// <summary>
        /// POST: Flight/Update/{id}
        /// This POST method is responsible for making the call to the UpdateFlight method of the flight api. The information collected by the form will be sent in the body of the request
        /// Go to (if success) -> /Views/Flight/Details/{id}.cshtml
        /// Go to (if not success) -> /Views/Flight/Error.cshtml
        /// </summary>
        /// <param name="id">This is the parameter provided by the url that identifies the FlightId that is going to be updated</param>
        /// <param name="flight">The flight object, this parameter holds the new data, this new data will be sent as a body to the UpdateFlight method of the Flight API</param>
        /// <returns>
        /// If the update is satisfactory the user will be redirected to the flight list, otherwise it will be sent to the error page
        /// </returns>
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Flight flight)
        {
            GetApplicationCookie();//get token credentials
            //serialize into JSON
            //Send the request to the API
            string url = "FlightData/UpdateFlight/" + id;

            string jsonpayload = jss.Serialize(flight);
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
        /// GET: Flight/DeleteConfirm/{id}
        /// This is a GET method that is responsible for finding the information of the flight to delete, this is done through its flight id which is provided by the id of the url
        /// Go to  -> /Views/Flight/DeleteConfirm.cshtml
        /// </summary>
        /// <param name="id">This is an int datatype parameter of the FlightId that will be displayed in DeleteConfirm View in order to delete the record</param>
        /// <returns>
        /// Returns a view that provides information about the flight to delete, this is through the selectedflight that was found by the supplied id
        /// </returns>
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "FlightData/FindFlight/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            FlightDto selectedflight = response.Content.ReadAsAsync<FlightDto>().Result;
            return View(selectedflight);
        }

        /// <summary>
        /// POST: Flight/Delete/{id}
        /// This POST method is responsible for making the request to api/FlightData/DeleteFlight to be able to delete the indicated flight from the database. 
        /// If the IsSuccessStatusCode is true it will send the user to the list of flights, otherwise it will send the user to the Error page
        /// Go to (if success) -> /Views/Flight/List.cshtml
        /// Go to (if not success) -> /Views/Flight/Error.cshtml
        /// </summary>
        /// <param name="id">This id indicates the FlightId that will be used to determine the flight that will be deleted</param>
        /// <returns>
        /// If the deletion is completed and no error occurs the user will be directed to the list of flights which will not show the recently deleted flight. 
        /// If the IsSuccessStatusCode is false, this will indicate that the record was not deleted and the user will be directed to the View Error 
        /// </returns>
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();//get token credentials
            string url = "FlightData/DeleteFlight/" + id;
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
        /// Go to -> /Views/Flight/Error.cshtml
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