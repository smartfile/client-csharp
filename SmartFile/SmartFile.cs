using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections;

using SmartFile.Errors;

namespace SmartFile
{
	namespace Errors
	{
		public class APIError : Exception
		{
			public APIError(string message)
			{
			}
		}

		public class RequestError : APIError
		{
			public RequestError(string message)
				: base(message)
			{
			}
		}

		public class ResponseError: APIError
		{
			private WebException error;

			public ResponseError(WebException e)
				: base("Server responded with error")
			{
				this.error = e;
			}

			public int StatusCode
			{
				get {
					return (int)this.error.Status;
				}
			}

			public WebResponse Response
			{
				get {
					return this.error.Response;
				}
			}
		}
	}

	internal class Util
	{
		internal class PostItem
		{
			public string Name;
			public string Data;
			public string Boundary;
			protected byte[] _header = null;
			
			protected PostItem(string name, string boundary)
			{
				this.Name = name;
				this.Boundary = boundary;
			}
			
			public PostItem(string name, string data, string boundary)
				: this(name, boundary)
			{
				this.Data = data;
			}
			
			public virtual byte[] Header
			{
				get
				{
					if (this._header == null)
					{
						this._header = UTF8Encoding.UTF8.GetBytes(
							string.Format("\r\n--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n",
						              this.Boundary, this.Name));
					}
					return this._header;
				}
			}
			
			public virtual long Length
			{
				get
				{
					long length = 0;
					length += this.Header.LongLength;
					length += UTF8Encoding.UTF8.GetBytes(this.Data).Length;
					return length;
				}
			}
		}
		
		internal class PostFile : PostItem
		{
			public new FileInfo Data;
			public string ContentType = "application/octet-stream";
			
			public PostFile(string name, FileInfo data, string boundary)
				: base(name, boundary)
			{
				this.Data = data;
			}
			
			public PostFile(string name, FileInfo data, string boundary, string contentType)
				: this(name, data, boundary)
			{
				this.ContentType = contentType;
			}
			
			public override byte[] Header
			{
				get
				{
					if (this._header == null)
					{
						this._header = UTF8Encoding.UTF8.GetBytes(
							string.Format("\r\n--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
						              this.Boundary, this.Name, this.Data.Name, this.ContentType));
					}
					return this._header;
				}
			}
			
			public override long Length
			{
				get
				{
					long length = 0;
					length += this.Header.LongLength;
					length += this.Data.Length;
					return length;
				}
			}
			
			public FileStream OpenRead()
			{
				return this.Data.OpenRead();
			}
		}

		public static string CleanToken(string token)
		{
			if (string.IsNullOrEmpty(token)) {
				throw new APIError("null or empty");
			}
			token = token.Trim();
			if (token.Length < 30) {
				throw new APIError("too short");
			}
			return token;
		}

		public static string UrlEncode(Hashtable data)
		{
			string pre = "";
			StringBuilder enc = new StringBuilder();
			foreach (string key in data.Keys)
			{
				enc.AppendFormat("{0}{1}={2}", pre, key,
				           	     HttpUtility.UrlEncode(data[key].ToString()));
				pre = "&";
			}
			return enc.ToString();
		}

		public static void HttpForm(HttpWebRequest request, Hashtable data)
		{
			// No files, use simple form encoding.
			byte[] postBytes = UTF8Encoding.UTF8.GetBytes(Util.UrlEncode(data));
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = postBytes.Length;
			using (Stream requestStream = request.GetRequestStream())
			{
				requestStream.Write(postBytes, 0, postBytes.Length);
			}
		}
	
		public static void HttpMultipart(HttpWebRequest request, Hashtable data, Hashtable files)
		{
			// We have files, so use the more complex multipart encoding.
			string boundary = string.Format("----------------------------{0}",
			                                DateTime.Now.Ticks.ToString("x"));
			request.ContentType = string.Format ("multipart/form-data; boundary={0}",
			                                     boundary);
			ArrayList items = new ArrayList();
			// Determine the amount of data we will be sending
			long length = 0;
			foreach (string key in data.Keys)
			{
				PostItem item = new PostItem(key, data[key].ToString(), boundary);
				items.Add(item);
				length += item.Length;
			}
			foreach (string key in files.Keys)
			{
				PostFile file = new PostFile(key, (FileInfo)data[key], boundary);
				items.Add(file);
				length += file.Length;
			}
			request.ContentLength = length;
			// Now stream the data.
			using (Stream requestStream = request.GetRequestStream())
			{
				foreach (PostItem item in items)
				{
					requestStream.Write(item.Header, 0, item.Header.Length);
					if (item.GetType() == typeof(PostFile))
					{
						FileStream fileData = ((PostFile)item).OpenRead();
						byte[] buffer = new byte[32768];
						int read = 0;
						while ((read = fileData.Read (buffer, 0, buffer.Length)) != 0)
						{
							requestStream.Write(buffer, 0, read);
						}
					} else {
						byte[] itemData = UTF8Encoding.UTF8.GetBytes(item.Data);
						requestStream.Write(itemData, 0, itemData.Length);
					}
				}
			}
		}

