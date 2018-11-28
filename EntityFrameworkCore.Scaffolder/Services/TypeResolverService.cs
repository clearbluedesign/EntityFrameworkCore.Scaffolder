using System;
using System.Collections.Generic;
using System.Linq;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Services {
	/// <summary>
	/// Resolves a <see cref="Type"/> by name.
	/// </summary>
	public class TypeResolverService {
		private readonly IEnumerable<Type> types = new List<Type>();



		public TypeResolverService() {
			this.types = AppDomain.CurrentDomain
				.GetAssemblies()
				.SelectMany(a => a.GetTypes());
		}



		/// <summary>
		/// Resolves <see cref="Type"/> by provided <paramref name="typeName"/> and returns it.
		/// </summary>
		/// <param name="typeName">Type name to be resolved.</param>
		/// <returns>Resolved <see cref="Type"/> or null if none found.</returns>
		public Type GetType(String typeName) {
			if (typeName.Contains('<')) {
				var genericTypeParts = typeName.Split(new[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries);

				var genericTypeName = genericTypeParts[0];
				var genericTypeParams = genericTypeParts[1].Split(',');
				var genericType = this.GetType($"{genericTypeName}`{genericTypeParams.Length}");

				return genericType.MakeGenericType(genericTypeParams
					.Select(t => this.GetType(t))
					.ToArray()
				);
			}

			return this.types
				.FirstOrDefault(t => t.Name.Equals(typeName) || t.FullName.Equals(typeName));
		}
	}
}
