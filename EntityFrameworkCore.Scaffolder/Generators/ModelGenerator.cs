using System;
using System.IO;
using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Options;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.Options;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Generators {
	/// <summary>
	/// Used to generate code for a model.
	/// </summary>
	public class ModelGenerator : IModelCodeGenerator {
		private readonly DbContextOptions dbContextOptions;
		private readonly ScaffoldingOptions scaffoldingOptions;
		private readonly ICSharpDbContextGenerator dbContextGenerator;
		private readonly ICSharpEntityTypeGenerator entityTypeGenerator;

		private const String FileExtension = ".cs";


		/// <summary>
		/// Gets the programming language supported by this service.
		/// </summary>
		public String Language { get; } = "C#";



		public ModelGenerator(
			IOptions<DbContextOptions> dbContextOptionsAccessor,
			IOptions<ScaffoldingOptions> scaffoldingOptionsAccessor,
			ModelCodeGeneratorDependencies dependencies,
			ICSharpDbContextGenerator dbContextGenerator,
			ICSharpEntityTypeGenerator entityTypeGenerator
		) {
			this.dbContextOptions = dbContextOptionsAccessor.Value;
			this.scaffoldingOptions = scaffoldingOptionsAccessor.Value;
			this.dbContextGenerator = dbContextGenerator;
			this.entityTypeGenerator = entityTypeGenerator;
		}



		/// <summary>
		/// Generates code for a model.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="namespace">The namespace.</param>
		/// <param name="contextDir">The directory of the <see cref="DbContext"/>.</param>
		/// <param name="contextName">The name of the <see cref="DbContext"/>.</param>
		/// <param name="connectionString">The connection string.</param>
		/// <param name="options">The options to use during generation.</param>
		/// <returns>The generated model.</returns>
		public ScaffoldedModel GenerateModel(
			IModel model,
			String contextNamespace,
			String contextDir,
			String contextName,
			String connectionString,
			ModelCodeGenerationOptions options
		) {
			var files = new ScaffoldedModel();

			var contextCode = this.dbContextGenerator.WriteCode(
				model,
				contextNamespace,
				contextName,
				connectionString,
				options.UseDataAnnotations,
				options.SuppressConnectionStringWarning
			);

			files.ContextFile = new ScaffoldedFile {
				Path = Path.Combine(contextDir, contextName + FileExtension),
				Code = contextCode
			};

			foreach (var entityType in model.GetEntityTypes()) {
				var entityCode = this.entityTypeGenerator.WriteCode(
					entityType,
					contextNamespace,
					options.UseDataAnnotations
				);

				var entityFile = new ScaffoldedFile {
					Path = entityType.DisplayName() + FileExtension,
					Code = entityCode
				};

				files.AdditionalFiles.Add(entityFile);
			}

			return files;
		}
	}
}
