using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOT.Kiosk.Models
{
    public class AppContext : DbContext
    {
        public AppContext()
            : base("name=AppContext")
        {

        }

        public DbSet<Person> Persons { get; set; }
    }
}
