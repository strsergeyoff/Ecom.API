namespace Ecom.API.Models
{
    /// <summary>
    /// Склад
    /// </summary>
    public class Stock
    {
        /// <summary>
        /// Индификатор склада
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Дата и время обновления информации в сервисе.
        /// Это поле соответствует параметру dateFrom в запросе.
        /// Если часовой пояс не указан, то берется Московское время (UTC+3)
        /// </summary>
        public DateTime? LastChangeDate { get; set; }

        /// <summary>
        /// Название склада
        /// </summary>
        public string? WarehouseName { get; set; }

        /// <summary>
        /// Артикул продавца
        /// </summary>
        public string? SupplierArticle { get; set; }

        /// <summary>
        /// Артикул WB
        /// </summary>
        public int? NmId { get; set; }

        /// <summary>
        /// Баркод
        /// </summary>
        public string? Barcode { get; set; }

        /// <summary>
        /// Количество, доступное для продажи (сколько можно добавить в корзину)
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// В пути к клиенту
        /// </summary>
        public int? InWayToClient { get; set; }

        /// <summary>
        /// В пути от клиента
        /// </summary>
        public int? InWayFromClient { get; set; }

        /// <summary>
        /// Полное (непроданное) количество, которое числится за складом (= quantity + в пути)
        /// </summary>
        public int? QuantityFull { get; set; }

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
        /// Размер
        /// </summary>
        public string? TechSize { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// Скидка
        /// </summary>
        public decimal? Discount { get; set; }

        /// <summary>
        /// Договор поставки (внутренние технологические данные)
        /// </summary>
        public bool? IsSupply { get; set; }

        /// <summary>
        /// Договор реализации (внутренние технологические данные)
        /// </summary>
        public bool? IsRealization { get; set; }

        /// <summary>
        /// Код контракта (внутренние технологические данные)
        /// </summary>
        public string? SCCode { get; set; }

        /// <summary>
        /// Магазин
        /// </summary>
        public int ProjectId { get; set; }
    }
}
