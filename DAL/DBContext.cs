using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Model;

namespace DAL
{
    public class DBContext : DbContext
    {
        public DBContext() : base("FERDisplaySystem") { }

        public DbSet<Terminal> Terminals { get; set; }
        public DbSet<ScheduledDisplayTime> ScheduledDisplayTimes { get; set; }
        public DbSet<DigitalSign> DigitalSigns { get; set; }
        public DbSet<DisplaySetting> DisplaySettings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }


    }
}
