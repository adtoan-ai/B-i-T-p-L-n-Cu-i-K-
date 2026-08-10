using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GlassesShop.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountID);
                    table.CheckConstraint("CK_Account_Role", "[Role] IN (N'Customer', N'Staff', N'Admin')");
                });

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    BrandID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.BrandID);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NumberPhone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerID);
                    table.CheckConstraint("CK_Customer_Gender", "[Gender] IN (N'Nam', N'Nữ')");
                    table.ForeignKey(
                        name: "FK_Customers_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Staffs",
                columns: table => new
                {
                    StaffID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NumberPhone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staffs", x => x.StaffID);
                    table.ForeignKey(
                        name: "FK_Staffs_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    BrandID = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Style = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Material = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductID);
                    table.ForeignKey(
                        name: "FK_Products_Brands_BrandID",
                        column: x => x.BrandID,
                        principalTable: "Brands",
                        principalColumn: "BrandID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    CartID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.CartID);
                    table.ForeignKey(
                        name: "FK_Carts_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<int>(type: "int", nullable: false),
                    StaffID = table.Column<int>(type: "int", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ReceiverName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReceiverPhone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ShippingAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrderStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderID);
                    table.CheckConstraint("CK_Order_Status", "[OrderStatus] IN (N'Chờ xác nhận', N'Đã xác nhận', N'Đang giao', N'Đã giao', N'Đã hủy')");
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Staffs_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staffs",
                        principalColumn: "StaffID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariants",
                columns: table => new
                {
                    VariantID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariants", x => x.VariantID);
                    table.CheckConstraint("CK_Variant_Price", "[Price] > 0");
                    table.CheckConstraint("CK_Variant_Stock", "[StockQuantity] >= 0");
                    table.ForeignKey(
                        name: "FK_ProductVariants_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentID);
                    table.CheckConstraint("CK_Payment_Status", "[PaymentStatus] IN (N'Chưa thanh toán', N'Đã thanh toán', N'Thất bại', N'Đã hoàn tiền')");
                    table.ForeignKey(
                        name: "FK_Payments_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartDetails",
                columns: table => new
                {
                    CartDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartID = table.Column<int>(type: "int", nullable: false),
                    VariantID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartDetails", x => x.CartDetailID);
                    table.CheckConstraint("CK_CartDetail_Quantity", "[Quantity] > 0");
                    table.ForeignKey(
                        name: "FK_CartDetails_Carts_CartID",
                        column: x => x.CartID,
                        principalTable: "Carts",
                        principalColumn: "CartID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartDetails_ProductVariants_VariantID",
                        column: x => x.VariantID,
                        principalTable: "ProductVariants",
                        principalColumn: "VariantID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    OrderDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    VariantID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.OrderDetailID);
                    table.CheckConstraint("CK_OrderDetail_Quantity", "[Quantity] > 0");
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_ProductVariants_VariantID",
                        column: x => x.VariantID,
                        principalTable: "ProductVariants",
                        principalColumn: "VariantID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    ImageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VariantID = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IsMain = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.ImageID);
                    table.ForeignKey(
                        name: "FK_ProductImages_ProductVariants_VariantID",
                        column: x => x.VariantID,
                        principalTable: "ProductVariants",
                        principalColumn: "VariantID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountID", "CreatedAt", "IsLocked", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "cIxkaRlf6+h8hvydSSwJW+VqHp2aXJrJ8Bz2hNZ0AVQ=", "Admin", "admin" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "TeAIjA2k0T3/YkinS8oU1/Aj+tsg7T1fD9GwRJ1AZFo=", "Staff", "staff01" },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "SFBkCgS9TjspxIZa0JiKUgMgcX/nDdwbOrLltWb+uS8=", "Customer", "khachhang01" }
                });

            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "BrandID", "BrandName", "Description" },
                values: new object[,]
                {
                    { 1, "Ray-Ban", "Thương hiệu kính mắt Mỹ" },
                    { 2, "Gucci", "Thương hiệu thời trang Ý" },
                    { 3, "Oakley", "Kính thể thao cao cấp" },
                    { 4, "Molsion", "Thương hiệu kính Trung Quốc" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryID", "CategoryName", "Description" },
                values: new object[] { 1, "Kính thời trang", "Kính mắt thời trang hoàn chỉnh" });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerID", "AccountID", "Address", "DateOfBirth", "Email", "FullName", "Gender", "NumberPhone" },
                values: new object[] { 1, 3, "123 Cầu Giấy, Hà Nội", null, "mai@gmail.com", "Trần Thị Mai", "Nữ", "0987654321" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductID", "BrandID", "CategoryID", "CreatedAt", "Description", "IsActive", "Material", "ProductName", "Style" },
                values: new object[,]
                {
                    { 1, 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kính phi công huyền thoại, gọng kim loại nhẹ, tròng chống UV400.", true, "Kim loại", "Kính Ray-Ban Aviator Classic", "Phi công" },
                    { 2, 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Thiết kế vuông cổ điển, phù hợp mọi khuôn mặt.", true, "Nhựa Acetate", "Kính Ray-Ban Wayfarer", "Vuông" },
                    { 3, 2, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kính bản to sang trọng, họa tiết đặc trưng Gucci.", true, "Nhựa Acetate", "Kính Gucci Oversized GG0022", "Oversized" },
                    { 4, 3, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kính thể thao năng động, chống trầy xước.", true, "Nhựa O-Matter", "Kính Oakley Holbrook", "Vuông" },
                    { 5, 4, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kính mắt mèo nữ tính, tôn dáng khuôn mặt.", true, "Kim loại", "Kính Molsion Cat Eye MS3005", "Mắt mèo" },
                    { 6, 4, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kính tròn phong cách retro, nhẹ và bền.", true, "Titanium", "Kính Molsion Round MS5012", "Tròn" }
                });

            migrationBuilder.InsertData(
                table: "Staffs",
                columns: new[] { "StaffID", "AccountID", "Address", "Email", "FullName", "NumberPhone" },
                values: new object[,]
                {
                    { 1, 1, "Hà Nội", "admin@glassesshop.vn", "Quản Trị Viên", "0901234567" },
                    { 2, 2, "Hà Nội", "staff01@glassesshop.vn", "Nguyễn Văn Bán", "0912345678" }
                });

            migrationBuilder.InsertData(
                table: "Carts",
                columns: new[] { "CartID", "CreatedAt", "CustomerID" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "ProductVariants",
                columns: new[] { "VariantID", "Color", "IsActive", "Price", "ProductID", "StockQuantity" },
                values: new object[,]
                {
                    { 1, "Đen", true, 2500000m, 1, 20 },
                    { 2, "Vàng Gold", true, 2800000m, 1, 15 },
                    { 3, "Đen", true, 2200000m, 2, 25 },
                    { 4, "Nâu", true, 2200000m, 2, 10 },
                    { 5, "Đen", true, 7500000m, 3, 8 },
                    { 6, "Nâu", true, 7800000m, 3, 5 },
                    { 7, "Đen", true, 3200000m, 4, 18 },
                    { 8, "Xanh", true, 3400000m, 4, 12 },
                    { 9, "Hồng", true, 1800000m, 5, 30 },
                    { 10, "Bạc", true, 1750000m, 5, 22 },
                    { 11, "Bạc", true, 2100000m, 6, 16 },
                    { 12, "Vàng Gold", true, 2300000m, 6, 9 }
                });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "ImageID", "DisplayOrder", "ImageUrl", "IsMain", "VariantID" },
                values: new object[,]
                {
                    { 1, 1, "/images/products/rayban-aviator-den.jpg", true, 1 },
                    { 2, 1, "/images/products/rayban-aviator-gold.jpg", true, 2 },
                    { 3, 1, "/images/products/rayban-wayfarer-den.jpg", true, 3 },
                    { 4, 1, "/images/products/rayban-wayfarer-nau.jpg", true, 4 },
                    { 5, 1, "/images/products/gucci-oversized-den.jpg", true, 5 },
                    { 6, 1, "/images/products/gucci-oversized-nau.jpg", true, 6 },
                    { 7, 1, "/images/products/oakley-holbrook-den.jpg", true, 7 },
                    { 8, 1, "/images/products/oakley-holbrook-xanh.jpg", true, 8 },
                    { 9, 1, "/images/products/molsion-cateye-hong.jpg", true, 9 },
                    { 10, 1, "/images/products/molsion-cateye-bac.jpg", true, 10 },
                    { 11, 1, "/images/products/molsion-round-bac.jpg", true, 11 },
                    { 12, 1, "/images/products/molsion-round-gold.jpg", true, 12 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Username",
                table: "Accounts",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartDetails_CartID_VariantID",
                table: "CartDetails",
                columns: new[] { "CartID", "VariantID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartDetails_VariantID",
                table: "CartDetails",
                column: "VariantID");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CustomerID",
                table: "Carts",
                column: "CustomerID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AccountID",
                table: "Customers",
                column: "AccountID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_NumberPhone",
                table: "Customers",
                column: "NumberPhone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderID_VariantID",
                table: "OrderDetails",
                columns: new[] { "OrderID", "VariantID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_VariantID",
                table: "OrderDetails",
                column: "VariantID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerID",
                table: "Orders",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_StaffID",
                table: "Orders",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderID",
                table: "Payments",
                column: "OrderID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_VariantID",
                table: "ProductImages",
                column: "VariantID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandID",
                table: "Products",
                column: "BrandID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryID",
                table: "Products",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ProductID_Color",
                table: "ProductVariants",
                columns: new[] { "ProductID", "Color" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_AccountID",
                table: "Staffs",
                column: "AccountID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_Email",
                table: "Staffs",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartDetails");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ProductVariants");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Staffs");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
