using Ecom.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{
    public class ProjectController(IDataRepository dataRepository) : Controller
    {


        /// <summary>
        /// Загрузка нового магазина
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("/project")]
        public async Task<IActionResult> AddStoreAsync([FromQuery] int id)
        {
                await dataRepository.LoadStore(id);
           

            return Ok();
        }

        [HttpGet("/test")]
        public async Task<IActionResult> Test()
        {

            return Ok("erferferferf");
        }
    }
}