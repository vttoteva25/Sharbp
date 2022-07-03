using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Models;
using Sharbp.HelpingTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sharbp.Services
{
	public class JsonFilePostService
	{
		private readonly JsonFileUserService _userService;

		public JsonFilePostService(IWebHostEnvironment webHostEnvironment, JsonFileUserService userService)
		{
			this.WebHostEnvironment = webHostEnvironment;
			this._userService = userService;

			if (File.ReadAllText(JsonFileName).Any())
			{
				this.Posts = GetPosts();
			}
			else
			{
				this.Posts = new List<Post>();
			}
		}

		public IWebHostEnvironment WebHostEnvironment { get; }

		public List<Post> Posts { get; set; }

		private string JsonFileName
		{
			get
			{
				return Path.Combine(WebHostEnvironment.WebRootPath, "data", "posts.json");
			}
		}

		public List<Post> GetPosts()
		{
			using (var jsonFileReader = File.OpenText(JsonFileName))
			{
				return JsonSerializer.Deserialize<List<Post>>(jsonFileReader.ReadToEnd(),
					new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});
			}
		}

		public void AddNewPost(string title, string content, string imgUrl)
		{
			Post post = new Post
			{
				Id = Guid.NewGuid().ToString(),
				Title = title,
				Content = content,
				IsApprovedByAdmin = false,
				CreationDate = DateTime.Now,
				UpdateDate = DateTime.Now,
				ImageUrl = imgUrl,
			};

			if (Logged.User.PostsId == null)
			{
				_userService.Users.Find(x => x.Username == Logged.User.Username).PostsId = new string[] { post.Id };
			}
			else
			{
				List<string> postsIds = _userService.Users.Find(x => x.Username == Logged.User.Username).PostsId.ToList();
				postsIds.Add(post.Id);
				_userService.Users.Find(x => x.Username == Logged.User.Username).PostsId = postsIds.ToArray();
			}
			
			Logged.User.PostsId = _userService.Users.Find(x => x.Username == Logged.User.Username).PostsId.ToArray();

			this.Posts.Add(post);
			SaveChangesToFile();
			_userService.SaveChangesToFile();
		}

		public void DeletePost(Post post)
		{
			this.Posts.Remove(post);
			var PostIdList = _userService.Users.Where(x => x.PostsId is not null).First(x => x.PostsId.Contains(post.Id)).PostsId.ToList();
			PostIdList.Remove(post.Id);
			_userService.Users.Where(x => x.PostsId is not null).First(x => x.PostsId.Contains(post.Id)).PostsId = PostIdList.ToArray();
			_userService.SaveChangesToFile();
			this.SaveChangesToFile();
		}

		public void EditPost(Post postForUpdate, string title, string content, string imgUrl)
		{
			this.Posts.Remove(postForUpdate);

			postForUpdate.Title = title;
			postForUpdate.Content = content;
			postForUpdate.UpdateDate = DateTime.Now;
			postForUpdate.IsApprovedByAdmin = false;

			this.Posts.Add(postForUpdate);
			SaveChangesToFile();
			_userService.SaveChangesToFile();
		}

		public bool LeaveKudo(Post post)
		{
			if(post.KudosBy == null)
			{
				post.KudosBy = new string[] { Logged.User.Username };
				SaveChangesToFile();

				return true;
			}
			else
			{
				if (post.KudosBy.Contains(Logged.User.Username))
				{
					return false;
				}
				else
				{
					List<string> kudosByUsers = post.KudosBy.ToList();
					kudosByUsers.Add(Logged.User.Username);
					post.KudosBy = kudosByUsers.ToArray();
					SaveChangesToFile();

					return true;
				}
			}
		}

		public void SaveChangesToFile()
		{
			File.Delete(JsonFileName);
			using (var outputStream = File.OpenWrite(JsonFileName))
			{
				JsonSerializer.Serialize<List<Post>>(
					new Utf8JsonWriter(outputStream, new JsonWriterOptions
					{
						SkipValidation = true,
						Indented = true
					}),
					this.Posts
				);
			}
		}
	}
}
