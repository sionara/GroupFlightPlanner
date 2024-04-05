using GroupFlightPlanner.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace GroupFlightPlanner.Controllers
{
    public class FlightDataController : ApiController
    {
        //utilizing the database connection
        private ApplicationDbContext db = new ApplicationDbContext();

        //GET: 
        //output a list of Flights in the system


        /// <summary>
        /// 1. This is a GET method that will return a list of all the flights in the system. This method presents a foreach that will be responsible 
        /// for setting each of the FlightDto objects with the corresponding information. To do this, the information must first be collected, which will be a list of Flight datatypes. 
        /// Once this is done, the foreach will be run to have each of the data types as FlightDto. Additionally, this table has 2 Foreign Key that will be accessed through the class Airline
        /// for exaxmple to get the Airline name (AirlineName = f.Airline.AirlineName) and Airplane Model (AirplaneModel = f.Airplane.AirplaneModel)
        /// 
        /// 2. In this iteration of my passion project users can fiter flights; thats why the implementation of the FlightSearch parameter of this methos was added in order to look for one particular FLight in the system
        /// if the user provide a string on chich on search the code will interpret the value of FlightSearch != null, this will allow the code to look for a particular FLight. in case that the user does not
        /// want to search for a particular flight. The FlightSearch will be null and it will display all the Flights taht are in teh system
        /// </summary>
        /// <example>
        /// Using browser => GET: api/FlightData/ListFlights
        /// 
        /// Using curl comands in the terminal => curl https://localhost:44379/api/FlightData/ListFlights
        /// 
        /// Using browser with a search key => GET: api/FlightData/ListFlights/UA
        /// 
        /// Using curl comands in the terminal => curl https://localhost:44379/api/FlightData/ListFlights/UA
        /// </example>
        /// <returns>
        /// A list of Flights in the system
        /// GET: api/FlightData/ListFlights =>
        ///[{"FlightId":18,"FlightNumber":"AC 8606","From":"Montreal","To":"Boston","DepartureAirport":"Montréal-Pierre Elliott Trudeau International Airport (YUL)","DestinationAirport":"Boston Logan International Airport (BOS)","DepartureTime":"2024-02-06T13:10:00","ArrivalTime":"2024-02-06T14:38:00","TicketPrice":588.00,"TimeZoneFrom":"Eastern Standard Time","TimeZoneTo":"Eastern Standard Time","AirlineId":0,"AirlineName":"Air Canada","AirplaneId":0,"AirplaneModel":"Embraer E175SU\r\n"},
        /// {"FlightId":22,"FlightNumber":"AC 8607","From":"Boston","To":"Montreal","DepartureAirport":"Boston Logan International Airport (BOS)","DestinationAirport":"Montréal-Pierre Elliott Trudeau International Airport (YUL)","DepartureTime":"2024-02-06T15:40:00","ArrivalTime":"2024-02-06T17:08:00","TicketPrice":661.00,"TimeZoneFrom":"Eastern Standard Time","TimeZoneTo":"Eastern Standard Time","AirlineId":0,"AirlineName":"Air Canada","AirplaneId":0,"AirplaneModel":"Embraer E175SU\r\n"}]
        ///  
        /// curl https://localhost:44379/api/FlightData/ListFlights
        /// [{"FlightId":18,"FlightNumber":"AC 8606","From":"Montreal","To":"Boston","DepartureAirport":"Montréal-Pierre Elliott Trudeau International Airport (YUL)","DestinationAirport":"Boston Logan International Airport (BOS)","DepartureTime":"2024-02-06T13:10:00","ArrivalTime":"2024-02-06T14:38:00","TicketPrice":588.00,"TimeZoneFrom":"Eastern Standard Time","TimeZoneTo":"Eastern Standard Time","AirlineId":0,"AirlineName":"Air Canada","AirplaneId":0,"AirplaneModel":"Embraer E175SU\r\n"},
        /// {"FlightId":22,"FlightNumber":"AC 8607","From":"Boston","To":"Montreal","DepartureAirport":"Boston Logan International Airport (BOS)","DestinationAirport":"Montréal-Pierre Elliott Trudeau International Airport (YUL)","DepartureTime":"2024-02-06T15:40:00","ArrivalTime":"2024-02-06T17:08:00","TicketPrice":661.00,"TimeZoneFrom":"Eastern Standard Time","TimeZoneTo":"Eastern Standard Time","AirlineId":0,"AirlineName":"Air Canada","AirplaneId":0,"AirplaneModel":"Embraer E175SU\r\n"}]
        /// 
        ///     Search key example
        /// curl https://localhost:44379/api/FlightData/ListFlights/UA
        /// [{"FlightId":28,"FlightNumber":"UA 816","From":"Newark","To":"New Delhi","DepartureAirport":"Newark Liberty International Airport (EWR)","DestinationAirport":"Indira Gandhi International Airport (Delhi Airport) (DEL)","DepartureTime":"2024-02-06T09:40:00","ArrivalTime":"2024-02-07T11:10:00","TicketPrice":643.00,"TimeZoneFrom":"Eastern Standard Time","TimeZoneTo":"India Standard Time","AirlineId":22,"AirlineName":"United Airlines","AirplaneId":30,"AirplaneModel":"Boeing 787-9 Dreamliner\r\n","RegistrationNum":null},
        /// {"FlightId":29,"FlightNumber":"UA 817","From":"New Delhi","To":"Newark","DepartureAirport":"Indira Gandhi International Airport (Delhi Airport) (DEL)","DestinationAirport":"Newark Liberty International Airport (EWR)","DepartureTime":"2024-02-07T13:15:00","ArrivalTime":"2024-02-07T19:40:00","TicketPrice":1090.00,"TimeZoneFrom":"India Standard Time","TimeZoneTo":"Eastern Standard Time","AirlineId":22,"AirlineName":"United Airlines","AirplaneId":30,"AirplaneModel":"Boeing 787-9 Dreamliner\r\n","RegistrationNum":null}]
        /// 
        /// </returns>
        [HttpGet]
        [Route("api/FlightData/ListFlights/{FlightSearch?}")]
        public IEnumerable<FlightDto> ListFlights(string FlightSearch = null)
        {
            //sending a query to the database

            List<Flight> Flights = new List<Flight>();
            if (FlightSearch == null)
            {
                Flights = db.Flights.ToList();
            } else {
                Flights = db.Flights.Where(f => f.FlightNumber.ToLower().Contains(FlightSearch.ToLower())).ToList();
            }
            List<FlightDto> FlightsDtos = new List<FlightDto>();

            Flights.ForEach(f => FlightsDtos.Add(new FlightDto()
            {
                FlightId = f.FlightId,
                FlightNumber = f.FlightNumber,
                From = f.From,
                To = f.To,
                DepartureAirport = f.DepartureAirport,
                DestinationAirport = f.DestinationAirport,
                DepartureTime = f.DepartureTime,
                ArrivalTime = f.ArrivalTime,
                TicketPrice = f.TicketPrice,
                TimeZoneFrom = f.TimeZoneFrom,
                TimeZoneTo = f.TimeZoneTo,
                AirlineId = f.AirlineId,
                AirlineName = f.Airline.AirlineName,
                AirplaneId = f.AirplaneId,
                AirplaneModel = f.Airplane.AirplaneModel
            }));

            return FlightsDtos;
        }

        /// <summary>
        /// The method takes an integer parameter "id," representing the AirlineId, and retrieves a list of flights from the database associated with the specified airline. 
        /// The retrieved Flight entities are then mapped to FlightDto objects, which are a data transfer object. This was made in order to solve the related method ListFlightsForAirline => that a Airline has with the FLights (1 Airline - Many Flights)
        /// </summary>
        /// <param name="id">This is the Id of the Airline which will be used to retieve all the flights for this particular Airline</param>
        /// <example>
        /// curl https://localhost:44379/api/FlightData/ListFlightsForAirline/22
        /// 
        /// [{"FlightId":28,"FlightNumber":"UA 816","From":"Newark","To":"New Delhi","DepartureAirport":"Newark Liberty International Airport (EWR)","DestinationAirport":"Indira Gandhi International Airport (Delhi Airport) (DEL)","DepartureTime":"2024-02-06T09:40:00","ArrivalTime":"2024-02-07T11:10:00","TicketPrice":643.00,"TimeZoneFrom":"Eastern Standard Time","TimeZoneTo":"India Standard Time","AirlineId":22,"AirlineName":"United Airlines","AirplaneId":30,"AirplaneModel":"Boeing 787-9 Dreamliner\r\n","RegistrationNum":null},
        /// {"FlightId":29,"FlightNumber":"UA 817","From":"New Delhi","To":"Newark","DepartureAirport":"Indira Gandhi International Airport (Delhi Airport) (DEL)","DestinationAirport":"Newark Liberty International Airport (EWR)","DepartureTime":"2024-02-07T13:15:00","ArrivalTime":"2024-02-07T19:40:00","TicketPrice":1090.00,"TimeZoneFrom":"India Standard Time","TimeZoneTo":"Eastern Standard Time","AirlineId":22,"AirlineName":"United Airlines","AirplaneId":30,"AirplaneModel":"Boeing 787-9 Dreamliner\r\n","RegistrationNum":null}]
        /// 
        /// curl https://localhost:44379/api/FlightData/ListFlightsForAirline/30
        /// 
        /// [{"FlightId":35,"FlightNumber":"G5 2662","From":"Shihezi","To":"Kuqa, Xinjiang","DepartureAirport":"Shihezi Huayuan Airport (SHF)","DestinationAirport":"Kuqa Qiuci Airport (KCA)","DepartureTime":"2024-02-07T14:50:00","ArrivalTime":"2024-02-07T16:10:00","TicketPrice":94.00,"TimeZoneFrom":"China Standard Time","TimeZoneTo":"China Standard Time","AirlineId":30,"AirlineName":"China Express Airlines\r\nChina\r\n","AirplaneId":34,"AirplaneModel":"Mitsubishi CRJ-900LR\r\n","RegistrationNum":null},
        /// {"FlightId":36,"FlightNumber":"G5 2662","From":"Kuqa, Xinjiang","To":"Aksu City","DepartureAirport":"Kuqa Qiuci Airport (KCA)","DestinationAirport":"Aksu Airport (AKU)","DepartureTime":"2024-02-07T16:50:00","ArrivalTime":"2024-02-07T17:30:00","TicketPrice":47.00,"TimeZoneFrom":"China Standard Time","TimeZoneTo":"China Standard Time","AirlineId":30,"AirlineName":"China Express Airlines\r\nChina\r\n","AirplaneId":34,"AirplaneModel":"Mitsubishi CRJ-900LR\r\n","RegistrationNum":null}]
        /// </example>
        /// <returns>A list of Flights of type FlightDto which its AirlineId == id (parameter given in the method)</returns>
        [HttpGet]
        public IEnumerable<FlightDto> ListFlightsForAirline(int id)
        {
            //sending a query to the database
            List<Flight> Flights = db.Flights.Where(f => f.AirlineId == id).ToList();
            List<FlightDto> FlightsDtos = new List<FlightDto>();

            Flights.ForEach(f => FlightsDtos.Add(new FlightDto()
            {
                FlightId = f.FlightId,
                FlightNumber = f.FlightNumber,
                From = f.From,
                To = f.To,
                DepartureAirport = f.DepartureAirport,
                DestinationAirport = f.DestinationAirport,
                DepartureTime = f.DepartureTime,
                ArrivalTime = f.ArrivalTime,
                TicketPrice = f.TicketPrice,
                TimeZoneFrom = f.TimeZoneFrom,
                TimeZoneTo = f.TimeZoneTo,
                AirlineId = f.Airline.AirlineId,
                AirlineName = f.Airline.AirlineName,
                AirplaneId = f.Airplane.AirplaneId,
                AirplaneModel = f.Airplane.AirplaneModel
            }));

            return FlightsDtos;
        }


        /// <summary>
        /// The method takes an integer parameter "id," representing the AirplaneId, and retrieves a list of flights from the database associated with the specified airplane. 
        /// The retrieved Flight entities are then mapped to FlightDto objects, which are a data transfer object. This was made in order to solve the related method ListFlightsForAirplane => that a Airplane has with the FLights (1 Airplane - Many Flights)
        /// </summary>
        /// <param name="id">This is the Id of the Airplane which will be used to retieve all the flights for this particular Airplane</param>
        /// <example>
        /// curl https://localhost:44379/api/FlightData/ListFlightsForAirplane/31
        /// 
        /// [{"FlightId":30,"FlightNumber":"WS 3205","From":"Calgary","To":"Grande Prairie","DepartureAirport":"Calgary International Airport (YYC) (YYC)","DestinationAirport":"Grande Prairie Airport (CYQU) (YQU)","DepartureTime":"2024-02-06T15:00:00","ArrivalTime":"2024-02-06T16:33:00","TicketPrice":583.00,"TimeZoneFrom":"Mountain Standard Time","TimeZoneTo":"Mountain Standard Time","AirlineId":24,"AirlineName":"WestJet Airlines Ltd.","AirplaneId":31,"AirplaneModel":"De Havilland Canada Dash 8-400\r\n","RegistrationNum":null},
        /// {"FlightId":31,"FlightNumber":"WS 3206","From":"Grande Prairie","To":"Calgary","DepartureAirport":"Grande Prairie Airport (CYQU) (YQU)","DestinationAirport":"Calgary International Airport (YYC) (YYC)","DepartureTime":"2024-02-06T17:15:00","ArrivalTime":"2024-02-06T18:37:00","TicketPrice":500.00,"TimeZoneFrom":"Mountain Standard Time","TimeZoneTo":"Mountain Standard Time","AirlineId":24,"AirlineName":"WestJet Airlines Ltd.","AirplaneId":31,"AirplaneModel":"De Havilland Canada Dash 8-400\r\n","RegistrationNum":null},
        /// { "FlightId":32,"FlightNumber":"WS 3137","From":"Calgary","To":"Edmonton","DepartureAirport":"Calgary International Airport (YYC) (YYC)","DestinationAirport":"Edmonton International Airport (YEG)","DepartureTime":"2024-02-06T20:00:00","ArrivalTime":"2024-02-06T21:03:00","TicketPrice":535.00,"TimeZoneFrom":"Mountain Standard Time","TimeZoneTo":"Mountain Standard Time","AirlineId":24,"AirlineName":"WestJet Airlines Ltd.","AirplaneId":31,"AirplaneModel":"De Havilland Canada Dash 8-400\r\n","RegistrationNum":null}]
        /// 
        /// 
        /// curl https://localhost:44379/api/FlightData/ListFlightsForAirplane/35
        /// [{"FlightId":37,"FlightNumber":"AC 171","From":"Toronto","To":"Edmonton","DepartureAirport":"Toronto Pearson International Airport (YYZ)","DestinationAirport":"Edmonton International Airport (YEG)","DepartureTime":"2024-02-06T10:00:00","ArrivalTime":"2024-02-06T12:18:00","TicketPrice":850.00,"TimeZoneFrom":"Eastern Standard Time","TimeZoneTo":"Mountain Standard Time","AirlineId":28,"AirlineName":"Air Canada","AirplaneId":35,"AirplaneModel":"Airbus A220-300\r\n","RegistrationNum":null},
        /// {"FlightId":38,"FlightNumber":"AC 426","From":"Toronto","To":"Montreal","DepartureAirport":"Toronto Pearson International Airport (YYZ)","DestinationAirport":"Montréal-Pierre Elliott Trudeau International Airport (YUL)","DepartureTime":"2024-02-07T20:30:00","ArrivalTime":"2024-02-07T21:50:00","TicketPrice":965.00,"TimeZoneFrom":"Eastern Standard Time","TimeZoneTo":"Eastern Standard Time","AirlineId":28,"AirlineName":"Air Canada","AirplaneId":35,"AirplaneModel":"Airbus A220-300\r\n","RegistrationNum":null}]
        /// </example>
        /// <returns>A list of Flights of type FlightDto which its AirplaneId == id (parameter given in the method)</returns>
        [HttpGet]
        public IEnumerable<FlightDto> ListFlightsForAirplane(int id)
        {
            //sending a query to the database
            List<Flight> Flights = db.Flights.Where(f => f.AirplaneId == id).ToList();
            List<FlightDto> FlightsDtos = new List<FlightDto>();

            Flights.ForEach(f => FlightsDtos.Add(new FlightDto()
            {
                FlightId = f.FlightId,
                FlightNumber = f.FlightNumber,
                From = f.From,
                To = f.To,
                DepartureAirport = f.DepartureAirport,
                DestinationAirport = f.DestinationAirport,
                DepartureTime = f.DepartureTime,
                ArrivalTime = f.ArrivalTime,
                TicketPrice = f.TicketPrice,
                TimeZoneFrom = f.TimeZoneFrom,
                TimeZoneTo = f.TimeZoneTo,
                AirlineId = f.Airline.AirlineId,
                AirlineName = f.Airline.AirlineName,
                AirplaneId = f.Airplane.AirplaneId,
                AirplaneModel = f.Airplane.AirplaneModel
            }));

            return FlightsDtos;
        }

        /// <summary>
        /// The funcion of this method is to access to all the Airplane Model that a Airline has. The method takes an integer parameter "id," representing the AirlineId, and retrieves a list of flights from the database 
        /// associated with the specified airline. Once this is done i made a filter for the RegistrationNum because the idea was to get the planes that a Airline has for that I first Group the recod by the RegistrationNum.
        /// Then I select the first record of the Group and finally I make it a list of the records. The reason that I select the first it's to not get duplicate records of the Airplane
        /// </summary>
        /// <param name="id">This is the Id of the Airline which will be used to retieve all the flights with the informacion of the Plane</param>
        /// <example>
        /// curl https://localhost:44379/api/FlightData/ListPlanesForAirline/26
        /// </example>
        /// <returns>
        /// Returns the list of unique FlightDto objects containing information about flights and their associated airplanes
        /// This was done to display the Airplanes that belongs to a Airline
        /// </returns>
        [HttpGet]
        public IEnumerable<FlightDto> ListPlanesForAirline(int id)
        {
            //sending a query to the database
            List<Flight> Flights = db.Flights.Where(f => f.AirlineId == id).ToList();
            List<FlightDto> FlightsDtos = new List<FlightDto>();

            Flights.ForEach(f => FlightsDtos.Add(new FlightDto()
            {
                FlightId = f.FlightId,
                FlightNumber = f.FlightNumber,
                From = f.From,
                To = f.To,
                DepartureAirport = f.DepartureAirport,
                DestinationAirport = f.DestinationAirport,
                DepartureTime = f.DepartureTime,
                ArrivalTime = f.ArrivalTime,
                TicketPrice = f.TicketPrice,
                TimeZoneFrom = f.TimeZoneFrom,
                TimeZoneTo = f.TimeZoneTo,
                AirlineId = f.Airline.AirlineId,
                AirlineName = f.Airline.AirlineName,
                AirplaneId = f.Airplane.AirplaneId,
                AirplaneModel = f.Airplane.AirplaneModel,
                RegistrationNum = f.Airplane.RegistrationNum
            }));

            //Grouping the FlightsDtos based on the RegistrationNum, selecting the first item from each group, and creating a new list of unique FlightDto objects

            List<FlightDto> FlightsDtosUnique = FlightsDtos
            .GroupBy(flight => flight.RegistrationNum)
            .Select(group => group.First())
            .ToList();

            foreach (var flight in FlightsDtosUnique)
            {
                Debug.WriteLine($"Name: {flight.AirplaneModel}, Valor: {flight.RegistrationNum}");
            }

            return FlightsDtosUnique;
        }

        /// <summary>
        /// Gather information on Flights that are gonna go to a location (list of associated flights to a location), this Get method will find all the flights that were associated with a location
        /// this method is in charge of lopping the database and look for any flight that contains locations that match with the location id that will be providadded in the url
        /// </summary>
        /// <example>
        /// Using browser => GET: api/FlightData/ListFlightsForLocation/1
        /// 
        /// Using curl comands in the terminal => curl https://localhost:44380/api/FlightData/ListFlightsForLocation/1
        /// </example>
        /// <param name="id">This is the id of the location that we are looking for</param>
        /// <returns>
        /// Return a list of the flights that are associated to the location 
        /// </returns>
        [HttpGet]
        public IEnumerable<FlightDto> ListFlightsForLocation(int id)
        {

            List<Flight> Flights = db.Flights.Where(
                f => f.Locations.Any(
                    l => l.LocationId == id
                )).ToList();

            List<FlightDto> FlightsDtos = new List<FlightDto>();

            Flights.ForEach(f => FlightsDtos.Add(new FlightDto()
            {
                FlightId = f.FlightId,
                FlightNumber = f.FlightNumber,
                From = f.From,
                To = f.To,
                DepartureAirport = f.DepartureAirport,
                DestinationAirport = f.DestinationAirport,
                DepartureTime = f.DepartureTime,
                ArrivalTime = f.ArrivalTime,
                TicketPrice = f.TicketPrice,
                TimeZoneFrom = f.TimeZoneFrom,
                TimeZoneTo = f.TimeZoneTo,
                AirlineId = f.AirlineId,
                AirlineName = f.Airline.AirlineName,
                AirplaneId = f.AirplaneId,
                AirplaneModel = f.Airplane.AirplaneModel
            }));

            return FlightsDtos;
        }

        /// <summary>
        /// This Get method will list the differente flights taht are not asssociated with aa Location. the purpose of this method is to be able
        /// to assign a flight to the location, but if the flight is already assigned it shoul not display it on the dropdown list
        /// </summary>
        /// <example>
        /// Using browser => GET: api/FlightData/ListFlightsNotAssociatedForLocation/1
        /// 
        /// Using curl comands in the terminal => curl https://localhost:44380/api/FlightData/ListFlightsNotAssociatedForLocation/1
        /// </example>
        /// <param name="id">This is the id of the location that we are looking for</param>
        /// <returns>
        /// Return a list of the flights that are associated to the location 
        /// </returns>
        [HttpGet]
        public IEnumerable<FlightDto> ListFlightsNotAssociatedForLocation(int id)
        {

            List<Flight> Flights = db.Flights.Where(
                f => !f.Locations.Any(
                    l => l.LocationId == id
                )).ToList();

            List<FlightDto> FlightsDtos = new List<FlightDto>();

            Flights.ForEach(f => FlightsDtos.Add(new FlightDto()
            {
                FlightId = f.FlightId,
                FlightNumber = f.FlightNumber,
                From = f.From,
                To = f.To,
                DepartureAirport = f.DepartureAirport,
                DestinationAirport = f.DestinationAirport,
                DepartureTime = f.DepartureTime,
                ArrivalTime = f.ArrivalTime,
                TicketPrice = f.TicketPrice,
                TimeZoneFrom = f.TimeZoneFrom,
                TimeZoneTo = f.TimeZoneTo,
                AirlineId = f.AirlineId,
                AirlineName = f.Airline.AirlineName,
                AirplaneId = f.AirplaneId,
                AirplaneModel = f.Airplane.AirplaneModel
            }));

            return FlightsDtos;
        }


        /// <summary>
        /// This GET method returns an individual flight from the database by specifying the primary key FlightId
        /// Additionally, this method will allow you to collect the information on the Foreign Key IDs as well as the airline and the plane model.
        /// since these will be requested in Details View as well as in Edit View
        /// </summary>
        /// <example>
        /// Using browser => GET: api/FlightData/FindFlight/25
        /// 
        /// Using curl comands in the terminal => curl https://localhost:44379/api/FlightData/FindFlight/25
        /// </example>
        /// <param name="id">This is the FlightId parameter you want to search for.</param>
        /// <returns>
        /// A FlightDto object which represent the flight with the id given 
        /// 
        /// curl https://localhost:44379/api/FlightData/FindFlight/25
        /// {"FlightId":25,"FlightNumber":"AA 33","From":"New York","To":"Los Angeles","DepartureAirport":"John F. Kennedy International Airport (JFK)","DestinationAirport":"Los Angeles International Airport (LAX)","DepartureTime":"2024-02-06T06:00:00","ArrivalTime":"2024-02-06T09:23:00","TicketPrice":583.00,"TimeZoneFrom":"Eastern Standard Time","TimeZoneTo":"Pacific Standard Time","AirlineId":26,"AirlineName":"American\r\n","AirplaneId":28,"AirplaneModel":"Airbus A321-231\r\n"}
        /// 
        /// curl https://localhost:44379/api/FlightData/FindFlight/37
        /// {"FlightId":37,"FlightNumber":"AC 171","From":"Toronto","To":"Edmonton","DepartureAirport":"Toronto Pearson International Airport (YYZ)","DestinationAirport":"Edmonton International Airport (YEG)","DepartureTime":"2024-02-06T10:00:00","ArrivalTime":"2024-02-06T12:18:00","TicketPrice":850.00,"TimeZoneFrom":"Eastern Standard Time","TimeZoneTo":"Mountain Standard Time","AirlineId":28,"AirlineName":"Air Canada","AirplaneId":35,"AirplaneModel":"Airbus A220-300\r\n"}
        /// 
        /// </returns>
        [ResponseType(typeof(Flight))]
        [HttpGet]
        public IHttpActionResult FindFlight(int id)
        {
            Flight Flight = db.Flights.Find(id);

            if (Flight == null)
            {
                return NotFound();
            }

            FlightDto FlightDto = new FlightDto()
            {
                FlightId = Flight.FlightId,
                FlightNumber = Flight.FlightNumber,
                From = Flight.From,
                To = Flight.To,
                DepartureAirport = Flight.DepartureAirport,
                DestinationAirport = Flight.DestinationAirport,
                DepartureTime = Flight.DepartureTime,
                ArrivalTime = Flight.ArrivalTime,
                TicketPrice = Flight.TicketPrice,
                TimeZoneFrom = Flight.TimeZoneFrom,
                TimeZoneTo = Flight.TimeZoneTo,
                AirlineName = Flight.Airline.AirlineName,
                AirplaneModel = Flight.Airplane.AirplaneModel,
                AirlineId = Flight.Airline.AirlineId,
                AirplaneId = Flight.Airplane.AirplaneId
            };


            return Ok(FlightDto);
        }


        /// <summary>
        /// This method is responsible for deleting a flight record in the Flights table, the deleted record will be according to the value indicated in the id parameter
        /// </summary>
        /// <example>
        /// Because this method is POST, you must use the curl command in the terminal to delete the record
        /// 
        /// POST: api/FlightData/DeleteFlight/39
        /// curl -d "" https://localhost:44379/api/FlightData/DeleteFlight/39
        /// </example>
        /// <param name="id">This will indicate the FlightId value that you want to eliminate</param>
        /// <returns>
        /// curl -d "" https://localhost:44379/api/FlightData/DeleteFlight/39
        /// This method will not return anything on the console, it will delete a flight from the DB, in this case the record with the FlightId = 39 will be deleated
        /// </returns>
        [ResponseType(typeof(Flight))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeleteFlight(int id)
        {
            Flight Flight = db.Flights.Find(id);
            if (Flight == null)
            {
                return NotFound();
            }

            db.Flights.Remove(Flight);
            db.SaveChanges();

            return Ok();
        }


        /// <summary>
        /// This method is responsible for receiving a new Flight record. For this, the Flight type object must be sent. In order to be able to test this method, 
        /// you must open the folder where the flight.json is located
        /// 
        /// When a user registers a flight to the system, they must enter the date and time of take off and arrival, however this is affected by the offset of the Time Zone where the computer is as it stores Date and time provided + time offset
        /// In order to solve this, the timeZoneLocal.GetUtcOffset method was used and the .Add method was performed, which would allow adding or subtracting depending on the offset. 
        /// This was used to save the correct date and time (supplied by the form) for the DepartureTime and ArrivalTime on the Database
        /// </summary>
        /// <example>
        /// curl -d @flight.json -H "Content-type: application/json" https://localhost:44379/api/FlightData/AddFlight
        /// The flight.json file contains the flight information that will be added to the DB.
        /// </example>
        /// <param name="Flight">This is the json object that contains the information of the flight to add.</param>
        /// <returns>
        /// {
        ///     "FlightId": 46,
        ///     "FlightNumber": "AC 426",
        ///     "From": "Toronto",
        ///     "To": "Montreal",
        ///     "DepartureAirport": "Toronto Pearson International Airport (YYZ)",
        ///     "DestinationAirport": "Montréal-Pierre Elliott Trudeau International Airport (YUL)",
        ///     "DepartureTime": "2/6/2024 8:30:00 AM",
        ///     "ArrivalTime": "2/6/2024 9:50:00 PM",
        ///     "TicketPrice": 965.00,
        ///     "TimeZoneFrom": "Eastern Standard Time",
        ///     "TimeZoneTo": "Eastern Standard Time",
        ///     "AirlineId": 28,
        ///     "AirplaneId": 35
        /// }
        /// </returns>
        [ResponseType(typeof(Flight))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddFlight(Flight Flight)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // initializes a local TimeZoneInfo object
            // getting and constructing components from the DepartureTime given in the form and assign it to a DateTime formDateDeparture
            // calculates the correct departure date by adding the local time zone's UTC offset to the formDateDeparture
            // Updates the Flight's DepartureTime with the corrected value

            TimeZoneInfo timeZoneLocal = TimeZoneInfo.Local;

            Debug.WriteLine("Date Form with offset DepartureTime: " + Flight.DepartureTime);
            DateTime formDateDeparture = new DateTime(Flight.DepartureTime.Year, Flight.DepartureTime.Month, Flight.DepartureTime.Day, Flight.DepartureTime.Hour, Flight.DepartureTime.Minute, Flight.DepartureTime.Millisecond);
            DateTime departureDateCorrect = formDateDeparture.Add(timeZoneLocal.GetUtcOffset(formDateDeparture));
            Debug.WriteLine("Real Date correcting offset DepartureTime : " + departureDateCorrect);
            Flight.DepartureTime = departureDateCorrect;

            //the same process was done to get the correct value assigned to the ArrivalTime

            Debug.WriteLine("Date Form with offset ArrivalTime: " + Flight.ArrivalTime);
            DateTime formDateArrival = new DateTime(Flight.ArrivalTime.Year, Flight.ArrivalTime.Month, Flight.ArrivalTime.Day, Flight.ArrivalTime.Hour, Flight.ArrivalTime.Minute, Flight.ArrivalTime.Millisecond);
            DateTime arrivalDateCorrect = formDateArrival.Add(timeZoneLocal.GetUtcOffset(formDateArrival));
            Debug.WriteLine("Real Date correcting offset ArrivalTime : " + arrivalDateCorrect);
            Flight.ArrivalTime = arrivalDateCorrect;

            db.Flights.Add(Flight);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Flight.FlightId }, Flight);
        }



        /// <summary>
        /// This POST method will be in charge of updating the record according to the id parameter that is provided, for this both the value that is delivered by the url (id) 
        /// and the value that the FlightId (within the object received in FLight) have to match. otherwise the record is not updated
        /// additionally, because this method is post to check its operation you must use the curl command in the terminal. for this open the folder where the flight.json is located on the terminal screen
        /// 
        /// once you have the folder open you can use this command curl -d @flight.json -H "Content-type: application/json" https://localhost:44379/api/FlightData/UpdateFlight/39
        /// 
        /// This method would also have the offset problem mentioned in the AddFlight method, therefore using the timeZoneLocal.GetUtcOffset and .Add methods would be necessary to handle the offset of the time. 
        /// If this is not solved, every time the user updates a flight, the DepartureTime and ArrivalTime will add the number of hours according to the time zone offset
        /// This was used to save the correct date and time (supplied by the form) for the DepartureTime and ArrivalTime on the Database
        /// </summary>
        /// <example>
        /// curl -d @flight.json -H "Content-type: application/json" https://localhost:44379/api/FlightData/UpdateFlight/39
        /// </example>
        /// <param name="id">The value of the FlightId to update</param>
        /// <param name="Flight">The Flight object, this parameter holds the new data, with this data the method will update the specified Flight</param>
        /// <returns>
        /// This method will not return anything on the console, but the method will return whether the update was successful or not, in case it was not successful because there was no id match (id != Flight.FlightId)
        /// The values of the Flight object and the id sent in the url are printed in the console. If the method runs, without problem, the record will be updated.
        /// </returns>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateFlight(int id, Flight Flight)
        {
            Debug.WriteLine("Update Flight method starts here");
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model State is invalid");
                return BadRequest(ModelState);
            }

            if (id != Flight.FlightId)
            {
                Debug.WriteLine("ID mismatch");
                Debug.WriteLine("GET parameter " + id);
                Debug.WriteLine("POST parameter " + Flight.FlightId);
                Debug.WriteLine("POST parameter " + Flight.FlightNumber);
                Debug.WriteLine("POST parameter " + Flight.From);
                Debug.WriteLine("POST parameter " + Flight.To);
                Debug.WriteLine("POST parameter " + Flight.DepartureAirport);
                Debug.WriteLine("POST parameter " + Flight.DestinationAirport);
                Debug.WriteLine("POST parameter " + Flight.DepartureTime);
                Debug.WriteLine("POST parameter " + Flight.ArrivalTime);
                Debug.WriteLine("POST parameter " + Flight.TicketPrice);
                Debug.WriteLine("POST parameter " + Flight.TimeZoneFrom);
                Debug.WriteLine("POST parameter " + Flight.TimeZoneTo);
                Debug.WriteLine("POST parameter " + Flight.AirlineId);
                Debug.WriteLine("POST parameter " + Flight.AirplaneId);
                return BadRequest();
            }

            // initializes a local TimeZoneInfo object
            // getting and constructing components from the DepartureTime given in the form and assign it to a DateTime formDateDeparture
            // calculates the correct departure date by adding the local time zone's UTC offset to the formDateDeparture
            // Updates the Flight's DepartureTime with the corrected value

            TimeZoneInfo timeZoneLocal = TimeZoneInfo.Local;

            Debug.WriteLine("Date Form with offset DepartureTime: " + Flight.DepartureTime);
            DateTime formDateDeparture = new DateTime(Flight.DepartureTime.Year, Flight.DepartureTime.Month, Flight.DepartureTime.Day, Flight.DepartureTime.Hour, Flight.DepartureTime.Minute, Flight.DepartureTime.Millisecond);
            DateTime departureDateCorrect = formDateDeparture.Add(timeZoneLocal.GetUtcOffset(formDateDeparture));
            Debug.WriteLine("Real Date correcting offset DepartureTime : " + departureDateCorrect);
            Flight.DepartureTime = departureDateCorrect;

            //the same process was done to get the correct value assigned to the ArrivalTime

            Debug.WriteLine("Date Form with offset ArrivalTime: " + Flight.ArrivalTime);
            DateTime formDateArrival = new DateTime(Flight.ArrivalTime.Year, Flight.ArrivalTime.Month, Flight.ArrivalTime.Day, Flight.ArrivalTime.Hour, Flight.ArrivalTime.Minute, Flight.ArrivalTime.Millisecond);
            DateTime arrivalDateCorrect = formDateArrival.Add(timeZoneLocal.GetUtcOffset(formDateArrival));
            Debug.WriteLine("Real Date correcting offset ArrivalTime : " + arrivalDateCorrect);
            Flight.ArrivalTime = arrivalDateCorrect;

            db.Entry(Flight).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightExists(id))
                {
                    Debug.WriteLine("Flight not found");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            Debug.WriteLine("None of the conditions triggered");
            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        /// <summary>
        /// This method is responsible for returning a bool indicating if the flight exists
        /// </summary>
        /// <param name="id">The value of the FlightId to update</param>
        /// <returns>A bool indicating if the record exists</returns>
        private bool FlightExists(int id)
        {
            return db.Flights.Count(e => e.FlightId == id) > 0;
        }
    }
}
