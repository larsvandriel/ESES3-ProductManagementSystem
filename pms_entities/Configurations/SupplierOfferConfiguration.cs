using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManagementSystem.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementSystem.Entities.Configurations
{
    public class SupplierOfferConfiguration : IEntityTypeConfiguration<SupplierOffer>
    {
        public void Configure(EntityTypeBuilder<SupplierOffer> builder)
        {
            builder.HasKey(so => new { so.SupplierId, so.ProductId, so.Amount });
            builder.Property(so => so.Price).IsRequired().HasPrecision(15, 2);
        }
    }
}
