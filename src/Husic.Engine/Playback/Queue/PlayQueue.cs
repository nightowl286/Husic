using Caliburn.Micro;
using Husic.Standard.Playback;
using Husic.Standard.Playback.Queue;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

      #region Events
      private void PlayOnce_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         UpdateIndex(PlayOnceSection.Entries, 0);
         UpdateIndex(UpNextSection.Entries, PlayOnceSection.Entries.Count);
         GetPreviousSong();
         GetNextSong();
         if (CurrentSong == null)
            GetStartingSong();
      }
      private void UpNext_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         UpdateIndex(UpNextSection.Entries, PlayOnceSection.Entries.Count);
         GetPreviousSong();
         GetNextSong();
         if (CurrentSong == null)
            GetStartingSong();
      }
      private void UpdateIndex<T>(ObservableCollection<T> entries, int playOffset) where T: ISectionEntry
      {
         for (int i = 0; i < entries.Count; i++)
         {
            T entry = entries[i];
            entry.Index = i;
            entry.PlayIndex = i + playOffset;
         }
      }
      #endregion

      #region Methods
      private void CreateSections()
      {
         HistorySection = IoC.Get<IHistorySection>();
         HistorySection.Name = "history";
         HistorySection.MaxSize = MaxHistorySize;

         PlayOnceSection = IoC.Get<IQueueSection>();
         PlayOnceSection.Name = "play-once";
         PlayOnceSection.Entries.CollectionChanged += PlayOnce_CollectionChanged;

         UpNextSection = IoC.Get<IUpNextSection>();
         UpNextSection.Name = "up-next";
         UpNextSection.Entries.CollectionChanged += UpNext_CollectionChanged;

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

         ISong oldCurrent = CurrentSong;
         CurrentSong = NextSong;

         if (CurrentPlayIndex < 0)
         {
            CurrentPlayIndex++;
            PreviousSong = oldCurrent;

            GetNextSong();
         }
         else
         {
            Debug.Assert(CurrentPlayIndex == 0);
            AddToHistory(PreviousSong); // current song before the move

            if (PlayOnceSection.Entries.Count > 0)
               PlayOnceSection.Entries.RemoveAt(0);
            else
               UpNextSection.Entries.RemoveAt(0);
         }

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
      public void Play(int playIndex)
      {
         if (playIndex < 0)
         {
            CurrentPlayIndex = playIndex;
            ISectionEntry historyEntry = HistorySection.GetByPlayIndex(playIndex);
            Debug.Assert(historyEntry != null);

            CurrentSong = historyEntry.Song;

            GetNextSong();
            GetPreviousSong();
         }
         else
         {
            if (playIndex < PlayOnceSection.Entries.Count)
            {
               CurrentSong = PlayOnceSection.Entries[playIndex].Song;

               for (int i = 0; i < playIndex; i++)
               {
                  ISectionEntry entry = PlayOnceSection.Entries[0];
                  AddToHistory(entry.Song);
                  PlayOnceSection.Entries.RemoveAt(0);
               }
               if (playIndex == 0)
               {
                  GetNextSong();
                  GetPreviousSong();
               }
            }
            else
            {
               int index = playIndex - PlayOnceSection.Entries.Count;
               Debug.Assert(index < UpNextSection.Entries.Count);
               CurrentSong = UpNextSection.Entries[index].Song;

               int playOnceAmount = PlayOnceSection.Entries.Count;

               foreach (ISectionEntry entry in PlayOnceSection.Entries)
                  AddToHistory(entry.Song);
               PlayOnceSection.Clear();

               int relativePlayIndex = playIndex - playOnceAmount;
               for (int i = 0; i < relativePlayIndex; i++)
               {
                  ISectionEntry entry = UpNextSection.Entries[0];
                  AddToHistory(entry.Song);
                  UpNextSection.Entries.RemoveAt(0);
               }
               if (relativePlayIndex == 0)
               {
                  GetNextSong();
                  GetPreviousSong();
               }
            }
         }
      }
      private void AddToHistory(ISong song)
      {
         ISectionEntry entry = MakeEntry(song);
         entry.PlayIndex = -1;
         HistorySection.AddEntry(entry);
      }
      private void GetPreviousSong()
      {
         PreviousSong = HistorySection.GetByPlayIndex(CurrentPlayIndex - 1)?.Song;
      }
      private void GetNextSong()
      {
         int nextIndex = CurrentPlayIndex + 1;
         if (nextIndex < -1)
            NextSong = HistorySection.GetByPlayIndex(nextIndex + 1).Song;
         else
         {
            if (nextIndex < PlayOnceSection.Entries.Count)
               NextSong = PlayOnceSection.Entries[nextIndex].Song;
            else
            {
               int index = nextIndex - PlayOnceSection.Entries.Count;
               if (index < UpNextSection.Entries.Count)
                  NextSong = UpNextSection.Entries[index].Song;
               else
                  NextSong = null;
            }
         }
      }
      private void GetStartingSong()
      {
         if (PlayOnceSection.Entries.Count > 0)
            CurrentSong = PlayOnceSection.Entries[0].Song;
         else if (UpNextSection.Entries.Count > 0)
            CurrentSong = UpNextSection.Entries[0].Song;


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
