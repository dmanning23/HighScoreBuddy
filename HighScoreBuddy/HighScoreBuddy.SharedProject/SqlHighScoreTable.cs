using HighScoreBuddy.Models;
using SQLite.Net;
using SQLiteConnectionBuddy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HighScoreBuddy
{
	/// <summary>
	/// A high score table that is persisted to SQLite
	/// </summary>
	public class SqlHighScoreTable : IHighScoreTable
	{
		#region Fields

		const string _db = "HighScores.db";

		/// <summary>
		/// Lock object
		/// </summary>
		static object locker = new object();

		#endregion //Fields

		#region Methods

		/// <summary>
		/// hello standard constructor!
		/// </summary>
		public SqlHighScoreTable()
		{
		}

		public void InitializeDatabase()
		{
			//load settingsfile
			using (var db = SQLiteConnectionHelper.GetConnection(_db))
			{
				//this will create the table if it doesn exist, upgrade if it has changed, or nothing if it is the same
				db.CreateTable<Day>();
				db.CreateTable<HighScoreList>();
				db.CreateTable<Score>();
			}
		}

		public void AddHighScore(string highScoreList, uint points, string initials)
		{
			lock (locker)
			{
				using (var connection = SQLiteConnectionHelper.GetConnection(_db))
				{
					var highScoreListId = SaveHighScoreList(connection, highScoreList);
					var dayId = SaveDay(connection);

					//Add the new high score
					connection.Insert(new Score
					{
						Points = points,
						Initials = initials,
						DayId = dayId,
						HighScoreListId = highScoreListId
					});
				}
			}
		}

		public IEnumerable<Tuple<string, uint>> GetDailyHighScoreList(string highScoreList, int num)
		{
			lock (locker)
			{
				using (var connection = SQLiteConnectionHelper.GetConnection(_db))
				{
					//get the scores from 
					var scores = connection.Query<Score>(
						@"select top ? score from Scores as s
						inner join Days as d on s.DayId = d.Id
						inner join HighScoreList as hsl on s.HighScoreListId = hsl.Id
						where d.Date = ?
						and hsl.Name = ?
						order by s.Points desc", num, highScoreList, DateTime.Now.Date.ToString());

					//convert to tuples
					return scores.Select(x => new Tuple<string, uint>(x.Initials, x.Points));
				}
			}
		}

		public IEnumerable<Tuple<string, uint>> GetHighScoreList(string highScoreList, int num)
		{
			lock (locker)
			{
				using (var connection = SQLiteConnectionHelper.GetConnection(_db))
				{
					//get the scores from 
					var scores = connection.Query<Score>(
						@"select * from Scores as s
						inner join HighScoreList as hsl on s.HighScoreListId = hsl.Id
						where hsl.Name = ?
						order by s.Points desc
						limit ?", highScoreList, num);

					//convert to tuples
					return scores.Select(x => new Tuple<string, uint>(x.Initials, x.Points));
				}
			}
		}

		public bool IsHighScore(string highScoreList, uint points, int num)
		{
			var highscores = GetHighScoreList(highScoreList, num);
			return highscores.Any(x => x.Item2 < points);
		}

		public bool IsDailyHighScore(string highScoreList, uint points, int num)
		{
			var highscores = GetDailyHighScoreList(highScoreList, num);
			return highscores.Any(x => x.Item2 < points);
		}

		public void TruncateHighScoreList(string highScoreList, int num)
		{
			throw new NotImplementedException();
		}

		private int SaveHighScoreList(SQLiteConnection connection, string highScoreList)
		{
			//get the lesson
			var scores = connection.Table<HighScoreList>().FirstOrDefault(x => x.Name == highScoreList);
			if (null == scores)
			{
				scores = new HighScoreList
				{
					Name = highScoreList
				};
				connection.Insert(scores);
			}

			return scores.Id;
		}

		private int SaveDay(SQLiteConnection connection)
		{
			//get the day
			var date = DateTime.Now.Date.ToString();
			var day = connection.Table<Day>().FirstOrDefault(x => x.Date == date);
			if (null == day)
			{
				day = new Day
				{
					Date = date
				};
				connection.Insert(day);
			}

			return day.Id;
		}

		public void ClearHighScoreList(string highScoreList)
		{
			lock (locker)
			{
				using (var connection = SQLiteConnectionHelper.GetConnection(_db))
				{
					connection.Execute(
						@"delete from Scores 
						where HighScoreListId IN ( 
							select Id from HighScoreList
							where Name = ? )", highScoreList);
				}
			}
		}

		#endregion //Methods
	}
}