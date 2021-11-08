﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Husic.Standard.Playback
{
   public interface IPlayer
   {
      #region Properties
      double Volume { get; set; }
      TimeSpan Duration { get; }
      TimeSpan Position { get; set; }
      #endregion

      #region Events
      event Action PlaybackFinished;
      event Action PlaybackLoaded;
      #endregion

      #region Methods
      void Load(Uri source);
      void Stop();
      void Play();
      void Pause();
      #endregion
   }
}
