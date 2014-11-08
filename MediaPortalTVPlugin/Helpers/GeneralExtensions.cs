using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortalTVPlugin.Helpers
{
    public static class GeneralExtensions
    {
        public static String ToUrlDate(this DateTime value)
        {
            return value.ToString("s");
        }
    }
}
