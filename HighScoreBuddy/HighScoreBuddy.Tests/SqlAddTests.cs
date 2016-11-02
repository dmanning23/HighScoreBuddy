using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HighScoreBuddy;

namespace HighScoreBuddy.Tests
{
	[TestFixture]
	public class SqlAddTests
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
		public void VerifyEmpty()
		{
			var scores = _highScores.GetHighScoreList(tableName, 100);
			Assert.AreEqual(0, scores.Count());
		}

		[Test]
		public void AddOne()
		{
			_highScores.AddHighScore(tableName, 100, "ass");
			var scores = _highScores.GetHighScoreList(tableName, 100);
			Assert.AreEqual(1, scores.Count());
		}

		[Test]
		public void GetInitials()
		{
			_highScores.AddHighScore(tableName, 100, "ass");
			var scores = _highScores.GetHighScoreList(tableName, 100);
			Assert.AreEqual("ass", scores.First().Item1);
		}

		[Test]
		public void GetScore()
		{
			_highScores.AddHighScore(tableName, 100, "ass");
			var scores = _highScores.GetHighScoreList(tableName, 100);
			Assert.AreEqual(100, scores.First().Item2);
		}

		[Test]
		public void AddExact()
		{
			for (uint i = 0; i < 100; i++)
			{
				_highScores.AddHighScore(tableName, i * 100, i.ToString());
			}
			
			var scores = _highScores.GetHighScoreList(tableName, 100);
			Assert.AreEqual(100, scores.Count());
		}

		[Test]
		public void AddTooMany()
		{
			for (uint i = 0; i < 1000; i++)
			{
				_highScores.AddHighScore(tableName, i * 100, i.ToString());
			}

			var scores = _highScores.GetHighScoreList(tableName, 100);
			Assert.AreEqual(100, scores.Count());
		}
	}
}
