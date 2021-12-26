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
    public class CategoryRepository: RepositoryBase<Category>, ICategoryRepository
    {
        private readonly ISortHelper<Category> _sortHelper;

        private readonly IDataShaper<Category> _dataShaper;

        public CategoryRepository(RepositoryContext repositoryContext, ISortHelper<Category> sortHelper, IDataShaper<Category> dataShaper) : base(repositoryContext)
        {
            _sortHelper = sortHelper;
            _dataShaper = dataShaper;
        }

        public void CreateCategory(Category category)
        {
            List<Product> products = category.Products;
            category.Products = new List<Product>();

            category.TimeCreated = DateTime.Now;
            Create(category);

            category.Products = products;
        }

        public void DeleteCategory(Category category)
        {
            category.Deleted = true;
            category.TimeDeleted = DateTime.Now;
            Category dbCategory = GetCategoryById(category.Id);
            UpdateCategory(dbCategory, category);
        }

        public PagedList<ShapedEntity> GetAllCategories(CategoryParameters categoryParameters)
        {
            var categories = FindByCondition(category => !category.Deleted).Include(category => category.Products).AsQueryable();

            SearchByName(ref categories, categoryParameters.Name);

            var sortedCategories = _sortHelper.ApplySort(categories, categoryParameters.OrderBy);
            var shapedCategories = _dataShaper.ShapeData(sortedCategories, categoryParameters.Fields).AsQueryable();

            return PagedList<ShapedEntity>.ToPagedList(shapedCategories, categoryParameters.PageNumber, categoryParameters.PageSize);
        }

        public ShapedEntity GetCategoryById(Guid categoryId, string fields)
        {
            var category = FindByCondition(category => category.Id.Equals(categoryId)).Include(category => category.Products).FirstOrDefault();

            if (category == null)
            {
                category = new Category();
            }

            return _dataShaper.ShapeData(category, fields);
        }

        public Category GetCategoryById(Guid categoryId)
        {
            return FindByCondition(category => category.Id.Equals(categoryId)).Include(category => category.Products).FirstOrDefault();
        }

        public void UpdateCategory(Category dbCategory, Category category)
        {
            dbCategory.Map(category);

            RepositoryContext.Categories.Attach(dbCategory);

            Update(dbCategory);
        }

        private void SearchByName(ref IQueryable<Category> categories, string categoryName)
        {
            if (!categories.Any() || string.IsNullOrWhiteSpace(categoryName))
            {
                return;
            }

            categories = categories.Where(i => i.Name.ToLower().Contains(categoryName.Trim().ToLower()));
        }
    }
}
