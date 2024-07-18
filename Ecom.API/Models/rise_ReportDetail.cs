namespace Ecom.API.Models
{
    /// <summary>
    /// Подробный отчет
    /// </summary>
    public class rise_ReportDetail
    {
        /// <summary>
        /// Индификатор отчета
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Номер отчёта
        /// </summary>
        public long? Realizationreport_id { get; set; }

        /// <summary>
        /// Дата начала отчётного периода
        /// </summary>
        public DateTime? Date_from { get; set; }

        /// <summary>
        /// Дата конца отчётного периода
        /// </summary>
        public DateTime? Date_to { get; set; }

        /// <summary>
        /// Дата формирования отчёта
        /// </summary>
        public DateTime? Create_dt { get; set; }

        /// <summary>
        /// Валюта
        /// </summary>
        public string? Currency_name { get; set; }

        /// <summary>
        /// Номер строки
        /// </summary>
        public long? Rrd_id { get; set; }

        /// <summary>
        /// Номер поставки
        /// </summary>
        public long? Gi_id { get; set; }

        /// <summary>
        /// Предмет
        /// </summary>
        public string? Subject_name { get; set; }

        /// <summary>
        /// Артикул
        /// </summary>
        public long? Nm_id { get; set; }

        /// <summary>
        /// Бренд
        /// </summary>
        public string? Brand_name { get; set; }

        /// <summary>
        /// Артикул продавца
        /// </summary>
        public string? Sa_name { get; set; }

        /// <summary>
        /// Размер
        /// </summary>
        public string? Ts_name { get; set; }

        /// <summary>
        /// Баркод
        /// </summary>
        public string? Barcode { get; set; }

        /// <summary>
        /// Тип документа
        /// </summary>
        public string? Doc_type_name
        {
            get => doc_type_name1; set
            {
                doc_type_name1 = string.IsNullOrWhiteSpace(value) ? null : value;
            }
        }

        private string? doc_type_name1;

        /// <summary>
        /// Количество
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// Цена розничная
        /// </summary>
        public double Retail_price { get; set; }

        /// <summary>
        /// Сумма продаж (возвратов)
        /// </summary>
        public double? Retail_amount { get; set; }

        /// <summary>
        /// Уникальный идентификатор заказа.
        /// Примечание для использующих API Marketplace: srid равен rid в ответах методов сборочных заданий
        /// </summary>
        public int? Sale_percent { get; set; }

        /// <summary>
        /// Процент комиссии
        /// </summary>
        public double? Commission_percent { get; set; }

        /// <summary>
        /// Склад
        /// </summary>
        public string? Office_name { get; set; }

        /// <summary>
        /// Обоснование для оплаты (тип транкзации)
        /// </summary>
        public string? Supplier_oper_name { get; set; }

        /// <summary>
        /// Дата заказа.
        /// Присылается с явным указанием часового пояса
        /// </summary>
        public DateTime? Order_dt { get; set; }

        /// <summary>
        /// Дата продажи.
        /// Присылается с явным указанием часового пояса
        /// </summary>
        public DateTime? Sale_dt { get; set; }

        /// <summary>
        /// Дата операции.
        /// Присылается с явным указанием часового пояса
        /// </summary>
        public DateTime? Rr_dt { get; set; }

        /// <summary>
        /// Штрих-код
        /// </summary>
        public long? Shk_id { get; set; }

        /// <summary>
        /// Цена розничная с учетом согласованной скидки
        /// </summary>
        public double? Retail_price_withdisc_rub { get; set; }

        /// <summary>
        /// Количество доставок
        /// </summary>
        public double? Delivery_amount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double? Return_amount { get; set; }

        /// <summary>
        /// Количество возвратов
        /// </summary>
        public double? Delivery_rub { get; set; }

        /// <summary>
        /// Тип коробов
        /// </summary>
        public string? Gi_box_type_name { get; set; }

        /// <summary>
        /// Согласованный продуктовый дисконт
        /// </summary>
        public int? Product_discount_for_report { get; set; }

        /// <summary>
        /// Согласованный продуктовый дисконт
        /// </summary>
        public double? Supplier_promo { get; set; }

        /// <summary>
        /// Уникальный идентификатор заказа.
        /// Примечание для использующих API Marketplace: srid равен rid в ответах методов сборочных заданий
        /// </summary>
        public long? Rid { get; set; }

        /// <summary>
        /// Скидка постоянного покупателя
        /// </summary>
        public double? Ppvz_spp_prc { get; set; }

        /// <summary>
        /// Размер кВВ без НДС, % базовый
        /// </summary>
        public double? Ppvz_kvw_prc_base { get; set; }

        /// <summary>
        /// Размер кВВ без НДС
        /// </summary>
        public double? Ppvz_kvw_prc { get; set; }

        /// <summary>
        /// Размер снижения кВВ из-за рейтинга
        /// </summary>
        public double? Sup_rating_prc_up { get; set; }

        /// <summary>
        /// Размер снижения кВВ из-за акции
        /// </summary>
        public decimal? Is_kgvp_v2 { get; set; }

        /// <summary>
        /// Вознаграждение с продаж до вычета услуг поверенного, без НДС
        /// </summary>
        public double? Ppvz_sales_commission { get; set; }

        /// <summary>
        /// К перечислению продавцу за реализованный товар
        /// </summary>
        public double? Ppvz_for_pay { get; set; }

        /// <summary>
        /// Возмещение за выдачу и возврат товаров на ПВЗ
        /// </summary>
        public double? Ppvz_reward { get; set; }

        /// <summary>
        /// Возмещение издержек по эквайрингу.
        /// Издержки WB за услуги эквайринга: вычитаются из вознаграждения WB и не влияют на доход продавца.
        /// </summary>
        public double? Acquiring_fee { get; set; }

        /// <summary>
        /// Наименование банка-эквайера
        /// </summary>
        public string? Acquiring_bank { get; set; }

        /// <summary>
        /// Вознаграждение WB без НДС
        /// </summary>
        public double? Ppvz_vw { get; set; }

        /// <summary>
        /// НДС с вознаграждения WB
        /// </summary>
        public double? Ppvz_vw_nds { get; set; }

        /// <summary>
        /// Номер офиса
        /// </summary>
        public long? Ppvz_office_id { get; set; }

        /// <summary>
        /// Наименование офиса доставки
        /// </summary>
        public string? Ppvz_office_name { get; set; }

        /// <summary>
        /// Номер партнера
        /// </summary>
        public long? Ppvz_supplier_id { get; set; }

        /// <summary>
        /// Партнер
        /// </summary>
        public string? Ppvz_supplier_name { get; set; }

        /// <summary>
        /// ИНН партнера
        /// </summary>
        public string? Ppvz_inn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Declaration_number { get; set; }

        /// <summary>
        /// Номер таможенной декларации
        /// </summary>
        public string? Sticker_id { get; set; }

        /// <summary>
        /// Страна продажи
        /// </summary>
        public string? Site_country { get; set; }

        /// <summary>
        /// Штрафы
        /// </summary>
        public double? Penalty { get; set; }

        /// <summary>
        /// Доплаты
        /// </summary>
        public double? Additional_payment { get; set; }

        /// <summary>
        /// Уникальный идентификатор заказа.
        /// Примечание для использующих API Marketplace: srid равен rid в ответах методов сборочных заданий.
        /// </summary>
        public string? Srid { get; set; }

        /// <summary>
        /// Обоснование штрафов и доплат.
        /// Поле будет в ответе при наличии значения
        /// </summary>
        public string? Bonus_type_name { get; set; }

        /// <summary>
        ///  Цена
        /// </summary>
        public double? Price => Rr_dt == null ? 0 :
            (Supplier_oper_name == "Возврат" || Supplier_oper_name == "Корректный возврат" || Supplier_oper_name == "Сторно продаж" ? -Retail_amount :
            (Supplier_oper_name == "Частичная компенсация брака" ? Retail_amount :
            (Doc_type_name == "Возврат" && Supplier_oper_name == "Авансовая оплата за товар без движения" ? -Retail_amount : Retail_amount)));

        /// <summary>
        /// Логистика
        /// </summary>
        public double? Logistics => Supplier_oper_name == "Логистика сторно" ? -Delivery_rub : (Delivery_rub ?? 0);

        /// <summary>
        /// Комиссия
        /// </summary>
        public double? Commission => Rr_dt == null ? null : Price - PaymentsWB;

        /// <summary>
        /// Продажи (шт.)
        /// </summary>
        public int? OrderCount => Rr_dt == null ? 0 :
            (Supplier_oper_name == "Возврат" || Supplier_oper_name == "Корректный возврат" || Supplier_oper_name == "Сторно продаж" ? -1 :
            (Supplier_oper_name == "Корректная продажа"
            || Supplier_oper_name == "Продажа"
            || Supplier_oper_name == "Сторно возвратов"
            || Supplier_oper_name == "Штрафы" ? Quantity.Value : 0));

        /// <summary>
        /// Комиссия ВБ
        /// </summary>
        public double PaymentsWB => Rr_dt == null ?
            0 : (Supplier_oper_name == "Возврат" || Supplier_oper_name == "Корректный возврат" || Supplier_oper_name == "Сторно продаж" ? -Ppvz_for_pay.Value :
            (Supplier_oper_name == "Частичная компенсация брака" ? Ppvz_for_pay.Value :
            (Doc_type_name == "Возврат" && Supplier_oper_name == "Авансовая оплата за товар без движения" ? -Ppvz_for_pay.Value : Ppvz_for_pay.Value)));

        /// <summary>
        /// Магазин
        /// </summary>
        public int ProjectId { get; set; }
    }
}
