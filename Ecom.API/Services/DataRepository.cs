﻿using Ecom.API.Models;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Z.BulkOperations;

namespace Ecom.API.Services
{
    public class DataRepository : IDataRepository
    {
        #region Поля и свойства

        private readonly string ConnectionMySQL = "Server=31.31.196.247;Database=u2693092_default;Uid=u2693092_default;Pwd=V2o0oyRuG8DKLl7F;Charset=utf8";
        private readonly string DateFrom = "2024-01-29";

        private const string WbSmallImageUrlTemplate = "https://basket-#id#.wb.ru/vol#count4#/part#count6#/#article#/images/tm/#number#.jpg";

        private readonly ApplicationDbContext _context;
        private readonly ITelegramBotClient _telegramBot;
        private readonly IHttpClientFactory _httpClientFactory;

        public Dictionary<int, List<string>> MessageIncomes { get; set; } = new Dictionary<int, List<string>>();
        public Dictionary<int, List<string>> MessageStocks { get; set; } = new Dictionary<int, List<string>>();
        public Dictionary<int, List<string>> MessageSales { get; set; } = new Dictionary<int, List<string>>();
        public Dictionary<int, List<string>> MessageCards { get; set; } = new Dictionary<int, List<string>>();
        public Dictionary<int, List<string>> MessageOrders { get; set; } = new Dictionary<int, List<string>>();
        public Dictionary<int, List<string>> MessageAdverts { get; set; } = new Dictionary<int, List<string>>();
        public Dictionary<int, List<string>> MessageReportDetails { get; set; } = new Dictionary<int, List<string>>();
        public Dictionary<int, List<string>> MessageUnits { get; set; } = new Dictionary<int, List<string>>();
        public Dictionary<int, List<string>> MessageCompetitors { get; set; } = new Dictionary<int, List<string>>();
        #endregion


        #region Конструктор по умолчанию

        public DataRepository(ApplicationDbContext context, ITelegramBotClient telegramBot, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _telegramBot = telegramBot;
            _httpClientFactory = httpClientFactory;

            _telegramBot.StartReceiving(HandleUpdateAsync, HandleErrorAsync);
        }
        #endregion


        #region Другое
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)

