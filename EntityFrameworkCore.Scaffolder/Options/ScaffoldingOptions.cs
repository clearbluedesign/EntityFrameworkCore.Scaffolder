using System;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Options {
	/// <summary>
	/// Common options used by scaffolder
	/// </summary>
	public class ScaffoldingOptions {
		/// <summary>
		/// Gets or sets a value indicating whether to use plural and singular equivalents for the identifiers.
		/// </summary>
		public Boolean UsePluralizer { get; set; } = true;

		/// <summary>
		/// Gets or sets a value indicating whether to use data annotations.
		/// </summary>
		public Boolean UseDataAnnotations { get; set; } = false;
	}
}
