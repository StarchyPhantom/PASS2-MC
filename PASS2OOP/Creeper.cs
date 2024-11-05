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
	class Creeper : Mob
	{
		//random number generator
		private Random rng = new Random();

		//get the player so the information can be accessed
		private Player player;

		//creeper explosion radius
		private const int EXPLOSION_RADIUS = 150;

		/// <summary>
		/*Creeper, aw man
		So we back in the mine, got our pickaxe swingin' from side to side, side, side to side
		This task a grueling one, hope to find some diamonds tonight, night, night, diamonds tonight
		Heads up
		You hear a sound, turn around and look up
		Total shock fills your body
		Oh no it's you again, I can never forget those eyes, eyes, eyes, eyes, eyes, eyes...*/
		///ok, im done, so basically, the creeper tries to follow the player and explode if they are at the bottom of the screen or are killed
		/// </summary>
		/// <param name="mobTexture"></param>
		/// <param name="player"></param>
		public Creeper(Texture2D mobTexture, Player player, int screenWidth, int screenHeight) : base(mobTexture, screenWidth, screenHeight)
		{
			//the required mob data: location, score, health. take in the player
			this.player = player;
			mobHealth = 3;
			mobScore = 40;
			mobID = 1;
			mobLoc.Y = -screenHeight / 10;
			mobLoc.X = screenWidth / 10 * (float)Math.Floor((double)rng.Next(0, 72) / 8);
			hitBox.X = (int)mobLoc.X;
			hitBox.Y = (int)mobLoc.Y;
		}

		//PRE: 
		//POST: 
		//DESC: this version of move will follow the player
		public override void Move()
		{
			//update y pos
			mobLoc.Y += (float)mobSpeed;
			hitBox.Y = (int)mobLoc.Y;

			//changes direction based on player location
			if (mobLoc.X - 20 > player.GetRectangle().X)
			{
				//move the mob left
				mobLoc.X -= (float)mobSpeed;
			}
			else if (mobLoc.X + 20 < player.GetRectangle().X)
			{
				//move the mob right
				mobLoc.X += (float)mobSpeed;
			}

			//update x pos
			hitBox.X = (int)mobLoc.X;
		}

		//PRE: 
		//POST: 
		//DESC: perform a check around the creeper and deduct points if the player is nearby
		public void Explode()
		{
			//is the mob is in the explosion radius, reduce the player's points
			if (Math.Sqrt(Math.Pow(player.GetRectangle().Center.X - hitBox.Center.X, 2) + Math.Pow(player.GetRectangle().Center.Y - hitBox.Center.Y, 2)) < EXPLOSION_RADIUS)
			{
				//reduce the player's points
				player.UpdateScore(-mobScore, false);
			}
		}

		//PRE: 
		//POST: return if it is dead or not
		//DESC: return if it is out of bounds or killed
		public override bool Die()
		{
			//get rid of the mod and do the explosion check if it is dead or offscreen
			if (mobHealth <= 0 || hitBox.Bottom > screenHeight)
			{
				//perform explosion check, signal deletion
				Explode();
				return true;
			}

			//signal to keep the mob
			return false;
		}
	}
}