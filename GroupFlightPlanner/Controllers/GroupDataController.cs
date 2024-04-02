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
    public class GroupDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // List Groups
        [HttpGet]
        [Route("api/GroupData/ListGroups")]
        public List<GroupDto> ListGroups()
        {
            List<Group> Groups = db.Groups.ToList();
            List<GroupDto> GroupDtos = new List<GroupDto>();

            Groups.ForEach(b => GroupDtos.Add(new GroupDto()
            {
                GroupId = b.GroupId,
                GroupName = b.GroupName,
                Description = b.Description,

            }));
            return GroupDtos;
        }

        /// <summary>
        /// Gathers information about groups related to a particular activity
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all groups in the database that match to a particular activity id
        /// </returns>
        /// <param name="id">Activity Id.</param>
        /// <example>
        /// GET: api/GroupData/ListGroupsForActivity/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(GroupDto))]

        public IHttpActionResult ListGroupsForActivity(int id)
        {
            //all activities that have groups which match with our ID
            List<Group> Groups = db.Groups.Where(
                g => g.Activities.Any(
                    a => a.ActivityId == id
                )).ToList();
            List<GroupDto> GroupDtos = new List<GroupDto>();

            Groups.ForEach(b => GroupDtos.Add(new GroupDto()
            {
                GroupId = b.GroupId,
                GroupName = b.GroupName,
                Description = b.Description,

            }));

            return Ok(GroupDtos);
        }

        /// <summary>
        /// Returns Groups in the system not joining in a particular activity.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all groups in the database not joining in a particular activity
        /// </returns>
        /// <param name="id">Activity Primary Key</param>
        /// <example>
        /// GET: api/GroupData/ListGroupsNotJoinInActivity/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(GroupDto))]
        public IHttpActionResult ListGroupsNotJoinInActivity(int id)
        {
            List<Group> Groups = db.Groups.Where(
               g => !g.Activities.Any(
                   a => a.ActivityId == id
               )).ToList();
            List<GroupDto> GroupDtos = new List<GroupDto>();

            Groups.ForEach(b => GroupDtos.Add(new GroupDto()
            {
                GroupId = b.GroupId,
                GroupName = b.GroupName,
                Description = b.Description,

            }));

            return Ok(GroupDtos);
        }
        // Find Group
        // GET: api/GroupData/FindGroup/2


        [HttpGet]
        [ResponseType(typeof(GroupDto))]
        public IHttpActionResult FindGroup(int id)
        {
            {
                Group Group = db.Groups.Find(id);
                GroupDto GroupDto = new GroupDto()
                {
                    GroupId = Group.GroupId,
                    GroupName = Group.GroupName,
                    Description = Group.Description,
                };
                if (Group == null)
                {
                    return NotFound();
                }
                return Ok(GroupDto);
            }
        }
        /// <summary>
        /// Adds a group to the system
        /// </summary>
        /// <param name="Group">JSON FORM DATA of an Group</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Group ID, Group Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/GroupData/AddGroup
        /// FORM DATA: Group JSON Object
        /// </example>
        [ResponseType(typeof(Group))]
        [HttpPost]
        public IHttpActionResult AddGroup(Group Group)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Groups.Add(Group);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Group.GroupId }, Group);
        }

        /// <summary>
        /// Updates a particular group in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the group ID primary key</param>
        /// <param name="Group">JSON FORM DATA of a Group</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/GroupData/UpdateGroup/5
        /// FORM DATA: Group JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateGroup(int id, Group Group)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Group.GroupId)
            {

                return BadRequest();
            }

            db.Entry(Group).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
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
        /// Deletes a group from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the group</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/GroupData/DeleteGroup/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Group))]
        [HttpPost]
        public IHttpActionResult DeleteGroup(int id)
        {
            Group Group = db.Groups.Find(id);
            if (Group == null)
            {
                return NotFound();
            }

            db.Groups.Remove(Group);
            db.SaveChanges();

            return Ok();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GroupExists(int id)
        {
            return db.Groups.Count(e => e.GroupId == id) > 0;
        }
    }
}
