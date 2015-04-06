using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;

namespace DAL.Model
{
    public class SequenceScene
    {
        public int Id { get; set; }
        public ISequence Sequence { get; set; }
        public IScene Scene { get; set; }
        public TimeSpan Duration { get; set; }
        public bool ShouldCleanCache { get; set; } 
    }
}
