namespace Onime.Models;

public class ShopProductView
{
    public int ShopItemId { get; set; }       // id из таблицы shop_items
    public int ItemId { get; set; }           // id товара из таблицы shop_items
    public string Name { get; set; } = "";    // название из products
    public string ImageUrl { get; set; } = ""; // картинка из products
    public decimal Price { get; set; }        // цена из shop_items
    public int Quantity { get; set; }         // количество из shop_items
    
    public string Description { get; set; } = ""; // описание товара
}