namespace Ecom.API.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public int? PanelPromoId { get; set; }
        public string? PromoTextCat { get; set; }
        public double? SalePriceU { get; set; }
    }
}
