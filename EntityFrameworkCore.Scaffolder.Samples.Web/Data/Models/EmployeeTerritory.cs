using System;
using System.Collections.Generic;

namespace ClearBlueDesign.EntityFrameworkCore.Scaffolder.Samples.Web.Data
{
    public partial class EmployeeTerritory
    {
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public string TerritoryId { get; set; }
        public virtual Territory Territory { get; set; }
    }
}
