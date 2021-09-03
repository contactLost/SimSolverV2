using Google.OrTools.ConstraintSolver;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimSolverV2.Helpers
{
    class ortoolsOptimiser
    {

        class DataModel
        {
            public long[,] DistanceMatrix = {
            };
            public long[] Demands = { 0, 1, 1, 2, 4, 2, 4, 8, 8, 1, 2, 1, 2, 4, 4, 8, 8 };
            public long[] VehicleCapacities = { 200 , 200 }; //If you dont want to use capasities, give this large numbers.
            public int VehicleNumber = 2;
            public int Depot = 0;
        };

        /// <summary>
        ///   Print the solution.
        /// </summary>
        static OptimisationResult PrintSolution(in DataModel data, in RoutingModel routing, in RoutingIndexManager manager,
                                  in Assignment solution)
        {

            string result = "";

            //result = result + "Objective" + solution.ObjectiveValue() + "\n";
            List<List<int>> vehicleResultList= new List<List<int>>();

            // Inspect solution.
            long totalDistance = 0;
            long totalLoad = 0;
            for (int i = 0; i < data.VehicleNumber; ++i)
            {
                result = result + "Route for Vehicle " + i + ":\n";
                long routeDistance = 0;
                long routeLoad = 0;
                var index = routing.Start(i);
                List<int> routeList = new List<int>();
                while (routing.IsEnd(index) == false)
                {
                    long nodeIndex = manager.IndexToNode(index);
                    routeLoad += data.Demands[nodeIndex];
                    result = result + nodeIndex + " -> ";

                    //result = result + nodeIndex + "Load( " + routeLoad + " ) -> \n";
                    routeList.Add((int)nodeIndex);
                    var previousIndex = index;
                    index = solution.Value(routing.NextVar(index));
                    routeDistance += routing.GetArcCostForVehicle(previousIndex, index, 0);
                }
                vehicleResultList.Add(routeList);

                result = result + manager.IndexToNode((int)index) + "\n";
                result = result + "Distance of the route: " + routeDistance +" m \n";
                totalDistance += routeDistance;
                totalLoad += routeLoad;
            }
            result = result + "Total distance of all routes:" + totalDistance + " m \n";
            //result = result + "$Total load of all routes: "  + totalLoad + " m \n";

            OptimisationResult optimisationResult = new OptimisationResult();
            optimisationResult.resultString = result;
            optimisationResult.routeList = vehicleResultList;
            return optimisationResult;
        }




        public static OptimisationResult ortools( long[,] distanceMatrix, int vehicleCount)
        {
            // Instantiate the data problem.
            DataModel data = new DataModel();
            data.DistanceMatrix = distanceMatrix;
            data.VehicleNumber = vehicleCount;
            // Create Routing Index Manager
            RoutingIndexManager manager =
                new RoutingIndexManager(data.DistanceMatrix.GetLength(0), data.VehicleNumber, data.Depot);

            // Create Routing Model.
            RoutingModel routing = new RoutingModel(manager);

            // Create and register a transit callback.
            int transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) => {
                // Convert from routing variable Index to distance matrix NodeIndex.
                var fromNode = manager.IndexToNode(fromIndex);
                var toNode = manager.IndexToNode(toIndex);
                return data.DistanceMatrix[fromNode, toNode];
            });

            // Define cost of each arc.
            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);


            // Add Distance constraint.
            routing.AddDimension(transitCallbackIndex, 0, 200000,
                                 true, // start cumul to zero
                                 "Distance");
            RoutingDimension distanceDimension = routing.GetMutableDimension("Distance");
            distanceDimension.SetGlobalSpanCostCoefficient(100);

            /*            // Add Capacity constraint.
                        int demandCallbackIndex = routing.RegisterUnaryTransitCallback((long fromIndex) => {
                            // Convert from routing variable Index to demand NodeIndex.
                            var fromNode = manager.IndexToNode(fromIndex);
                            return data.Demands[fromNode];
                        });
                        routing.AddDimensionWithVehicleCapacity(demandCallbackIndex, 0, // null capacity slack
                                                                data.VehicleCapacities, // vehicle maximum capacities
                                                                true,                   // start cumul to zero
                                                                "Capacity");*/

            // Setting first solution heuristic.
            RoutingSearchParameters searchParameters =
                operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;
            searchParameters.LocalSearchMetaheuristic = LocalSearchMetaheuristic.Types.Value.GuidedLocalSearch;
            searchParameters.TimeLimit = new Duration { Seconds = 1 };

            // Solve the problem.
            Assignment solution = routing.SolveWithParameters(searchParameters);

            // Print solution on console.
            
            return PrintSolution(data, routing, manager, solution);
        }


    }

    //A helper class to send data to view
    public class OptimisationResult
    {
        public List<List<int>> routeList;
        public string resultString;

        public OptimisationResultWithArray changeToArrayVersion()
        {
            OptimisationResultWithArray orA = new OptimisationResultWithArray();

            orA.resultString = this.resultString;
            orA.routeList = this.routeList.Select(a => a.ToArray()).ToArray();
            return orA;
        }
    };

    public class OptimisationResultWithArray
    {
        public int [][] routeList { get; set; }
        public string resultString { get; set; }

    };
}


    /*class DataModel
        {
            //Dummy Data
            public long[,] DistanceMatrix = {
            { 0, 2451, 713, 1018, 1631, 1374, 2408, 213, 2571, 875, 1420, 2145, 1972 },
            { 2451, 0, 1745, 1524, 831, 1240, 959, 2596, 403, 1589, 1374, 357, 579 },
            { 713, 1745, 0, 355, 920, 803, 1737, 851, 1858, 262, 940, 1453, 1260 },
            { 1018, 1524, 355, 0, 700, 862, 1395, 1123, 1584, 466, 1056, 1280, 987 },
            { 1631, 831, 920, 700, 0, 663, 1021, 1769, 949, 796, 879, 586, 371 },
            { 1374, 1240, 803, 862, 663, 0, 1681, 1551, 1765, 547, 225, 887, 999 },
            { 2408, 959, 1737, 1395, 1021, 1681, 0, 2493, 678, 1724, 1891, 1114, 701 },
            { 213, 2596, 851, 1123, 1769, 1551, 2493, 0, 2699, 1038, 1605, 2300, 2099 },
            { 2571, 403, 1858, 1584, 949, 1765, 678, 2699, 0, 1744, 1645, 653, 600 },
            { 875, 1589, 262, 466, 796, 547, 1724, 1038, 1744, 0, 679, 1272, 1162 },
            { 1420, 1374, 940, 1056, 879, 225, 1891, 1605, 1645, 679, 0, 1017, 1200 },
            { 2145, 357, 1453, 1280, 586, 887, 1114, 2300, 653, 1272, 1017, 0, 504 },
            { 1972, 579, 1260, 987, 371, 999, 701, 2099, 600, 1162, 1200, 504, 0 },
        };
            public int VehicleNumber = 1;
            public int Depot = 0;
        };



        public static List<int> Ortools(long[,] distanceMatrix)
        {
            // Instantiate the data problem.
            DataModel data = new DataModel();
            data.DistanceMatrix = distanceMatrix;
            RoutingIndexManager manager =
           new RoutingIndexManager(data.DistanceMatrix.GetLength(0), data.VehicleNumber, data.Depot);

            // Create Routing Model.
            RoutingModel routing = new RoutingModel(manager);

            int transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) => {
                // Convert from routing variable Index to distance matrix NodeIndex.
                var fromNode = manager.IndexToNode(fromIndex);
                var toNode = manager.IndexToNode(toIndex);
                return data.DistanceMatrix[fromNode, toNode];
            });

            // Define cost of each arc.
            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

            // Setting first solution heuristic.
            RoutingSearchParameters searchParameters =
                operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;

            // Solve the problem.
            Assignment solution = routing.SolveWithParameters(searchParameters);

            // Print solution on console.
            PrintSolution(routing, manager, solution);

            // Print solution on console.
//            PrintSolution(data, routing, manager, solution);
            List<int> a = null;
            return a;
                
        }


        /// <summary>
        ///   Print the solution.
        /// </summary>
        static List<int> PrintSolution(in RoutingModel routing, in RoutingIndexManager manager, in Assignment solution)
        {
            Console.WriteLine("Objective: {0} meters", solution.ObjectiveValue());
            // Inspect solution.
            Console.WriteLine("Route:");
            long routeDistance = 0;
            var index = routing.Start(0);

            List<int> indexList = new List<int>();

            while (routing.IsEnd(index) == false)
            {
                Console.Write("{0} -> ", manager.IndexToNode((int)index));

                indexList.Add(manager.IndexToNode((int)index));

                var previousIndex = index;
                index = solution.Value(routing.NextVar(index));
                routeDistance += routing.GetArcCostForVehicle(previousIndex, index, 0);
            }
            Console.WriteLine("{0}", manager.IndexToNode((int)index));
            Console.WriteLine("Route distance: {0}meters", routeDistance);
            return indexList;
        }

        static string PrintSolution(in DataModel data, in RoutingModel routing, in RoutingIndexManager manager,
                              in Assignment solution)
        {
            string result = "";

            result = result + $"Objective {solution.ObjectiveValue()}:" + "\n";

            // Inspect solution.
            long maxRouteDistance = 0;
            for (int i = 0; i < data.VehicleNumber; ++i)
            {
                result = result + ("Route for Vehicle {0}:", i) + "\n";
                long routeDistance = 0;
                var index = routing.Start(i);
                while (routing.IsEnd(index) == false)
                {
                    result = result + ("{0} -> ", manager.IndexToNode((int)index)) + "\n";
                    var previousIndex = index;
                    index = solution.Value(routing.NextVar(index));
                    routeDistance += routing.GetArcCostForVehicle(previousIndex, index, 0);
                }
                result = result + ("{0}", manager.IndexToNode((int)index)) + "\n";
                result = result + ("Distance of the route: {0} m", routeDistance) + "\n";
                maxRouteDistance = Math.Max(routeDistance, maxRouteDistance);
            }
            result = result + ("Maximum distance of the routes: {0}m", maxRouteDistance) + "\n";

            return result;
        }
    }*/