using System;
using Bricelam.EntityFrameworkCore.Design;
using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Generators;
using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Options;
using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Services;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.Configuration;



namespace Microsoft.Extensions.DependencyInjection {
	public static class AddScaffolderExtension {
		/// <summary>
		/// Adds scaffolder services.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection" /> the services will be added to.</param>
		/// <returns>The <paramref name="services" />.</returns>
		public static IServiceCollection AddScaffolder(this IServiceCollection services) {
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Environment.CurrentDirectory)
				.AddJsonFile("scaffolder.json", true, true)
				.Build();

			services.Configure<ScaffoldingOptions>(configuration.GetSection("Scaffolding"));
			services.Configure<DbContextOptions>(configuration.GetSection("DbContext"));
			services.Configure<EntityTypeOptions>(configuration.GetSection("EntityType"));

			services.AddSingleton<TypeResolverService>();

			services.AddSingleton<IPluralizer, Pluralizer>();
			services.AddSingleton<IModelCodeGenerator, ModelGenerator>();
			services.AddSingleton<ICSharpDbContextGenerator, DbContextGenerator>();
			services.AddSingleton<ICSharpEntityTypeGenerator, EntityTypeGenerator>();

			return services;
		}
	}
}
