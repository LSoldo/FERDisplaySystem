﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;
using DAL.Model;

namespace BL
{
    public class BLTerminalManager
    {
        /// <summary>
        /// Recalculates display times for specified terminal and checks if new times overlap with existing ones.
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="terminalSequence"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="showEvery"></param>
        /// <param name="consecutiveTimesToShow"></param>
        /// <returns>Returns true if new calculated times don't overlap with existing ones, and false if they do.</returns>
        public bool RecalculateDisplayTimesForTerminal(Terminal terminal,
            TerminalSequence terminalSequence,
            DateTime startTime,
            DateTime endTime,
            TimeSpan? showEvery,
            int? consecutiveTimesToShow)
        {
            var manager = new BLDisplaySettingsManager();

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
        /// <summary>
        /// Updates terminal with new sequence (if it changes) and returns current sequence.
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="targetTime"></param>
        /// <returns>Returns currenct sequence</returns>
        public TerminalSequence UpdateTerminalSequenceAndGetCurrentSequence(Terminal terminal, DateTime targetTime)
        {
            var manager = new BLDisplaySettingsManager();

            //default case
            if (terminal.CurrentSequenceValidFromToInterval == null && terminal.CurrentSequence == null &&
                terminal.ManualSequence == null)
            {
                var newCurrentSequence = GetCurrentScheduleForTerminal(terminal, targetTime);

                if (newCurrentSequence == null)
                    return terminal.DefaultSequence;

                terminal.CurrentSequence = newCurrentSequence;
                terminal.CurrentSequenceValidFromToInterval = GetCurrentTimeIntervalForSequence(newCurrentSequence,
                    targetTime);
                terminal.CurrentSequence.CurrentType = DataDefinition.CurrentSequence.ScheduledSequence;
                return newCurrentSequence;
            }

            //simply get new sequence if we're out of time interval, it doesn't matter what it is
            if ((terminal.CurrentSequenceValidFromToInterval != null &&
                 !manager.IsOverlappingWithExistingDate(terminal.CurrentSequenceValidFromToInterval, targetTime)))
            {
                terminal.ManualSequence = null;
                var newCurrentSequence = GetCurrentScheduleForTerminal(terminal, targetTime);

                if (newCurrentSequence == null)
                {
                    terminal.CurrentSequence = null;
                    terminal.CurrentSequenceValidFromToInterval = null;
                    return terminal.DefaultSequence;
                }
                terminal.CurrentSequence = newCurrentSequence;
                terminal.CurrentSequenceValidFromToInterval = GetCurrentTimeIntervalForSequence(newCurrentSequence,
                    targetTime);
                terminal.CurrentSequence.CurrentType = DataDefinition.CurrentSequence.ScheduledSequence;
                return newCurrentSequence;
            }
            else if (terminal.ManualSequence != null)
                return terminal.CurrentSequence = terminal.ManualSequence;
            return terminal.CurrentSequence;
        }
        /// <summary>
        /// Overrides current sequence and sets manual sequence
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="manualSequence"></param>
        /// <param name="showFromToInterval"></param>
        public void SetManualSequenceForTerminal(Terminal terminal, TerminalSequence manualSequence, TimeInterval showFromToInterval = null)
        {
            terminal.CurrentSequenceValidFromToInterval = null;
            if (showFromToInterval != null)
            {
                showFromToInterval.TimeFrom = DateTime.Now;
                if (showFromToInterval.TimeFrom >= showFromToInterval.TimeTo)
                    throw new Exception("'Time to' must be placed somewhere in the future");
                terminal.CurrentSequenceValidFromToInterval = showFromToInterval;
            }
            terminal.ManualSequence = manualSequence;
            terminal.CurrentSequence = null;
            //TODO ORM save
            //TODO update group SignalR
        }
        /// <summary>
        /// Sets default sequence for terminal
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="defaultSequence"></param>
        public void SetDefaultSequenceForTerminal(Terminal terminal, TerminalSequence defaultSequence)
        {
            terminal.DefaultSequence = defaultSequence;
            //TODO ORM
        }
        /// <summary>
        /// Adds sequence to terminal
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="sequence"></param>
        public void AddSequenceToTerminal(Terminal terminal, TerminalSequence sequence)
        {
            terminal.AllSequences.Add(sequence);
            //TODO ORM
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

            //default case
            if (terminal.CurrentSequenceValidFromToInterval == null && 
                terminal.CurrentSequence == null &&
                terminal.ManualSequence == null)
                return GetCurrentScheduleForTerminal(terminal, targetTime) ?? terminal.DefaultSequence;

            //simply get new sequence if we're out of time interval, it doesn't matter what it is
            if ((terminal.CurrentSequenceValidFromToInterval != null &&
                 !manager.IsOverlappingWithExistingDate(terminal.CurrentSequenceValidFromToInterval, targetTime)))
                return GetCurrentScheduleForTerminal(terminal, targetTime) ?? terminal.DefaultSequence;

            return terminal.ManualSequence ?? terminal.CurrentSequence;
        }

        public TimeInterval GetCurrentTimeIntervalForSequence(TerminalSequence sequence, DateTime targetTime)
        {
            return
                sequence.TimeIntervals.FirstOrDefault(t => t.TimeFrom <= targetTime && targetTime <= t.TimeTo);
        }
        public TerminalSequence GetCurrentScheduleForTerminal(Terminal terminal)
        {
            return GetCurrentSchedule(terminal.Id, DateTime.Now);
        }

        public TerminalSequence GetCurrentScheduleForTerminal(Terminal terminal, DateTime targetTime)
        {
            var manager = new BLDisplaySettingsManager();
            var sequences = terminal.AllSequences;

            foreach (var sequence in sequences)
            {
                if (manager.IsOverlappingWithExistingDate(sequence.TimeIntervals, targetTime))
                    return sequence;
            }
            return null;
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
            var sequences = GetScheduleForTerminal(terminalId);

            return (from sequence in sequences
                    let interval =
                        sequence.TimeIntervals.FirstOrDefault(i => i.TimeFrom <= targetTime && i.TimeTo >= targetTime)
                    where interval != null
                    select sequence).FirstOrDefault();
        }

        public List<TerminalSequence> GetScheduleForTerminal(int terminalId)
        {
            return null;
        }

        private List<TerminalSequence> GetSchedule(int terminalId,
            DateTime timeFrom,
            DateTime timeTo)
        {
            var sequences = GetScheduleForTerminal(terminalId);

            return
                sequences.Where(
                    sequence =>
                        sequence.TimeIntervals.Any(
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
