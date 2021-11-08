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
      public double Volume
      {
         get => _Player.Volume;
         set
         {
            _Player.Volume = value;
            NotifyOfPropertyChange();
         }
      }
      // TODO(Nightowl) - Create a value converter for Timespan <-> String/Double
      public double Duration
      {
         get => _Player.Duration.TotalSeconds;
      }
      public double Position
      {
         get => _Player.Position.TotalSeconds;
         set
         {
            _Player.Position = TimeSpan.FromSeconds(value);
            NotifyOfPropertyChange();
         }
      }
      public string? SongName => _Player.CurrentlyPlaying?.Name;
      #endregion
      public DashboardViewModel(IHusicPlayer player, IDataAccess access)
      {
         _Player = player;
         player.PropertyChanged += Player_PropertyChanged;

         access.GetSongs().ContinueWith(songs => App.Current.Dispatcher.Invoke(() => player.Play(songs.Result.First())));
         
      }

      #region Events
      private void Player_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
         string? prop = e.PropertyName;
         if (prop == nameof(Volume) || prop == nameof(Duration) || prop == nameof(Position))
            NotifyOfPropertyChange(prop);
         else if (prop == nameof(_Player.CurrentlyPlaying))
            NotifyOfPropertyChange(nameof(SongName));
      }
      #endregion
   }
}
