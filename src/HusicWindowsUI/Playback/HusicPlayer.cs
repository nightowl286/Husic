using Caliburn.Micro;
using Husic.Engine.Playback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Husic.Windows.Playback
{
   internal class HusicPlayer : PropertyChangedBase, IHusicPlayer
   {
      #region Private
      private readonly IPlayer _Player;
      #endregion

      #region Backings
      private bool _IsSongLoaded = false;
      private bool _IsPlaying = false;
      private bool _IsMuted = false;
      private double _DesiredVolume;
      private ISong? _CurrentlyPlaying;
      #endregion

      #region Properties
      public bool IsSongLoaded { get => _IsSongLoaded; set => Set(ref _IsSongLoaded, value); }
      public bool IsPlaying
      {
         get => _IsPlaying;
         set
         {
            if (IsPlaying && !value)
               _Player.Pause();
            else if (!IsPlaying && value && IsSongLoaded)
               _Player.Play();

            Set(ref _IsPlaying, value);
         }
      }
      public double Volume 
      {
         get => _DesiredVolume;
         set
         {
            if (!IsMuted)
               _Player.Volume = value;

            Set(ref _DesiredVolume, value);
         }
      }
      public bool IsMuted
      {
         get => _IsMuted;
         set
         {
            if (IsMuted & !value) // unmute
               _Player.Volume = Volume;
            else if (!IsMuted & value) // mute
               _Player.Volume = 0;

            Set(ref _IsMuted, value);
         }
      }
      public TimeSpan Duration => _Player.Duration;
      public TimeSpan Position
      {
         get => _Player.Position;
         set
         {
            _Player.Position = value;
            NotifyOfPropertyChange();
         }
      }
      public ISong? CurrentlyPlaying { get => _CurrentlyPlaying; private set => Set(ref _CurrentlyPlaying, value); }
      #endregion
      public HusicPlayer(IPlayer player)
      {
         _Player = player;
      }

      #region Methods
      public void Pause() => IsPlaying = false;
      public void Play(ISong song)
      {
         _Player.Load(song.Source);
         CurrentlyPlaying = song;
         _Player.Position = TimeSpan.Zero;
         _Player.Play();
      }
      public void Play() => IsPlaying = true;
      #endregion
   }
}
