using System;

namespace SmartFileAPI {
	class MainClass {
		// A short function to ask the user a question and
		// return their response.
		private static string Prompt(string prompt) {
			Console.Write(prompt);
			return Console.ReadLine();
		}

		// Start things off in Main()
		public static void Main(string[] args) {
			// Ask the user for the required parameters. These will be
    		// passed to the API via an HTTP POST request.
			string fullname = Prompt("Please enter a full name: ");
			string username = Prompt("Please enter a username: ");
			string password = Prompt("Please enter a password: ");
			string email = Prompt("Please enter an email address: ");
			try {
				// Try to create the new user...
				SmartFileAPI.CreateUser(fullname, username, password, email);
				Console.WriteLine("Successfully created user {0}.", username);
			}
			catch (SmartFileException e) {
				// Print the server response on failure.
				Console.WriteLine("Error creating user {0}: {1}", username, e.Message);
			}
		}
	}
}