using Husic.Standard.Playback;
using System;
using System.Collections.Generic;
using System.Text;

namespace Husic.DataAccess.Models
{
   internal class SongModel : ISong
   {
      #region Properties
      public int Id { get; private set; }
      public Uri Source { get; set; }
      public string Name { get; set; }
      public TimeSpan Duration { get; set; }
      #endregion
      public SongModel(int id)
      {
         Id = id;
      }
   }
}
