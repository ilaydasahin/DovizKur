using DovizKur.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DovizKur.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<DovizKuru> DovizKurlari { get; set; }
    }
}