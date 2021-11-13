using Caliburn.Micro;
using Husic.Standard.Playback.Queue;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Husic.Engine.Playback.Queue
{
   public class QueueSection : PropertyChangedBase, IQueueSection
   {
      #region Private
      private string _Name;
      private ObservableCollection<ISectionEntry> _Entries;
      #endregion

      #region Properties
      public string Name { get => _Name; set => Set(ref _Name, value); }
      public ObservableCollection<ISectionEntry> Entries { get => _Entries; set => Set(ref _Entries, value); }
      #endregion
      public QueueSection()
      {
         Entries = new ObservableCollection<ISectionEntry>();
      }

      #region Methods
      public void Clear() => Entries.Clear();
      #endregion
   }
}
