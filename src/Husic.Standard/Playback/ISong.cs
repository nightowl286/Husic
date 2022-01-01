using System;
using System.Collections.Generic;
using System.Text;

namespace Husic.Standard.Playback
{
   public interface ISong
   {
      #region Properties
      int Id { get; }
      Uri? Source { get; set; }
      string Name { get; set; }
      TimeSpan Duration { get; set; }
      #endregion
   }
}
