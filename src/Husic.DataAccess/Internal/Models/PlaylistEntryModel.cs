using System;
using System.Collections.Generic;
using System.Text;

namespace Husic.DataAccess.Internal.Models
{
   internal class PlaylistEntryModel
   {
      #region Properties
      public int PlaylistId { get; set; }
      public int SongId { get; set; }
      public int EntryIndex { get; set; }
      #endregion
   }
}
