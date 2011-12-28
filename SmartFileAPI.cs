using System;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;
using System.Text;
using System.Collections;

namespace SmartFileAPI {
	class SmartFileAPI {
	    // These constants are needed to access the API.
		private static string API_URL = "http://app.smartfile.com/api/1";
		private static string API_KEY = "api-key";
		private static string API_PWD = "api-password";

		// This function does the bulk of the work by performing
		// the HTTP request and raising an exception for any HTTP
		// status code other than 201.
		private static void httpRequest(string uri, Hashtable data, string method) {
			// We use the XML format for C# because there is no native JSON decoder.
			string url = String.Format("{0}{1}?format=xml", API_URL, uri);
			string auth = String.Format("{0}:{1}", API_KEY, API_PWD);
			auth = Convert.ToBase64String(new ASCIIEncoding().GetBytes(auth));
			HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
			request.Headers.Add("Authorization", String.Format("Basic {0}", auth));
			request.ContentType = "application/x-www-form-urlencoded";
			request.UserAgent = ".NET SmartFile API Sample Client";
			request.KeepAlive = false;
			request.Method = method;
			if (method == "POST") {
				string pre = "";
				StringBuilder post = new StringBuilder();
				foreach (string key in data.Keys) {
					string val = HttpUtility.UrlEncode(data[key].ToString());
					post.Append(String.Format("{0}{1}={2}", pre, key, val));
					pre = "&";
				}
				byte[] byteData = UTF8Encoding.UTF8.GetBytes(post.ToString());
				request.ContentLength = byteData.Length;
				using (Stream requestStream = request.GetRequestStream()) {
					requestStream.Write(byteData, 0, byteData.Length);
				}
			}
			try {
				using (HttpWebResponse response = request.GetResponse() as HttpWebResponse) {
					return;
				}
			}
			catch (WebException e) {
				// Any HTTP status other than 2XX will land us here.
				using (HttpWebResponse response = (HttpWebResponse)e.Response) {
					StreamReader responseStream = new StreamReader(response.GetResponseStream());
					string message = responseStream.ReadToEnd();
					try {
						using (XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(message)))
						{
							bool inMessage = false;
							while (reader.Read())
								if (reader.Name == "message" && reader.NodeType == XmlNodeType.Element)
									inMessage = true;
								else if (inMessage && reader.NodeType == XmlNodeType.Text)
								{
									message = reader.Value;
									break;
								}
						}
					}
					catch (Exception)
					{ }
					throw new SmartFileException(response.StatusCode, message);
				}
			}
		}
		
		// This function makes the User add API call. It uses the httpRequest
		// function to handle the transport. Additional API calls could be supported
		// simply by writing additional wrappers that create the data Hashtable and
		// use httpRequest to do the grunt work.
		public static void CreateUser(string fullname, string username, string password, string email) {
			Hashtable data = new Hashtable();
			data["name"] = fullname;
			data["username"] = username;
			data["password"] = password;
			data["email"] = email;
			httpRequest("/users/add/", data, "POST");
		}
	}
}
