using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupFlightPlanner.Models.ViewModels
{
    public class UpdateVolunteer
    {
        //This viewmodel is a class which stores information that we need to present to /Volunteer/Update/{}

        //the existing volunteer information

        public VolunteerDto SelectedVolunteer { get; set; }

        // all groups to choose from when updating this volunteer

        public IEnumerable<GroupDto> GroupOptions { get; set; }
    }
}