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
    public class ActivityDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // List Activities
        [HttpGet]
        [Route("api/ActivityData/ListActivities")]
        public List<ActivityDto> ListActivities()
        {
            List<Activity> Activities = db.Activities.ToList();
            List<ActivityDto> ActivityDtos = new List<ActivityDto>();

            Activities.ForEach(b => ActivityDtos.Add(new ActivityDto()
            {
                ActivityId = b.ActivityId,
                ActivityName = b.ActivityName,
                Description = b.Description,
                DateTime = b.DateTime,

            }));
            return ActivityDtos;
        }
        /// <summary>
        /// Gathers information about activities related to a particular group
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all activities in the database that match to a particular group id
        /// </returns>
        /// <param name="id">Group Id.</param>
        /// <example>
        /// GET: api/ActivityData/ListActivitiesForGroup/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(ActivityDto))]

        public IHttpActionResult ListActivitiesForGroup(int id)
        {
            //all activities that have groups which match with our ID
            List<Activity> Activities = db.Activities.Where(
                a => a.Groups.Any(
                    g => g.GroupId == id
                )).ToList();
            List<ActivityDto> ActivityDtos = new List<ActivityDto>();

            Activities.ForEach(a => ActivityDtos.Add(new ActivityDto()
            {
                ActivityId = a.ActivityId,
                ActivityName = a.ActivityName,
                Description = a.Description,
                DateTime = a.DateTime,
            }));

            return Ok(ActivityDtos);
        }

        /// <summary>
        /// Associates a particular group with a particular activity
        /// </summary>
        /// <param name="activityid">The activity ID primary key</param>
        /// <param name="groupid">The group ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/ActivityData/AssociateActivityWithGroup/9/1
        /// </example>
        [HttpPost]
        [Route("api/ActivityData/AssociateActivityWithGroup/{activityid}/{groupid}")]

        public IHttpActionResult AssociateActivityWithGroup(int activityid, int groupid)
        {

            Activity SelectedActivity = db.Activities.Include(a => a.Groups).Where(a => a.ActivityId == activityid).FirstOrDefault();
            Group SelectedGroup = db.Groups.Find(groupid);

            if (SelectedActivity == null || SelectedGroup == null)
            {
                return NotFound();
            }

            Debug.WriteLine("input activity id is: " + activityid);
            Debug.WriteLine("selected activity name is: " + SelectedActivity.ActivityName);
            Debug.WriteLine("input group id is: " + groupid);
            Debug.WriteLine("selected group name is: " + SelectedGroup.GroupName);


            SelectedActivity.Groups.Add(SelectedGroup);
            db.SaveChanges();

            return Ok();
        }
        /// <summary>
        /// Removes an association between a particular group and a particular activity
        /// </summary>
        /// <param name="activityid">The activity ID primary key</param>
        /// <param name="groupid">The group ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/ActivityData/AssociateActivityWithGroup/9/1
        /// </example>
        [HttpPost]
        [Route("api/ActivityData/UnAssociateActivityWithGroup/{activityid}/{groupid}")]

        public IHttpActionResult UnAssociateActivityWithGroup(int activityid, int groupid)
        {

            Activity SelectedActivity = db.Activities.Include(a => a.Groups).Where(a => a.ActivityId == activityid).FirstOrDefault();
            Group SelectedGroup = db.Groups.Find(groupid);

            if (SelectedActivity == null || SelectedGroup == null)
            {
                return NotFound();
            }

            Debug.WriteLine("input activity id is: " + activityid);
            Debug.WriteLine("selected activity name is: " + SelectedActivity.ActivityName);
            Debug.WriteLine("input group id is: " + groupid);
            Debug.WriteLine("selected group name is: " + SelectedGroup.GroupName);


            SelectedActivity.Groups.Remove(SelectedGroup);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Returns all activities in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: An activity in the system matching up to the activity ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the activity</param>
        /// <example>
        /// GET: api/ActivityData/FindActivity/5
        /// </example>
        [ResponseType(typeof(ActivityDto))]
        [HttpGet]
        public IHttpActionResult FindActivity(int id)
        {
            Activity Activity = db.Activities.Find(id);
            ActivityDto ActivityDto = new ActivityDto()
            {
                ActivityId = Activity.ActivityId,
                ActivityName = Activity.ActivityName,
                Description = Activity.Description,
                DateTime = Activity.DateTime
            };
            if (Activity == null)
            {
                return NotFound();
            }

            return Ok(ActivityDto);
        }

        /// <summary>
        /// Adds an activity to the system
        /// </summary>
        /// <param name="activity">JSON FORM DATA of an animal</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Activity ID, Activity Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/ActivityData/AddActivity
        /// FORM DATA: Activity JSON Object
        /// </example>
        [ResponseType(typeof(Activity))]
        [HttpPost]
        //[Authorize]
        public IHttpActionResult AddActivity(Activity activity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Activities.Add(activity);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = activity.ActivityId }, activity);
        }

        /// <summary>
        /// Updates a particular activity in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Activity ID primary key</param>
        /// <param name="activity">JSON FORM DATA of an activity</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/ActivityData/UpdateActivity/5
        /// FORM DATA: Animal JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        //[Authorize]
        public IHttpActionResult UpdateActivity(int id, Activity activity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != activity.ActivityId)
            {

                return BadRequest();
            }

            db.Entry(activity).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityExists(id))
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
        /// Deletes a activity from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the activity</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/ActivityData/DeleteActivity/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Activity))]
        [HttpPost]
        public IHttpActionResult DeleteActivity(int id)
        {
            Activity Activity = db.Activities.Find(id);
            if (Activity == null)
            {
                return NotFound();
            }

            db.Activities.Remove(Activity);
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

        private bool ActivityExists(int id)
        {
            return db.Activities.Count(e => e.ActivityId == id) > 0;
        }
    }
}
