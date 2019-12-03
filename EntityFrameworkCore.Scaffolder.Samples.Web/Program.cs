using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web {
	public class Program {
		public static void Main(String[] args) {
			CreateWebHostBuilder(args)
				.Build()
				.Run();
		}



		public static IHostBuilder CreateWebHostBuilder(String[] args) {
			return Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder => {
					webBuilder.UseStartup<Startup>();
				});
		}
	}
}
