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

More information is available at `GitHub <https://github.com/smartfile/client-php>`_

Upload File
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
				var request = Client.Upload("pathToFile");

				IRestResponse response = client.Execute(request);
				var content = response.Content; // raw content as string

				return 0;
			}
		}
	}


Download File
------------------
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
				var request = Client.Download("myFile.pdf");
				client.DownloadData(request).SaveAs("pathToFileSaveLocation");

				IRestResponse response = client.Execute(request);
				var content = response.Content; // raw content as string

				return 0;
			}
		}
	}


Move File
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
				var request = Client.Move("myFile.txt", "/newFolder/");

				IRestResponse response = client.Execute(request);
				var content = response.Content; // raw content as string

				return 0;
			}
		}
	}


Delete File
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
				var request = Client.Remove("myPhoto.jpg");

				IRestResponse response = client.Execute(request);
				var content = response.Content; // raw content as string

				return 0;
			}
		}
	}


Other endpoints can be found here: https://app.smartfile.com/api/

.. _SmartFile: https://www.smartfile.com/
