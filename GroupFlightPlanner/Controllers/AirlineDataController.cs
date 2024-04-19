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
    public class AirlineDataController : ApiController
    {

        //utilizing the database connection
        private ApplicationDbContext db = new ApplicationDbContext();

        //output a list of Airlines in the system


        /// <summary>
        /// This is a GET method that will return a list of all the airlines in the system. This method presents a foreach that will be responsible 
        /// for setting each of the AirlineDto objects with the corresponding information. To do this, the information must first be collected, which will be a list of Airline datatypes. 
        /// Once this is done, the foreach will be run to have each of the data types as AirlineDto.
        /// 
        /// Additionally it will accept a search key, this was used in order to filter the list, the concept it's the same but will accept nother parameter in the URL
        /// </summary>
        /// <example>
        /// Using browser => GET: api/AirlineData/ListAirlines
        /// 
        /// Using curl comands in the terminal => curl https://localhost:44379/api/AirlineData/ListAirlines
        /// 
        /// Using browser with a search key => GET: api/AirlineData/ListAirlines/avianca
        /// 
        /// Using curl comands in the terminal => curl https://localhost:44379/api/AirlineData/ListAirlines/avianca
        /// </example>
        /// <param name="AirlineSearch">This is the search key, the api will use it to look for this specific Airline
        /// if the user does not enter a search key the GET method will display the list of all the Airlines otherwise, it will display the 
        /// Airlines that Contains the search key. Additionally, its case insensitive
        /// </param>
        /// <returns>
        /// A list of Airlines in the system or the airlines that contains the string given on the AirlineSearch parameter
        /// 
        /// GET: api/AirlineData/ListAirlines =>
        /// [{"AirlineId":22,"AirlineName":"United Airlines","Country":"United States","Headquarters":"Willis Tower, Chicago, Illinois U.S.","FounderName":"Walter Varney","FoundingYear":"1926-04-06T00:00:00","Website":"https://www.united.com/en/us","ContactNumber":"18008648331"},
        /// {"AirlineId":23,"AirlineName":"Lynx Air","Country":"Canada","Headquarters":"Calgary, Alberta, Canada","FounderName":"Tim Morgan","FoundingYear":"2006-09-06T00:00:00","Website":"https://www.flylynx.com/en#refreshforced","ContactNumber":"18778975969"},
        /// {"AirlineId":24,"AirlineName":"WestJet Airlines Ltd.","Country":"Canada","Headquarters":"Calgary, Alberta, Canada","FounderName":"Clive Beddoe","FoundingYear":"1994-06-27T00:00:00","Website":"https://www.westjet.com/en-ca","ContactNumber":"18889378538"}]
        /// 
        /// curl https://localhost:44379/api/AirlineData/ListAirlines
        /// [{"AirlineId":22,"AirlineName":"United Airlines","Country":"United States","Headquarters":"Willis Tower, Chicago, Illinois U.S.","FounderName":"Walter Varney","FoundingYear":"1926-04-06T00:00:00","Website":"https://www.united.com/en/us","ContactNumber":"18008648331"},
        /// {"AirlineId":23,"AirlineName":"Lynx Air","Country":"Canada","Headquarters":"Calgary, Alberta, Canada","FounderName":"Tim Morgan","FoundingYear":"2006-09-06T00:00:00","Website":"https://www.flylynx.com/en#refreshforced","ContactNumber":"18778975969"},
        /// {"AirlineId":24,"AirlineName":"WestJet Airlines Ltd.","Country":"Canada","Headquarters":"Calgary, Alberta, Canada","FounderName":"Clive Beddoe","FoundingYear":"1994-06-27T00:00:00","Website":"https://www.westjet.com/en-ca","ContactNumber":"18889378538"}]
        /// 
        ///     Search key example
        /// curl https://localhost:44379/api/AirlineData/ListAirlines/avianca
        /// [{"AirlineId":32,"AirlineName":"Avianca","Country":"Colombia","Headquarters":"Bogotá, Colombia","FounderName":"Ernesto Cortissoz","FoundingYear":"2024-02-07T00:00:00","Website":"https://www.avianca.com/es/","ContactNumber":"8007228222"}]
        /// 
        /// curl https://localhost:44379/api/AirlineData/ListAirlines/ET
        /// [{"AirlineId":24,"AirlineName":"WestJet Airlines Ltd.","Country":"Canada","Headquarters":"Calgary, Alberta, Canada","FounderName":"Clive Beddoe","FoundingYear":"1994-06-27T00:00:00","Website":"https://www.westjet.com/en-ca","ContactNumber":"18889378538"},
        /// {"AirlineId":27,"AirlineName":"JetBlue\r\n","Country":"United States\r\n","Headquarters":"Brewster Building Long Island City, New York City, U.S.\r\n","FounderName":"David Neeleman\r\n","FoundingYear":"1998-08-01T00:00:00","Website":"https://www.jetblue.com/\r\n","ContactNumber":"18558385841\r\n"}]
        /// </returns>
        [HttpGet]
        [Route("api/AirlineData/ListAirlines/{AirlineSearch?}")]
        public IEnumerable<AirlineDto> ListAirlines(string AirlineSearch = null)
        {
            //sending a query to the database

            List<Airline> Airlines = new List<Airline>();
            /*if the search is not given == null, the API will return all the Airlines in the system */
            if (AirlineSearch == null)
            {
                Airlines = db.Airlines.ToList();
            }
            /* if the search is different than null means that the user provide a string to filter the Airlines 
            when this occurs the API will look for the AirlineName that contains the AirlineSearch parameter. The
            ToLower was used in order to make it case insensitive*/
            else
            {
                Airlines = db.Airlines.Where(a => a.AirlineName.ToLower().Contains(AirlineSearch.ToLower())).ToList();
            }
            List<AirlineDto> AirlinesDtos = new List<AirlineDto>();

            Airlines.ForEach(a => AirlinesDtos.Add(new AirlineDto()
            {
                AirlineId = a.AirlineId,
                AirlineName = a.AirlineName,
                Country = a.Country,
                Headquarters = a.Headquarters,
                FounderName = a.FounderName,
                FoundingYear = a.FoundingYear,
                Website = a.Website,
                ContactNumber = a.ContactNumber
            }));

            return AirlinesDtos;
        }

        /// <summary>
        /// This GET method returns an individual airline from the database by specifying the primary key AirlineId
        /// </summary>
        /// <example>
        /// Using browser => GET: api/AirlineData/FindAirline/24
        /// 
        /// Using curl comands in the terminal => curl https://localhost:44379/api/AirlineData/FindAirline/24
        /// </example>
        /// <param name="id">This is the AirlineId parameter you want to search for.</param>
        /// <returns>
        /// A AirlineDto object which represent the airline with the id given 
        /// 
        /// curl https://localhost:44379/api/AirlineData/FindAirline/24
        /// {"AirlineId":24,"AirlineName":"WestJet Airlines Ltd.","Country":"Canada","Headquarters":"Calgary, Alberta, Canada","FounderName":"Clive Beddoe","FoundingYear":"1994-06-27T00:00:00","Website":"https://www.westjet.com/en-ca","ContactNumber":"18889378538"}
        /// 
        /// curl https://localhost:44379/api/AirlineData/FindAirline/28
        /// {"AirlineId":28,"AirlineName":"Air Canada","Country":"Canada","Headquarters":"Saint-Laurent, Quebec, Canada","FounderName":"Canadian National Railway","FoundingYear":"1937-04-10T00:00:00","Website":"https://www.aircanada.com/ca/en/aco/home.html","ContactNumber":"18882472262"}
        /// 
        /// </returns>
        [ResponseType(typeof(Airline))]
        [HttpGet]
        //[Route("api/AirlineData/FindAirline/{id}")]
        public IHttpActionResult FindAirline(int id)
        {
            Airline Airline = db.Airlines.Find(id);

            if (Airline == null)
            {
                return NotFound();
            }

            AirlineDto AirlineDto = new AirlineDto()
            {
                AirlineId = Airline.AirlineId,
                AirlineName = Airline.AirlineName,
                Country = Airline.Country,
                Headquarters = Airline.Headquarters,
                FounderName = Airline.FounderName,
                FoundingYear = Airline.FoundingYear,
                Website = Airline.Website,
                ContactNumber = Airline.ContactNumber
            };


            return Ok(AirlineDto);
        }


        // POST: api/AirlineData/DeleteAirline/8
        //

        /// <summary>
        /// This method is responsible for deleting a airline record in the Airlines table, the deleted record will be according to the value indicated in the id parameter
        /// </summary>
        /// <example>
        /// Because this method is POST, you must use the curl command in the terminal to delete the record
        /// 
        /// POST: api/AirlineData/DeleteAirline/31
        /// curl -d "" https://localhost:44379/api/AirlineData/DeleteAirline/31
        /// </example>
        /// <param name="id">This will indicate the AirlineId value that you want to eliminate</param>
        /// <returns>
        /// curl -d "" https://localhost:44379/api/AirlineData/DeleteAirline/31
        /// This method will not return anything on the console, it will delete a airline from the DB, in this case the record with the AirlineId = 31 will be deleated
        /// </returns>
        [ResponseType(typeof(Airline))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        //[Route("api/AirlineData/DeleteAirline/{id}")]
        public IHttpActionResult DeleteAirline(int id)
        {
            Airline Airline = db.Airlines.Find(id);
            if (Airline == null)
            {
                return NotFound();
            }

            db.Airlines.Remove(Airline);
            db.SaveChanges();

            return Ok();
        }


        /// <summary>
        /// This method is responsible for receiving a new Airline record. For this, the Airline type object must be sent. In order to be able to test this method, 
        /// you must open the folder where the airline.json is located
        /// </summary>
        /// <example>
        /// curl -d @airline.json -H "Content-type: application/json" https://localhost:44379/api/AirlineData/AddAirline
        /// The airline.json file contains the airline information that will be added to the DB.
        /// </example>
        /// <param name="Airline">This is the json object that contains the information of the airline to add.</param>
        /// <returns>
        /// {
        ///     "AirlineId":9,
        ///     "AirlineName":"American",
        ///     "Country":"United States",
        ///     "Headquarters":"Fort Worth, Texas, U.S.",
        ///     "FounderName":"E.L. Cord and C. R. Smith",
        ///     "FoundingYear":"1926-04-15T00:00:00",
        ///     "Website":"https://www.aa.com/homePage.do",
        ///     "ContactNumber":"+18004337300"
        ///}
        /// </returns>
        [ResponseType(typeof(Airline))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        //[Route("api/AirlineData/AddAirline")]
        public IHttpActionResult AddAirline(Airline Airline)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Airlines.Add(Airline);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Airline.AirlineId }, Airline);
        }

        // POST: api/AirlineData/UpdateAirline/14
        // open the folder where the airline.json is located
        // 


        /// <summary>
        /// This POST method will be in charge of updating the record according to the id parameter that is provided, for this both the value that is delivered by the url (id) 
        /// and the value that the AirlineId (within the object received in Airline) have to match. otherwise the record is not updated
        /// additionally, because this method is post to check its operation you must use the curl command in the terminal. for this open the folder where the airline.json is located on the terminal screen
        /// 
        /// once you have the folder open you can use this command curl -d @airline.json -H "Content-type: application/json" https://localhost:44379/api/AirlineData/UpdateAirline/31
        /// </summary>
        /// <example>
        /// curl -d @airline.json -H "Content-type: application/json" https://localhost:44379/api/AirlineData/UpdateAirline/31
        /// </example>
        /// <param name="id">The value of the AirlineId to update</param>
        /// <param name="Airline">The Airline object, this parameter holds the new data, with this data the method will update the specified Airline</param>
        /// <returns>
        /// This method will not return anything on the console, but the method will return whether the update was successful or not, in case it was not successful because there was no id match (id != Airline.AirlineId)
        /// The values of the Airline object and the id sent in the url are printed in the console. If the method runs, without problem, the record will be updated.
        /// </returns>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult UpdateAirline(int id, Airline Airline)
        {
            Debug.WriteLine("Update Airline method starts here");
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model State is invalid");
                return BadRequest(ModelState);
            }

            if (id != Airline.AirlineId)
            {
                Debug.WriteLine("ID mismatch");
                Debug.WriteLine("GET parameter " + id);
                Debug.WriteLine("POST parameter " + Airline.AirlineId);
                Debug.WriteLine("POST parameter " + Airline.AirlineName);
                Debug.WriteLine("POST parameter " + Airline.Country);
                Debug.WriteLine("POST parameter " + Airline.Headquarters);
                Debug.WriteLine("POST parameter " + Airline.FounderName);
                Debug.WriteLine("POST parameter " + Airline.FoundingYear);
                Debug.WriteLine("POST parameter " + Airline.Website);
                Debug.WriteLine("POST parameter " + Airline.ContactNumber);
                return BadRequest();
            }

            db.Entry(Airline).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AirlineExists(id))
                {
                    Debug.WriteLine("Airline not found");
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
        /// This method is responsible for returning a bool indicating if the airline exists
        /// </summary>
        /// <param name="id">The value of the AirlineId to update</param>
        /// <returns>A bool indicating if the record exists</returns>
        private bool AirlineExists(int id)
        {
            return db.Airlines.Count(e => e.AirlineId == id) > 0;
        }
    }
}
