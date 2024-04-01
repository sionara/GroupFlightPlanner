using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupFlightPlanner.Models.ViewModels
{
    public class AddFlight
    {
        //This ViewModel is a class which stores information that we need to present to /Flight/New
        //this will the user to prive the name or model of the diferent options for the Airle and Airplane instead of IDs

        //Include all airlines and airplanes to choose from when adding flight
        public IEnumerable<AirlineDto> AirlinesOptions { get; set; }
        public IEnumerable<AirplaneDto> AirplanesOptions { get; set; }
    }
}