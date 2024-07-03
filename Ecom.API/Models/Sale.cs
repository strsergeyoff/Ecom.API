namespace Ecom.API.Models
{
    /// <summary>
    /// Продажа
    /// </summary>
    public class Sale
    {
        /// <summary>
        /// Индификатор продажи
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Дата поступления.
        /// Если часовой пояс не указан, то берется Московское время UTC+3.
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Дата и время обновления информации в сервисе.
        /// Это поле соответствует параметру dateFrom в запросе. 
        /// Если часовой пояс не указан, то берется Московское время UTC+3.
        /// </summary>
        public DateTime? LastChangeDate { get; set; }

        /// <summary>
        /// Название склада
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
        public long? IncomeID { get; set; }

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
        public double? DiscountPercent { get; set; }

        /// <summary>
        /// Скидка WB
        /// </summary>
        public int? Spp { get; set; }

        /// <summary>
        /// Оплачено с WB Кошелька
        /// </summary>
        public int? PaymentSaleAmount { get; set; }

        /// <summary>
        /// К перечислению продавцу
        /// </summary>
        public double? ForPay { get; set; }

        /// <summary>
        /// Цена с учетом всех скидок, кроме суммы по WB Кошельку
        /// </summary>
        public double? FinishedPrice { get; set; }

        /// <summary>
        /// Цена со скидкой продавца (= totalPrice * (1 - discountPercent/100))
        /// </summary>
        public double? PriceWithDisc { get; set; }

        /// <summary>
        /// Уникальный идентификатор продажи/возврата
        /// S********** — продажа
        /// R********** — возврат(на склад WB)
        /// </summary>
        public string? SaleID { get; set; }

        /// <summary>
        /// Тип заказа:
        /// Клиентский — заказ, поступивший от покупателя
        /// Возврат Брака — возврат товара продавцу
        /// Принудительный возврат — возврат товара продавцу
        /// Возврат обезлички — возврат товара продавцу
        /// Возврат Неверного Вложения — возврат товара продавцу
        /// Возврат Продавца — возврат товара продавцу
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
        /// Уникальный идентификатор заказа.
        /// Примечание для использующих API Маркетплейс: srid равен rid в ответах методов сборочных заданий.
        /// </summary>
        public string? Srid { get; set; }

        /// <summary>
        /// Магазин
        /// </summary>
        public int ProjectId { get; set; }
    }
}
