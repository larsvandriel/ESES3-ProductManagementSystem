using Microsoft.EntityFrameworkCore;
using ProductManagementSystem.Contracts;
using ProductManagementSystem.Entities;
using ProductManagementSystem.Entities.Extensions;
using ProductManagementSystem.Entities.Helpers;
using ProductManagementSystem.Entities.Models;
using ProductManagementSystem.Entities.Parameters;
using ProductManagementSystem.Entities.ShapedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementSystem.Repository
{
    public class SupplierOfferRepository: RepositoryBase<SupplierOffer>, ISupplierOfferRepository
    {
        private readonly ISortHelper<SupplierOffer> _sortHelper;

        private readonly IDataShaper<SupplierOffer> _dataShaper;

        public SupplierOfferRepository(RepositoryContext repositoryContext, ISortHelper<SupplierOffer> sortHelper, IDataShaper<SupplierOffer> dataShaper) : base(repositoryContext)
        {
            _sortHelper = sortHelper;
            _dataShaper = dataShaper;
        }

        public PagedList<ShapedEntity> GetAllSupplierOffersBySupplier(SupplierOfferParameters supplierOfferParameters)
        {
            var supplierOffers = RepositoryContext.SupplierOffers.AsNoTracking().AsQueryable().Include(supplierOffer => supplierOffer.Product).Include(supplierOffer => supplierOffer.Supplier);

            var sortedSupplierOffers = _sortHelper.ApplySort(supplierOffers, supplierOfferParameters.OrderBy);
            var shapedSupplierOffers = _dataShaper.ShapeData(sortedSupplierOffers, supplierOfferParameters.Fields).AsQueryable();

            return PagedList<ShapedEntity>.ToPagedList(shapedSupplierOffers, supplierOfferParameters.PageNumber, supplierOfferParameters.PageSize);
        }

        public ShapedEntity GetSupplierOfferBySupplierProductAndAmount(Guid supplierId, Guid productId, int amount, string fields)
        {
            var supplierOffer = RepositoryContext.SupplierOffers.Where(supplierOffer => supplierOffer.SupplierId == supplierId && supplierOffer.ProductId == productId && supplierOffer.Amount == amount).Include(supplierOffer => supplierOffer.Product).Include(supplierOffer => supplierOffer.Supplier).FirstOrDefault();

            if (supplierOffer == null)
            {
                supplierOffer = new SupplierOffer();
            }

            return _dataShaper.ShapeData(supplierOffer, fields);
        }

        public SupplierOffer GetSupplierOfferBySupplierProductAndAmount(Guid supplierId, Guid productId, int amount)
        {
            return RepositoryContext.SupplierOffers.Where(supplierOffer => supplierOffer.SupplierId == supplierId && supplierOffer.ProductId == productId && supplierOffer.Amount == amount).Include(supplierOffer => supplierOffer.Product).Include(supplierOffer => supplierOffer.Supplier).FirstOrDefault();
        }

        public void CreateSupplierOffer(SupplierOffer supplierOffer)
        {
            RepositoryContext.SupplierOffers.Add(supplierOffer);
        }

        public void UpdateSupplierOffer(SupplierOffer dbSupplierOffer, SupplierOffer supplierOffer)
        {
            dbSupplierOffer.Map(supplierOffer);

            RepositoryContext.Attach(dbSupplierOffer);
            RepositoryContext.Attach(dbSupplierOffer.Product);
            RepositoryContext.Attach(dbSupplierOffer.Supplier);

            RepositoryContext.SupplierOffers.Update(dbSupplierOffer);
        }

        public void DeleteSupplierOffer(SupplierOffer supplierOffer)
        {
            RepositoryContext.SupplierOffers.Remove(supplierOffer);
        }
    }
}
