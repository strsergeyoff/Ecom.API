using Ecom.API.Models;

namespace Ecom.API.Services
{
    public interface IDataRepository
    {
        /// <summary>
        /// Поставки
        /// </summary>
        /// <returns></returns>
        Task LoadIncomes();

        /// <summary>
        /// Склад
        /// </summary>
        /// <returns></returns>
        Task LoadStocks();

        /// <summary>
        /// Заказы
        /// </summary>
        /// <returns></returns>
        Task LoadOrders();

        /// <summary>
        /// Продажи
        /// </summary>
        /// <returns></returns>
        Task LoadSales();

        /// <summary>
        /// Подробный отчет
        /// </summary>
        /// <returns></returns>
        Task LoadReportDetails();

        /// <summary>
        /// Юнит
        /// </summary>
        /// <returns></returns>
        Task LoadUnits();

        /// <summary>
        /// Карточки Wildberries
        /// </summary>
        /// <returns></returns>
        Task CardsWildberries();

        /// <summary>
        /// Конкуренты
        /// </summary>
        /// <returns></returns>
        Task LoadCompetitors();

        /// <summary>
        /// Рекламные кампании
        /// </summary>
        /// <returns></returns>
        Task LoadAdverts();
    }
}
 