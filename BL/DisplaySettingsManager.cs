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
        public DisplaySetting CreateNewDisplaySetting(List<Terminal> terminals, DateTime startTime, TimeSpan durationSpan, int? showEveryNthDay, int? consecutiveTimesToShow)
        {
            DisplaySetting displaySetting = new DisplaySetting();
            displaySetting.InsertionTS = DateTime.Now;

            if(terminals == null || terminals.Count == 0)
                throw new Exception("No terminals are inserted for this display setting");

            if(startTime == null || startTime < DateTime.Today  )
                throw new Exception("Start time should be equal or later than now");

            if(durationSpan == null || durationSpan < TimeSpan.FromMinutes(3))
                throw new Exception("Time span of this digital sign is smaller than 3 minutes or null");

            displaySetting.StartTime = startTime;

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
                    int nOfDays = (int)Math.Ceiling(durationSpan.TotalDays);

                    if (consecutiveTimesToShow == 0)
                        throw new Exception("Consecutive number of times displayed should be bigger than zero.");
                    else
                    {
                        displaySetting.ValidUntil =
                            displaySetting.StartTime.Add(durationSpan).Add(TimeSpan.FromDays((double)(consecutiveTimesToShow * nOfDays - nOfDays)));
                    }
                }
                    //periodic showing, not gonna end
                else
                {
                    displaySetting.ValidUntil = DateTime.MaxValue;
                }
            }

            foreach (var terminal in terminals)
            {
                CreateDisplayTimesForTerminal(terminal, displaySetting);
            }

            return displaySetting;
        }

        public List<ScheduledDisplayTime> CreateDisplayTimesForTerminal(Terminal terminal, DisplaySetting setting)
        {
            
        }
    }
}
