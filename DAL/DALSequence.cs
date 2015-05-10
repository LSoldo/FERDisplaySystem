using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Model;
using System.Data.Entity;

namespace DAL
{
    public class DALSequence : IDisposable
    {
        private DisplayContext context;

        public DALSequence()
        {
            this.context = new DisplayContext();
        }

        public int AddSequence(Sequence sequence)
        {
            try
            {
                if (sequence == null)
                    return -1;

                foreach (var sequenceScene in sequence.SequenceScenes)
                {
                    if (sequenceScene.Scene != null && sequenceScene.SceneId < 1)
                    {
                        sequenceScene.SceneId = sequenceScene.Scene.Id;
                        sequenceScene.Scene = null;
                    }
                    else if (sequenceScene.Scene != null && sequenceScene.SceneId > 0)
                        sequenceScene.Scene = null;
                }
                this.context.Sequences.Add(sequence);
                this.context.SaveChanges();
                return sequence.Id;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding new scene: " + ex.Message);
            }
        }

        public void RemoveSequence(Sequence sequence)
        {
            try
            {
                if (sequence == null)
                    return;

                if (sequence.SequenceScenes != null)
                    this.context.SequenceScenes.RemoveRange(sequence.SequenceScenes);

                this.context.Sequences.Remove(sequence);
                this.context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while deleting sequence by id, id:{0}, error message: {1} ", sequence.Id, ex.Message));
            }
        }

        public void UpdateSequence(Sequence newSequence)
        {
            try
            {
                var oldSequence = this.context.Sequences
                    .Include(s => s.SequenceScenes)
                    .SingleOrDefault(x => x.Id == newSequence.Id);

                if (oldSequence == null)
                    throw new Exception("Sequence not found");

                this.context.Entry(oldSequence).CurrentValues.SetValues(newSequence);
                if (oldSequence.SequenceScenes != null)
                    this.context.SequenceScenes.RemoveRange(oldSequence.SequenceScenes);
                oldSequence.SequenceScenes = newSequence.SequenceScenes;
                this.context.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while updating sequence, id:{0}, error message: {1} ", newSequence.Id, ex.Message));
            }
        }

        public Sequence GetSequenceById(int id)
        {
            try
            {
                return this.context.Sequences
                    .Include(x => x.SequenceScenes.Select(s => s.Scene).Select(s => s.DataSources))
                    .SingleOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while getting new sequence, id: {0}, {1}", id, ex.Message));
            }
        }
        
        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}
