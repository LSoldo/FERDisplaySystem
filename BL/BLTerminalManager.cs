using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using DAL.Interfaces;
using DAL.Model;

namespace BL
{
    public class BLTerminalManager : IDisposable
    {
        private DALTerminal dalTerminal;
        private DALSequence dalSequence;

        public BLTerminalManager()
        {
            this.dalTerminal = new DALTerminal();
            this.dalSequence = new DALSequence();
        }

        /// <summary>
        /// Recalculates display times for specified terminal and checks if new times overlap with existing ones.
        /// </summary>
        /// <param name="terminalId"></param>
        /// <param name="terminalSequence"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="showEvery"></param>
        /// <param name="consecutiveTimesToShow"></param>
        /// <returns>Returns true if new calculated times don't overlap with existing ones, and false if they do.</returns>
        public bool EditDisplayTimesForTerminalSequence(int terminalId,
            TerminalSequence terminalSequence,
            DateTime startTime,
            DateTime endTime,
            TimeSpan? showEvery,
            int? consecutiveTimesToShow)
        {
            var manager = new BLDisplaySettingsManager();
            var terminal = dalTerminal.GetTerminalById(terminalId);
            if (terminal == null)
                throw new Exception(string.Format("Terminal with id {0} not found", terminalId));

            DisplaySetting setting;
            var newTimeIntervals = manager.CreateDisplayTimes(startTime, endTime, showEvery, consecutiveTimesToShow,
                out setting);
            var existingTimeIntervalsForTerminal =
                terminal.TerminalSequencePool.Where(seq => seq.Active)
                    .Except(new List<TerminalSequence>() {terminalSequence})
                    .ToList();

            var timeIntervalsUnion = new List<TimeInterval>();
            existingTimeIntervalsForTerminal.ForEach(i => timeIntervalsUnion.AddRange(i.TimeIntervals));

            if (manager.IsOverlappingWithExistingDate(timeIntervalsUnion, newTimeIntervals))
                return false;

            terminalSequence.TimeIntervals = newTimeIntervals;
            terminalSequence.Setting = setting;
            dalSequence.UpdateTerminalSequenceDisplaySetting(terminalSequence.Id, setting);
            dalSequence.UpdateTerminalSequenceTimeIntervals(terminalSequence.Id, newTimeIntervals);
            return true;
        }

        /// <summary>
        /// Overrides current sequence and sets manual sequence
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="manualSequence"></param>
        /// <param name="showFromToInterval"></param>
        public void SetManualSequenceForTerminal(Terminal terminal, TerminalSequence manualSequence)
        {
            if(terminal != null && terminal.Id > 0 && manualSequence != null)
                dalTerminal.UpdateOrAddTerminalSequence(terminal.Id, manualSequence, DataDefinition.CurrentSequence.ManualSequence);
            else
                throw new Exception("Error while updating manual sequence: possible null reference on manual sequence and/or terminal, or terminal id not correct");
        }
        /// <summary>
        /// Sets default sequence for terminal
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="defaultSequence"></param>
        public void SetDefaultSequenceForTerminal(Terminal terminal, TerminalSequence defaultSequence)
        {
            if (terminal != null && terminal.Id > 0 && defaultSequence != null)
                dalTerminal.UpdateOrAddTerminalSequence(terminal.Id, defaultSequence, DataDefinition.CurrentSequence.DefaultSequence);
            else
                throw new Exception("Error while updating default sequence: possible null reference on default sequence and/or terminal, or terminal id not correct");
        }
        /// <summary>
        /// Adds sequence to terminal
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="sequence"></param>
        public void AddSequenceToTerminal(Terminal terminal, TerminalSequence sequence)
        {
            if (terminal != null && terminal.Id > 0 && sequence != null)
                dalTerminal.UpdateOrAddTerminalSequence(terminal.Id, sequence, DataDefinition.CurrentSequence.ScheduledSequence);
            else
                throw new Exception("Error while adding scheduled sequence: possible null reference on scheduled sequence and/or terminal, or terminal id not correct");
        }

