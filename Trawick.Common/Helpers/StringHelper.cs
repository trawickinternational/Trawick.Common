using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trawick.Common.Helpers
{
    public class StringHelper
    {
        public static  string FSQ(string v)
        {
            if (v != null)
                return v.Replace("'", "''");
            else
                return "";
        }
    }
}
