namespace Ecom.API.Models
{
    /// <summary>
    /// Магазин
    /// </summary>
    public class rise_project
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Project_Type { get; set; }
        public DateTime? Start_Date { get; set; }
        public DateTime? Deadline { get; set; }
        public int? Client_Id { get; set; }
        public DateTime? Created_Date { get; set; }
        public int? Created_By { get; set; }
        public string? Status { get; set; }
        public int? Status_Id { get; set; }
        public string? Labels { get; set; }
        public double? Price { get; set; }
        public string? Starred_By { get; set; }
        public int? Estimate_Id { get; set; }
        public int? Order_Id { get; set; }
        public bool? Deleted { get; set; }
        public string? Token { get; set; }
        public double? Tax { get; set; }

    }
}