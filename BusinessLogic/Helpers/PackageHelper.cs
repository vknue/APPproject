using BusinessLogic.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Helpers
{
    public static class PackageHelper
    {
        public static int GetDailyLimit(PackageType type) => type switch
        {
            PackageType.GOLD => 100,
            PackageType.PRO => 10,
            _ => 3 
        };
    }
}
