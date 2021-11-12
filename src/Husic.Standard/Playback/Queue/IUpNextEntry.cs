﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Husic.Standard.Playback.Queue
{
   public interface IUpNextEntry : ISectionEntry
   {
      #region Properties
      int RepeatSectionIndex { get; }
      #endregion
   }
}
