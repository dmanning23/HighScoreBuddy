﻿using RandomExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HighScoreBuddy
{
	/// <summary>
	/// A list of high scores that only live in memory.
	/// Obviously these high scores are reset every time the game restarts.
	/// </summary>
	public class MemHighScoreTable : IHighScoreTable
	{
		#region Properties

		class Score
		{
			public uint Points { get; set; }

			public string Initials { get; set; }
		}

		private Dictionary<string, List<Score>> HighScoreLists { get; set; }

		#endregion //Properties

		#region Methods

		public MemHighScoreTable()
		{
			HighScoreLists = new Dictionary<string, List<Score>>();
		}

		public void InitializeDatabase()
		{
		}

		public void SeedFakeHighScores(string highScoreList)
		{
			//add a bunch of fake high scores
			var rand = new Random();
			uint score = 100;
			for (int i = 0; i < 100; i++)
			{
				AddHighScore(highScoreList, score, rand.NextWord(3, 3));
				score += 10;
			}
		}

		public void AddHighScore(string highScoreList, uint points, string initials, DateTime? date = null)
		{
			var scores = CreateOrGetHighScoreList(highScoreList);

			scores.Add(new Score()
			{
				Points = points, 
				Initials = initials,
			});
		}

		public bool IsHighScore(string highScoreList, uint points, int num)
		{
			var scores = GetHighScoreList(highScoreList, num);

			//Check for a high score
			foreach (var score in scores)
			{
				if (points > score.Item2)
				{
					//player got a new high score!
					return true;
				}
			}

			//no high scores :(
			return false;
		}

		public bool IsDailyHighScore(string highScoreList, uint points, int num)
		{
			return IsHighScore(highScoreList, points, num);
		}

		public void TruncateHighScoreList(string highScoreList, int num)
		{
			//just dont bother
		}

		public IEnumerable<Tuple<string, uint>> GetHighScoreList(string highScoreList, int num)
		{
			var scores = CreateOrGetHighScoreList(highScoreList);

			return scores
				.OrderByDescending(x => x.Points)
				.Take(num)
				.Select(x => new Tuple<string, uint>(x.Initials, x.Points));
		}

		public IEnumerable<Tuple<string, uint>> GetDailyHighScoreList(string highScoreList, int num)
		{
			return GetHighScoreList(highScoreList, num);
		}

		private List<Score> CreateOrGetHighScoreList(string highScoreList)
		{
			//double check that the high score list exists
			if (!HighScoreLists.ContainsKey(highScoreList))
			{
				HighScoreLists[highScoreList] = new List<Score>();
			}

			return HighScoreLists[highScoreList];
		}

		public void ClearHighScoreList(string highScoreList)
		{
			var scores = CreateOrGetHighScoreList(highScoreList);
			scores.Clear();
		}

		#endregion //Methods
	}
}
