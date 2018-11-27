using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web {
	public class Program {
		public static void Main(String[] args) {
			CreateWebHostBuilder(args)
				.Build()
				.Run();
		}




		public static IWebHostBuilder CreateWebHostBuilder(String[] args) {
			return WebHost.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((context, builder) => {
					builder.AddJsonFile("appsettings.json", false, true);
				})
				.UseStartup<Startup>();
		}
	}
}
