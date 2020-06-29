using System;
using System.Linq;
using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.Options;

using ScaffolderDbContextOptions = ClearBlueDesign.EntityFrameworkCore.Scaffolder.Options.DbContextOptions;
using ScaffolderEntityTypeOptions = ClearBlueDesign.EntityFrameworkCore.Scaffolder.Options.EntityTypeOptions;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Generators {
	/// <summary>
	/// Used to generate code for <see cref="DbContext"/>.
	/// </summary>
	public class DbContextGenerator : CSharpDbContextGenerator {
		private readonly TypeResolverService typeResolver;
		private readonly ScaffolderDbContextOptions dbContextOptions;
		private readonly ScaffolderEntityTypeOptions entityOptions;



		public DbContextGenerator(
			TypeResolverService typeResolver,
			IOptions<ScaffolderDbContextOptions> dbContextOptionsAccessor,
			IOptions<ScaffolderEntityTypeOptions> entityOptionsAccessor,
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
			this.entityOptions = entityOptionsAccessor.Value;
		}



		/// <summary>
		/// Generates <see cref="DbContext"/> code.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="contextName">The name of the <see cref="DbContext" />.</param>
		/// <param name="connectionString">The connection string.</param>
		/// <param name="contextNamespace">The namespace for context class.</param>
		/// <param name="modelNamespace"></param>
		/// <param name="useDataAnnotations">A value indicating whether to use data annotations.</param>
		/// <param name="suppressConnectionStringWarning">A value indicating whether to suppress the connection string sensitive information warning.</param>
		/// <returns>The generated <see cref="DbContext"/> code.</returns>
		public override String WriteCode(
			IModel model,
			String contextName,
			String connectionString,
			String contextNamespace,
			String modelNamespace,
			Boolean useDataAnnotations,
			Boolean suppressConnectionStringWarning
		) {
			var code = base.WriteCode(
				model,
				contextName,
				connectionString,
				contextNamespace,
				modelNamespace,
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

					foreach (var entityType in model.GetEntityTypes()) {
						if (this.entityOptions.PropertyMappings.TryGetValue(entityType.Name, out var propertyMappings)) {
							var entityProperties = entityType.GetProperties();

							var entityConfigurationStartLine = lines.FirstOrDefault(line => line.Contains($"modelBuilder.Entity<{entityType.Name}>(entity =>"));
							var entityConfigurationStartIndex = lines.IndexOf(entityConfigurationStartLine);
							var entityConfigurationEndLine = lines.Skip(entityConfigurationStartIndex).FirstOrDefault(line => line.Contains("});"));
							var entityConfigurationEndIndex = entityConfigurationStartIndex + lines.Skip(entityConfigurationStartIndex).IndexOf(entityConfigurationEndLine);

							foreach (var propertyMap in propertyMappings) {
								var entityProperty = entityProperties.FirstOrDefault(p => p.GetColumnName().Equals(propertyMap.Key, StringComparison.OrdinalIgnoreCase));

								if (entityProperty != null) {
									var oldPropertyName = entityProperty.Name;
									var newPropertyName = propertyMap.Value;

									var columnNameConfiguration = $"entity.Property(e => e.{newPropertyName}).HasColumnName(\"{entityProperty.GetColumnName()}\")";
									var columnNameConfigurationExists = false;

									for (var lineIndex = entityConfigurationStartIndex; lineIndex < entityConfigurationEndIndex; lineIndex++) {
										lines[lineIndex] = lines[lineIndex].Replace($"entity.Property(e => e.{oldPropertyName})", $"entity.Property(e => e.{newPropertyName})");

										if (lines[lineIndex].Contains(columnNameConfiguration)) {
											columnNameConfigurationExists = true;
										}
									}

									if (!columnNameConfigurationExists) {
										lines.Insert(entityConfigurationEndIndex, Environment.NewLine);
										lines.Insert(entityConfigurationEndIndex + 1, $"                {columnNameConfiguration};");
									}
								}
							}
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
