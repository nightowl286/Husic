using Caliburn.Micro;
using Husic.Standard.DataAccess;
using Husic.Standard.Playback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Husic.Windows.ViewModels
{
   internal class DashboardViewModel : Conductor<object>
   {
      #region Private
      private readonly IHusicPlayer _Player;
      #endregion

      #region Properties
      public IHusicPlayer Player => _Player;
      #endregion
      public DashboardViewModel(IHusicPlayer player, IDataAccess access)
      {
         _Player = player;

         //player.Volume = 0.5;

         player.PropertyChanged += Player_PropertyChanged;

         access.GetSongs().ContinueWith(songs => App.Current.Dispatcher.Invoke(() => player.Play(songs.Result.First())));
      }

      #region Events
      private void Player_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
         string? prop = e.PropertyName;
         if (prop == nameof(Player.IsSongLoaded))
            NotifyOfPropertyChange(() => CanTogglePause);
      }
      #endregion

      #region Control buttons
      public bool CanPlayPrevious => false;
      public bool CanPlayNext => false;
      public bool CanTogglePause => Player.IsSongLoaded;
      public void TogglePause()
      {
         if (Player.IsPlaying)
            Player.Pause();
         else
            Player.Play();
      }
      #endregion
   }
}
