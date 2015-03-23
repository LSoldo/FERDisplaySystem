using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Model;

namespace DAL
{
    public class DalScheduledDisplayTime : IDisposable
    {
        private DBContext context;

        public DalScheduledDisplayTime()
        {
            this.context = new DBContext();
        }

        public int Add(ScheduledDisplayTime sdt)
        {
            this.context.ScheduledDisplayTimes.Add(sdt);
            this.context.SaveChanges();
            return sdt.Id;
        }

        public void Update(ScheduledDisplayTime newScheduledDisplayTime)
        {
            ScheduledDisplayTime oldScheduledDisplayTime = this.context.ScheduledDisplayTimes.Find(newScheduledDisplayTime.Id);

            if (oldScheduledDisplayTime == null)
                throw new Exception("Could not find existing scheduled display time in the database. ID possibly modified.");

            this.context.Entry(oldScheduledDisplayTime).CurrentValues.SetValues(newScheduledDisplayTime);
            this.context.Entry(oldScheduledDisplayTime.Terminal).CurrentValues.SetValues(newScheduledDisplayTime.Terminal);
            this.context.Entry(oldScheduledDisplayTime.TimeIntervals).CurrentValues.SetValues(newScheduledDisplayTime.TimeIntervals);
            this.context.Entry(oldScheduledDisplayTime.DigitalSign).CurrentValues.SetValues(newScheduledDisplayTime.DigitalSign);
            this.context.Entry(oldScheduledDisplayTime.DisplaySetting).CurrentValues.SetValues(newScheduledDisplayTime.DisplaySetting);
            this.context.SaveChanges();
        }

        public void SetSDTAsInactive(int sdtId)
        {
            ScheduledDisplayTime sdt = this.context.ScheduledDisplayTimes.Find(sdtId);
            if (sdt != null)
                sdt.Active = false;
            this.context.SaveChanges();
        }

        public void UpdateTimeIntervals(int sdtId, List<TimeInterval> timeIntervals)
        {
            ScheduledDisplayTime sdt = this.context.ScheduledDisplayTimes.Find(sdtId);
            if (sdt != null)
                sdt.TimeIntervals = timeIntervals;
            this.context.SaveChanges();
        }

        public void Delete(int id)
        {
            ScheduledDisplayTime sdt = this.context.ScheduledDisplayTimes.Find(id);
            if (sdt != null)
            {
                if(sdt.DisplaySetting != null)
                    this.context.DisplaySettings.Remove(sdt.DisplaySetting);
                if (sdt.TimeIntervals != null)
                    this.context.TimeIntervals.RemoveRange(sdt.TimeIntervals);
                this.context.ScheduledDisplayTimes.Remove(sdt);
                this.context.SaveChanges();
            }
        }

        public ScheduledDisplayTime Fetch(int id)
        {
            return this.context.ScheduledDisplayTimes.Find(id);
        }

        public List<ScheduledDisplayTime> FetchByTerminal(int terminalId)
        {
            return this.context.ScheduledDisplayTimes.Where(sdt => sdt.Terminal.Id == terminalId).ToList();
        }
        public List<ScheduledDisplayTime> FetchByTerminalActive(int terminalId)
        {
            return this.context.ScheduledDisplayTimes.Where(sdt => sdt.Terminal.Id == terminalId && sdt.Active).ToList();
        }

        public List<ScheduledDisplayTime> FetchByTerminalInactive(int terminalId)
        {
            return this.context.ScheduledDisplayTimes.Where(sdt => sdt.Terminal.Id == terminalId && !sdt.Active).ToList();
        }

        public List<ScheduledDisplayTime> FetchByDigitalSign(int digitalSignId)
        {
            return this.context.ScheduledDisplayTimes.Where(sdt => sdt.DigitalSign.Id == digitalSignId).ToList();
        }
      
        public void Dispose()
        {
            if(this.context != null)
                this.context.Dispose();
        }
    }
}
