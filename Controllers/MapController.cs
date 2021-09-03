using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SimSolverV2.Data;
using SimSolverV2.Helpers;
using SimSolverV2.Models;
using System.Text.Json;
using Newtonsoft.Json;

namespace SimSolverV2.Controllers
{
    public class MapController : Controller
    {
        private readonly SimSolverV2Context _db;
        public static OptimisationResult opResult;

        public MapController(SimSolverV2Context db)
        {
            _db = db;
        }

        // GET: /Map

        public async Task<IActionResult> Index()
        {
            if (_db.Results.Count() == 0)
            {
                ViewData["solution"] = "Results are Empty. Go to results and create a new result.";
                ViewData["LocList"] = null;
                return View();
            }

 //           long[,] distanceMatrix = await getDistanceMatrix();
 //           OptimisationResult solution = SimSolverV2.Helpers.ortoolsOptimiser.ortools(distanceMatrix);
 //           List<Locations> objList = _db.Locations.ToList<Locations>();

            //Get the Latest Result
            var latestResult = _db.Results.OrderBy(item => item.Id).Last();
            //Extract OptimisationResult and Location Data
            var LocList = System.Text.Json.JsonSerializer.Deserialize<List<Locations>>(latestResult.LocationsData);

            string LocListString = "";

            for(int i = 0; i < LocList.Count(); i++)
            {
                LocListString = LocListString + LocList.ElementAt(i).Name + ": " + LocList.ElementAt(i).Latitude + " , " + LocList.ElementAt(i).Longitude + "\n";
            }

            ViewData["solution"] = latestResult.displayString;
            ViewData["LocList"] = LocListString;
            return View();
        }

        private async Task<long[,]> getDistanceMatrix()
        {
            List<Locations> objList = _db.Locations.ToList<Locations>();
            long[,] distanceMatrix = await SimSolverV2.Helpers.distanceMatrixFiller.prepareDistanceMatrix(objList);
            return distanceMatrix;
        }

        // GET: Map/GPX/5
        public string GPX(int id)
        {
            //Get Latest Result
            var latestResult = _db.Results.OrderBy(item => item.Id).Last();

            var locationData = latestResult.LocationsData;
            var optimisationData = latestResult.OptimizationResult;

            //Convert locationData to a list
            var LocList = System.Text.Json.JsonSerializer.Deserialize<List<Locations>>(latestResult.LocationsData);

            List<List<int>> optiData = JsonConvert.DeserializeObject<List<List<int>>>(latestResult.OptimizationResult);

            string url = "http://localhost:8989/route?";
            var firstOptiArray = optiData.ElementAt(id);
            for (int i = 0; i < firstOptiArray.Count; i++)
            {
                url = url + "point=" + LocList.ElementAt(firstOptiArray.ElementAt(i)).Latitude + "%2C" + LocList.ElementAt(firstOptiArray.ElementAt(i)).Longitude + "&";
            }
            url = url + "point=" + LocList.ElementAt(firstOptiArray.ElementAt(0)).Latitude + "%2C" + LocList.ElementAt(firstOptiArray.ElementAt(0)).Longitude + "&";
            return url + "type=gpx&gpx.route=false&gpx.track=true&gpx.waypoints=true&locale=tr-TR&key=&elevation=false&profile=car";

        }

        public string MetaData()
        {
            //What do I want to return
            // Locations 
            // GPX IDs
            // Results
            // Vehiacle Count

            return _db.Vehicles.Count().ToString();
        }
    }
}
