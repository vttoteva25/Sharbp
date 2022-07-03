using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Sharbp.Services;
using Sharbp.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sharbp.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly JsonFilePostService _post;
		private readonly JsonFileUserService _user;
		private readonly RegisterService _register;
		private readonly LoginService _login;
		private readonly JsonFileMessageService _message;

		public HomeController(ILogger<HomeController> logger, JsonFilePostService service, JsonFileUserService user,
			RegisterService register, LoginService login, JsonFileMessageService message)
		{
			_logger = logger;
			_post = service;
			_user = user;
			_register = register;
			_login = login;
			_message = message;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[Route("/aboutus")]
		[HttpGet]
		public IActionResult AboutUs()
		{
			return View();
		}

		[Route("/contactus")]
		[HttpGet]
		public IActionResult ContactUs()
		{
			SendMessageViewModel model = new SendMessageViewModel();

			return View(model);
		}

		[Route("/contactus")]
		[HttpPost]
		public IActionResult ContactUs(SendMessageViewModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					_message.AddMessage(model.Title, model.Content, model.Email);
				}
				catch (Exception)
				{
					return View(model);
				}

				return RedirectToAction("Index", "Home");
			}

			return View(model);
		}
	}
}
