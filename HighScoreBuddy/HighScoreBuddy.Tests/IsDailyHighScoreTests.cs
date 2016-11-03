using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighScoreBuddy.Tests
{
	[TestFixture]
	public class IsDailyHighScoreTests
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
		public void ZeroList()
		{
			Assert.IsFalse(_highScores.IsDailyHighScore(tableName, 100, 0));
		}

		[Test]
		public void EmptyTable()
		{
			Assert.IsTrue(_highScores.IsDailyHighScore(tableName, 100, 10));
		}

		[Test]
		public void HalfTable()
		{
			for (uint i = 0; i < 5; i++)
			{
				_highScores.AddHighScore(tableName, i * 100, i.ToString());
			}

			Assert.IsTrue(_highScores.IsDailyHighScore(tableName, 100, 10));
		}

		[Test]
		public void FullTable_IsHighScore()
		{
			for (uint i = 10; i < 20; i++)
			{
				_highScores.AddHighScore(tableName, i * 100, i.ToString());
			}

			Assert.IsTrue(_highScores.IsDailyHighScore(tableName, 1100, 10));
		}

		[Test]
		public void FullTable_NotHighScore()
		{
			for (uint i = 10; i < 20; i++)
			{
				_highScores.AddHighScore(tableName, i * 100, i.ToString());
			}

			Assert.IsFalse(_highScores.IsDailyHighScore(tableName, 10, 10));
		}

		[Test]
		public void FirstScoreOfToday()
		{
			var yesterday = DateTime.Now - TimeSpan.FromDays(1);
			for (uint i = 10; i < 20; i++)
			{
				_highScores.AddHighScore(tableName, i * 100, i.ToString(), yesterday);
			}

			Assert.IsTrue(_highScores.IsDailyHighScore(tableName, 10, 10));
		}

		[Test]
		public void SeveralScoresToday()
		{
			var yesterday = DateTime.Now - TimeSpan.FromDays(1);
			for (uint i = 10; i < 20; i++)
			{
				_highScores.AddHighScore(tableName, i * 100, i.ToString(), yesterday);
			}

			for (uint i = 10; i < 15; i++)
			{
				_highScores.AddHighScore(tableName, i * 100, i.ToString());
			}

			Assert.IsTrue(_highScores.IsDailyHighScore(tableName, 10, 10));
		}

		[Test]
		public void NotTodayHighScore()
		{
			var yesterday = DateTime.Now - TimeSpan.FromDays(1);
			for (uint i = 10; i < 20; i++)
			{
				_highScores.AddHighScore(tableName, i * 100, i.ToString(), yesterday);
			}

			for (uint i = 10; i < 20; i++)
			{
				_highScores.AddHighScore(tableName, i * 100, i.ToString());
			}

			Assert.IsFalse(_highScores.IsDailyHighScore(tableName, 10, 10));
		}
	}
}
