using Azure;
using Google.Protobuf.WellKnownTypes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class AddResponseExamples : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {

        // Добавляем пример для ответа 208
        if (operation.Responses.ContainsKey("208"))
        {
            var responseExample = new OpenApiExample
            {
                Value = new OpenApiObject
                {
                    ["data"] = new OpenApiObject
                    {
                        ["id"] = new OpenApiInteger(1111111),
                        ["alreadyExists"] = new OpenApiBoolean(true)
                    },
                    ["error"] = new OpenApiBoolean(false),
                    ["errorText"] = new OpenApiString("Task already exists")
                },
                Description = "Задача уже существует"
            };

            operation.Responses["208"].Content["application/json"].Examples.Add("TaskAlreadyExistsError", responseExample);
        }

        // Добавляем пример для успешного ответа 400
        if (operation.Responses.ContainsKey("400"))
        {
            var responseExample = new OpenApiExample
            {
                Value = new OpenApiObject
                {
                    ["data"] = new OpenApiNull(), // Если data не должно содержать значения, используйте OpenApiNull
                    ["error"] = new OpenApiBoolean(false),
                    ["errorText"] = new OpenApiString("Goods number limit exceeded")
                },
                Description = "В запросе слишком много товаров"
            };

            var DuplicatedNmIdInTask = new OpenApiExample
            {
                Value = new OpenApiObject
                {
                    ["data"] = new OpenApiNull(), // Если data не должно содержать значения, используйте OpenApiNull
                    ["error"] = new OpenApiBoolean(true),
                    ["errorText"] = new OpenApiString("Duplicated nm_id in task")
                },
                Description = "В запросе несколько одинаковых nmId"
            };

            operation.Responses["400"].Content["application/json"].Examples.Add("DuplicatedNmIdInTask", DuplicatedNmIdInTask);

            var NoGoodsForProcess = new OpenApiExample
            {
                Value = new OpenApiObject
                {
                    ["data"] = new OpenApiNull(), // Если data не должно содержать значения, используйте OpenApiNull
                    ["error"] = new OpenApiBoolean(true),
                    ["errorText"] = new OpenApiString("No goods for process")
                },
                Description = "Цены и скидки такие же, как сейчас"
            };

            operation.Responses["400"].Content["application/json"].Examples.Add("NoGoodsForProcess", NoGoodsForProcess);

            var FailedToParseData = new OpenApiExample
            {
                Value = new OpenApiObject
                {
                    ["data"] = new OpenApiNull(), // Если data не должно содержать значения, используйте OpenApiNull
                    ["error"] = new OpenApiBoolean(true),
                    ["errorText"] = new OpenApiString("Failed to parse data")
                },
                Description = "Не удалось обработать данные, проверьте, что запрос правильный"
            };

            operation.Responses["400"].Content["application/json"].Examples.Add("FailedToParseData", FailedToParseData);

            var PriceHasDecimalPlaces = new OpenApiExample
            {
                Value = new OpenApiObject
                {
                    ["data"] = new OpenApiNull(), // Если data не должно содержать значения, используйте OpenApiNull
                    ["error"] = new OpenApiBoolean(true),
                    ["errorText"] = new OpenApiString("Price has decimal places")
                },
                Description = "У цены дробное значение, исправьте на целое"
            };

            operation.Responses["400"].Content["application/json"].Examples.Add("PriceHasDecimalPlaces", PriceHasDecimalPlaces);

            var WrongPriceValue = new OpenApiExample
            {
                Value = new OpenApiObject
                {
                    ["data"] = new OpenApiNull(), // Если data не должно содержать значения, используйте OpenApiNull
                    ["error"] = new OpenApiBoolean(true),
                    ["errorText"] = new OpenApiString("Wrong price value")
                },
                Description = "Некорректная цена"
            };

            operation.Responses["400"].Content["application/json"].Examples.Add("WrongPriceValue", WrongPriceValue);

            var WrongNmValue = new OpenApiExample
            {
                Value = new OpenApiObject
                {
                    ["data"] = new OpenApiNull(), // Если data не должно содержать значения, используйте OpenApiNull
                    ["error"] = new OpenApiBoolean(true),
                    ["errorText"] = new OpenApiString("Wrong nm value")
                },
                Description = "Неправильный nmId"
            };

            operation.Responses["400"].Content["application/json"].Examples.Add("WrongNmValue", WrongNmValue);

            var PriceNotSet = new OpenApiExample
            {
                Value = new OpenApiObject
                {
                    ["data"] = new OpenApiNull(), // Если data не должно содержать значения, используйте OpenApiNull
                    ["error"] = new OpenApiBoolean(true),
                    ["errorText"] = new OpenApiString("Both discount and price are not set")
                },
                Description = "Цена не установлена"
            };

            operation.Responses["400"].Content["application/json"].Examples.Add("PriceNotSet", PriceNotSet);

            var EmptyData = new OpenApiExample
            {
                Value = new OpenApiObject
                {
                    ["data"] = new OpenApiNull(), // Если data не должно содержать значения, используйте OpenApiNull
                    ["error"] = new OpenApiBoolean(true),
                    ["errorText"] = new OpenApiString("Empty data")
                },
                Description = "Нет данных"
            };

            operation.Responses["400"].Content["application/json"].Examples.Add("EmptyData", EmptyData);

            var NoValidGoodsInTask = new OpenApiExample
            {
                Value = new OpenApiObject
                {
                    ["data"] = new OpenApiNull(), // Если data не должно содержать значения, используйте OpenApiNull
                    ["error"] = new OpenApiBoolean(true),
                    ["errorText"] = new OpenApiString("No valid goods in task")
                },
                Description = "Таких товаров или размеров нет (например, их удалили)"
            };

            operation.Responses["400"].Content["application/json"].Examples.Add("NoValidGoodsInTask", NoValidGoodsInTask);

        }

        // Добавляем пример для успешного ответа 401
        if (operation.Responses.ContainsKey("401"))
        {
            var responseExample = new OpenApiExample
            {
                Value = new OpenApiObject
                {
                    ["title"] = new OpenApiString("unauthorized"),
                    ["detail"] = new OpenApiString("token problem; token is malformed: could not base64 decode signature: illegal base64 data at input byte 84"),
                    ["code"] = new OpenApiString("07e4668e--a53a3d31f8b0-[UK-oWaVDUqNrKG]; 03bce=277; 84bd353bf-75"),
                    ["requestId"] = new OpenApiString("7b80742415072fe8b6b7f7761f1d1211"),
                    ["origin"] = new OpenApiString("s2sauth-ca"),
                    ["status"] = new OpenApiInteger(401),
                    ["statusText"] = new OpenApiString("Unauthorized")
                }
            };

            operation.Responses["401"].Content["application/json"].Examples.Add("Unauthorized", responseExample);
        }

        // Добавляем пример для успешного ответа 429
        if (operation.Responses.ContainsKey("429"))
        {
            var responseExample = new OpenApiExample
            {
                Value = new OpenApiObject
                {
                    ["title"] = new OpenApiString("too many requests"),
                    ["detail"] = new OpenApiString("limited by c122a060-a7fb-4bb4-abb0-32fd4e18d489"),
                    ["code"] = new OpenApiString("07e4668e-ac2242c5c8c5-[UK-4dx7JUdskGZ]"),
                    ["requestId"] = new OpenApiString("9d3c02cc698f8b041c661a7c28bed293"),
                    ["origin"] = new OpenApiString("s2s-api-auth-stat"),
                    ["status"] = new OpenApiInteger(429),
                    ["statusText"] = new OpenApiString("Too Many Requests")
                }
            };

            operation.Responses["429"].Content["application/json"].Examples.Add("TooManyRequests", responseExample);
        }
    }
}