﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
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

        public virtual DbSet<Announcement> Announcement { get; set; }
        public virtual DbSet<Cart> Cart { get; set; }
        public virtual DbSet<CustomerService> CustomerService { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<FocusDetail> FocusDetail { get; set; }
        public virtual DbSet<FocusSlide> FocusSlide { get; set; }
        public virtual DbSet<Member> Member { get; set; }
        public virtual DbSet<MemberCredential> MemberCredential { get; set; }
        public virtual DbSet<OrderDetail> OrderDetail { get; set; }
        public virtual DbSet<PointHistory> PointHistory { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductOrder> ProductOrder { get; set; }
        public virtual DbSet<Purchase> Purchase { get; set; }
        public virtual DbSet<PurchaseDetail> PurchaseDetail { get; set; }
        public virtual DbSet<Supplier> Supplier { get; set; }
        public virtual DbSet<TarotCard> TarotCard { get; set; }
        public virtual DbSet<TarotOrder> TarotOrder { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=20.89.169.61;User ID=lin; Password=fullstack133no3;Initial Catalog=ChilLax;Encrypt=False;TrustServerCertificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;");
                optionsBuilder.EnableSensitiveDataLogging();
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.Property(e => e.AnnouncementId).HasColumnName("announcement_id");

                entity.Property(e => e.Announcement1)
                    .IsRequired()
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

                entity.Property(e => e.MemberId).HasColumnName("member_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.CartProductQuantity).HasColumnName("cart_product_quantity");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.Cart)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cart_Member");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Cart)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cart_Product");
            });

            modelBuilder.Entity<CustomerService>(entity =>
            {
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
                    .WithMany(p => p.CustomerService)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MemberService_Member");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmpId);

                entity.Property(e => e.EmpId).HasColumnName("emp_id");

                entity.Property(e => e.Available).HasColumnName("available");

                entity.Property(e => e.EmpAccount)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("emp_account");

                entity.Property(e => e.EmpName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("emp_name");

                entity.Property(e => e.EmpPassword)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("emp_password");

                entity.Property(e => e.EmpPermission).HasColumnName("emp_permission");
            });

            modelBuilder.Entity<FocusDetail>(entity =>
            {
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

                entity.Property(e => e.FocusId).HasColumnName("focus_id");

                entity.Property(e => e.AudioPath)
                    .IsRequired()
                    .HasColumnName("audio_path");

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("category");

                entity.Property(e => e.ImagePath)
                    .IsRequired()
                    .HasColumnName("image_path");
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.Property(e => e.MemberId).HasColumnName("member_id");

                entity.Property(e => e.Available).HasColumnName("available");

                entity.Property(e => e.MemberAddress)
                    .HasMaxLength(100)
                    .HasColumnName("member_address");

                entity.Property(e => e.MemberBirthday)
                    .HasColumnType("date")
                    .HasColumnName("member_birthday");

                entity.Property(e => e.MemberEmail)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("member_email");

                entity.Property(e => e.MemberJoinTime)
                    .HasColumnType("datetime")
                    .HasColumnName("member_joinTime");

                entity.Property(e => e.MemberName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("member_name");

                entity.Property(e => e.MemberPoint).HasColumnName("member_point");

                entity.Property(e => e.MemberSex).HasColumnName("member_sex");

                entity.Property(e => e.MemberTel)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("member_tel");

                entity.Property(e => e.Provider).HasMaxLength(50);

                entity.Property(e => e.ProviderUserId).HasMaxLength(100);
            });

            modelBuilder.Entity<MemberCredential>(entity =>
            {
                entity.HasKey(e => e.MemberId);

                entity.Property(e => e.MemberId)
                    .ValueGeneratedNever()
                    .HasColumnName("member_id");

                entity.Property(e => e.MemberAccount)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("member_account");

                entity.Property(e => e.MemberPassword)
                    .IsRequired()
                    .HasMaxLength(50)
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

                entity.Property(e => e.OrderId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("order_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.CartProductQuantity).HasColumnName("cart_product_quantity");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_Order");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_Product");
            });

            modelBuilder.Entity<PointHistory>(entity =>
            {
                entity.HasKey(e => e.PointDetailId);

                entity.Property(e => e.PointDetailId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("point_detail_id");

                entity.Property(e => e.MemberId).HasColumnName("member_id");

                entity.Property(e => e.ModifiedAmount).HasColumnName("modified_amount");

                entity.Property(e => e.ModifiedSource)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("modified_source");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.PointHistory)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PointDetail_Member");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.ProductCategory)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("product_category");

                entity.Property(e => e.ProductDesc)
                    .IsRequired()
                    .HasColumnName("product_desc");

                entity.Property(e => e.ProductImg)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("product_img");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("product_name");

                entity.Property(e => e.ProductPrice).HasColumnName("product_price");

                entity.Property(e => e.ProductQuantity).HasColumnName("product_quantity");

                entity.Property(e => e.ProductState).HasColumnName("product_state");

                entity.Property(e => e.SupplierId).HasColumnName("supplier_id");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Supplier");
            });

            modelBuilder.Entity<ProductOrder>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("PK_Order");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.MemberId).HasColumnName("member_id");

                entity.Property(e => e.OrderAddress)
                    .IsRequired()
                    .HasColumnName("order_address");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime")
                    .HasColumnName("order_date");

                entity.Property(e => e.OrderDelivery).HasColumnName("order_delivery");

                entity.Property(e => e.OrderNote).HasColumnName("order_note");

                entity.Property(e => e.OrderPayment).HasColumnName("order_payment");

                entity.Property(e => e.OrderState)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("order_state");

                entity.Property(e => e.OrderTotalPrice).HasColumnName("order_totalPrice");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.ProductOrder)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Member");
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.Property(e => e.PurchaseId).HasColumnName("purchase_id");

                entity.Property(e => e.PurchaseDate)
                    .HasColumnType("datetime")
                    .HasColumnName("purchase_date");

                entity.Property(e => e.PurchaseNote).HasColumnName("purchase_note");

                entity.Property(e => e.SupplierId).HasColumnName("supplier_id");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Purchase)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Purchase_Supplier");
            });

            modelBuilder.Entity<PurchaseDetail>(entity =>
            {
                entity.HasKey(e => new { e.PurchaseId, e.ProductId })
                    .HasName("PK_OrderProductDetail");

                entity.Property(e => e.PurchaseId).HasColumnName("purchase_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.PurchasePrice).HasColumnName("purchase_price");

                entity.Property(e => e.PurchaseQuantity).HasColumnName("purchase_quantity");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.PurchaseDetail)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PurchaseDetail_Product");

                entity.HasOne(d => d.Purchase)
                    .WithMany(p => p.PurchaseDetail)
                    .HasForeignKey(d => d.PurchaseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PurchaseDetail_Purchase");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.Property(e => e.SupplierId).HasColumnName("supplier_id");

                entity.Property(e => e.SupplierAddress)
                    .IsRequired()
                    .HasColumnName("supplier_address");

                entity.Property(e => e.SupplierName)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("supplier_name");

                entity.Property(e => e.SupplierTel)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("supplier_tel");
            });

            modelBuilder.Entity<TarotCard>(entity =>
            {
                entity.Property(e => e.TarotCardId)
                    .ValueGeneratedNever()
                    .HasColumnName("tarotCard_id");

                entity.Property(e => e.TarotCardName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("tarotCard_name");

                entity.Property(e => e.TarotCardType)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("tarotCard_type")
                    .IsFixedLength();
            });

            modelBuilder.Entity<TarotOrder>(entity =>
            {
                entity.Property(e => e.TarotOrderId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("tarot_order_id");

                entity.Property(e => e.CardResult)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("card_result");

                entity.Property(e => e.DivinationChat)
                    .IsRequired()
                    .HasColumnName("divination_chat");

                entity.Property(e => e.MemberId).HasColumnName("member_id");

                entity.Property(e => e.TarotTime)
                    .HasColumnType("datetime")
                    .HasColumnName("tarot_time");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.TarotOrder)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TarotOrder_Member");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}