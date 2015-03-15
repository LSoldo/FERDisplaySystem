using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Model;
using DAL;
using Newtonsoft.Json;
using System.Linq;

namespace BL
{
    public class DisplaySettingsManager
    {
        public List<ScheduledDisplayTime> CreateDisplayTimes(Terminal terminal,
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

            if (terminal == null)
                throw new Exception("No terminal are inserted for this display setting");

            if (sign == null)
                throw new Exception("Digital content for this terminal should not be null");

            if (startTime == null || startTime < DateTime.Today)
                throw new Exception("Start time should be equal or later than now");

            if (durationSpan == null || durationSpan < TimeSpan.FromMinutes(3))
                throw new Exception("Time span of this digital sign is smaller than 3 minutes or null");

            var times = CreateDisplayTimes(terminal, displaySetting, sign);

            if (showEvery.HasValue && (consecutiveTimesToShow == null || consecutiveTimesToShow == 0))
                displaySetting.ValidUntil = DateTime.MaxValue;
            else
                displaySetting.ValidUntil = times.Last().EndTime;

            return times;
        }

        private List<ScheduledDisplayTime> CreateDisplayTimes(Terminal terminal, DisplaySetting setting, DigitalSign sign)
        {
            List<ScheduledDisplayTime> times = new List<ScheduledDisplayTime>();

            int nOfDays = (int)Math.Ceiling(setting.DurationSpan.TotalDays);
            
            //play once
            if ((!setting.ShowEvery.HasValue || setting.ShowEvery.Value == TimeSpan.Zero) && (setting.ConsecutiveTimesToShow == null || setting.ConsecutiveTimesToShow == 0))
            {
                times.Add(MapDisplaySettingToDisplayTime(terminal, setting, sign, setting.StartTime, setting.ValidUntil, setting.ShowEvery));
            }
            else if (setting.ConsecutiveTimesToShow != null && setting.ConsecutiveTimesToShow != 0)
            {
                times = CreateSchedule(terminal, setting, sign, setting.ConsecutiveTimesToShow);
            }
                //indefinite play
            else if (setting.ShowEvery.HasValue)
            {
                times = CreateSchedule(terminal, setting, sign, DataDefinition.RepeatTimes.Month);
            }

            return times;
        }

        private List<ScheduledDisplayTime> CreateSchedule(Terminal terminal, DisplaySetting setting, DigitalSign sign, int? numberOfEvents)
        {
            List<ScheduledDisplayTime> times = new List<ScheduledDisplayTime>();
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
                times.Add(MapDisplaySettingToDisplayTime(terminal, setting, sign, start, end, null));
            }
            return times;
        }

        private ScheduledDisplayTime MapDisplaySettingToDisplayTime(Terminal terminal, DisplaySetting setting, DigitalSign sign, DateTime startTime, DateTime endTime, TimeSpan? indefiniteRun)
        {
            ScheduledDisplayTime time = new ScheduledDisplayTime();            
            time.Terminal = terminal;
            time.DisplaySetting = setting;
            time.DigitalSign = sign;           
            time.StartTime = startTime;
            time.EndTime = endTime;
            time.IndefiniteRunEvery = indefiniteRun;
            time.Active = true;
            time.Name = setting.Name;

            return time;
        }

        public bool IsOverlappingWithExistingDate(List<ScheduledDisplayTime> scheduledTimes, DateTime newTime)
        {
            return scheduledTimes.Any(t => t.StartTime <= newTime && t.EndTime >= newTime);
        }

        public bool IsOverlappingWithExistingDate(List<ScheduledDisplayTime> scheduledTimes,
            ScheduledDisplayTime newTimeToCheck)
        {
            return
                scheduledTimes.Any(
                    t =>
                        (t.StartTime <= newTimeToCheck.StartTime && t.EndTime > newTimeToCheck.StartTime) ||
                        (t.StartTime < newTimeToCheck.EndTime && t.EndTime >= newTimeToCheck.EndTime) ||
                        (t.StartTime > newTimeToCheck.StartTime && t.EndTime < newTimeToCheck.EndTime));
        }

        public bool IsOverlappingWithExistingDate(List<ScheduledDisplayTime> scheduledTimes,
            List<ScheduledDisplayTime> newTimesToCheck)
        {
            foreach (var newTime in newTimesToCheck)
            {
                if (IsOverlappingWithExistingDate(scheduledTimes, newTime))
                    return true;               
            }
            return false;
        }

        public string ConvertScheduledTimesToJson(List<ScheduledDisplayTime> times)
        {
            return JsonConvert.SerializeObject(times);
        }

