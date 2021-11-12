using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Husic.Standard.Playback.Queue
{
   public interface ISectionEntry : INotifyPropertyChanged
   {
      #region Properties
      ISong Song { get; }
      int Index { get; set; }
      int PlayIndex { get; set; }
      #endregion
   }
}
