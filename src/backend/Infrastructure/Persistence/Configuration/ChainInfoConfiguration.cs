using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration
{
    public class ChainInfoConfiguration : IEntityTypeConfiguration<ChainInfo>
    {
        public void Configure(EntityTypeBuilder<ChainInfo> builder)
        {
            builder.ToTable("ChainInfo", "Chain");

            builder.Property(t => t.ChainIdBase58).HasMaxLength(20).IsRequired();
            builder.Property(t => t.RpcApi).HasMaxLength(256).IsRequired();
            builder.Property(t => t.Explorer).HasMaxLength(256);
            builder.Property(t => t.ChainType).HasMaxLength(16);
            builder.Property(t => t.LongestChainHash).HasMaxLength(128);
            builder.Property(t => t.GenesisBlockHash).HasMaxLength(128);
            builder.Property(t => t.GenesisContractAddress).HasMaxLength(128);
            builder.Property(t => t.LastIrreversibleBlockHash).HasMaxLength(128);
            builder.Property(t => t.BestChainHash).HasMaxLength(128);
            builder.Property(t => t.TokenContractAddress).HasMaxLength(128);
            builder.Property(t => t.LockVaultContractAddress).HasMaxLength(128);
            builder.Property(t => t.VestingVaultContractAddress).HasMaxLength(128);
            builder.Property(t => t.LaunchpadContractAddress).HasMaxLength(128);
        }
    }
}
