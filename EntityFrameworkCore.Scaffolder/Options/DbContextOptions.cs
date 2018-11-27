using System;
using Microsoft.EntityFrameworkCore;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Options {
	/// <summary>
	/// The options to be used for <see cref="DbContext"/> generation
	/// </summary>
	public class DbContextOptions {
		/// <summary>
		/// Gets or sets base class for generated <see cref="DbContext"/>
		/// </summary>
		public String Base { get; set; } = "DbContext";
	}
}
