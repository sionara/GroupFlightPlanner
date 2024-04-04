using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupFlightPlanner.Models.ViewModels
{
    public class DetailsFlight
    {
        //This ViewModel is a class which stores information that we need to present to /Flight/Details/{id}

        //1. The existing flight information
        public FlightDto SelectedFlight { get; set; }

        //2. The variable which will store the Flight duration
        public string FlightDuration { get; set; }

        //the related locations to a flight
        public IEnumerable<LocationDto> RelatedLocations { get; set; }
    }
}