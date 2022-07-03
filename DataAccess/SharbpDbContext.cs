using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
	public class SharbpDbContext : DbContext
	{
		public SharbpDbContext()
		{
		}

		public SharbpDbContext(DbContextOptions<SharbpDbContext> options) : base(options)
		{
		}

		public DbSet<Post> Posts { get; set; }
	}
}
