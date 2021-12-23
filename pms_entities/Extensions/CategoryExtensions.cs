using ProductManagementSystem.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementSystem.Entities.Extensions
{
    public static class CategoryExtensions
    {
        public static void Map(this Category dbCategory, Category category)
        {
            dbCategory.Name = category.Name;
            dbCategory.Description = category.Description;
            dbCategory.TimeCreated = category.TimeCreated;
            dbCategory.Deleted = category.Deleted;
            dbCategory.TimeDeleted = category.TimeDeleted;
        }
    }
}
