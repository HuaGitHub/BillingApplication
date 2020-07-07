using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingApplication.Data.Classes
{
    public class Invoice
    {
        public int Id { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadTimestamp { get; set; }
        public long FileSize { get; set; }
        public string VendorName { get; set; }
        public string InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalAmountDue { get; set; }
        public string Currency { get; set; }
        public decimal TaxAmount { get; set; }
        public string ProcessingStatus { get; set; }

    }
}
