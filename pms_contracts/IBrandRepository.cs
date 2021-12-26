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
    public interface IBrandRepository: IRepositoryBase<Brand>
    {
        PagedList<ShapedEntity> GetAllBrands(BrandParameters brandParameters);
        ShapedEntity GetBrandById(Guid brandId, string fields);
        Brand GetBrandById(Guid brandId);
        void CreateBrand(Brand brand);
        void UpdateBrand(Brand dbBrand, Brand brand);
        void DeleteBrand(Brand brand);
    }
}
