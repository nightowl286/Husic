using Husic.Standard.Playback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Husic.Windows.Playback
{
   internal class WindowsPlayer : IPlayer
   {
      #region Private
      private readonly MediaPlayer _Player;
      #endregion

      #region Events
      public event Action? PlaybackFinished;
      public event Action? PlaybackLoaded;
      #endregion

      #region Properties
      public double Volume
      {
         get => _Player.Volume;
         set => _Player.Volume = value;
      }
      public TimeSpan Duration
      {
         get
         {
            if (_Player.Source == null)
               return TimeSpan.Zero;

            while (!_Player.NaturalDuration.HasTimeSpan)
               Thread.Sleep(10);

            return _Player.NaturalDuration.TimeSpan;
         }
      }
      public TimeSpan Position
      {
         get => _Player.Position;
         set => _Player.Position = value;
      }
      #endregion
      public WindowsPlayer()
      {
         _Player = new MediaPlayer();
         _Player.MediaOpened += Player_MediaOpened;
         _Player.MediaEnded += Player_MediaEnded;
         _Player.MediaFailed += Player_MediaFailed;
      }

      #region Events
      private void Player_MediaFailed(object? sender, ExceptionEventArgs e) => PlaybackFinished?.Invoke();
      private void Player_MediaEnded(object? sender, EventArgs e) => PlaybackFinished?.Invoke();
      private void Player_MediaOpened(object? sender, EventArgs e) => PlaybackLoaded?.Invoke();
      #endregion

      #region Methods
      public void Load(Uri source) => _Player.Open(source);
      public void Pause() => _Player.Pause();
      public void Play() => _Player.Play();
      public void Stop()
      {
         _Player.Stop();
         PlaybackFinished?.Invoke();
      }
      #endregion
   }
}
