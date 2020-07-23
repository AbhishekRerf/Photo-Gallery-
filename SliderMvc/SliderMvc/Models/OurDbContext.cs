using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace SliderMvc.Models
{
    public class OurDbContext : DbContext
    {
        public DbSet<Gallery> gallery { get; set; }

    }
}