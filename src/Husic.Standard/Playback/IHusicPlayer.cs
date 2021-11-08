using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Husic.Standard.Playback
{
   public interface IHusicPlayer : INotifyPropertyChanged
   {
      #region Properties
      bool IsSongLoaded { get; }
      bool IsPlaying { get; set; }
      double Volume { get; set; }
      bool IsMuted { get; set; }
      TimeSpan Duration { get; }
      TimeSpan Position { get; set; }
      ISong CurrentlyPlaying { get; }
      #endregion

      #region Methods
      void Play(ISong song);
      void Play();
      void Pause();
      #endregion
   }
}