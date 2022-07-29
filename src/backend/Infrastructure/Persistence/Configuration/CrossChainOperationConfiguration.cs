using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration
{
    public class CrossChainOperationConfiguration : IEntityTypeConfiguration<CrossChainOperation>
    {
        public void Configure(EntityTypeBuilder<CrossChainOperation> builder)
        {
            builder.ToTable("CrossChainOperation", "Chain");

            builder.Property(t => t.From).HasMaxLength(128).IsRequired();
            builder.Property(t => t.ChainId).IsRequired();
            builder.Property(t => t.ContractName).HasMaxLength(64).IsRequired();
            builder.Property(t => t.TransactionId).HasMaxLength(128).IsRequired();
            builder.Property(t => t.ChainOperation).HasMaxLength(64).IsRequired();
            builder.Property(t => t.ChainBlockHash).HasMaxLength(128).IsRequired();
            builder.Property(t => t.IssueChainId).IsRequired();
            builder.Property(t => t.IssueChainOperation).HasMaxLength(64).IsRequired();
            builder.Property(t => t.IsConfirmed).HasMaxLength(64).IsRequired();
            builder.Property(t => t.RawTxData).IsRequired();
            builder.Property(t => t.RawMerklePathData).IsRequired();
            builder.Property(t => t.Created).IsRequired();
        }
    }
}
