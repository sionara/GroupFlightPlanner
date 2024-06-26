﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupFlightPlanner.Models.ViewModels
{
    public class DetailsAirplane
    {

        public bool IsAdmin { get; set; }
        public AirplaneDto SelectedAirplane { get; set; }
        public IEnumerable<FlightDto> RelatedFlights { get; set; }
    }
}