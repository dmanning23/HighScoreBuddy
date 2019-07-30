using FontBuddyLib;
using MenuBuddy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ResolutionBuddy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
		private const float ScrollSpeed = 180f;

		/// <summary>
		/// how long the scores are displayed before they start scrolling
		/// </summary>
		private const float InitialPause = 1.5f;

		protected string HighScoreList { get; set; }

		protected int NumHighScores { get; set; }

		private ScrollLayout Scroller { get; set; }

		/// <summary>
		/// Setting this flag will push the names to be displayed directly after the number, to the far left
		/// </summary>
		public bool HasLongNames { get; set; }

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
		public override async Task LoadContent()
		{
			await base.LoadContent();

			//Get the high scores
			var highScores = GetHighScores();

			//Add the name of the high score list
			var title = new Label(ScreenName, Content, FontSize.Large)
			{
				Vertical = VerticalAlignment.Top,
				Horizontal = HorizontalAlignment.Center,
				Position = new Point(Resolution.ScreenArea.Center.X, Resolution.TitleSafeArea.Top),
				TransitionObject = new WipeTransitionObject(TransitionWipeType.PopTop),
				Highlightable = false,
			};
			title.ShrinkToFit((int)(Resolution.TitleSafeArea.Width * 0.6f));
			AddItem(title);

			//Add the scroll layout
			Scroller = new ScrollLayout()
			{
				Position = new Point(0, title.Rect.Bottom),
				Size = new Vector2(Resolution.ScreenArea.Width, Resolution.ScreenArea.Height - title.Rect.Bottom),
				Vertical = VerticalAlignment.Top,
				Horizontal = HorizontalAlignment.Left
			};
			AddItem(Scroller);

			//create the stack layout to hold the scores
			var scoreStack = new StackLayout()
			{
				Vertical = VerticalAlignment.Top,
				Horizontal = HorizontalAlignment.Center,
				Position = new Point(Resolution.ScreenArea.Center.X, 0),
				Alignment = StackAlignment.Top
			};

			//add each high score
			int index = 1;
			foreach (var highScore in highScores)
			{
				//add the number to the left
				var number = new Label($"{index.ToString()}.", Content, FontSize.Medium)
				{
					Horizontal = HorizontalAlignment.Left,
					Vertical = VerticalAlignment.Top,
					Highlightable = false,
				};

				//add the initials in teh center
				var initials = new Label(highScore.Item1, Content, FontSize.Medium)
				{
					Horizontal = HorizontalAlignment.Center,
					Vertical = VerticalAlignment.Top,
					Highlightable = false,
				};

				//add the score to the right
				var score = new Label(highScore.Item2.ToString(), Content, FontSize.Medium)
				{
					Horizontal = HorizontalAlignment.Right,
					Vertical = VerticalAlignment.Top,
					Highlightable = false,
				};

				//If this is the top score, use a big rainbow font
				if (index == 1)
				{
					//use a big gay rainbow font for teh top score
					var topScoreFont = new RainbowTextBuddy();
					topScoreFont.Font = Content.Load<SpriteFont>(StyleSheet.MediumFontResource);

					number.Font = topScoreFont;
					initials.Font = topScoreFont;
					score.Font = topScoreFont;
				}

				//create the layout item
				var scoreLayout = new RelativeLayout()
				{
					Vertical = VerticalAlignment.Top,
					Horizontal = HorizontalAlignment.Center,
					Size = new Vector2(Resolution.TitleSafeArea.Width, initials.Rect.Height),
					Layer = index
				};

				//add all the items to the layout
				scoreLayout.AddItem(number);

				if (HasLongNames)
				{
					scoreLayout.AddItem(new PaddedLayout(85, 0, 0, 0, initials)
					{
						Horizontal = HorizontalAlignment.Left,
						Vertical = VerticalAlignment.Top,
					});
				}
				else
				{
					scoreLayout.AddItem(initials);
				}
				
				scoreLayout.AddItem(score);

				//add to hte score stack
				scoreStack.AddItem(scoreLayout);

				//make sure to increment the index
				index++;
			}

			Scroller.AddItem(scoreStack);

			//Scroll all the way to the bottom
			Scroller.ScrollPosition = Scroller.MaxScroll;
		}

		protected virtual IEnumerable<Tuple<string, uint>> GetHighScores()
		{
			var highScoreComponent = ScreenManager.Game.Services.GetService<IHighScoreTable>();

			if (null == highScoreComponent)
			{
				throw new Exception("No IHighScoreTable found in Game.Services. Make sure to create a HighScoreTableComponent on game startup!");
			}

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
				textPos.Y -= (ScrollSpeed * Time.TimeDelta);
				Scroller.ScrollPosition = textPos;
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
