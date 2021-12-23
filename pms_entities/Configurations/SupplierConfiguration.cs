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
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.HasKey(s => s.Id);
            //builder.HasMany(s => s.Offers).WithOne().HasForeignKey(so => so.SupplierId).OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(s => s.Name).IsUnique();
            //builder.HasOne(s => s.Address).WithOne(a => a.Supplier).HasForeignKey<Address>(a => a.SupplierId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Property(s => s.Email).IsRequired();
            builder.Property(s => s.PhoneNumber).IsRequired();
            builder.Property(s => s.TimeCreated).IsRequired();
            builder.Property(s => s.Deleted).IsRequired();
            builder.Property(s => s.TimeDeleted).IsRequired();
        }
    }
}
