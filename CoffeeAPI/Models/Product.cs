using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoffeeAPI.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,3)")]
        public decimal Price { get; set; }

        public string? Img { get; set; }

        [NotMapped] // Không lưu vào database
        [JsonIgnore]
        public IFormFile? ImageFile { get; set; } // Nhận file ảnh từ form
        public int CategoryId { get; set; }

        public int EmployeeId { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; }  = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        [ForeignKey("CategoryId")]
        [JsonIgnore] // Bỏ qua khi tuần tự hóa JSON
        public ProductCategories? Category { get; set; }

        [ForeignKey("EmployeeId")]
        [JsonIgnore] // Bỏ qua khi tuần tự hóa JSON
        public Employee? Employee { get; set; }

        [JsonIgnore] // Bỏ qua khi tuần tự hóa JSON
        public ICollection<Feedback>? Feedbacks { get; set; }

        [JsonIgnore] // Bỏ qua khi tuần tự hóa JSON
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
