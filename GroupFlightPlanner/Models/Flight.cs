using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GroupFlightPlanner.Models
{
    public class Flight
    {
        //the following fields define a Flight
        [Key]
        public int FlightId { get; set; }

        [Required]
        public string FlightNumber { get; set; }

        [Required]
        public string From { get; set; }

        [Required]
        public string To { get; set; }

        [Required]
        public string DepartureAirport { get; set; }

        [Required]
        public string DestinationAirport { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required]
        public Decimal TicketPrice { get; set; }

        [Required]
        public string TimeZoneFrom { get; set; }

        [Required]
        public string TimeZoneTo { get; set; }

        //Foreign key that refers the Airline entity
        //A Flight belongs to one Airline Company
        //An Airline Company can have many Flights
        [ForeignKey("Airline")]
        public int AirlineId { get; set; }
        public virtual Airline Airline { get; set; }

        //Foreign key that refers the Airplane entity
        //A Flight belongs to one Airplane
        //An Airplane can have many Flights
        [ForeignKey("Airplane")]
        public int AirplaneId { get; set; }
        public virtual Airplane Airplane { get; set; }

        //A flight can go to a specific city but that city is holding events in different locations (addresses)
        public ICollection<Location> Locations { get; set; }
    }

    public class FlightDto
    {
        public int FlightId { get; set; }

        public string FlightNumber { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string DepartureAirport { get; set; }

        public string DestinationAirport { get; set; }

        public DateTime DepartureTime { get; set; }

        public DateTime ArrivalTime { get; set; }

        public Decimal TicketPrice { get; set; }

        public string TimeZoneFrom { get; set; }

        public string TimeZoneTo { get; set; }

        public int AirlineId { get; set; }
        public string AirlineName { get; set; }

        public int AirplaneId { get; set; }
        public string AirplaneModel { get; set; }
        public string RegistrationNum { get; set; }
    }
}