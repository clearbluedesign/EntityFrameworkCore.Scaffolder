using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;



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
				.AddMvc()
				.AddJsonOptions(options => {
					options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
					options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
				});
		}



		public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			}

			app.UseMvc();
		}
	}
}
