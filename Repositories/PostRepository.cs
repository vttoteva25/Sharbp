using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
	public class PostRepository : IPostsRepository
	{
		private readonly SharbpDbContext _db;

		public PostRepository(SharbpDbContext db)
		{
			this._db = db;
		}

		public void AddNewPost(Post post)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Post> GetAllPosts()
		{
			return this._db.Posts.Where(x=>x.IsApprovedByAdmin);
		}
	}
}
