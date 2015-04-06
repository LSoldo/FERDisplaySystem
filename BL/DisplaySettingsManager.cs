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
            int? consecutiveTimesToShow,
            out DisplaySetting setting
            )
        {
            setting = CreateDisplaySetting(startTime, endTime, showEvery, consecutiveTimesToShow);
            var times = CreateDisplayTimes(setting);
            if (times == null) 
                throw new ArgumentNullException("times");
            CalculateDisplaySettingAdditionalValues(ref setting, times);
            return times;
        }

        public void CalculateDisplaySettingAdditionalValues(ref DisplaySetting setting, List<TimeInterval> times)
        {
            if (setting.ShowEvery.HasValue && (setting.ConsecutiveTimesToShow == null || setting.ConsecutiveTimesToShow == 0))
                setting.ValidUntil = DateTime.MaxValue;
            else
                setting.ValidUntil = times.Last().TimeTo;
        }

        private static void CheckTimeRules(DateTime startTime, DateTime endTime)
        {
            if (startTime == null || startTime < DateTime.Today)
                throw new Exception("Start time should be equal or later than now");

            if (endTime == null || endTime <= DateTime.Today)
                throw new Exception("Start time should be equal or later than now");

            if (endTime < startTime || endTime.Equals(startTime))
                throw new Exception("End time must be later than start time");
        }

        public DisplaySetting CreateDisplaySetting(
            DateTime startTime,
            DateTime endTime,
            TimeSpan? showEvery,
            int? consecutiveTimesToShow
            )
        {
            CheckTimeRules(startTime, endTime);

            var durationSpan = endTime - startTime;

            return new DisplaySetting
            {
                InsertionTs = DateTime.Now,
                StartTime = startTime,
                ShowEvery = showEvery,
                ConsecutiveTimesToShow = consecutiveTimesToShow,
                DurationSpan = durationSpan
            };
        }

        private static List<TimeInterval> CreateDisplayTimes(DisplaySetting setting)
        {
            var intervals = new List<TimeInterval>();

            //play once
            if ((!setting.ShowEvery.HasValue || setting.ShowEvery.Value == TimeSpan.Zero) &&
                (setting.ConsecutiveTimesToShow == null || setting.ConsecutiveTimesToShow == 0))
            {
                intervals.Add(new TimeInterval() {TimeFrom = setting.StartTime, TimeTo = setting.ValidUntil});
            }
            else if (setting.ConsecutiveTimesToShow.HasValue && setting.ConsecutiveTimesToShow.Value != 0)
            {
                intervals = CreateSchedule(setting, setting.ConsecutiveTimesToShow.Value);
            }
            //indefinite play
            else if (setting.ShowEvery.HasValue)
            {
                intervals = CreateSchedule(setting, DataDefinition.RepeatTimes.Month);
            }

            return intervals;
        }

        private static List<TimeInterval> CreateSchedule(DisplaySetting setting, int numberOfEvents)
        {
            var times = new List<TimeInterval>();
            var showEvery = !setting.ShowEvery.HasValue || setting.ShowEvery.Value == TimeSpan.Zero
                ? TimeSpan.Zero
                : setting.ShowEvery.Value;

            DateTime startTime = setting.StartTime;
            DateTime endTime = setting.StartTime.Add(setting.DurationSpan);
            for (var i = 0; i < numberOfEvents; i++)
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

    }
}
