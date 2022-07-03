using Microsoft.AspNetCore.Hosting;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Sharbp.HelpingTools;

namespace Sharbp.Services
{
	public class JsonFileMessageService
	{
		private IWebHostEnvironment webHostEnvironment;

		public JsonFileMessageService(IWebHostEnvironment webHostEnvironment)
		{
			this.webHostEnvironment = webHostEnvironment;

			if (File.ReadAllText(JsonFileName).Any())
			{
				this.Messages = GetMessages();
			}
			else
			{
				this.Messages = new List<Message>();
			}
		}

		public List<Message> Messages { get; private set; }

		private string JsonFileName
		{
			get
			{
				return Path.Combine(webHostEnvironment.WebRootPath, "data", "messages.json");
			}
		}

		public List<Message> GetMessages()
		{
			using (var jsonFileReader = File.OpenText(JsonFileName))
			{
				return JsonSerializer.Deserialize<List<Message>>(jsonFileReader.ReadToEnd(),
					new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});
			}
		}

		public void AddMessage(string title, string content, string email)
		{
			Message message = new Message
			{
				Id = Guid.NewGuid().ToString(),
				Title = title,
				Content = content,
				CreationDate = DateTime.Now,
				UserEmail = email,
				IsRead = false
			};

			this.Messages.Add(message);
			this.SaveChangesToFile();
		}

		public void DeleteMessage(string id)
		{
			Message messageForDelete = this.Messages.FirstOrDefault(x => x.Id == id);

			if(messageForDelete is null)
			{
				throw new ArgumentNullException();
			}

			this.Messages.Remove(messageForDelete);
			this.SaveChangesToFile();
		}

		public void SaveChangesToFile()
		{
			File.Delete(JsonFileName);
			using (var outputStream = File.OpenWrite(JsonFileName))
			{
				JsonSerializer.Serialize<List<Message>>(
					new Utf8JsonWriter(outputStream, new JsonWriterOptions
					{
						SkipValidation = true,
						Indented = true
					}),
					this.Messages
				);
			}
		}
	}
}
