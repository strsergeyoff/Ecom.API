using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using Polly;
using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;

namespace Ecom.API.Services
{
    public class WbParser
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(10); // Ограничение на 10 параллельных запросов
        private static readonly int maxRetries = 3; // Максимальное количество попыток повторного запроса

        //Парсер артикулов
        public static async Task<List<int>> GetAllUniqueProductIds(string query)
        {
            var allData = await ParseData(query);
            return allData.Select(row => (int)row[1])
                          .Distinct()
                          .ToList();
        }
        public static async Task<List<object[]>> GetProductDataFromIds(List<int> productIds)
        {
            var allData = new List<object[]>();
            int step = 100;

            for (int x = 0; x < productIds.Count; x += step)
            {
                var batchIds = productIds.Skip(x).Take(step).ToList();
                var batchData = await FetchProductData(batchIds);
                allData.AddRange(batchData);
            }

            return allData;
        }

        private static async Task<List<object[]>> FetchProductData(List<int> batchIds)
        {
            var productData = new List<object[]>();
            var idsString = string.Join(";", batchIds);
            var url = $"https://card.wb.ru/cards/detail?nm={idsString}&dest=-1216601,-115136,-421732,123585595";

            var response = await GetHttpResponseWithRetries(url);
            if (string.IsNullOrWhiteSpace(response))
            {
                throw new ArgumentNullException(nameof(response), "Response from server is null or empty.");
            }

            var jsonResponse = JObject.Parse(response);
            var data = jsonResponse["data"]?["products"];

            if (data != null)
            {
                foreach (var row in data)
                {
                    var id = (int?)row["id"] ?? 0;
                    var name = (string)row["name"] ?? string.Empty;
                    var brand = (string)row["brand"] ?? string.Empty;
                    var brandId = (int?)row["brandId"] ?? 0;
                    var priceU = (decimal?)row["priceU"] / 100 ?? 0;
                    var sale = (int?)row["sale"] ?? 0;
                    var salePriceU = (decimal?)row["salePriceU"] / 100 ?? 0;
                    var pics = (string)row["pics"] ?? string.Empty;
                    var rating = (decimal?)row["rating"] ?? 0;
                    var feedbacks = (int?)row["feedbacks"] ?? 0;
                    var volume = (int?)row["volume"] ?? 0;
                    var colors = GetColorsString(row["colors"]);
                    var root = (string)row["root"] ?? string.Empty;

                    var sizes = row["sizes"];
                    if (sizes == null)
                    {
                        continue;
                    }

                    foreach (var size in sizes)
                    {
                        var sizeName = (string)size["name"] ?? string.Empty;
                        var sizeOrigName = (string)size["origName"] ?? string.Empty;
                        var stocks = GetStocksString(size["stocks"]);

                        int totalQty = 0;
                        foreach (var stock in size["stocks"])
                        {
                            totalQty += (int?)stock["qty"] ?? 0;
                        }

                        var productRow = new object[]
                        {
                            id, name, brand, brandId, priceU, sale, salePriceU, pics, rating, feedbacks,
                            volume, colors, sizeName, sizeOrigName, root, stocks, totalQty
                        };
                        productData.Add(productRow);
                    }
                }
            }

            return productData;
        }

        private static string GetColorsString(JToken colors)
        {
            if (colors == null) return string.Empty;
            return string.Join(", ", colors?.Select(c => (string)c["name"]) ?? Enumerable.Empty<string>());
        }

        private static string GetStocksString(JToken stocks)
        {
            if (stocks == null) return string.Empty;
            return string.Join(", ", stocks?.Select(s => JsonConvert.SerializeObject(s)) ?? Enumerable.Empty<string>());
        }
        public static async Task WriteArtikulsToExcel(List<object[]> productData)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var wsArticul = package.Workbook.Worksheets.Add("ArticulData");
                var articulHeaders = new[]
                {
            "ID", "Name", "Brand", "BrandID", "Price", "Sale%", "SalePrice", "Pics", "Rating", "Feedbacks",
            "Volume", "Colors", "SizeName", "SizeOrigName", "Root", "Stocks", "Quantity"
        };

                for (int i = 0; i < articulHeaders.Length; i++)
                {
                    wsArticul.Cells[1, i + 1].Value = articulHeaders[i];
                }

