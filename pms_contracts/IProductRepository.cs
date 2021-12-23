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
    public interface IProductRepository: IRepositoryBase<Product>
    {
        PagedList<ShapedEntity> GetAllProducts(ProductParameters productParameters);
        ShapedEntity GetProductById(Guid productId, string fields);
        Product GetProductById(Guid productId);
        void CreateProduct(Product product);
        void UpdateProduct(Product dbProduct, Product product);
        void DeleteProduct(Product product);
    }
}
