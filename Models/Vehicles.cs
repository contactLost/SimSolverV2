using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimSolverV2.Models
{
    public class Vehicles
    {
        public int Id { get; set; }

        public string VehicleType { get; set; }

        public string Details { get; set; }

        public double fuelConsumptionPer100KM { get; set; }

    }
}
