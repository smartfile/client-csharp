using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

using RestSharp;


namespace SmartFile
{

    public class Client : RestClient
    {
        // Constructor
        public Client(string baseUrl) : base(baseUrl)
        {
        }

        public void DoRequest()
        {
            //var request = Client; 

            //IRestResponse response = Client.Execute();
            //var content = response.Content;
        }

        public static RestRequest GetUploadRequest(string filename)
        {
            var request = new RestRequest("/path/data/", Method.POST);
            request.AddFile("file", filename);

            return request;
        }

        public static IRestResponse Upload(RestClient client, string filename)
        { 
            var request = GetUploadRequest(filename);

            return client.Execute(request);
        }

        public static RestRequest Download(string downloadName)
        {
            var request = new RestRequest("/path/data/" + downloadName, Method.GET);

            return request;
        }

        public static RestRequest Move(string sourceFile, string destinationFolder)
        {
            var request = new RestRequest("/path/oper/move/", Method.POST);
            request.AddParameter("src", sourceFile);
            request.AddParameter("dst", destinationFolder);

            return request;
        }

        public static RestRequest Remove(string FileToBeDeleted)
        {
            var request = new RestRequest("/path/oper/remove/", Method.POST);
            request.AddParameter("path", FileToBeDeleted);

            return request;
        }
    }
}
