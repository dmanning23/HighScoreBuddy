using HighScoreBuddy.Models;
using Microsoft.Xna.Framework;
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
	public abstract class SqlHighScoreTable : DrawableGameComponent, IHighScoreTable
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
		public SqlHighScoreTable(Game game)
			: base(game)
		{
			// Register ourselves to implement the IToastBuddy service.
			game.Components.Add(this);
			game.Services.AddService(typeof(IHighScoreTable), this);
		}

		protected override void LoadContent()
		{
			base.LoadContent();

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
			throw new NotImplementedException();
		}

		public IEnumerable<Tuple<string, uint>> GetHighScoreList(string highScoreList, int num)
		{
			throw new NotImplementedException();
		}

		public bool IsHighScore(string highScoreList, uint points, int num)
		{
			throw new NotImplementedException();
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

		#endregion //Methods
	}
}