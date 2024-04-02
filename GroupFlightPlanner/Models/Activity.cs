using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GroupFlightPlanner.Models
{
    public class Activity
    {
        [Key]
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }

        // a activity has many groups participating
        public ICollection<Group> Groups { get; set; }
    }
    public class ActivityDto
    {
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
    }
}