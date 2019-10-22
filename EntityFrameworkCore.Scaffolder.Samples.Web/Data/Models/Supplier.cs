using System;
using System.Collections.Generic;
using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data.Abstractions;

namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data
{
    public partial class Supplier : Company<string>, IContact<string>
    {
        public int SupplierId { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string HomePage { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}
