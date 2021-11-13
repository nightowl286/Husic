using Caliburn.Micro;
using Husic.Standard.Playback;
using Husic.Standard.Playback.Queue;

namespace Husic.Engine.Playback.Queue
{
   public class SectionEntry : PropertyChangedBase, ISectionEntry
   {
      #region Private
      private ISong _Song;
      private int _Index;
      private int _PlayIndex;
      #endregion

      #region Properties
      public ISong Song { get => _Song; set => Set(ref _Song, value); }
      public int Index { get => _Index; set => Set(ref _Index, value); }
      public int PlayIndex { get => _PlayIndex; set => Set(ref _PlayIndex, value); }
      #endregion
   }
}
