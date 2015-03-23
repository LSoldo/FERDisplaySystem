using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Model;
using DAL;
using Newtonsoft.Json;

namespace BL
{
    public class DisplaySettingsManager:IDisposable
    {
        private DalScheduledDisplayTime dalSDT;

        public DisplaySettingsManager()
        {
            this.dalSDT = new DalScheduledDisplayTime();
        }
        public List<ScheduledDisplayTime> CreateDisplayTimes(List<Terminal> terminals,
            DateTime startTime,
            TimeSpan durationSpan,
            TimeSpan? showEvery,
            int? consecutiveTimesToShow,
            DigitalSign sign,
            string name
            )
        {
            DisplaySetting displaySetting = new DisplaySetting();
            displaySetting.InsertionTS = DateTime.Now;
            displaySetting.StartTime = startTime;
            displaySetting.ShowEvery = showEvery;
            displaySetting.ConsecutiveTimesToShow = consecutiveTimesToShow;
            displaySetting.Name = name;
            displaySetting.DurationSpan = durationSpan;

            if (terminals == null || terminals.Count == 0)
                throw new Exception("No terminal are inserted for this display setting");

            if (sign == null)
                throw new Exception("Digital content for this terminal should not be null");

            if (startTime == null || startTime < DateTime.Today)
                throw new Exception("Start time should be equal or later than now");

            if (durationSpan == null || durationSpan < TimeSpan.FromMinutes(3))
                throw new Exception("Time span of this digital sign is smaller than 3 minutes or null");

            var times = CreateDisplayTimes(terminals, displaySetting, sign);

            if (showEvery.HasValue && (consecutiveTimesToShow == null || consecutiveTimesToShow == 0))
                displaySetting.ValidUntil = DateTime.MaxValue;
            else
                displaySetting.ValidUntil = times.First().TimeIntervals.Last().EndTime;

            try
            {
                foreach (var time in times)
                    dalSDT.Add(time);
                return times;
            }
            catch (Exception)
            {
                throw new Exception(
                    "DisplaySettingsManager: CreateDisplayTimes: Error while inserting scheduled display times");
            }
        }

