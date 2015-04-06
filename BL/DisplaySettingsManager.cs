using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Model;
using DAL.Interfaces;
using Newtonsoft.Json;

namespace BL
{
    public class DisplaySettingsManager
    {
        public List<TimeInterval> CreateDisplayTimes(
            DateTime startTime,
            DateTime endTime,
            TimeSpan? showEvery,
            int? consecutiveTimesToShow
            )
        {
            if (startTime == null || startTime < DateTime.Today)
                throw new Exception("Start time should be equal or later than now");

            if (endTime == null || endTime <= DateTime.Today)
                throw new Exception("Start time should be equal or later than now");

            if (endTime < startTime || endTime.Equals(startTime))
                throw new Exception("End time must be later than start time");

            TimeSpan durationSpan = endTime - startTime;

            DisplaySetting displaySetting = new DisplaySetting
            {
                InsertionTs = DateTime.Now,
                StartTime = startTime,
                ShowEvery = showEvery,
                ConsecutiveTimesToShow = consecutiveTimesToShow,
                DurationSpan = durationSpan
            };

            var times = CreateDisplayTimes(displaySetting);

            if (showEvery.HasValue && (consecutiveTimesToShow == null || consecutiveTimesToShow == 0))
                displaySetting.ValidUntil = DateTime.MaxValue;
            else
                displaySetting.ValidUntil = times.Last().TimeTo;

            return times;
        }

        public List<TimeInterval> CreateDisplayTimes(DisplaySetting setting)
        {
            var intervals = new List<TimeInterval>();

            //play once
            if ((!setting.ShowEvery.HasValue || setting.ShowEvery.Value == TimeSpan.Zero) &&
                (setting.ConsecutiveTimesToShow == null || setting.ConsecutiveTimesToShow == 0))
            {
                intervals.Add(new TimeInterval() {TimeFrom = setting.StartTime, TimeTo = setting.ValidUntil});
            }
            else if (setting.ConsecutiveTimesToShow != null && setting.ConsecutiveTimesToShow != 0)
            {
                intervals = CreateSchedule(setting, setting.ConsecutiveTimesToShow);
            }
            //indefinite play
            else if (setting.ShowEvery.HasValue)
            {
                intervals = CreateSchedule(setting, DataDefinition.RepeatTimes.Month);
            }

            return intervals;
        }

        public List<TimeInterval> CreateSchedule(DisplaySetting setting, int? numberOfEvents)
        {
            var times = new List<TimeInterval>();
            var showEvery = !setting.ShowEvery.HasValue || setting.ShowEvery.Value == TimeSpan.Zero
                ? TimeSpan.Zero
                : setting.ShowEvery.Value;

            DateTime startTime = setting.StartTime;
            DateTime endTime = setting.StartTime.Add(setting.DurationSpan);
            for (int i = 0; i < numberOfEvents; i++)
            {
                var multipliedSkippedTime = TimeSpan.FromTicks(i * showEvery.Ticks);
                var multipliedDuration = TimeSpan.FromTicks(i * setting.DurationSpan.Ticks);

                var start = startTime.Add(multipliedDuration + multipliedSkippedTime);
                var end = endTime.Add(multipliedDuration + multipliedSkippedTime);
                times.Add(new TimeInterval() {TimeFrom = start, TimeTo = end});
            }
            return times;
        }


        public bool IsOverlappingWithExistingDate(List<TimeInterval> scheduledTimes, DateTime newTime)
        {
            return scheduledTimes.Any(t => t.TimeFrom <= newTime && t.TimeTo >= newTime);
        }

        public bool IsOverlappingWithExistingDate(List<TimeInterval> scheduledTimes,
            TimeInterval newTimeToCheck)
        {
            return
                scheduledTimes.Any(
                    t =>
                        (t.TimeFrom <= newTimeToCheck.TimeFrom && t.TimeTo > newTimeToCheck.TimeFrom) ||
                        (t.TimeFrom < newTimeToCheck.TimeTo && t.TimeTo >= newTimeToCheck.TimeTo) ||
                        (t.TimeFrom > newTimeToCheck.TimeFrom && t.TimeTo < newTimeToCheck.TimeTo));
        }

        public bool IsOverlappingWithExistingDate(List<TimeInterval> scheduledTimes,
            List<TimeInterval> newTimesToCheck)
        {
            return newTimesToCheck.Any(newTime => IsOverlappingWithExistingDate(scheduledTimes, newTime));
        }

        public string ConvertScheduledTimesToJson(List<TimeInterval> times)
        {
            return JsonConvert.SerializeObject(times);
        }

        public ISequence GetCurrentScheduleForTerminal(Terminal terminal)
        {
            return GetCurrentSchedule(terminal.Id, DateTime.Now);
        }

        public ISequence GetCurrentScheduleForTerminal(Terminal terminal, DateTime targetTime)
        {
            return GetCurrentSchedule(terminal.Id, targetTime);
        }

        public ISequence GetCurrentScheduleForTerminal(int terminalId)
        {
            return GetCurrentSchedule(terminalId, DateTime.Now);
        }

        public ISequence GetCurrentScheduleForTerminal(int terminalId, DateTime targetTime)
        {
            return GetCurrentSchedule(terminalId, targetTime);
        }

        private ISequence GetCurrentSchedule(int terminalId, DateTime targetTime)
        {
            var times = GetScheduleForTerminal(terminalId);

            return (from time in times
                let interval =
                    time.TimeIntervals.FirstOrDefault(i => i.TimeFrom <= targetTime && i.TimeTo >= targetTime)
                where interval != null
                select time).FirstOrDefault();
        }

        public List<ISequence> GetScheduleForTerminal(int terminalId)
        {
            return null;
        }

        private List<ISequence> GetSchedule(int terminalId,
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

        public List<ISequence> GetScheduleForTerminal(Terminal terminal, DateTime timeFrom, DateTime timeTo)
        {
            return GetSchedule(terminal.Id, timeFrom, timeTo);
        }

        public List<ISequence> GetScheduleForTerminal(int terminalId, DateTime timeFrom, DateTime timeTo)
        {
            return GetSchedule(terminalId, timeFrom, timeTo);
        }

        public List<ISequence> GetScheduleForTerminal(Terminal terminal)
        {
            return GetScheduleForTerminal(terminal.Id);
        }

        public List<ISequence> GetEntireHistoryForTerminal(Terminal terminal)
        {
            return null;
        }

        public List<ISequence> GetEntireHistoryForTerminal(int terminalId)
        {
            return null;
        }

        public List<ISequence> GetInactiveScheduleForTerminal(Terminal terminal)
        {
            return null;
        }

        public List<ISequence> GetInactiveScheduleForTerminal(int terminalId)
        {
            return null;
        }

    }
}
