using FontBuddyLib;
using MenuBuddy;
using Microsoft.Xna.Framework;
using ResolutionBuddy;
using System;
using System.Collections.Generic;

namespace HighScoreBuddy
{
	/// <summary>
	/// This is a screen that shows one of the high scores tables
	/// </summary>
	public class HighScoreTableScreen : WidgetScreen
	{
		#region Properties

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

		protected string HighScoreList { get; set; }

		protected int NumHighScores { get; set; }

		private ScrollLayout Scroller { get; set; }

		#endregion //Properties

		#region Initialization

		/// <summary>
		/// Constructor fills in the menu contents.
		/// </summary>
		public HighScoreTableScreen(string screenTitle, string highScoreList, int num)
			: base(screenTitle)
		{
			HighScoreList = highScoreList;
			NumHighScores = num;
		}

		/// <summary>
		/// Load graphics content for the screen.
		/// </summary>
		public override void LoadContent()
		{
			base.LoadContent();

			//Get the high scores
			var highScores = GetHighScores();

			//Add the scroll layout
			Scroller = new ScrollLayout()
			{
				Position = new Point(0, 0),
				Size = Resolution.ScreenArea.Size.ToVector2(),
				
			};
			AddItem(Scroller);

			//Add the name of the high score list
			var title = new Label(ScreenName, FontSize.Large)
			{
				Vertical = VerticalAlignment.Top, 
				Horizontal = HorizontalAlignment.Center,
				Position = new Point(Resolution.ScreenArea.Center.X, Resolution.TitleSafeArea.Top),
				Transition = new WipeTransitionObject(TransitionWipeType.PopTop)
			};
			Scroller.AddItem(title);

			//create the stack layout to hold the scores
			var scoreStack = new StackLayout()
			{
				Vertical = VerticalAlignment.Top,
				Horizontal = HorizontalAlignment.Center,
				Position = new Point(Resolution.ScreenArea.Center.X, title.Rect.Bottom),
				Alignment = StackAlignment.Top
			};

			//add each high score
			int index = 1;
			foreach (var highScore in highScores)
			{
				//add the number to the left
				var number = new Label(index.ToString() + ".", FontSize.Medium)
				{
					Horizontal = HorizontalAlignment.Left,
					Vertical = VerticalAlignment.Top
				};

				//add the initials in teh center
				var initials = new Label(highScore.Item1, FontSize.Medium)
				{
					Horizontal = HorizontalAlignment.Center,
					Vertical = VerticalAlignment.Top
				};

				//add the score to the right
				var score = new Label(highScore.Item1, FontSize.Medium)
				{
					Horizontal = HorizontalAlignment.Right,
					Vertical = VerticalAlignment.Top
				};

				//create the layout item
				var scoreLayout = new RelativeLayout()
				{
					Vertical = VerticalAlignment.Top,
					Horizontal = HorizontalAlignment.Center,
					Size = new Vector2(Resolution.TitleSafeArea.Width, initials.Rect.Height)
				};

				//add all the items to the layout
				scoreLayout.AddItem(number);
				scoreLayout.AddItem(initials);
				scoreLayout.AddItem(score);

				//add to hte score stack
				scoreStack.AddItem(scoreLayout);

				//make sure to increment the index
				index++;
			}
		}

		protected virtual IEnumerable<Tuple<string, uint>> GetHighScores()
		{
			var highScoreComponent = ScreenManager.Game.Services.GetService<IHighScoreTable>();
			return highScoreComponent.GetHighScoreList(HighScoreList, NumHighScores);
		}

		#endregion //Initialization

		#region Update and Draw

		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

			//the position to start drawing text
			Vector2 textPos = Scroller.ScrollPosition;

			//scroll the text up 
			if (TimeSinceInput.CurrentTime > InitialPause)
			{
				if (TimeSinceInput.CurrentTime >= (HowLongToDisplayScreen - TopScorePause))
				{
					textPos.Y += ((HowLongToDisplayScreen - TopScorePause) - InitialPause) * (ScrollSpeed * Time.TimeDelta);
				}
				else
				{
					textPos.Y += (TimeSinceInput.CurrentTime - InitialPause) * (ScrollSpeed * Time.TimeDelta);
				}
			}

			Scroller.ScrollPosition = textPos;

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
			//fade the background
			ScreenManager.SpriteBatchBegin();
			FadeBackground();
			ScreenManager.SpriteBatchEnd();

			base.Draw(gameTime);
		}

		#endregion //Update and Draw
	}
}
