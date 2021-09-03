using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace SimoSolver.Models
{
    public class Results
    {
        public int Id { get; set; }

        public string displayString { get; set; }

        public string vehicleData { get; set; }

        public string LocationsData { get; set; }

        public string OptimizationResult { get; set; }
    }
}