		public static void HttpSendData(HttpWebRequest request, Hashtable data)
		{
			// Find files, and move them to a separate handler.
			Hashtable files = new Hashtable();
			foreach (string key in data.Keys)
			{
				object obj = data[key];
				if (obj.GetType() == typeof(FileInfo))
				{
					files[key] = obj;
					data.Remove(key);
				}
			}
			if (files.Count == 0)
			{
				Util.HttpForm(request, data);
			} else {
				Util.HttpMultipart(request, data, files);
			}
		}
	}

	public abstract class Client
	{
		protected const string API_URL = "https://app.smartfile.com/";
		protected const string API_VER = "2.1";
		protected const string HTTP_USER_AGENT = "SmartFile C# API client v{0}";
		protected const string THROTTLE_PATTERN = "^.*; next=([\\d\\.]+) sec$";

		public string url;
		public string version;
		public bool throttleWait;
		public Regex throttleRegex;

		public Client(string url = API_URL, string version = API_VER, bool throttleWait = true)
		{
			this.url = url;
			this.version = version;
			this.throttleWait = throttleWait;
			this.throttleRegex = new Regex(THROTTLE_PATTERN);
		}

		protected virtual HttpWebResponse _DoRequest(HttpWebRequest request, Hashtable data)
		{
			HttpWebResponse response;
			try
			{
				if (data != null)
				{
					Util.HttpSendData(request, data);
				}
				response = request.GetResponse() as HttpWebResponse;
			} catch (WebException e)
			{
				throw new ResponseError(e);
			}
			// figure out the return type we want...
			return response;
		}
	
		protected HttpWebResponse _Request(string method, string endpoint, object id = null, Hashtable data = null, Hashtable query = null)
		{
			ArrayList parts = new ArrayList();
			parts.Add("api");
			parts.Add(this.version);
			parts.Add(endpoint);
			if (id != null)
				parts.Add(id.ToString());
			string path = string.Join("/", (string[])parts.ToArray());
			if (!path.EndsWith("/"))
				path += "/";
			while (path.Contains("//"))
				path = path.Replace("//", "/");
			StringBuilder url = new StringBuilder();
			url.Append(this.url);
			url.Append(path);
			if (query != null)
			{
				url.Append(Util.UrlEncode(query));
			}
			HttpWebRequest request = HttpWebRequest.Create(url.ToString()) as HttpWebRequest;
			request.Method = method;
			request.UserAgent = string.Format(HTTP_USER_AGENT, this.version);
			int trys = 0;
			for (; trys < 3; trys++)
			{
				try
				{
					return this._DoRequest(request, data);
				} catch (ResponseError e)
				{
					if (this.throttleWait && e.StatusCode == 503)
					{
						string throttleHeader = e.Response.Headers.Get("x-throttle");
						if (!string.IsNullOrEmpty(throttleHeader))
						{
							float wait;
							Match m = this.throttleRegex.Match(throttleHeader);
							if (float.TryParse(m.Groups[1].Value, out wait))
							{
								Thread.Sleep((int)(wait*1000));
								continue;
							}
						}
					}
					throw e;
				}
			}
			throw new RequestError(string.Format("Could not complete request after {0} trys.", trys));
		}

		public HttpWebResponse Get(string endpoint, object id = null, Hashtable data = null)
		{
			return this._Request("GET", endpoint, id, query: data);
		}

		public HttpWebResponse Put(string endpoint, object id = null, Hashtable data = null)
		{
			return this._Request("PUT", endpoint, id, data: data);
		}

		public HttpWebResponse Post(string endpoint, object id = null, Hashtable data = null)
		{
			return this._Request("POST", endpoint, id, data: data);
		}

		public HttpWebResponse Delete(string endpoint, object id = null, Hashtable data = null)
		{
			return this._Request("DELETE", endpoint, id, data: data);
		}
	}

	public class BasicClient : Client
	{
		private string key;
		private string password;

		public BasicClient(string key = null, string password = null, string url = Client.API_URL, string version = Client.API_VER, bool throttleWait = true)
		{
			if (key == null) {
				key = Environment.GetEnvironmentVariable("SMARTFILE_API_KEY");
			}
			if (key == null) {
				password = Environment.GetEnvironmentVariable("SMARTFILE_API_PASSWORD");
			}
			try
			{
				this.key = Util.CleanToken(key);
				this.password = Util.CleanToken(password);
			} catch (APIError)
			{
				throw new APIError("Please provide an API key and password. Use arguments or environment variables.");
			}
		}

		protected override HttpWebResponse _DoRequest(HttpWebRequest request, Hashtable data)
		{
			string authInfo = this.key + ":" + this.password;
			authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
			request.Headers["Authorization"] = "Basic " + authInfo;
			return base._DoRequest(request, data);
		}
	}
}
