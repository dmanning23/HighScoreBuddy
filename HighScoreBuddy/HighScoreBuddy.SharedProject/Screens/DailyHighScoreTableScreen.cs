using MenuBuddy;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace HighScoreBuddy
{
	/// <summary>
	/// This is a screen that shows one of the daily high scores tables
	/// </summary>
	public class DailyHighScoreTableScreen : HighScoreTableScreen
	{
		/// <summary>
		/// Constructor fills in the menu contents.
		/// </summary>
		public DailyHighScoreTableScreen(string screenTitle, string highScoreList, int num, ContentManager content = null)
			: base(screenTitle, highScoreList, num, content)
		{
		}

		protected override IEnumerable<Tuple<string, uint>> GetHighScores()
		{
			var highScoreComponent = ScreenManager.Game.Services.GetService<IHighScoreTable>();
			return highScoreComponent.GetDailyHighScoreList(HighScoreList, NumHighScores);
		}
	}
}