        private List<ScheduledDisplayTime> CreateDisplayTimes(List<Terminal> terminals, DisplaySetting setting, DigitalSign sign)
        {
            List<ScheduledDisplayTime> times = new List<ScheduledDisplayTime>();
            List<TimeInterval> intervals = new List<TimeInterval>();
        
            //play once
            if ((!setting.ShowEvery.HasValue || setting.ShowEvery.Value == TimeSpan.Zero) && (setting.ConsecutiveTimesToShow == null || setting.ConsecutiveTimesToShow == 0))
            {
                intervals.Add(new TimeInterval(){StartTime = setting.StartTime, EndTime = setting.ValidUntil});
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

            foreach (var terminal in terminals)
            {
                ScheduledDisplayTime sdt = new ScheduledDisplayTime();
                sdt = MapDisplaySettingToDisplayTime(terminal, setting, sign);
                sdt.TimeIntervals = intervals;
                times.Add(sdt);
            }

            return times;
        }

        private List<TimeInterval> CreateSchedule(DisplaySetting setting, int? numberOfEvents)
        {
            List<TimeInterval> times = new List<TimeInterval>();
            TimeSpan showEvery = !setting.ShowEvery.HasValue || setting.ShowEvery.Value == TimeSpan.Zero
                ? TimeSpan.Zero
                : setting.ShowEvery.Value;

            DateTime startTime = setting.StartTime;
            DateTime endTime = setting.StartTime.Add(setting.DurationSpan);
            for (int i = 0; i < numberOfEvents; i++)
            {
                //offset is defined because adding days while we have overflow to other day is not the same as when we don't!
                TimeSpan multipliedSkippedTime = TimeSpan.FromTicks(i*showEvery.Ticks);
                TimeSpan multipliedDuration = TimeSpan.FromTicks(i*setting.DurationSpan.Ticks);

                DateTime start = startTime.Add(multipliedDuration + multipliedSkippedTime);
                DateTime end = endTime.Add(multipliedDuration + multipliedSkippedTime);
                times.Add(new TimeInterval(){StartTime = start, EndTime = end});
            }
            return times;
        }

        private ScheduledDisplayTime MapDisplaySettingToDisplayTime(Terminal terminal, DisplaySetting setting, DigitalSign sign)
        {
            ScheduledDisplayTime time = new ScheduledDisplayTime();
            time.Name = setting.Name;
            time.Terminal = terminal;
            time.DisplaySetting = setting;
            time.DigitalSign = sign;           
            time.Active = true;
            
            return time;
        }

        public bool IsOverlappingWithExistingDate(List<TimeInterval> scheduledTimes, DateTime newTime)
        {
            return scheduledTimes.Any(t => t.StartTime <= newTime && t.EndTime >= newTime);
        }

        public bool IsOverlappingWithExistingDate(List<TimeInterval> scheduledTimes,
            TimeInterval newTimeToCheck)
        {
            return
                scheduledTimes.Any(
                    t =>
                        (t.StartTime <= newTimeToCheck.StartTime && t.EndTime > newTimeToCheck.StartTime) ||
                        (t.StartTime < newTimeToCheck.EndTime && t.EndTime >= newTimeToCheck.EndTime) ||
                        (t.StartTime > newTimeToCheck.StartTime && t.EndTime < newTimeToCheck.EndTime));
        }

        public bool IsOverlappingWithExistingDate(List<TimeInterval> scheduledTimes,
            List<TimeInterval> newTimesToCheck)
        {
            foreach (var newTime in newTimesToCheck)
            {
                if (IsOverlappingWithExistingDate(scheduledTimes, newTime))
                    return true;               
            }
            return false;
        }

        public string ConvertScheduledTimesToJson(List<TimeInterval> times)
        {
            return JsonConvert.SerializeObject(times);
        }

        public ScheduledDisplayTime GetCurrentScheduleForTerminal(Terminal terminal)
        {
            return GetScheduleWrapper(terminal.Id, DateTime.Now);
        }

        public ScheduledDisplayTime GetCurrentScheduleForTerminal(Terminal terminal, DateTime targetTime)
        {
            return GetScheduleWrapper(terminal.Id, targetTime);
        }

        public ScheduledDisplayTime GetCurrentScheduleForTerminal(int terminalId)
        {
            return GetScheduleWrapper(terminalId, DateTime.Now);
        }

        public ScheduledDisplayTime GetCurrentScheduleForTerminal(int terminalId, DateTime targetTime)
        {
            return GetScheduleWrapper(terminalId, targetTime);
        }

        private ScheduledDisplayTime GetScheduleWrapper(int terminalId, DateTime targetTime)
        {
            List<ScheduledDisplayTime> times = GetScheduleForTerminal(terminalId);

            foreach (var time in times)
            {
                TimeInterval interval = time.TimeIntervals.FirstOrDefault(
                    i => i.StartTime <= targetTime &&
                         i.EndTime >= targetTime);
                if (interval != null) return time;
            }
            return null;
        }

        public List<ScheduledDisplayTime> GetScheduleForTerminal(int terminalId)
        {
            return dalSDT.FetchByTerminalActive(terminalId);
        }

        private List<ScheduledDisplayTime> GetScheduleForTerminalWrapper(int terminalId,
            DateTime timeFrom,
            DateTime timeTo)
        {
            List<ScheduledDisplayTime> foundTimes = new List<ScheduledDisplayTime>();
            List<ScheduledDisplayTime> times = GetScheduleForTerminal(terminalId);

            foreach (var time in times)
            {
                if(time.TimeIntervals.Any(
                    t =>
                        ((t.StartTime <= timeFrom && t.EndTime > timeFrom) ||
                        (t.StartTime < timeTo && t.EndTime >= timeTo) ||
                        (t.StartTime > timeFrom && t.EndTime < timeTo))))
                    foundTimes.Add(time);
            }

            return foundTimes;
        }

        public List<ScheduledDisplayTime> GetScheduleForTerminal(Terminal terminal, DateTime timeFrom, DateTime timeTo)
        {
            return GetScheduleForTerminalWrapper(terminal.Id, timeFrom, timeTo);
        }

        public List<ScheduledDisplayTime> GetScheduleForTerminal(int terminalId, DateTime timeFrom, DateTime timeTo)
        {
            return GetScheduleForTerminalWrapper(terminalId, timeFrom, timeTo);
        }

        public List<ScheduledDisplayTime> GetScheduleForTerminal(Terminal terminal)
        {
            return GetScheduleForTerminal(terminal.Id);
        }

        public List<ScheduledDisplayTime> GetEntireHistoryForTerminal(Terminal terminal)
        {
            return dalSDT.FetchByTerminal(terminal.Id);
        }

        public List<ScheduledDisplayTime> GetEntireHistoryForTerminal(int terminalId)
        {
            return dalSDT.FetchByTerminal(terminalId);
        }

        public List<ScheduledDisplayTime> GetInactiveScheduleForTerminal(Terminal terminal)
        {
            return dalSDT.FetchByTerminalInactive(terminal.Id);
        }

        public List<ScheduledDisplayTime> GetInactiveScheduleForTerminal(int terminalId)
        {
            return dalSDT.FetchByTerminalInactive(terminalId);
        }

        public void Dispose()
        {
            if(dalSDT != null)
                dalSDT.Dispose();
        }


    }
}
