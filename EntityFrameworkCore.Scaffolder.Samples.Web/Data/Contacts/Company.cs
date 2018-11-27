using System;



namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data.Contacts {
	public class Company<T> {
		public T CompanyName { get; set; }
		public String Address { get; set; }
		public String City { get; set; }
		public String Region { get; set; }
		public String PostalCode { get; set; }
		public String Country { get; set; }
	}
}
