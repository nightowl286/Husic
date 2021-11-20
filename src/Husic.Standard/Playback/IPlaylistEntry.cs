using System;
using System.Collections.Generic;
using System.Text;

namespace Husic.Standard.Playback
{
   public interface IPlaylistEntry
   {
      #region Properties
      int Index { get; set; }
      ISong Song { get; }
      #endregion
   }
}
