using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GroupFlightPlanner.Models
{
    public class Airplane
    {
        //the following fields define an Airplane
        [Key]
        public int AirplaneId { get; set; }

        [Required]
        public string AirplaneModel { get; set; }

        [Required]
        public string RegistrationNum { get; set; }

        [Required]
        public string ManufacturerName { get; set; }

        [Required]
        public DateTime ManufactureYear { get; set; }

        [Required]
        public int MaxPassenger { get; set; }

        [Required]
        public string EngineModel { get; set; }

        [Required]
        public decimal Speed { get; set; }

        [Required]
        public decimal Range { get; set; }
    }

    public class AirplaneDto
    {
        public int AirplaneId { get; set; }

        public string AirplaneModel { get; set; }

        public string RegistrationNum { get; set; }

        public string ManufacturerName { get; set; }

        public DateTime ManufactureYear { get; set; }

        public int MaxPassenger { get; set; }

        public string EngineModel { get; set; }

        public decimal Speed { get; set; }

        public decimal Range { get; set; }
    }
}