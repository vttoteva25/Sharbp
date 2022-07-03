﻿using System.ComponentModel.DataAnnotations;

namespace Sharbp.ViewModel
{
	public class LoginUserViewModel
	{
		[Required(ErrorMessage = "Полето за потребителско име не може да е празно")]
		public string Username { get; set; }

		[Required(ErrorMessage = "Полето за парола не може да е празно")]
		public string Password { get; set; }
	}
}
