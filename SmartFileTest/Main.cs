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
		public static void Main (string[] args)
		{
			BasicClient api = new BasicClient();

			Hashtable p = new Hashtable();
			p.Add("file", new FileInfo(""));
			HttpWebResponse r = api.Post ("path/data", p);
		}
	}
}
