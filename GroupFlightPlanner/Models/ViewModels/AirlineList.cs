using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupFlightPlanner.Models.ViewModels
{
    public class AirlineList
    {

        //provide the page information in the ailine list

        public bool IsAdmin { get; set; }
        public IEnumerable<AirlineDto> Airlines { get; set; }
    }
}