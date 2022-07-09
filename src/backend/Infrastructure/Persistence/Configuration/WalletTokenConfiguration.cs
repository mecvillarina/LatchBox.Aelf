using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration
{
    public class WalletTokenConfiguration : IEntityTypeConfiguration<WalletToken>
    {
        public void Configure(EntityTypeBuilder<WalletToken> builder)
        {
            builder.ToTable("Tokens", "Wallet");

            builder.Property(t => t.ChainIdBase58).HasMaxLength(20).IsRequired();
            builder.Property(t => t.WalletAddress).HasMaxLength(256).IsRequired();
            builder.Property(t => t.TokenName).HasMaxLength(128).IsRequired();
            builder.Property(t => t.TokenSymbol).HasMaxLength(128).IsRequired();
            builder.Property(t => t.TokenDecimals).IsRequired();
            builder.Property(t => t.TokenSupply).HasMaxLength(128).IsRequired();
            builder.Property(t => t.TokenTotalSupply).HasMaxLength(128).IsRequired();
        }
    }
}
