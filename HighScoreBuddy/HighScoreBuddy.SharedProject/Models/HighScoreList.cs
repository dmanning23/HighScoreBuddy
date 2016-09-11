using SQLite.Net.Attributes;

namespace HighScoreBuddy.Models
{
	[Table("HighScoreList")]
	public class HighScoreList
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		[Unique]
		[MaxLength(32)]
		public string Name { get; set; }
	}
}