        /// <summary>
        /// Gets current sequence for specified terminal based on targetTime
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="targetTime"></param>
        /// <returns>Current sequence for terminal</returns>
        public TerminalSequence GetCurrentSequence(Terminal terminal, DateTime targetTime)
        {
            var manager = new BLDisplaySettingsManager();

            if (terminal.ManualSequence != null &&
                (terminal.ManualSequence.TimeIntervals == null || terminal.ManualSequence.TimeIntervals.Count == 0 ||
                 manager.IsOverlappingWithExistingDate(terminal.ManualSequence.TimeIntervals, targetTime)))
            {
                return terminal.ManualSequence;
            }
            return GetCurrentScheduleForTerminal(terminal, targetTime) ?? terminal.DefaultSequence;
        }
        /// <summary>
        /// Gets time interval inside which targetTime parameter is located
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="targetTime"></param>
        /// <returns>Returns TimeInterval if found, null if not found</returns>
        public TimeInterval GetCurrentTimeIntervalForSequence(TerminalSequence sequence, DateTime targetTime)
        {
            return
                sequence.TimeIntervals.FirstOrDefault(t => t.TimeFrom <= targetTime && targetTime <= t.TimeTo);
        }
        /// <summary>
        /// Gets TerminalSequence for current time for specified terminal
        /// </summary>
        /// <param name="terminal"></param>
        /// <returns>Returns TerminalSequence</returns>
        public TerminalSequence GetCurrentScheduleForTerminal(Terminal terminal)
        {
            return GetCurrentSchedule(terminal.Id, DateTime.Now);
        }
        /// <summary>
        /// Gets TerminalSequence for target time for specified terminal
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="targetTime"></param>
        /// <returns>Returns TerminalSequence</returns>
        public TerminalSequence GetCurrentScheduleForTerminal(Terminal terminal, DateTime targetTime)
        {
            var manager = new BLDisplaySettingsManager();
            var sequences = terminal.TerminalSequencePool.Where(ts => ts.Active).ToList();

            return sequences.FirstOrDefault(sequence => manager.IsOverlappingWithExistingDate(sequence.TimeIntervals, targetTime));
        }
        /// <summary>
        /// Wrapper that gets time interval inside which targetTime parameter is located
        /// </summary>
        /// <param name="terminalId"></param>
        /// <returns>Returns TerminalSequence</returns>
        public TerminalSequence GetCurrentScheduleForTerminal(int terminalId)
        {
            return GetCurrentSchedule(terminalId, DateTime.Now);
        }
        /// <summary>
        /// Wrapper that gets TerminalSequence for target time for specified terminal
        /// </summary>
        /// <param name="terminalId"></param>
        /// <param name="targetTime"></param>
        /// <returns>Returns TerminalSequence</returns>
        public TerminalSequence GetCurrentScheduleForTerminal(int terminalId, DateTime targetTime)
        {
            return GetCurrentSchedule(terminalId, targetTime);
        }
        /// <summary>
        /// Gets TerminalSequence for target time for specified terminal
        /// </summary>
        /// <param name="terminalId"></param>
        /// <param name="targetTime"></param>
        /// <returns>Returns TerminalSequence</returns>
        private TerminalSequence GetCurrentSchedule(int terminalId, DateTime targetTime)
        {
            var sequences = GetScheduleForTerminal(terminalId);

            return (from sequence in sequences
                    let interval =
                        sequence.TimeIntervals.FirstOrDefault(i => i.TimeFrom <= targetTime && i.TimeTo >= targetTime)
                    where interval != null
                    select sequence).FirstOrDefault();
        }
        /// <summary>
        /// Gets all active scheduled terminal sequences for specified terminal
        /// </summary>
        /// <param name="terminalId"></param>
        /// <returns>Returns list of found terminal sequences</returns>
        public List<TerminalSequence> GetScheduleForTerminal(int terminalId)
        {
            var terminal = dalTerminal.GetTerminalById(terminalId);
            return terminal != null ? terminal.TerminalSequencePool.Where(pool => pool.Active).ToList() : null;
        }

        /// <summary>
        /// Gets all active scheduled terminal sequences for specified terminal in specified time interval
        /// </summary>
        /// <param name="terminalId"></param>
        /// <param name="timeFrom"></param>
        /// <param name="timeTo"></param>
        /// <returns>Returns list of found terminal sequences</returns>
        private List<TerminalSequence> GetSchedule(int terminalId,
            DateTime timeFrom,
            DateTime timeTo)
        {
            var sequences = GetScheduleForTerminal(terminalId);

            return
                sequences.Where(
                    sequence => sequence.Active &&
                                sequence.TimeIntervals.Any(
                                    t =>
                                        ((t.TimeFrom <= timeFrom && t.TimeTo > timeFrom) ||
                                         (t.TimeFrom < timeTo && t.TimeTo >= timeTo) ||
                                         (t.TimeFrom > timeFrom && t.TimeTo < timeTo)))).ToList();
        }
        /// <summary>
        /// Wrapper that gets all active scheduled terminal sequences for specified terminal in specified time interval
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="timeFrom"></param>
        /// <param name="timeTo"></param>
        /// <returns>Returns list of found terminal sequences</returns>
        public List<TerminalSequence> GetScheduleForTerminal(Terminal terminal, DateTime timeFrom, DateTime timeTo)
        {
            return terminal != null ? GetSchedule(terminal.Id, timeFrom, timeTo) : null;
        }
        /// <summary>
        /// Wrapper that gets all active scheduled terminal sequences for specified terminal in specified time interval
        /// </summary>
        /// <param name="terminalId"></param>
        /// <param name="timeFrom"></param>
        /// <param name="timeTo"></param>
        /// <returns>Returns list of found terminal sequences</returns>
        public List<TerminalSequence> GetScheduleForTerminal(int terminalId, DateTime timeFrom, DateTime timeTo)
        {
            return GetSchedule(terminalId, timeFrom, timeTo);
        }

        public List<TerminalSequence> GetScheduleForTerminal(Terminal terminal)
        {
            return terminal != null ? GetScheduleForTerminal(terminal.Id) : null;
        }

        public List<TerminalSequence> GetEntireHistoryForTerminal(Terminal terminal)
        {
            if (terminal != null && terminal.Id > 0)
                return GetEntireHistoryForTerminal(terminal.Id);

            return null;
        }

        public List<TerminalSequence> GetEntireHistoryForTerminal(int terminalId)
        {
            var terminal = dalTerminal.GetTerminalById(terminalId);
            return terminal != null ? terminal.TerminalSequencePool : null;
        }

        public List<TerminalSequence> GetInactiveScheduleForTerminal(Terminal terminal)
        {
            if (terminal != null && terminal.Id > 0)
                return GetInactiveScheduleForTerminal(terminal.Id);

            return null;
        }

        public List<TerminalSequence> GetInactiveScheduleForTerminal(int terminalId)
        {
            var terminal = dalTerminal.GetTerminalById(terminalId);
            return terminal != null && terminal.TerminalSequencePool != null
                ? terminal.TerminalSequencePool.Where(t => !t.Active).ToList()
                : null;
        }

        public void Dispose()
        {
            if (this.dalTerminal != null)
            {
                this.dalTerminal.Dispose();
                this.dalTerminal = null;
            }
            if (this.dalSequence != null)
            {
                this.dalSequence.Dispose();
                this.dalSequence = null;
            }
        }
    }
}
