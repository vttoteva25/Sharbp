using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Models;
using Sharbp.HelpingTools;
using Sharbp.Services;
using Sharbp.ViewModel;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sharbp.Controllers
{
	public class PostController : Controller
	{
		private JsonFilePostService _postService;
		private IWebHostEnvironment _webHostEnv;
		private JsonFileUserService _userService;

		public PostController(JsonFilePostService postService, IWebHostEnvironment webHostEnvironment, JsonFileUserService userService)
		{
			this._postService = postService;
			this._webHostEnv = webHostEnvironment;
			this._userService = userService;
		}

		#region CreatePost
		[Route("post/create")]
		[HttpGet]
		public IActionResult Create()
		{
			if(Logged.User is not null)
			{
				CreatePostViewModel model = new CreatePostViewModel();

				return View(model);
			}

			return RedirectToAction("Login", "User");
		}

		[HttpPost]
		public IActionResult Create(CreatePostViewModel model)
		{
			if (ModelState.IsValid)
			{
				if (model.Image != null)
				{
					string Pathern = Path.Combine(_webHostEnv.WebRootPath, "images", "UploadedImages");
					string fileName = Guid.NewGuid() + "-" + model.Image.FileName;
					string filePathern = Path.Combine(Pathern, fileName);
					using (var fileStream = new FileStream(filePathern, FileMode.Create))
					{
						model.Image.CopyTo(fileStream);
					}
					this._postService.AddNewPost(model.Title, model.Content, Path.Combine("images", "UploadedImages", fileName));
				}
				else
				{
					this._postService.AddNewPost(model.Title, model.Content, "/images/default.png");
				}

				return RedirectToAction("Index", "Home");
			}

			return View(model);
		}
		#endregion

		#region OpenPost
		[Route("post/{id}")]
		[HttpGet]
		public IActionResult Open([FromRoute] string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();
			}

			Post post = _postService.Posts.FirstOrDefault(x => x.Id == id);

			if (post == null)
			{
				return NotFound();
			}

			dynamic model = new ExpandoObject();
			model.PostId = post.Id;

			List<User> usersWithContent = _userService.GetUsers().Where(x => x.PostsId is not null).ToList();
			if (usersWithContent.Any())
			{
				model.User = usersWithContent.First(x => x.PostsId.Contains(post.Id));
			}

			return View(model);
		}
		#endregion

		#region EditPost
		[Route("post/edit/{id}")]
		[HttpGet]
		public IActionResult Edit([FromRoute] string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();
			}

			if (!Logged.User.PostsId.Contains(id)&&Logged.User.Username!="Admin")
			{
				return Unauthorized();
			}

			Post post = _postService.Posts.FirstOrDefault(x => x.Id == id);

			if (post == null)
			{
				return NotFound();
			}

			EditPostViewModel model = new EditPostViewModel();
			model.Id = post.Id;
			model.Title = post.Title;
			model.ImageUrl = post.ImageUrl;
			model.Image = null;
			model.Content = post.Content;

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(EditPostViewModel model)
		{
			if (ModelState.IsValid)
			{
				Post updatePost = _postService.Posts.FirstOrDefault(x => x.Id == model.Id);

				if(updatePost is null)
				{
					return NotFound();
				}

				string imgUrl = "";

				if (model.Image != null)
				{
					string Pathern = Path.Combine(_webHostEnv.WebRootPath, "images", "UploadedImages");
					string fileName = Guid.NewGuid() + "-" + model.Image.FileName;
					string filePathern = Path.Combine(Pathern, fileName);
					using (var fileStream = new FileStream(filePathern, FileMode.Create))
					{
						model.Image.CopyTo(fileStream);
					}
					imgUrl = Path.Combine("images", "UploadedImages", fileName);
				}
				else
				{
					imgUrl = "~/images/default.png";
				}

				_postService.EditPost(updatePost, model.Title, model.Content, imgUrl);

				return RedirectToAction("Index", "Home");
			}
			return View(model);
		}
		#endregion

		[HttpGet]
		[Route("post/delete/{id}")]
		public IActionResult Delete([FromRoute] string id)
		{
			if(id is null)
			{
				return NotFound();
			}

			Post postForDelete = _postService.Posts.FirstOrDefault(x => x.Id == id);

			if(postForDelete is null)
			{
				return NotFound();
			}

			User user = _userService.Users.Where(x => x.PostsId is not null).FirstOrDefault(x => x.PostsId.Contains(id));

			if (user is not null)
			{
				if(user.Username == Logged.User.Username || Logged.User.Username  == "Admin")
				{
					_postService.DeletePost(postForDelete);
				}
				else
				{
					return Unauthorized();
				}
			}
			else
			{
				return Unauthorized();
			}

			return RedirectToAction("Profile", "User", new { username = user.Username });
		}
	}
}
