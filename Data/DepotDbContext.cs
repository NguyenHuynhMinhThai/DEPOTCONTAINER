using DEPOTCONTAINER.Models.Entities;
using DEPOTCONTAINER.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DEPOTCONTAINER.Data;

/// <summary>
/// DbContext chính của hệ thống Depot Container.
/// Quản lý kết nối đến MySQL và cấu hình các entity thông qua Fluent API.
/// </summary>
public class DepotDbContext : DbContext
{
    public DepotDbContext(DbContextOptions<DepotDbContext> options) : base(options)
    {
    }

    // ============ DbSets ============
    public DbSet<LineOperator> LineOperators { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Block> Blocks { get; set; } = null!;
    public DbSet<Bay> Bays { get; set; } = null!;
    public DbSet<Row> Rows { get; set; } = null!;
    public DbSet<Tier> Tiers { get; set; } = null!;
    public DbSet<Container> Containers { get; set; } = null!;
    public DbSet<ContainerMovement> ContainerMovements { get; set; } = null!;
    public DbSet<ReleaseOrder> ReleaseOrders { get; set; } = null!;
    public DbSet<ReleaseOrderDetail> ReleaseOrderDetails { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Lưu enum dưới dạng int để dễ truy vấn và hiển thị
        modelBuilder.Entity<Container>().Property(c => c.ContainerType).HasConversion<int>();
        modelBuilder.Entity<Container>().Property(c => c.Size).HasConversion<int>();
        modelBuilder.Entity<Container>().Property(c => c.Condition).HasConversion<int>();
        modelBuilder.Entity<Container>().Property(c => c.Category).HasConversion<int>();
        modelBuilder.Entity<ContainerMovement>().Property(m => m.MovementType).HasConversion<int>();
        modelBuilder.Entity<Block>().Property(b => b.BlockType).HasConversion<int>();
        modelBuilder.Entity<Block>().Property(b => b.MaxContainerSize).HasConversion<int?>();
        modelBuilder.Entity<Bay>().Property(b => b.ContainerSize).HasConversion<int>();
        modelBuilder.Entity<ReleaseOrder>().Property(r => r.Status).HasConversion<int>();
        modelBuilder.Entity<ReleaseOrderDetail>().Property(r => r.ContainerSize).HasConversion<int>();
        modelBuilder.Entity<ReleaseOrderDetail>().Property(r => r.ContainerType).HasConversion<int>();

        // ============ Indexes ============
        modelBuilder.Entity<LineOperator>().HasIndex(l => l.OwnerCode).IsUnique();
        modelBuilder.Entity<Customer>().HasIndex(c => c.TaxCode).IsUnique();
        modelBuilder.Entity<Block>().HasIndex(b => b.Code).IsUnique();
        modelBuilder.Entity<Container>().HasIndex(c => c.ContainerNumber).IsUnique();
        modelBuilder.Entity<ReleaseOrder>().HasIndex(r => r.OrderNumber).IsUnique();

        modelBuilder.Entity<Bay>().HasIndex(b => new { b.BlockId, b.BayNumber }).IsUnique();
        modelBuilder.Entity<Row>().HasIndex(r => new { r.BayId, r.RowNumber }).IsUnique();
        modelBuilder.Entity<Tier>().HasIndex(t => new { t.RowId, t.TierNumber }).IsUnique();

        // ============ Relationships ============
        // Bay -> Block (many-to-one)
        modelBuilder.Entity<Bay>()
            .HasOne(b => b.Block)
            .WithMany(b => b.Bays)
            .HasForeignKey(b => b.BlockId)
            .OnDelete(DeleteBehavior.Cascade);

        // Row -> Bay (many-to-one)
        modelBuilder.Entity<Row>()
            .HasOne(r => r.Bay)
            .WithMany(b => b.Rows)
            .HasForeignKey(r => r.BayId)
            .OnDelete(DeleteBehavior.Cascade);

        // Tier -> Row (many-to-one)
        modelBuilder.Entity<Tier>()
            .HasOne(t => t.Row)
            .WithMany(r => r.Tiers)
            .HasForeignKey(t => t.RowId)
            .OnDelete(DeleteBehavior.Cascade);

        // Container -> LineOperator (many-to-one)
        modelBuilder.Entity<Container>()
            .HasOne(c => c.LineOperator)
            .WithMany(l => l.Containers)
            .HasForeignKey(c => c.LineOperatorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Container -> Block/Bay/Row/Tier (current location)
        modelBuilder.Entity<Container>()
            .HasOne(c => c.CurrentBlock)
            .WithMany(b => b.Containers)
            .HasForeignKey(c => c.CurrentBlockId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Container>()
            .HasOne(c => c.CurrentBay)
            .WithMany()
            .HasForeignKey(c => c.CurrentBayId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Container>()
            .HasOne(c => c.CurrentRow)
            .WithMany()
            .HasForeignKey(c => c.CurrentRowId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Container>()
            .HasOne(c => c.CurrentTier)
            .WithMany()
            .HasForeignKey(c => c.CurrentTierId)
            .OnDelete(DeleteBehavior.SetNull);

        // ContainerMovement -> Container
        modelBuilder.Entity<ContainerMovement>()
            .HasOne(m => m.Container)
            .WithMany(c => c.Movements)
            .HasForeignKey(m => m.ContainerId)
            .OnDelete(DeleteBehavior.Cascade);

        // ContainerMovement -> Block (destination)
        modelBuilder.Entity<ContainerMovement>()
            .HasOne(m => m.ToBlock)
            .WithMany()
            .HasForeignKey(m => m.ToBlockId)
            .OnDelete(DeleteBehavior.SetNull);

        // ContainerMovement -> ReleaseOrder
        modelBuilder.Entity<ContainerMovement>()
            .HasOne(m => m.ReleaseOrder)
            .WithMany(r => r.Movements)
            .HasForeignKey(m => m.ReleaseOrderId)
            .OnDelete(DeleteBehavior.SetNull);

        // ReleaseOrder -> LineOperator
        modelBuilder.Entity<ReleaseOrder>()
            .HasOne(r => r.LineOperator)
            .WithMany(l => l.ReleaseOrders)
            .HasForeignKey(r => r.LineOperatorId)
            .OnDelete(DeleteBehavior.Restrict);

        // ReleaseOrder -> Customer
        modelBuilder.Entity<ReleaseOrder>()
            .HasOne(r => r.Customer)
            .WithMany(c => c.ReleaseOrders)
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // ReleaseOrderDetail -> ReleaseOrder
        modelBuilder.Entity<ReleaseOrderDetail>()
            .HasOne(d => d.ReleaseOrder)
            .WithMany(r => r.Details)
            .HasForeignKey(d => d.ReleaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // ============ Global query filter for soft delete ============
        modelBuilder.Entity<LineOperator>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Customer>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Block>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Bay>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Row>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Tier>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Container>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ContainerMovement>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ReleaseOrder>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ReleaseOrderDetail>().HasQueryFilter(e => !e.IsDeleted);
    }
}