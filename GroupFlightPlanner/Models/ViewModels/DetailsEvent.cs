using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupFlightPlanner.Models.ViewModels
{
    public class DetailsEvent
    {
        public EventDto SelectedEvent { get; set; }
        public IEnumerable<GroupDto> ResponsibleGroups { get; set; }

        public IEnumerable<GroupDto> AvailableGroups { get; set; }
    }
}