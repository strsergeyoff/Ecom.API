using Ecom.API.Models;

namespace Ecom.API.Services
{
    public interface IDataRepository
    {
        /// <summary>
        /// Поставки
        /// </summary>
        /// <returns></returns>
        Task LoadIncomes(int? id = null);

        /// <summary>
        /// Склад
        /// </summary>
        /// <returns></returns>
        Task LoadStocks(int? id = null);

        /// <summary>
        /// Заказы
        /// </summary>
        /// <returns></returns>
        Task LoadOrders(int? id = null);


        /// <summary>
        /// Подробный отчет
        /// </summary>
        /// <returns></returns>
        Task LoadReportDetails(int? id = null);

        /// <summary>
        /// Юнит
        /// </summary>
        /// <returns></returns>
        Task LoadUnits(int? id = null);

        /// <summary>
        /// Карточки Wildberries
        /// </summary>
        /// <returns></returns>
        Task CardsWildberries(int? id = null);

        /// <summary>
        /// Конкуренты
        /// </summary>
        /// <returns></returns>
        Task LoadCompetitors();

        Task LoadStore(int id);

        /// <summary>
        /// Рекламные кампании
        /// </summary>
        /// <returns></returns>
        Task LoadAdverts(int? id = null);
    }
}
 