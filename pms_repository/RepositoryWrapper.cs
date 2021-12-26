using ProductManagementSystem.Contracts;
using ProductManagementSystem.Entities;
using ProductManagementSystem.Entities.Helpers;
using ProductManagementSystem.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementSystem.Repository
{
    public class RepositoryWrapper: IRepositoryWrapper
    {
        private RepositoryContext _repoContext;

        private IBrandRepository _brand;
        private ISortHelper<Brand> _brandSortHelper;
        private IDataShaper<Brand> _brandDataShaper;

        private ICategoryRepository _category;
        private ISortHelper<Category> _categorySortHelper;
        private IDataShaper<Category> _categoryDataShaper;

        private IProductRepository _product;
        private ISortHelper<Product> _productSortHelper;
        private IDataShaper<Product> _productDataShaper;

        private ISupplierRepository _supplier;
        private ISortHelper<Supplier> _supplierSortHelper;
        private IDataShaper<Supplier> _supplierDataShaper;

        private ISupplierOfferRepository _supplierOffer;
        private ISortHelper<SupplierOffer> _supplierOfferSortHelper;
        private IDataShaper<SupplierOffer> _supplierOfferDataShaper;

        public IBrandRepository Brand
        {
            get
            {
                if (_brand == null)
                {
                    _brand = new BrandRepository(_repoContext, _brandSortHelper, _brandDataShaper);
                }

                return _brand;
            }
        }

        public ICategoryRepository Category
        {
            get
            {
                if (_category == null)
                {
                    _category = new CategoryRepository(_repoContext, _categorySortHelper, _categoryDataShaper);
                }

                return _category;
            }
        }

        public IProductRepository Product
        {
            get
            {
                if (_product == null)
                {
                    _product = new ProductRepository(_repoContext, _productSortHelper, _productDataShaper);
                }

                return _product;
            }
        }

        public ISupplierRepository Supplier
        {
            get
            {
                if (_supplier == null)
                {
                    _supplier = new SupplierRepository(_repoContext, _supplierSortHelper, _supplierDataShaper);
                }

                return _supplier;
            }
        }

        public ISupplierOfferRepository SupplierOffer
        {
            get
            {
                if (_supplierOffer == null)
                {
                    _supplierOffer = new SupplierOfferRepository(_repoContext, _supplierOfferSortHelper, _supplierOfferDataShaper);
                }

                return _supplierOffer;
            }
        }

        public RepositoryWrapper(RepositoryContext repositoryContext, ISortHelper<Brand> brandSortHelper, IDataShaper<Brand> brandDataShaper, ISortHelper<Category> categorySortHelper, IDataShaper<Category> categoryDataShaper, ISortHelper<Product> productSortHelper, IDataShaper<Product> productDataShaper, ISortHelper<Supplier> supplierSortHelper, IDataShaper<Supplier> supplierDataShaper, ISortHelper<SupplierOffer> supplierOfferSortHelper, IDataShaper<SupplierOffer> supplierOfferDataShaper)
        {
            _repoContext = repositoryContext;
            _brandSortHelper = brandSortHelper;
            _brandDataShaper = brandDataShaper;
            _categorySortHelper = categorySortHelper;
            _categoryDataShaper = categoryDataShaper;
            _productSortHelper = productSortHelper;
            _productDataShaper = productDataShaper;
            _supplierSortHelper = supplierSortHelper;
            _supplierDataShaper = supplierDataShaper;
            _supplierOfferSortHelper = supplierOfferSortHelper;
            _supplierOfferDataShaper = supplierOfferDataShaper;
        }

        public void Save()
        {
            _repoContext.SaveChanges();
        }
    }
}
