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
    public class BrandRepository: RepositoryBase<Brand>, IBrandRepository
    {
        private readonly ISortHelper<Brand> _sortHelper;

        private readonly IDataShaper<Brand> _dataShaper;

        public BrandRepository(RepositoryContext repositoryContext, ISortHelper<Brand> sortHelper, IDataShaper<Brand> dataShaper) : base(repositoryContext)
        {
            _sortHelper = sortHelper;
            _dataShaper = dataShaper;
        }

        public void CreateBrand(Brand brand)
        {
            brand.TimeCreated = DateTime.Now;
            Create(brand);
        }

        public void DeleteBrand(Brand brand)
        {
            brand.Deleted = true;
            brand.TimeDeleted = DateTime.Now;
            Brand dbBrand = GetBrandById(brand.Id);
            UpdateBrand(dbBrand, brand);
        }

        public PagedList<ShapedEntity> GetAllBrands(BrandParameters brandParameters)
        {
            var brands = FindByCondition(brand => !brand.Deleted);

            SearchByName(ref brands, brandParameters.Name);

            var sortedBrands = _sortHelper.ApplySort(brands, brandParameters.OrderBy);
            var shapedBrands = _dataShaper.ShapeData(sortedBrands, brandParameters.Fields).AsQueryable();

            return PagedList<ShapedEntity>.ToPagedList(shapedBrands, brandParameters.PageNumber, brandParameters.PageSize);
        }

        public ShapedEntity GetBrandById(Guid brandId, string fields)
        {
            var brand = FindByCondition(brand => brand.Id.Equals(brandId)).FirstOrDefault();

            if (brand == null)
            {
                brand = new Brand();
            }

            return _dataShaper.ShapeData(brand, fields);
        }

        public Brand GetBrandById(Guid brandId)
        {
            return FindByCondition(i => i.Id.Equals(brandId)).FirstOrDefault();
        }

        public void UpdateBrand(Brand dbBrand, Brand brand)
        {
            dbBrand.Map(brand);
            Update(dbBrand);
        }

        private void SearchByName(ref IQueryable<Brand> brands, string brandName)
        {
            if (!brands.Any() || string.IsNullOrWhiteSpace(brandName))
            {
                return;
            }

            brands = brands.Where(i => i.Name.ToLower().Contains(brandName.Trim().ToLower()));
        }
    }
}
