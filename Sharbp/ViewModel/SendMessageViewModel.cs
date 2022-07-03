using System.ComponentModel.DataAnnotations;

namespace Sharbp.ViewModel
{
	public class SendMessageViewModel
	{
		[Required(ErrorMessage = "Заглавието е задължително")]
		public string Title { get; set; }

		[Required(ErrorMessage = "Съдържанието е задължително")]
		public string Content { get; set; }

		[Required(ErrorMessage = "Имейлът за връзка е задължителен")]
		public string Email { get; set; }
	}
}
