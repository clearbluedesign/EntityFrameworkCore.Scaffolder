using System;
using System.Collections.Generic;
using ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data.Abstractions;

namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data
{
    public partial class Category : IEmptyContract
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}
