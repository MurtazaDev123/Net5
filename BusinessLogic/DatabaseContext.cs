using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(): base("ConnectionString")
        {

        }

        public DbSet<Partner> VU_Partners { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    [Table("VU_Partners")]
    public class Partner
    {
        public int Id { get; set; }
        public string UserName { get; set; }
    }
}
