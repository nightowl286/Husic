using System;
using System.Collections.Generic;
using System.Text;

namespace Husic.Standard.Playback
{
   public interface IPlaylist
   {
      #region Properties
      int Id { get; }
      string Name { get; set; }
      int Count { get; set; }
      #endregion
   }
}
