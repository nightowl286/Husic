using Caliburn.Micro;
using Husic.Standard.Playback;
using Husic.Standard.Playback.Queue;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Husic.Engine.Playback.Queue
{
   public class PlayQueue : PropertyChangedBase, IPlayQueue
   {
      #region Private
      private int _CurrentPlayIndex;
      private ISong _PreviousSong;
      private ISong _CurrentSong;
      private ISong _NextSong;
      private IHistorySection _HistorySection;
      private IQueueSection _PlayOnceSection;
      private IUpNextSection _UpNextSection;
      private IQueueSection _RepeatingSection;
      public int _MaxHistoryCount = 100; // current default, update later once settings are implemented
      #endregion

      #region Properties
      public int MaxHistorySize
      {
         get => _MaxHistoryCount;
         set
         {
            if (Set(ref _MaxHistoryCount, value)) 
            {
               HistorySection.MaxSize = value;
               HistorySection.TrimToMax();
            }
         }
      }
      public int CurrentPlayIndex { get => _CurrentPlayIndex; private set => Set(ref _CurrentPlayIndex, value); }
      public ISong PreviousSong { get => _PreviousSong; private set => Set(ref _PreviousSong, value); }
      public ISong CurrentSong { get => _CurrentSong; private set => Set(ref _CurrentSong, value); }
      public ISong NextSong { get => _NextSong; private set => Set(ref _NextSong, value); }
      public IHistorySection HistorySection { get => _HistorySection; private set => Set(ref _HistorySection, value); }
      public IQueueSection PlayOnceSection { get => _PlayOnceSection; private set => Set(ref _PlayOnceSection, value); }
      public IUpNextSection UpNextSection { get => _UpNextSection; private set => Set(ref _UpNextSection, value); }
      public IQueueSection RepeatingSection { get => _RepeatingSection; private set => Set(ref _RepeatingSection, value); }
      #endregion
      public PlayQueue()
      {
         CreateSections();
      }

      #region Methods
      private void CreateSections()
      {
         HistorySection = IoC.Get<IHistorySection>();
         HistorySection.Name = "history";
         HistorySection.MaxSize = MaxHistorySize;

         PlayOnceSection = IoC.Get<IQueueSection>();
         PlayOnceSection.Name = "play-once";

         UpNextSection = IoC.Get<IUpNextSection>();
         UpNextSection.Name = "up-next";

         RepeatingSection = IoC.Get<IQueueSection>();
         RepeatingSection.Name = "repeating";
      }
      public void MoveBackwards()
      {
         if (PreviousSong == null) return;

         NextSong = CurrentSong;
         CurrentSong = PreviousSong;

         CurrentPlayIndex--;

         Debug.Assert(CurrentPlayIndex < 0);

         GetPreviousSong();
      }
      public void MoveFowards()
      {
         if (NextSong == null) return;

         PreviousSong = CurrentSong;
         CurrentSong = NextSong;

         if (CurrentPlayIndex < 0)
            CurrentPlayIndex++;
         else
         {
            Debug.Assert(CurrentPlayIndex == 0);
            AddToHistory(PreviousSong);

            bool updatePlayOnce = false;

            if (PlayOnceSection.Entries.Count > 0)
            {
               PlayOnceSection.Entries.RemoveAt(0);
               updatePlayOnce = true;
            }
            else
               UpNextSection.Entries.RemoveAt(0);

            if (updatePlayOnce)
            {
               // no need to figure out the index, it can just be decremented
               foreach (ISectionEntry entry in PlayOnceSection.Entries)
               {
                  entry.Index--;
                  entry.PlayIndex--;
               }
            }

            for (int i = 0; i < UpNextSection.Entries.Count; i++)
            {
               IUpNextEntry entry = UpNextSection.Entries[i];
               entry.PlayIndex--;

               // won't change anything if updatePlayOnce is true
               // but an if check would just slow it down
               entry.Index = i;
            }
         }

         GetNextSong();

      }
      public void Clear()
      {
         PreviousSong = null;
         CurrentSong = null;
         NextSong = null;
         CurrentPlayIndex = 0;

         HistorySection.Clear();
         PlayOnceSection.Clear();
         UpNextSection.Clear();
         RepeatingSection.Clear();
      }
      private void AddToHistory(ISong song)
      {
         ISectionEntry entry = MakeEntry(song);
         HistorySection.AddEntry(entry);
      }
      private void GetPreviousSong()
      {
         PreviousSong = HistorySection.GetByPlayIndex(CurrentPlayIndex - 1)?.Song;
      }
      private void GetNextSong()
      {
         if (CurrentPlayIndex < -1)
            NextSong = HistorySection.GetByPlayIndex(CurrentPlayIndex + 1).Song;
         else
         {
            if (CurrentPlayIndex < PlayOnceSection.Entries.Count)
               NextSong = PlayOnceSection.Entries[CurrentPlayIndex].Song;
            else
            {
               int index = CurrentPlayIndex - PlayOnceSection.Entries.Count;
               if (index < UpNextSection.Entries.Count)
                  NextSong = UpNextSection.Entries[index].Song;
               else
                  NextSong = null;
            }
         }
      }
      #endregion

      #region Functions
      private static ISectionEntry MakeEntry(ISong song)
      {
         SectionEntry entry = new SectionEntry();
         entry.Song = song;
         return entry;
      }
      #endregion
   }
}
