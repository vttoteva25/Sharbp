using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Models
{
	public class Post
	{
		[Key]
		public string Id { get; set; }

		[JsonPropertyName("img")]
		public string ImageUrl { get; set; }

		[Required]
		public string Title { get; set; }

		[Required]
		public string Content { get; set; }

		public DateTime CreationDate { get; set; }
		
		public DateTime UpdateDate { get; set; }

		public bool IsApprovedByAdmin { get; set; }

		public string[] KudosBy { get; set; }

		public override string ToString()
		{
			return JsonSerializer.Serialize<Post>(this);
		}
	}
}
