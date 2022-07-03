using System.ComponentModel.DataAnnotations;

namespace Sharbp.ViewModel
{
	public class RegisterUserViewModel
	{
		[Required(ErrorMessage = "Потребителското име не може да е празно")]
		public string Username { get; set; }

		[Required(ErrorMessage = "Името не може да е празно")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Фамилията не може да е празна")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "Паролата не може да е празна")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Паролата за потвържедние не може да е празна")]
		public string ConfirmPassword { get; set; }

		public bool TermsAndConditions { get; set; }
	}
}
