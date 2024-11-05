//Author: Benjamin Huynh
//File name: Program.cs
//Project name: PASS2OOP
//Creation date: March 24, 2023
//Modified date: April 12, 2023
//Description: a 2d minecraft themed shooter, featuring different mobs and boosts

using Animation2D;
using Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PASS2OOP
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		//screen dimensions
		private int screenHeight;
		private int screenWidth;

		//gamestate options
		private const byte MENU = 0;
		private const byte STATS = 1;
		private const byte PREGAME = 2;
		private const byte GAME = 3;
		private const byte ENDGAME = 4;
		private const byte SHOP = 5;

		//numbers representing the boosts
		private const byte SPEED = 0;
		private const byte DAMAGE = 1;
		private const byte FIRE_RATE = 2;
		private const byte POINTS = 3;

		//numbers representing the mobs
		private const byte VILLAGER = 0;
		private const byte CREEPER = 1;
		private const byte SKELETON = 2;
		private const byte PILLAGER = 3;
		private const byte ENDERMAN = 4;

		//the max levels allowed
		private const byte MAX_LEVELS = 5;

		//shop button sizes
		private const int SHOP_BUTTON_SIZE_X = 300;
		private const int SHOP_BUTTON_SIZE_Y = 80;

		//shop prices
		private static readonly int[] SHOP_PRICES = { 100, 200, 300, 500 };

		//gamestate, starts in menu
		private byte gameState = MENU;

		//the boosts active or not
		private bool[] boostsActive = { false, false, false, false };

		//the previous and current mouse states
		private MouseState prevMouse;
		private MouseState mouse;

		//the fonts
		private SpriteFont eveningFont;
		private SpriteFont boldFont;
		private SpriteFont regularFont;

		//game images: bg, arrows, player, etc
		private Texture2D menuBackgroundImg;
		private Texture2D arrowUpImg;
		private Texture2D arrowDownImg;
		private Texture2D playerImg;
		private Texture2D shieldImg;
		private Texture2D explosionImg;
		private Texture2D buttonImg;

		//shop images, game bg images, mob images, boost images
		private Texture2D[] shopBoostImgs = new Texture2D[4];
		private Texture2D[] bgImgs = new Texture2D[4];
		private Texture2D[] mobImgs = new Texture2D[5];
		private Texture2D[] boostIcons = new Texture2D[4];

		//text locations
		private Vector2 continueTextLoc;
		private Vector2 menuPlayTextLoc;
		private Vector2 menuStatsTextLoc;
		private Vector2 menuExitTextLoc;
		private Vector2 statsHighScoreTextLoc;
		private Vector2 statsInfoTextLocL;
		private Vector2 statsInfoTextLocR;
		private Vector2 statsKillsTextLoc;
		private Vector2 instructionsTextLoc;
		private Vector2 gameScoreTextLoc;
		private Vector2 endgameStatsLoc;
		private Vector2 shopScoreTextLoc;
		private Vector2 shopContinueTextLoc;
		private Vector2[] endgameScoreLocs = new Vector2[MAX_LEVELS];

		//button and background rectangles
		private Rectangle menuBackroundRec;
		private Rectangle menuPlayButtonRec;
		private Rectangle menuStatsButtonRec;
		private Rectangle menuExitButtonRec;
		private Rectangle playerPathHazeRec;
		private Rectangle gameContinueButtonRec;
		private Rectangle[] shopButtonRecs;

		//player and level instances
		private Player player;
		private Level level;

		//global timer and level end timer
		private Timer gameTimer = new Timer(Timer.INFINITE_TIMER, true);
		private Timer settleLevelTimer = new Timer(1000, false);

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			IsMouseVisible = true;
			this.graphics.PreferredBackBufferWidth = 800;
			this.graphics.PreferredBackBufferHeight = 800;
			this.graphics.ApplyChanges();
			screenWidth = this.graphics.GraphicsDevice.Viewport.Width;
			screenHeight = this.graphics.GraphicsDevice.Viewport.Height;
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// TODO: use this.Content to load your game content here

			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			//load the fonts
			eveningFont = Content.Load<SpriteFont>("Fonts/MinecraftEvenings");
			boldFont = Content.Load<SpriteFont>("Fonts/MinecraftBold");
			regularFont = Content.Load<SpriteFont>("Fonts/MinecraftRegular");

			//load all the game images
			menuBackgroundImg = Content.Load<Texture2D>("BG/MenuBG3");
			shopBoostImgs[SPEED] = Content.Load<Texture2D>("Sprites/ShopSpeedBoost_150");
			shopBoostImgs[DAMAGE] = Content.Load<Texture2D>("Sprites/ShopDamageBoost_150");
			shopBoostImgs[FIRE_RATE] = Content.Load<Texture2D>("Sprites/ShopFireRateBoost_150");
			shopBoostImgs[POINTS] = Content.Load<Texture2D>("Sprites/ShopPointsBoost_150");
			bgImgs[0] = Content.Load<Texture2D>("BG/Cobblestone_64");
			bgImgs[1] = Content.Load<Texture2D>("BG/Dirt_64");
			bgImgs[2] = Content.Load<Texture2D>("BG/Grass1_64");
			bgImgs[3] = Content.Load<Texture2D>("BG/Grass2_64");
			mobImgs[0] = Content.Load<Texture2D>("Sprites/Villager_64");
			mobImgs[1] = Content.Load<Texture2D>("Sprites/Creeper_64");
			mobImgs[2] = Content.Load<Texture2D>("Sprites/Skeleton_64");
			mobImgs[3] = Content.Load<Texture2D>("Sprites/Pillager_64");
			mobImgs[4] = Content.Load<Texture2D>("Sprites/Enderman_64");
			boostIcons[SPEED] = Content.Load<Texture2D>("Sprites/IconSpeed_32");
			boostIcons[DAMAGE] = Content.Load<Texture2D>("Sprites/IconDamage_32");
			boostIcons[FIRE_RATE] = Content.Load<Texture2D>("Sprites/IconFireRate_32");
			boostIcons[POINTS] = Content.Load<Texture2D>("Sprites/IconPoints_32");
			arrowUpImg = Content.Load<Texture2D>("Sprites/ArrowUp");
			arrowDownImg = Content.Load<Texture2D>("Sprites/ArrowDown");
			playerImg = Content.Load<Texture2D>("Sprites/Steve_64");
			shieldImg = Content.Load<Texture2D>("Sprites/Shield_48");
			explosionImg = Content.Load<Texture2D>("Sprites/Explode_200");
			buttonImg = Content.Load<Texture2D>("Sprites/Button");

			//define button and haze rectangles
			menuBackroundRec = new Rectangle(0, 0, screenWidth, screenHeight);
			menuPlayButtonRec = new Rectangle((screenWidth - buttonImg.Width) / 2, screenHeight / 2 - buttonImg.Height / 2, buttonImg.Width, buttonImg.Height);
			menuStatsButtonRec = new Rectangle(menuPlayButtonRec.X, menuPlayButtonRec.Y + buttonImg.Height, buttonImg.Width, buttonImg.Height);
			menuExitButtonRec = new Rectangle(menuPlayButtonRec.X, menuStatsButtonRec.Y + buttonImg.Height, buttonImg.Width, buttonImg.Height);
			playerPathHazeRec = new Rectangle(0, screenHeight / 10 * 9, screenWidth, screenHeight / 10);
			gameContinueButtonRec = new Rectangle((screenWidth - buttonImg.Width) / 2, screenHeight / 4 - buttonImg.Height / 2, buttonImg.Width, buttonImg.Height);

			//define the shop button rectangles
			shopButtonRecs = new Rectangle[]{ new Rectangle(screenWidth / 10,
															screenHeight / 2,
															SHOP_BUTTON_SIZE_X,
															SHOP_BUTTON_SIZE_Y),
											  new Rectangle(screenWidth / 10,
															screenHeight / 2 + SHOP_BUTTON_SIZE_Y * 2,
															SHOP_BUTTON_SIZE_X,
															SHOP_BUTTON_SIZE_Y),
											  new Rectangle(screenWidth / 10 * 9 - SHOP_BUTTON_SIZE_X,
															screenHeight / 2,
															SHOP_BUTTON_SIZE_X,
															SHOP_BUTTON_SIZE_Y),
											  new Rectangle(screenWidth / 10 * 9 - SHOP_BUTTON_SIZE_X,
															screenHeight / 2 + SHOP_BUTTON_SIZE_Y * 2,
															SHOP_BUTTON_SIZE_X,
															SHOP_BUTTON_SIZE_Y) };

			//define the vectors for text
			continueTextLoc = new Vector2((screenWidth - boldFont.MeasureString("Press enter to continue").X) / 2, screenHeight / 10 * 8);
			menuPlayTextLoc = new Vector2(menuPlayButtonRec.X + menuPlayButtonRec.Width / 2 - boldFont.MeasureString("Play").X / 2,
										  menuPlayButtonRec.Bottom - menuPlayButtonRec.Height / 2 - boldFont.MeasureString("Play").Y / 2);
			menuStatsTextLoc = new Vector2(menuStatsButtonRec.X + menuStatsButtonRec.Width / 2 - boldFont.MeasureString("Stats").X / 2,
										  menuStatsButtonRec.Bottom - menuStatsButtonRec.Height / 2 - boldFont.MeasureString("Stats").Y / 2);
			menuExitTextLoc = new Vector2(menuExitButtonRec.X + menuExitButtonRec.Width / 2 - boldFont.MeasureString("Exit").X / 2,
										  menuExitButtonRec.Bottom - menuExitButtonRec.Height / 2 - boldFont.MeasureString("Exit").Y / 2);
			statsHighScoreTextLoc = new Vector2((screenWidth - eveningFont.MeasureString("Highscore: xxxxx").X) / 2, screenHeight / 10);
			statsInfoTextLocL = new Vector2(screenWidth / 20, statsHighScoreTextLoc.Y + eveningFont.MeasureString("l").Y);
			statsInfoTextLocR = new Vector2(statsInfoTextLocL.X + regularFont.MeasureString("xxxxxxxxxx: xxxxxxxx").X, statsInfoTextLocL.Y);
			statsKillsTextLoc = new Vector2((screenWidth - eveningFont.MeasureString("Total kills: xxxx").X) / 2, statsInfoTextLocL.Y + regularFont.MeasureString("l").Y * 6);
			instructionsTextLoc = new Vector2(0, screenHeight / 3);
			gameScoreTextLoc = new Vector2(0, playerPathHazeRec.Y - regularFont.MeasureString("l").Y);
			shopScoreTextLoc = new Vector2(0, screenHeight - regularFont.MeasureString("l").Y);
			shopContinueTextLoc = new Vector2(gameContinueButtonRec.X + gameContinueButtonRec.Width / 2 - boldFont.MeasureString("Continue").X / 2,
											  gameContinueButtonRec.Bottom - gameContinueButtonRec.Height / 2 - boldFont.MeasureString("Play").Y / 2);

			//define the vectors of the scores displayed at the end
			for (int i = 0; i < MAX_LEVELS; i++)
			{
				//define the vectors a subjective amount away from each other
				endgameScoreLocs[i] = new Vector2(screenWidth / 20, screenHeight / 2 + (i * 40)); 
			}

			//has to be done after endgameScoreLocs
			endgameStatsLoc = new Vector2(endgameScoreLocs[0].X + regularFont.MeasureString("Level x score: xxxxxxxx").X, endgameScoreLocs[0].Y);

			//instanciate the player and the level
			player = new Player(gameTimer);
			level = new Level(mobImgs, bgImgs, boostIcons, arrowDownImg, shieldImg, explosionImg, player, screenWidth, screenHeight);
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			// TODO: Add your update logic here

			//get the current and previous state of the mouse
			prevMouse = mouse;
			mouse = Mouse.GetState();

			//perform logic based on the game's state
			switch (gameState)
			{
				case MENU:
					//update the menu
					UpdateMenu();
					break;
				case STATS:
					//go to the menu if enter is pressed
					if (Keyboard.GetState().IsKeyDown(Keys.Enter))
					{
						//go to the menu
						gameState = MENU;
					}
					break;
				case PREGAME:
					//go to the game if enter is pressed
					if (Keyboard.GetState().IsKeyDown(Keys.Enter))
					{
						//go to the game
						gameState = GAME;
					}
					break;
				case GAME:
					//update the game
					UpdateGame(gameTime);
					break;
				case ENDGAME:
					//update the endgame
					UpdateEndgame();
					break;
				case SHOP:
					//update the shop
					UpdateShop();
					break;
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
			spriteBatch.Begin();

			//draws the current game state
			switch (gameState)
			{
				case MENU:
					//draw the menu bg and it's buttons
					spriteBatch.Draw(menuBackgroundImg, menuBackroundRec, Color.White);
					HoverColour(boldFont, "Play", menuPlayTextLoc, menuPlayButtonRec);
					HoverColour(boldFont, "Stats", menuStatsTextLoc, menuStatsButtonRec);
					HoverColour(boldFont, "Exit", menuExitTextLoc, menuExitButtonRec);
					break;
				case STATS:
					//draw the stats
					DrawStats();
					break;
				case PREGAME:
					//draw the pregame
					DrawGame();
					DrawPregame();
					break;
				case GAME:
					//draw the game
					DrawGame();
					break;
				case ENDGAME:
					//draw the endgame
					DrawGame();
					DrawEndgame();
					break;
				case SHOP:
					//draw the shop
					DrawShop();
					break;
			}
			spriteBatch.End();

			base.Draw(gameTime);
		}

		//PRE: 
		//POST: 
		//DESC: update the menu; allow 3 choices
		private void UpdateMenu()
		{
			//if clicked, do the following: go to the game, go to the stats, or exit 
			if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
			{
				//go to the game, the stats, or exit
				if (menuPlayButtonRec.Contains(mouse.Position))
				{
					//go to pregame
					gameState = PREGAME;
				}
				else if (menuStatsButtonRec.Contains(mouse.Position) && player.GetGamesPlayed() != 0)
				{
					//go to stats
					gameState = STATS;
				}
				else if (menuExitButtonRec.Contains(mouse.Position))
				{
					//exit
					Exit();
				}
			}
		}

		//PRE: game time recording the.. uhh.. game time
		//POST: 
		//DESC: update the game
		private void UpdateGame(GameTime gameTime)
		{
			//update the global timer
			gameTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

			//update the player
			player.UpdatePlayer();

			//update the level
			level.UpdateLevel(gameTimer);

			//if the level is done, perform needed actions and go to endgame
			if (level.IsLevelDone())
			{
				//activate and update the level settle timer
				settleLevelTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
				settleLevelTimer.Activate();

				if (settleLevelTimer.IsFinished())
				{
					//reset the timer for letting the dust of the level settle, update the player's stats
					settleLevelTimer.ResetTimer(false);
					player.UpdateLevelStats(level.GetLevel());

					//disable the boosts if the player was shot
					if (level.BoostsDisabled())
					{
						//clear boosts
						boostsActive = new bool[]{ false, false, false, false };
					}

					//setup the next level
					level.NextLevel();

					//go to endgame
					gameState = ENDGAME;
				}
			}
		}

		//PRE: 
		//POST: 
		//DESC: allow the player to view their stats
		private void UpdateEndgame()
		{
			//continue if enter is pressed
			if (Keyboard.GetState().IsKeyDown(Keys.Enter))
			{
				//go to the shop it it is not level 5 yet, else, back to the menu
				if (level.GetLevel() != MAX_LEVELS)
				{
					// go to the shop
					gameState = SHOP;
				}
				else
				{
					//save the player's progress, reset everything, go to menu
					player.UpdateGameStats();
					player.SaveProgress();
					player = new Player(gameTimer);
					level = new Level(mobImgs, bgImgs, boostIcons, arrowDownImg, shieldImg, explosionImg, player, screenWidth, screenHeight);
					gameState = MENU;
				}
			}
		}

		//PRE: 
		//POST: 
		//DESC: allow the player to get boosts and go to the game
		private void UpdateShop()
		{
			//go to the game if the player clicks the button
			if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed && gameContinueButtonRec.Contains(mouse.Position))
			{
				//apply boosts, clear last level's stats, go to the game
				level.UpdateBoosts(boostsActive);
				player.ClearLevelStats();
				gameState = GAME;
			}

			//allow the player to buy powerups
			for (int i = 0; i < shopButtonRecs.Length; i++)
			{
				//if the player presses the button, see if they can buy it
				if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed && shopButtonRecs[i].Contains(mouse.Position))
				{
					//give the player the powerup if they can afford it
					if (SHOP_PRICES[i] <= player.GetScoresSum())
					{
						//charge the player and update the boost
						player.UpdateScore(-SHOP_PRICES[i], true);
						player.UpdateLevelStats(level.GetLevel() - 1);
						boostsActive[i] = true;
						player.UpdateBoosts(boostsActive);
					}
				}
			}
		}

		//PRE: 
		//POST: 
		//DESC: draw all statistics
		private void DrawStats()
		{
			//draw the background and every statistic
			spriteBatch.Draw(menuBackgroundImg, menuBackroundRec, Color.White);
			spriteBatch.DrawString(eveningFont, "HIGHSCORE: " + player.GetHighScore(), statsHighScoreTextLoc, Color.CornflowerBlue);
			spriteBatch.DrawString(regularFont, "Games played: " + player.GetGamesPlayed() +
												"\nShots fired: " + player.GetTotalShotsFired() +
												"\nShots hit: " + player.GetTotalShotsHit() +
												"\nTop hit %: " + player.GetTopAccuracy() + "%" +
												"\nAvg hit %: " + player.GetAverageAccuracy() + "%" +
												"\nAvg shots: " + (player.GetTotalShotsFired() / player.GetGamesPlayed()), statsInfoTextLocL, Color.Orchid);
			spriteBatch.DrawString(regularFont, "Villager kills: " + player.GetTotalKills()[VILLAGER] +
												"\nCreeper kills: " + player.GetTotalKills()[CREEPER] +
												"\nSkeleton kills: " + player.GetTotalKills()[SKELETON] +
												"\nPillager kills: " + player.GetTotalKills()[PILLAGER] +
												"\nEnderman kills: " + player.GetTotalKills()[ENDERMAN] +
												"\nAvg kills: " + (player.GetTotalShotsHit() / player.GetGamesPlayed()), statsInfoTextLocR, Color.Orchid);
			spriteBatch.DrawString(eveningFont, "TOTAL KILLS: " + player.GetTotalKills().Sum(), statsKillsTextLoc, Color.MediumVioletRed);
			spriteBatch.DrawString(boldFont, "Press ENTER to continue", continueTextLoc, Color.White);
		}

		//PRE: 
		//POST: 
		//DESC: hive quick instuctions
		private void DrawPregame()
		{
			//give instruction
			spriteBatch.DrawString(boldFont, "Move: A & D       Shoot: E\n" +
											 "Goal: Kill mobs\n", instructionsTextLoc, Color.White); 
			spriteBatch.DrawString(boldFont, "Press ENTER to continue", continueTextLoc, Color.White);
		}

		//PRE: 
		//POST: 
		//DESC: draw the game and it's objects and information 
		private void DrawGame()
		{
			//draw the level, the player, haze of the area the player can move in
			level.DrawLevel(spriteBatch);
			spriteBatch.Draw(playerImg, player.GetRectangle(), Color.White);
			spriteBatch.Draw(buttonImg, playerPathHazeRec, Color.DarkGray * 0.2f);

			//draw the player's arrows
			for (int i = 0; i < player.GetArrows().Count; i++)
			{
				//draw the arrow
				spriteBatch.Draw(arrowUpImg, player.GetArrows()[i].GetRectangle(), Color.White);
			}

			//draw the score of the level
			spriteBatch.DrawString(regularFont, "Score: " + player.GetScore(), gameScoreTextLoc, Color.Yellow);
		}

		//PRE: 
		//POST: 
		//DESC: draws endgame stats
		private void DrawEndgame()
		{
			//draws scores of each level
			for (int i = 0; i < MAX_LEVELS; i++)
			{
				//draws the level's score
				spriteBatch.DrawString(regularFont, "Level " + (i + 1) + " score: " + player.GetAllScores()[i], endgameScoreLocs[i], Color.White);
			}

			//draw the level stats, prompt continue
			spriteBatch.DrawString(regularFont, "Level: " + level.GetLevel() +
												"\nKills: " + player.GetKills()[level.GetLevel() - 1] +
												"\nShots fired: " + player.GetArrowsShot()[level.GetLevel() - 1] +
												"\nShots hit: " + player.GetArrowsHit()[level.GetLevel() - 1] +
												"\nAccuracy: " + Math.Round(player.GetArrowsHit()[level.GetLevel() - 1] * 100 / (double)player.GetArrowsShot()[level.GetLevel() - 1], 2) + "%",
												endgameStatsLoc, Color.White);
			spriteBatch.DrawString(boldFont, "Press ENTER to continue", continueTextLoc, Color.White);

			//congratulate the player for getting through the game and getting a new high score
			if (level.GetLevel() == MAX_LEVELS)
			{
				//congratuate the player
				spriteBatch.DrawString(boldFont, "\nGood game! Stats added", continueTextLoc, Color.Yellow);

				//congratulate the user for a new high score if possible
				if (player.GetScoresSum() > player.GetHighScore())
				{
					//congratulate the user for a new high score
					spriteBatch.DrawString(boldFont, "New highscore! Yippee!", statsHighScoreTextLoc, Color.Yellow);
				}
			}
		}

		//PRE: 
		//POST: 
		//DESC: draw the shop buttons
		private void DrawShop()
		{
			//draw the background, button, text
			spriteBatch.Draw(menuBackgroundImg, menuBackroundRec, Color.White);
			spriteBatch.DrawString(regularFont, "Score: " + player.GetScoresSum(), shopScoreTextLoc, Color.Yellow);
			HoverColour(boldFont, "Continue", shopContinueTextLoc, gameContinueButtonRec);

			//draw the boost buttons based on wealth and if the player has it
			for (int i = 0; i < SHOP_PRICES.Length; i++)
			{
				//the button is normal if the player has enough points or doenst have that boost yet, else, grey and translucent
				if (player.GetScoresSum() >= SHOP_PRICES[i] && !boostsActive[i])
				{
					//draw the button
					spriteBatch.Draw(shopBoostImgs[i], shopButtonRecs[i], Color.White);
				}
				else
				{
					//draw the button but ghostly
					spriteBatch.Draw(shopBoostImgs[i], shopButtonRecs[i], Color.LightGray * 0.5f);
				}
			}
		}

		//PRE: the font used, text written, text location, button rectangle
		//POST: 
		//DESC: draws a button and it's text for when the mouse if over it or not
		private void HoverColour(SpriteFont font, string text, Vector2 textLoc, Rectangle buttonRec)
		{
			//if the mouse in over the button, draw it darker with yellow text
			if (buttonRec.Contains(mouse.Position))
			{
				//dark button, yellow text
				spriteBatch.Draw(buttonImg, buttonRec, Color.Gray);
				spriteBatch.DrawString(font, text, textLoc, Color.Yellow);
			}
			else
			{
				//light button, white text
				spriteBatch.Draw(buttonImg, buttonRec, Color.White);
				spriteBatch.DrawString(font, text, textLoc, Color.White);
			}
		}
	}
}
