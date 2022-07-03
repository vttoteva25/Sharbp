using Models;
using System.Collections.Generic;

namespace Repositories
{
	public interface IPostsRepository
	{
		IEnumerable<Post> GetAllPosts();

		void AddNewPost(Post post);
	}
}
