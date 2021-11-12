using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Husic.Standard.Playback.Queue
{
   public interface IQueueSectionBase<T> where T : ISectionEntry
   {
      #region Properties
      string Name { get; }
      ObservableCollection<T> Entries { get; }
      #endregion
   }
}
