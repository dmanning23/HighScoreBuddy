using SQLite;
using SQLiteNetExtensions.Attributes;

namespace HighScoreBuddy.Models
{
	[Table("Scores")]
	public class Score
	{
		[PrimaryKey, AutoIncrement]
		public int? Id { get; set; }

		public uint Points { get; set; }

		[MaxLength(16)]
		public string Initials { get; set; }

		[ForeignKey(typeof(Day))]
		public int DayId { get; set; }

		[ForeignKey(typeof(HighScoreList))]
		public int HighScoreListId { get; set; }
	}
}
