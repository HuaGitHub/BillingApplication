using BillingApplication.Data.Classes;
using BillingApplication.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BillingApplication.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly ILogger<DatabaseService> _logger;
        private readonly DatabaseContext _databaseContext;

        public DatabaseService(ILogger<DatabaseService> logger, DatabaseContext dbContext)
        {
            _logger = logger;
            _databaseContext = dbContext;
        }

        /// <summary>
        /// Add invoice into database
        /// </summary>
        /// <param name="invoice">Invoice</param>
        /// <returns>Boolean</returns>
        public bool AddInvoice(Invoice invoice)
        {
            try
            {
                _databaseContext.Invoices.Add(invoice);
                //Calculate statistcs for each unique user
                //In order to have consistent currency for the totals, US dollar will used
                var newStat = new Statistic()
                {
                    UploadedBy = invoice.UploadedBy,
                    FileCount = 1,
                    TotalFileSize = invoice.FileSize,
                    TotalAmountUSD = ConvertToUSD(invoice.TotalAmount, invoice.Currency),
                    TotalAmountDueUSD = ConvertToUSD(invoice.TotalAmountDue, invoice.Currency)
                };

                var currentStat = _databaseContext.Statistics.FirstOrDefault(x => x.UploadedBy.Equals(invoice.UploadedBy));
                //Stat doesn't exist
                if (currentStat == null){
                    _databaseContext.Statistics.Add(newStat);
                }
                //Accumulate totals
                else
                {
                    currentStat.FileCount += newStat.FileCount;
                    currentStat.TotalFileSize += newStat.TotalFileSize;
                    currentStat.TotalAmountUSD += newStat.TotalAmountUSD;
                    currentStat.TotalAmountDueUSD += newStat.TotalAmountDueUSD;

                    _databaseContext.Statistics.Update(currentStat);
                }
                _databaseContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Insert invoice into database failed, {ex}");
                return false;
            }
        }

        /// <summary>
        /// Get all invoice from database
        /// </summary>
        /// <returns>IEnumerable<Invoice></returns>
        public IEnumerable<Invoice> GetAllInvoice()
        {
            return _databaseContext.Invoices.ToList();
        }

        /// <summary>
        /// Get invoice by Id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Invoice</returns>
        public Invoice GetInvoice(int id)
        {
            return _databaseContext.Invoices.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Get statistics totals
        /// </summary>
        /// <returns>IEnumerable<Statistic></returns>
        public IEnumerable<Statistic> GetStats()
        {
            return _databaseContext.Statistics.ToList();
        }

        //Convert to USD for statistical data
        private decimal ConvertToUSD(decimal amount, string currency)
        {
            switch (currency)
            {
                case Constant.CURRENCY_CAD:
                    amount *= Constant.CAD_To_USD;
                    break;
                case Constant.CURRENCY_GBP:
                    amount *= Constant.GBP_To_USD;
                    break;
            }
            return amount;
        } 
    }
}
