using ProductManagementSystem.Entities.Helpers;
using ProductManagementSystem.Entities.Models;
using ProductManagementSystem.Entities.Parameters;
using ProductManagementSystem.Entities.ShapedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementSystem.Contracts
{
    public interface ISupplierRepository: IRepositoryBase<Supplier>
    {
        PagedList<ShapedEntity> GetAllSuppliers(SupplierParameters supplierParameters);
        ShapedEntity GetSupplierById(Guid supplierId, string fields);
        Supplier GetSupplierById(Guid supplierId);
        void CreateSupplier(Supplier supplier);
        void UpdateSupplier(Supplier dbSupplier, Supplier supplier);
        void DeleteSupplier(Supplier supplier);
    }
}
