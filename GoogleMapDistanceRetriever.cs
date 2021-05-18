using QuikGraph;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DistanceDrivenCalc
{
    internal class GoogleMapDistanceRetriever
    {
        public GoogleMapDistanceRetriever(string googleKey)
        {
            GoogleKey = googleKey;
        }

        public string GoogleKey { get; }

        internal Dictionary<Edge<TravelItem>, double> Execute(AdjacencyGraph<TravelItem, Edge<TravelItem>> graph)
        {
            var result = new Dictionary<Edge<TravelItem>, double>();

            foreach (var edge in graph.Edges)
            {
                if (result.ContainsKey(edge))//minimize requests
                {
                    continue;
                }

                Console.Write($"Query distance: {edge.Source} to {edge.Target}...");

                string jsonResult = GetEdgeDistance(edge);

                if (String.IsNullOrEmpty(jsonResult))
                {
                    Console.WriteLine($"Unable to get distance from {edge.Source.StreetAddress} to {edge.Target.StreetAddress}");
                    continue;
                }

                var response = JsonSerializer.Deserialize<Parent>(jsonResult);

                if (response == null)
                {
                    Console.WriteLine($"Unable to deserialize json for {edge}: {jsonResult}");
                    continue;
                }

                if (response.rows.Length == 0)
                {
                    Console.WriteLine($"No rows returned for distance {edge}");
                    continue;
                }

                if (response.rows[0].elements.Length == 0)
                {
                    Console.WriteLine($"Elements in rows is empty for {edge}");
                    continue;
                }

                Console.WriteLine($"{response.rows[0].elements[0].distance.value / 1000.0} km");

                result.Add(edge, response.rows[0].elements[0].distance.value / 1000.0);//in kms
            }

            return result;
        }


            //* GOOGLE Single response *//
            /*
            {
                "destination_addresses": [
                  "Karnataka, India"
                ],
                "origin_addresses": [
                   "Delhi, India"
                ],
                "rows": [
                   {
                       "elements": [
                           {
                               "distance": {
                                   "text": "1,942 km",
                                   "value": 1941907
                               },
                               "duration": {
                                   "text": "1 day 9 hours",
                                   "value": 120420
                               },
                               "status": "OK"
                           }
                       ]
                   }
               ],
               "status": "OK"
            }
            */

            //INTERNAL METHODS//
            string DistanceMatrixRequest(string source, string destination)
            {
                try
                {
                    //string keyString = ConfigurationManager.AppSettings["keyString"].ToString(); // passing API key
                    //string clientID = ConfigurationManager.AppSettings["clientID"].ToString(); // passing client id
                    string urlRequest = "";
                    string travelMode = "Driving"; //Driving, Walking, Bicycling, Transit.

                    string ipAddress = "95.82.119.101"; //TODO: Gain IP address from restricted in google console

                    urlRequest = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins=" + source + "&destinations=" + destination + "&mode='" + travelMode + $"'&sensor=false&address={ipAddress}";
                    if (GoogleKey.ToString() != "")
                    {
                        //urlRequest += "&client=" + clientID;
                        //urlRequest = Sign(urlRequest, GoogleKey); // request with api key and client id
                        urlRequest += $"&key={GoogleKey}";
                    }

                    WebRequest request = WebRequest.Create(urlRequest);
                    request.Method = "POST";
                    string postData = "This is a test that posts this string to a Web server.";
                    byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = byteArray.Length;

                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();

                    WebResponse response = request.GetResponse();
                    dataStream = response.GetResponseStream();

                    StreamReader reader = new StreamReader(dataStream);
                    string resp = reader.ReadToEnd();

                    reader.Close();
                    dataStream.Close();
                    response.Close();

                    return resp;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            string Sign(string url, string googleKey)
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                // converting key to bytes will throw an exception, need to replace '-' and '_' characters first.
                string usablePrivateKey = googleKey.Replace("-", "+")
                    .Replace("_", "/");
                byte[] privateKeyBytes = Convert.FromBase64String(usablePrivateKey);
                Uri uri = new Uri(url);
                byte[] encodedPathAndQueryBytes = encoding.GetBytes(uri.LocalPath + uri.Query);
                // compute the hash
                HMACSHA1 algorithm = new HMACSHA1(privateKeyBytes);
                byte[] hash = algorithm.ComputeHash(encodedPathAndQueryBytes);
                // convert the bytes to string and make url-safe by replacing '+' and '/' characters
                string signature = Convert.ToBase64String(hash)
                    .Replace("+", "-")
                    .Replace("/", "_");
                // Add the signature to the existing URI.
                return uri.Scheme + "://" + uri.Host + uri.LocalPath + uri.Query + "&signature=" + signature;
            }

        string GetEdgeDistance(Edge<TravelItem> edge)
        {
            return DistanceMatrixRequest($"{edge.Source.StreetAddress}+{edge.Source.City}", $"{edge.Target.StreetAddress}+{edge.Target.City}");
            //TODO: add city to address
        }

        internal double GetEdgeDistanceDouble(Edge<TravelItem> edge)
        {
            string jsonResult = GetEdgeDistance(edge);

            if (String.IsNullOrEmpty(jsonResult))
            {
                Console.WriteLine($"Unable to get distance from {edge.Source.StreetAddress} to {edge.Target.StreetAddress}");
                return -1.0;
            }

            var response = JsonSerializer.Deserialize<Parent>(jsonResult);

            if (response == null)
            {
                Console.WriteLine($"Unable to deserialize json for {edge}: {jsonResult}");
                return -1.0;
            }

            if (response.rows.Length == 0)
            {
                Console.WriteLine($"No rows returned for distance {edge}");
                return -1.0;
            }

            if (response.rows[0].elements.Length == 0)
            {
                Console.WriteLine($"Elements in rows is empty for {edge}");
                return -1.0;
            }

            Console.WriteLine($"{response.rows[0].elements[0].distance.value / 1000.0} km");

            return (response.rows[0].elements[0].distance.value / 1000.0);//in kms
        }

        /*{
           "destination_addresses" : [ "al. Marszałka Józefa Piłsudskiego 15/23, 90-307 Łódź, Poland" ],
           "origin_addresses" : [ "Plac Wolności 2, 96-230 Biała Rawska, Poland" ],
           "rows" : [
              {
                 "elements" : [
                    {
                       "distance" : {
                          "text" : "79.3 km",
                          "value" : 79343
                       },
                       "duration" : {
                          "text" : "1 hour 22 mins",
                          "value" : 4902
                       },
                       "status" : "OK"
                    }
                 ]
              }
           ],
           "status" : "OK"
        }
        */

        public class Response
        {
            public string Status { get; set; }

            [JsonProperty(PropertyName = "destination_addresses")]
            public string[] DestinationAddresses { get; set; }

            [JsonProperty(PropertyName = "origin_addresses")]
            public string[] OriginAddresses { get; set; }

            [JsonProperty(PropertyName = "rows")]
            public Row[] Rows { get; set; }

            public class Data
            {
                [JsonProperty(PropertyName = "value")]
                public int Value { get; set; } //meters
                [JsonProperty(PropertyName = "text")]
                public string Text { get; set; }
            }

            public class Element
            {
                [JsonProperty(PropertyName = "status")]
                public string Status { get; set; }
                [JsonProperty(PropertyName = "duration")]
                public Data Duration { get; set; }
                [JsonProperty(PropertyName = "distance")]
                public Data Distance { get; set; }
            }

            public class Row
            {
                [JsonProperty(PropertyName = "elements")]
                public Element[] Elements { get; set; }
            }
        }

        public class Distance
        {
            public string text { get; set; }
            public int value { get; set; }
        }

        public class Duration
        {
            public string text { get; set; }
            public int value { get; set; }
        }

        public class Element
        {
            public Distance distance { get; set; }
            public Duration duration { get; set; }
            public string status { get; set; }
        }

        public class Row
        {
            public Element[] elements { get; set; }
        }

        public class Parent
        {
            public string[] destination_addresses { get; set; }
            public string[] origin_addresses { get; set; }
            public Row[] rows { get; set; }
            public string status { get; set; }
        }
    }
}