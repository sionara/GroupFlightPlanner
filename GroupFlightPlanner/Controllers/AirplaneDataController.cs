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
    public class AirplaneDataController : ApiController
    {
        //utilizing the database connection
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// This is a GET method that will return a list of all the airplanes in the system. This method presents a foreach that will be responsible 
        /// for setting each of the AirplaneDto objects with the corresponding information. To do this, the information must first be collected, which will be a list of Airplane datatypes. 
        /// Once this is done, the foreach will be run to have each of the data types as AirplaneDto.
        /// 
        /// the extra feature was made for this entity so the list can filter when a user provides a value in the search bar, this value will be part of the URl cause the AirplaneSearch will store this value given
        /// </summary>
        /// <example>
        /// Using browser => GET: api/AirplaneData/ListAirplanes
        /// 
        /// Using curl comands in the terminal => curl https://localhost:44379/api/AirplaneData/ListAirplanes
        /// 
        /// Using browser with a search key => GET: api/AirplaneData/ListAirplanes
        /// 
        /// Using curl comands in the terminal => curl https://localhost:44379/api/AirplaneData/ListAirplanes
        /// </example>
        /// <param name="AirplaneSearch">This is the search key, the api will use it to look for this specific Airplane
        /// if the user does not enter a search key the GET method will display the list of all the Airplanes otherwise, it will display the 
        /// Airplanes that Contains the search key. Additionally, its case insensitive</param>
        /// <returns>
        /// A list of Airplanes in the system
        /// GET: api/AirplaneData/ListAirplanes =>
        /// [{"AirplaneId":24,"AirplaneModel":"Embraer E175SU","RegistrationNum":"C-FEKD","ManufacturerName":"Brazilian aerospace manufacturer Embraer","ManufactureYear":"2021-04-28T00:00:00","MaxPassenger":88,"EngineModel":"2 General Electric CF34-8E","Speed":870.00,"Range":3704.00},
        /// {"AirplaneId":25,"AirplaneModel":"Airbus A321-231","RegistrationNum":"N115NN","ManufacturerName":"Airbus S.A.S.","ManufactureYear":"2014-05-02T00:00:00","MaxPassenger":220,"EngineModel":"2 CFM Intl. CFM56-5B3/2P","Speed":904.00,"Range":5600.00},
        /// {"AirplaneId":28,"AirplaneModel":"Airbus A321-231","RegistrationNum":"N110AN","ManufacturerName":"Airbus S.A.S.","ManufactureYear":"2014-02-26T00:00:00","MaxPassenger":220,"EngineModel":"2 CFM Intl. CFM56-5B3/2P","Speed":904.00,"Range":5600.00}]
        /// 
        /// curl https://localhost:44379/api/AirplaneData/ListAirplanes
        /// [{"AirplaneId":24,"AirplaneModel":"Embraer E175SU","RegistrationNum":"C-FEKD","ManufacturerName":"Brazilian aerospace manufacturer Embraer","ManufactureYear":"2021-04-28T00:00:00","MaxPassenger":88,"EngineModel":"2 General Electric CF34-8E","Speed":870.00,"Range":3704.00},
        /// {"AirplaneId":25,"AirplaneModel":"Airbus A321-231","RegistrationNum":"N115NN","ManufacturerName":"Airbus S.A.S.","ManufactureYear":"2014-05-02T00:00:00","MaxPassenger":220,"EngineModel":"2 CFM Intl. CFM56-5B3/2P","Speed":904.00,"Range":5600.00},
        /// {"AirplaneId":28,"AirplaneModel":"Airbus A321-231","RegistrationNum":"N110AN","ManufacturerName":"Airbus S.A.S.","ManufactureYear":"2014-02-26T00:00:00","MaxPassenger":220,"EngineModel":"2 CFM Intl. CFM56-5B3/2P","Speed":904.00,"Range":5600.00}]
        /// 
        /// 
        ///     Search key example
        /// curl https://localhost:44379/api/AirplaneData/ListAirplanes/airbus
        /// [{"AirplaneId":25,"AirplaneModel":"Airbus A321-231\r\n","RegistrationNum":"N115NN\r\n","ManufacturerName":"Airbus S.A.S.\r\n","ManufactureYear":"2014-05-02T00:00:00","MaxPassenger":220,"EngineModel":"2 CFM Intl. CFM56-5B3/2P\r\n","Speed":904.00,"Range":5600.00},
        /// {"AirplaneId":28,"AirplaneModel":"Airbus A321-231\r\n","RegistrationNum":"N110AN\r\n","ManufacturerName":"Airbus S.A.S.\r\n","ManufactureYear":"2014-02-26T00:00:00","MaxPassenger":220,"EngineModel":"2 CFM Intl. CFM56-5B3/2P\r\n","Speed":904.00,"Range":5600.00},
        /// { "AirplaneId":35,"AirplaneModel":"Airbus A220-300\r\n","RegistrationNum":"C-GMZN\r\n","ManufacturerName":"Airbus S.A.S.\r\n","ManufactureYear":"2020-11-17T00:00:00","MaxPassenger":160,"EngineModel":"2 Pratt & Whitney PW1500G\r\n","Speed":871.00,"Range":6700.00}]
        /// </returns>
        [HttpGet]
        [Route("api/AirplaneData/ListAirplanes/{AirplaneSearch?}")]
        public IEnumerable<AirplaneDto> ListAirplanes(string AirplaneSearch = null)
        {
            //sending a query to the database

            List<Airplane> Airplanes = new List<Airplane>();
            /*if the search is not given == null, the API will return all the Airplanes in the system */
            if (AirplaneSearch == null)
            {
                Airplanes = db.Airplanes.ToList();
            }
            /* if the search is different than null means that the user provide a string to filter the Airplanes 
            when this occurs the API will look for the AirplaneModel that contains the AirplaneSearch parameter. The
            ToLower was used in order to make it case insensitive*/
            else
            {
                Airplanes = db.Airplanes.Where(a => a.AirplaneModel.ToLower().Contains(AirplaneSearch.ToLower())).ToList();
            }
            List<AirplaneDto> AirplanesDtos = new List<AirplaneDto>();

            Airplanes.ForEach(a => AirplanesDtos.Add(new AirplaneDto()
            {
                AirplaneId = a.AirplaneId,
                AirplaneModel = a.AirplaneModel,
                RegistrationNum = a.RegistrationNum,
                ManufacturerName = a.ManufacturerName,
                ManufactureYear = a.ManufactureYear,
                MaxPassenger = a.MaxPassenger,
                EngineModel = a.EngineModel,
                Speed = a.Speed,
                Range = a.Range
            }));

            return AirplanesDtos;
        }

        /// <summary>
        /// This GET method returns an individual airplane from the database by specifying the primary key AirplaneId
        /// </summary>
        /// <example>
        /// Using browser => GET: api/AirplaneData/FindAirplane/24
        /// 
        /// Using curl comands in the terminal => curl https://localhost:44379/api/AirplaneData/FindAirplane/24
        /// </example>
        /// <param name="id">This is the AirplaneId parameter you want to search for.</param>
        /// <returns>
        /// A AirplaneDto object which represent the airplane with the id given 
        /// 
        /// curl https://localhost:44379/api/AirplaneData/FindAirplane/24
        /// {"AirplaneId":24,"AirplaneModel":"Embraer E175SU","RegistrationNum":"C-FEKD","ManufacturerName":"Brazilian aerospace manufacturer Embraer","ManufactureYear":"2021-04-28T00:00:00","MaxPassenger":88,"EngineModel":"2 General Electric CF34-8E","Speed":870.00,"Range":3704.00}
        /// 
        /// curl https://localhost:44379/api/AirplaneData/FindAirplane/33
        /// {"AirplaneId":33,"AirplaneModel":"Saab 340B","RegistrationNum":"C-GOIA","ManufacturerName":"Saab AB and Fairchild Aircraft","ManufactureYear":"2023-02-10T00:00:00","MaxPassenger":37,"EngineModel":"2 General Electric CT7-9B","Speed":522.00,"Range":1685.00}
        /// </returns>
        [ResponseType(typeof(Airplane))]
        [HttpGet]
        public IHttpActionResult FindAirplane(int id)
        {
            Airplane Airplane = db.Airplanes.Find(id);

            if (Airplane == null)
            {
                return NotFound();
            }

            AirplaneDto AirplaneDto = new AirplaneDto()
            {
                AirplaneId = Airplane.AirplaneId,
                AirplaneModel = Airplane.AirplaneModel,
                RegistrationNum = Airplane.RegistrationNum,
                ManufacturerName = Airplane.ManufacturerName,
                ManufactureYear = Airplane.ManufactureYear,
                MaxPassenger = Airplane.MaxPassenger,
                EngineModel = Airplane.EngineModel,
                Speed = Airplane.Speed,
                Range = Airplane.Range
            };


            return Ok(AirplaneDto);
        }


        /// <summary>
        /// This method is responsible for deleting a airplane record in the Airplanes table, the deleted record will be according to the value indicated in the id parameter
        /// </summary>
        /// <example>
        /// Because this method is POST, you must use the curl command in the terminal to delete the record
        /// 
        /// POST: api/AirplaneData/DeleteAirplane/36
        /// curl -d "" https://localhost:44379/api/AirplaneData/DeleteAirplane/36
        /// </example>
        /// <param name="id">This will indicate the AirplaneId value that you want to eliminate</param>
        /// <returns>
        /// curl -d "" https://localhost:44379/api/AirplaneData/DeleteAirplane/36
        /// This method will not return anything on the console, it will delete a airplane from the DB, in this case the record with the AirplaneId = 36 will be deleated
        /// </returns>
        [ResponseType(typeof(Airplane))]
        [HttpPost]
        public IHttpActionResult DeleteAirplane(int id)
        {
            Airplane Airplane = db.Airplanes.Find(id);
            if (Airplane == null)
            {
                return NotFound();
            }

            db.Airplanes.Remove(Airplane);
            db.SaveChanges();

            return Ok();
        }


        /// <summary>
        /// This method is responsible for receiving a new Airplane record. For this, the Airplane type object must be sent. In order to be able to test this method, 
        /// you must open the folder where the airplane.json is located
        /// </summary>
        /// <example>
        /// curl -d @airplane.json -H "Content-type: application/json" https://localhost:44379/api/AirplaneData/AddAirplane
        /// The airplane.json file contains the airplane information that will be added to the DB.
        /// </example>
        /// <param name="Airplane">This is the json object that contains the information of the airplane to add.</param>
        /// <returns>
        /// {
        ///     "AirplaneId":16,
        ///     "AirplaneModel":"Airbus A220-300",
        ///     "RegistrationNum":"C-GMZN",
        ///     "ManufacturerName":"Airbus S.A.S.",
        ///     "ManufactureYear":"2020-10-01T00:00:00",
        ///     "MaxPassenger":160,
        ///     "EngineModel":"2 Pratt & Whitney PW1500G",
        ///     "Speed":871.00,
        ///     "Range":6700.00
        ///}
        /// </returns>
        [ResponseType(typeof(Airplane))]
        [HttpPost]
        public IHttpActionResult AddAirplane(Airplane Airplane)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Airplanes.Add(Airplane);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Airplane.AirplaneId }, Airplane);
        }

        /// <summary>
        /// This POST method will be in charge of updating the record according to the id parameter that is provided, for this both the value that is delivered by the url (id) 
        /// and the value that the AirplaneId (within the object received in Airplane) have to match. otherwise the record is not updated
        /// additionally, because this method is post to check its operation you must use the curl command in the terminal. for this open the folder where the airplane.json is located on the terminal screen
        /// 
        /// once you have the folder open you can use this command curl -d @airplane.json -H "Content-type: application/json" https://localhost:44379/api/AirplaneData/UpdateAirplane/36
        /// </summary>
        /// <example>
        /// curl -d @airplane.json -H "Content-type: application/json" https://localhost:44379/api/AirplaneData/UpdateAirplane/36
        /// </example>
        /// <param name="id">The value of the AirplaneId to update</param>
        /// <param name="Airplane">The Airplane object, this parameter holds the new data, with this data the method will update the specified Airplane</param>
        /// <returns>
        /// This method will not return anything on the console, but the method will return whether the update was successful or not, in case it was not successful because there was no id match (id != Airplane.AirplaneId)
        /// The values of the Airplane object and the id sent in the url are printed in the console. If the method runs, without problem, the record will be updated.
        /// </returns>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateAirplane(int id, Airplane Airplane)
        {
            Debug.WriteLine("Update Airplane method starts here");
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model State is invalid");
                return BadRequest(ModelState);
            }

            if (id != Airplane.AirplaneId)
            {
                Debug.WriteLine("ID mismatch");
                Debug.WriteLine("GET parameter " + id);
                Debug.WriteLine("POST parameter " + Airplane.AirplaneId);
                Debug.WriteLine("POST parameter " + Airplane.AirplaneModel);
                Debug.WriteLine("POST parameter " + Airplane.RegistrationNum);
                Debug.WriteLine("POST parameter " + Airplane.ManufacturerName);
                Debug.WriteLine("POST parameter " + Airplane.ManufactureYear);
                Debug.WriteLine("POST parameter " + Airplane.MaxPassenger);
                Debug.WriteLine("POST parameter " + Airplane.EngineModel);
                Debug.WriteLine("POST parameter " + Airplane.Speed);
                Debug.WriteLine("POST parameter " + Airplane.Range);
                return BadRequest();
            }

            db.Entry(Airplane).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AirplaneExists(id))
                {
                    Debug.WriteLine("Airplane not found");
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
        /// This method is responsible for returning a bool indicating if the airplane exists
        /// </summary>
        /// <param name="id">The value of the AirplaneId to update</param>
        /// <returns>A bool indicating if the record exists</returns>
        private bool AirplaneExists(int id)
        {
            return db.Airplanes.Count(e => e.AirplaneId == id) > 0;
        }
    }
}
