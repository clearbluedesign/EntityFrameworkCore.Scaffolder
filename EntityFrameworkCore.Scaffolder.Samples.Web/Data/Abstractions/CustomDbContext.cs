using Microsoft.EntityFrameworkCore;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data.Abstractions {
	public abstract class CustomDbContext<T> : DbContext where T : DbContext {
		public CustomDbContext() {

		}



		public CustomDbContext(DbContextOptions<T> options) : base(options) {
		}
	}
}
