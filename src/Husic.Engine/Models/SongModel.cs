using Husic.Engine.Playback;
using System;
using System.Collections.Generic;
using System.Text;

namespace Husic.Engine.Models
{
   public class SongModel : ISong
   {
      #region Properties
      public int Id { get; set; }
      public Uri Source { get; set; }
      public string Name { get; set; }
      public TimeSpan Duration { get; set; }
      #endregion
   }
}
