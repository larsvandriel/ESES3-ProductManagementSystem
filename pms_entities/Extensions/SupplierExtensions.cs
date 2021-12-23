using ProductManagementSystem.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementSystem.Entities.Extensions
{
    public static class SupplierExtensions
    {
        public static void Map(this Supplier dbSupplier, Supplier supplier)
        {
            dbSupplier.Name = supplier.Name;
            dbSupplier.Address = supplier.Address;
            dbSupplier.AddressId = supplier.AddressId;
            dbSupplier.Email = supplier.Email;
            dbSupplier.PhoneNumber = supplier.PhoneNumber;
            dbSupplier.TimeCreated = supplier.TimeCreated;
            dbSupplier.Deleted = supplier.Deleted;
            dbSupplier.TimeDeleted = supplier.TimeDeleted;
        }
    }
}
