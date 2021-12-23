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
    public interface ISupplierOfferRepository: IRepositoryBase<SupplierOffer>
    {
        PagedList<ShapedEntity> GetAllSupplierOffersBySupplier(SupplierOfferParameters supplierOfferParameters);
        ShapedEntity GetSupplierOfferBySupplierProductAndAmount(Guid supplierId, Guid productId, int amount, string fields);
        SupplierOffer GetSupplierOfferBySupplierProductAndAmount(Guid supplierId, Guid productId, int amount);
        void CreateSupplierOffer(SupplierOffer supplierOffer);
        void UpdateSupplierOffer(SupplierOffer dbSupplierOffer, SupplierOffer supplierOffer);
        void DeleteSupplierOffer(SupplierOffer dbSupplierOffer);
    }
}
