using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementSystem.Entities.Parameters
{
    public class SupplierParameters: QueryStringParameters
    {
        public SupplierParameters()
        {
            OrderBy = "name";
        }

        public string Name { get; set; }
    }
}
