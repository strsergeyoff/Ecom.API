using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Ecom.API.Contracts
{

    public class UploadPriceDisocuntRequest
    {
        /// <summary>
        /// Токен магазина
        /// </summary>
        /// <example>eyJhbGciOiJFUzI1NiIsImtpZCI6IjIwMjQwNjE3djEiLCJ0eXAiOiJKV1QifQ.eyJlbnQiOjEsImV4cCI6MTczNjM4NDQyNywiaWQiOiJkNmFhMDQ4MS01MGU4LTRmMTMtYWM4YS1mMTFiZDRiYjUxYjUiLCJpaWQiOjE3NjYzMTI3LCJvaWQiOjEyMDkwMjYsInMiOjMxOTgsInNpZCI6IjBjOGM4MjM5LWM3MzktNDc2Yy1iOGVmLWY1MjE3NGM0YWM3ZSIsInQiOmZhbHNlLCJ1aWQiOjE3NjYzMTI3fQ.y93ZHwt9XZ9fZtWmYPfjFLrs4FzzR6ASp1wr2MSu8jcykUZRHfITXemc183-kTn4aOyQX_X7jx9hEJQHMzmvyg</example>
        [Required]
        public string Token { get; set; }

        /// <summary>
        /// Список товаров с ценой и скидкой
        /// </summary>
        [Required]
        public PriceDiscoundRequest[] PriceDiscounds { get; set; }
    }
}
