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
    public interface ICategoryRepository: IRepositoryBase<Category>
    {
        PagedList<ShapedEntity> GetAllCategories(CategoryParameters categoryParameters);
        ShapedEntity GetCategoryById(Guid categoryId, string fields);
        Category GetCategoryById(Guid categoryId);
        void CreateCategory(Category category);
        void UpdateCategory(Category dbCategory, Category category);
        void DeleteCategory(Category category);
    }
}
