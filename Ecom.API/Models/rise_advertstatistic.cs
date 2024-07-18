namespace Ecom.API.Models
{
    /// <summary>
    /// Статистика рекламной кампании товара
    /// </summary>
    public class rise_advertstatistic
    {
        /// <summary>
        /// Индификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Артикул товара
        /// </summary>
        public long? nmId { get; set; }

        /// <summary>
        /// Артикул продавца
        /// </summary>
        public string? Sa_name { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Показы
        /// </summary>
        public int? Views { get; set; }

        /// <summary>
        /// Клики
        /// </summary>
        public int? Clicks { get; set; }

        /// <summary>
        /// Показатель кликабельности, отношение числа кликов к количеству показов, %
        /// </summary>
        public decimal? Ctr { get; set; }

        /// <summary>
        /// Средняя стоимость клика, ₽
        /// </summary>
        public decimal? Cpc { get; set; }

        /// <summary>
        /// Затраты, ₽
        /// </summary>
        public decimal? Sum { get; set; }

        /// <summary>
        /// Количество добавлений товаров в корзину
        /// </summary>
        public int? Atbs { get; set; }

        /// <summary>
        /// Количество заказов
        /// </summary>
        public int? Orders { get; set; }

        /// <summary>
        /// CR(conversion rate) — отношение количества заказов к общему количеству посещений кампании
        /// </summary>
        public decimal? Cr { get; set; }

        /// <summary>
        /// Количество заказанных товаров, шт
        /// </summary>
        public int? Shks { get; set; }

        /// <summary>
        /// Заказов на сумму, ₽
        /// </summary>
        public decimal? Sum_price { get; set; }

        /// <summary>
        /// Рекламная кампания
        /// </summary>
        public int AdvertId { get; set; }
    }
}