using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupFlightPlanner.Models.ViewModels
{
    public class DetailsLocation
    {
        public LocationDto selectedLocation { get; set; }

        public IEnumerable<EventDto> hostedEvents { get; set; }

        //the related flights to a location
        public IEnumerable<FlightDto> RelatedFlights { get; set; }

        //List of flights that are not associated to alocation
        public IEnumerable<FlightDto> AvailableFlights { get; set; }
    }
}