using Ecom.API.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text;
namespace Ecom.API.Controllers
{


    [Tags("Установка цен и скидок")]
    public class PriceDisocuntController : Controller
    {

        /// <summary>
        /// Установить цены и скидки.
        /// </summary>
        /// <remarks>
        /// Товары, цены и скидки для них. Максимум 1 000 товаров. Цена и скидка не могут быть пустыми одновременно.
        /// Если новая цена со скидкой будет хотя бы в 3 раза меньше старой, она попадёт в карантин и товар будет продаваться по старой цене.
        /// Максимум — 10 запросов за 6 секунд.
        /// </remarks>
        /// <response code="200">Успешно</response>
        /// <response code="208">Такая загрузка уже есть</response>
        /// <response code="400">Неправильный запрос</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="429">Слишком много запросов</response>
        /// <response code="500">Внутренняя ошибка сервиса</response>
        /// <param name="upload">Объект с информацией о ценах и скидках для товаров с токеном магазина</param>
        
        [HttpPost("/upload/task")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TaskCreated), 200)]
        [ProducesResponseType(typeof(AddResponseExamples), 208)]
        [ProducesResponseType(typeof(AddResponseExamples), 400)]
        [ProducesResponseType(typeof(AddResponseExamples), 401)]
        [ProducesResponseType(typeof(AddResponseExamples), 429)]
        public async Task<IActionResult> UploadTask([Required][FromBody] UploadPriceDisocuntRequest upload)
        {
            string apiUrl = "https://discounts-prices-api.wildberries.ru/api/v2/upload/task";

            var source = new
            {
                data = upload.PriceDiscounds.Select(x => new
                {
                    nmID = x.NmId,
                    price = x.Price,
                    discount = x.Discount
                }).ToArray()
            };

            var js = System.Text.Json.JsonSerializer.Serialize(source);
            var jsonContent = new StringContent(js, Encoding.UTF8, "application/json");


            using (HttpClient httpClient = new HttpClient())
            {
                var hdr = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                hdr.Headers.Add("contentType", "application/json");
                hdr.Content = jsonContent;
                hdr.Headers.Add("Authorization", upload.Token);

                HttpResponseMessage response = await httpClient.SendAsync(hdr);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    return Json(responseData);
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    return BadRequest(Json(errorMessage));
                }
            }

        }
    }
}