using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GroupFlightPlanner.Models
{
    public class Organization
    {
        [Key]
        public int OrganizationId { get; set; }

        public string OrganizationName { get; set; }

        public string OrganizationContact { get; set; }
    }

    public class OrganizationDto
    {
        public int OrganizationId { get; set; }

        public string OrganizationName { get; set; }

        public string OrganizationContact { get; set; }
    }
}