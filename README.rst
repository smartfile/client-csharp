.. image:: https://d2xtrvzo9unrru.cloudfront.net/brands/smartfile/logo.png
   :alt: SmartFile

A `SmartFile`_ Open Source SDK. Developer information: https://www.smartfile.com/developer/


SmartFile API Client (C#)
=========================
This is a fully working API client for SmartFile. It allows you to interact with all objects within the SmartFile system via the REST API. This client library can be used as-is for your integration projects.

Installation
--------------
via source code / GitHub.

::

    $ git clone https://github.com/smartfile/client-csharp.git smartfile

More information is available at `GitHub <https://github.com/smartfile/client-csharp>`_

Upload Files
--------------
.. code:: csharp

	using RestSharp;
	using RestSharp.Authenticators;

	namespace SmartFile
	{
		class MainClass
		{
			public static int Main(string[] args)
			{
				// Setup new SmartFile client
				var client = new RestClient("https://app.smartfile.com/api/2/");
				client.Authenticator = new HttpBasicAuthenticator("**********", "**********");

				// Upload
				var response = Client.Upload(client, "pathToFile");

				return 0;
			}
		}
	}


You can also upload using the GetUploadRequest method, if you want to further customize your request.

.. code:: csharp

	using RestSharp;
	using RestSharp.Authenticators;

	namespace SmartFile
	{
		class MainClass
		{
			public static int Main(string[] args)
			{
				// Setup new SmartFile client
				var client = new RestClient("https://app.smartfile.com/api/2/");
				client.Authenticator = new HttpBasicAuthenticator("**********", "**********");

				// Upload
				var request = Client.GetUploadRequest("pathToFile");
				var response = client.Execute(request);

				return 0;
			}
		}
	}


Download Files
------------------
.. code:: csharp

	using RestSharp;
	using RestSharp.Authenticators;

	namespace SmartFile
	{
		class MainClass
		{
			public static int Main(string[] args)
			{
				// Setup new SmartFile client
				var client = new RestClient("https://app.smartfile.com/api/2/");
				client.Authenticator = new HttpBasicAuthenticator("**********", "**********");

				// Download
				var response = Client.Download(client, "myFile.txt", "pathToFileSaveLocation");

				return 0;
			}
		}
	}


You can also download using the GetDownloadRequest method, if you want to further customize your request.

.. code:: csharp

	using RestSharp;
	using RestSharp.Authenticators;
	using RestSharp.Extensions;

	namespace SmartFile
	{
		class MainClass
		{
			public static int Main(string[] args)
			{
				// Setup new SmartFile client
				var client = new RestClient("https://app.smartfile.com/api/2/");
				client.Authenticator = new HttpBasicAuthenticator("**********", "**********");

				// Download
				var request = Client.GetDownloadRequest("myFile.pdf");
				client.DownloadData(request).SaveAs("pathToFileSaveLocation");

				IRestResponse response = client.Execute(request);
				var content = response.Content; // raw content as string

				return 0;
			}
		}
	}


Move Files
------------------
.. code:: csharp

	using RestSharp;
	using RestSharp.Authenticators;

	namespace SmartFile
	{
		class MainClass
		{
			public static int Main(string[] args)
			{
				// Setup new SmartFile client
				var client = new RestClient("https://app.smartfile.com/api/2/");
				client.Authenticator = new HttpBasicAuthenticator("**********", "**********");

				// Move
				var response = Client.Move(client, "myPicture.jpg", "/myFolder/");

				return 0;
			}
		}
	}


You can also download using the GetMoveRequest method, if you want to further customize your request.

.. code:: csharp

	using RestSharp;
	using RestSharp.Authenticators;

	namespace SmartFile
	{
		class MainClass
		{
			public static int Main(string[] args)
			{
				// Setup new SmartFile client
				var client = new RestClient("https://app.smartfile.com/api/2/");
				client.Authenticator = new HttpBasicAuthenticator("**********", "**********");

				// Move
				var request = Client.GetMoveRequest("myFile.txt", "/newFolder/");

				IRestResponse response = client.Execute(request);
				var content = response.Content; // raw content as string

				return 0;
			}
		}
	}


Delete Files
------------------
.. code:: csharp

	using RestSharp;
	using RestSharp.Authenticators;

	namespace SmartFile
	{
		class MainClass
		{
			public static int Main(string[] args)
			{
				// Setup new SmartFile client
				var client = new RestClient("https://app.smartfile.com/api/2/");
				client.Authenticator = new HttpBasicAuthenticator("**********", "**********");

				// Delete file or path
				var response = Client.Remove(client, "myFile.txt")

				return 0;
			}
		}
	}


You can also download using the GetRemoveRequest method, if you want to further customize your request.

.. code:: csharp

	using RestSharp;
	using RestSharp.Authenticators;

	namespace SmartFile
	{
		class MainClass
		{
			public static int Main(string[] args)
			{
				// Setup new SmartFile client
				var client = new RestClient("https://app.smartfile.com/api/2/");
				client.Authenticator = new HttpBasicAuthenticator("**********", "**********");

				// Delete file or path
				var request = Client.GetRemoveRequest("myPhoto.jpg");

				IRestResponse response = client.Execute(request);
				var content = response.Content; // raw content as string

				return 0;
			}
		}
	}


Other endpoints can be found here: https://app.smartfile.com/api/

.. _SmartFile: https://www.smartfile.com/


Running Tests
------------------

To run tests for the ClientTests.cs file:

First, make an EnvironmentalVariables.cs file: 

.. code:: csharp

	namespace SmartFileTests
	{
		public static class EnvironmentalVariables
		{
			public static string API_KEY = "**********";
			public static string API_PASS = "**********";
		}
	}

Then, build SmartFileTests and run the tests through the Test Explorer.