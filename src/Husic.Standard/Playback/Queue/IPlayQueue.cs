using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Husic.Standard.Playback.Queue
{
   public interface IPlayQueue : INotifyPropertyChanged
   {
      #region Properties
      ISong PreviousSong { get; }
      ISong CurrentSong { get; }
      ISong NextSong { get; }

      /// <summary>
      /// Stores a list of maximum <c>n</c> songs that have been previously
      /// played, with the most recent ones at the beginning
      /// </summary>
      IQueueSection HistorySection { get; }

      /// <summary>
      /// Stores a list of songs that will be played next but only once
      /// </summary>
      IQueueSection PlayOnceSection { get; }

      /// <summary>
      /// Stores a list of songs that are taken from <c>RepeatingSection</c> 
      /// that will play after <c>PlayOnceSection</c>
      /// </summary>
      IUpNextSection UpNextSection { get; }

      /// <summary>
      /// Stores a list of songs that should be repeated, if repeating is turned on
      /// </summary>
      IQueueSection RepeatingSection { get; }
      #endregion

      #region Methods
      void MoveBackwards();
      void MoveFowards();
      #endregion
   }
}
