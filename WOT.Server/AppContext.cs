using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOT.Server.Models;

namespace WOT.Server
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
