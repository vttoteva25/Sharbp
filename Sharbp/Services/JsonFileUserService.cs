using Microsoft.AspNetCore.Hosting;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sharbp.Services
{
	public class JsonFileUserService
	{
		public JsonFileUserService(IWebHostEnvironment webHostEnvironment)
		{
			this.WebHostEnvironment = webHostEnvironment;

			if (File.ReadAllText(JsonFileName).Any())
			{
				this.Users = GetUsers();
			}
			else
			{
				this.Users = new List<User>();
			}
		}

		public IWebHostEnvironment WebHostEnvironment { get; }

		public List<User> Users { get; set; }

		private string JsonFileName
		{
			get
			{
				return Path.Combine(WebHostEnvironment.WebRootPath, "data", "users.json");
			}
		}

		public List<User> GetUsers()
		{
			using (var jsonFileReader = File.OpenText(JsonFileName))
			{
				return JsonSerializer.Deserialize<List<User>>(jsonFileReader.ReadToEnd(),
					new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});
			}
		}

		public User AddNewUser(string username, string firstName, string lastName, string password)
		{
			User user = new User
			{
				Username = username,
				Password = password,
				FirstName = firstName,
				LastName = lastName
			};

			if (!CheckForExistingUser(username))
			{
				this.Users.Add(user);
				SaveChangesToFile();

				return user;
			}
			else
			{
				throw new ArgumentException("A user with the same username already exists");
			}
		}

		public void SaveChangesToFile()
		{
			File.Delete(JsonFileName);
			using (var outputStream = File.OpenWrite(JsonFileName))
			{
				JsonSerializer.Serialize<List<User>>(
					new Utf8JsonWriter(outputStream, new JsonWriterOptions
					{
						SkipValidation = true,
						Indented = true
					}),
					this.Users
				);
			}
		}

		private bool CheckForExistingUser(string username) => this.Users.Any(x => x.Username == username);
	}
}
