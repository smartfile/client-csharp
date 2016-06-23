//using System;
//using System.IO;
//using System.Web;
//using System.Net;
//using System.Collections;

//using SmartFile;

//namespace SmartFileTest
//{
//    class MainClass
//    {
//        public static int Main(string[] args)
//        {
//            /*
//			 * We expect command line arguments such as:
//			 * SmartFileTest.exe <method> <endpoint> [ <id> <name>=<value> ]
//			*/
//            if (args.Length < 3)
//            {
//                Console.WriteLine("You must provide an HTTP method and endpoint.");
//                return 1;
//            }

//            String method = args[0];
//            String endpoint = args[1];

//            BasicClient api = new BasicClient();

//            /* SmartFileTest.exe POST path/data file0= */
//            if (method == "POST" && endpoint == "path/data")
//            {
//                Hashtable p = new Hashtable();
//                for (int i = 2; i < args.Length; i++)
//                {
//                    String argPair = args[i];
//                    String[] parts = argPair.Split('=');
//                    if (File.Exists(parts[1]))
//                        p.Add(parts[0], new FileInfo(parts[1]));
//                    else
//                        p.Add(parts[0], parts[1]);
//                }
//                HttpWebResponse r = api.Post("path/data", null, p);
//            }

//            return 0;
//        }
//    }
//}
