using Caliburn.Micro;
using Husic.Standard.Playback;
using Husic.Standard.Playback.Queue;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Husic.Windows.Playback
{
   public class HusicPlayer : PropertyChangedBase, IHusicPlayer
   {
      #region Private
      private readonly IPlayer _Player;
      private readonly IPlayQueue _PlayQueue;
      private readonly Timer _Timer;
      #endregion

      #region Backings
      private bool _IsSongLoaded = false;
      private bool _IsPlaying = false;
      private bool _IsMuted = false;
      private double _DesiredVolume;
      private ISong _CurrentlyPlaying;
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
      public ISong CurrentlyPlaying { get => _CurrentlyPlaying; private set => Set(ref _CurrentlyPlaying, value); }
      #endregion
      public HusicPlayer(IPlayer player, IPlayQueue playQueue)
      {
         _Player = player;
         _PlayQueue = playQueue;
         Volume = player.Volume;

         _Timer = new Timer();
         _Timer.Elapsed += Timer_Elapsed;
         _Timer.Interval = 50;
         _Timer.AutoReset = true;

         _PlayQueue.PropertyChanged += PlayQueue_PropertyChanged;
      }

      #region Events
      private void Timer_Elapsed(object sender, ElapsedEventArgs e)
      {
         if (IsPlaying)
         {
            try
            {
               NotifyOfPropertyChange(() => Position);
               NotifyOfPropertyChange(() => Duration);
            }
            catch (TaskCanceledException) // in case of shutdown
            {
               _Timer.Stop();
            }
         }
      }
      private void PlayQueue_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         if (e.PropertyName == nameof(IPlayQueue.CurrentSong))
         {
            if (_PlayQueue.CurrentSong == null)
               Stop();
            else
            {
               Play(_PlayQueue.CurrentSong);
            }
         }
      }
      #endregion

      #region Methods
      private void Stop()
      {
         CurrentlyPlaying = null;
         _Player.Stop();
         IsSongLoaded = false;
         _IsPlaying = false;
         _Timer.Stop();
            NotifyOfPropertyChange(() => IsPlaying);
      }
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
         _Player.Load(song.Source);
         CurrentlyPlaying = song;
         Position = TimeSpan.Zero;
         IsSongLoaded = true;
         Play();
         NotifyOfPropertyChange(() => Duration);
      }
      public void Play()
      {
         if (!IsPlaying)
            _Timer.Start();

         _Player.Play();
         _IsPlaying = true;
         NotifyOfPropertyChange(() => IsPlaying);
      }
      #endregion
   }
}
