using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupFlightPlanner.Models.ViewModels
{
    public class DetailsAirline
    {

        public bool IsAdmin { get; set; }
        public AirlineDto SelectedAirline { get; set; }
        public IEnumerable<FlightDto> RelatedFlights { get; set; }
        public IEnumerable<FlightDto> RelatedAirplanes { get; set; }

    }
}