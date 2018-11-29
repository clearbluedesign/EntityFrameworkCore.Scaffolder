using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data.Models;
using Microsoft.EntityFrameworkCore;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data.Abstractions {
	public abstract class CustomDbContext<T> : DbContext where T : DbContext {
		public DbSet<Category> Categories { get; set; }
		public DbSet<Product> Products { get; set; }



		public CustomDbContext() {

		}



		public CustomDbContext(DbContextOptions<T> options) : base(options) {
		}
	}
}
