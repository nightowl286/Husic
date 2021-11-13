using Husic.Standard.Playback.Queue;
using System;
using System.Collections.Generic;
using System.Text;

namespace Husic.Engine.Playback.Queue
{
   public class UpNextEntry : SectionEntry, IUpNextEntry
   {
      #region Private
      private int _RepeatSectionIndex;
      #endregion

      #region Properties
      public int RepeatSectionIndex { get => _RepeatSectionIndex; set => Set(ref _RepeatSectionIndex, value); }
      #endregion
   }
}
