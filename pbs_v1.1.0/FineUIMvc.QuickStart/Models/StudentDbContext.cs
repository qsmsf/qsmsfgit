using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FineUIMvc.QuickStart.Models
{
    public class StudentDbContext : DbContext
    {
        public StudentDbContext()
            //: base("DefaultConnection")
        {
        }

        public DbSet<Student> Students { get; set; }
    }
}