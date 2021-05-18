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
            var result = new Dictionary<Edge<TravelItem>, double> ();

            foreach(var edge in graph.Edges)
            {
                string jsonResult = DistanceMatrixRequest(edge.Source.StreetAddress, edge.Target.StreetAddress); //TODO: add city to address

                if (String.IsNullOrEmpty(jsonResult))
                {
                    Console.WriteLine($"Unable to get distance from {edge.Source.StreetAddress} to {edge.Target.StreetAddress}");
                    continue;
                }

                var response = JsonSerializer.Deserialize<Response>(jsonResult);

                if(response == null)
                {
                    Console.WriteLine($"Unable to deserialize json for {edge}: {jsonResult}");
                    continue;
                }

                if(response.Rows.Length == 0)
                {
                    Console.WriteLine($"No rows returned for distance {edge}");
                    continue;
                }

                if(response.Rows[0].Elements.Length == 0)
                {
                    Console.WriteLine($"Elements in rows is empty for {edge}");
                    continue;
                }

                result.Add(edge, response.Rows[0].Elements[0].Distance.Value / 1000.0);//in kms
            }

            return result;


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
            string DistanceMatrixRequest(string source, string Destination)
            {
                try
                {
                    //string keyString = ConfigurationManager.AppSettings["keyString"].ToString(); // passing API key
                    //string clientID = ConfigurationManager.AppSettings["clientID"].ToString(); // passing client id
                    string urlRequest = "";
                    string travelMode = "Driving"; //Driving, Walking, Bicycling, Transit.

                    urlRequest = @"http://maps.googleapis.com/maps/api/distancematrix/json?origins=" + source + "&destinations=" + Destination + "&mode='" + travelMode + "'&sensor=false";
                    if (GoogleKey.ToString() != "")
                    {
                        //urlRequest += "&client=" + clientID;
                        urlRequest = Sign(urlRequest, GoogleKey); // request with api key and client id
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
        }

        public class Response
        {
            public string Status { get; set; }

            [JsonProperty(PropertyName = "destination_addresses")]
            public string[] DestinationAddresses { get; set; }

            [JsonProperty(PropertyName = "origin_addresses")]
            public string[] OriginAddresses { get; set; }

            public Row[] Rows { get; set; }

            public class Data
            {
                public int Value { get; set; } //meters
                public string Text { get; set; }
            }

            public class Element
            {
                public string Status { get; set; }
                public Data Duration { get; set; }
                public Data Distance { get; set; }
            }

            public class Row
            {
                public Element[] Elements { get; set; }
            }
        }
    }
}