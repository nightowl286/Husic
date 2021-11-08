using Caliburn.Micro;
using Husic.Engine.Playback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Husic.Windows.Playback
{
   internal class HusicPlayer : PropertyChangedBase, IHusicPlayer
   {
      #region Private
      private readonly IPlayer _Player;
      private readonly DispatcherTimer _Timer;
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
               Pause();
            else if (!IsPlaying && value && IsSongLoaded)
               Play();
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
         _Timer = new DispatcherTimer();
         _Timer.Tick += Timer_Tick;
         _Timer.Interval = TimeSpan.FromMilliseconds(50);
      }

      #region Events
      private void Timer_Tick(object? sender, EventArgs e)
      {
         if (IsPlaying)
         {
            NotifyOfPropertyChange(() => Position);
            NotifyOfPropertyChange(() => Duration);
         }
      }
      #endregion

      #region Methods
      public void Pause()
      {
         if (IsPlaying)
         {
            _Player.Pause();
            _Timer.Stop();
            NotifyOfPropertyChange(() => Position);
            NotifyOfPropertyChange(() => Duration);
            _IsPlaying = false;
            NotifyOfPropertyChange(() => IsPlaying);
         }
      }
      public void Play(ISong song)
      {
         App.Current.Dispatcher.Invoke(() =>
         {
            _Player.Load(song.Source);
            CurrentlyPlaying = song;
            Position = TimeSpan.Zero;
            Play();
            NotifyOfPropertyChange(() => Duration);
         });
      }
      public void Play()
      {
         if (!IsPlaying)
         {
            _Player.Play();
            _Timer.Start();
            _IsPlaying = true;
            NotifyOfPropertyChange(() => IsPlaying);
         }
      }
      #endregion
   }
}
