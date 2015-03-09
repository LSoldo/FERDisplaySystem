using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Model;
using DAL;

namespace BL
{
    public class DisplaySettingsManager
    {
        public List<ScheduledDisplayTime> CreateDisplayTimes(List<Terminal> terminals,
            DateTime startTime,
            TimeSpan durationSpan,
            int? showEveryNthDay,
            int? consecutiveTimesToShow,
            DigitalSign sign)
        {
            DisplaySetting displaySetting = new DisplaySetting();
            displaySetting.InsertionTS = DateTime.Now;
            displaySetting.StartTime = startTime;
            displaySetting.ShowEveryNthDay = showEveryNthDay;
            displaySetting.ConsecutiveTimesToShow = consecutiveTimesToShow;
            displaySetting.DurationSpan = durationSpan;

            if (terminals == null || terminals.Count == 0)
                throw new Exception("No terminals are inserted for this display setting");

            if (sign == null)
                throw new Exception("Digital content for this terminal should not be null");

            if (startTime == null || startTime < DateTime.Today)
                throw new Exception("Start time should be equal or later than now");

            if (durationSpan == null || durationSpan < TimeSpan.FromMinutes(3))
                throw new Exception("Time span of this digital sign is smaller than 3 minutes or null");

            if (consecutiveTimesToShow == 0)
                throw new Exception("Consecutive number of times displayed should be bigger than zero.");

            var times = CreateDisplayTimes(terminals, displaySetting, sign);

            if (showEveryNthDay != null && (consecutiveTimesToShow == null || consecutiveTimesToShow == 0))
                displaySetting.ValidUntil = DateTime.MaxValue;
            else
                displaySetting.ValidUntil = times.Last().EndTime;

            return times;
        }

        private List<ScheduledDisplayTime> CreateDisplayTimes(List<Terminal> terminals, DisplaySetting setting, DigitalSign sign)
        {
            List<ScheduledDisplayTime> times = new List<ScheduledDisplayTime>();

            int nOfDays = (int)Math.Ceiling(setting.DurationSpan.TotalDays);
            
            //play once
            if (setting.ShowEveryNthDay == null && setting.ConsecutiveTimesToShow == null)
            {
                times.Add(MapDisplaySettingToDisplayTime(terminals, setting, sign, setting.StartTime, setting.ValidUntil, setting.ShowEveryNthDay));
            }
            else if (setting.ConsecutiveTimesToShow != null)
            {
                int showEveryNthDay = setting.ShowEveryNthDay == null || setting.ShowEveryNthDay == 0 ? 0 : (int)setting.ShowEveryNthDay;

                DateTime startTime = setting.StartTime;
                for (int i = 0; i < setting.ConsecutiveTimesToShow; i++)
                {
                    //offset is defined because adding days while we have overflow to other day is not the same as when we don't!
                    int offset = i == 0 || showEveryNthDay == 0 ? 0 : startTime.Date != startTime.Add(setting.DurationSpan).Date ? 0 : -1;

                    DateTime start = startTime.AddDays((double)((i * nOfDays) + i * showEveryNthDay)).AddDays(offset);
                    DateTime end = startTime.Add(setting.DurationSpan).AddDays((double)((i * nOfDays) + i * showEveryNthDay)).AddDays(offset);
                    times.Add(MapDisplaySettingToDisplayTime(terminals, setting, sign, start, end, null));                  
                }
            }
            else if (setting.ShowEveryNthDay != null )
            {
                times.Add(MapDisplaySettingToDisplayTime(terminals, setting, sign, setting.StartTime, setting.StartTime.Add(setting.DurationSpan), setting.ShowEveryNthDay));
            }

            return times;
        }

        private ScheduledDisplayTime MapDisplaySettingToDisplayTime(List<Terminal> terminals, DisplaySetting setting, DigitalSign sign, DateTime startTime, DateTime endTime, int? indefiniteRun)
        {
            ScheduledDisplayTime time = new ScheduledDisplayTime();            
            time.Terminals = terminals;
            time.DisplaySetting = setting;
            time.DigitalSign = sign;           
            time.StartTime = startTime;
            time.EndTime = endTime;
            time.IndefiniteRunEveryNDays = indefiniteRun;
            time.Active = true;

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
                        (t.StartTime <= newTimeToCheck.StartTime && t.EndTime >= newTimeToCheck.StartTime) ||
                        (t.StartTime <= newTimeToCheck.EndTime && t.EndTime >= newTimeToCheck.EndTime) ||
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
    }
}
