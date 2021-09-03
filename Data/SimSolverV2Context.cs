using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimSolverV2.Models;
using SimoSolver.Models;

namespace SimSolverV2.Data
{
    public class SimSolverV2Context : DbContext
    {
        public SimSolverV2Context (DbContextOptions<SimSolverV2Context> options)
            : base(options)
        {
        }

        public DbSet<SimSolverV2.Models.Locations> Locations { get; set; }

        public DbSet<SimSolverV2.Models.Vehicles> Vehicles { get; set; }

        public DbSet<SimoSolver.Models.Results> Results { get; set; }
    }
}
