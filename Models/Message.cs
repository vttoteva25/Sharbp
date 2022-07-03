using System;

namespace Models
{
	public class Message
	{
		public string Id { get; set; }

		public string Title { get; set; }

		public string Content { get; set; }

		public string UserEmail { get; set; }

		public DateTime CreationDate { get; set; }

		public bool IsRead { get; set; }
	}
}
