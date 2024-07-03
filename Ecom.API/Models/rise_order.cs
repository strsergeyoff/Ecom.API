namespace Ecom.API.Models
{
    /// <summary>
    /// Заказы
    /// </summary>
    public class rise_order
    {
        /// <summary>
        /// Индификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Дата отмены
        /// </summary>
        private DateTime? cancelDate;

        /// <summary>
        /// Дата и время заказа. Поле соответствует параметру dateFrom в запросе, если параметр flag=1.
        /// Если часовой пояс не указан, то используется Московское время (UTC+3).
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Дата и время обновления информации в сервисе. Поле соответствует параметру dateFrom в запросе, если параметр flag=0 или не указан.
        /// Если часовой пояс не указан, то используется Московское время (UTC+3).
        /// </summary>
        public DateTime? LastChangeDate { get; set; }

        /// <summary>
        /// Склад отгрузки
        /// </summary>
        public string? WarehouseName { get; set; }

        /// <summary>
        /// Страна
        /// </summary>
        public string? CountryName { get; set; }

        /// <summary>
        /// Округ
        /// </summary>
        public string? OblastOkrugName { get; set; }

        /// <summary>
        /// Регион
        /// </summary>
        public string? RegionName { get; set; }

        /// <summary>
        /// Артикул продавца
        /// </summary>
        public string? SupplierArticle { get; set; }

        /// <summary>
        /// Артикул WB
        /// </summary>
        public long? NmId { get; set; }

        /// <summary>
        /// Баркод
        /// </summary>
        public string? Barcode { get; set; }

        /// <summary>
        /// Категория
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Предмет
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// Бренд
        /// </summary>
        public string? Brand { get; set; }

        /// <summary>
        /// Размер товара
        /// </summary>
        public string? TechSize { get; set; }

        /// <summary>
        /// Номер поставки
        /// </summary>
        public long? IncomeId { get; set; }

        /// <summary>
        /// Договор поставки
        /// </summary>
        public bool? IsSupply { get; set; }

        /// <summary>
        /// Договор реализации
        /// </summary>
        public bool? IsRealization { get; set; }

        /// <summary>
        /// Цена без скидок
        /// </summary>
        public double? TotalPrice { get; set; }

        /// <summary>
        /// Скидка продавца
        /// </summary>
        public int? DiscountPercent { get; set; }

        /// <summary>
        /// Скидка WB
        /// </summary>
        public double? Spp { get; set; }

        /// <summary>
        /// Цена с учетом всех скидок, кроме суммы по WB Кошельку
        /// </summary>
        public double? FinishedPrice { get; set; }

        /// <summary>
        /// Цена со скидкой продавца (= totalPrice * (1 - discountPercent/100))
        /// </summary>
        public double? PriceWithDisc { get; set; }

        /// <summary>
        /// Отмена заказа. true - заказ отменен
        /// </summary>
        public bool IsCancel { get; set; }

        /// <summary>
        /// Дата и время отмены заказа. Если заказ не был отменен, то "0001-01-01T00:00:00".
        /// Если часовой пояс не указан, то используется Московское время UTC+3.
        /// </summary>
        public DateTime? CancelDate
        {
            get => cancelDate; set
            {
                cancelDate = value == new DateTime(0001, 01, 01) ? null : value;
            }
        }

        /// <summary>
        /// Тип заказа. Может быть "Клиентский", "Возврат Брака", "Принудительный возврат", "Возврат обезлички", "Возврат Неверного Вложения", "Возврат Продавца"
        /// </summary>
        public string? OrderType { get; set; }

        /// <summary>
        /// Идентификатор стикера
        /// </summary>
        public string? Sticker { get; set; }

        /// <summary>
        /// Номер заказа
        /// </summary>
        public string? GNumber { get; set; }

        /// <summary>
        /// Уникальный идентификатор заказа (srid равен rid в ответах методов сборочных заданий)
        /// </summary>
        public string? Srid { get; set; }

        /// <summary>
        /// Заказано
        /// </summary>
        public bool IsOrdered => !string.IsNullOrEmpty(Srid);

        /// <summary>
        /// Отправлено
        /// </summary>
        public bool IsDispatch => IsOrdered && !IsCancel ? true : false;

        /// <summary>
        ///  Итоговая цена
        /// </summary>
        public double? TotalPriceDiscount => TotalPrice - (TotalPrice * DiscountPercent / 100);

        /// <summary>
        /// Индификатор магазина
        /// </summary>
        public int? ProjectId { get; set; }
    }
}
