using Microsoft.AspNetCore.Hosting;
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
	public class AdminController : Controller
	{
		private JsonFilePostService _postService;
		private IWebHostEnvironment _webHostEnv;
		private JsonFileUserService _userService;
		private AdminService _adminService;
		private JsonFileMessageService _messageService;

		public AdminController(JsonFilePostService postService, IWebHostEnvironment webHostEnvironment,
			JsonFileUserService userService, AdminService adminService, JsonFileMessageService messageService)
		{
			this._postService = postService;
			this._webHostEnv = webHostEnvironment;
			this._userService = userService;
			this._adminService = adminService;
			this._messageService = messageService;
		}

		[HttpGet]
		public IActionResult Index()
		{
			if (Logged.User is not null)
			{
				if (Logged.User.Username == "Admin")
					return View();
				else return Unauthorized();
			}
			return RedirectToAction("Login", "User");
		}

		public IActionResult ApprovePosts()
		{
			if (Logged.User is not null)
			{
				if (Logged.User.Username == "Admin")
				{
					return View();
				}

				else return Unauthorized();
			}
			return RedirectToAction("Login", "User");
		}

		public IActionResult ManagePosts()
		{
			if (Logged.User is not null)
			{
				if (Logged.User.Username == "Admin")
					return View();
				else return Unauthorized();
			}
			return RedirectToAction("Login", "User");
		}

		public IActionResult Users()
		{
			if (Logged.User is not null)
			{
				if (Logged.User.Username == "Admin")
					return View();
				else return Unauthorized();
			}
			return RedirectToAction("Login", "User");
		}

		[Route("Admin/ApprovePost/{id}")]
		[HttpGet]
		public IActionResult ApprovePost([FromRoute] string id)
		{
			if (Logged.User is not null)
			{
				if (Logged.User.Username == "Admin")
				{
					if (string.IsNullOrEmpty(id))
					{
						return NotFound();
					}

					if (_postService.Posts.FirstOrDefault(x => x.Id == id) == null)
					{
						return NotFound();
					}

					_postService.Posts.FirstOrDefault(x => x.Id == id).IsApprovedByAdmin = true;
					_postService.SaveChangesToFile();

					return RedirectToAction("ApprovePosts", "Admin");
				}

				else return Unauthorized();
			}
			else
				return RedirectToAction("Login", "User");


		}

		[Route("Admin/DeletePost/{id}")]
		[HttpGet]
		public IActionResult DeletePost([FromRoute] string id)
		{
			if (Logged.User is not null)
			{
				if (Logged.User.Username == "Admin")
				{
					if (string.IsNullOrEmpty(id))
					{
						return NotFound();
					}

					if (_postService.Posts.FirstOrDefault(x => x.Id == id) == null)
					{
						return NotFound();
					}

					_postService.DeletePost(_postService.Posts.FirstOrDefault(x => x.Id == id));
					return RedirectToAction("ApprovePosts", "Admin");
				}

				else return Unauthorized();
			}
			else
				return RedirectToAction("Login", "User");
		}

		[Route("Admin/DeleteUser/{username}")]
		[HttpGet]
		public IActionResult DeleteUser([FromRoute] string username)
		{
			if (Logged.User is not null)
			{
				if (Logged.User.Username == "Admin")
				{
					if (string.IsNullOrEmpty(username))
					{
						return NotFound();
					}

					if (_userService.Users.FirstOrDefault(x => x.Username == username) == null)
					{
						return NotFound();
					}

					_adminService.RemoveUser(username);
					return RedirectToAction("Users", "Admin");
				}
				else return Unauthorized();
			}
			else
				return RedirectToAction("Login", "User");
		}

		#region Messages

		[HttpGet]
		public IActionResult GetMessages()
		{
			if(Logged.User.Username == "Admin")
			{
				return View("Messages");
			}
			else
			{
				return Unauthorized();
			}
		}

		[Route("message/{id}")]
		[HttpGet]
		public IActionResult Message([FromRoute] string id)
		{
			if(Logged.User.Username == "Admin")
			{
				Message message = _messageService.Messages.FirstOrDefault(x => x.Id == id);

				if(message is null)
				{
					return NotFound();
				}

				message.IsRead = true;
				_messageService.SaveChangesToFile();

				return View(message);
			}

			return Unauthorized();
		}

		[Route("delete/{id}")]
		[HttpGet]
		public IActionResult Delete([FromRoute] string id)
		{
			try
			{
				_messageService.DeleteMessage(id);
			}
			catch (ArgumentNullException)
			{
				return NotFound();
			}

			return RedirectToAction("GetMessages", "Admin");
		}
		#endregion
	}
}
