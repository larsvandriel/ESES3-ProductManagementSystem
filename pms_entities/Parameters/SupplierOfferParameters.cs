using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementSystem.Entities.Parameters
{
    public class SupplierOfferParameters: QueryStringParameters
    {
        public SupplierOfferParameters()
        {
            OrderBy = "SupplierId";
        }
    }
}
