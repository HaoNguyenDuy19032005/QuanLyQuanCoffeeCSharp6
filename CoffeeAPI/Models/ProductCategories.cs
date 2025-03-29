using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoffeeAPI.Models
{
    public class ProductCategories
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [JsonIgnore]
        public ICollection<Product>? Products { get; set; }
    }
}
