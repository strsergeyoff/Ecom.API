using Ecom.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Ecom.API.Controllers
{
    [Tags("Магазины")]
    public class StoreController(IDataRepository _dataRepository) : Controller
    {
        ///// <summary>
        ///// Обновление данных магазина.
        ///// </summary>
        ///// <remarks>
        ///// Добавление магазина в очередь на обновление данных.
        ///// </remarks>
        ///// <response code="200">Успешно</response>
        ///// <param name="StoreId">Индификатор магазина в системе</param>

        //[HttpGet("/upload/store")]
        //[Consumes("application/json")]
        //[Produces("application/json")]
        //public async Task<IActionResult> Upload([Required] int StoreId)
        //{
        //    await _dataRepository.LoadStore(StoreId);

        //    return Ok();
        //}
    }
}
