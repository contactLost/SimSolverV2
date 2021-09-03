using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimSolverV2.Models
{
    public class Locations
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
