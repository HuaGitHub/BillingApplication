using BillingApplication.Data.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingApplication.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options){ }
    }
}
