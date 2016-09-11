using SQLite.Net.Attributes;

namespace HighScoreBuddy.Models
{
	[Table("Days")]
	public class Day
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		[Unique]
		[MaxLength(16)]
		public string Date { get; set; }
	}
}
