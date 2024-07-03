namespace Ecom.API.Models
{
    /// <summary>
    /// Статистика конкурента
    /// </summary>
    public class rise_competitorstatistic
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
        /// Цена
        /// </summary>
        public int SalePrice { get; set; }

        /// <summary>
        /// Остатки
        /// </summary>
        public int InStock { get; set; }

    }
}
