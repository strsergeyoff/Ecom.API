namespace Ecom.API.Models
{
    /// <summary>
    /// Рекламная кампания
    /// </summary>
    public class rise_advert
    {
        private DateTime? endTime;

        /// <summary>
        /// Индификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Номер рекламной кампании
        /// </summary>
        public int AdvertId { get; set; }

        /// <summary>
        /// Наименование рекламной кампании
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Дата создания кампании
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Дата завершения кампании
        /// </summary>
        public DateTime? EndTime
        {
            get => endTime;
            set
            {
                if (value.Value.ToString("yyyy-MM-dd") == "2100-01-01")
                    endTime = null;
                else
                    endTime = value;
            }
        }

        /// <summary>
        /// Статус рекламной кампании
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Строковый статус рекламной кампании
        /// </summary>
        public string StatusString => Status == 7 ? "Кампания завершена" :
            (Status == 9 ? "Идут показы" : "Кампания на паузе");

        /// <summary>
        /// Статистика рекламной кампании по дням
        /// </summary>
        public virtual List<rise_advertstatistic> AdvertsStatistics { get; set; }

        /// <summary>
        /// Магазин
        /// </summary>
        public int ProjectId { get; set; }

        #region Не нужные свойства

        ///// <summary>
        ///// Показатель кликабельности, отношение числа кликов к количеству показов, %
        ///// </summary>
        //public double? Ctr => AdvertsStatistics.Sum(x => x.Clicks).Value == 0 || AdvertsStatistics.Sum(x => x.Views).Value == 0 ? 0 : ((double)AdvertsStatistics.Sum(x => x.Clicks).Value / (double)AdvertsStatistics.Sum(x => x.Views).Value) * 100;

        ///// <summary>
        ///// Средняя стоимость клика, ₽
        ///// </summary>
        //public decimal? Cpc => AdvertsStatistics.Average(x => x.Cpc);

        ///// <summary>
        ///// Затраты, ₽
        ///// </summary>
        //public decimal? Sum => AdvertsStatistics.Sum(x => x.Sum);

        ///// <summary>
        ///// Количество добавлений товаров в корзину
        ///// </summary>
        //public int? Atbs => AdvertsStatistics.Sum(x => x.Atbs);

        ///// <summary>
        ///// Количество заказов
        ///// </summary>
        //public int? Orders => AdvertsStatistics.Sum(x => x.Orders);

        ///// <summary>
        ///// CR(conversion rate) — отношение количества заказов к общему количеству посещений кампании
        ///// </summary>
        //public double? Cr => AdvertsStatistics.Sum(x => x.Orders).Value == 0 || AdvertsStatistics.Sum(x => x.Clicks).Value == 0 ? 0 : ((double)AdvertsStatistics.Sum(x => x.Orders).Value / (double)AdvertsStatistics.Sum(x => x.Clicks).Value) * 100;

        ///// <summary>
        ///// Количество заказанных товаров, шт
        ///// </summary>
        //public int? Shks => AdvertsStatistics.Sum(x => x.Shks);

        ///// <summary>
        ///// Заказов на сумму, ₽
        ///// </summary>
        //public decimal? Sum_price => AdvertsStatistics.Sum(x => x.Sum_price);
        #endregion
    }
}
