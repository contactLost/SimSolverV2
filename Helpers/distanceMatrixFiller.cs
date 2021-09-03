using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using SimSolverV2.Models;

namespace SimSolverV2.Helpers
{
    class distanceMatrixFiller
    {

        static readonly HttpClient client = new HttpClient();

        //Makes API call to Graphhopper and gets distance between two points.
        public static async Task<double> getDistanceForTwoPoints(string point1X, string point1Y, string point2X, string point2Y)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.

            string url = "http://localhost:8989/route?point=" + point1X + "%2C" + point1Y + "&point=" + point2X + "%2C" + point2Y;

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(responseBody);
                Console.WriteLine(myDeserializedClass.paths[0].distance);
                return myDeserializedClass.paths[0].distance;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return -1;


        }
        //Takes a list of coordinates and returns a matrix of distance between those coordinates.
        public static async Task<long[,]> prepareDistanceMatrix(string[,] points)
        {
           
            int locationNumber = points.GetLength(0);
            long[,] distanceMatrix = new long[locationNumber, locationNumber];
            for (int fromNode = 0; fromNode < locationNumber; fromNode++)
            {
                for (int toNode = 0; toNode < locationNumber; toNode++)
                {
                    if (fromNode == toNode)
                        distanceMatrix[fromNode, toNode] = 0;
                    else
                        distanceMatrix[fromNode, toNode] =
                            Convert.ToInt64(await getDistanceForTwoPoints(points[fromNode, 0], points[fromNode, 1], points[toNode, 0], points[toNode, 1]));
                }
            }
            return distanceMatrix;
        }

        //Takes a list of coordinates and returns a matrix of distance between those coordinates.
        public static async Task<long[,]> prepareDistanceMatrix(List<Locations> points)
        {

            int locationNumber = points.Count();

            long[,] distanceMatrix = new long[locationNumber, locationNumber];

            for (int fromNode = 0; fromNode < locationNumber; fromNode++)
            {
                for (int toNode = 0; toNode < locationNumber; toNode++)
                {
                    if (fromNode == toNode)
                        distanceMatrix[fromNode, toNode] = 0;
                    else
                        distanceMatrix[fromNode, toNode] =
                            Convert.ToInt64( await getDistanceForTwoPoints(
                                    points.ElementAt(fromNode).Latitude,
                                    points.ElementAt(fromNode).Longitude,
                                    points.ElementAt(toNode).Latitude,
                                    points.ElementAt(toNode).Longitude));
                }
            }
            return distanceMatrix;
        }
    }

    public class Hints
    {
        [JsonProperty("visited_nodes.sum")]
        public int VisitedNodesSum { get; set; }

        [JsonProperty("visited_nodes.average")]
        public double VisitedNodesAverage { get; set; }
    }

    public class Info
    {
        public List<string> copyrights { get; set; }
        public int took { get; set; }
    }

    public class Instruction
    {
        public double distance { get; set; }
        public double heading { get; set; }
        public int sign { get; set; }
        public List<int> interval { get; set; }
        public string text { get; set; }
        public int time { get; set; }
        public string street_name { get; set; }
        public double? last_heading { get; set; }
    }

    public class Details
    {
    }

    public class Path
    {
        public double distance { get; set; }
        public double weight { get; set; }
        public int time { get; set; }
        public int transfers { get; set; }
        public bool points_encoded { get; set; }
        public List<double> bbox { get; set; }
        public string points { get; set; }
        public List<Instruction> instructions { get; set; }
        public List<object> legs { get; set; }
        public Details details { get; set; }
        public double ascend { get; set; }
        public double descend { get; set; }
        public string snapped_waypoints { get; set; }
    }

    public class Root
    {
        public Hints hints { get; set; }
        public Info info { get; set; }
        public List<Path> paths { get; set; }
    }

}
