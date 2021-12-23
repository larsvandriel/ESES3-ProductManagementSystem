using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementSystem.Contracts
{
    public interface IRepositoryWrapper
    {
        IBrandRepository Brand { get; }
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        ISupplierRepository Supplier { get; }
        ISupplierOfferRepository SupplierOffer { get; }
        void Save();
    }
}
