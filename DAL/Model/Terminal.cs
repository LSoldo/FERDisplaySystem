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
        public DigitalSign ActiveSign { get; set; }
 
    }
}
