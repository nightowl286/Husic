using Husic.Standard.Playback;
using System;
using System.Collections.Generic;
using System.Text;

namespace Husic.DataAccess.Models
{
   internal class PlaylistEntryModel : IPlaylistEntry
   {
      #region Properties
      public int Index { get; set; }
      public ISong Song { get; private set; }
      #endregion
      public PlaylistEntryModel(int index, ISong song)
      {
         Index = index;
         Song = song;
      }
   }
}
