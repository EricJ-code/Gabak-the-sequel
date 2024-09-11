using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.IO;

namespace GabakWinForms
{
    internal static class Program
    {
        // Define classes to match the Json structure
        public class UserData
        {
            public object Module_Task_ID { get; set; }
            public List<int?> Pick_List_ID { get; set; } = new List<int?>();
            public List<double> x { get; set; } = new List<double>();
            public List<double> y { get; set; } = new List<double>();
            public List<double> z { get; set; } = new List<double>();
        }

        // Matching Json structure with the root class
        public class RootObject
        {
            public UserData data { get; set; }
        }

        // Define classes to match the XML structure
        public class WarehouseData
        {
            public List<RackLocation> RacksLocation { get; set; } = new List<RackLocation>();
            public Dictionary<string, List<ObjectiveLocation>> ObjectivesLocation { get; set; } = new Dictionary<string, List<ObjectiveLocation>>();
            public double WarehouseWidth { get; set; }
            public double WarehouseDepth { get; set; }
            public double CenterX { get; set; }
            public double CenterY { get; set; }
            public double RackWidth { get; set; }
            public double RackDepth { get; set; }
        }

        public class RackLocation
        {
            public double X1 { get; set; }
            public double X2 { get; set; }
            public double X3 { get; set; }
            public double X4 { get; set; }
            public double Y1 { get; set; }
            public double Y2 { get; set; }

            public double Y3 { get; set; }
            public double Y4 { get; set; }
            public double Angle { get; set; }
        }

        public class ObjectiveLocation
        {
            public double X { get; set; }
            public double Y { get; set; }
        }

        [STAThread]
        static void Main()
        {
            // Run Application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static (UserData, WarehouseData) FetchData()
        {
            try
            {
                // hardcoded path for the downloads folder file "test.ard"
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "test5.ard");
                string fileContent = File.ReadAllText(filePath);

                // Parse the XML data from the file
                var warehouseData = ParseARD(fileContent);

                // DEBUG OUTPUT
                Console.WriteLine("Warehouse Width: " + warehouseData.WarehouseWidth);
                Console.WriteLine("Warehouse Depth: " + warehouseData.WarehouseDepth);
                Console.WriteLine("Number of Racks: " + warehouseData.RacksLocation.Count);

                // Return mock data for UserData
                var userData = new UserData();
                return (userData, warehouseData);
            }
            catch (Exception ex)
            {
                // if file not found
                Console.WriteLine("Error reading file: " + ex.Message);
                return (null, null);
            }
        }

        /*
        public static (UserData, WarehouseData) FetchData(string sessionID = "OLXex0tmxckGhA4vvZKhvHhUZfMKVEnf")
        {
            Create an HTTPClient instance (using statement to automatically dispose of object)
            using (var client = new HttpClient())
            {
                var offset = 0;
                var limit = 100;
                var endpoint = new Uri($"https://www.gokhanozden.com/augmentedwarehouse/warp/php/get_user_coordinates.php?sessionID={sessionID}&offset={offset}&limit={limit}");

                // Fetch data from HTTP GET request and disregard async and do it synchronously
                var result = client.GetAsync(endpoint).Result;
                var json = result.Content.ReadAsStringAsync().Result;

                // DEBUG OUTPUT: Print the raw JSON response
                Console.WriteLine("Raw JSON: " + json);

                // Deserialize JSON into the RootObject class
                RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(json);

                if (rootObject?.data != null)
                {
                    // Accessing specific values from the C# object
                    var moduleTaskID = rootObject.data.Module_Task_ID;
                    var pickListID = rootObject.data.Pick_List_ID;
                    var userX = rootObject.data.x;
                    var userY = rootObject.data.y;
                    var userZ = rootObject.data.z;

                    // DEBUG OUTPUT
                    Console.WriteLine("Module_Task_ID: " + moduleTaskID);
                    Console.WriteLine("Pick_List_ID: " + (pickListID.Count > 0 ? string.Join(", ", pickListID) : "No Data"));
                    Console.WriteLine("X Coordinates: " + (userX.Count > 0 ? string.Join(", ", userX) : "No Data"));
                    Console.WriteLine("Y Coordinates: " + (userY.Count > 0 ? string.Join(", ", userY) : "No Data"));
                    Console.WriteLine("Z Coordinates: " + (userZ.Count > 0 ? string.Join(", ", userZ) : "No Data"));
                    Console.WriteLine("First X Coordinate: " + userX[0]);
                }
                else
                {
                    Console.WriteLine("UserData is null or JSON deserialization failed.");
                }

                // Handle fetching data from map (ARD file)
                var mapEndpoint = new Uri($"https://www.gokhanozden.com/augmentedwarehouse/warp/php/get_module_task_info.php?moduleTaskId={rootObject.data.Module_Task_ID}");

                // Fetch data from HTTP GET request and disregard async and do it synchronously
                var mapResult = client.GetAsync(mapEndpoint).Result;
                var mapString = mapResult.Content.ReadAsStringAsync().Result;

                // DEBUG OUTPUT
                Console.WriteLine("Raw string: " + mapString);

                string extractedXML = CleanXML(mapString);

                var warehouseData = ParseARD(extractedXML);

                // DEBUG OUTPUT
                Console.WriteLine("Warehouse Width: " + warehouseData.WarehouseWidth);
                Console.WriteLine("Warehouse Depth: " + warehouseData.WarehouseDepth);
                Console.WriteLine("Number of Racks: " + warehouseData.RacksLocation.Count);

                // DEBUG OUTPUT of storage location (racks location x,y and angle)
                //foreach (var rack in warehouseData.RacksLocation)
                //{
                //    Console.WriteLine($"Rack Location - X: {rack.X}, Y: {rack.Y}, Angle: {rack.Angle}");
                //}
                return (rootObject.data, warehouseData);
            } 
        }
        */
        public static string CleanXML(string jsonString)
        {
            var jsonObject = JObject.Parse(jsonString);
            string xmlString = jsonObject["ARD"].ToString();

            xmlString = xmlString.Replace("\\\"", "\"")
                                 .Replace("\\\\", "\\")
                                 .Replace("\\/", "/")
                                 .Replace("\\n", "\n")
                                 .Replace("\\r", "\r")
                                 .Trim();
            return xmlString;
        }

