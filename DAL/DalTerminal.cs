using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Model;

namespace DAL
{
    public class DalTerminal
    {
        private DBContext context;

        public DalTerminal()
        {
            this.context = new DBContext();
        }

        public int Add(Terminal terminal)
        {
            this.context.Terminals.Add(terminal);
            this.context.SaveChanges();
            return terminal.Id;
        }

        public void Update(Terminal newTerminal)
        {
            Terminal oldTerminal = this.context.Terminals.Find(newTerminal.Id);

            if(oldTerminal == null)
                throw new Exception("Could not find existing terminal in the database. ID possibly modified.");

            oldTerminal.Name = newTerminal.Name;
            oldTerminal.ActiveSign = newTerminal.ActiveSign;

            this.context.SaveChanges();
        }

        public void Delete(Terminal terminal)
        {
            this.context.ScheduledDisplayTimes.RemoveRange(this.context.ScheduledDisplayTimes.Where(t => t.Terminal.Id == terminal.Id));
            this.context.Terminals.Remove(terminal);
            this.context.SaveChanges();
        }

        public void Delete(int terminalId)
        {
            this.context.ScheduledDisplayTimes.RemoveRange(this.context.ScheduledDisplayTimes.Where(t => t.Terminal.Id == terminalId));
            this.context.Terminals.Remove(this.context.Terminals.Find(terminalId));
            this.context.SaveChanges();
        }

        public Terminal Fetch(int id)
        {
            return this.context.Terminals.Find(id);
        }

        public List<Terminal> FetchAll()
        {
            return this.context.Terminals.ToList();
        }

        public int GetTotalNumberOfTerminals()
        {
            return this.context.Terminals.Count();
        }

        public void Dispose()
        {
            if(this.context != null)
                this.context.Dispose();
        }
    }
}
