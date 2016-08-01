using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;

using SmartFile;

namespace SmartFileTests
{
    [TestClass()]
    public class ClientTests
    {

        public Client client = new Client("https://app.smartfile.com/api/2/");
        public const string TESTFN = "TESTFN.txt";
        public const string TESTFN2 = "/TESTFN2/";

        public void ConstructorTest()
        {
            // Checks constructors
            client.Authenticator = new HttpBasicAuthenticator(EnvironmentalVariables.API_KEY, EnvironmentalVariables.API_PASS);
        }

        public void CanConnectTest()
        {

            // Ping test to assure we can connect
            client.Authenticator = new HttpBasicAuthenticator(EnvironmentalVariables.API_KEY, EnvironmentalVariables.API_PASS);

            var request = new RestRequest("/ping/", Method.GET);

            IRestResponse response = client.Execute(request);
            var content = response.Content; // raw content as string

            // Deserialize response to a C# dict
            JObject o = JObject.Parse(content);
            string ping = (string)o.SelectToken("ping");

            // Assert 'ping':'pong'
            Assert.AreEqual("pong", ping);
        }

        public void setUp()
        {
            // makes a directory for the tests.
            var request = new RestRequest("/path/oper/mkdir/", Method.POST);
            request.AddParameter("path", TESTFN2);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
        }

        public void tearDown()
        {
            // removes the directory made for the tests
            var request = new RestRequest("/path/oper/remove/", Method.POST);
            request.AddParameter("path", TESTFN2);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
        }

        public void UploadTest()
        {
            client.Authenticator = new HttpBasicAuthenticator(EnvironmentalVariables.API_KEY, EnvironmentalVariables.API_PASS);

            // Get info from local file
            FileInfo localFileInfo = new FileInfo(TESTFN);
            long localFileSize = localFileInfo.Length;

            // Uploads that file
            var request = Client.Upload(client, TESTFN);
            // Checks info of file we just uploaded
            var requestFileInfo = new RestRequest("/path/info/" + TESTFN, Method.GET);

            IRestResponse responseFileInfo = client.Execute(requestFileInfo);
            var fileInfoContent = responseFileInfo.Content; // raw content as string

            // Parses the JSON to get the size of the uploaded file
            JObject o = JObject.Parse(fileInfoContent);
            long uploadedFileSize = (long)o.SelectToken("size");

            Assert.AreEqual(localFileSize, uploadedFileSize);
        }

        public void DownloadTest()
        {
            client.Authenticator = new HttpBasicAuthenticator(EnvironmentalVariables.API_KEY, EnvironmentalVariables.API_PASS);

            // Downloads file from SmartFile to a location other than the orginal local copy
            var request = Client.Download(client, TESTFN, AppDomain.CurrentDomain.BaseDirectory + "\\TESTFNsave.txt");

            // Reads text from both files to compare and assure they are the same
            string LocalFileText = File.ReadAllText(TESTFN);
            string RemoteFileText = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\TESTFNsave.txt");

            Assert.AreEqual(LocalFileText, RemoteFileText);
        }

        public void MoveTest()
        {
            client.Authenticator = new HttpBasicAuthenticator(EnvironmentalVariables.API_KEY, EnvironmentalVariables.API_PASS);

            // Create new folder to move test file to


            // Moves file to a new location
            var request = Client.Move(client, TESTFN, TESTFN2);
        }

        public void DeleteTest()
        {
            client.Authenticator = new HttpBasicAuthenticator(EnvironmentalVariables.API_KEY, EnvironmentalVariables.API_PASS);

            // Deletes file from that new location
            var request = Client.GetRemoveRequest("/TESTFN2/TESTFN.txt");

            IRestResponse response = client.Execute(request);
            var content = response.Content;

            JObject o = JObject.Parse(content);
            string detail = (string)o.SelectToken("type");

            // If the file does not exist this will fail because detail will return null
            Assert.AreEqual("Remove", detail);
        }

        // Runs all the tests in the order wanted
        [TestMethod]
        public void RunAllTests()
        {
            ConstructorTest();
            CanConnectTest();
            setUp();
            UploadTest();
            DownloadTest();
            MoveTest();
            DeleteTest();
            tearDown();
        }

    }
}