using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Husic.Standard.Playback.Queue
{
   public interface IHistorySection
   {
      #region Properties
      string Name { get; set; }
      ReadOnlyObservableCollection<ISectionEntry> Entries { get; }
      int MaxSize { get; set; }
      #endregion

      #region Methods
      void Clear();
      void TrimToMax();
      void AddEntry(ISectionEntry entry);
      ISectionEntry GetByPlayIndex(int playIndex);
      #endregion
   }
}
