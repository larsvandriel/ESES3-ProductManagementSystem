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
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasMany(p => p.Categories).WithMany(c => c.Products);
            builder.HasOne(p => p.Brand).WithMany().IsRequired();
            builder.HasIndex(p => p.Name).IsUnique();
            builder.Property(p => p.Description).IsRequired();
            builder.HasIndex(p => p.EanNumber).IsUnique();
            builder.Property(p => p.ImageLocation).IsRequired();
            builder.Property(p => p.Price).IsRequired().HasPrecision(6, 2);
            builder.Property(p => p.TimeCreated).IsRequired();
            builder.Property(p => p.Deleted).IsRequired();
            builder.Property(p => p.TimeDeleted);
        }
    }
}
