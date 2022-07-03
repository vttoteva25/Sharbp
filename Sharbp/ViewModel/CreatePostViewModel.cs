using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Sharbp.ViewModel
{
	public class CreatePostViewModel
	{
		[Required(ErrorMessage = "Заглавието не може да е празно")]
		public string Title { get; set; }

		[Required(ErrorMessage = "Съдържанието на статията не може да е празно")]
		public string Content { get; set; }

		public string ImageUrl { get; set; }
		public IFormFile Image { get; set; }
	}
}
