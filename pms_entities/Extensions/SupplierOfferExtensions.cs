using ProductManagementSystem.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementSystem.Entities.Extensions
{
    public static class SupplierOfferExtensions
    {
        public static void Map(this SupplierOffer dbSupplierOffer, SupplierOffer supplierOffer)
        {
            dbSupplierOffer.Price = supplierOffer.Price;
        }

        public static bool IsObjectNull(this SupplierOffer supplierOffer)
        {
            return supplierOffer == null;
        }

        public static bool IsEmptyObject(this SupplierOffer supplierOffer)
        {
            return supplierOffer.ProductId.Equals(Guid.Empty) || supplierOffer.SupplierId.Equals(Guid.Empty) || supplierOffer.Amount <= 0;
        }
    }
}
