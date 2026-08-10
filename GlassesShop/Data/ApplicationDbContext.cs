using GlassesShop.Helpers;
using GlassesShop.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace GlassesShop.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasIndex(a => a.Username).IsUnique();
                entity.ToTable(t => t.HasCheckConstraint("CK_Account_Role",
                    "[Role] IN (N'Customer', N'Staff', N'Admin')"));
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(c => c.NumberPhone).IsUnique();
                entity.HasIndex(c => c.Email).IsUnique();
                entity.HasIndex(c => c.AccountID).IsUnique();
                entity.ToTable(t => t.HasCheckConstraint("CK_Customer_Gender",
                    "[Gender] IN (N'Nam', N'Nữ')"));
                entity.HasOne(c => c.Account)
                      .WithOne(a => a.Customer)
                      .HasForeignKey<Customer>(c => c.AccountID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.HasIndex(s => s.AccountID).IsUnique();
                entity.HasIndex(s => s.Email).IsUnique();
                entity.HasOne(s => s.Account)
                      .WithOne(a => a.Staff)
                      .HasForeignKey<Staff>(s => s.AccountID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryID)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(p => p.Brand)
                      .WithMany(b => b.Products)
                      .HasForeignKey(p => p.BrandID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProductVariant>(entity =>
            {
                entity.HasIndex(v => new { v.ProductID, v.Color }).IsUnique();
                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Variant_Price", "[Price] > 0");
                    t.HasCheckConstraint("CK_Variant_Stock", "[StockQuantity] >= 0");
                });
                entity.HasOne(v => v.Product)
                      .WithMany(p => p.Variants)
                      .HasForeignKey(v => v.ProductID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasOne(i => i.Variant)
                      .WithMany(v => v.Images)
                      .HasForeignKey(i => i.VariantID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasIndex(c => c.CustomerID).IsUnique();
                entity.HasOne(c => c.Customer)
                      .WithOne(cu => cu.Cart)
                      .HasForeignKey<Cart>(c => c.CustomerID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CartDetail>(entity =>
            {
                entity.HasIndex(cd => new { cd.CartID, cd.VariantID }).IsUnique();
                entity.ToTable(t => t.HasCheckConstraint("CK_CartDetail_Quantity", "[Quantity] > 0"));
                entity.HasOne(cd => cd.Cart)
                      .WithMany(c => c.CartDetails)
                      .HasForeignKey(cd => cd.CartID)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(cd => cd.Variant)
                      .WithMany(v => v.CartDetails)
                      .HasForeignKey(cd => cd.VariantID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable(t => t.HasCheckConstraint("CK_Order_Status",
                    "[OrderStatus] IN (N'Chờ xác nhận', N'Đã xác nhận', N'Đang giao', N'Đã giao', N'Đã hủy')"));
                entity.HasOne(o => o.Customer)
                      .WithMany(c => c.Orders)
                      .HasForeignKey(o => o.CustomerID)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(o => o.Staff)
                      .WithMany(s => s.Orders)
                      .HasForeignKey(o => o.StaffID)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasIndex(od => new { od.OrderID, od.VariantID }).IsUnique();
                entity.ToTable(t => t.HasCheckConstraint("CK_OrderDetail_Quantity", "[Quantity] > 0"));
                entity.HasOne(od => od.Order)
                      .WithMany(o => o.OrderDetails)
                      .HasForeignKey(od => od.OrderID)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(od => od.Variant)
                      .WithMany(v => v.OrderDetails)
                      .HasForeignKey(od => od.VariantID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasIndex(p => p.OrderID).IsUnique();
                entity.ToTable(t => t.HasCheckConstraint("CK_Payment_Status",
                    "[PaymentStatus] IN (N'Chưa thanh toán', N'Đã thanh toán', N'Thất bại', N'Đã hoàn tiền')"));
                entity.HasOne(p => p.Order)
                      .WithOne(o => o.Payment)
                      .HasForeignKey<Payment>(p => p.OrderID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0);

            modelBuilder.Entity<Account>().HasData(
                new Account { AccountID = 1, Username = "admin", PasswordHash = PasswordHelper.Hash("admin123"), Role = "Admin", IsLocked = false, CreatedAt = seedDate },
                new Account { AccountID = 2, Username = "staff01", PasswordHash = PasswordHelper.Hash("staff123"), Role = "Staff", IsLocked = false, CreatedAt = seedDate },
                new Account { AccountID = 3, Username = "khachhang01", PasswordHash = PasswordHelper.Hash("123456"), Role = "Customer", IsLocked = false, CreatedAt = seedDate }
            );

            modelBuilder.Entity<Staff>().HasData(
                new Staff { StaffID = 1, AccountID = 1, FullName = "Quản Trị Viên", NumberPhone = "0901234567", Email = "admin@glassesshop.vn", Address = "Hà Nội" },
                new Staff { StaffID = 2, AccountID = 2, FullName = "Nguyễn Văn Bán", NumberPhone = "0912345678", Email = "staff01@glassesshop.vn", Address = "Hà Nội" }
            );

            modelBuilder.Entity<Customer>().HasData(
                new Customer { CustomerID = 1, AccountID = 3, FullName = "Trần Thị Mai", Gender = "Nữ", NumberPhone = "0987654321", Email = "mai@gmail.com", Address = "123 Cầu Giấy, Hà Nội" }
            );

            modelBuilder.Entity<Cart>().HasData(
                new Cart { CartID = 1, CustomerID = 1, CreatedAt = seedDate }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryID = 1, CategoryName = "Kính thời trang", Description = "Kính mắt thời trang hoàn chỉnh" }
            );

            modelBuilder.Entity<Brand>().HasData(
                new Brand { BrandID = 1, BrandName = "Ray-Ban", Description = "Thương hiệu kính mắt Mỹ" },
                new Brand { BrandID = 2, BrandName = "Gucci", Description = "Thương hiệu thời trang Ý" },
                new Brand { BrandID = 3, BrandName = "Oakley", Description = "Kính thể thao cao cấp" },
                new Brand { BrandID = 4, BrandName = "Molsion", Description = "Thương hiệu kính Trung Quốc" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { ProductID = 1, ProductName = "Kính Ray-Ban Aviator Classic", CategoryID = 1, BrandID = 1, Description = "Kính phi công huyền thoại, gọng kim loại nhẹ, tròng chống UV400.", Style = "Phi công", Material = "Kim loại", IsActive = true, CreatedAt = seedDate },
                new Product { ProductID = 2, ProductName = "Kính Ray-Ban Wayfarer", CategoryID = 1, BrandID = 1, Description = "Thiết kế vuông cổ điển, phù hợp mọi khuôn mặt.", Style = "Vuông", Material = "Nhựa Acetate", IsActive = true, CreatedAt = seedDate },
                new Product { ProductID = 3, ProductName = "Kính Gucci Oversized GG0022", CategoryID = 1, BrandID = 2, Description = "Kính bản to sang trọng, họa tiết đặc trưng Gucci.", Style = "Oversized", Material = "Nhựa Acetate", IsActive = true, CreatedAt = seedDate },
                new Product { ProductID = 4, ProductName = "Kính Oakley Holbrook", CategoryID = 1, BrandID = 3, Description = "Kính thể thao năng động, chống trầy xước.", Style = "Vuông", Material = "Nhựa O-Matter", IsActive = true, CreatedAt = seedDate },
                new Product { ProductID = 5, ProductName = "Kính Molsion Cat Eye MS3005", CategoryID = 1, BrandID = 4, Description = "Kính mắt mèo nữ tính, tôn dáng khuôn mặt.", Style = "Mắt mèo", Material = "Kim loại", IsActive = true, CreatedAt = seedDate },
                new Product { ProductID = 6, ProductName = "Kính Molsion Round MS5012", CategoryID = 1, BrandID = 4, Description = "Kính tròn phong cách retro, nhẹ và bền.", Style = "Tròn", Material = "Titanium", IsActive = true, CreatedAt = seedDate }
            );

            modelBuilder.Entity<ProductVariant>().HasData(
                new ProductVariant { VariantID = 1, ProductID = 1, Color = "Đen", Price = 2500000m, StockQuantity = 20, IsActive = true },
                new ProductVariant { VariantID = 2, ProductID = 1, Color = "Vàng Gold", Price = 2800000m, StockQuantity = 15, IsActive = true },
                new ProductVariant { VariantID = 3, ProductID = 2, Color = "Đen", Price = 2200000m, StockQuantity = 25, IsActive = true },
                new ProductVariant { VariantID = 4, ProductID = 2, Color = "Nâu", Price = 2200000m, StockQuantity = 10, IsActive = true },
                new ProductVariant { VariantID = 5, ProductID = 3, Color = "Đen", Price = 7500000m, StockQuantity = 8, IsActive = true },
                new ProductVariant { VariantID = 6, ProductID = 3, Color = "Nâu", Price = 7800000m, StockQuantity = 5, IsActive = true },
                new ProductVariant { VariantID = 7, ProductID = 4, Color = "Đen", Price = 3200000m, StockQuantity = 18, IsActive = true },
                new ProductVariant { VariantID = 8, ProductID = 4, Color = "Xanh", Price = 3400000m, StockQuantity = 12, IsActive = true },
                new ProductVariant { VariantID = 9, ProductID = 5, Color = "Hồng", Price = 1800000m, StockQuantity = 30, IsActive = true },
                new ProductVariant { VariantID = 10, ProductID = 5, Color = "Bạc", Price = 1750000m, StockQuantity = 22, IsActive = true },
                new ProductVariant { VariantID = 11, ProductID = 6, Color = "Bạc", Price = 2100000m, StockQuantity = 16, IsActive = true },
                new ProductVariant { VariantID = 12, ProductID = 6, Color = "Vàng Gold", Price = 2300000m, StockQuantity = 9, IsActive = true }
            );

            modelBuilder.Entity<ProductImage>().HasData(
                new ProductImage { ImageID = 1, VariantID = 1, ImageUrl = "/images/products/rayban-aviator-den.jpg", IsMain = true, DisplayOrder = 1 },
                new ProductImage { ImageID = 2, VariantID = 2, ImageUrl = "/images/products/rayban-aviator-gold.jpg", IsMain = true, DisplayOrder = 1 },
                new ProductImage { ImageID = 3, VariantID = 3, ImageUrl = "/images/products/rayban-wayfarer-den.jpg", IsMain = true, DisplayOrder = 1 },
                new ProductImage { ImageID = 4, VariantID = 4, ImageUrl = "/images/products/rayban-wayfarer-nau.jpg", IsMain = true, DisplayOrder = 1 },
                new ProductImage { ImageID = 5, VariantID = 5, ImageUrl = "/images/products/gucci-oversized-den.jpg", IsMain = true, DisplayOrder = 1 },
                new ProductImage { ImageID = 6, VariantID = 6, ImageUrl = "/images/products/gucci-oversized-nau.jpg", IsMain = true, DisplayOrder = 1 },
                new ProductImage { ImageID = 7, VariantID = 7, ImageUrl = "/images/products/oakley-holbrook-den.jpg", IsMain = true, DisplayOrder = 1 },
                new ProductImage { ImageID = 8, VariantID = 8, ImageUrl = "/images/products/oakley-holbrook-xanh.jpg", IsMain = true, DisplayOrder = 1 },
                new ProductImage { ImageID = 9, VariantID = 9, ImageUrl = "/images/products/molsion-cateye-hong.jpg", IsMain = true, DisplayOrder = 1 },
                new ProductImage { ImageID = 10, VariantID = 10, ImageUrl = "/images/products/molsion-cateye-bac.jpg", IsMain = true, DisplayOrder = 1 },
                new ProductImage { ImageID = 11, VariantID = 11, ImageUrl = "/images/products/molsion-round-bac.jpg", IsMain = true, DisplayOrder = 1 },
                new ProductImage { ImageID = 12, VariantID = 12, ImageUrl = "/images/products/molsion-round-gold.jpg", IsMain = true, DisplayOrder = 1 }
            );
        }
    }
}