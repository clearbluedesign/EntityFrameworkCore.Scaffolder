using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data {
	/// <summary>
	/// Custom <see cref="IDesignTimeServices"/> implementation.
	/// </summary>
	public class DesignTimeServices : IDesignTimeServices {
		/// <summary>
		/// Configures design-time services.
		/// </summary>
		public void ConfigureDesignTimeServices(IServiceCollection services) {
			services.AddScaffolder();
		}
	}
}
