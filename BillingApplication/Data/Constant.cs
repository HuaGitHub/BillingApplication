using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingApplication.Data
{
    public class Constant
    {
        #region Currency
        public const string CURRENCY_CAD = "CAD";
        public const string CURRENCY_GBP = "GBP";
        public const string CURRENCY_USD = "USD";
        public const decimal CAD_To_USD = 0.74M;
        public const decimal GBP_To_USD = 1.25M;

        #endregion
    }
}
