using System;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data.Contacts {
	public class Contact : IContact<String> {
		public String ContactName { get; set; }
		public String ContactTitle { get; set; }
		public String Phone { get; set; }
		public String Fax { get; set; }
	}
}
