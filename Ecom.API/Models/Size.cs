namespace Ecom.API.Models
{
    public class Size
    {
        public string? techSize { get; set; }
        public List<string> skus { get; set; }
        public double? Price { get; set; }
        public double? DiscountedPrice { get; set; }
    }
}
