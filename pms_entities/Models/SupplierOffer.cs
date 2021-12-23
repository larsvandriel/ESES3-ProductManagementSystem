using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementSystem.Entities.Models
{
    public class SupplierOffer
    {
        public Guid SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
    }
}
