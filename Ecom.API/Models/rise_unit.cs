namespace Ecom.API.Models
{
    public class rise_unit
    {
        public int Id { get; set; }

        /// <summary>
        /// Ссылка на товар
        /// </summary>
        public string? Url { get; set; }

        public string? Tags { get; set; }

        /// <summary>
        /// Артикул продавца
        /// </summary>
        public string? Sa_name { get; set; }

        /// <summary>
        /// Артикул товара
        /// </summary>
        public long? NmID { get; set; }

        /// <summary>
        /// Себестоимость
        /// </summary>
        public double? Cost_price { get; set; }

        /// <summary>
        /// Название акции
        /// </summary>
        public string? Promotion_name { get; set; }

        /// <summary>
        /// Заказано сегодня
        /// </summary>
        public int Ordered_today { get; set; }

        /// <summary>
        /// Средняя комиссия в процентах
        /// </summary>
        public double? AvgCommissionPercent { get; set; }

        /// <summary>
        /// Средняя стоимость логистики
        /// </summary>
        public double? AvgDeliveryRub { get; set; }

        public bool IsCommissionRecorded { get; set; }

        public bool IsLogisticsRecorded { get; set; }

        /// <summary>
        /// Фактическая цена до
        /// </summary>
        public double? PriceBeforeDiscount { get; set; }

        /// <summary>
        /// Фактическая скидка
        /// </summary>
        public double? Discount { get; set; }

        /// <summary>
        /// Фактическая цена после
        /// </summary>
        public double? PriceAfterDiscount { get; set; }

        public int ProjectId { get; set; }
    }
}
