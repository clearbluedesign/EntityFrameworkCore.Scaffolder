using System;
using System.Collections.Generic;
using System.Linq;
using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Options;
using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.Options;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Generators {
	/// <summary>
	/// Used to generate code for <see cref="IEntityType"/>.
	/// </summary>
	public class EntityTypeGenerator : CSharpEntityTypeGenerator {
		private readonly EntityTypeOptions entityOptions;
		private readonly TypeResolverService typeResolver;
		private readonly ICSharpHelper helper;
		private readonly IServiceProvider serviceProvider;



		public EntityTypeGenerator(
			TypeResolverService typeResolver,
			IOptions<EntityTypeOptions> entityOptionsAccessor,
			ICSharpHelper cSharpHelper,
			IServiceProvider serviceProvider
		) : base(cSharpHelper) {
			this.entityOptions = entityOptionsAccessor.Value;
			this.typeResolver = typeResolver;
			this.helper = cSharpHelper;
			this.serviceProvider = serviceProvider;

		}



		/// <summary>
		/// Generates code for <see cref="IEntityType"/>.
		/// </summary>
		/// <param name="entityType">The <see cref="IEntityType"/> to generate code for.</param>
		/// <param name="entityNamespace">The namespace for generated class.</param>
		/// <param name="useDataAnnotations">A value indicating whether to use data annotations.</param>
		/// <returns>The generated code for <see cref="IEntityType"/>.</returns>
		public override String WriteCode(IEntityType entityType, String entityNamespace, Boolean useDataAnnotations) {
			var code = new IndentedStringBuilder();

			var inheritances = this.GetInheritedTypes(entityType);
			var namespaces = this.GetNamespaces(entityType, useDataAnnotations);
			var properties = this.GetProperties(entityType);
			var navigations = this.GetNavigations(entityType);

			foreach (var ns in namespaces) {
				code.AppendLine($"using {ns};");
			}

			code.AppendLine();
			code.AppendLine($"namespace {entityNamespace}");
			code.AppendLine("{");

			using (code.Indent()) {
				var inheritedTypes = inheritances.Select(t => t.DisplayName(false));
				var entityTypeBase = inheritedTypes.Count() > 0 ? $" : {String.Join(", ", inheritedTypes)}" : "";
				var propertyModifiers = this.entityOptions.UseLazyLoading ? "public virtual" : "public";

				code.AppendLine($"public partial class {entityType.Name}{entityTypeBase}");
				code.AppendLine("{");

				using (code.Indent()) {
					foreach (var property in properties) {
						code.AppendLine($"public {this.helper.Reference(property.ClrType)} {property.Name} {{ get; set; }}");

						if (property.IsForeignKey()) {
							foreach (var foreignKey in property.GetContainingForeignKeys()) {
								code.AppendLine($"{propertyModifiers} {foreignKey.PrincipalEntityType.Name} {foreignKey.DependentToPrincipal.Name} {{ get; set; }}");
							}
						}
					}

					foreach (var navigation in navigations.Where(n => n.IsCollection())) {
						code.AppendLine($"{propertyModifiers} ICollection<{navigation.GetTargetType().Name}> {navigation.Name} {{ get; set; }} = new HashSet<{navigation.GetTargetType().Name}>();");
					}
				}

				code.AppendLine("}");
			}

			code.AppendLine("}");

			return code.ToString();
		}



		/// <summary>
		/// Returns types inherited by class represented by <paramref name="entityType"/>.
		/// </summary>
		/// <param name="entityType"><see cref="IEntityType"/> that represents class to get inherited types of.</param>
		/// <returns>A collection of inherited types by class that is represented by <paramref name="entityType"/>.</returns>
		private ICollection<Type> GetInheritedTypes(IEntityType entityType) {
			var types = new List<Type>();
			var tableName = entityType.GetTableName();

			if (this.entityOptions.BaseMappings.ContainsKey(tableName)) {
				foreach (var typeName in this.entityOptions.BaseMappings[tableName]) {
					var type = this.typeResolver.GetType(typeName);

					if (type != null) {
						types.Add(type);
					}
				}
			}

			return types;
		}



		/// <summary>
		/// Returns namespaces used by the class that is represented by <paramref name="entityType"/>.
		/// </summary>
		/// <param name="entityType"><see cref="IEntityType"/> that represents class to get namespaces of.</param>
		/// <param name="useDataAnnotations">Should we add namespaces that is required by data annotation attributes?</param>
		/// <returns>A collection of namespaces used by class represented by <paramref name="entityType"/>.</returns>
		private ICollection<String> GetNamespaces(IEntityType entityType, Boolean useDataAnnotations) {
			var namespaces = new List<String> {
				"System",
				"System.Collections.Generic"
			};

			if (useDataAnnotations) {
				namespaces.Add("System.ComponentModel.DataAnnotations");
				namespaces.Add("System.ComponentModel.DataAnnotations.Schema");
			}

			foreach (var type in this.GetInheritedTypes(entityType)) {
				namespaces.Add(type.Namespace);
			}

			namespaces.AddRange(entityType
				.GetProperties()
				.SelectMany(p => p.ClrType.GetNamespaces()));

			return namespaces
				.Distinct()
				.OrderBy(x => x, new NamespaceComparer())
				.ToList();
		}



		/// <summary>
		/// Returns properties of the class that is represented by <paramref name="entityType"/>.
		/// </summary>
		/// <param name="entityType"><see cref="IEntityType"/> that represents class to get properties of.</param>
		/// <remarks>Inherited properties will be excluded from the returned list.</remarks>
		/// <returns>A collection of <see cref="IProperty"/> that describes properties of the class represented by <paramref name="entityType"/>.</returns>
		private ICollection<IProperty> GetProperties(IEntityType entityType) {
			var inheritedProperties = this.GetInheritedTypes(entityType)
				.Where(t => t.IsInterface == false)
				.SelectMany(t => t.GetProperties());

			return entityType
				.GetProperties()
				.OrderBy(p => p.GetColumnOrdinal())
				.Where(p => inheritedProperties.Any(pi => pi.Name == p.Name) == false)
				.ToList();
		}



		/// <summary>
		/// Returns navigation properties of the class that is represented by <paramref name="entityType"/>.
		/// </summary>
		/// <param name="entityType"><see cref="IEntityType"/> that represents class to get navigation properties of.</param>
		/// <returns>A collection of <see cref="INavigation"/> that describes navigation properties of the class represented by <paramref name="entityType"/>.</returns>
		private ICollection<INavigation> GetNavigations(IEntityType entityType) {
			return entityType
				.GetNavigations()
				.OrderBy(n => n.IsDependentToPrincipal() ? 0 : 1)
				.ThenBy(n => n.IsCollection() ? 1 : 0)
				.ToList();
		}
	}
}
