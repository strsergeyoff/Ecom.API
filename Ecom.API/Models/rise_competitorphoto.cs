namespace Ecom.API.Models
{
    /// <summary>
    /// Фотографии конкурента
    /// </summary>
    public class rise_competitorphoto
    {
        /// <summary>
        /// Индификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Изображение товара
        /// </summary>
        public string? Url { get; set; }
    }
}