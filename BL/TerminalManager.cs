using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;
using DAL.Model;

namespace BL
{
    public class TerminalManager
    {
        public bool RecalculateDisplayTimesForTerminal(Terminal terminal,
            TerminalSequence terminalSequence,
            DateTime startTime,
            DateTime endTime,
            TimeSpan? showEvery,
            int? consecutiveTimesToShow)
        {
            var manager = new DisplaySettingsManager();

            DisplaySetting setting;
            var newTimeIntervals = manager.CreateDisplayTimes(startTime, endTime, showEvery, consecutiveTimesToShow,
                out setting);
            var existingTimeIntervalsForTerminal =
                terminal.AllSequences.Where(seq => seq.Active)
                    .Except(new List<TerminalSequence>() {terminalSequence})
                    .ToList();

            var timeIntervalsUnion = new List<TimeInterval>();
            existingTimeIntervalsForTerminal.ForEach(i => timeIntervalsUnion.AddRange(i.TimeIntervals));

            if (manager.IsOverlappingWithExistingDate(timeIntervalsUnion, newTimeIntervals))
                return false;

            terminalSequence.TimeIntervals = newTimeIntervals;
            terminalSequence.Setting = setting;
            return true;
        }



        public TerminalSequence GetCurrentScheduleForTerminal(Terminal terminal)
        {
            return GetCurrentSchedule(terminal.Id, DateTime.Now);
        }

        public TerminalSequence GetCurrentScheduleForTerminal(Terminal terminal, DateTime targetTime)
        {
            return GetCurrentSchedule(terminal.Id, targetTime);
        }

        public TerminalSequence GetCurrentScheduleForTerminal(int terminalId)
        {
            return GetCurrentSchedule(terminalId, DateTime.Now);
        }

        public TerminalSequence GetCurrentScheduleForTerminal(int terminalId, DateTime targetTime)
        {
            return GetCurrentSchedule(terminalId, targetTime);
        }

        private TerminalSequence GetCurrentSchedule(int terminalId, DateTime targetTime)
        {
            var times = GetScheduleForTerminal(terminalId);

            return (from time in times
                    let interval =
                        time.TimeIntervals.FirstOrDefault(i => i.TimeFrom <= targetTime && i.TimeTo >= targetTime)
                    where interval != null
                    select time).FirstOrDefault();
        }

        public List<TerminalSequence> GetScheduleForTerminal(int terminalId)
        {
            return null;
        }

        private List<TerminalSequence> GetSchedule(int terminalId,
            DateTime timeFrom,
            DateTime timeTo)
        {
            var times = GetScheduleForTerminal(terminalId);

            return
                times.Where(
                    time =>
                        time.TimeIntervals.Any(
                            t =>
                                ((t.TimeFrom <= timeFrom && t.TimeTo > timeFrom) ||
                                 (t.TimeFrom < timeTo && t.TimeTo >= timeTo) ||
                                 (t.TimeFrom > timeFrom && t.TimeTo < timeTo)))).ToList();
        }

        public List<TerminalSequence> GetScheduleForTerminal(Terminal terminal, DateTime timeFrom, DateTime timeTo)
        {
            return GetSchedule(terminal.Id, timeFrom, timeTo);
        }

        public List<TerminalSequence> GetScheduleForTerminal(int terminalId, DateTime timeFrom, DateTime timeTo)
        {
            return GetSchedule(terminalId, timeFrom, timeTo);
        }

        public List<TerminalSequence> GetScheduleForTerminal(Terminal terminal)
        {
            return GetScheduleForTerminal(terminal.Id);
        }

        public List<TerminalSequence> GetEntireHistoryForTerminal(Terminal terminal)
        {
            return null;
        }

        public List<TerminalSequence> GetEntireHistoryForTerminal(int terminalId)
        {
            return null;
        }

        public List<TerminalSequence> GetInactiveScheduleForTerminal(Terminal terminal)
        {
            return null;
        }

        public List<TerminalSequence> GetInactiveScheduleForTerminal(int terminalId)
        {
            return null;
        }
    }
}
