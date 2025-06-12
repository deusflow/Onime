using System.ComponentModel.DataAnnotations;

namespace OnimeBestofrieeeendo.Models
{
    public class TradeItems

    {
        public int ShopItemId { get; set; }
        [Required]
        public int Price { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
        public bool Available { get; set; } = true;
        [Required]
        public string ItemName { get; set; }
        [Required]
        public string ItemType { get; set; }
        [Required]
        public string Rarity { get; set; }
        public string? ImageUrl { get; set; }
        [Required]
        public string? Description { get; set; }
    }

}
