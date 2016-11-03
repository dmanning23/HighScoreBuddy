using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighScoreBuddy.Tests
{
	[TestFixture]
	public class GetHighScoreListTests
	{
		IHighScoreTable _highScores;

		private const string tableName = "testList";

		[SetUp]
		public void Setup()
		{
			_highScores = new SqlHighScoreTable();
			_highScores.InitializeDatabase();
			_highScores.ClearHighScoreList(tableName);
		}

		[Test]
		public void CorrectNum_NotEnough()
		{
			for (uint i = 0; i < 5; i++)
			{
				_highScores.AddHighScore(tableName, i * 100, i.ToString());
			}

			var scores = _highScores.GetHighScoreList(tableName, 10);
			Assert.AreEqual(5, scores.Count());
		}

		[Test]
		public void CorrectNum_TooMany()
		{
			for (uint i = 0; i < 20; i++)
			{
				_highScores.AddHighScore(tableName, i * 100, i.ToString());
			}

			var scores = _highScores.GetHighScoreList(tableName, 10);
			Assert.AreEqual(10, scores.Count());
		}

		[Test]
		public void InOrder_Score()
		{
			_highScores.AddHighScore(tableName, 500, "2");
			_highScores.AddHighScore(tableName, 1000, "1");
			_highScores.AddHighScore(tableName, 100, "3");

			var scores = _highScores.GetHighScoreList(tableName, 10);

			int i = 0;
			foreach (var score in scores)
			{
				switch (i)
				{
					case 0:
						{
							Assert.AreEqual(1000, score.Item2);
						}
						break;
					case 1:
						{
							Assert.AreEqual(500, score.Item2);
						}
						break;
					case 2:
						{
							Assert.AreEqual(100, score.Item2);
						}
						break;
				}
				i++;
			}
		}

		[Test]
		public void InOrder_Initials()
		{
			_highScores.AddHighScore(tableName, 500, "2");
			_highScores.AddHighScore(tableName, 1000, "1");
			_highScores.AddHighScore(tableName, 100, "3");

			var scores = _highScores.GetHighScoreList(tableName, 10);

			int i = 0;
			foreach (var score in scores)
			{
				switch (i)
				{
					case 0:
						{
							Assert.AreEqual("1", score.Item1);
						}
						break;
					case 1:
						{
							Assert.AreEqual("2", score.Item1);
						}
						break;
					case 2:
						{
							Assert.AreEqual("3", score.Item1);
						}
						break;
				}
				i++;
			}
		}
	}
}
