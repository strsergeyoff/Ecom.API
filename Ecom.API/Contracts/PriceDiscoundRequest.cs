using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Ecom.API.Contracts
{

    public record TaskCreated(Data data, bool error, string errorText);

    public record Data(int id, bool alreadyExists);

    public class PriceDiscoundRequest
    {
        /// <summary>
        /// Артикул товара
        /// </summary>
        /// <example>123</example>
        [Required]
        public long NmId { get; set; }

        /// <summary>
        /// Цена товара
        /// </summary>
        /// <example>null</example>
        public decimal? Price { get; set; }

        /// <summary>
        /// Скидка товара
        /// </summary>
        /// <example>30</example>
        public decimal? Discount { get; set; }
    }
}
