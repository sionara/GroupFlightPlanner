using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GroupFlightPlanner.Models
{
    public class Airline
    {
        //what describe a Airline, the following fields define an Airline
        [Key]
        public int AirlineId { get; set; }

        [Required]
        public string AirlineName { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string Headquarters { get; set; }

        [Required]
        public string FounderName { get; set; }

        [Required]
        public DateTime FoundingYear { get; set; }

        [Required]
        public string Website { get; set; }

        [Required]
        public string ContactNumber { get; set; }
    }

    //Represent my own version of a Airline to serve in a webapi
    //Although the method will work if Data Transfer Objet is not used, the DTO will be used to be able to follow the knowledge learned in class

    public class AirlineDto
    {
        public int AirlineId { get; set; }

        public string AirlineName { get; set; }

        public string Country { get; set; }

        public string Headquarters { get; set; }

        public string FounderName { get; set; }

        public DateTime FoundingYear { get; set; }

        public string Website { get; set; }

        public string ContactNumber { get; set; }
    }
}