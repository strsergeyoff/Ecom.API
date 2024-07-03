namespace Ecom.API.Models
{
    /// <summary>
    /// Цены
    /// </summary>
    public class Price
    {
        /// <summary>
        /// Артикул продавца
        /// </summary>
        public string? Sa_name { get; set; }

        /// <summary>
        /// Артикул товара
        /// </summary>
        public long? nmID { get; set; }

        /// <summary>
        ///  Цена до скидки
        /// </summary>
        public double? PriceBeforeDiscont { get; set; }

        /// <summary>
        /// Процент скидки
        /// </summary>
        public double? Discount { get; set; }

        /// <summary>
        /// Цена после скидки
        /// </summary>
        public double? PriceAfterDiscount { get; set; }
    }
}
