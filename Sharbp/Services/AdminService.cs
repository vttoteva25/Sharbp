using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;

namespace Sharbp.Services
{
    public class AdminService
    {
        private readonly JsonFileUserService _userService;
        private readonly JsonFilePostService _postService;
        public AdminService(JsonFileUserService userService, JsonFilePostService postService)
        {
            this._userService = userService;
            this._postService = postService;
        }
        public void RemoveUser(string username)
        {
            if (_userService.Users.Any(x => x.Username == username))
            {
				User user = _userService.Users.Find(x => x.Username == username);
                if(user.PostsId is not null)
				{
                    foreach (string postId in user.PostsId)
                    {
                        Post post = _postService.Posts.Find(x => x.Id == postId);
                        _postService.DeletePost(post);
                    }
                }

                _userService.Users.Remove(user);
                _userService.SaveChangesToFile();
            }
            else throw new ArgumentException("User Not Found");
        }
        public void RemovePost(Post post)
        {
            if (_postService.Posts.Contains(post))
            {
                _postService.Posts.Remove(post);
                _postService.SaveChangesToFile();
            }
            else throw new ArgumentException("Post not found");
        }
        public void ApprovePost(Post post)
        {
            if (_postService.Posts.Contains(post))
            {
                _postService.Posts.Find(x=>x==post).IsApprovedByAdmin = true;
            }
            else throw new ArgumentException("Post not found");
        }
    }
}
