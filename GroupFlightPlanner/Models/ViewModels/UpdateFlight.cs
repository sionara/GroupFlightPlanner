using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupFlightPlanner.Models.ViewModels
{
    public class UpdateFlight
    {
        //This ViewModel is a class which stores information that we need to present to /Flight/Edit/{id}

        //1. The existing flight information
        public FlightDto SelectedFlight { get; set; }

        //2. Include all airlines and airplanes to choose from when updating this flight
        public IEnumerable<AirlineDto> AirlinesOptions { get; set; }
        public IEnumerable<AirplaneDto> AirplanesOptions { get; set; }

    }
}