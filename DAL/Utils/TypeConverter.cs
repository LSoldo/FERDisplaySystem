using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Model;

namespace DAL.Utils
{
    public static class TypeConverter
    {
        public static List<DataSource> ConvertToDataSource(List<string> strings)
        {
            return strings.Select(s => new DataSource() {Path = s}).ToList();
        }

        public static List<JsCodeWrapper> ConvertToJsCodeWrapper(List<string> jsCode)
        {
            return jsCode.Select(s => new JsCodeWrapper() { Code = s }).ToList();
        }
    }
}
