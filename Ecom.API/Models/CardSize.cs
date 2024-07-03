using Microsoft.AspNetCore.Identity;

namespace Ecom.API.Models
{
    /// <summary>
    /// Размеры товара
    /// </summary>
    public class CardSize
    {
        /// <summary>
        /// Индификатор размера
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Размер товара
        /// </summary>
        public string? TechSize { get; set; }

        public int CardId { get; set; }
    }
}