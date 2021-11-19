using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Husic.Windows.Helpers
{
   internal static class Shell32Helper
   {
      public static TimeSpan? GetDuration(string audioFile)
      {
         using ShellFile file = ShellFile.FromFilePath(audioFile);
         ulong? nano = file.Properties.System.Media.Duration.Value;
         if (!nano.HasValue)
            return null;

         ulong ms = nano.Value / 10_000;

         TimeSpan duration = TimeSpan.FromMilliseconds(ms);

         return duration;
      }
   }
}
