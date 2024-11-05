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
	class Enderman : Mob
	{
		//time between teleports
		private const int TELEPORT_TIME = 3000;

		//the player
		private Player player;

		//random number generator
		private Random rng = new Random();

		//times teleported, next teleport time, locations to teleport
		private byte teleportCount = 0;
		private double teleportAt = 0;
		private List<Vector2> teleLocs;

		/// <summary>
		/// endermen will teleport every 3 seconds and stun the player for .5 seconds
		/// </summary>
		/// <param name="mobTexture"></param>
		/// <param name="player"></param>
		/// <param name="screenWidth"></param>
		/// <param name="screenHeight"></param>
		public Enderman(Texture2D mobTexture, Player player, int screenWidth, int screenHeight) : base(mobTexture, screenWidth, screenHeight)
		{
			//set up the following for the enderman: health, kill score, teleport locations, spawn position
			this.player = player;
			mobID = 4;
			mobHealth = 5;
			mobScore = 100;
			mobLoc.X = screenWidth / 10 * 8;
			mobLoc.Y = screenHeight / 10;
			hitBox.X = (int)mobLoc.X;
			hitBox.Y = (int)mobLoc.Y;
			teleLocs = new List<Vector2>{ new Vector2(screenWidth / 10 * 6, screenHeight / 2),
										  new Vector2(screenWidth / 10, screenHeight / 10 * 4), 
										  new Vector2(screenWidth / 10 * 3, screenHeight / 10 * 7) };
		}

		//PRE: 
		//POST: 
		//DESC: do not move the enderman
		public override void Move()
		{
			//override to not move besides teleporting
		}

		//PRE: global timer
		//POST: 
		//DESC: updates the mob, here, it teleports the enderman and stuns the player
		public override void UpdateMob(Timer timer)
		{
			//teleport after the timer is over
			if (timer.GetTimePassed() > teleportAt)
			{
				//stun the player due to fear
				player.Stun();

				//teleport based on the teleports occured
				if (teleportCount < 4 && teleportCount != 0)
				{
					//random index of the teleport locations
					int number = rng.Next(1, 100) % teleLocs.Count;

					//teleport the enderman to the index loc and remove that loc from the list
					mobLoc = teleLocs[number];
					teleLocs.RemoveAt(number);
				}
				else if (teleportCount == 4)
				{
					//teleport in front of the player
					mobLoc = new Vector2(player.GetRectangle().X, player.GetRectangle().Y - 80);
				}
				else if (teleportCount >= 5)
				{
					//teleport off the screen to signal deletion
					mobLoc = new Vector2(2000, 2000);
				}

				//update the hitbox position
				hitBox.X = (int)mobLoc.X;
				hitBox.Y = (int)mobLoc.Y;

				//increase the amount of teleports occured and the timer
				teleportCount++;
				teleportAt = timer.GetTimePassed() + TELEPORT_TIME;
			}
		}
	}
}
