using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DovizKur.Models
{
    public class DovizKuru
    {
        public int Id { get; set; }
        public DateTime Tarih { get; set; }
        public decimal Euro { get; set; }
        public decimal Dolar { get; set; }
    }
}