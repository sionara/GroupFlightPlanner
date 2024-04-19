using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupFlightPlanner.Models.ViewModels
{
    public class AirplaneList
    {
        public bool IsAdmin { get; set; }
        public IEnumerable<AirplaneDto> Airplanes { get; set; }
    }
}