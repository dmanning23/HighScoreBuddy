using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace HighScoreBuddy.SharedProject
{
	public class HighScoreTableComponent : DrawableGameComponent, IHighScoreTable
	{
		#region Fields

		private readonly IHighScoreTable _highScoreTable;

		#endregion //Fields

		#region Methods

		/// <summary>
		/// hello standard constructor!
		/// </summary>
		public HighScoreTableComponent(Game game, IHighScoreTable highScoreTable)
			: base(game)
		{
			_highScoreTable = highScoreTable;

			// Register ourselves to implement the IToastBuddy service.
			game.Components.Add(this);
			game.Services.AddService(typeof(IHighScoreTable), this);
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			InitializeDatabase();
		}

		public void InitializeDatabase()
		{
			_highScoreTable.InitializeDatabase();
		}

		public void AddHighScore(string highScoreList, uint points, string initials, DateTime? date = null)
		{
			_highScoreTable.AddHighScore(highScoreList, points, initials, date);
		}

		public void ClearHighScoreList(string highScoreList)
		{
			_highScoreTable.ClearHighScoreList(highScoreList);
		}

		public IEnumerable<Tuple<string, uint>> GetDailyHighScoreList(string highScoreList, int num)
		{
			return _highScoreTable.GetDailyHighScoreList(highScoreList, num);
		}

		public IEnumerable<Tuple<string, uint>> GetHighScoreList(string highScoreList, int num)
		{
			return _highScoreTable.GetHighScoreList(highScoreList, num);
		}

		public bool IsDailyHighScore(string highScoreList, uint points, int num)
		{
			return _highScoreTable.IsDailyHighScore(highScoreList, points, num);
		}

		public bool IsHighScore(string highScoreList, uint points, int num)
		{
			return _highScoreTable.IsHighScore(highScoreList, points, num);
		}

		public void TruncateHighScoreList(string highScoreList, int num)
		{
			_highScoreTable.TruncateHighScoreList(highScoreList, num);
		}

		#endregion //Methods
	}
}
