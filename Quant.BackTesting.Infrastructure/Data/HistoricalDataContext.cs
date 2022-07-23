using Microsoft.EntityFrameworkCore;
using Quant.BackTesting.Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quant.BackTesting.Infrastructure.Data
{
    public class HistoricalDataContext : DbContext
    {
        //add-migration MigrationName
        //Update-Database

        public HistoricalDataContext(DbContextOptions<HistoricalDataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SymbolData>()
            .HasIndex(p => new { p.Symbol, p.DateTime }).IsUnique();
        }

      


        public DbSet<SymbolData> Stocks { get; set; }

    }
}
