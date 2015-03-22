using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Terminal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual DigitalSign ActiveSign { get; set; }
 
    }
}
