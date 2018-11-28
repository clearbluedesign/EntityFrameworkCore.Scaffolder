using System;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data.Abstractions {
	public abstract class Contact : IContact<String> {
		public String ContactName { get; set; }
		public String ContactTitle { get; set; }
		public String Phone { get; set; }
		public String Fax { get; set; }
	}
}
