using ProductManagementSystem.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementSystem.Entities.Extensions
{
    public static class BrandExtensions
    {
        public static void Map(this Brand dbBrand, Brand brand)
        {
            dbBrand.Name = brand.Name;
            dbBrand.TimeCreated = brand.TimeCreated;
            dbBrand.Deleted = brand.Deleted;
            dbBrand.TimeDeleted = brand.TimeDeleted;
        }
    }
}
