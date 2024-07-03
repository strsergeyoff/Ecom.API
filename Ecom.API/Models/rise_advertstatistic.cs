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
        public double? Ctr { get; set; }

        /// <summary>
        /// Средняя стоимость клика, ₽
        /// </summary>
        public double? Cpc { get; set; }

        /// <summary>
        /// Затраты, ₽
        /// </summary>
        public double? Sum { get; set; }

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
        public double? Cr { get; set; }

        /// <summary>
        /// Количество заказанных товаров, шт
        /// </summary>
        public int? Shks { get; set; }

        /// <summary>
        /// Заказов на сумму, ₽
        /// </summary>
        public double? Sum_price { get; set; }

        /// <summary>
        /// Рекламная кампания
        /// </summary>
        public int AdvertId { get; set; }
    }
}