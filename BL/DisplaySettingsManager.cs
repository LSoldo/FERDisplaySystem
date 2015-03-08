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
        public DisplaySetting CreateDisplaySettingForDigitalSign(List<Terminal> terminals,
            DateTime startTime,
            TimeSpan durationSpan,
            int? showEveryNthDay,
            int? consecutiveTimesToShow,
            DigitalSign sign)
        {
            DisplaySetting displaySetting = new DisplaySetting();
            
            if (terminals == null || terminals.Count == 0)
                throw new Exception("No terminals are inserted for this display setting");

            if (sign == null)
                throw new Exception("Digital content for this terminal should not be null");

            if (startTime == null || startTime < DateTime.Today)
                throw new Exception("Start time should be equal or later than now");

            if (durationSpan == null || durationSpan < TimeSpan.FromMinutes(3))
                throw new Exception("Time span of this digital sign is smaller than 3 minutes or null");


            //
            if (showEveryNthDay == null && consecutiveTimesToShow == null)
            {
                displaySetting.ValidUntil = displaySetting.StartTime.Add(durationSpan);
            }
            else
            {
                //just show content for consecutive number of days
                if (showEveryNthDay == null)
                {
                    int nOfDays = (int) Math.Ceiling(durationSpan.TotalDays);

                    if (consecutiveTimesToShow == 0)
                        throw new Exception("Consecutive number of times displayed should be bigger than zero.");
                    else
                    {
                        displaySetting.ValidUntil =
                            displaySetting.StartTime.Add(durationSpan)
                                .Add(TimeSpan.FromDays((double) (consecutiveTimesToShow*nOfDays - nOfDays)));
                    }
                }
                //periodic showing, not gonna end
                else
                {
                    displaySetting.ValidUntil = DateTime.MaxValue;
                }
            }

            displaySetting.InsertionTS = DateTime.Now;
            displaySetting.StartTime = startTime;
            displaySetting.ShowEveryNthDay = showEveryNthDay;
            displaySetting.ConsecutiveTimesToShow = consecutiveTimesToShow;
            displaySetting.DurationSpan = durationSpan;

            CreateDisplayTimesForTerminal(terminals, displaySetting, sign);

            return displaySetting;
        }

        public List<ScheduledDisplayTime> CreateDisplayTimesForTerminal(List<Terminal> terminals, DisplaySetting setting, DigitalSign sign)
        {
            List<ScheduledDisplayTime> times = new List<ScheduledDisplayTime>();
            
            //play once
            if (setting.ShowEveryNthDay == null && setting.ConsecutiveTimesToShow == null)
            {
                times.Add(MapDisplaySettingToDisplayTime(terminals, setting, sign, setting.StartTime, setting.ValidUntil, setting.ShowEveryNthDay));
            }
            else if (setting.ShowEveryNthDay != null)
            {
                times.Add(MapDisplaySettingToDisplayTime(terminals, setting, sign, setting.StartTime, setting.StartTime.Add(setting.DurationSpan), setting.ShowEveryNthDay));
            }
            else if (setting.ConsecutiveTimesToShow != null)
            {
                for (int i = 0; i < setting.ConsecutiveTimesToShow; i++)
                {
                    
                }
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
    }
}
