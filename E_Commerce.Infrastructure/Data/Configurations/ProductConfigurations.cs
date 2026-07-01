using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrastructure.Data.Configurations
{
    internal class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasOne(x => x.ProductBrand)
                 .WithMany()
                 .HasForeignKey(x => x.BrandId);

            builder.HasOne(x => x.ProductType)
                 .WithMany()
                 .HasForeignKey(x => x.TypeId);

            builder.Property(x => x.Price)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Name)
                .HasColumnType("nvarchar")
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasColumnType("nvarchar")
                .HasMaxLength(500);

            builder.Property(x => x.PictureUrl)
                .HasColumnType("nvarchar")
                .HasMaxLength(200);
        }
    }
}
