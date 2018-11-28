using Microsoft.EntityFrameworkCore;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data.Contacts {
	public class CustomDbContext<T> : DbContext where T : DbContext {
		public CustomDbContext() {

		}



		public CustomDbContext(DbContextOptions<T> options) : base(options) {
		}
	}
}
