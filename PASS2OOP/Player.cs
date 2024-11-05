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
	class Player
	{
		//define stream read/write for file I/O
		private static StreamWriter outFile;
		private static StreamReader inFile;

		//numbers representing the boosts
		private const byte SPEED = 0;
		private const byte DAMAGE = 1;
		private const byte FIRE_RATE = 2;
		private const byte POINTS = 3;

		//the boosted and normal player speed
		private const int NORMAL_PLAYER_SPEED = 5;
		private const int BOOST_PLAYER_SPEED = 10;

		//global timer
		private Timer timer;

		//keyboard states
		private KeyboardState kb;
		private KeyboardState prevKB;

		//firerates
		private const int DEFAULT_FIRERATE = 333;
		private const int BOOSTED_FIRERATE = 125;

		private Vector2 playerLoc;
		private Rectangle hitBox;

		//game character stats
		private int playerSpeed = NORMAL_PLAYER_SPEED;
		private int playerDamage = 1;
		private int scoreMultiplier = 1;

		//list containing the player's arrows
		private List<Arrow> arrows = new List<Arrow>(32);

		//stats about the player for one level
		private int score = 0;
		private int kills = 0;
		private int arrowsShot = 0;
		private int arrowsHit = 0;

		//stats about the player for one game
		private int[] levelScores = new int[5] { 0, 0, 0, 0, 0 };
		private int[] levelKills = new int[5];
		private int[] levelArrowsShot = new int[5];
		private int[] levelArrowsHit = new int[5];

		//stats about the player overall
		private int gamesPlayed = 0;
		private double topAccuracy = 0;
		private int highScore = 0;
		private int[] totalKills = new int[5] { 0, 0, 0, 0, 0 };
		private int totalShotsFired = 0;
		private int totalShotsHit = 0;

		//the frequency of shots
		private int currentFirerate = DEFAULT_FIRERATE;

		//pseudo timers for stun and arrow cooldown
		private double stunUntil = 0;
		private double arrowCooldown = 0;

		/// <summary>
		/// the player, holds all the game statistics, shoots, moves
		/// </summary>
		/// <param name="timer"></param>
		public Player(Timer timer)
		{
			//take in the global timer
			this.timer = timer;

			//spawn the player in
			playerLoc = new Vector2(400, 720);
			hitBox = new Rectangle((int)playerLoc.X, (int)playerLoc.Y, 80, 80);

			//load stats
			LoadProgress();
		}

		//PRE: 
		//POST: 
		//DESC: updates the player: movement, shooting
		public void UpdatePlayer()
		{
			//get the previous and current keyboard states
			prevKB = kb;
			kb = Keyboard.GetState();

			//let the player moves and shoot if not stunned
			if (stunUntil < timer.GetTimePassed())
			{
				//move the player
				Move(kb);

				//shoot an arrow if the player presses "E"
				if (kb.IsKeyDown(Keys.E) && !prevKB.IsKeyDown(Keys.E))
				{
					//shoot the arrow
					ShootArrow();
				}
			}

			//updates the arrows
			UpdateArrows();
		}


		//PRE: 
		//POST: 
		//DESC: update the arrows; move them
		private void UpdateArrows()
		{
			//move the arrows and delete if needed
			for (int i = 0; i < GetArrows().Count; i++)
			{
				//move the arrow
				GetArrows()[i].Move();

				//remove if it is offscreen
				if (GetArrows()[i].GetRectangle().Bottom <= 0)
				{
					//remove the arrow
					GetArrows().RemoveAt(i);
				}
			}
		}

		//PRE: keyboard state
		//POST: 
		//DESC: move the player
		public void Move(KeyboardState kb)
		{
			//move right if d
			if (kb.IsKeyDown(Keys.D) && hitBox.Right < 800)
			{
				//update position right
				playerLoc.X += playerSpeed;
				hitBox.X = (int)playerLoc.X;
			}

			//move left if a
			if (kb.IsKeyDown(Keys.A) && hitBox.Left > 0)
			{
				//update position left
				playerLoc.X -= playerSpeed;
				hitBox.X = (int)playerLoc.X;
			}
		}

		//PRE: 
		//POST: 
		//DESC: fire an arrow
		public void ShootArrow()
		{
			//if there is no cooldown, shoot an arrow
			if (arrowCooldown < timer.GetTimePassed())
			{
				//add a new arrow to the list, reset the cooldown, indicate an arrow was shot
				arrows.Add(new Arrow(true, playerLoc.X + hitBox.Width / 2, playerLoc.Y));
				arrowCooldown = timer.GetTimePassed() + currentFirerate;
				arrowsShot++;
				totalShotsFired++;
			}
		}

		//PRE: 
		//POST: list of arrows
		//DESC: return the list of arrows
		public List<Arrow> GetArrows()
		{
			//return the list of arrows
			return arrows;
		}

		//PRE: 
		//POST: player hitbox
		//DESC: return the rectangle of the player
		public Rectangle GetRectangle()
		{
			//return the rectangle of the player
			return hitBox;
		}

		//PRE: the score change, is this the shop
		//POST: 
		//DESC: update the score of the player
		public void UpdateScore(int change, bool isShop)
		{
			//change the score
			score += change;

			//can only go negative if it is the shop, else score is bottommed at 0
			if (score < 0 && !isShop)
			{
				//score is 0
				score = 0;
			}
		}

		//PRE: level
		//POST: 
		//DESC: add the stats to the arrays
		public void UpdateLevelStats(int gameLevel)
		{
			//add the stats to the arrays
			levelScores[gameLevel] = score;
			levelKills[gameLevel] = kills;
			levelArrowsShot[gameLevel] = arrowsShot;
			levelArrowsHit[gameLevel] = arrowsHit;
		}

		//PRE: 
		//POST: 
		//DESC: update the culmative stats
		public void UpdateGameStats()
		{
			//1 more game played
			gamesPlayed++;

			//replace the top acc if possible
			if (levelArrowsHit.Sum() / (double)levelArrowsShot.Sum() * 100 > topAccuracy)
			{
				//make a new top acc
				topAccuracy = Math.Round(levelArrowsHit.Sum() / (double)levelArrowsShot.Sum() * 100, 2);
			}

			//replace the top score if possible
			if (GetScoresSum() > highScore)
			{
				//make a new top score
				highScore = GetScoresSum();
			}
		}

		//PRE: 
		//POST:  games played
		//DESC: return the games played
		public int GetGamesPlayed()
		{
			//return the games played
			return gamesPlayed;
		}

		//PRE: 
		//POST: the highest acc
		//DESC: return the highest acc
		public double GetTopAccuracy()
		{
			//return the highest acc
			return topAccuracy;
		}

		//PRE: 
		//POST: the avg acc
		//DESC: return the avg acc
		public double GetAverageAccuracy()
		{
			//return the avg acc
			return Math.Round(totalShotsHit * 100 / (double)totalShotsFired, 2);
		}

		//PRE: 
		//POST: the total shots fired
		//DESC: return the total shots fired
		public int GetTotalShotsFired()
		{
			//return the total shots fired
			return totalShotsFired;
		}

		//PRE: 
		//POST: the total shots hit
		//DESC: return the total shots hit
		public int GetTotalShotsHit()
		{
			//return the total shots hit
			return totalShotsHit;
		}

		//PRE: 
		//POST: the high score
		//DESC: return the high score
		public int GetHighScore()
		{
			//return the high score
			return highScore;
		}

		//PRE: 
		//POST: the sum of the scores
		//DESC: return the sum of the scores
		public int GetScoresSum()
		{
			//return the sum of the scores
			return levelScores.Sum();
		}

		//PRE: 
		//POST: current score
		//DESC: return the current score
		public int GetScore()
		{
			//return the current score
			return score;
		}

		//PRE: 
		//POST: each level's score
		//DESC: return each level's score
		public int[] GetAllScores()
		{
			//return each level's score
			return levelScores;
		}

		//PRE: 
		//POST: each level's kills
		//DESC: return each level's kills
		public int[] GetKills()
		{
			//return each level's kills
			return levelKills;
		}

		//PRE: 
		//POST: arrows shot each level
		//DESC: return arrows shot each level
		public int[] GetArrowsShot()
		{
			//return arrows shot each level
			return levelArrowsShot;
		}

		//PRE: 
		//POST: arrows hit each level
		//DESC: return arrows hit each level
		public int[] GetArrowsHit()
		{
			//return arrows hit each level
			return levelArrowsHit;
		}

		//PRE: 
		//POST: 
		//DESC: clear the stats of the level
		public void ClearLevelStats()
		{
			//clear the stats of the level
			score = 0;
			kills = 0;
			arrowsShot = 0;
			arrowsHit = 0;
		}

		//PRE: 
		//POST: 
		//DESC: indicate an arrow hit
		public void ArrowHit()
		{
			//increase the arrows hit
			arrowsHit++;
			totalShotsHit++;
		}

		//PRE: 
		//POST: 
		//DESC: increase kills
		public void KillUp()
		{
			//increase kills
			kills++;
		}

		//PRE: the mob killed
		//POST: 
		//DESC: indicate one of that mob was killed
		public void UpdateKills(int mob)
		{
			//indicate one of that mob was killed
			totalKills[mob]++;
		}

		//PRE: 
		//POST: the array holding all of each mob killed
		//DESC: return the total kills
		public int[] GetTotalKills()
		{
			//return the total kills
			return totalKills;
		}

		//PRE: 
		//POST: 
		//DESC: update the boosts
		public void UpdateBoosts(bool[] boosts)
		{
			//if boosted, use the boosted speed
			if (boosts[SPEED])
			{
				playerSpeed = BOOST_PLAYER_SPEED;
			}
			else
			{
				playerSpeed = NORMAL_PLAYER_SPEED;
			}

			//if boosted, use the boosted damage
			if (boosts[DAMAGE])
			{
				playerDamage = 3;
			}
			else
			{
				playerDamage = 1;
			}

			//if boosted, use the boosted firerate
			if (boosts[FIRE_RATE])
			{
				currentFirerate = BOOSTED_FIRERATE;
			}
			else
			{
				currentFirerate = DEFAULT_FIRERATE;
			}

			//if boosted, use the boosted multiplier
			if (boosts[POINTS])
			{
				scoreMultiplier = 2;
			}
			else
			{
				scoreMultiplier = 1;
			}
		}

		//PRE: 
		//POST: damage of the player
		//DESC: return the damage done by the player
		public int GetPlayerDamage()
		{
			//return the damage done by the player
			return playerDamage;
		}

		//PRE: 
		//POST: multiplier
		//DESC: return the score multiplier
		public int GetScoreMultiplier()
		{
			//return the score multiplier
			return scoreMultiplier;
		}

		public void ClearArrows()
		{
			//clear the list holding the arrows
			arrows.Clear();
		}

		//PRE: 
		//POST: 
		//DESC: stun the player
		public void Stun()
		{
			//signal that the player is stunned until .5 seconds later
			stunUntil = timer.GetTimePassed() + 500;
		}

		//PRE: 
		//POST: bool indicating stun
		//DESC: checks if the player is stunned
		public bool IsPlayerStunned()
		{
			//return true if the time has not reached the unstun point yet
			if (stunUntil > timer.GetTimePassed())
			{
				//return stunned
				return true;
			}

			//return not stunned
			return false;
		}

		//PRE: 
		//POST: 
		//DESC: saves the stats to a file
		public void SaveProgress()
		{
			//create/replace the stats file
			outFile = File.CreateText("PlayerStats.txt");

			//write played games and streak data
			outFile.WriteLine(gamesPlayed);
			outFile.WriteLine(topAccuracy);
			outFile.WriteLine(highScore);
			outFile.WriteLine(totalShotsFired);
			outFile.WriteLine(totalShotsHit);

			//write the win guess distribution
			foreach (int i in totalKills)
			{
				//write the wins on that guess
				outFile.WriteLine(i);
			}

			//close writing to the file
			outFile.Close();
		}

		//PRE: 
		//POST: 
		//DESC: loads the stats file
		private void LoadProgress()
		{
			//attempt to read saved data
			try
			{
				//open the file to read
				inFile = File.OpenText("PlayerStats.txt");

				//read the data from the file until there is none left
				while (!inFile.EndOfStream)
				{
					//read the lines in the file and convert to the required data types
					gamesPlayed = Convert.ToInt32(inFile.ReadLine());
					topAccuracy = Convert.ToDouble(inFile.ReadLine());
					highScore = Convert.ToInt32(inFile.ReadLine());
					totalShotsFired = Convert.ToInt32(inFile.ReadLine());
					totalShotsHit = Convert.ToInt32(inFile.ReadLine());

					for (int i = 0; i < totalKills.Length; i++)
					{
						totalKills[i] = Convert.ToInt32(inFile.ReadLine()); ;
					}
				}
			}
			catch (FileNotFoundException fnf)
			{
				//file not found error feedback
				Console.WriteLine("ERROR: file not found." + fnf);
			}
			catch (EndOfStreamException eos)
			{
				//stream ended error feedback
				Console.WriteLine("ERROR: read past file." + eos);
			}
			catch (FormatException fe)
			{
				//bad format error feedback
				Console.WriteLine("ERROR: bad format." + fe);
			}
			catch (Exception e)
			{
				//general error feedback
				Console.WriteLine("ERROR: error? what???" + e);
			}
			finally
			{
				//if the file was being read, close reading
				if (inFile != null)
				{
					//close reading the file
					inFile.Close();
				}
			}
		}
	}
}
