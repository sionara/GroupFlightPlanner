using GroupFlightPlanner.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace GroupFlightPlanner.Controllers
{
    public class VolunteerDataController : ApiController
    {
        // utilizing the database connection

        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all volunteres in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all volunteers in the database, including their associated groups.
        /// </returns>
        /// <example>
        /// GET: api/VolunteerData/ListVolunteers
        /// </example>
        [HttpGet]
        [Route("api/VolunteerData/ListVolunteers")]
        public List<VolunteerDto> ListVolunteers()
        {
            List<Volunteer> Volunteers = db.Volunteers.ToList();
            List<VolunteerDto> VolunteerDtos = new List<VolunteerDto>();

            Volunteers.ForEach(b => VolunteerDtos.Add(new VolunteerDto()
            {
                VolunteerId = b.VolunteerId,
                FirstName = b.FirstName,
                LastName = b.LastName,
                ChristianName = b.ChristianName,
                PhoneNumber = b.PhoneNumber,
                Email = b.Email,
                Address = b.Address,
                GroupId = b.GroupId,
                GroupName = b.Group.GroupName,

            }));
            return VolunteerDtos;
        }


        /// <summary>
        /// Gathers information about all volunteers related to a particular groups ID
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all volunteers in the database, including their associated groups matched with a particular groups Id
        /// </returns>
        /// <param name="id">Groups Id.</param>
        /// <example>
        /// GET: api/VolunteerData/ListVolunteersForGroups/3
        /// </example>
        [HttpGet]
        [ResponseType(typeof(VolunteerDto))]
        public List<VolunteerDto> ListVolunteersForGroups(int id)
        {
            List<Volunteer> Volunteers = db.Volunteers.Where(a => a.GroupId == id).ToList();
            List<VolunteerDto> VolunteerDtos = new List<VolunteerDto>();

            Volunteers.ForEach(b => VolunteerDtos.Add(new VolunteerDto()
            {
                VolunteerId = b.VolunteerId,
                FirstName = b.FirstName,
                LastName = b.LastName,
                ChristianName = b.ChristianName,
                PhoneNumber = b.PhoneNumber,
                Email = b.Email,
                Address = b.Address,
                GroupId = b.GroupId,
                GroupName = b.Group.GroupName,

            }));
            return VolunteerDtos;
        }


        // Find Volunteer
        // GET: api/VolunteerData/FindVolunteer/2


        [HttpGet]
        [ResponseType(typeof(VolunteerDto))]
        public IHttpActionResult FindVolunteer(int id)
        {
            {
                Volunteer Volunteer = db.Volunteers.Find(id);
                VolunteerDto VolunteerDto = new VolunteerDto()
                {
                    VolunteerId = Volunteer.VolunteerId,
                    FirstName = Volunteer.FirstName,
                    LastName = Volunteer.LastName,
                    ChristianName = Volunteer.ChristianName,
                    PhoneNumber = Volunteer.PhoneNumber,
                    Email = Volunteer.Email,
                    Address = Volunteer.Address,
                    GroupId = Volunteer.GroupId,
                    GroupName = Volunteer.Group.GroupName,
                };
                if (Volunteer == null)
                {
                    return NotFound();
                }
                return Ok(VolunteerDto);
            }
        }
        // Add Volunteer

        /// <summary>
        /// Adds a volunteer to the system
        /// </summary>
        /// <param name="Volunteer">JSON FORM DATA of a volunteer</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Voluntere ID, Volunteer Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/VolunteerData/AddVolunteer
        /// FORM DATA: Volunteer JSON Object
        /// </example>
        [ResponseType(typeof(Volunteer))]
        [HttpPost]

        public IHttpActionResult AddVolunteer(Volunteer Volunteer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Volunteers.Add(Volunteer);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Volunteer.VolunteerId }, Volunteer);
        }
        // UpdateVolunteer

        /// <summary>
        /// Updates a particular volunteer in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the volunteer ID primary key</param>
        /// <param name="Volunteer">JSON FORM DATA of an volunteer</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/VolunteerData/UpdateVolunteer/5
        /// FORM DATA: volunteer JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        //[Authorize]
        public IHttpActionResult UpdateVolunteer(int id, Volunteer Volunteer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Volunteer.VolunteerId)
            {

                return BadRequest();
            }

            db.Entry(Volunteer).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VolunteerExists(id))
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
        //DeleteVolunteer

        /// <summary>
        /// Deletes an volunteer from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the Volunteer</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/VolunteerData/DeleteVolunteer/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Volunteer))]
        [HttpPost]
        public IHttpActionResult DeleteVolunteer(int id)
        {
            Volunteer Volunteer = db.Volunteers.Find(id);
            if (Volunteer == null)
            {
                return NotFound();
            }

            db.Volunteers.Remove(Volunteer);
            db.SaveChanges();

            return Ok();
        }
        // related methods include:
        // ListVolunteerForGroup*/

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool VolunteerExists(int id)
        {
            return db.Volunteers.Count(e => e.VolunteerId == id) > 0;
        }
    }
}
