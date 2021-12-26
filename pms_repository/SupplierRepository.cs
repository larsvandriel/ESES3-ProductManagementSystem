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
    public class SupplierRepository: RepositoryBase<Supplier>, ISupplierRepository
    {
        private readonly ISortHelper<Supplier> _sortHelper;

        private readonly IDataShaper<Supplier> _dataShaper;

        public SupplierRepository(RepositoryContext repositoryContext, ISortHelper<Supplier> sortHelper, IDataShaper<Supplier> dataShaper) : base(repositoryContext)
        {
            _sortHelper = sortHelper;
            _dataShaper = dataShaper;
        }

        public void CreateSupplier(Supplier supplier)
        {
            var address = RepositoryContext.Addresses.Add(supplier.Address);
            supplier.Address = address.Entity;
            supplier.AddressId = supplier.Address.Id;
            supplier.TimeCreated = DateTime.Now;
            Create(supplier);
            //supplier.Address.Supplier = supplier;
            //supplier.Address.SupplierId = supplier.Id;
        }

        public void DeleteSupplier(Supplier supplier)
        {
            supplier.Deleted = true;
            supplier.TimeDeleted = DateTime.Now;
            Supplier dbSupplier = GetSupplierById(supplier.Id);
            UpdateSupplier(dbSupplier, supplier);
        }

        public PagedList<ShapedEntity> GetAllSuppliers(SupplierParameters supplierParameters)
        {
            var suppliers = FindByCondition(supplier => !supplier.Deleted).Include(supplier => supplier.Address).Include(supplier => supplier.Offers).AsQueryable();

            SearchByName(ref suppliers, supplierParameters.Name);

            var sortedSuppliers = _sortHelper.ApplySort(suppliers, supplierParameters.OrderBy);
            var shapedSuppliers = _dataShaper.ShapeData(sortedSuppliers, supplierParameters.Fields).AsQueryable();

            return PagedList<ShapedEntity>.ToPagedList(shapedSuppliers, supplierParameters.PageNumber, supplierParameters.PageSize);
        }

        public ShapedEntity GetSupplierById(Guid supplierId, string fields)
        {
            var supplier = FindByCondition(supplier => supplier.Id.Equals(supplierId)).Include(supplier => supplier.Address).Include(supplier => supplier.Offers).FirstOrDefault();

            if (supplier == null)
            {
                supplier = new Supplier();
            }

            return _dataShaper.ShapeData(supplier, fields);
        }

        public Supplier GetSupplierById(Guid supplierId)
        {
            return FindByCondition(i => i.Id.Equals(supplierId)).Include(supplier => supplier.Offers).Include(supplier => supplier.Address).FirstOrDefault();
        }

        public void UpdateSupplier(Supplier dbSupplier, Supplier supplier)
        {
            dbSupplier.Map(supplier);

            RepositoryContext.Attach(dbSupplier);
            RepositoryContext.Attach(dbSupplier.Address);

            RepositoryContext.Addresses.Update(supplier.Address);

            Update(dbSupplier);
        }

        private void SearchByName(ref IQueryable<Supplier> suppliers, string supplierName)
        {
            if (!suppliers.Any() || string.IsNullOrWhiteSpace(supplierName))
            {
                return;
            }

            suppliers = suppliers.Where(i => i.Name.ToLower().Contains(supplierName.Trim().ToLower()));
        }
    }
}
