using Microsoft.AspNetCore.Mvc;
using Models;
using Sharbp.HelpingTools;
using Sharbp.Services;
using Sharbp.ViewModel;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Sharbp.Controllers
{
	public class UserController : Controller
	{
		private RegisterService _registerService;
		private LoginService _loginService;
		private JsonFilePostService _post;
		private JsonFileUserService _user;

		public UserController(RegisterService registerService, LoginService login, JsonFilePostService post, JsonFileUserService user)
		{
			this._registerService = registerService;
			this._loginService = login;
			this._post = post;
			this._user = user;
		}

		#region RegisterUser

		[HttpGet]
		public IActionResult Register()
		{
			RegisterUserViewModel model = new RegisterUserViewModel();

			return View(model);
		}

		[HttpPost]
		public IActionResult Register(RegisterUserViewModel model)
		{
			if (ModelState.IsValid)
			{
				if (model.Password != model.ConfirmPassword)
				{
					ModelState.AddModelError("passConfirm", "Паролите не съвпадат!");
					return View(model);
				}

				if (!model.TermsAndConditions)
				{
					ModelState.AddModelError("termsAndCond", "Не сте се съгласили с Условията за поверителност");
					return View(model);
				}

				string firstName = model.FirstName;
				string lastName = model.LastName;
				string username = model.Username;
				string password = model.Password;

				try
				{
					Logged.User = _registerService.RegisterUser(username, firstName, lastName, password);
				}
				catch (Exception)
				{
					ModelState.AddModelError("usernameExists", "Потребител със същото име вече съществува");
					return View(model);
				}
			}
			else
			{
				return View(model);
			}

			return RedirectToAction("Index", "Home");
		}

		#endregion

		#region LoginUser

		[HttpGet]
		public IActionResult Login()
		{
			LoginUserViewModel model = new LoginUserViewModel();

			return View(model);
		}

		[HttpPost]
		public IActionResult Login(LoginUserViewModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					string username = model.Username;
					string password = model.Password;

					Logged.User = this._loginService.Login(username, password);
				}
				catch(Exception)
				{
					ModelState.AddModelError("loginError", "Невалидно потребителско име или парола");
					return View(model);
				}
			}
			else
			{
				return View(model);
			}

			return RedirectToAction("Index", "Home");
		}

		#endregion

		#region ProfileUser

		[HttpGet]
		[Route("user/profile/{username}")]
		public IActionResult Profile([FromRoute] string username)
		{
			User user = _user.Users.FirstOrDefault(x => x.Username == username);

			if(user is null)
			{
				return NotFound();
			}

			dynamic model = new ExpandoObject();

			if (user.PostsId is not null)
			{
				List<Post> posts = _post.Posts.Where(x => user.PostsId.Contains(x.Id)).Where(x => x.IsApprovedByAdmin).ToList();
				model.UserPosts = posts;
			}
			else
			{
				model.UserPosts = null;
			}

			model.User = user;

			return View(model);
		}

		#endregion

		public IActionResult TermsAndConditions()
        {
			return View();
        }

		public IActionResult SigningOut()
		{
			Logged.User = null;

			return RedirectToAction("Index", "Home");
		}
	}
}
