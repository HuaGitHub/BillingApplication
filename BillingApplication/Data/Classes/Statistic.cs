using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingApplication.Data.Classes
{
    public class Statistic
    {
        public int Id { get; set; }
        public string UploadedBy { get; set; }
        public int FileCount { get; set; }
        public long TotalFileSize { get; set; }
        public decimal TotalAmountUSD { get; set; }
        public decimal TotalAmountDueUSD { get; set; }
    }
}
