using System;
using System.Net;

namespace SmartFileAPI {
	// A simple Exception class that can handle the HTTP
	// status.
	class SmartFileException : Exception {
		private HttpStatusCode status;

		public SmartFileException(HttpStatusCode status, string error) : base(error) {
			this.status = status;
		}
		
		public override string ToString() {
			return this.Message;
		}
		
		public HttpStatusCode StatusCode {
			get {
				return this.status;
			}
		}
	}
}