                for (int row = 0; row < productData.Count; row++)
                {
                    var dataRow = productData[row];
                    for (int col = 0; col < dataRow.Length; col++)
                    {
                        wsArticul.Cells[row + 2, col + 1].Value = dataRow[col];
                    }
                }

                var filePath = $"Парсер_артикулов_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx";
                package.SaveAs(new FileInfo(filePath));
                Console.WriteLine($"Data saved to {filePath}");
            }
        }

        //Парсер выдачи
        private static async Task<int> GetTotalPages(string query, int maxItems)
        {
            var url = $"https://search.wb.ru/exactmatch/ru/common/v4/search?page=1&appType=1&curr=rub&dest=-1257786&lang=ru&locale=ru&query={query}&resultset=catalog&showAll=true";

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonData = await response.Content.ReadAsStringAsync();
            var data = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(jsonData);

            int totalItems = data.GetProperty("data").GetProperty("total").GetInt32();
            return Math.Min(totalItems, maxItems) / 100 + 1;
        }

        public static async Task<List<object[]>> ParseData(string query)
        {
            var allData = new ConcurrentBag<object[]>();
            var tasks = new List<Task>();

            // Определяем максимальное количество товаров в зависимости от количества слов в запросе
            int wordCount = query.Split(' ').Length;
            int maxItems = wordCount switch
            {
                >= 5 => 1000,
                >= 4 => 5000,
                >= 3 => 50000,
                _ => 100000
            };

            int totalPages = await GetTotalPages(query, maxItems); // получаем общее количество страниц

            int batchSize = 100;
            var semaphoreSlim = new SemaphoreSlim(10, 10);

            int batchCount = (totalPages + batchSize - 1) / batchSize; // количество батчей

            for (int batchIndex = 0; batchIndex < batchCount; batchIndex++)
            {
                int startPage = batchIndex * batchSize + 1;
                int endPage = Math.Min(startPage + batchSize - 1, totalPages);
                var batchPages = Enumerable.Range(startPage, endPage - startPage + 1).ToList();

                await semaphoreSlim.WaitAsync();

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await FetchBatchData(query, batchPages, allData);
                    }
                    finally
                    {
                        semaphoreSlim.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);

            return allData.ToList();
        }

        private static async Task FetchBatchData(string query, List<int> pages, ConcurrentBag<object[]> allData)
        {
            foreach (var page in pages)
            {
                await FetchPageData(query, page, allData);
            }
        }

        private static async Task FetchPageData(string query, int page, ConcurrentBag<object[]> allData)
        {
            try
            {
                var url = $"https://search.wb.ru/exactmatch/ru/common/v4/search?page={page}&appType=1&curr=rub&dest=-1257786&lang=ru&locale=ru&query={query}&resultset=catalog&showAll=true";
                Console.WriteLine($"Fetching page {page}: {url}");

                var response = await GetHttpResponseWithRetries(url);
                var jsonResponse = JObject.Parse(response);
                var data = jsonResponse["data"]?["products"];

                if (data != null)
                {
                    var pageData = data.Select(row => new object[]
                    {
                $"https://www.wildberries.ru/catalog/{row["id"]}/detail.aspx",
                (int?)row["id"] ?? 0,
                (string)row["root"] ?? string.Empty,
                (int?)row["kindId"] ?? 0,
                (int?)row["subjectId"] ?? 0,
                (int?)row["subjectParentId"] ?? 0,
                (string)row["name"] ?? string.Empty,
                (string)row["brand"] ?? string.Empty,
                (int?)row["brandId"] ?? 0,
                (int?)row["siteBrandId"] ?? 0,
                (int?)row["supplierId"] ?? 0,
                (int?)row["sale"] ?? 0,
                (decimal?)row["priceU"] / 100 ?? 0,
                (decimal?)row["salePriceU"] / 100 ?? 0,
                (int?)row["logisticsCost"] ?? 0,
                (string)row["saleConditions"] ?? string.Empty,
                (string)row["pics"] ?? string.Empty,
                (decimal?)row["rating"] ?? 0,
                (int?)row["feedbacks"] ?? 0,
                (int?)row["panelPromoId"] ?? 0,
                (string)row["promoTextCat"] ?? string.Empty,
                (int?)row["volume"] ?? 0,
                (string)row["stocks"] ?? string.Empty,
                (int?)row["q"] ?? 0,
                row["colors"]?.ToString() ?? string.Empty,
                row["sizes"]?.ToString() ?? string.Empty
                    }).ToArray();

                    foreach (var rowData in pageData)
                    {
                        allData.Add(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching page {page}: {ex.Message}");
            }
        }

        private static async Task<string> GetHttpResponseWithRetries(string url)
        {
            int attempts = 0;
            while (attempts < maxRetries)
            {
                try
                {
                    return await httpClient.GetStringAsync(url);
                }
                catch (HttpRequestException ex)
                {
                    attempts++;
                    Console.WriteLine($"Error fetching {url}, attempt {attempts}: {ex.Message}");
                    if (attempts >= maxRetries)
                    {
                        throw;
                    }

                    await Task.Delay(1000 * attempts); // Экспоненциальная задержка перед повтором
                }
            }

            return null;
        }

        public static void SaveToExcel(List<object[]> data)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Wildberries Data");
                var headers = new[]
                {
                "URL", "ID", "Root", "Kind ID", "Subject ID", "Subject Parent ID",
            "Name", "Brand", "Brand ID", "Site Brand ID", "Supplier ID",
            "Sale", "Price (RUB)", "Sale Price (RUB)", "Logistics Cost",
            "Sale Conditions", "Pics", "Rating", "Feedbacks", "Panel Promo ID",
            "Promo Text Cat", "Volume", "Stocks", "Q", "Promo Text Cat",
            "Colors", "Sizes"
            };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }
                // Добавление данных
                for (int i = 0; i < data.Count; i++)
                {
                    for (int j = 0; j < data[i].Length; j++)
                    {
                        worksheet.Cells[i + 2, j + 1].Value = data[i][j];
                    }
                }

                //// Выбор места сохранения файла пользователем
                //var saveFileDialog = new SaveFileDialog
                //{
                //    Filter = "Excel Files|*.xlsx",
                //    Title = "Save an Excel File",
                //    FileName = $"Парсер_выдачи_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}"
                //};
                //if (saveFileDialog.ShowDialog() == true)
                //{
                //    var tempFilePath = Path.GetTempFileName();
                //    using (var stream = new FileStream(tempFilePath, FileMode.Create))
                //    {
                //        package.SaveAs(stream);
                //    }

                //    File.Copy(tempFilePath, saveFileDialog.FileName, true);
                //    File.Delete(tempFilePath);
                //}
            }
        }

        //Парсер позиций
        public static async Task<Dictionary<int, string>> ReadSearchQueriesFromExcelAsync(string filePath)
        {
            var searchQueries = new Dictionary<int, string>();
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int row = 2;
                while (true)
                {
                    var cell = worksheet.Cells[row, 1];
                    if (cell == null || cell.Value == null || string.IsNullOrWhiteSpace(cell.Text))
                    {
                        break;
                    }
                    searchQueries.Add(row, cell.Text.Trim());
                    row++;
                }
            }
            return searchQueries;
        }

        public static async Task<Dictionary<int, string>> SearchDataAsync(string artikul, Dictionary<int, string> searchQueries)
        {
            var results = new ConcurrentDictionary<int, string>();
            var httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            using var httpClient = new HttpClient(httpClientHandler)
            {
                Timeout = TimeSpan.FromMinutes(5)  // Увеличение таймаута
            };

            int maxPage = 1000;
            var tasks = new List<Task>();
            var semaphore = new SemaphoreSlim(10);  // Ограничение параллельных задач до 10
            int moscowRegionId = 77;
            foreach (var query in searchQueries)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        int rowIndex = query.Key;
                        string searchQuery = query.Value;
                        bool found = false;

                        for (int page = 1; page <= maxPage; page++)
                        {
                            try
                            {
                                string url = $"https://search.wb.ru/exactmatch/ru/common/v4/search?page={page}&appType=1&curr=rub&dest=-1257786&lang=ru&locale=ru&query={searchQuery}&resultset=catalog&regionId={moscowRegionId}";
                                var response = await Policy
                                    .Handle<HttpRequestException>()
                                    .Or<TaskCanceledException>()
                                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                                    .ExecuteAsync(() => httpClient.GetStringAsync(url));

                                var products = JObject.Parse(response)["data"]["products"];
                                for (int p = 0; p < products.Count(); p++)
                                {
                                    if (products[p]["id"].ToString() == artikul)
                                    {
                                        results[rowIndex] = $"Page {page} - {p + 1}";
                                        found = true;
                                        break;
                                    }
                                }
                                if (found || products.Count() < 100)
                                {
                                    break;
                                }
                            }
                            catch (HttpRequestException ex)
                            {
                                results[rowIndex] = $"Ошибка HTTP-запроса: {ex.Message}";
                                break;
                            }
                            catch (TaskCanceledException ex)
                            {
                                results[rowIndex] = $"Запрос отменен (таймаут): {ex.Message}";
                                break;
                            }
                            catch (Exception ex)
                            {
                                results[rowIndex] = $"Неизвестная ошибка: {ex.Message}";
                                break;
                            }
                        }

                        if (!found && !results.ContainsKey(rowIndex))
                        {
                            results[rowIndex] = "Не найден";
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);
            return results.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public static async Task WriteQueriesToExcelAsync(string filePath, string artikul, Dictionary<int, string> searchQueries, Dictionary<int, string> results)
        {
            var file = new FileInfo(filePath);
            using (var package = new ExcelPackage(file))
            {
                var worksheetFound = package.Workbook.Worksheets.Add("Найденные запросы");
                var worksheetNotFound = package.Workbook.Worksheets.Add("Ненайденные запросы");
                var worksheetAll = package.Workbook.Worksheets.Add("Все запросы");

                // Заполнение листа "Все запросы"
                worksheetAll.Cells[1, 1].Value = "Артикул WB";
                worksheetAll.Cells[1, 2].Value = "Запрос";
                worksheetAll.Cells[1, 3].Value = "Дата и время";
                worksheetAll.Cells[1, 4].Value = "Результат";
                int rowIndexAll = 2;

                foreach (var query in searchQueries)
                {
                    worksheetAll.Cells[rowIndexAll, 1].Value = artikul;
                    worksheetAll.Cells[rowIndexAll, 2].Value = query.Value;
                    worksheetAll.Cells[rowIndexAll, 3].Value = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
                    worksheetAll.Cells[rowIndexAll, 4].Value = results.ContainsKey(query.Key) ? results[query.Key] : "Не найден";
                    rowIndexAll++;
                }

                // Заполнение листа "Найденные запросы"
                worksheetFound.Cells[1, 1].Value = "Артикул WB";
                worksheetFound.Cells[1, 2].Value = "Запрос";
                worksheetFound.Cells[1, 3].Value = "Дата и время";
                worksheetFound.Cells[1, 4].Value = "Результат";
                int rowIndexFound = 2;

                foreach (var query in searchQueries)
                {
                    if (results.ContainsKey(query.Key) && results[query.Key] != "Не найден")
                    {
                        worksheetFound.Cells[rowIndexFound, 1].Value = artikul;
                        worksheetFound.Cells[rowIndexFound, 2].Value = query.Value;
                        worksheetFound.Cells[rowIndexFound, 3].Value = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
                        worksheetFound.Cells[rowIndexFound, 4].Value = results[query.Key];
                        rowIndexFound++;
                    }
                }

                // Заполнение листа "Ненайденные запросы"
                worksheetNotFound.Cells[1, 1].Value = "Артикул WB";
                worksheetNotFound.Cells[1, 2].Value = "Запрос";
                worksheetNotFound.Cells[1, 3].Value = "Дата и время";
                worksheetNotFound.Cells[1, 4].Value = "Результат";
                int rowIndexNotFound = 2;

                foreach (var query in searchQueries)
                {
                    if (results.ContainsKey(query.Key) && results[query.Key] == "Не найден")
                    {
                        worksheetNotFound.Cells[rowIndexNotFound, 1].Value = artikul;
                        worksheetNotFound.Cells[rowIndexNotFound, 2].Value = query.Value;
                        worksheetNotFound.Cells[rowIndexNotFound, 3].Value = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
                        worksheetNotFound.Cells[rowIndexNotFound, 4].Value = results[query.Key];
                        rowIndexNotFound++;
                    }
                }

                await package.SaveAsync();
            }
        }
    }
    public class Product
    {
        public string id { get; set; }
    }
}
