using System.ComponentModel.DataAnnotations;
using GlassesShop.Models.Entities;

namespace GlassesShop.Models.ViewModels
{
    public class ProductFormVM
    {
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(200)]
        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thương hiệu")]
        [Display(Name = "Thương hiệu")]
        public int BrandID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập kiểu dáng")]
        [StringLength(50)]
        [Display(Name = "Kiểu dáng")]
        public string Style { get; set; } = null!;

        [StringLength(100)]
        [Display(Name = "Chất liệu")]
        public string? Material { get; set; }

        [StringLength(2000)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Đang kinh doanh")]
        public bool IsActive { get; set; } = true;

        public List<Category> Categories { get; set; } = new();
        public List<Brand> Brands { get; set; } = new();
    }

    public class AdminProductListVM
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public string BrandName { get; set; } = null!;
        public string Style { get; set; } = null!;
        public string MainImageUrl { get; set; } = "/images/no-image.png";
        public int VariantCount { get; set; }
        public int TotalStock { get; set; }
        public decimal MinPrice { get; set; }
        public bool IsActive { get; set; }
    }

    public class VariantFormVM
    {
        public int VariantID { get; set; }

        public int ProductID { get; set; }

        public string ProductName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập màu sắc")]
        [StringLength(50)]
        [Display(Name = "Màu sắc")]
        public string Color { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        [Range(1, 999999999, ErrorMessage = "Giá phải lớn hơn 0")]
        [Display(Name = "Giá bán (₫)")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn không được âm")]
        [Display(Name = "Số lượng tồn")]
        public int StockQuantity { get; set; }

        [Display(Name = "Đang kinh doanh")]
        public bool IsActive { get; set; } = true;

        public List<ProductImage> ExistingImages { get; set; } = new();
    }
}