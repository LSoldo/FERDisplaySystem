using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Model;
using System.Data.Entity;
using DAL.Interfaces;

namespace DAL
{
    public class DALSequence : IDisposable
    {
        private DisplayContext context;

        public DALSequence()
        {
            this.context = new DisplayContext();
        }

        public DisplayContext GetContext()
        {
            return this.context;
        }

        public int AddSequence(Sequence sequence)
        {
            try
            {
                if (sequence == null)
                    return -1;

                foreach (var sequenceScene in sequence.SequenceScenes)
                {
                    if (sequenceScene.Scene != null && sequenceScene.Scene.Id > 0 && sequenceScene.SceneId < 1)
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
        /// <summary>
        /// Deletes sequence from the database
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns>Returns false if sequence could not be removed if it's contained in at least one terminal sequence or it's null</returns>
        public bool RemoveSequence(Sequence sequence)
        {
            try
            {
                if (sequence == null)
                    return false;
                if (this.context.TerminalSequences.Any(s => s.Sequence == sequence))
                    return false;

                if (sequence.SequenceScenes != null)
                    this.context.SequenceScenes.RemoveRange(sequence.SequenceScenes);

                this.context.Sequences.Remove(sequence);
                this.context.SaveChanges();
                return true;
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

        public int AddTerminalSequence(TerminalSequence terminalSequence)
        {
            try
            {
                if (terminalSequence == null)
                    return -1;

                this.context.TerminalSequences.Add(terminalSequence);
                this.context.SaveChanges();
                return terminalSequence.Id;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while adding new terminal sequence, {0}", ex.Message));
            }
        }

        public void RemoveTerminalSequence(int id)
        {
            try
            {
                var termSeq = this.context.TerminalSequences
                    .Include(t => t.TimeIntervals)
                    .Include(t => t.Setting)
                    .SingleOrDefault(x => x.Id == id);

                if (termSeq == null)
                    throw new Exception("Terminal sequence not found");

                if (termSeq.TimeIntervals != null)
                    this.context.TimeIntervals.RemoveRange(termSeq.TimeIntervals);
                if (termSeq.Setting != null)
                    this.context.DisplaySettings.Remove(termSeq.Setting);

                this.context.TerminalSequences.Remove(termSeq);
                this.context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while deleting terminal sequence, id: {0}, {1} ", id, ex.Message));
            }
        }

        public void UpdateTerminalSequenceDisplaySetting(int terminalSequenceId, DisplaySetting newSetting)
        {
            try
            {
                var termSeq = this.context.TerminalSequences
                    .Include(t => t.Setting)
                    .SingleOrDefault(x => x.Id == terminalSequenceId);

                if (termSeq == null)
                    throw new Exception("Terminal sequence not found");
                
                if(termSeq.Setting != null)
                    this.context.DisplaySettings.Remove(termSeq.Setting);
                termSeq.Setting = newSetting;
                this.context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while updating terminal sequence, id: {0}, {1} ", terminalSequenceId, ex.Message));
            }
        }

        public void UpdateTerminalSequenceTimeIntervals(int terminalSequenceId, List<TimeInterval> newTimeIntervals)
        {
            try
            {
                var termSeq = this.context.TerminalSequences
                    .Include(t => t.TimeIntervals)
                    .SingleOrDefault(x => x.Id == terminalSequenceId);

                if (termSeq == null)
                    throw new Exception("Terminal sequence not found");

                if (termSeq.TimeIntervals != null)
                    this.context.TimeIntervals.RemoveRange(termSeq.TimeIntervals);
                termSeq.TimeIntervals = newTimeIntervals;
                this.context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while updating terminal sequence, id: {0}, {1} ", terminalSequenceId, ex.Message));
            }
        }

        public void UpdateTerminalSequence(TerminalSequence terminalSequence)
        {
            try
            {
                if(terminalSequence == null || terminalSequence.Id < 1)
                    throw new Exception("Terminal sequence not valid - either null or id value is 0");

                this.context.TerminalSequences.Attach(terminalSequence);
                this.context.Entry(terminalSequence).State = EntityState.Modified;
                this.context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while updating terminal sequence, id: {0}, {1} ", terminalSequence != null ? terminalSequence.Id : 0, ex.Message));
            }
        }

        public TerminalSequence GetTerminalSequenceById(int id)
        {
            try
            {
                return this.context.TerminalSequences
                    .Include(t => t.TimeIntervals)
                    .Include(t => t.Setting)
                    .Include(t => t.Sequence)
                    .Include(t => t.Sequence.SequenceScenes.Select(x => x.Scene).Select(x => x.DataSources))
                    .SingleOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while getting terminal sequence, id: {0}, {1} ", id, ex.Message));
            }
        }

        public List<TerminalSequence> GetTerminalSequenceById(List<int> terminalSequenceList)
        {
            try
            {
                if (terminalSequenceList == null || terminalSequenceList.Count == 0)
                    return null;

                return this.context.TerminalSequences
                    .Include(t => t.TimeIntervals)
                    .Include(t => t.Setting)
                    .Include(t => t.Sequence)
                    .Include(t => t.Sequence.SequenceScenes.Select(x => x.Scene).Select(x => x.DataSources))
                    .Where(x => terminalSequenceList.Contains(x.Id)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while getting terminal sequence, {0}", ex.Message));
            }
        }

        public List<TerminalSequence> GetTerminalTerminalSequencePool(int terminalId)
        {
            try
            {
                var terminal = this.context.Terminals.SingleOrDefault(x => x.Id == terminalId);
                return terminal == null ? null : GetTerminalSequenceById(terminal.TerminalSequencePool.Select(x => x.Id).ToList());
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while getting terminal's terminal sequence, id: {0}, {1} ", terminalId, ex.Message));
            }
        }

        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}
