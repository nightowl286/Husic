namespace Husic.DataAccess.Internal.Models
{
   internal class SongModel
   {
      #region Properties
      public int Id { get; set; }
      public string Source { get; set; } = string.Empty;
      public string Name { get; set; } = string.Empty;
      public int Duration { get; set; }
      #endregion
   }
}
