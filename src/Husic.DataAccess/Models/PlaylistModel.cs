using Husic.Standard.Playback;
using System;
using System.Collections.Generic;
using System.Text;

namespace Husic.DataAccess.Models
{
   internal class PlaylistModel : IPlaylist
   {
      #region Properties
      public int Id { get; private set; }
      public string Name { get; set; }
      public int Count { get; set; }
      #endregion
      public PlaylistModel(int id)
      {
         Id = id;
      }
   }
}
