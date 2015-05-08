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
        //public DbSet<IScene> Scenes { get; set; }
        //public DbSet<ISequence> Sequences { get; set; }
    }
}
