using Caliburn.Micro;
using Husic.Standard.Playback.Queue;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Husic.Engine.Playback.Queue
{
   public class UpNextSection : PropertyChangedBase, IUpNextSection
   {
      #region Private
      private string _Name;
      private ObservableCollection<IUpNextEntry> _Entries;
      #endregion

      #region Properties
      public string Name { get => _Name; set => Set(ref _Name, value); }
      public ObservableCollection<IUpNextEntry> Entries { get => _Entries; set => Set(ref _Entries, value); }
      #endregion
      public UpNextSection()
      {
         Entries = new ObservableCollection<IUpNextEntry>();
      }

      #region Methods
      public void Clear() => Entries.Clear();
      #endregion
   }
}
