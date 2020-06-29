using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Options {
	/// <summary>
	/// The options to be used for <see cref="IEntityType"/> generation
	/// </summary>
	public class EntityTypeOptions {
		/// <summary>
		/// Gets or sets a value indicating whether to add <see langword="virtual"/>
		/// to the generated navigation properties.
		/// </summary>
		public Boolean UseLazyLoading { get; set; } = true;

		/// <summary>
		/// Gets or sets map which indicates what base classes
		/// should be used for the generated entities.
		/// </summary>
		public Dictionary<String, ICollection<String>> BaseMappings { get; set; } = new Dictionary<String, ICollection<String>>();

		/// <summary>
		/// Gets or sets map which indicates what property names
		/// should be used when generating entity mappings.
		/// </summary>
		public Dictionary<String, Dictionary<String, String>> PropertyMappings { get; set; } = new Dictionary<String, Dictionary<String, String>>();
	}
}
