using BillingApplication.Data.Classes;
using System.Collections;
using System.Collections.Generic;

namespace BillingApplication.Services
{
    public interface IDatabaseService
    {
        bool AddInvoice(Invoice invoice);
        Invoice GetInvoice(int id);
        IEnumerable<Invoice> GetAllInvoice();
        IEnumerable<Statistic> GetStats();
    }
}