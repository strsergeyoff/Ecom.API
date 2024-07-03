using Microsoft.EntityFrameworkCore;

namespace Ecom.API.Models
{
    /// <summary>
    /// Лента товара
    /// </summary>
    public class rise_feed
    {
        /// <summary>
        /// Индификатор товара
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
        /// Артикул продавца
        /// </summary>
        public string? Sa_name { get; set; }

        /// <summary>
        /// Артикул
        /// </summary>
        public long? NmId { get; set; }

        /// <summary>
        /// Баркод
        /// </summary>
        public string? Barcode { get; set; }

        /// <summary>
        /// Размер
        /// </summary>
        public string? Ts_name { get; set; }

        /// <summary>
        /// Закупочная стоимость
        /// </summary>
        public double? CostPrice { get; set; }

        /// <summary>
        /// Поставлено количество
        /// </summary>
        public int? QuantityOfSupplies { get; set; }

        /// <summary>
        /// Дата крайней поставки
        /// </summary>
        public DateTime? DateTimeQuantityOfSupplies { get; set; }

        /// <summary>
        /// Количество на складе
        /// </summary>
        public int? InStock { get; set; }

        /// <summary>
        /// Комиссия
        /// </summary>
        public double? Commision { get; set; }

        /// <summary>
        /// Логистика
        /// </summary>
        public double? Logistics { get; set; }


        /// <summary>
        /// Заказано
        /// </summary>
        public int? OrderedCount { get; set; }

        /// <summary>
        /// Выкуплены
        /// </summary>
        public int? PurchasedCount { get; set; }

        /// <summary>
        /// Отменено
        /// </summary>
        public int? CancelCount { get; set; }

        /// <summary>
        /// Отправлено
        /// </summary>
        public int? DispatchCount { get; set; }

        /// <summary>
        /// Возвращено
        /// </summary>
        public int? ReturnCount { get; set; }

        /// <summary>
        /// Без статуса
        /// </summary>
        public int? WitchStatusCount { get; set; }

        /// <summary>
        ///  Заказано на сумму
        /// </summary>
        public double? OrderSummaPrice { get; set; }

        /// <summary>
        /// Отменено на сумму
        /// </summary>
        public double? CancelSummaPrice { get; set; }

        /// <summary>
        /// Отправлено на сумму
        /// </summary>
        public double? DispatchSummaPrice { get; set; }

        /// <summary>
        /// Куплено на сумму
        /// </summary>
        public double? PurchasedSummaPrice { get; set; }

        /// <summary>
        /// Возвращено на сумму
        /// </summary>
        public double? ReturnSummaPrice { get; set; }

        /// <summary>
        /// Бренд
        /// </summary>
        public string? Brand_name { get; set; }

        /// <summary>
        /// Тип товара
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// Категория
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Теги
        /// </summary>
        public string? Tags { get; set; }

        /// <summary>
        /// Магазин
        /// </summary>
        public int ProjectId { get; set; }
    }
}
