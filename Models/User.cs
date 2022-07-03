using System.Text.Json;

namespace Models
{
	public class User
	{
		public string Username { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Password { get; set; }

		public string[]? PostsId { get; set; }

		public override string ToString()
		{
			return JsonSerializer.Serialize<User>(this);
		}
	}
}
