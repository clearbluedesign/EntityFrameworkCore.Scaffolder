using System;
using System.IO;
using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.Options;
using DbContextOptions = ClearBlueDesign.EntityFrameworkCore.Scaffolder.Options.DbContextOptions;

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
		/// <param name="options">The model generatation options <see cref="ModelCodeGenerationOptions"/>.</param>
		/// <returns></returns>
		public ScaffoldedModel GenerateModel(IModel model, ModelCodeGenerationOptions options)
		{
			var files = new ScaffoldedModel();

			var contextCode = this.dbContextGenerator.WriteCode(
				model,
				options.ContextNamespace,
				options.ContextName,				
				options.ConnectionString,
				options.UseDataAnnotations,
				options.SuppressConnectionStringWarning
			);

			files.ContextFile = new ScaffoldedFile
			{
				Path = Path.Combine(options.ContextDir, options.ContextName + FileExtension),
				Code = contextCode
			};

			foreach (var entityType in model.GetEntityTypes())
			{
				var entityCode = this.entityTypeGenerator.WriteCode(
					entityType,
					options.ContextNamespace,
					options.UseDataAnnotations
				);

				var entityFile = new ScaffoldedFile
				{
					Path = entityType.DisplayName() + FileExtension,
					Code = entityCode
				};

				files.AdditionalFiles.Add(entityFile);
			}

			return files;
		}
	}
}
