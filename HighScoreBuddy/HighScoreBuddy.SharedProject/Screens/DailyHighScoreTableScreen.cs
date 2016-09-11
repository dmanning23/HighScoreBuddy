using FontBuddyLib;
using HighScoreBuddy;
using InsertCoinBuddy;
using MenuBuddy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ResolutionBuddy;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace HighScoreBuddy
{
	/// <summary>
	/// This is a screen that shows one of the high scores tables
	/// </summary>
	public class DailyHighScoreTableScreen : WidgetScreen
	{
		#region Properties

		/// <summary>
		/// A list of all the high scores in the game
		/// </summary>
		private List<string> Scores { get; set; }

		/// <summary>
		/// buddy to draw all the text
		/// </summary>
		private IFontBuddy ScoresFont;

		/// <summary>
		/// how fast the scores scroll by
		/// </summary>
		private const float ScrollSpeed = 90.0f;

		/// <summary>
		/// how long the scores are displayed before they start scrolling
		/// </summary>
		private const float InitialPause = 1.0f;

		/// <summary>
		/// how long to pause on the top score before screen exits
		/// </summary>
		private const float TopScorePause = 2.6f;

		/// <summary>
		/// how long to display this screen before popping it off
		/// </summary>
		private const float HowLongToDisplayScreen = 25.0f;

		private string ScoreListKey { get; set; }

		private int NumHighScores { get; set; }

		#endregion //Properties

		#region Initialization

		/// <summary>
		/// Constructor fills in the menu contents.
		/// </summary>
		public DailyHighScoreTableScreen(string scoreList, int numHighScores =10)
			: base("Today's High Scores")
		{
			//move the menu title to the top of the screen
			MenuTitleOffset = -256;

			//create the list of credits
			Scores = new List<string>();
			NumHighScores = numHighScores;
			ScoreListKey = scoreList;
		}

		/// <summary>
		/// Load graphics content for the screen.
		/// </summary>
		public override void LoadContent()
		{
			base.LoadContent();

			ScoresFont = new FontBuddy();
			ScoresFont = StyleSheet.Instance().MediumNeutralFont;

			//get the correct high score list
			var highScoreComponent = ScreenManager.Game.Services.GetService<IHighScoreTable>();
			HighScoreList HighScores = screens.HighScores.GetHighScoreList(ScoreListKey);

			//Add "High Scores" to the top of the list
			Scores.Add(ScoreListKey);

			//add each high score as a menu entry
			for (int i = 0; i < HighScores.Entries.Count; i++)
			{
				StringBuilder entryText = new StringBuilder();
				entryText.Append((i + 1).ToString());
				entryText.Append(".  ");
				entryText.Append(HighScores.Entries[i].PlayerName);
				entryText.Append("  ");
				entryText.Append(HighScores.Entries[i].Score.ToString());

				Scores.Add(entryText.ToString());
			}
		}

		#endregion //Initialization

		#region Update and Draw

		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

			//if all the credits are finished displaying, exit this screen
			if (TimeSinceInput.CurrentTime > HowLongToDisplayScreen)
			{
				ExitScreen();
			}
		}

		/// <summary>
		/// Draws the menu.
		/// </summary>
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			//the position to start drawing text
			Vector2 textPos = new Vector2(MenuEntryPositionX(), Resolution.TitleSafeArea.Bottom * 0.6f);

			//scroll the text up 
			if (TimeSinceInput.CurrentTime > InitialPause)
			{
				if (TimeSinceInput.CurrentTime >= (HowLongToDisplayScreen - TopScorePause))
				{
					textPos.Y += ((HowLongToDisplayScreen - TopScorePause) - InitialPause) * ScrollSpeed;
				}
				else
				{
					textPos.Y += (TimeSinceInput.CurrentTime - InitialPause) * ScrollSpeed;
				}
			}


			//Drwa all the credit text
			ScreenManager.SpriteBatchBegin();
			for (int i = Scores.Count - 1; i >= 0 ; i--)
			{
				string text = Scores[i];
				ScoresFont.Write(text, textPos, Justify.Center, 1.0f, FadeAlphaDuringTransition(Color.White), ScreenManager.SpriteBatch, gameTime.TotalGameTime.TotalSeconds);
				textPos.Y -= ScoresFont.Font.LineSpacing * 0.9f;
			}
			ScreenManager.SpriteBatchEnd();
		}

		#endregion //Update and Draw
	}
}
