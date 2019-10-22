using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Options;
using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Services;
using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Generators
{
	/// <summary>
	/// Used to generate code for <see cref="IEntityType"/>.
	/// </summary>
	public class EntityTypeGenerator : CSharpEntityTypeGenerator
	{
		private readonly ICSharpHelper _code;
		private readonly EntityTypeOptions entityOptions;
		private readonly TypeResolverService typeResolver;
		private readonly ICSharpHelper helper;
		private readonly IServiceProvider serviceProvider;



		public EntityTypeGenerator(
			TypeResolverService typeResolver,
			IOptions<EntityTypeOptions> entityOptionsAccessor,
			ICSharpHelper cSharpHelper,
			IServiceProvider serviceProvider
		) : base(cSharpHelper)
		{
			_code = cSharpHelper;
			entityOptions = entityOptionsAccessor.Value;
			this.typeResolver = typeResolver;
			helper = cSharpHelper;
			this.serviceProvider = serviceProvider;

		}



		/// <summary>
		/// Generates code for <see cref="IEntityType"/>.
		/// </summary>
		/// <param name="entityType">The <see cref="IEntityType"/> to generate code for.</param>
		/// <param name="entityNamespace">The namespace for generated class.</param>
		/// <param name="useDataAnnotations">A value indicating whether to use data annotations.</param>
		/// <returns>The generated code for <see cref="IEntityType"/>.</returns>
		public override string WriteCode(IEntityType entityType, string entityNamespace, bool useDataAnnotations)
		{
			IndentedStringBuilder code = new IndentedStringBuilder();

			ICollection<Type> inheritances = GetInheritedTypes(entityType);
			ICollection<string> namespaces = GetNamespaces(entityType, useDataAnnotations);
			ICollection<IProperty> properties = GetProperties(entityType);
			ICollection<INavigation> navigations = GetNavigations(entityType);

			foreach (string ns in namespaces)
			{
				code.AppendLine($"using {ns};");
			}

			code.AppendLine();
			code.AppendLine($"namespace {entityNamespace}");
			code.AppendLine("{");

			using (code.Indent())
			{

				if (useDataAnnotations)
				{
					GenerateEntityTypeDataAnnotations(entityType, code);
				}

				IEnumerable<string> inheritedTypes = inheritances.Select(t => t.DisplayName(false));
				string entityTypeBase = inheritedTypes.Count() > 0 ? $" : {string.Join(", ", inheritedTypes)}" : "";
				string propertyModifiers = entityOptions.UseLazyLoading ? "public virtual" : "public";

				code.AppendLine($"public partial class {entityType.Name}{entityTypeBase}");
				code.AppendLine("{");

				using (code.Indent())
				{
					foreach (IProperty property in properties)
					{
						if (useDataAnnotations)
						{
							GeneratePropertyDataAnnotations(property, code);
						}

						code.AppendLine($"public {helper.Reference(property.ClrType)} {property.Name} {{ get; set; }}");

						if (property.IsForeignKey())
						{
							foreach (IForeignKey foreignKey in property.GetContainingForeignKeys())
							{
								code.AppendLine($"{propertyModifiers} {foreignKey.PrincipalEntityType.Name} {foreignKey.DependentToPrincipal.Name} {{ get; set; }}");
							}
						}
					}

					foreach (INavigation navigation in navigations.Where(n => n.IsCollection()))
					{
						if (useDataAnnotations)
						{
							GenerateNavigationDataAnnotations(navigation,code);
						}
						code.AppendLine($"{propertyModifiers} ICollection<{navigation.GetTargetType().Name}> {navigation.Name} {{ get; set; }} = new HashSet<{navigation.GetTargetType().Name}>();");
					}
				}

				code.AppendLine("}");
			}

			code.AppendLine("}");

			return code.ToString();
		}

		protected virtual void GenerateEntityTypeDataAnnotations(
			IEntityType entityType, IndentedStringBuilder code)
		{
			GenerateTableAttribute(entityType, code);
		}

		private void GenerateTableAttribute(IEntityType entityType, IndentedStringBuilder code)
		{
			string tableName = entityType.GetTableName();
			string schema = entityType.GetSchema();
			string defaultSchema = entityType.Model.GetDefaultSchema();

			bool schemaParameterNeeded = schema != null && schema != defaultSchema;
			bool isView = entityType.FindAnnotation(RelationalAnnotationNames.ViewDefinition) != null;
			bool tableAttributeNeeded = !isView && (schemaParameterNeeded || tableName != null && tableName != entityType.GetDbSetName());

			if (tableAttributeNeeded)
			{
				AttributeWriter tableAttribute = new AttributeWriter(nameof(TableAttribute));

				tableAttribute.AddParameter(_code.Literal(tableName));

				if (schemaParameterNeeded)
				{
					tableAttribute.AddParameter($"{nameof(TableAttribute.Schema)} = {_code.Literal(schema)}");
				}

				code.AppendLine(tableAttribute.ToString());
			}
		}

		/// <summary>
		///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
		///     the same compatibility standards as public APIs. It may be changed or removed without notice in
		///     any release. You should only use it directly in your code with extreme caution and knowing that
		///     doing so can result in application failures when updating to a new Entity Framework Core release.
		/// </summary>
		protected virtual void GeneratePropertyDataAnnotations(
			IProperty property, IndentedStringBuilder code)
		{
			GenerateKeyAttribute(property,code);
			GenerateRequiredAttribute(property, code);
			GenerateColumnAttribute(property, code);
			GenerateMaxLengthAttribute(property, code);
		}

		private void GenerateKeyAttribute(IProperty property, IndentedStringBuilder code)
		{
			var key = property.FindContainingPrimaryKey();
			if (key != null)
			{
				code.AppendLine(new AttributeWriter(nameof(KeyAttribute)));
			}
		}

		private void GenerateColumnAttribute(IProperty property, IndentedStringBuilder code)
		{
			var columnName = property.GetColumnName();
			var columnType = property.GetConfiguredColumnType();

			var delimitedColumnName = columnName != null && columnName != property.Name ? _code.Literal(columnName) : null;
			var delimitedColumnType = columnType != null ? _code.Literal(columnType) : null;

			if ((delimitedColumnName ?? delimitedColumnType) != null)
			{
				var columnAttribute = new AttributeWriter(nameof(ColumnAttribute));

				if (delimitedColumnName != null)
				{
					columnAttribute.AddParameter(delimitedColumnName);
				}

				if (delimitedColumnType != null)
				{
					columnAttribute.AddParameter($"{nameof(ColumnAttribute.TypeName)} = {delimitedColumnType}");
				}

				code.AppendLine(columnAttribute);
			}
		}

		private void GenerateMaxLengthAttribute(IProperty property, IndentedStringBuilder code)
		{
			var maxLength = property.GetMaxLength();

			if (maxLength.HasValue)
			{
				var lengthAttribute = new AttributeWriter(
					property.ClrType == typeof(string)
						? nameof(StringLengthAttribute)
						: nameof(MaxLengthAttribute));

				lengthAttribute.AddParameter(_code.Literal(maxLength.Value));

				code.AppendLine(lengthAttribute.ToString());
			}
		}

		private void GenerateRequiredAttribute(IProperty property, IndentedStringBuilder code)
		{
			if (!property.IsNullable
				&& property.ClrType.IsNullableType()
				&& !property.IsPrimaryKey())
			{
				code.AppendLine(new AttributeWriter(nameof(RequiredAttribute)).ToString());
			}
		}

		private void GenerateNavigationDataAnnotations(INavigation navigation, IndentedStringBuilder code)
		{
			GenerateForeignKeyAttribute(navigation,code);
			GenerateInversePropertyAttribute(navigation,code);
		}

		private void GenerateForeignKeyAttribute(INavigation navigation, IndentedStringBuilder code)
		{
			if (navigation.IsDependentToPrincipal())
			{
				if (navigation.ForeignKey.PrincipalKey.IsPrimaryKey())
				{
					var foreignKeyAttribute = new AttributeWriter(nameof(ForeignKeyAttribute));

					if (navigation.ForeignKey.Properties.Count > 1)
					{
						foreignKeyAttribute.AddParameter(
							  _code.Literal(
								  string.Join(",", navigation.ForeignKey.Properties.Select(p => p.Name))));
					}
					else
					{
						foreignKeyAttribute.AddParameter($"nameof({navigation.ForeignKey.Properties.First().Name})");
					}

					code.AppendLine(foreignKeyAttribute.ToString());
				}
			}
		}

		private void GenerateInversePropertyAttribute(INavigation navigation, IndentedStringBuilder code)
		{
			if (navigation.ForeignKey.PrincipalKey.IsPrimaryKey())
			{
				var inverseNavigation = navigation.FindInverse();

				if (inverseNavigation != null)
				{
					var inversePropertyAttribute = new AttributeWriter(nameof(InversePropertyAttribute));

					inversePropertyAttribute.AddParameter(
						navigation.Name != inverseNavigation.DeclaringEntityType.Name
							? $"nameof({inverseNavigation.DeclaringEntityType.Name}.{inverseNavigation.Name})"
							: _code.Literal(inverseNavigation.Name));

					code.AppendLine(inversePropertyAttribute.ToString());
				}
			}
		}

		/// <summary>
		/// Returns types inherited by class represented by <paramref name="entityType"/>.
		/// </summary>
		/// <param name="entityType"><see cref="IEntityType"/> that represents class to get inherited types of.</param>
		/// <returns>A collection of inherited types by class that is represented by <paramref name="entityType"/>.</returns>
		private ICollection<Type> GetInheritedTypes(IEntityType entityType)
		{
			List<Type> types = new List<Type>();
			string tableName = entityType.GetTableName();

			if (entityOptions.BaseMappings.ContainsKey(tableName))
			{
				foreach (string typeName in entityOptions.BaseMappings[tableName])
				{
					Type type = typeResolver.GetType(typeName);

					if (type != null)
					{
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
		private ICollection<string> GetNamespaces(IEntityType entityType, bool useDataAnnotations)
		{
			List<string> namespaces = new List<string> {
				"System",
				"System.Collections.Generic"
			};

			if (useDataAnnotations)
			{
				namespaces.Add("System.ComponentModel.DataAnnotations");
				namespaces.Add("System.ComponentModel.DataAnnotations.Schema");
			}

			foreach (Type type in GetInheritedTypes(entityType))
			{
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
		private ICollection<IProperty> GetProperties(IEntityType entityType)
		{
			IEnumerable<System.Reflection.PropertyInfo> inheritedProperties = GetInheritedTypes(entityType)
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
		private ICollection<INavigation> GetNavigations(IEntityType entityType)
		{
			return entityType
				.GetNavigations()
				.OrderBy(n => n.IsDependentToPrincipal() ? 0 : 1)
				.ThenBy(n => n.IsCollection() ? 1 : 0)
				.ToList();
		}

		private class AttributeWriter
		{
			private readonly string _attributeName;
			private readonly List<string> _parameters = new List<string>();

			public AttributeWriter(string attributeName)
			{

				_attributeName = attributeName;
			}

			public void AddParameter(string parameter)
			{

				_parameters.Add(parameter);
			}

			public override string ToString()
				=> "[" + (_parameters.Count == 0
					   ? StripAttribute(_attributeName)
					   : StripAttribute(_attributeName) + "(" + string.Join(", ", _parameters) + ")") + "]";

			private static string StripAttribute(string attributeName)
				=> attributeName.EndsWith("Attribute", StringComparison.Ordinal)
					? attributeName[0..^9]
					: attributeName;
		}
	}
}
