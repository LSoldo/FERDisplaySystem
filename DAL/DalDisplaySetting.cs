using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Model;

namespace DAL
{
    public class DalDisplaySetting : IDisposable
    {
        private DBContext context;

        public DalDisplaySetting()
        {
            this.context = new DBContext();
        }

        public int Add(DisplaySetting displaySetting)
        {
            this.context.DisplaySettings.Add(displaySetting);
            this.context.SaveChanges();
            return displaySetting.Id;
        }

        public void Update(DisplaySetting newDisplaySetting)
        {
            DisplaySetting oldDisplaySetting = this.context.DisplaySettings.Find(newDisplaySetting.Id);

            if (oldDisplaySetting == null)
                throw new Exception("Could not find existing display setting in the database. ID possibly modified.");

            this.context.Entry(oldDisplaySetting).CurrentValues.SetValues(newDisplaySetting);
            this.context.SaveChanges();
        }

        public void Delete(DisplaySetting displaySetting)
        {
            this.context.ScheduledDisplayTimes.RemoveRange(
                this.context.ScheduledDisplayTimes.Where(t => t.DisplaySetting.Id == displaySetting.Id));
            this.context.DisplaySettings.Remove(this.context.DisplaySettings.Find(displaySetting.Id));
            this.context.SaveChanges();
        }

        public void Delete(int terminalId)
        {
            this.context.ScheduledDisplayTimes.RemoveRange(
                this.context.ScheduledDisplayTimes.Where(t => t.DisplaySetting.Id == terminalId));
            this.context.DisplaySettings.Remove(this.context.DisplaySettings.Find(terminalId));
            this.context.SaveChanges();
        }

        public DisplaySetting Fetch(int id)
        {
            return this.context.DisplaySettings.Find(id);
        }

        public List<DisplaySetting> FetchByTerminal(int terminalId)
        {
            return
                this.context.ScheduledDisplayTimes.Where(t => t.Terminal.Id == terminalId)
                    .Select(sdt => sdt.DisplaySetting)
                    .Distinct()
                    .ToList();
        }

        public void Dispose()
        {
            if(this.context != null)
                this.context.Dispose();
        }
    }
}
