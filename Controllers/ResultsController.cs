using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimSolverV2.Data;
using SimoSolver.Models;
using System.Text.Json;
using SimSolverV2.Models;
using SimSolverV2.Helpers;

namespace SimSolverV2.Controllers
{
    public class ResultsController : Controller
    {
        private readonly SimSolverV2Context _context;

        public ResultsController(SimSolverV2Context context)
        {
            _context = context;
        }

        // GET: Results
        public async Task<IActionResult> Index()
        {
            return View(await _context.Results.ToListAsync());
        }

        // GET: Results/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var results = await _context.Results
                .FirstOrDefaultAsync(m => m.Id == id);
            if (results == null)
            {
                return NotFound();
            }

            return View(results);
        }

        // GET: Results/Create
        public async Task<IActionResult> Create()
        {
            //Check if there is a vehicle
            if (_context.Vehicles.Count() == 0)
                return NotFound();

            //Check if there is a location
            if (_context.Locations.Count() == 0)
                return NotFound();

            //Create a result 
            Results result = new Results();

            //Get Locations
            List<Models.Locations> locationsList = await _context.Locations.ToListAsync();

            //Record Locations to Result
            var jsonString = JsonSerializer.Serialize(locationsList);
            result.LocationsData = jsonString;

            //Get Vehicles 
            List<Models.Vehicles> vehiclesList = await _context.Vehicles.ToListAsync();

            //Record Vehicles to Result
            var vehicleString = JsonSerializer.Serialize(vehiclesList);
            result.vehicleData = vehicleString;

            //Run Optimisation

            long[,] distanceMatrix = await getDistanceMatrix();
            OptimisationResult solution = SimSolverV2.Helpers.ortoolsOptimiser.ortools(distanceMatrix , _context.Vehicles.Count());

            //Record Optimisation Result
            var OpResultWithArray = solution.changeToArrayVersion();
            var optimisationString = JsonSerializer.Serialize(OpResultWithArray.routeList);
            result.OptimizationResult = optimisationString;

            result.displayString = solution.resultString;

            //Save Result
            _context.Add(result);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //A Helper Method for Create
        private async Task<long[,]> getDistanceMatrix()
        {
            List<Locations> objList = _context.Locations.ToList<Locations>();
            long[,] distanceMatrix = await SimSolverV2.Helpers.distanceMatrixFiller.prepareDistanceMatrix(objList);
            return distanceMatrix;
        }

        // POST: Results/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,displayString,vehicleData,LocationsData,OptimizationResult")] Results results)
        {
            if (ModelState.IsValid)
            {
                _context.Add(results);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(results);
        }

        // GET: Results/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var results = await _context.Results.FindAsync(id);
            if (results == null)
            {
                return NotFound();
            }
            return View(results);
        }

        // POST: Results/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,displayString,vehicleData,LocationsData,OptimizationResult")] Results results)
        {
            if (id != results.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(results);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResultsExists(results.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(results);
        }

        // GET: Results/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var results = await _context.Results
                .FirstOrDefaultAsync(m => m.Id == id);
            if (results == null)
            {
                return NotFound();
            }

            return View(results);
        }

        // POST: Results/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var results = await _context.Results.FindAsync(id);
            _context.Results.Remove(results);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResultsExists(int id)
        {
            return _context.Results.Any(e => e.Id == id);
        }
    }
}
