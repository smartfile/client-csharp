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

    public abstract class Client
    {

        public static RestClient client = new RestClient("https://app.smartfile.com/api/2/");

        public static RestRequest Upload(string filename)
        {

            var request = new RestRequest("/path/data/", Method.POST);
            request.AddFile("file", filename);

            return request;
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

        public static RestRequest Delete(string FileToBeDeleted)
        {
            var request = new RestRequest("/path/oper/remove/", Method.POST);
            request.AddParameter("path", FileToBeDeleted);

            return request;
        }
    }
}
