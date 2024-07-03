using System.Drawing;

namespace Ecom.API.Models
{
    public class Good
    {
        public long? nmID { get; set; }

        public string? vendorCode { get; set; }

        public IList<Size>? sizes { get; set; }

        public int? discount { get; set; }

        public bool? editableSizePrice { get; set; }
    }
}
