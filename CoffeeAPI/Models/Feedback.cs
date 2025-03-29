using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoffeeAPI.Models
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        public byte Rating { get; set; }

        public int ProductId { get; set; }

        // Navigation Property
        [ForeignKey("ProductId")]
        [JsonIgnore]
        public Product? Product { get; set; }
    }
}
