﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ChilLaxFrontEnd.Models
{
    public partial class ChilLaxContext : DbContext
    {
        public ChilLaxContext()
        {
        }

        public ChilLaxContext(DbContextOptions<ChilLaxContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Announcement> Announcements { get; set; } = null!;
        public virtual DbSet<Cart> Carts { get; set; } = null!;
        public virtual DbSet<CustomerService> CustomerServices { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<FocusDetail> FocusDetails { get; set; } = null!;
        public virtual DbSet<FocusSlide> FocusSlides { get; set; } = null!;
        public virtual DbSet<Member> Members { get; set; } = null!;
        public virtual DbSet<MemberCredential> MemberCredentials { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<PointHistory> PointHistories { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductOrder> ProductOrders { get; set; } = null!;
        public virtual DbSet<Purchase> Purchases { get; set; } = null!;
        public virtual DbSet<PurchaseDetail> PurchaseDetails { get; set; } = null!;
        public virtual DbSet<Supplier> Suppliers { get; set; } = null!;
        public virtual DbSet<TarotCard> TarotCards { get; set; } = null!;
        public virtual DbSet<TarotOrder> TarotOrders { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.EnableSensitiveDataLogging();
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=ChilLax;Integrated Security=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.ToTable("Announcement");

                entity.Property(e => e.AnnouncementId).HasColumnName("announcement_id");

                entity.Property(e => e.Announcement1)
                    .HasMaxLength(1000)
                    .HasColumnName("announcement");

                entity.Property(e => e.BonusForFocus).HasColumnName("bonus_for_focus");

                entity.Property(e => e.BonusForShopping).HasColumnName("bonus_for_shopping");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("end_date");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("start_date");
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => new { e.MemberId, e.ProductId });

                entity.ToTable("Cart");

                entity.Property(e => e.MemberId).HasColumnName("member_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.CartProductQuantity).HasColumnName("cart_product_quantity");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cart_Member");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cart_Product");
            });

            modelBuilder.Entity<CustomerService>(entity =>
            {
                entity.ToTable("CustomerService");

                entity.Property(e => e.CustomerServiceId).HasColumnName("customer_service_id");

                entity.Property(e => e.MemberId).HasColumnName("member_ID");

                entity.Property(e => e.Message).HasColumnName("message");

                entity.Property(e => e.MessageDatetime)
                    .HasMaxLength(50)
                    .HasColumnName("message_datetime");

                entity.Property(e => e.Reply).HasColumnName("reply");

                entity.Property(e => e.ReplyDatetime)
                    .HasMaxLength(50)
                    .HasColumnName("reply_datetime");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.CustomerServices)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MemberService_Member");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmpId);

                entity.ToTable("Employee");

                entity.Property(e => e.EmpId).HasColumnName("emp_id");

                entity.Property(e => e.Available).HasColumnName("available");

                entity.Property(e => e.EmpAccount)
                    .HasMaxLength(50)
                    .HasColumnName("emp_account");

                entity.Property(e => e.EmpName)
                    .HasMaxLength(50)
                    .HasColumnName("emp_name");

                entity.Property(e => e.EmpPassword)
                    .HasMaxLength(50)
                    .HasColumnName("emp_password");

                entity.Property(e => e.EmpPermission).HasColumnName("emp_permission");
            });

            modelBuilder.Entity<FocusDetail>(entity =>
            {
                entity.ToTable("FocusDetail");

                entity.Property(e => e.FocusDetailId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("focus_detail_id");

                entity.Property(e => e.Duration).HasColumnName("duration");

                entity.Property(e => e.EndDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("end_datetime");

                entity.Property(e => e.StartDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("start_datetime");
            });

            modelBuilder.Entity<FocusSlide>(entity =>
            {
                entity.HasKey(e => e.FocusId)
                    .HasName("PK_Focus");

                entity.ToTable("FocusSlide");

                entity.Property(e => e.FocusId).HasColumnName("focus_id");

                entity.Property(e => e.AudioPath).HasColumnName("audio_path");

                entity.Property(e => e.Category)
                    .HasMaxLength(100)
                    .HasColumnName("category");

                entity.Property(e => e.ImagePath).HasColumnName("image_path");
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.ToTable("Member");

                entity.Property(e => e.MemberId).HasColumnName("member_id");

                entity.Property(e => e.Available).HasColumnName("available");

                entity.Property(e => e.MemberAddress)
                    .HasMaxLength(100)
                    .HasColumnName("member_address");

                entity.Property(e => e.MemberBirthday)
                    .HasColumnType("date")
                    .HasColumnName("member_birthday");

                entity.Property(e => e.MemberEmail)
                    .HasMaxLength(50)
                    .HasColumnName("member_email");

                entity.Property(e => e.MemberJoinTime)
                    .HasColumnType("datetime")
                    .HasColumnName("member_joinTime");

                entity.Property(e => e.MemberName)
                    .HasMaxLength(50)
                    .HasColumnName("member_name");

                entity.Property(e => e.MemberPoint).HasColumnName("member_point");

                entity.Property(e => e.MemberSex).HasColumnName("member_sex");

                entity.Property(e => e.MemberTel)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("member_tel");

                entity.Property(e => e.Provider).HasMaxLength(50);

                entity.Property(e => e.ProviderUserId).HasMaxLength(100);
            });

            modelBuilder.Entity<MemberCredential>(entity =>
            {
                entity.HasKey(e => e.MemberId);

                entity.ToTable("MemberCredential");

                entity.Property(e => e.MemberId)
                    .ValueGeneratedNever()
                    .HasColumnName("member_id");

                entity.Property(e => e.MemberAccount)
                    .HasMaxLength(50)
                    .HasColumnName("member_account");

                entity.Property(e => e.MemberPassword)
                    .HasMaxLength(150)
                    .HasColumnName("member_password");

                entity.HasOne(d => d.Member)
                    .WithOne(p => p.MemberCredential)
                    .HasForeignKey<MemberCredential>(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MemberCredential_Member");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductId });

                entity.ToTable("OrderDetail");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.CartProductQuantity).HasColumnName("cart_product_quantity");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_Order");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_Product");
            });

            modelBuilder.Entity<PointHistory>(entity =>
            {
                entity.HasKey(e => e.PointDetailId);

                entity.ToTable("PointHistory");

                entity.Property(e => e.PointDetailId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("point_detail_id");

                entity.Property(e => e.MemberId).HasColumnName("member_id");

                entity.Property(e => e.ModifiedAmount).HasColumnName("modified_amount");

                entity.Property(e => e.ModifiedSource)
                    .HasMaxLength(50)
                    .HasColumnName("modified_source");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.PointHistories)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PointDetail_Member");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.ProductCategory)
                    .HasMaxLength(50)
                    .HasColumnName("product_category");

                entity.Property(e => e.ProductDesc).HasColumnName("product_desc");

                entity.Property(e => e.ProductImg)
                    .HasMaxLength(50)
                    .HasColumnName("product_img");

                entity.Property(e => e.ProductName)
                    .HasMaxLength(100)
                    .HasColumnName("product_name");

                entity.Property(e => e.ProductPrice).HasColumnName("product_price");

                entity.Property(e => e.ProductQuantity).HasColumnName("product_quantity");

                entity.Property(e => e.ProductState).HasColumnName("product_state");

                entity.Property(e => e.SupplierId).HasColumnName("supplier_id");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Supplier");
            });

            modelBuilder.Entity<ProductOrder>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("PK_Order");

                entity.ToTable("ProductOrder");

                entity.Property(e => e.OrderId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("order_id");

                entity.Property(e => e.MemberId).HasColumnName("member_id");

                entity.Property(e => e.OrderAddress).HasColumnName("order_address");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime")
                    .HasColumnName("order_date");

                entity.Property(e => e.OrderDelivery).HasColumnName("order_delivery");

                entity.Property(e => e.OrderNote).HasColumnName("order_note");

                entity.Property(e => e.OrderPayment).HasColumnName("order_payment");

                entity.Property(e => e.OrderState)
                    .HasMaxLength(500)
                    .HasColumnName("order_state");

                entity.Property(e => e.OrderTotalPrice).HasColumnName("order_totalPrice");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.ProductOrders)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Member");
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.ToTable("Purchase");

                entity.Property(e => e.PurchaseId).HasColumnName("purchase_id");

                entity.Property(e => e.PurchaseDate)
                    .HasColumnType("datetime")
                    .HasColumnName("purchase_date");

                entity.Property(e => e.PurchaseNote).HasColumnName("purchase_note");

                entity.Property(e => e.SupplierId).HasColumnName("supplier_id");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Purchases)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Purchase_Supplier");
            });

            modelBuilder.Entity<PurchaseDetail>(entity =>
            {
                entity.HasKey(e => new { e.PurchaseId, e.ProductId })
                    .HasName("PK_OrderProductDetail");

                entity.ToTable("PurchaseDetail");

                entity.Property(e => e.PurchaseId).HasColumnName("purchase_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.PurchasePrice).HasColumnName("purchase_price");

                entity.Property(e => e.PurchaseQuantity).HasColumnName("purchase_quantity");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.PurchaseDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PurchaseDetail_Product");

                entity.HasOne(d => d.Purchase)
                    .WithMany(p => p.PurchaseDetails)
                    .HasForeignKey(d => d.PurchaseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PurchaseDetail_Purchase");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Supplier");

                entity.Property(e => e.SupplierId).HasColumnName("supplier_id");

                entity.Property(e => e.SupplierAddress).HasColumnName("supplier_address");

                entity.Property(e => e.SupplierName)
                    .HasMaxLength(500)
                    .HasColumnName("supplier_name");

                entity.Property(e => e.SupplierTel)
                    .HasMaxLength(50)
                    .HasColumnName("supplier_tel");
            });

            modelBuilder.Entity<TarotCard>(entity =>
            {
                entity.ToTable("TarotCard");

                entity.Property(e => e.TarotCardId)
                    .ValueGeneratedNever()
                    .HasColumnName("tarotCard_id");

                entity.Property(e => e.TarotCardName)
                    .HasMaxLength(50)
                    .HasColumnName("tarotCard_name");

                entity.Property(e => e.TarotCardType)
                    .HasMaxLength(10)
                    .HasColumnName("tarotCard_type")
                    .IsFixedLength();
            });

            modelBuilder.Entity<TarotOrder>(entity =>
            {
                entity.ToTable("TarotOrder");

                entity.Property(e => e.TarotOrderId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("tarot_order_id");

                entity.Property(e => e.CardResult)
                    .HasMaxLength(50)
                    .HasColumnName("card_result");

                entity.Property(e => e.DivinationChat).HasColumnName("divination_chat");

                entity.Property(e => e.MemberId).HasColumnName("member_id");

                entity.Property(e => e.TarotTime)
                    .HasColumnType("datetime")
                    .HasColumnName("tarot_time");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.TarotOrders)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TarotOrder_Member");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
