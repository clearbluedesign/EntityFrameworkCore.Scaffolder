using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Options;
using Microsoft.Extensions.Options;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Services {
	/// <summary>
	/// Resolves a <see cref="Type"/> by name.
	/// </summary>
	public class TypeResolverService {
		private readonly EntityTypeOptions entityOptions;
		private readonly IEnumerable<TypeInfo> definedTypes;



		public TypeResolverService(
			IOptions<EntityTypeOptions> entityOptionsAccessor
		) {
			this.entityOptions = entityOptionsAccessor.Value;

			this.definedTypes = this.entityOptions.LoadAssemblies
				.Select(a => Assembly.Load(a))
				.SelectMany(a => a.DefinedTypes);
		}



		/// <summary>
		/// Resolves <see cref="Type"/> by provided <paramref name="typeName"/> and returns its <see cref="TypeInfo"/>.
		/// </summary>
		/// <param name="typeName">Type name to be resolved.</param>
		/// <returns>Resolved <see cref="TypeInfo"/> or null if none found.</returns>
		public TypeInfo GetType(String typeName) {
			if (typeName.Contains('<')) {
				typeName = typeName.Substring(0, typeName.IndexOf('<'));
			}

			return this.definedTypes.FirstOrDefault(t =>
				t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase)
					||
				t.FullName.Equals(typeName, StringComparison.OrdinalIgnoreCase)
			);
		}
	}
}
