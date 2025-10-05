using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            
            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.Property(p => p.Name).HasMaxLength(100).IsRequired();

            builder.Property(p => p.Description).HasMaxLength(500);

            builder.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();

            builder.HasIndex(e => e.Name);
        }
    }
}
