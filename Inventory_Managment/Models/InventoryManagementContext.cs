using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Managment.Models;

public partial class InventoryManagementContext : DbContext
{
    public InventoryManagementContext()
    {
    }

    public InventoryManagementContext(DbContextOptions<InventoryManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<ItemAuditTrail> ItemAuditTrails { get; set; }

    public virtual DbSet<ItemMaster> ItemMasters { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<ItemReport> ItemReports { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("PK__Inventor__F5FDE6D366CF812A");

            entity.ToTable("Inventory");

            entity.Property(e => e.InventoryId).HasColumnName("InventoryID");
            entity.Property(e => e.TransactionDate).HasColumnType("date");
            entity.Property(e => e.TransactionType)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.ItemCodeNavigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.ItemCode)
                .HasConstraintName("FK__Inventory__ItemC__3E52440B");

            entity.HasOne(d => d.TransactionNoNavigation).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.TransactionNo)
                .HasConstraintName("FK__Inventory__Trans__3F466844");
        });

        modelBuilder.Entity<ItemAuditTrail>(entity =>
        {
            entity.HasKey(e => e.AuditTrailId).HasName("PK__ItemAudi__41B2DDD341CA6A01");

            entity.ToTable("ItemAuditTrail");

            entity.Property(e => e.AuditTrailId).HasColumnName("AuditTrailID");
            entity.Property(e => e.Action)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ItemName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Mrp)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("MRP");
            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Uom)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UOM");
        });

        modelBuilder.Entity<ItemMaster>(entity =>
        {
            entity.HasKey(e => e.ItemCode).HasName("PK__ItemMast__3ECC0FEBDC94DC37");

            entity.ToTable("ItemMaster", tb => tb.HasTrigger("ItemMasterAudit"));

            entity.Property(e => e.ItemCode).ValueGeneratedNever();
            entity.Property(e => e.ItemName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Mrp)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("MRP");
            entity.Property(e => e.Uom)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UOM");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionNo).HasName("PK__Transact__554342D8F7B36875");

            entity.ToTable(tb => tb.HasTrigger("TransactionTrigger"));

            entity.Property(e => e.TransactionNo).ValueGeneratedNever();
            entity.Property(e => e.EntryDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TransactionDate).HasColumnType("date");
            entity.Property(e => e.TransactionType)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.ItemCodeNavigation).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.ItemCode)
                .HasConstraintName("FK__Transacti__ItemC__31EC6D26");
        });

        OnModelCreatingPartial(modelBuilder);
        modelBuilder.Entity<ItemReport>().HasNoKey();
    }

   
   

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
