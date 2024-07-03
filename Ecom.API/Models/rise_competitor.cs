namespace Ecom.API.Models
{
    /// <summary>
    /// Конкуренты
    /// </summary>
    public class rise_competitor
    {
        /// <summary>
        /// Индификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Артикул товара
        /// </summary>
        public long nmId { get; set; }

        /// <summary>
        /// Категория
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Брэнд
        /// </summary>
        public string? Brand { get; set; }

        /// <summary>
        /// Коллекция фотографий
        /// </summary>
        public virtual ICollection<rise_competitorphoto>? Photos { get; set; } = new List<rise_competitorphoto>();

        /// <summary>
        /// Статистика
        /// </summary>
        public virtual ICollection<rise_competitorstatistic>? Statistics { get; set; } = new List<rise_competitorstatistic>();

        /// <summary>
        /// Магазин
        /// </summary>
        public int ProjectId { get; set; }
    }
}
