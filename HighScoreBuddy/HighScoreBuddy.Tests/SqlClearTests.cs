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
	public class SqlClearTests
	{
		IHighScoreTable _highScores;

		private const string tableName = "testList";

		[SetUp]
		public void Setup()
		{
			_highScores = new SqlHighScoreTable();
			_highScores.InitializeDatabase();
		}

		[Test]
		public void VerifyEmpty()
		{
			_highScores.ClearHighScoreList(tableName);
			var scores = _highScores.GetHighScoreList(tableName, 100);
			Assert.AreEqual(0, scores.Count());
		}

		[Test]
		public void AddAndClear()
		{
			_highScores.AddHighScore("testList", 100, "ass");
			_highScores.ClearHighScoreList(tableName);
			var scores = _highScores.GetHighScoreList(tableName, 100);
			Assert.AreEqual(0, scores.Count());
		}
	}
}
