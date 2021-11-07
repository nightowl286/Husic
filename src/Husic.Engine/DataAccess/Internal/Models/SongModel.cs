using Husic.Engine.Playback;
using System;
using System.Collections.Generic;
using System.Text;

namespace Husic.Engine.DataAccess.Internal.Models
{
   internal class SongModel
   {
      #region Properties
      public int Id { get; set; }
      public string Source { get; set; }
      public string Name { get; set; }
      public int Duration { get; set; }
      #endregion
   }
}
