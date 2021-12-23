using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementSystem.Entities.Parameters
{
    public class ProductParameters: QueryStringParameters
    {
        public ProductParameters()
        {
            OrderBy = "name";
        }

        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public decimal MinimalPrice { get; set; }
        public decimal MaximumPrice { get; set; }
        public string Name { get; set; }
    }
}
