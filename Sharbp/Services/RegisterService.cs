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
	public class RegisterService
	{
		private readonly JsonFileUserService _userService;

		public RegisterService(JsonFileUserService userService)
		{
			this._userService = userService;
		}


		public User RegisterUser(string username, string firstName, string lastName, string password)
		{
			return _userService.AddNewUser(username, firstName, lastName, Hasher.Hash(password));
		}
	}
}
