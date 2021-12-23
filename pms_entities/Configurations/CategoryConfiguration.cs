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
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasIndex(c => c.Name).IsUnique();
            builder.Property(c => c.Description);
            builder.Property(c => c.TimeCreated).IsRequired();
            builder.Property(c => c.Deleted).IsRequired();
            builder.Property(c => c.TimeDeleted);
        }
    }
}
