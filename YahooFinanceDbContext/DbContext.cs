using System;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;

namespace System.Data
{
    public class YahooFinanceDbContext : DbContext
    {
        const string connectString = @"data source=(LocalDb)\v11.0;initial catalog=FinanceEntityFramework.Finance.CodeListDbContext;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";

        public YahooFinanceDbContext() : base(connectString)
        {

        }
        public DbSet<CodeList> CodeLists { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<TradeIndex> TradeIndexs { get; set; }
    }
}
