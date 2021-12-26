using ProductManagementSystem.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementSystem.Entities.Extensions
{
    public static class ProductExtensions
    {
        public static void Map(this Product dbProduct, Product product)
        {
            dbProduct.Name = product.Name;
            dbProduct.EanNumber = product.EanNumber;
            dbProduct.Description = product.Description;
            dbProduct.Price = product.Price;
            dbProduct.ImageLocation = product.ImageLocation;
            dbProduct.Brand = product.Brand;
            //dbProduct.Categories = product.Categories;
            dbProduct.TimeCreated = product.TimeCreated;
            dbProduct.Deleted = product.Deleted;
            dbProduct.TimeDeleted = product.TimeDeleted;
        }
    }
}
