namespace Ecom.API.Models
{
    /// <summary>
    /// Карточка Wildberries
    /// </summary>
    public class Card
    {
        /// <summary>
        /// Индификатор товара
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Артикул WB
        /// </summary>
        public long? NmID { get; set; }

        /// <summary>
        /// Артикул продавца
        /// </summary>
        public string? VendorCode { get; set; }

        /// <summary>
        /// Название предмета
        /// </summary>
        public string? SubjectName { get; set; }

        /// <summary>
        /// Бренд
        /// </summary>
        public string? Brand { get; set; }

        /// <summary>
        /// Ссылки на фотографии
        /// </summary>
        public virtual ICollection<CardPhoto>? Photos { get; set; }

        /// <summary>
        /// Ссылка на видео
        /// </summary>
        public string? Video { get; set; }

        /// <summary>
        /// Размеры
        /// </summary>
        public virtual ICollection<CardSize>? Sizes { get; set; }

        /// <summary>
        /// Магазин
        /// </summary>
        public int ProjectId { get; set; }
    }
}
