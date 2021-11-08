using System;
using System.Collections.Generic;
using System.Text;

namespace Husic.Standard.Playback
{
   public interface ISong
   {
      #region Properties
      int Id { get; }
      Uri Source { get; }
      string Name { get; }
      TimeSpan Duration { get; }
      #endregion
   }
}
