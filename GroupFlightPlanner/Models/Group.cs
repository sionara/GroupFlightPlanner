using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GroupFlightPlanner.Models
{
    public class Group
    {
        [Key]
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }

        // a group has many volunteers
        public ICollection<Volunteer> Volunteers { get; set; }

        // a group has many activities to join in
        public ICollection<Activity> Activities { get; set; }

        // a group has many events to participate in
        public ICollection<Event> Events { get; set; }
    }
    public class GroupDto
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
    }
}