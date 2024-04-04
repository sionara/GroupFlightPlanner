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
    public class LocationDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns a list of locations in db
        /// </summary>
        /// <returns>
        /// List of Location data
        /// </returns>
        // GET: api/LocationData/ListLocations
        [HttpGet]
        public IEnumerable<LocationDto> ListLocations()
        {
            List<Location> Locations = db.Locations.ToList();
            List<LocationDto> locationDtos = new List<LocationDto>();

            Locations.ForEach(l => locationDtos.Add(new LocationDto()
            {
                LocationId = l.LocationId,
                LocationName = l.LocationName,
                LocationAddress = l.LocationAddress
            }));
            Debug.WriteLine(locationDtos);

            return locationDtos;
        }

        /// <summary>
        /// Gather information on Locations related to a flight, this Get method will find all the locations that were associated with a flight
        /// this method is in charge of lopping the database and look for any location that contains flights taht match with the flight id that will be providadded in the url
        /// </summary>
        /// <example>
        /// Using browser => GET: api/LocationData/ListLocationsForFlight/1
        /// 
        /// Using curl comands in the terminal => curl https://localhost:44380/api/LocationData/ListLocationsForFlight/1
        /// </example>
        /// <param name="id">This is the id of the flight that we are looking for </param>
        /// <returns>
        /// Return a list of the locations that are associated to the flight 
        /// </returns>
        [HttpGet]
        [ResponseType(typeof(LocationDto))]
        public IHttpActionResult ListLocationsForFlight(int id)
        {
            //all locations that have flight that match with the ID provided
            List<Location> Locations = db.Locations.Where(
                l => l.Flights.Any(
                    f => f.FlightId == id
                )).ToList();

            List<LocationDto> LocationDtos = new List<LocationDto>();

            Locations.ForEach(l => LocationDtos.Add(new LocationDto()
            {
                LocationId = l.LocationId,
                LocationName = l.LocationName,
                LocationAddress = l.LocationAddress
            }));


            return Ok(LocationDtos);
        }

        /// <summary>
        /// This is a POST method. Its function is to associate a flight to a location, the flight will be one of the flighst taht are not associated.
        /// In order to work, this method requier 2 different parameters, the location id that will let the logic know which location will receive the association of a fligh and the flight id 
        /// that will be the flight to associate
        /// </summary>
        /// <example>
        /// POST api/LocationData/AssociateLocationWithFlight/1/1
        /// </example>
        /// <param name="locationid">The location ID primary key</param>
        /// <param name="flightid">The flight ID primary key</param>
        /// <returns>
        /// it will return a 200(OK) if the association is successful or a 404 (Not Found) when the API does not found the id of the location or flight
        /// </returns>
        [HttpPost]
        [Route("api/LocationData/AssociateLocationWithFlight/{locationid}/{flightid}")]
        public IHttpActionResult AssociateLocationWithFlight(int locationid, int flightid)
        {

            Location SelectedLocation = db.Locations.Include(l => l.Flights).Where(l => l.LocationId == locationid).FirstOrDefault();
            Flight SelectedFlight = db.Flights.Find(flightid);

            if (SelectedLocation == null || SelectedFlight == null)
            {
                return NotFound();
            }

            SelectedLocation.Flights.Add(SelectedFlight);
            db.SaveChanges();

            return Ok();
        }


        /// <summary>
        /// This is a POST method. Its function is to unassociate a flight that is associated to a location.
        /// In order to work, this method requier 2 different parameters, the location id that will let the logic know which location will remoce the association of a fligh and the flight id 
        /// that will be the flight to unassociate
        /// </summary>
        /// <example>
        /// POST api/LocationData/UnAssociateLocationWithFlight/1/1
        /// </example>
        /// <param name="locationid">The location ID primary key</param>
        /// <param name="flightid">The flight ID primary key</param>
        /// <returns>
        /// it will return a 200(OK) if the unassociation is successful or a 404 (Not Found) when the API does not found the id of the location or flight
        /// </returns>
        [HttpPost]
        [Route("api/LocationData/UnAssociateLocationWithFlight/{locationid}/{flightid}")]
        public IHttpActionResult UnAssociateLocationWithFlight(int locationid, int flightid)
        {

            Location SelectedLocation = db.Locations.Include(l => l.Flights).Where(l => l.LocationId == locationid).FirstOrDefault();
            Flight SelectedFlight = db.Flights.Find(flightid);

            if (SelectedLocation == null || SelectedFlight == null)
            {
                return NotFound();
            }

            SelectedLocation.Flights.Remove(SelectedFlight);
            db.SaveChanges();

            return Ok();
        }


        /// <summary>
        /// Returns a particular location based on id
        /// </summary>
        /// <param name="id">Id of a location</param>
        /// <returns>
        /// NotFound message or data of a location with id = {id}
        /// </returns>
        // GET: api/LocationData/FindLocation/5
        [ResponseType(typeof(LocationDto))]
        [HttpGet]
        public IHttpActionResult FindLocation(int id)
        {
            Location location = db.Locations.Find(id);
            LocationDto locationDto = new LocationDto()
            {
                LocationId = location.LocationId,
                LocationName = location.LocationName,
                LocationAddress = location.LocationAddress
            };
            if (location == null)
            {
                return NotFound();
            }

            return Ok(locationDto);
        }

        /// <summary>
        /// Updates the data of a location with id = {id} based on user input
        /// </summary>
        /// <param name="id">Event Id</param>
        /// <param name="location"> JSON data of an event </param>
        /// <returns>
        /// 204 (success, no response) 
        /// BAD REQUESST (400)
        /// or NOT FOUND (404) response.
        /// </returns>
        // POST: api/LocationData/UpdateLocation/5
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateLocation(int id, Location location)
        {
            Debug.WriteLine("Method is reached.");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != location.LocationId)
            {
                return BadRequest();
            }

            db.Entry(location).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Adds new location into the database
        /// </summary>
        /// <param name="location">JSON data of a location</param>
        /// <returns>
        /// 201 (created)
        /// or 400 (Bad request)
        /// response
        /// </returns>
        // POST: api/LocationData/AddLocation
        [ResponseType(typeof(Location))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddLocation(Location location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Locations.Add(location);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = location.LocationId }, location);
        }

        /// <summary>
        /// Delets a location from db with id = {id}
        /// </summary>
        /// <param name="id">location id</param>
        /// <returns>
        /// 404 not found
        /// or 200 OK
        /// </returns>
        /// 
        // POST: api/LocationData/DeleteLocation/5
        [ResponseType(typeof(Location))]
        [HttpPost]
        [Authorize]

        public IHttpActionResult DeleteLocation(int id)
        {
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return NotFound();
            }

            db.Locations.Remove(location);
            db.SaveChanges();

            return Ok(location);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LocationExists(int id)
        {
            return db.Locations.Count(e => e.LocationId == id) > 0;
        }
    }
}
