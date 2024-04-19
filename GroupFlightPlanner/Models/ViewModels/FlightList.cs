using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupFlightPlanner.Models.ViewModels
{
    public class FlightList
    {
        public bool IsAdmin { get; set; }
        public IEnumerable<FlightDto> Flights { get; set; }
    }
}