        public static WarehouseData ParseARD(string fullARD)
        {
            var warehouseData = new WarehouseData();
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(fullARD);

            // should select the first node that matches the xml path "//warehouse"
            // if not null, parse the warehouse dimensions / storage location dimensions from XML attributes and convert to meters
            var warehouseNode = xmlDoc.SelectSingleNode("//warehouse");
            if (warehouseNode != null) 
            { 
                warehouseData.WarehouseWidth = double.Parse(warehouseNode.Attributes["width"].Value) / 3.281;
                warehouseData.WarehouseDepth = double.Parse(warehouseNode.Attributes["depth"].Value) / 3.281;
                warehouseData.CenterX = double.Parse(warehouseNode.Attributes["center_x"].Value) / 3.281;
                warehouseData.CenterY = double.Parse(warehouseNode.Attributes["center_y"].Value) / 3.281;
                warehouseData.RackWidth = double.Parse(warehouseNode.Attributes["storagelocationwidth"].Value) / 3.281;
                warehouseData.RackDepth = double.Parse(warehouseNode.Attributes["storagelocationdepth"].Value) / 3.281;
            }

            // should select all nodes that matches the xml path "//region"
            var regionNodes = xmlDoc.SelectNodes("//region");
            foreach (XmlNode regionNode in regionNodes)
            {
                // parse the angle attribute from the region node & select all "storagelocation" nodes within current region node
                var angle = double.Parse(regionNode.Attributes["angle"].Value);
                var storageLocationNodes = regionNode.SelectNodes("storagelocation");
                foreach (XmlNode storageLocationNode in storageLocationNodes)
                {
                    // parse the X and Y coords of each storage location and convert to meters
                    var x1 = double.Parse(storageLocationNode.Attributes["x1"].Value) / 3.281;
                    var x2 = double.Parse(storageLocationNode.Attributes["x2"].Value) / 3.281;
                    var x3 = double.Parse(storageLocationNode.Attributes["x3"].Value) / 3.281;
                    var x4 = double.Parse(storageLocationNode.Attributes["x4"].Value) / 3.281;
                    var y1 = double.Parse(storageLocationNode.Attributes["y1"].Value) / 3.281;
                    var y2 = double.Parse(storageLocationNode.Attributes["y2"].Value) / 3.281;
                    var y3 = double.Parse(storageLocationNode.Attributes["y3"].Value) / 3.281;
                    var y4 = double.Parse(storageLocationNode.Attributes["y4"].Value) / 3.281;

                    // add the storage location data
                    warehouseData.RacksLocation.Add(new RackLocation
                    {
                        X1 = x1,
                        X2 = x2,
                        X3 = x3,
                        X4 = x4,
                        Y1 = y1,
                        Y2 = y2,
                        Y3 = y3,
                        Y4 = y4,
                        Angle = angle
                    });
                }
            }

            // should select all nodes that matches the xml path "//picklist"
            var picklistNodes = xmlDoc.SelectNodes("//picklist");
            foreach(XmlNode picklistNode in picklistNodes)
            {
                // get id of current picklist & init a list for storing pick locations for current picklist ID
                var id = picklistNode.Attributes["id"].Value;
                warehouseData.ObjectivesLocation[id] = new List<ObjectiveLocation>();

                // select all "picklocation" nodes within the current picklist ID
                var pickLocationNodes = picklistNode.SelectNodes("picklocation");
                foreach (XmlNode pickLocationNode in  pickLocationNodes)
                {
                    // parse the X and Y coordinates of each pick location & convert to meters
                    var x = double.Parse(pickLocationNode.Attributes["x"].Value) / 3.281;
                    var y = double.Parse(pickLocationNode.Attributes["y"].Value) / 3.281;

                    // add the pick location data to the ObjectivesLocation dict for the current picklist ID
                    warehouseData.ObjectivesLocation[id].Add(new ObjectiveLocation
                    {
                        X = x,
                        Y = y
                    });
                }
            }
            return warehouseData;
        }
    }
}
