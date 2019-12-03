using System;
using System.Linq;
using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Options;
using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Services;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.Options;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Generators {
	/// <summary>
	/// Used to generate code for <see cref="DbContext"/>.
	/// </summary>
	public class DbContextGenerator : CSharpDbContextGenerator {
		private readonly DbContextOptions dbContextOptions;
		private readonly TypeResolverService typeResolver;
		private readonly IProviderConfigurationCodeGenerator providerCodeGenerators;
		private readonly IAnnotationCodeGenerator annotationCodeGenerator;
		private readonly ICSharpHelper cSharpHelper;



		public DbContextGenerator(
			IOptions<DbContextOptions> dbContextOptionsAccessor,
			TypeResolverService typeResolver,
			IProviderConfigurationCodeGenerator providerCodeGenerators,
			IAnnotationCodeGenerator annotationCodeGenerator,
			ICSharpHelper cSharpHelper
		) : base(
			providerCodeGenerators,
			annotationCodeGenerator,
			cSharpHelper
		) {
			this.dbContextOptions = dbContextOptionsAccessor.Value;
			this.typeResolver = typeResolver;
			this.providerCodeGenerators = providerCodeGenerators;
			this.annotationCodeGenerator = annotationCodeGenerator;
			this.cSharpHelper = cSharpHelper;
		}



		/// <summary>
		/// Generates <see cref="DbContext"/> code.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="contextNamespace">The namespace for context class.</param>
		/// <param name="contextName">The name of the <see cref="DbContext" />.</param>
		/// <param name="connectionString">The connection string.</param>
		/// <param name="useDataAnnotations">A value indicating whether to use data annotations.</param>
		/// <param name="suppressConnectionStringWarning">A value indicating whether to suppress the connection string sensitive information warning.</param>
		/// <returns>The generated <see cref="DbContext"/> code.</returns>
		public override String WriteCode(
			IModel model,
			String contextNamespace,
			String contextName,
			String connectionString,
			Boolean useDataAnnotations,
			Boolean suppressConnectionStringWarning
		) {
			var code = base.WriteCode(
				model,
				contextNamespace,
				contextName,
				connectionString,
				useDataAnnotations,
				suppressConnectionStringWarning
			);

			if (this.dbContextOptions.Base != "DbContext") {
				var type = this.typeResolver.GetType(this.dbContextOptions.Base);

				if (type != null) {
					var lines = code.Split(Environment.NewLine).ToList();
					var lastUsing = lines.LastOrDefault(l => l.StartsWith("using"));
					var lastUsingIndex = lines.IndexOf(lastUsing);

					lines.Insert(lastUsingIndex + 1, $"using {type.Namespace};");

					var inheritedDbSets = type.GetProperties()
						.Where(p => p.PropertyType.Name == "DbSet`1")
						.Select(p => p.Name);

					foreach (var entityType in model.GetEntityTypes()) {
						if (inheritedDbSets.Contains(entityType.GetDbSetName())) {
							var dbSetLine = lines.Find(l => l.Contains($"DbSet<{entityType.Name}> {entityType.GetDbSetName()}"));
							var dbSetLineIndex = lines.IndexOf(dbSetLine);

							lines.RemoveAt(dbSetLineIndex);
						}
					}

					code = String.Join(Environment.NewLine, lines);

					code = code.Replace($"{contextName} : DbContext", $"{contextName} : {type.DisplayName(false)}");
				}
			}

			return code;
		}
	}
}
