using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementSystem.Entities.ShapedEntities
{
    public class ShapedSupplierOfferEntity: ShapedEntity
    {
        public Guid SupplierId { get; set; }
        public Guid ProductId { get; set; }
        public int Amount { get; set; }
    }
}
