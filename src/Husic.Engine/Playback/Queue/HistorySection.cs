using Caliburn.Micro;
using Husic.Standard.Playback;
using Husic.Standard.Playback.Queue;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace Husic.Engine.Playback.Queue
{
   public class HistorySection : PropertyChangedBase, IHistorySection
   {
      #region Private
      private string _Name;
      private ReadOnlyObservableCollection<ISectionEntry> _Entries;
      private ObservableCollection<ISectionEntry> _MutableEntries;
      private int _MaxSize;
      #endregion

      #region Properties
      public string Name { get => _Name; set => Set(ref _Name, value); }
      public ReadOnlyObservableCollection<ISectionEntry> Entries { get => _Entries; set => Set(ref _Entries, value); }

      public int MaxSize { get => _MaxSize; set => Set(ref _MaxSize, value); }
      #endregion
      public HistorySection()
      {
         _MutableEntries = new ObservableCollection<ISectionEntry>();
         Entries = new ReadOnlyObservableCollection<ISectionEntry>(_MutableEntries);
      }

      #region Methods
      public void Clear() => _MutableEntries.Clear();
      public void TrimToMax()
      {
         Debug.Assert(MaxSize > 0);

         while (Entries.Count > MaxSize)
            _MutableEntries.RemoveAt(Entries.Count - 1);
      }
      public void AddEntry(ISectionEntry entry)
      {
         Debug.Assert(MaxSize > 0);
         entry.Index = 0;
         if (Entries.Count == MaxSize)
         {
            for (int i = Entries.Count-1; i > 0; i++)
            {
               ISectionEntry previous = Entries[i - 1];
               previous.Index = i;
               previous.PlayIndex = -(i + 1);
               _MutableEntries[i] = previous;
            }
            _MutableEntries[0] = entry;
         }
         else
            _MutableEntries.Insert(0, entry);
      }
      public ISectionEntry GetByPlayIndex(int playIndex)
      {
         int index = Math.Abs(playIndex) - 1;
         if (index >= 0 && index < Entries.Count)
            return Entries[index];
         
         return null;
      }
      #endregion
   }
}
