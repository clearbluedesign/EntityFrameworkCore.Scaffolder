using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Options;
using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Services;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Generators
{
	/// <summary>
	/// Used to generate code for <see cref="DbContext"/>.
	/// </summary>
	public class DbContextGenerator : CSharpDbContextGenerator
	{
		private readonly ICSharpHelper _code;
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
		)
		{
			_code = cSharpHelper;
			dbContextOptions = dbContextOptionsAccessor.Value;
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
		public override string WriteCode(
			IModel model,
			string contextNamespace,
			string contextName,
			string connectionString,
			bool useDataAnnotations,
			bool suppressConnectionStringWarning
		)
		{
			string code = base.WriteCode(
				model,
				contextNamespace,
				contextName,
				connectionString,
				useDataAnnotations,
				suppressConnectionStringWarning
			);

			if (dbContextOptions.Base != "DbContext")
			{
				Type type = typeResolver.GetType(dbContextOptions.Base);

				if (type != null)
				{
					List<string> lines = code.Split(Environment.NewLine).ToList();
					string lastUsing = lines.LastOrDefault(l => l.StartsWith("using"));
					int lastUsingIndex = lines.IndexOf(lastUsing);

					lines.Insert(lastUsingIndex + 1, $"using {type.Namespace};");

					IEnumerable<string> inheritedDbSets = type.GetProperties()
						.Where(p => p.PropertyType.Name == "DbSet`1")
						.Select(p => p.Name);

					foreach (IEntityType entityType in model.GetEntityTypes())
					{
						string dbSetName = entityType.GetDbSetName();
						if (inheritedDbSets.Contains(dbSetName))
						{
							string dbSetLine = lines.Find(l => l.Contains($"DbSet<{entityType.Name}> {dbSetName}"));
							int dbSetLineIndex = lines.IndexOf(dbSetLine);

							lines.RemoveAt(dbSetLineIndex);
						}
					}

					code = string.Join(Environment.NewLine, lines);

					code = code.Replace($"{contextName} : DbContext", $"{contextName} : {type.DisplayName(false)}");
				}
			}

			return code;
		}
	}
}
