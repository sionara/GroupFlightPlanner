using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupFlightPlanner.Models.ViewModels
{
    public class DetailsOrganization
    {
        public OrganizationDto selectedOrganization { get; set; }

        public IEnumerable<EventDto> hostedEvents { get; set; }
    }
}