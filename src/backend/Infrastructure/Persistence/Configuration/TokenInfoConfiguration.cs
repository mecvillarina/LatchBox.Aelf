using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration
{
    public class TokenInfoConfiguration : IEntityTypeConfiguration<TokenInfo>
    {
        public void Configure(EntityTypeBuilder<TokenInfo> builder)
        {
            builder.ToTable("Tokens", "Chain");

            builder.Property(t => t.Symbol).HasMaxLength(128).IsRequired();
            builder.Property(t => t.RawExplorerData).IsRequired();
            builder.Property(t => t.Issuer).HasMaxLength(128);
        }
    }
}