        {

            try
            {
                var callbackQuery = update?.CallbackQuery;



                switch (callbackQuery?.Data)
                {
                }


                // Проверяем, является ли обновление сообщением
                if (update.Type == UpdateType.Message)
                {
                    // Получаем объект сообщения
                    var message = update.Message;

                    // Получаем текст сообщения
                    var messageText = message.Text;

                    // Проверяем, является ли сообщение командой (начинается с '/')
                    if (messageText.StartsWith("/statistics"))
                    {
                        // Получаем время запуска приложения
                        DateTime startTime = Process.GetCurrentProcess().StartTime;

                        // Вычисляем время работы приложения
                        TimeSpan elapsedTime = DateTime.Now - startTime;

                        int hours = elapsedTime.Hours;
                        int minutes = elapsedTime.Minutes;
                        int seconds = elapsedTime.Seconds;

                        // Отправляем ответное сообщение
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: @$"Статистика работы сервера:
Время работы `{hours} ч {minutes} м. {seconds} с.`",
                            parseMode: ParseMode.Markdown);
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
        }

        private static List<string> SplitToPages(string longMessage, int pageSize)
        {
            List<string> pages = new List<string>();
            for (int i = 0; i < longMessage.Length; i += pageSize)
            {
                pages.Add(longMessage.Substring(i, Math.Min(pageSize, longMessage.Length - i)));
            }
            return pages;
        }
        #endregion

        #region Поставки

        /// <summary>
        /// Загрузка поставок
        /// </summary>
        /// <returns></returns>
        public async Task LoadIncomes()
        {
            int incomesCount = 0;
            int storesCount = 0;
            int error = 0;

            var stores = _context.rise_projects
                .Where(x => !string.IsNullOrWhiteSpace(x.Token)
                && x.Token.Length > 155
                && x.Deleted.Value == false)
                .ToList();


            var messageIncomes = await _telegramBot.SendTextMessageAsync("740755376", "Загрузка поставок",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

            Stopwatch _stopwatch = new Stopwatch();
            _stopwatch.Start();

            foreach (var store in stores)
            {

                try
                {
                    storesCount++;
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    DateTime? lastOrder = _context?.Incomes?.Where(x => x.ProjectId == store.Id)?.Max(x => x.LastChangeDate);
                    var incomes = await FetchIncomesFromApi(store, lastOrder);

                    incomesCount += incomes.Count;

                    if (incomes.Count > 0)
                        await BulkInsertEntitiesAsync(incomes, "Incomes");

                    stopwatch.Stop();

                    TimeSpan elapsed = stopwatch.Elapsed;

                    await InsertAndEditMessage(messageIncomes, MessageIncomes, @$"🏦 Магазин `{store.Title}`
🆕 Загружено строк `{incomes.Count} шт.`
⏱️ Время загрузки поставок `{elapsed.Hours} ч {elapsed.Minutes} м. {elapsed.Seconds} с.`");
                }
                catch (Exception ex)
                {
                    error++;

                    await InsertAndEditMessage(messageIncomes, MessageIncomes, @$"🏦 Магазин `{store.Title}`
```{ex.Message.ToString()}```");
                }
            }

            _stopwatch.Stop();

            TimeSpan _elapsed = _stopwatch.Elapsed;

            await InsertAndEditMessage(messageIncomes, MessageIncomes, $@"✅ Успешно: `{storesCount - error} из {storesCount}`
🆕 Загружено строк `{incomesCount} шт.`
⏱️ Потраченное время: `{_elapsed.Hours} ч {_elapsed.Minutes} м. {_elapsed.Seconds} с.`");

            MessageIncomes.Clear();
        }

        /// <summary>
        /// Список поставок
        /// </summary>
        /// <param name="store">Магазин</param>
        /// <param name="lastIncome">Последняя поставка</param>
        /// <returns></returns>
        public async Task<List<Income>> FetchIncomesFromApi(rise_project store, DateTime? lastIncome)
        {

            var incomes = new List<Income>();
            var httpClient = _httpClientFactory.CreateClient();

            string dateFrom = lastIncome?.ToString("yyyy-MM-ddTHH:mm:ss") ?? DateFrom;
            string apiUrlBase = "https://statistics-api.wildberries.ru/api/v1/supplier/incomes?dateFrom=";

            try
            {
                string apiUrl = apiUrlBase + dateFrom;
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                requestMessage.Headers.Add("contentType", "application/json");
                requestMessage.Headers.Add("Authorization", store.Token);

                HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var fetchedIcomes = JsonConvert.DeserializeObject<List<Income>>(responseContent);

                    fetchedIcomes = fetchedIcomes?.Where(x => lastIncome == null || x.Date > lastIncome)?.ToList();

                    foreach (var income in fetchedIcomes)
                        income.ProjectId = store.Id;

                    incomes.AddRange(fetchedIcomes);
                }

            }
            catch (HttpRequestException ex)
            {
                // Обработка ошибок связанных с HTTP-запросом
                return incomes;
            }
            catch (System.Text.Json.JsonException ex)
            {
                // Обработка ошибок десериализации JSON
                return incomes;
            }
            catch (Exception ex)
            {
                // Другие специфические исключения
                return incomes;
            }

            return incomes;
        }
        #endregion

        #region Склад

        /// <summary>
        /// Загрузка склада
        /// </summary>
        /// <returns></returns>
        public async Task LoadStocks()
        {
            int stocksCount = 0;
            int storesCount = 0;
            int error = 0;

            var stores = _context.rise_projects
                .Where(x => !string.IsNullOrWhiteSpace(x.Token)
                && x.Token.Length > 155
                && x.Deleted.Value == false)
                .ToList();

            var messageStocks = await _telegramBot.SendTextMessageAsync("740755376",
                "Загрузка склада",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

            Stopwatch _stopwatch = new Stopwatch();
            _stopwatch.Start();

            foreach (var store in stores)
            {
                storesCount++;

                try
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    List<Stock> Stocks = _context?.Stocks?.Where(x => x.ProjectId == store.Id).ToList();
                    List<Stock> stocks = await FetchStocksFromApi(store);

                    stocksCount += stocks.Count;

                    if (stocks.Count > 0)
                        await BulkInsertEntitiesAsync(stocks, "Stocks", Stocks);

                    stopwatch.Stop();
                    TimeSpan elapsed = stopwatch.Elapsed;

                    await InsertAndEditMessage(messageStocks, MessageStocks, @$"🏦 Магазин `{store.Title}`
🆕 Загружено строк `{stocks.Count} шт.`
⏱️ Время загрузки склада `{elapsed.Hours} ч {elapsed.Minutes} м. {elapsed.Seconds} с.`");
                }

                catch (Exception ex)
                {
                    error++;

                    await InsertAndEditMessage(messageStocks, MessageStocks, @$"🏦 Магазин `{store.Title}`
```{ex.Message.ToString()}```");
                }


            }

            _stopwatch.Stop();

            TimeSpan _elapsed = _stopwatch.Elapsed;

            await InsertAndEditMessage(messageStocks, MessageStocks, $@"✅ Успешно: `{storesCount - error} из {storesCount}`
🆕 Загружено строк `{stocksCount} шт.`
⏱️ Потраченное время: `{_elapsed.Hours} ч {_elapsed.Minutes} м. {_elapsed.Seconds} с.`");

            MessageStocks.Clear();
        }

        /// <summary>
        ///  Остатки товаров на складах WB. Данные обновляются раз в 30 минут.
        /// Сервис статистики не хранит историю остатков товаров, поэтому получить данные о них можно только в режиме "на текущий момент".
        ///  Максимум 1 запрос в минуту 
        /// </summary>
        /// <param name="store">Магазин</param>
        /// <returns></returns>
        public async Task<List<Stock>> FetchStocksFromApi(rise_project store)
        {

            var stocks = new List<Stock>();
            var httpClient = _httpClientFactory.CreateClient();

            string apiUrlBase = $"https://statistics-api.wildberries.ru/api/v1/supplier/stocks?dateFrom=2019-06-20";

            try
            {
                string apiUrl = apiUrlBase;
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                requestMessage.Headers.Add("contentType", "application/json");
                requestMessage.Headers.Add("Authorization", store.Token);

                HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var fetchedStocks = JsonConvert.DeserializeObject<List<Stock>>(responseContent);

                    foreach (var stock in fetchedStocks)
                        stock.ProjectId = store.Id;

                    stocks.AddRange(fetchedStocks);
                }

            }
            catch (HttpRequestException ex)
            {
                // Обработка ошибок связанных с HTTP-запросом
                return stocks;
            }
            catch (System.Text.Json.JsonException ex)
            {
                // Обработка ошибок десериализации JSON
                return stocks;
            }
            catch (Exception ex)
            {
                // Другие специфические исключения
                return stocks;
            }

            return stocks;
        }
        #endregion

        #region Заказы

        /// <summary>
        /// Загрузка заказов
        /// </summary>
        /// <returns></returns>
        public async Task LoadOrders()
        {
            int newRowCount = 0;
            int foreachStoresCount = 0;
            int errors = 0;

            Stopwatch globalStopwatch = new Stopwatch();
            Stopwatch privateStopwatch = new Stopwatch();

            var stores = _context.rise_projects
                .Where(x => !string.IsNullOrWhiteSpace(x.Token)
                && x.Token.Length > 155
                && x.Deleted.Value == false)
                .ToList();

            var messageOrders = await _telegramBot.SendTextMessageAsync("740755376", "Загрузка заказов",
                parseMode: ParseMode.Markdown);

            globalStopwatch.Start();


            foreach (var store in stores)
            {

                foreachStoresCount++;

                try
                {

                    privateStopwatch.Start();

                    DateTime? lastOrder = await _context?.rise_orders
                        ?.Where(x => x.ProjectId == store.Id)
                        ?.MaxAsync(x => x.Date);

                    var orders = await FetchOrdersFromApi(store, lastOrder);
                    newRowCount += orders.Count;

                    if (orders.Count > 0)
                        await BulkInsertEntitiesAsync(orders, "rise_orders");

                    privateStopwatch.Stop();
                    TimeSpan elapsed = privateStopwatch.Elapsed;

                    await InsertAndEditMessage(messageOrders, MessageOrders, @$"🏦 Магазин `{store.Title}`
🆕 Загружено строк `{orders.Count} шт.`
⏱️ Время загрузки заказов `{elapsed.Hours} ч {elapsed.Minutes} м. {elapsed.Seconds} с.`");

                }
                catch (Exception ex)
                {
                    errors++;

                    await InsertAndEditMessage(messageOrders, MessageOrders, @$"🏦 Магазин `{store.Title}`
```{ex.Message.ToString()}```");
                }
            }

            globalStopwatch.Stop();

            TimeSpan _elapsed = globalStopwatch.Elapsed;

            await InsertAndEditMessage(messageOrders, MessageOrders, $@"✅ Успешно: `{foreachStoresCount - errors} из {foreachStoresCount}`
🆕 Загружено строк `{newRowCount} шт.`
⏱️ Потраченное время: `{_elapsed.Hours} ч {_elapsed.Minutes} м. {_elapsed.Seconds} с.`");

            MessageOrders.Clear();
        }

        /// <summary>
        /// Добавление сообщения в список и изменение в телеграме
        /// </summary>
        /// <param name="message"></param>
        /// <param name="keyValues"></param>
        /// <param name="newText"></param>
        /// <returns></returns>
        private async Task InsertAndEditMessage(Message message, Dictionary<int, List<string>> keyValues, string newText)
        {
            if (!keyValues.ContainsKey(message.MessageId))
                keyValues.Add(message.MessageId, new List<string>());

            keyValues[message.MessageId].Add(newText);

            string text = string.Join($"{Environment.NewLine}{Environment.NewLine}",
                keyValues.Where(kv => kv.Key == message.MessageId)
                .SelectMany(kv => kv.Value));

            await EditMessage(message, text);
        }


        /// <summary>
        /// Редактирование сообщения в телеграм
        /// </summary>
        /// <param name="message">Телеграм сообщение</param>
        /// <param name="newText">Новое сообщение</param>
        /// <returns></returns>
        public async Task EditMessage(Message message, string newText)
        {

            await _telegramBot.EditMessageTextAsync(
                    chatId: message.Chat.Id,
                    messageId: message.MessageId,
                    text: message.Text + Environment.NewLine + newText,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }


        /// <summary>
        /// Массовое сохранение данных в базу данных
        /// </summary>
        /// <typeparam name="T">Сущность</typeparam>
        /// <param name="entities">Список сущностей</param>
        /// <param name="tableName">Таблица для сохранения в базе данных</param>
        /// <returns></returns>
        private async Task BulkInsertEntitiesAsync<T>(List<T> entities, string tableName, List<T> entitiesOld = null) where T : class
        {
            using (var connection = new MySqlConnection(ConnectionMySQL))
            {
                await connection.OpenAsync();

                var bulk = new BulkOperation<T>(connection)
                {
                    DestinationTableName = tableName
                };

                if(entitiesOld is not null)
                    await bulk.BulkDeleteAsync(entitiesOld);

                await bulk.BulkInsertAsync(entities);
                await connection.CloseAsync();
            }
        }



        /// <summary>
        /// Заказы
        /// Важно: гарантируется хранение данных по заказам не более 90 дней от даты заказа.
        /// Данные обновляются раз в 30 минут.
        /// Точное время обновления информации в сервисе можно увидеть в поле lastChangeDate.
        /// Для идентификации товаров из одного заказа, а также продаж по ним, следует использовать поле gNumber (строки с одинаковым значением этого поля относятся к одному заказу) и номер уникальной позиции в заказе odid (rid).
        /// </summary>
        /// <param name="store">Магазин</param>
        /// <param name="lastOrder">Последний заказ</param>
        /// <returns></returns>
        /// 
        public async Task<List<rise_order>> FetchOrdersFromApi(rise_project store, DateTime? lastOrder)
        {
            var orders = new List<rise_order>();
            var httpClient = _httpClientFactory.CreateClient();

            string dateFrom = lastOrder?.ToString("yyyy-MM-ddTHH:mm:ss") ?? DateFrom;
            string apiUrlBase = "https://statistics-api.wildberries.ru/api/v1/supplier/orders?dateFrom=";

            bool fetchMore = true;
            while (fetchMore)
            {
                try
                {
                    string apiUrl = apiUrlBase + dateFrom;
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                    requestMessage.Headers.Add("contentType", "application/json");
                    requestMessage.Headers.Add("Authorization", store.Token);

                    HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        var fetchedOrders = JsonConvert.DeserializeObject<List<rise_order>>(responseContent);

                        fetchedOrders = fetchedOrders?.Where(x => lastOrder == null || x.Date > lastOrder)?.ToList();

                        foreach (var order in fetchedOrders)
                        {
                            order.ProjectId = store.Id;
                        }

                        orders.AddRange(fetchedOrders);

                        var lastFetchedDate = fetchedOrders.Max(x => x?.LastChangeDate.Value)?.Date;

                        if (lastFetchedDate == null || lastFetchedDate == DateTime.Now.Date)
                        {
                            fetchMore = false;
                        }
                        else
                        {
                            dateFrom = lastFetchedDate.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                            await Task.Delay(TimeSpan.FromMinutes(1)); // Управление частотой запросов
                        }
                    }
                    else
                    {
                        fetchMore = false; // Прекратить попытки при неудачном статусе ответа
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Обработка ошибок связанных с HTTP-запросом
                    break;
                }
                catch (System.Text.Json.JsonException ex)
                {
                    // Обработка ошибок десериализации JSON
                    break;
                }
                catch (Exception ex)
                {
                    // Другие специфические исключения
                    break;
                }

            }

            return orders;
        }


        #endregion

        #region Отчет

        /// <summary>
        /// Получение подробного отчета
        /// </summary>
        /// <returns></returns>
        public async Task LoadReportDetails()
        {
            int storeCount = 0;
            int error = 0;

            var stores = _context.rise_projects
                .Where(x => !string.IsNullOrWhiteSpace(x.Token)
                && x.Token.Length > 155
                && x.Deleted.Value == false)
                .ToList();

            var messageReportDetails = await _telegramBot.SendTextMessageAsync("740755376", "Загрузка отчетов и ленты товаров",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

            Stopwatch _stopwatch = new Stopwatch();
            _stopwatch.Start();

            foreach (var store in stores)
            {

                storeCount++;

                try
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    DateTime? lastDate = _context?.ReportDetails?.Where(x => x.ProjectId == store.Id)?.Max(x => x.Create_dt);
                    var reportDetails = await FetchReportDetailsFromApi(store, lastDate);

                    if (reportDetails.Count > 0)
                        await BulkInsertEntitiesAsync(reportDetails, "ReportDetails");

                    stopwatch.Stop();

                    TimeSpan elapsed = stopwatch.Elapsed;

                    await InsertAndEditMessage(messageReportDetails, MessageReportDetails, @$"🏦 Магазин `{store.Title}`
🆕 Загружено строк `{reportDetails.Count} шт.`
⏱️ Время загрузки отчета `{elapsed.Hours} ч {elapsed.Minutes} м. {elapsed.Seconds} с.`");

                    await DataAnalysisForCardsFeedsAsync(store, messageReportDetails);
                }
                catch (Exception ex)
                {
                    error++;
                    await InsertAndEditMessage(messageReportDetails, MessageReportDetails, @$"🏦 Магазин `{store.Title}`
```{ex.Message.ToString()}```");
                }
            }

            _stopwatch.Stop();

            TimeSpan _elapsed = _stopwatch.Elapsed;

            await InsertAndEditMessage(messageReportDetails, MessageReportDetails, $@"✅ Успешно: `{storeCount - error} из {storeCount}`
⏱️ Потраченное время: `{_elapsed.Hours} ч {_elapsed.Minutes} м. {_elapsed.Seconds} с.`");

            MessageReportDetails.Clear();
        }

        /// <summary>
        /// Отчет о продажах по реализации.
        /// Отчет о продажах по реализации.
        /// В отчете доступны данные за последние 3 месяца.
        /// В случае отсутствия данных за указанный период метод вернет null.
        /// Технический перерыв в работе метода: каждый понедельник с 3:00 до 16:00.
        /// </summary>
        /// <param name="store">Магазин</param>
        /// <param name="lastDate">Последняя дата обновления</param>
        /// <returns></returns>
        public async Task<List<ReportDetail>> FetchReportDetailsFromApi(rise_project store, DateTime? lastDate)
        {
            var reportDetails = new List<ReportDetail>();
            var httpClient = _httpClientFactory.CreateClient();

            string dateFrom = lastDate.HasValue ? lastDate.Value.AddDays(1).ToString("yyyy-MM-dd") : DateFrom;
            string rrdid = "0";
            string dateTo = DateTime.Now.Date.ToString("yyyy-MM-dd");


            bool fetchMore = true;

            while (fetchMore)
            {
                try
                {
                    string apiUrl = $"https://statistics-api.wildberries.ru/api/v5/supplier/reportDetailByPeriod?dateFrom={dateFrom}&rrdid={rrdid}&dateTo={dateTo}";

                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                    requestMessage.Headers.Add("contentType", "application/json");
                    requestMessage.Headers.Add("Authorization", store.Token);

                    HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        var fetchedReportDetails = JsonConvert.DeserializeObject<List<ReportDetail>>(responseContent) ?? new List<ReportDetail>();

                        foreach (var reportDetail in fetchedReportDetails)
                            reportDetail.ProjectId = store.Id;

                        if (fetchedReportDetails is not null && fetchedReportDetails.Count > 0)
                            reportDetails.AddRange(fetchedReportDetails);

                        if (fetchedReportDetails is not null && fetchedReportDetails.Count > 0)
                        {
                            DateTime? lastReportDetail = fetchedReportDetails?.Max(x => x.Create_dt);

                            if (lastReportDetail != GetMonday(DateTime.Today.Date).Date)
                            {
                                dateFrom = lastReportDetail?.ToString("yyyy-MM-dd");
                                rrdid = fetchedReportDetails?.LastOrDefault()?.Rrd_id.ToString();

                                await Task.Delay(TimeSpan.FromMinutes(1));
                            }
                            else
                                fetchMore = false;
                        }
                        else
                            fetchMore = false;

                    }
                    else
                        fetchMore = false; // Прекратить попытки при неудачном статусе ответа
                }
                catch (HttpRequestException ex)
                {
                    // Обработка ошибок связанных с HTTP-запросом
                    break;
                }
                catch (System.Text.Json.JsonException ex)
                {
                    // Обработка ошибок десериализации JSON
                    break;
                }
                catch (Exception ex)
                {
                    // Другие специфические исключения
                    break;
                }

            }

            return reportDetails;
        }

        public static DateTime GetMonday(DateTime date)
        {
            // Получаем день недели (0 - воскресенье, 6 - суббота)
            int dayOfWeek = (int)date.DayOfWeek;

            // Вычисляем разницу между текущим днем и понедельником
            int daysToMonday = (dayOfWeek == 0) ? -6 : 1 - dayOfWeek;

            // Возвращаем дату понедельника
            return date.AddDays(daysToMonday);
        }
        #endregion

        #region Продажи

        /// <summary>
        /// Загрузка продаж
        /// </summary>
        /// <returns></returns>
        public async Task LoadSales()
        {
            int salesCount = 0;
            int _stores = 0;
            int error = 0;

            var stores = _context.rise_projects
               .Where(x => !string.IsNullOrWhiteSpace(x.Token)
               && x.Token.Length > 155
               && x.Deleted.Value == false)
               .ToList();

            var messageSales = await _telegramBot.SendTextMessageAsync("740755376",
                "Загрузка продаж",
                parseMode: ParseMode.Markdown);

            Stopwatch _stopwatch = new Stopwatch();
            _stopwatch.Start();

            foreach (var store in stores)
            {
                try
                {
                    _stores++;
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    DateTime? lastSale = _context?.Sales?.Where(x => x.ProjectId == store.Id)?.Max(x => x.LastChangeDate);

                    var sales = await FetchSalesFromApi(store, lastSale);
                    salesCount += sales.Count;

                    if (sales.Count > 0)
                        await BulkInsertEntitiesAsync(sales,"Sales");

                    stopwatch.Stop();
                    TimeSpan elapsed = stopwatch.Elapsed;

                    await InsertAndEditMessage(messageSales, MessageSales, @$"🏦 Магазин `{store.Title}`
🆕 Загружено строк `{sales.Count} шт.`
⏱️ Время загрузки продаж `{elapsed.Hours} ч {elapsed.Minutes} м. {elapsed.Seconds} с.`");
                }

                catch (Exception ex)
                {
                    error++;

                    await InsertAndEditMessage(messageSales, MessageSales, @$"🏦 Магазин `{store.Title}`
```{ex}```");
                }

            }

            _stopwatch.Stop();

            TimeSpan _elapsed = _stopwatch.Elapsed;

            await InsertAndEditMessage(messageSales, MessageSales, $@"✅ Успешно: `{_stores - error} из {_stores}`
🆕 Загружено строк `{salesCount} шт.`
⏱️ Потраченное время: `{_elapsed.Hours} ч {_elapsed.Minutes} м. {_elapsed.Seconds} с.`");

            MessageSales.Clear();
        }

        /// <summary>
        ///  Продажи и возвраты.
        ///  Гарантируется хранение данных не более 90 дней от даты продажи.
        ///  Данные обновляются раз в 30 минут.
        ///  Для идентификации заказа следует использовать поле srid.
        ///  1 строка = 1 продажа/возврат = 1 единица товара.
        ///  Максимум 1 запрос в минуту
        /// </summary>
        /// <param name="store"></param>
        /// <param name="lastOrder"></param>
        /// <returns></returns>
        public async Task<List<Sale>> FetchSalesFromApi(rise_project store, DateTime? lastSale)
        {
            string dateFrom = lastSale is null ? $"?dateFrom={DateFrom}" : "?dateFrom=" + lastSale.Value.ToString("yyyy-MM-ddTHH:mm:ss");
            List<Sale> sales = new List<Sale>();

            try
            {
                do
                {
                    using (var httpClient = new HttpClient())
                    {

                        var apiUrl = $"https://statistics-api.wildberries.ru/api/v1/supplier/sales{dateFrom}";

                        var requestMessage = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                        requestMessage.Headers.Add("contentType", "application/json");
                        requestMessage.Headers.Add("Authorization", store.Token);

                        HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();

                            if (lastSale is not null)
                                sales.AddRange(JsonConvert.DeserializeObject<List<Sale>>(responseContent)?.Where(x => x.Date > lastSale)?.ToList());
                            else
                                sales.AddRange(JsonConvert.DeserializeObject<List<Sale>>(responseContent)?.ToList());

                            foreach (var order in sales)
                                order.ProjectId = store.Id;
                        }

                    }

                    var data = sales?.Max(x => x?.LastChangeDate.Value)?.Date;

                    if (data is null) break;

                    if (data != DateTime.Now.Date)
                    {
                        dateFrom = "?dateFrom=" + data?.ToString("yyyy-MM-ddTHH:mm:ss");
                        await Task.Delay(TimeSpan.FromMinutes(1));
                    }


                } while (sales?.Max(x => x?.LastChangeDate.Value.Date) != DateTime.Now.Date);
            }
            catch
            {
                return sales;
            }

            return sales;
        }
        #endregion

        #region Юнит

        /// <summary>
        /// Загрузка продаж
        /// </summary>
        /// <returns></returns>
        public async Task LoadUnits()
        {
            var stores = _context.rise_projects.Where(x => !string.IsNullOrWhiteSpace(x.Token)
            && x.Token.Length > 155
            && x.Deleted.Value == false).ToList();


            int unitsCount = 0;
            int _stores = 0;
            int error = 0;
            var messageUnits = await _telegramBot.SendTextMessageAsync("740755376", "Загрузка юнит",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

            MessageUnits.Add(messageUnits.MessageId, new List<string>());

            Stopwatch _stopwatch = new Stopwatch();
            _stopwatch.Start();

            foreach (var store in stores)
            {
                _stores++;
                try
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    var _units = _context.rise_units.ToList();

                    var units = await FetchUnitFromApi(store);

                    int UpdateRowUnits = 0;

                    foreach (var unit in _units)
                    {
                        var _unit = units.FirstOrDefault(x => x.NmID == unit.NmID);

                        if (_unit is not null)
                        {
                            UpdateRowUnits++;
                            unit.Url = _unit.Url;
                            unit.Ordered_today = _unit.Ordered_today;
                            unit.Promotion_name = _unit.Promotion_name;
                            unit.AvgCommissionPercent = _unit.AvgCommissionPercent;
                            unit.AvgDeliveryRub = _unit.AvgDeliveryRub;
                            unit.IsCommissionRecorded = _unit.IsCommissionRecorded;
                            unit.IsLogisticsRecorded = _unit.IsLogisticsRecorded;
                            unit.Discount = _unit.Discount;
                            unit.PriceBeforeDiscount = _unit.PriceBeforeDiscount;
                            unit.PriceAfterDiscount = _unit.PriceAfterDiscount;
                        }

                    }
                    await _context.SaveChangesAsync();

                    var uniqueUnits = new List<rise_unit>();

                    foreach (var unit in units)
                    {
                        var _unit = _units.FirstOrDefault(x => x.NmID == unit.NmID);

                        if (_unit is not null)
                            continue;
                        else
                            uniqueUnits.Add(unit);
                    }
                    if (uniqueUnits.Count > 0)
                    {
                        await _context.rise_units.AddRangeAsync(uniqueUnits);
                        unitsCount += await _context.SaveChangesAsync();
                    }

                    stopwatch.Stop();

                    TimeSpan elapsed = stopwatch.Elapsed;
                    int hours = elapsed.Hours;
                    int minutes = elapsed.Minutes;
                    int seconds = elapsed.Seconds;

                    MessageUnits[messageUnits.MessageId].Add(@$"🏦 Магазин `{store.Title}`
🔄 Обновлено строк `{UpdateRowUnits} шт.`
🆕 Загружено строк `{uniqueUnits.Count} шт.`
⏱️ Время загрузки юнит `{hours} ч {minutes} м. {seconds} с.`");

                    string _text = string.Join($"{Environment.NewLine}{Environment.NewLine}",
                       MessageUnits.Where(kv => kv.Key == messageUnits.MessageId)
                       .SelectMany(kv => kv.Value));

                    await EditMessage(messageUnits, _text);
                }
                catch (Exception ex)
                {
                    error++;

                    MessageUnits[messageUnits.MessageId].Add(@$"🏦 Магазин `{store.Title}`
```{ex}```");

                    string _text = string.Join($"{Environment.NewLine}{Environment.NewLine}",
                        MessageUnits.Where(kv => kv.Key == messageUnits.MessageId)
                        .SelectMany(kv => kv.Value));

                    await EditMessage(messageUnits, _text);
                }
            }

            _stopwatch.Stop();

            TimeSpan _elapsed = _stopwatch.Elapsed;
            int _hours = _elapsed.Hours;
            int _minutes = _elapsed.Minutes;
            int _seconds = _elapsed.Seconds;

            MessageUnits[messageUnits.MessageId].Add($@"✅ Успешно: `{_stores - error} из {_stores}`
🆕 Загружено строк `{unitsCount} шт.`
⏱️ Потраченное время: `{_hours} ч {_minutes} м. {_seconds} с.`");

            string text = string.Join($"{Environment.NewLine}{Environment.NewLine}",
                MessageUnits.Where(kv => kv.Key == messageUnits.MessageId).SelectMany(kv => kv.Value));

            await EditMessage(messageUnits, text);

            MessageUnits.Clear();
        }

        public static string GetWbImageUrl(string wbArticle, int number = 1)
        {
            if (string.IsNullOrWhiteSpace(wbArticle)) return null;

            if (wbArticle != null)
            {
                string wbArticleStr = wbArticle.ToString();
                if (long.TryParse(wbArticleStr, out long articleNumber))
                {
                    int count4 = Convert.ToInt32(wbArticleStr.Substring(0, wbArticleStr.Length - 5));
                    int count6 = Convert.ToInt32(wbArticleStr.Substring(0, wbArticleStr.Length - 3));

                    string num = count4 switch
                    {
                        int n when n >= 0 && n <= 143 => "01",
                        int n when n >= 144 && n <= 287 => "02",
                        int n when n >= 288 && n <= 431 => "03",
                        int n when n >= 432 && n <= 719 => "04",
                        int n when n >= 720 && n <= 1007 => "05",
                        int n when n >= 1008 && n <= 1061 => "06",
                        int n when n >= 1062 && n <= 1115 => "07",
                        int n when n >= 1116 && n <= 1169 => "08",
                        int n when n >= 1170 && n <= 1313 => "09",
                        int n when n >= 1314 && n <= 1601 => "10",
                        int n when n >= 1602 && n <= 1655 => "11",
                        int n when n >= 1656 && n <= 1918 => "12",
                        int n when n >= 1919 && n <= 2045 => "13",
                        int n when n >= 2045 && n <= 2200 => "14",
                        _ => "15"
                    };

                    var url = WbSmallImageUrlTemplate.Replace("#id#", num)
                        .Replace("wb.ru", num == "15" ? "wbbasket.ru" : "wb.ru")
                                                   .Replace("#count6#", count6.ToString())
                                                   .Replace("#count4#", count4.ToString())
                                                   .Replace("#article#", wbArticleStr)
                                                   .Replace("#number#", "1");

                    return url;

                }

                else return null;
            }
            else return null;
        }

        private async Task<List<rise_unit>> FetchUnitFromApi(rise_project store)
        {

            List<rise_unit> Units = new List<rise_unit>();

            string apiUrl = "https://discounts-prices-api.wb.ru/api/v2/list/goods/filter";
            string servUrl = apiUrl + "?limit=1000";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", store.Token);

                    HttpResponseMessage response = await client.GetAsync(servUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();

                        JsonDocument jsonDoc = JsonDocument.Parse(responseData);
                        JsonElement root = jsonDoc.RootElement;
                        var goods = root.GetProperty("data").GetProperty("listGoods");

                        List<Good> goodsList = JsonConvert.DeserializeObject<List<Good>>(goods.ToString());

                        var promotions = await GetPromotionFromApi(goodsList.Select(x => x.nmID).ToList());

                        foreach (var good in goodsList)
                        {
                            var comission = _context.ReportDetails
                                               ?.Where(x => x.Supplier_oper_name!.ToLower() == "продажа" && x.Commission_percent.HasValue)
                                               ?.Where(x => x.Commission_percent > 0)
                                               ?.Where(x => x.Nm_id == good.nmID);

                            var delivery = _context.ReportDetails
                                 ?.Where(x => x.Supplier_oper_name!.ToLower() == "логистика" && x.Delivery_rub.HasValue)
                                 ?.Where(x => x.Delivery_rub.Value > 0)
                                 ?.Where(x => x.Nm_id == good.nmID);

                            rise_unit price = new rise_unit()
                            {
                                Url = GetWbImageUrl(good.nmID.ToString()),
                                Sa_name = good.vendorCode,
                                NmID = good.nmID,
                                Ordered_today = _context.rise_orders
           .Where(x => x.Date != null && x.ProjectId == store.Id)
           .Where(x => x.Date.Value.Date == DateTime.Now.Date) // Сравниваем только даты
           .Where(x => x.NmId == good.nmID)
           .Count(),
                                Promotion_name = promotions.FirstOrDefault(x => x.Id == good.nmID)?.PromoTextCat,
                                AvgCommissionPercent = comission.Any() ? comission?.Average(x => x.Commission_percent.Value) : null,
                                AvgDeliveryRub = delivery.Any() ? delivery.Average(x => x.Delivery_rub) : null,
                                IsCommissionRecorded = comission.Any() ? true : false,
                                IsLogisticsRecorded = delivery.Any() ? true : false,
                                ProjectId = store.Id,
                                Discount = good.discount,
                                PriceBeforeDiscount = good.sizes.FirstOrDefault().Price,
                                PriceAfterDiscount = good.sizes.FirstOrDefault().DiscountedPrice
                            };

                            Units.Add(price);
                        }

                        return Units;
                    }


                    else
                    {
                        return Units;
                    }
                }
            }
            catch (Exception ex)
            {

                return Units;
            }
        }

        public static async Task<List<Promotion>> GetPromotionFromApi(List<long?> nmID)
        {
            List<Promotion> promotions = new List<Promotion>();

            using (var httpClient = new HttpClient())
            {

                var apiUrl = $"https://card.wb.ru/cards/detail?nm={string.Join(';', nmID)}&dest=-1216601,-115136,-421732,123585595";
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        JsonDocument jsonDoc = JsonDocument.Parse(responseContent);
                        JsonElement root = jsonDoc.RootElement;
                        var products = root.GetProperty("data").GetProperty("products").ToString();
                        var list = JsonConvert.DeserializeObject<List<Promotion>>(products);
                        promotions.AddRange(list);
                        return promotions;
                    }
                    else
                    {
                        //Console.WriteLine($"HTTP request failed with status code {response.StatusCode}");
                    }
                }
                catch (HttpRequestException e)
                {
                    //Console.WriteLine($"HTTP request failed: {e.Message}");
                    return promotions;
                }
            }

            return promotions;
        }
        #endregion

        #region Конкуренты

        /// <summary>
        /// Загрузка данных по конкурентам
        /// </summary>
        /// <returns></returns>
        public async Task LoadCompetitors()
        {
            var stores = _context.rise_projects.Where(x => !string.IsNullOrWhiteSpace(x.Token)
             && x.Token.Length > 155
             && x.Deleted.Value == false).ToList();


            int CompetitorsCount = 0;
            int _stores = 0;
            int error = 0;
            var messageCompetitors = await _telegramBot.SendTextMessageAsync("740755376", "Загрузка конкурентов",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);


            Stopwatch _stopwatch = new Stopwatch();
            _stopwatch.Start();

            foreach (var store in stores)
            {
                _stores++;
                try
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    var competit = _context.rise_competitors.Where(x => x.ProjectId == store.Id).ToList();
                    await GetCompetitorsUpdateFromApi(competit);

                    var result = await GetCompetitorsFromApi(store);

                    CompetitorsCount += result;

                    TimeSpan elapsed = stopwatch.Elapsed;
                    int hours = elapsed.Hours;
                    int minutes = elapsed.Minutes;
                    int seconds = elapsed.Seconds;

                    await InsertAndEditMessage(messageCompetitors, MessageCompetitors, @$"🏦 Магазин `{store.Title}`
🆕 Загружено строк `{result} шт.`
⏱️ Время загрузки конкурентов `{hours} ч {minutes} м. {seconds} с.`");
                }
                catch (Exception ex)
                {
                    error++;
                    await InsertAndEditMessage(messageCompetitors, MessageCompetitors, @$"🏦 Магазин `{store.Title}`
```{ex}```");
                }

            }

            _stopwatch.Stop();

            TimeSpan _elapsed = _stopwatch.Elapsed;
            int _hours = _elapsed.Hours;
            int _minutes = _elapsed.Minutes;
            int _seconds = _elapsed.Seconds;

            await InsertAndEditMessage(messageCompetitors, MessageCompetitors, $@"✅ Успешно: `{_stores - error} из {_stores}`
🆕 Загружено строк `{CompetitorsCount} шт.`
⏱️ Потраченное время: `{_hours} ч {_minutes} м. {_seconds} с.`");

            MessageCompetitors.Clear();

        }

        //}

        /// <summary>
        /// Загрузка конкурентов по магазину
        /// </summary>
        /// <param name="store">Магазин</param>
        /// <returns></returns>
        public async Task<int> GetCompetitorsFromApi(rise_project store)
        {
            try
            {
                List<rise_competitor> competitors = _context.rise_competitors.Where(x => x.ProjectId == store.Id)
                   .Include(x => x.Statistics)
                   .Where(c => !c.Statistics.Any(s => s.Date == DateTime.Today.Date))
                   .ToList();

                using (var httpClient = new HttpClient())
                {

                    var apiUrl = $"https://card.wb.ru/cards/detail?nm={string.Join(';', competitors.Select(x => x.nmId))}&dest=-1216601,-115136,-421732,123585595";

                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        JsonDocument jsonDoc = JsonDocument.Parse(responseContent);
                        JsonElement root = jsonDoc.RootElement;
                        var products = root.GetProperty("data").GetProperty("products").ToString();
                        var jsonArray = JArray.Parse(products);

                        var firstObject = jsonArray.First;

                        if (firstObject is null) return 0;

                        int rowadd = 0;
                        foreach (var row in jsonArray)
                        {
                            var sizesArray = firstObject["sizes"] as JArray;
                            var firstSize = sizesArray.First;
                            var stocksArray = firstSize["stocks"] as JArray;
                            var firstStockQty = (int)stocksArray.First["qty"];

                            var stat = new rise_competitorstatistic()
                            {
                                Date = DateTime.Now.Date,
                                SalePrice = (int)row["salePriceU"] / 100,
                                InStock = firstStockQty
                            };

                            var com = await _context.rise_competitors.FirstOrDefaultAsync(x => x.nmId == (int)row["id"]);
                            if (com is not null)
                            {
                                com.Statistics.Add(stat);
                                await _context.SaveChangesAsync();
                            }
                            rowadd++;
                        }
                        return rowadd;
                    }

                }

                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }


        /// <summary>
        /// Обновление информации по конкурентам
        /// </summary>
        /// <param name="competitors"></param>
        /// <returns></returns>
        public async Task GetCompetitorsUpdateFromApi(List<rise_competitor> competitors)
        {
            List<rise_competitor> Competitors = new List<rise_competitor>();

            using (var httpClient = new HttpClient())
            {
                var apiUrl = $"https://card.wb.ru/cards/detail?nm={string.Join(';', competitors.Select(x => x.nmId))}&dest=-1216601,-115136,-421732,123585595";

                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    JsonDocument jsonDoc = JsonDocument.Parse(responseContent);
                    JsonElement root = jsonDoc.RootElement;
                    var products = root.GetProperty("data").GetProperty("products").ToString();
                    var jsonArray = JArray.Parse(products);

                    foreach (var row in jsonArray)
                    {

                        var compet = await _context.rise_competitors.FirstOrDefaultAsync(x => x.nmId == (int)row["id"]);

                        if (compet is not null)
                        {
                            compet.Brand = (string)row["brand"];
                            compet.Name = (string)row["name"];

                            if (compet?.Photos is null || compet?.Photos?.Count == 0)
                            {
                                compet.Photos.Add(new rise_competitorphoto()
                                {
                                    Url = GetWbImageUrl(compet.nmId.ToString(), 1)
                                });
                                compet.Photos.Add(new rise_competitorphoto()
                                {
                                    Url = GetWbImageUrl(compet.nmId.ToString(), 2)
                                });
                                compet.Photos.Add(new rise_competitorphoto()
                                {
                                    Url = GetWbImageUrl(compet.nmId.ToString(), 3)
                                });
                                compet.Photos.Add(new rise_competitorphoto()
                                {
                                    Url = GetWbImageUrl(compet.nmId.ToString(), 4)
                                });
                            }

                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        #endregion

        #region Карточки WB

        /// <summary>
        /// Загрузка карточек Wildberries
        /// </summary>
        /// <returns></returns>
        public async Task CardsWildberries()
        {
            var stores = _context.rise_projects.Where(x => !string.IsNullOrWhiteSpace(x.Token)
            && x.Token.Length > 155
            && x.Deleted.Value == false).ToList();


            int cardsCount = 0;
            int _stores = 0;
            int error = 0;
            var messageCards = await _telegramBot.SendTextMessageAsync("740755376", "Загрузка карточек валбериз",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

            MessageCards.Add(messageCards.MessageId, new List<string>());

            Stopwatch _stopwatch = new Stopwatch();
            _stopwatch.Start();

            foreach (var store in stores)
            {
                _stores++;

                try
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    //Старые карточки
                    List<Card> cardWildberriesOld = _context.Cards.Where(x => x.ProjectId == store.Id)?.ToList();

                    //Новые карточки
                    List<Card> cardWildberriesNew = await FetchCardWildberriesFromApi(store);


                    if (cardWildberriesNew.Count > 0)
                    {
                        cardsCount += cardWildberriesNew.Count;
                        _context?.Cards.RemoveRange(cardWildberriesOld);
                        await _context.SaveChangesAsync();


                        await _context?.Cards.AddRangeAsync(cardWildberriesNew);
                        await _context.SaveChangesAsync();
                    }

                    stopwatch.Stop();

                    TimeSpan elapsed = stopwatch.Elapsed;
                    int hours = elapsed.Hours;
                    int minutes = elapsed.Minutes;
                    int seconds = elapsed.Seconds;

                    MessageCards[messageCards.MessageId].Add(@$"🏦 Магазин `{store.Title}`
🆕 Загружено строк `{cardWildberriesNew.Count} шт.`
⏱️ Время загрузки поставок `{hours} ч {minutes} м. {seconds} с.`");

                    string _text = string.Join($"{Environment.NewLine}{Environment.NewLine}",
                        MessageCards.Where(kv => kv.Key == messageCards.MessageId).SelectMany(kv => kv.Value));

                    await EditMessage(messageCards, _text);
                }
                catch (Exception ex)
                {
                    error++;
                    MessageCards[messageCards.MessageId].Add(@$"🏦 Магазин `{store.Title}`
```{ex}```");

                    string _text = string.Join($"{Environment.NewLine}{Environment.NewLine}",
                        MessageCards.Where(kv => kv.Key == messageCards.MessageId)
                        .SelectMany(kv => kv.Value));

                    await EditMessage(messageCards, _text);
                }
            }

            _stopwatch.Stop();

            TimeSpan _elapsed = _stopwatch.Elapsed;
            int _hours = _elapsed.Hours;
            int _minutes = _elapsed.Minutes;
            int _seconds = _elapsed.Seconds;

            MessageCards[messageCards.MessageId].Add($@"✅ Успешно: `{_stores - error} из {_stores}`
🆕 Загружено строк `{cardsCount} шт.`
⏱️ Потраченное время: `{_hours} ч {_minutes} м. {_seconds} с.`");

            string text = string.Join($"{Environment.NewLine}{Environment.NewLine}",
                MessageCards.Where(kv => kv.Key == messageCards.MessageId).SelectMany(kv => kv.Value));

            await EditMessage(messageCards, text);

            MessageCards.Clear();

        }



        /// <summary>
        /// Получение товаров
        /// </summary>
        /// <param name="store">Магазин</param>
        /// <returns>Список товаров</returns>
        public async Task<List<Card>> FetchCardWildberriesFromApi(rise_project store)
        {
            string updatedAt = string.Empty;
            int? nmID = null;

            var bodyzap = new
            {
                settings = new
                {
                    cursor = new { limit = 100 },
                    filter = new { withPhoto = -1 }
                }
            };

            List<Card> productWbs = new List<Card>();

            bool IsNextPage = false;

            try
            {
                do
                {
                    var bodyzap2 = new
                    {
                        settings = new
                        {
                            cursor = new
                            {
                                limit = 100,
                                updatedAt = updatedAt,
                                nmID = nmID
                            },

                            filter = new
                            {
                                withPhoto = -1
                            }
                        }
                    };

                    using (HttpClient client = new HttpClient())
                    {
                        var apiUrl = "https://suppliers-api.wildberries.ru/content/v2/get/cards/list";

                        object _content = !IsNextPage ? bodyzap : bodyzap2;

                        var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(_content), Encoding.UTF8, "application/json");
                        var hdr = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                        hdr.Headers.Add("contentType", "application/json");
                        hdr.Headers.Add("maxRedirects", "20");
                        hdr.Content = jsonContent;
                        hdr.Headers.Add("Authorization", store.Token);


                        HttpResponseMessage response = await client.SendAsync(hdr);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();

                            JsonDocument jsonDoc = JsonDocument.Parse(content);
                            JsonElement root = jsonDoc.RootElement;
                            var cards = root.GetProperty("cards");

                            var jsonObject = JObject.Parse(content);
                            var cursor = jsonObject["cursor"] as JObject;
                            updatedAt = DateTime.Parse(cursor["updatedAt"].ToString()).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                            nmID = (int)cursor["nmID"];
                            int total = (int)cursor["total"];

                            List<Card> _cardWildberries = JsonConvert.DeserializeObject<List<Card>>(cards.ToString());
                            productWbs.AddRange(_cardWildberries);

                            foreach (var cw in productWbs)
                                cw.ProjectId = store.Id;

                            if (total < 100)
                                IsNextPage = false;
                            else
                                IsNextPage = true;

                        }
                        else
                            IsNextPage = false;
                    }

                }

                while (IsNextPage);
            }
            catch (Exception ex)
            {
                return productWbs;
            }
            return productWbs;
        }
        #endregion

        #region Лента товаров

        /// <summary>
        /// Анализ данных для ленты новостей
        /// </summary>
        /// <param name="store">Магазин</param>
        /// <returns></returns>
        private async Task<List<rise_feed>> DataAnalysisForCardsFeedsAsync(rise_project store, Message message)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var reportDetails = _context?.ReportDetails?.Where(x => x.ProjectId == store.Id).ToList();

            var cards = _context?.Cards
                .Include(x => x.Photos)?
                .Include(X => X.Sizes)?
                .Where(x => x.ProjectId == store.Id)
                .ToList();

            var orders = _context?.rise_orders?.Where(x => x.ProjectId == store.Id).ToList();
            var stocks = _context?.Stocks?.Where(x => x.ProjectId == store.Id).ToList();
            var incomes = _context?.Incomes?.Where(x => x.ProjectId == store.Id).ToList();

            List<rise_feed> cardFeeds = new List<rise_feed>();

            //Список уникальных баркодов из отчета
            List<rise_feed>? barcodes = reportDetails?.Where(x => !string.IsNullOrEmpty(x.Barcode))
                .GroupBy(x => x.Barcode)
                .Select(group => new rise_feed
                {
                    Barcode = group.FirstOrDefault().Barcode,
                    Url = $"https://wb.ru/catalog/{group.FirstOrDefault().Nm_id}/detail.aspx",
                    NmId = group.First().Nm_id,
                })
                .ToList();

            //Список уникальных баркодов из склада
            List<rise_feed>? barcodesStoks = stocks?.Where(x => !string.IsNullOrEmpty(x.Barcode))
                .GroupBy(x => x.Barcode)
                .Select(group => new rise_feed
                {
                    Barcode = group.FirstOrDefault().Barcode,
                    Url = $"https://wb.ru/catalog/{group.FirstOrDefault().NmId}/detail.aspx",
                    NmId = group.First().NmId,
                })
                .ToList();

            //Список уникальных баркодов в заказах
            List<rise_feed>? barcodesOrders = orders?.Where(x => !string.IsNullOrEmpty(x.Barcode))
                .GroupBy(x => x.Barcode)
                .Select(group => new rise_feed
                {
                    Barcode = group.FirstOrDefault().Barcode,
                    Url = $"https://wb.ru/catalog/{group.FirstOrDefault().NmId}/detail.aspx",
                    NmId = group.First().NmId,
                })
                .ToList();

            //Объединение 3 коллекций с уникальными баркодами
            barcodes.AddRange(barcodesStoks);
            barcodes.AddRange(barcodesOrders);


            //Получение уникальных баркодов товаров
            List<rise_feed> UniqProducts = barcodes.Where(x => !string.IsNullOrEmpty(x.Barcode))
                                       .GroupBy(x => x.Barcode)
                                       .Select(x => x.First())
                                       .Select(x => new rise_feed()
                                       {
                                           Barcode = x.Barcode,
                                           NmId = x.NmId,
                                           Url = x.Url,

                                           Sa_name = cards?.FirstOrDefault(y => y.NmID == x.NmId)?.VendorCode ??
                                                     reportDetails?.FirstOrDefault(y => y.Barcode == x.Barcode)?.Sa_name ??
                                                     stocks?.FirstOrDefault(y => y.NmId == x.NmId)?.SupplierArticle ??
                                                     orders.FirstOrDefault(y => y.NmId == x.NmId)?.SupplierArticle,

                                           Ts_name = reportDetails?.FirstOrDefault(y => y.Barcode == x.Barcode)?.Ts_name ??
                                                     orders?.FirstOrDefault(y => y.Barcode == x.Barcode)?.TechSize ??
                                                     stocks?.FirstOrDefault(y => y.NmId == x.NmId)?.TechSize,


                                           Brand_name = cards?.FirstOrDefault(y => y.NmID == x.NmId)?.Brand ??
                                                        reportDetails?.FirstOrDefault(y => y.Barcode == x.Barcode)?.Brand_name ??
                                                        stocks?.FirstOrDefault(y => y.NmId == x.NmId)?.Brand ??
                                                         orders.FirstOrDefault(y => y.NmId == x.NmId)?.Brand,

                                           Subject = cards?.FirstOrDefault(y => y.NmID == x.NmId)?.SubjectName ??
                                                    reportDetails?.FirstOrDefault(y => y.Barcode == x.Barcode)?.Subject_name ??
                                                    stocks?.FirstOrDefault(y => y.NmId == x.NmId)?.Subject ??
                                                    orders.FirstOrDefault(y => y.NmId == x.NmId)?.Subject,

                                           Category = orders?.FirstOrDefault(y => y.NmId == x.NmId)?.Category ??
                                                      stocks?.FirstOrDefault(y => y.NmId == x.NmId)?.Category,

                                           Image = cards?.FirstOrDefault(y => y.NmID == x.NmId)?.Photos?.FirstOrDefault()?.Tm ??
                                                   GetWbImageUrl(x.NmId.ToString()),

                                           InStock = stocks?.FirstOrDefault(y => y.Barcode == x.Barcode)?.QuantityFull.Value ?? 0,

                                           Tags = string.Empty,

                                           Commision = reportDetails?.Where(y => y.Barcode == x.Barcode).Sum(x => x.Commission),
                                           Logistics = reportDetails.Where(y => y.Barcode == x.Barcode).Sum(x => x.Logistics),

                                           QuantityOfSupplies = incomes?.Where(y => y.Barcode == x.Barcode).Sum(x => x.Quantity),
                                           DateTimeQuantityOfSupplies = incomes?.Where(y => y.Barcode == x.Barcode).Max(x => x.Date),

                                           //Заказы
                                           OrderedCount = orders?.Where(y => y.Barcode == x.Barcode).Count(),
                                           OrderSummaPrice = orders?.Where(y => y.Barcode == x.Barcode).Sum(x => x.TotalPriceDiscount.Value),

                                           //Отмены
                                           CancelCount = orders?.Where(y => y.Barcode == x.Barcode && y.IsCancel).Count(),
                                           CancelSummaPrice = orders?.Where(y => y.Barcode == x.Barcode && y.IsCancel).Where(x => x.TotalPriceDiscount.HasValue).Sum(x => x.TotalPriceDiscount.Value),

                                           //Отправлены
                                           DispatchCount = _context?.rise_cardsdispatched?.Where(y => y.Barcode == x.Barcode && y.ProjectId == store.Id).Count(),
                                           DispatchSummaPrice = _context?.rise_cardsdispatched?.Where(y => y.Barcode == x.Barcode && y.ProjectId == store.Id).Where(x => x.TotalPriceDiscount.HasValue).Sum(x => x.TotalPriceDiscount.Value),

                                           //Выкуплены
                                           PurchasedCount = _context?.rise_cardspurchaed?.Where(y => y.Barcode == x.Barcode && y.ProjectId == store.Id)?.Count(),
                                           PurchasedSummaPrice = _context?.rise_cardspurchaed?.Where(y => y.Barcode == x.Barcode && y.ProjectId == store.Id)?.Where(x => x.TotalPriceDiscount.HasValue).Sum(x => x.TotalPriceDiscount.Value),

                                           //Возвращены
                                           ReturnCount = _context?.rise_cardsreturns?.Where(y => y.Barcode == x.Barcode && y.ProjectId == store.Id).Count(),
                                           ReturnSummaPrice = _context?.rise_cardsreturns?.Where(y => y.Barcode == x.Barcode && y.ProjectId == store.Id)?.Where(x => x.TotalPriceDiscount.HasValue).Sum(x => x.TotalPriceDiscount.Value),

                                           //Без статуса
                                           WitchStatusCount = _context?.rise_cardsreturns?.Where(y => y.Barcode == x.Barcode && y.ProjectId == store.Id).Count(),

                                           ProjectId = store.Id
                                       }).ToList();

            cardFeeds.AddRange(UniqProducts);

            var _cardFeeds = _context?.rise_feeds?.Where(x => x.ProjectId == store.Id).ToList();

            if (cardFeeds?.Count > 0)
            {
                using (var connection = new MySqlConnection(ConnectionMySQL))
                {
                    connection.Open();

                    var bulk = new BulkOperation<rise_feed>(connection)
                    {
                        DestinationTableName = "rise_feeds"
                    };

                    await bulk.BulkDeleteAsync(_cardFeeds);
                    await bulk.BulkInsertAsync(cardFeeds);
                    connection.Close();
                }
            }

            stopwatch.Stop();

            TimeSpan elapsed = stopwatch.Elapsed;
            int hours = elapsed.Hours;
            int minutes = elapsed.Minutes;
            int seconds = elapsed.Seconds;

            MessageReportDetails[message.MessageId].Add(@$"🏦 Магазин `{store.Title}`
🆕 Загружено строк `{UniqProducts.Count} шт.`
⏱️ Время загрузки ленты `{hours} ч {minutes} м. {seconds} с.`");

            string _text = string.Join($"{Environment.NewLine}{Environment.NewLine}",
                MessageReportDetails.Where(kv => kv.Key == message.MessageId).SelectMany(kv => kv.Value));

            await EditMessage(message, _text);

            return cardFeeds;

        }

        //        /// <summary>
        //        /// Купленные товары
        //        /// </summary>
        //        /// <param name="store">Магазин</param>
        //        /// <returns></returns>
        //        private async Task<List<rise_cardpurchased>> FetchCardsPurchasedAsync(rise_project store, List<ReportDetail> reportDetails, Message message)
        //        {
        //            Stopwatch stopwatch = new Stopwatch();
        //            stopwatch.Start();

        //            var result = await Task.Run(async () =>
        //            {
        //                try
        //                {

        //                    var list = reportDetails?.Where(x => x.Doc_type_name != null && x.Doc_type_name.ToLower() == "продажа" && x.ProjectId == store.Id).ToList();

        //                    var cards = context?.Cards
        //                        .Include(x => x.Photos)?
        //                        .Include(X => X.Sizes)?
        //                        .Where(x => x.ProjectId == store.Id).ToList();

        //                    var orders = context?.rise_orders
        //                        .Where(x => x.ProjectId == store.Id).ToList();

        //                    var result = orders
        //                     .Where(x => x.IsOrdered && !x.IsCancel)
        //                     .Where(x => list.Any(y => y.Srid == x.Srid))
        //                     .Select(x => new rise_cardpurchased
        //                     {

        //                         Url = $"https://wb.ru/catalog/{x.NmId}/detail.aspx",

        //                         Image = cards?.FirstOrDefault(y => y.NmID == x.NmId)?.Photos?.FirstOrDefault()?.Tm ?? GetWbImageUrl(x.NmId.ToString()),

        //                         Sale_dt = list?.FirstOrDefault(y => y.Srid == x.Srid)?.Sale_dt,
        //                         Order_dt = x.Date,

        //                         Barcode = x.Barcode,
        //                         NmId = x.NmId,
        //                         Sa_name = x.SupplierArticle,

        //                         Ts_name = x.TechSize,
        //                         TotalPriceDiscount = x.TotalPriceDiscount,
        //                         ProjectId = store.Id
        //                     });

        //                    return result.ToList();
        //                }
        //                catch (Exception ex)
        //                {

        //                    throw;
        //                }

        //            });

        //            if (result.Count > 0)
        //            {
        //                using (var connection = new MySqlConnection("Server=31.31.196.247;Database=u2693092_default;Uid=u2693092_default;Pwd=V2o0oyRuG8DKLl7F"))
        //                {
        //                    connection.Open();

        //                    var bulk = new BulkOperation<rise_cardpurchased>(connection)
        //                    {
        //                        DestinationTableName = "rise_cardspurchaed"
        //                    };

        //                    await bulk.BulkInsertAsync(result);
        //                    connection.Close();
        //                }
        //            }

        //            stopwatch.Stop();

        //            TimeSpan elapsed = stopwatch.Elapsed;
        //            int hours = elapsed.Hours;
        //            int minutes = elapsed.Minutes;
        //            int seconds = elapsed.Seconds;

        //            MessageReportDetails[message.MessageId].Add(@$"🏦 Магазин `{store.Title}`
        //🆕 Загружено строк `{result.Count} шт.`
        //⏱️ Время загрузки купленных товаров `{hours} ч {minutes} м. {seconds} с.`");

        //            string _text = string.Join($"{Environment.NewLine}{Environment.NewLine}",
        //                MessageReportDetails.Where(kv => kv.Key == message.MessageId).SelectMany(kv => kv.Value));

        //            await EditMessage(message, _text);

        //            return result;
        //        }

        //        /// <summary>
        //        /// Возвращенные товары
        //        /// </summary>
        //        /// <returns></returns>
        //        private async Task<List<rise_cardreturn>> FetchCardsReturnsAsync(rise_project store, List<ReportDetail> reportDetails, Message message)
        //        {
        //            Stopwatch stopwatch = new Stopwatch();
        //            stopwatch.Start();

        //            var result = await Task.Run(async () =>
        //            {
        //                try
        //                {


        //                    var list = reportDetails?.Where(x => x.Doc_type_name != null && x.Doc_type_name.ToLower() == "возврат" && x.ProjectId == store.Id).ToList();

        //                    var cards = context?.Cards
        //                           .Include(x => x.Photos)?
        //                           .Include(X => X.Sizes)?
        //                           .Where(x => x.ProjectId == store.Id).ToList();

        //                    var orders = context?.rise_orders
        //                        .Where(x => x.ProjectId == store.Id).ToList();

        //                    var _cardsReturns = orders
        //                    ?.Where(x => x.IsOrdered && !x.IsCancel)
        //                ?.Where(x => list.Any(y => y.Srid == x.Srid))
        //                    ?.Select(x => new rise_cardreturn
        //                    {
        //                        Url = $"https://wb.ru/catalog/{x.NmId}/detail.aspx",

        //                        Image = cards?.FirstOrDefault(y => y.NmID == x.NmId)?.Photos?.FirstOrDefault()?.Tm ?? GetWbImageUrl(x.NmId.ToString()),

        //                        Order_dt = x.Date,

        //                        Barcode = x.Barcode,
        //                        NmId = x.NmId,
        //                        Sa_name = x.SupplierArticle,

        //                        Ts_name = x.TechSize,
        //                        TotalPriceDiscount = x.TotalPriceDiscount,
        //                        ProjectId = store.Id
        //                    });

        //                    return _cardsReturns.ToList();
        //                }
        //                catch (Exception ex)
        //                {

        //                    throw;
        //                }

        //            });

        //            if (result.Count > 0)
        //            {
        //                using (var connection = new MySqlConnection("Server=31.31.196.247;Database=u2693092_default;Uid=u2693092_default;Pwd=V2o0oyRuG8DKLl7F"))
        //                {
        //                    connection.Open();

        //                    var bulk = new BulkOperation<rise_cardreturn>(connection)
        //                    {
        //                        DestinationTableName = "rise_cardsreturns"
        //                    };

        //                    await bulk.BulkInsertAsync(result);
        //                    connection.Close();
        //                }
        //            }

        //            stopwatch.Stop();

        //            TimeSpan elapsed = stopwatch.Elapsed;
        //            int hours = elapsed.Hours;
        //            int minutes = elapsed.Minutes;
        //            int seconds = elapsed.Seconds;


        //            MessageReportDetails[message.MessageId].Add(@$"🏦 Магазин `{store.Title}`
        //🆕 Загружено строк `{result.Count} шт.`
        //⏱️ Время загрузки возвращенных товаров `{hours} ч {minutes} м. {seconds} с.`");

        //            string _text = string.Join($"{Environment.NewLine}{Environment.NewLine}",
        //                MessageReportDetails.Where(kv => kv.Key == message.MessageId).SelectMany(kv => kv.Value));

        //            await EditMessage(message, _text);

        //            return result;
        //        }

        //        /// <summary>
        //        /// Без статуса товары
        //        /// </summary>
        //        /// <returns></returns>
        //        private async Task<List<rise_cardnullstatus>> FilterCardsNullStatusAsync(List<rise_carddispatched> cardsDispatched, rise_project store)
        //        {
        //            Stopwatch stopwatch = new Stopwatch();
        //            stopwatch.Start();

        //            var result = await Task.Run(async () =>
        //            {
        //                try
        //                {

        //                    List<rise_cardnullstatus> nullStatusOrders = new List<rise_cardnullstatus>();

        //                    foreach (var cd in cardsDispatched)
        //                    {
        //                        var dateFrom = cd.Order_dt.Value;
        //                        var dateTo = cd.Order_dt.Value.AddMonths(1);

        //                        var card = await context?.ReportDetails?.FirstOrDefaultAsync(x =>
        //                        x.Order_dt >= dateFrom &&
        //                        x.Order_dt <= dateTo &&
        //                        x.ProjectId == store.Id &&
        //                        x.Srid == cd.Srid);

        //                        if (card is null) continue;

        //                        nullStatusOrders.Add(new rise_cardnullstatus()
        //                        {
        //                            Srid = cd.Srid,
        //                            Url = cd.Url,

        //                            Image = cd.Image,

        //                            Order_dt = cd.Order_dt,

        //                            Barcode = cd.Barcode,
        //                            NmId = cd.NmId,
        //                            Sa_name = cd.Sa_name,

        //                            Ts_name = cd.Ts_name,
        //                            TotalPriceDiscount = cd.TotalPriceDiscount,
        //                            ProjectId = cd.ProjectId

        //                        });
        //                    }

        //                    return nullStatusOrders;
        //                }
        //                catch (Exception ex)
        //                {

        //                    throw;
        //                }

        //            });

        //            var cardsNullStatus = context?.rise_cardsnullstatus?.Where(x => x.ProjectId == store.Id).ToList();
        //            context?.rise_cardsnullstatus?.RemoveRange(cardsNullStatus);
        //            await context?.SaveChangesAsync();

        //            context?.rise_cardsnullstatus?.AddRangeAsync(result);
        //            await context?.SaveChangesAsync();

        //            stopwatch.Stop();

        //            TimeSpan elapsed = stopwatch.Elapsed;
        //            int hours = elapsed.Hours;
        //            int minutes = elapsed.Minutes;
        //            int seconds = elapsed.Seconds;

        //            string message = @$"🏦 Магазин `{store.Title}`
        //🆕 Загружено строк `{result.Count - cardsNullStatus.Count} шт.`
        //⏱️ Время загрузки без статуса товаров `{hours} ч {minutes} м. {seconds} с.`

        //#{store.Title.Replace(" ", "") + DateTime.Now.Date.ToString("ddMMyyy")} #БезСтатусаТовары";

        //            await telegramBot.SendTextMessageAsync("740755376", message, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);


        //            return result;
        //        }


        #endregion

        #region Рекламные кампании

        /// <summary>
        /// Загрузка рекламных кампаний
        /// </summary>
        /// <returns></returns>
        public async Task LoadAdverts()
        {
            var stores = _context.rise_projects.Where(x => !string.IsNullOrWhiteSpace(x.Token)
           && x.Token.Length > 155
           && x.Deleted.Value == false).ToList();


            int AdvertsCount = 0;
            int AdvertsStatCount = 0;
            int _stores = 0;
            int error = 0;
            var messageAdverts = await _telegramBot.SendTextMessageAsync("740755376", "Загрузка рекламных кампаний",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

            MessageAdverts.Add(messageAdverts.MessageId, new List<string>());

            Stopwatch _stopwatch = new Stopwatch();
            _stopwatch.Start();


            foreach (var store in stores)
            {
                try
                {
                    _stores++;
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();


                    //Получение список рекламных кампаний по магазину У валбериса
                    var advertsList = await GetAdvertIdAsync(store);

                    //Полчение список рекламных кампаний в базе данных по магазину
                    List<rise_advert> adverts = _context?.rise_adverts?.Where(x => x.ProjectId == store.Id)?.ToList();

                    //Проверяем есть уникальные рекламные кампании загруженные
                    var uniqueAdverts = advertsList
                        .Where(a => !adverts.Any(x => x.AdvertId == a.AdvertId))
                        .Distinct()
                        .ToList();

                    //Если есть уникальные новые рекламные кампании, то добавляем их в базу данных
                    if (uniqueAdverts.Count > 0)
                    {
                        //Привязываем к какому магазину они относятся
                        foreach (var row in uniqueAdverts)
                            row.ProjectId = store.Id;

                        //Добавляем новые рекламные кампании и сохраняем в базу данных
                        await _context.rise_adverts.AddRangeAsync(uniqueAdverts);
                        AdvertsCount += await _context.SaveChangesAsync();
                    }

                    //Загружаем обновленный список рекламных кампаний со статистикой
                    List<rise_advert> advertslist = _context.rise_adverts
                        .Where(x => store.Id == x.ProjectId)
                        .Include(z => z.AdvertsStatistics)
                        .ToList();

                    //Определяем минимальную дату создания рекламной кампнии
                    DateTime? MinDateOfAdvert = advertslist?.Min(x => x.CreateTime).Date;

                    //Есть ли данные статистики за вчерашний день
                    bool Yesterday = advertslist.Any(x => x.AdvertsStatistics.Any(s => s.Date.ToString("yyyy-MM-dd") == DateTime.Today.Date.AddDays(-1).ToString("yyyy-MM-dd")));

                    //Есть ли данные статистики вообще
                    bool YesData = advertslist.Any(x => x.AdvertsStatistics.Count > 0);

                    //Статистика по рекламным кампаниям
                    List<rise_advertstatistic> advertStatistics = new List<rise_advertstatistic>();

                    //Если уже были загружены данные, то пропускаем магазин
                    if (Yesterday) continue;

                    //Считаем сколько иттераций по запросу данных надо совершить
                    int count = (int)Math.Ceiling((double)advertslist.Count / 100);

                    for (int i = 0; i < count; i++)
                    {
                        //Загружаем список статистики
                        List<rise_advertstatistic> _advertslist = await GetFullStatAsync(
                            //Передаем токен
                            store,
                            //Пропускаем уже полученную статистику
                            advertslist.Skip(i * 100)
                            //Выбираем первые 100 по которым еще не получена статистика
                            .Take(100)
                            //Преобразовываем в лист
                            .ToList(),
                            YesData);

                        //Если список статистики не равен нал или больше 0, то опредлеяем артикул поставщика
                        if (_advertslist is not null || _advertslist.Count > 0)
                        {
                            foreach (var row in _advertslist)
                                row.Sa_name = _context?.Cards?.FirstOrDefault(y => y.NmID == row.nmId)?.VendorCode ??
                                    _context?.ReportDetails?.FirstOrDefault(y => y.Nm_id == row.nmId)?.Sa_name ??
                                    _context?.Stocks?.FirstOrDefault(y => y.NmId == row.nmId)?.SupplierArticle ??
                                    _context?.rise_orders.FirstOrDefault(y => y.NmId == row.nmId)?.SupplierArticle;

                            //Добавляем в общий список статистики
                            advertStatistics.AddRange(_advertslist);

                            //Делаем паузу в 1 минуту, если не перебрали весь список рекламных кампаний
                            if (i != count - 1)
                                await Task.Delay(TimeSpan.FromMinutes(1));
                        }
                    }

                    //Подсчет статистики по рекламным кампаниям в общем
                    foreach (var advert in advertslist)
                        advert.AdvertsStatistics.AddRange(advertStatistics.Where(x => x.AdvertId == advert.Id).ToList());

                    if (advertslist.Count > 0)
                        //Обновляем статистику
                        _context.rise_adverts.UpdateRange(advertslist);


                    var result = await _context?.SaveChangesAsync();
                    AdvertsStatCount += result;

                    stopwatch.Stop();

                    TimeSpan elapsed = stopwatch.Elapsed;
                    int hours = elapsed.Hours;
                    int minutes = elapsed.Minutes;
                    int seconds = elapsed.Seconds;

                    MessageAdverts[messageAdverts.MessageId].Add(@$"🏦 Магазин `{store.Title}`
🆕 Загружено строк рекламных кампаний `{uniqueAdverts.Count} шт.`
🆕 Загружено строк статистики `{advertStatistics.Count} шт.`
⏱️ Потраченное время: `{hours} ч {minutes} м. {seconds} с.`");

                    string _text = string.Join($"{Environment.NewLine}{Environment.NewLine}",
                        MessageAdverts.Where(kv => kv.Key == messageAdverts.MessageId).SelectMany(kv => kv.Value));

                    await EditMessage(messageAdverts, _text);
                }

                catch (Exception ex)
                {
                    error++;

                    MessageAdverts[messageAdverts.MessageId].Add(@$"🏦 Магазин `{store.Title}`
```{ex}```");

                    string _text = string.Join($"{Environment.NewLine}{Environment.NewLine}",
                        MessageAdverts.Where(kv => kv.Key == messageAdverts.MessageId)
                        .SelectMany(kv => kv.Value));

                    await EditMessage(messageAdverts, _text);

                }

            }


            _stopwatch.Stop();

            TimeSpan _elapsed = _stopwatch.Elapsed;
            int _hours = _elapsed.Hours;
            int _minutes = _elapsed.Minutes;
            int _seconds = _elapsed.Seconds;

            MessageAdverts[messageAdverts.MessageId].Add($@"✅ Успешно: `{_stores - error} из {_stores}`
🆕 Загружено строк рекламных кампаний `{AdvertsCount} шт.`
🆕 Загружено строк статистики `{AdvertsStatCount} шт.`
⏱️ Потраченное время: `{_hours} ч {_minutes} м. {_seconds} с.`");

            string text = string.Join($"{Environment.NewLine}{Environment.NewLine}",
                MessageAdverts.Where(kv => kv.Key == messageAdverts.MessageId).SelectMany(kv => kv.Value));

            await EditMessage(messageAdverts, text);

            MessageAdverts.Clear();
        }

        /// <summary>
        /// Получение статистики по рекламным кампаниям
        /// </summary>
        /// <param name="token">Токен</param>
        /// <param name="adverts">Список рекламных кампаний</param>
        /// <param name="dateFrom">Дата начала</param>
        /// <param name="dateTo">Дата окончания</param>
        /// <returns></returns>
        private async Task<List<rise_advertstatistic>> GetFullStatAsync(rise_project store, List<rise_advert> adverts, bool isData)
        {
            List<rise_advertstatistic> advertStatistics = new List<rise_advertstatistic>();
            List<rise_advertstatistic> _advertStatistics = new List<rise_advertstatistic>();
            var cards = _context?.Cards.Where(x => x.ProjectId == adverts!.FirstOrDefault().ProjectId);

            try
            {
                string apiUrl = "https://advert-api.wb.ru/adv/v2/fullstats";


                DateTime date = !isData ? DateTime.Now.AddMonths(-2) : _context.rise_advertsstatistics.Max(x => x.Date.Date);

                bool IsLoad = true;

                do
                {
                    TimeSpan difference = DateTime.Now.Subtract(date);
                    int numberOfDays = difference.Days;

                    if (numberOfDays >= 4)
                        date = date.AddDays(4);
                    else
                        date = date.AddDays(numberOfDays);

                    var Array = isData ? adverts.Select(x =>
    new
    {
        id = x.AdvertId,
        dates = new string[] { DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd") }
    }).ToArray() :
    adverts.Select(x =>
    new
    {
        id = x.AdvertId,
        dates = Enumerable.Range(0, 4)
    .Select(offset => date.AddDays(-offset).ToString("yyyy-MM-dd"))
    .ToArray()
    }).ToArray();

                    var js = System.Text.Json.JsonSerializer.Serialize(Array);

                    var jsonContent = new StringContent(js, Encoding.UTF8, "application/json");


                    using (HttpClient httpClient = new HttpClient())
                    {
                        var hdr = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                        hdr.Headers.Add("contentType", "application/json");
                        hdr.Content = jsonContent;
                        hdr.Headers.Add("Authorization", store.Token);

                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();

                        HttpResponseMessage response = await httpClient.SendAsync(hdr);


                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var jsonArray = JArray.Parse(content);

                            foreach (var advert in jsonArray)
                            {
                                var days = advert["days"] as JArray;
                                var _advertId = (long)advert["advertId"];

                                foreach (var day in days)
                                {
                                    var apps = day["apps"] as JArray;
                                    var _day = (DateTime)day["date"];

                                    foreach (var app in apps)
                                    {
                                        var nms = app["nm"] as JArray;

                                        foreach (var nm in nms)
                                        {
                                            rise_advertstatistic advertStatistic = new rise_advertstatistic()
                                            {
                                                Date = _day,
                                                AdvertId = adverts.FirstOrDefault(x => x.AdvertId == _advertId).Id,
                                                nmId = (long)nm["nmId"],
                                                Name = (string)nm["name"],
                                                Views = (int)nm["views"],
                                                Clicks = (int)nm["clicks"],
                                                Ctr = (double)nm["ctr"],
                                                Atbs = (int)nm["atbs"],
                                                Cpc = (double)nm["cpc"],
                                                Cr = (double)nm["cr"],
                                                Orders = (int)nm["orders"],
                                                Shks = (int)nm["shks"],
                                                Sum = (double)nm["sum"],
                                                Sum_price = (double)nm["sum_price"],
                                                Sa_name = cards.FirstOrDefault(x => x.NmID == _advertId)?.VendorCode
                                            };

                                            advertStatistics.Add(advertStatistic);
                                        }
                                    }
                                }

                            }

                        }

                        var uniqueElements = advertStatistics.GroupBy(x => new { x.Date, x.nmId, x.AdvertId })
                           .Select(group => new rise_advertstatistic()
                           {
                               Date = group.Key.Date,
                               AdvertId = group.Key.AdvertId,
                               nmId = group.Key.nmId,
                               Name = group.First().Name,
                               Views = group.Sum(x => x.Views),
                               Clicks = group.Sum(x => x.Clicks),
                               Ctr = group.Sum(x => x.Clicks).Value == 0 || group.Sum(x => x.Views).Value == 0 ? 0 : ((double)group.Sum(x => x.Clicks).Value / (double)group.Sum(x => x.Views).Value) * 100,
                               Atbs = group.Sum(x => x.Atbs),
                               Cpc = group.Sum(x => x.Cpc),
                               Cr = group.Sum(x => x.Orders).Value == 0 || group.Sum(x => x.Clicks).Value == 0 ? 0 : ((double)group.Sum(x => x.Orders).Value / (double)group.Sum(x => x.Clicks).Value) * 100,
                               Orders = group.Sum(x => x.Orders),
                               Shks = group.Sum(x => x.Shks),
                               Sum = group.Sum(x => x.Sum),
                               Sum_price = group.Sum(x => x.Sum_price),
                               Sa_name = group.First().Sa_name
                           }).ToList();

                        _advertStatistics.AddRange(uniqueElements);

                        if (date.AddDays(1).Date.ToString("yyyy-MM-dd") == DateTime.Today.Date.ToString("yyyy-MM-dd")
                            || date.Date.ToString("yyyy-MM-dd") == DateTime.Today.Date.ToString("yyyy-MM-dd"))
                            IsLoad = false;

                        stopwatch.Stop();


                        if (stopwatch.Elapsed.Minutes < 0)
                            await Task.Delay(TimeSpan.FromMinutes(1) - stopwatch.Elapsed);

                    }

                } while (IsLoad);

                return _advertStatistics;

            }
            catch (Exception ex)
            {
                return new List<rise_advertstatistic>();
            }
        }

        public async Task<List<rise_advert>> GetAdvertIdAsync(rise_project store)
        {
            List<int> Adverts = new List<int>();
            List<rise_advert> AdvertsList = new List<rise_advert>();
            try
            {
                var servUrl = "https://advert-api.wb.ru/adv/v1/promotion/count";

                var request = new HttpRequestMessage(HttpMethod.Get, servUrl);
                request.Headers.Add("Authorization", store.Token);


                using (HttpClient httpClient = new HttpClient())
                {
                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var responseString = await response.Content.ReadAsStringAsync();

                    var jsonObject = JObject.Parse(responseString);
                    var adverts = jsonObject["adverts"] as JArray;
                    var list = adverts.Where(x => (int)x["status"] == 7 || (int)x["status"] == 9 || (int)x["status"] == 11).ToList();

                    foreach (var ladv in list)
                    {
                        var advert_list = ladv["advert_list"] as JArray;

                        foreach (var adv in advert_list)
                            Adverts.Add((int)adv["advertId"]);
                    }

                    int count = (int)Math.Ceiling((double)Adverts.Count / 50);

                    for (int i = 0; i < count; i++)
                    {
                        var data = Adverts.Skip(i * 50).Take(50).ToList();
                        List<rise_advert> adverts1 = await GetRkName(store, data);

                        if (adverts1.Count() > 0)
                            AdvertsList.AddRange(adverts1);
                    }

                    return AdvertsList;
                }

            }
            catch (Exception ex)
            {
                return AdvertsList;
            }
        }

        public async Task<List<rise_advert>> GetRkName(rise_project store, List<int> adverts)
        {
            List<rise_advert> advertsList = new List<rise_advert>();

            var servUrl = "https://advert-api.wb.ru/adv/v1/promotion/adverts";

            var bodyzap = adverts.ToArray();

            var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(bodyzap), Encoding.UTF8, "application/json");

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var hdr = new HttpRequestMessage(HttpMethod.Post, servUrl);
                    hdr.Headers.Add("contentType", "application/json");
                    hdr.Headers.Add("maxRedirects", "20");
                    hdr.Content = jsonContent;
                    hdr.Headers.Add("Authorization", store.Token);


                    HttpResponseMessage response = await httpClient.SendAsync(hdr);


                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var jsonArray = JArray.Parse(content);
                        List<rise_advert> _advertsList = jsonArray.Select(x => new rise_advert()
                        {
                            AdvertId = (int)x["advertId"],
                            Status = (int)x["status"],
                            Name = (string)x["name"],
                            CreateTime = (DateTime)x["createTime"],
                            EndTime = (DateTime)x["endTime"]
                        }).ToList();

                        if (_advertsList.Count() > 0)
                            advertsList.AddRange(_advertsList);

                        await Task.Delay(TimeSpan.FromSeconds(1));

                        return advertsList;

                    }
                    return advertsList;
                }

            }
            catch (Exception)
            {
                return advertsList;
            }
        }
        #endregion
    }
}
