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
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Country).IsRequired();
            builder.Property(a => a.City).IsRequired();
            builder.Property(a => a.Street).IsRequired();
            builder.Property(a => a.Number).IsRequired();
            builder.Property(a => a.ZipCode).IsRequired();
        }
    }
}
