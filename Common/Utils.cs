using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace dm.AOL.Bot.Common
{
    public static class Util
    {
        public static string GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string fullV = fvi.FileVersion;
            string patchV = fullV.Substring(0, fullV.LastIndexOf('.')).TrimEnd(".0");
            string buildV = fullV.Substring(fullV.LastIndexOf('.') + 1);
            return $"{patchV} (build {buildV})";
        }
    }
}
