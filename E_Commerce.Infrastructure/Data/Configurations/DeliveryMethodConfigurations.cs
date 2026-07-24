using E_Commerce.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Data.Configurations
{
    public class DeliveryMethodConfigurations : IEntityTypeConfiguration<DeliveryMethod>
    {
        public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();
            builder.Property(x => x.Price)
                .HasColumnType("decimal(8,2)");
            builder.Property(x => x.ShortName)
                .HasColumnType("varchar")
                .HasMaxLength(50);
            builder.Property(x => x.Description)
                .HasColumnType("varchar")
                .HasMaxLength(100);
            builder.Property(x => x.DeliveryTime)
                .HasColumnType("varchar")
                .HasMaxLength(50);
        }
    }
}
