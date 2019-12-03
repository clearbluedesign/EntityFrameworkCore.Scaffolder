using System.Text.Json;
using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web {
	public class Startup {
		private IConfiguration configuration { get; }



		public Startup(IConfiguration configuration) {
			this.configuration = configuration;
		}



		public void ConfigureServices(IServiceCollection services) {
			services.AddDbContext<DataContext>(options => {
				options.UseLazyLoadingProxies();
				options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection"));
			});

			services
				.AddControllers()
				.AddJsonOptions(options => {
					options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
				});
		}



		public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseEndpoints(endpoints => {
				endpoints.MapControllers();
			});
		}
	}
}
