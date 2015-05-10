using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Factories;

namespace DAL.Model
{
    public class Sequence
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SequenceType { get; private set; }
        public List<SequenceScene> Scenes { get; set; }

        protected Sequence()
        {
        }

        public Sequence(string name, string sequenceType, List<SequenceScene> scenes)
        {           
            this.Name = name;
            this.SequenceType = sequenceType;
            this.Scenes = scenes;
        }
        //facade
        public string GenerateHtml(string generatorType, string groupId, string terminalSequenceId)
        {
            if(this.Scenes == null)
                throw new Exception("Scenes are null for sequence id " + this.Id);
            var sequence = new SequenceGeneratorFactory().GetSequence(generatorType ?? this.SequenceType);
            return sequence.GenerateHtml(this.Scenes.Where(s => s.Scene.Active).ToList(), groupId, terminalSequenceId);
        }


    }
}
