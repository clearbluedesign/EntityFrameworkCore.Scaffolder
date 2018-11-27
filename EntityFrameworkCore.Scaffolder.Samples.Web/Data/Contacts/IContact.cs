using System;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data.Contacts {
	public interface IContact<TName> {
		TName ContactName { get; set; }
		String ContactTitle { get; set; }
		String Phone { get; set; }
		String Fax { get; set; }
	}
}
