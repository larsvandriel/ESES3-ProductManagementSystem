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
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.HasKey(b => b.Id);
            builder.HasIndex(b => b.Name).IsUnique();
            builder.Property(b => b.TimeCreated).IsRequired();
            builder.Property(b => b.Deleted).IsRequired();
            builder.Property(b => b.TimeDeleted);
        }
    }
}
