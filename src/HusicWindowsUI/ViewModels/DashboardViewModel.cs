using Caliburn.Micro;
using Husic.Standard.DataAccess;
using Husic.Standard.DataAccess.Repositories;
using Husic.Standard.Playback;
using Husic.Standard.Playback.Queue;
using Husic.Windows.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Husic.Windows.ViewModels
{
   internal class DashboardViewModel : Conductor<object>
   {
      #region Private
      private readonly IHusicPlayer _Player;
      private readonly IPlayQueue _PlayQueue;
      #endregion

      #region Properties
      public IHusicPlayer Player => _Player;
      #endregion
      public DashboardViewModel(IHusicPlayer player, ISongRepository songRepo, IPlayQueue playQueue)
      {
         _Player = player;
         _PlayQueue = playQueue;

         //player.Volume = 0.5;

         player.PropertyChanged += Player_PropertyChanged;
         playQueue.PropertyChanged += PlayQueue_PropertyChanged;

         
         Task.Run(() => Stuff(songRepo));
      }


      private async Task Stuff(ISongRepository songRepo)
      {
         uint page = 0;
         IEnumerable<ISong> songs = await songRepo.GetSongs(page);
         while (songs.Count() > 0)
         {
            AddSongs(songs);
            songs = await songRepo.GetSongs(++page);
         }
      }

      #region Events
      private void Player_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
         string? prop = e.PropertyName;
         if (prop == nameof(Player.IsSongLoaded))
            NotifyOfPropertyChange(() => CanTogglePause);
      }
      private void PlayQueue_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
         string? prop = e.PropertyName;
         if (prop == nameof(_PlayQueue.NextSong))
            NotifyOfPropertyChange(() => CanPlayNext);
         if (prop == nameof(_PlayQueue.PreviousSong))
            NotifyOfPropertyChange(() => CanPlayPrevious);
      }
      #endregion

      #region Control buttons
      public bool CanPlayPrevious => _PlayQueue.PreviousSong != null;
      public bool CanPlayNext => _PlayQueue.NextSong != null;
      public bool CanTogglePause => Player.IsSongLoaded;
      public void TogglePause()
      {
         if (Player.IsPlaying)
            Player.Pause();
         else
            Player.Play();
      }
      public void PlayPrevious() => _PlayQueue.MoveBackwards();
      public void PlayNext() => _PlayQueue.MoveFowards();
      #endregion

      #region Methods
      private void AddSongs(IEnumerable<ISong> songs)
      {
         foreach (ISong song in songs)
         {
            IUpNextEntry entry = new Engine.Playback.Queue.UpNextEntry() { Song = song };
            _PlayQueue.UpNextSection.Entries.Add(entry);
         }
      }
      #endregion
   }
}
