using Animation2D;
using Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS2OOP
{
	class Level
	{
		//mob amounts
		private const byte INITIAL_MOBS = 10;
		private const byte MOB_INCREASE = 5;

		//mob IDs
		private const byte VILLAGER = 0;
		private const byte CREEPER = 1;
		private const byte SKELETON = 2;
		private const byte PILLAGER = 3;
		private const byte ENDERMAN = 4;

		//false pillager health when shielded
		private const int SHIELDED_HEALTH = 99;

		//creeper explosion radius
		private const int EXPLOSION_RADIUS = 150;

		//random number generator
		private Random rng = new Random();

		//textures for drawing, the player for interactions, screen dimensions for calculations
		private Texture2D[] bgTextures = new Texture2D[4];
		private Texture2D[] mobTextures = new Texture2D[5];
		private Texture2D[] boostIcons = new Texture2D[4];
		private Texture2D arrowImg;
		private Texture2D shieldImg;
		private Texture2D explosionImg;
		private Player player;
		private int screenWidth;
		private int screenHeight;

		//rectangles of: the background, boost icons, enderman scare
		private Rectangle[,] bgSquares = new Rectangle[10, 10];
		private Rectangle[] boostIconRecs = new Rectangle[4];
		private Rectangle jumpScareRec;

		//lists regarding mobs, arrows, and explosion data
		private List<Mob> mobs = new List<Mob>(10);
		private List<Arrow> skeletonArrows = new List<Arrow>(32);
		private List<Rectangle> explosionRecs = new List<Rectangle>(2);
		private List<double> deleteExplosionsAt = new List<double>(2);
		private List<bool> areExplosionsVisible = new List<bool>(2);

		//data in relation to mob spawning
		private int gameLevel = 0;
		private double spawnNextAt = 0;
		private int mobsToSpawn = 10;
		private int[,] mobProbs = new int[,]{ { 70, 50, 40, 50, 10 } , //vil
									  { 20, 30, 20, 15, 20 } , //cre
									  { 10, 20, 20, 15, 35 } , //ske
									  { 00, 00, 20, 15, 25 } , //pil
									  { 00, 00, 00, 05, 20 }}; //end
		private int[] mobsAllowed = new int[]{ 2, 3, 3, 5, 3 };
		private int[] spawnTimes = new int[] { 2000, 1700, 1300, 1200, 1000 };

		//data for player boosts
		private bool[] boosts = new bool[] { false, false, false, false, };
		private bool disableBoost = false;

		/// <summary>
		/// handles most of the gameplay logic, ie: managing mob/player interactions
		/// </summary>
		/// <param name="mobTextures"></param>
		/// <param name="bgTextures"></param>
		/// <param name="boostIcons"></param>
		/// <param name="arrowImg"></param>
		/// <param name="shieldImg"></param>
		/// <param name="explosionImg"></param>
		/// <param name="player"></param>
		/// <param name="screenWidth"></param>
		/// <param name="screenHeight"></param>
		public Level(Texture2D[] mobTextures, Texture2D[] bgTextures, Texture2D[] boostIcons, Texture2D arrowImg, Texture2D shieldImg, Texture2D explosionImg, 
					 Player player, int screenWidth, int screenHeight)
		{
			//take in images for drawing, player for logic, screen width/height for calculations
			this.mobTextures = mobTextures;
			this.bgTextures = bgTextures;
			this.boostIcons = boostIcons;
			this.arrowImg = arrowImg;
			this.shieldImg = shieldImg;
			this.explosionImg = explosionImg;
			this.player = player;
			this.screenWidth = screenWidth;
			this.screenHeight = screenHeight;

			//create the vector for the enderman scare
			jumpScareRec = new Rectangle(0, 0, screenWidth, screenHeight);

			//make the rectangles for the boost icons
			for (int i = 0; i < 4; i++)
			{
				//make the rectangle for the boost icon
				boostIconRecs[i] = new Rectangle(screenWidth - boostIcons[i].Width, screenHeight / 10 * 9 - (i + 1) * boostIcons[i].Height, boostIcons[i].Width, boostIcons[i].Height);
			}

			//define the background squares through loops
			for (int i = 0; i * screenHeight / 10 < screenHeight; i++)
			{
				for (int j = 0; j * screenWidth / 10 < screenWidth; j++)
				{
					//make the background square
					bgSquares[i, j] = new Rectangle(i * screenWidth / 10, j * screenHeight / 10, screenWidth / 10, screenHeight / 10);
				}
			}
		}

		//PRE: global timer
		//POST:
		//DESC: updates the level, handles collision, movement, etc.
		public void UpdateLevel(Timer timer)
		{
			//update each mob
			foreach (Mob i in mobs)
			{
				//update the mob
				i.UpdateMob(timer);
			}

			//spawn the next mob when the time has passed and spawning a mob wouldn't exceed the limits
			if (spawnNextAt < timer.GetTimePassed() && mobs.Count < mobsAllowed[gameLevel] && mobsToSpawn > 0)
			{
				//spawn the mob, reset the "timer", indicate one less mob to spawn
				SpawnMob();
				spawnNextAt = timer.GetTimePassed() + spawnTimes[gameLevel];
				mobsToSpawn--;
			}

			//update the skeleton arrows
			for (int i = 0; i < skeletonArrows.Count; i++)
			{
				//move the skeleton arrow
				skeletonArrows[i].Move();

				//delete the skeleton arrow if it is out of bounds or hit the player
				if (skeletonArrows[i].GetRectangle().Top > screenHeight)
				{
					//remove the arrow
					skeletonArrows.RemoveAt(i);
				}
				else if (skeletonArrows[i].GetRectangle().Intersects(player.GetRectangle()))
				{
					//remove the arrow, reduce player score, disbale the player's boosts
					skeletonArrows.RemoveAt(i);
					player.UpdateScore(-20, false);
					disableBoost = true;
					boosts = new bool[]{ false, false, false, false };
					player.UpdateBoosts(boosts);
				}
			}

			//make the explosions invisible if their time is up
			for (int i = 0; i < explosionRecs.Count; i++)
			{
				//make the explosion invisible if it's time is up
				if (timer.GetTimePassed() > deleteExplosionsAt[i])
				{
					//make the explosion invisible
					areExplosionsVisible[i] = false;
				}
			}

			//handle the collision between the player arrows and the mobs
			for (int i = 0; i < mobs.Count(); i++)
			{
				for (int j = 0; j < player.GetArrows().Count(); j++)
				{
					//hurt the mob if the arrow collided
					if (player.GetArrows()[j].GetRectangle().Intersects(mobs[i].GetRectangle()))
					{
						//hurt the mob and delete the arrow
						mobs[i].BeHurt(player.GetPlayerDamage());
						player.GetArrows().RemoveAt(j);
						player.ArrowHit();
					}
				}

				//handle the mob's death
				if (mobs[i].Die())
				{
					//give points if the mob's health is 0 or less
					if (mobs[i].GetHealth() <= 0)
					{
						//give points based on killed mob and add a kill
						player.UpdateScore(mobs[i].GetKillPoints() * player.GetScoreMultiplier(), false);
						player.KillUp();
						player.UpdateKills(mobs[i].GetMobID());
					}

					//if the mob dead is a creeper, make an explosion
					if (mobs[i] is Creeper)
					{
						//make an explosion image and indicate when it should be deleted
						explosionRecs.Add(new Rectangle(mobs[i].GetRectangle().X - (explosionImg.Width - mobs[i].GetRectangle().Width) / 2,
														mobs[i].GetRectangle().Y - (explosionImg.Height - mobs[i].GetRectangle().Height) / 2, 
														explosionImg.Width, explosionImg.Height));
						deleteExplosionsAt.Add(timer.GetTimePassed() + 1000);
						areExplosionsVisible.Add(true);
					}

					//remove the mob from the list
					mobs.RemoveAt(i);
					break;
				}
			}
		}

		//PRE: spritebatch for drawing
		//POST:
		//DESC: draw everything about the level: the background, mobs, arrows
		public void DrawLevel(SpriteBatch spriteBatch)
		{
			//Draws the background
			for (int i = 0; i * 80 < 800; i++)
			{
				for (int j = 0; j * 80 < 800; j++)
				{
					//subjective, draw stone accross the middle, else is grass
					if (i == 4 || i == 5 || j == 4 || j == 5)
					{
						//draw the background tile
						spriteBatch.Draw(bgTextures[0], bgSquares[i, j], Color.White);
					}
					else
					{
						//draw the background tile
						spriteBatch.Draw(bgTextures[2], bgSquares[i, j], Color.White);
					}

					//draws dirt if there as an explosion nearby
					for (int k = 0; k < explosionRecs.Count; k++)
					{
						//calculate a circle around the explosion and damage the dirt
						if (Math.Sqrt(Math.Pow(explosionRecs[k].Center.X - bgSquares[i, j].Center.X, 2) + Math.Pow(explosionRecs[k].Center.Y - bgSquares[i, j].Center.Y, 2)) < EXPLOSION_RADIUS)
						{
							//draw the backgorund tile
							spriteBatch.Draw(bgTextures[1], bgSquares[i, j], Color.White);
						}
					}
				}
			}

			//draw the explosions
			for (int i = 0; i < explosionRecs.Count; i++)
			{
				//if the explosions are visible, draw them
				if (areExplosionsVisible[i])
				{
					//draw the explosion
					spriteBatch.Draw(explosionImg, explosionRecs[i], Color.White);
				}
			}

			//draw each mob
			foreach (Mob i in mobs)
			{
				//draw the mob
				i.DrawMob(spriteBatch);

				//draw a shield if the mob is a pillager and they have a shield
				if (i is Pillager && i.GetHealth() == SHIELDED_HEALTH)
				{
					//draw the shield
					spriteBatch.Draw(shieldImg, i.GetRectangle().Center.ToVector2(), Color.White);
				}
			}

			//draw the skeleton arrows
			for (int i = 0; i < skeletonArrows.Count; i++)
			{
				//draw the skeleton arrow
				spriteBatch.Draw(arrowImg, skeletonArrows[i].GetRectangle(), Color.OrangeRed);
			}

			//draw the boost icons
			for (int i = 0; i < boosts.Length; i++)
			{
				//draws the boost icons depending if they are active or not
				if (boosts[i])
				{
					//draw an active boost icon
					spriteBatch.Draw(boostIcons[i], boostIconRecs[i], Color.White);
				}
				else
				{
					//draw an inactive boost icon
					spriteBatch.Draw(boostIcons[i], boostIconRecs[i], Color.White * 0.35f);
				}
			}

			//jumpscare the player if they are stunned
			if (player.IsPlayerStunned())
			{
				//draw a giant enderman to scare the player
				spriteBatch.Draw(mobTextures[4], jumpScareRec, Color.White * 0.5f);
			}
		}

		//PRE:
		//POST: a boolean indicating level completion
		//DESC: checks if the level is done or not
		public bool IsLevelDone()
		{
			//says the level is done if there are no mobs left onscreen or to spawn
			if (mobsToSpawn <= 0 && mobs.Count <= 0)
			{
				//done
				return true;
			}

			//donen't
			return false;
		}

		//PRE:
		//POST:
		//DESC: Prepares the variables for the next level
		public void NextLevel()
		{
			//increase the level, add more mobs, reset the boost disabler, clear the arrows
			gameLevel++;
			mobsToSpawn = INITIAL_MOBS + gameLevel * MOB_INCREASE;
			disableBoost = false;
			skeletonArrows.Clear();
			player.ClearArrows();
			deleteExplosionsAt.Clear();
			explosionRecs.Clear();
			areExplosionsVisible.Clear();
		}

		//PRE:
		//POST:
		//DESC: Adds a mob to the level based on spawn chance
		private void SpawnMob()
		{
			//define temporary variables to randomly pick a mob and hold the mob
			int spawnNum = rng.Next(1, 100);
			Mob mob;

			//add a new mob based on the chances
			if (spawnNum <= mobProbs[VILLAGER, gameLevel])
			{
				//the mob selected is a villager
				mob = new Villager(mobTextures[VILLAGER], screenWidth, screenHeight);
			}
			else if (spawnNum <= mobProbs[CREEPER, gameLevel] + mobProbs[VILLAGER, gameLevel])
			{
				//the mob selected is a creeper
				mob = new Creeper(mobTextures[CREEPER], player, screenWidth, screenHeight);
			}
			else if (spawnNum <= mobProbs[SKELETON, gameLevel] + mobProbs[CREEPER, gameLevel] + mobProbs[VILLAGER, gameLevel])
			{
				//the mob selected is a skeleton
				mob = new Skeleton(mobTextures[SKELETON], skeletonArrows, screenWidth, screenHeight);
			}
			else if (spawnNum <= mobProbs[PILLAGER, gameLevel] + mobProbs[SKELETON, gameLevel] + mobProbs[CREEPER, gameLevel] + mobProbs[VILLAGER, gameLevel])
			{
				//the mob selected is a pillager
				mob = new Pillager(mobTextures[PILLAGER], screenWidth, screenHeight);
			}
			else
			{
				//the mob selected is a enderman
				mob = new Enderman(mobTextures[ENDERMAN], player, screenWidth, screenHeight);
			}

			//add the mob to the list of mobs
			mobs.Add(mob);
		}

		//PRE:
		//POST: the current level of the game
		//DESC: returns the game level
		public int GetLevel()
		{
			//return the value
			return gameLevel;
		}

		public void UpdateBoosts(bool[] boosts)
		{
			this.boosts = boosts;
		}

		//PRE:
		//POST: boolean indicating if the boosts should be disabled or not
		//DESC: returns the boolean
		public bool BoostsDisabled()
		{
			//return the value
			return disableBoost;
		}
	}
}
