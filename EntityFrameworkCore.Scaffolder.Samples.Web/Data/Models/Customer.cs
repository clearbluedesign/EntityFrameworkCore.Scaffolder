using System;
using System.Collections.Generic;
using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data.Abstractions;

namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data
{
    public partial class Customer : IContact<string>
    {
        public string CustomerId { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public virtual ICollection<CustomerCustomerDemo> CustomerCustomerDemoes { get; set; } = new HashSet<CustomerCustomerDemo>();
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
}