        public ScheduledDisplayTime GetCurrentScheduleForTerminal(Terminal terminal)
        {
            //TODO ORM
            List<ScheduledDisplayTime> times = new List<ScheduledDisplayTime>();

            return
                times.FirstOrDefault(
                    t => t.Active && 
                        t.Terminal == terminal && 
                        t.StartTime <= DateTime.Now && 
                        t.EndTime >= DateTime.Now);
        }

        public ScheduledDisplayTime GetCurrentScheduleForTerminal(Terminal terminal, DateTime time)
        {
            //TODO ORM
            List<ScheduledDisplayTime> times = new List<ScheduledDisplayTime>();

            return
                times.FirstOrDefault(
                    t => t.Active &&
                        t.Terminal == terminal &&
                        t.StartTime <= time &&
                        t.EndTime >= time);
        }

        public ScheduledDisplayTime GetCurrentScheduleForTerminal(int terminalId)
        {
            //TODO ORM
            List<ScheduledDisplayTime> times = new List<ScheduledDisplayTime>();

            return
                times.FirstOrDefault(
                    t => t.Active &&
                        t.Terminal.Id == terminalId &&
                        t.StartTime <= DateTime.Now &&
                        t.EndTime >= DateTime.Now);
        }

        public ScheduledDisplayTime GetCurrentScheduleForTerminal(int terminalId, DateTime time)
        {
            //TODO ORM
            List<ScheduledDisplayTime> times = new List<ScheduledDisplayTime>();

            return
                times.FirstOrDefault(
                    t => t.Active &&
                        t.Terminal.Id == terminalId &&
                        t.StartTime <= time &&
                        t.EndTime >= time);
        }

        public List<ScheduledDisplayTime> GetScheduleForTerminal(int terminalId)
        {
            //TODO ORM
            List<ScheduledDisplayTime> times = new List<ScheduledDisplayTime>();

            return
                times.Where(
                    t => t.Active &&
                        t.Terminal.Id == terminalId)
                        .ToList();
        }

        public List<ScheduledDisplayTime> GetScheduleForTerminal(Terminal terminal, DateTime timeFrom, DateTime timeTo)
        {
            //TODO ORM
            List<ScheduledDisplayTime> times = new List<ScheduledDisplayTime>();

            return
                times.Where(
                    t =>
                        ((t.StartTime <= timeFrom && t.EndTime > timeFrom) ||
                        (t.StartTime < timeTo && t.EndTime >= timeTo) ||
                        (t.StartTime > timeFrom && t.EndTime < timeTo)) &&
                        t.Active &&
                        t.Terminal == terminal).ToList();
        }

        public List<ScheduledDisplayTime> GetScheduleForTerminal(int terminalId, DateTime timeFrom, DateTime timeTo)
        {
            //TODO ORM
            List<ScheduledDisplayTime> times = new List<ScheduledDisplayTime>();

            return
                times.Where(
                    t =>
                        ((t.StartTime <= timeFrom && t.EndTime > timeFrom) ||
                        (t.StartTime < timeTo && t.EndTime >= timeTo) ||
                        (t.StartTime > timeFrom && t.EndTime < timeTo)) &&
                        t.Active &&
                        t.Terminal.Id == terminalId).ToList();
        }

        public List<ScheduledDisplayTime> GetScheduleForTerminal(Terminal terminal)
        {
            //TODO ORM
            List<ScheduledDisplayTime> times = new List<ScheduledDisplayTime>();

            return
                times.Where(
                    t => t.Active &&
                        t.Terminal == terminal)
                        .ToList();
        }

        public List<ScheduledDisplayTime> GetEntireHistoryForTerminal(Terminal terminal)
        {
            //TODO ORM
            List<ScheduledDisplayTime> times = new List<ScheduledDisplayTime>();

            return
                times.Where(
                    t => t.Terminal == terminal)
                    .ToList();
        }

        public List<ScheduledDisplayTime> GetEntireHistoryForTerminal(int terminalId)
        {
            //TODO ORM
            List<ScheduledDisplayTime> times = new List<ScheduledDisplayTime>();

            return
                times.Where(
                    t => t.Terminal.Id == terminalId)
                    .ToList();
        }

        public List<ScheduledDisplayTime> GetInactiveScheduleForTerminal(Terminal terminal)
        {
            //TODO ORM
            List<ScheduledDisplayTime> times = new List<ScheduledDisplayTime>();

            return
                times.Where(
                    t => !t.Active && t.Terminal == terminal)
                    .ToList();
        }

        public List<ScheduledDisplayTime> GetInactiveScheduleForTerminal(int terminalId)
        {
            //TODO ORM
            List<ScheduledDisplayTime> times = new List<ScheduledDisplayTime>();

            return
                times.Where(
                    t => !t.Active && t.Terminal.Id == terminalId)
                    .ToList();
        }


    }
}
