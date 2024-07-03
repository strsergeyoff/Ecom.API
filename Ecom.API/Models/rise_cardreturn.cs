namespace Ecom.API.Models
{
    /// <summary>
    /// Возвращенные товары
    /// </summary>
    public class rise_cardreturn
    {
        /// <summary>
        /// Индификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Изображение товара
        /// </summary>
        public string? Image { get; set; }

        /// <summary>
        /// Ссылка на товар
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Дата заказа.
        /// Присылается с явным указанием часового пояса
        /// </summary>
        public DateTime? Order_dt { get; set; }

        /// <summary>
        /// Баркод
        /// </summary>
        public string? Barcode { get; set; }

        /// <summary>
        /// Артикул
        /// </summary>
        public long? NmId { get; set; }

        /// <summary>
        /// Артикул продавца
        /// </summary>
        public string? Sa_name { get; set; }

        /// <summary>
        /// Размер
        /// </summary>
        public string? Ts_name { get; set; }

        /// <summary>
        /// Итоговая стоимость
        /// </summary>
        public double? TotalPriceDiscount { get; set; }

        /// <summary>
        /// Магазин
        /// </summary>
        public int ProjectId { get; set; }
    }
}
