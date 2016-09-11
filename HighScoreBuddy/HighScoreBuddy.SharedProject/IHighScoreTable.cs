using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace HighScoreBuddy
{
	/// <summary>
	/// The interface for a set of high score lists
	/// </summary>
    interface IHighScoreTable : IGameComponent
    {
		/// <summary>
		/// Add a high score to the list of all high scores.
		/// </summary>
		/// <param name="highScoreList">The list to add this score to</param>
		/// <param name="points">The number of points that were scored</param>
		/// <param name="initials">Player initials</param>
		void AddHighScore(string highScoreList, uint points, string initials);

		/// <summary>
		/// Check if the player got a high score
		/// </summary>
		/// <param name="highScoreList">the list to check</param>
		/// <param name="points">the number of points that were scored</param>
		/// <param name="num">the number of items in each high score list</param>
		/// <returns></returns>
		bool IsHighScore(string highScoreList, uint points, int num);

		/// <summary>
		/// Truncate a high score list to the top num of items
		/// </summary>
		/// <param name="highScoreList">the list to truncate</param>
		/// <param name="num">the max number of items desired in the list</param>
		void TruncateHighScoreList(string highScoreList, int num);

		/// <summary>
		/// Get a list of highscores, in descending order
		/// </summary>
		/// <param name="highScoreList">the list to fecth</param>
		/// <param name="num">the number of items desired in the list</param>
		/// <returns></returns>
		IEnumerable<Tuple<string, uint>> GetHighScoreList(string highScoreList, int num);

		/// <summary>
		/// Get a today's high scores
		/// </summary>
		/// <param name="highScoreList">the list to fetch</param>
		/// <param name="num">the number of items desired in the list</param>
		/// <returns></returns>
		IEnumerable<Tuple<string, uint>> GetDailyHighScoreList(string highScoreList, int num);

	}
}
