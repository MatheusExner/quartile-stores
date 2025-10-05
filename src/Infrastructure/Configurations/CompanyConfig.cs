using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class CompanyConfig : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies");

            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.Property(p => p.Name).HasMaxLength(100).IsRequired();

            builder.HasIndex(e => e.Name).IsUnique();
        }
    }
}
