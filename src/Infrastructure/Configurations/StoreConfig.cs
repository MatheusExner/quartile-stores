using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class StoreConfig : IEntityTypeConfiguration<Store>
    {
        public void Configure(EntityTypeBuilder<Store> builder)
        {
            builder.ToTable("Stores");
            
            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.Property(p => p.Name).HasMaxLength(100).IsRequired();

            builder.Property(p => p.Address).HasMaxLength(200).IsRequired();

            builder.Property(p => p.City).HasMaxLength(100).IsRequired();

            builder.Property(p => p.Country).HasMaxLength(100).IsRequired();

            builder.HasIndex(e => e.Name);
        }
    }
}
