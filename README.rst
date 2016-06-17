.. image:: https://d2xtrvzo9unrru.cloudfront.net/brands/smartfile/logo.png
   :alt: SmartFile

A `SmartFile`_ Open Source SDK. Developer information: https://www.smartfile.com/developer/


SmartFile API Client (C#)
=========================
This is a fully working API client for SmartFile. It allows you to interact with all objects within the SmartFile system via the REST API. This client library can be used as-is for your integration projects.

File transfers
--------------
.. code:: csharp

using System;
using System.IO;
using System.Web;
using System.Net;
using System.Collections;

using SmartFile;

namespace SmartFileTest
{
    class MainClass
    {

        public static int Main(string[] args)
        {
            // Setup new SmartFile client
            BasicClient api = new BasicClient("*****", "*****");

            HttpWebResponse r = api.Upload("MyFile.txt");

            return 0;
        }
    }
}


Create a directory
------------------
.. code:: csharp

   using System;
   using System.IO;
   using System.Web;
   using System.Net;
   using System.Collections;
   
   using SmartFile;
   
   namespace SmartFileTest
   {
       class MainClass
       {
           public static int Main (string[] args)
           {
               // Setup new SmartFile client
               BasicClient api = new BasicClient();
               
               // Data to send in POST request
               Hashtable p = new Hashtable();
               p.Add("path", "newdirectory");
               
               // Create new directory at /newdirectory
               System.Net.WebResponse r = api.Post("/path/oper/mkdir/", null, p);
   
               return 0;
           }
       }
   }
   
   
Create a link
------------------
.. code:: csharp

   using System;
   using System.IO;
   using System.Web;
   using System.Net;
   using System.Collections;
   
   using SmartFile;
   
   namespace SmartFileTest
   {
       class MainClass
       {
           public static int Main(string[] args)
           {
               // Setup new SmartFile client
               BasicClient api = new BasicClient("xxxxxxxxxx", "xxxxxxxxxxxx");
   
               // Data to send in POST request
               Hashtable p = new Hashtable();
               p.Add("path", "/Public/mvp.jpg");
               p.Add("list", true);
               p.Add("read", true);
               p.Add("name", "Screenshot");
   
               // Create a link via POST request
               HttpWebResponse r = api.Post("/link", null, p);
   
               // Display output on the console
               using (var streamReader = new StreamReader(r.GetResponseStream()))
               {
                   var responseText = streamReader.ReadToEnd();
                   Console.WriteLine(responseText);
                   Console.ReadKey();
               }
   
               return 0;
           }
       }
   }

Other endpoints can be found here: https://app.smartfile.com/api/
 
.. _SmartFile: https://www.smartfile.com/
