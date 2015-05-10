using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;
using DAL.Model;

namespace DAL
{
    public class DisplayContext : DbContext
    {
        public DisplayContext() : base("FERDisplaySystem") { }

        public DbSet<Terminal> Terminals { get; set; }
        public DbSet<TerminalSequence> TerminalSequences { get; set; }
        public DbSet<SequenceScene> SequenceScenes { get; set; }
        public DbSet<Scene> Scenes { get; set; }
        public DbSet<Sequence> Sequences { get; set; }
        public DbSet<DataSource> DataSources { get; set; }
        public DbSet<DisplaySetting> DisplaySettings { get; set; }
        public DbSet<TimeInterval> TimeIntervals { get; set; }

    }
}
