namespace Ecom.API.Models
{
    /// <summary>
    /// Без статуса товры
    /// </summary>
    public class rise_cardnullstatus
    {
        /// <summary>
        /// Индификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Уникальный индификатор заказа
        /// </summary>
        public string? Srid { get; set; }

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
