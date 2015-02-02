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
           public static int Main (string[] args)
           {
               // Setup new SmartFile client
               BasicClient api = new BasicClient();
               
               // Data to send in POST request
               Hashtable p = new Hashtable();
               p.Add("file0", new FileInfo("vacation09.jpg"));
   
               // Upload file to /vacation09.jpg
               HttpWebResponse r = api.Post("/path/data", null, p);
               
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

Other endpoints can be found here: https://app.smartfile.com/api/
 
.. _SmartFile: https://www.smartfile.com/
