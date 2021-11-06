using System;
using System.Collections.Generic;
using System.Text;

namespace Husic.Engine.Playback
{
   public interface ISong
   {
      #region Properties
      Uri Source { get; }
      string Title { get; }
      TimeSpan Duration { get; }
      #endregion
   }
}
