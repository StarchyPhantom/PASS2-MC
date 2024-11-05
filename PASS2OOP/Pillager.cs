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
	class Pillager : Mob
	{
		//mob health values
		private const int SHIELDED_HEALTH = 99;
		private const int ACTUAL_HEALTH = 2;

		//random number generator
		private Random rng = new Random();

		//the angle for the sine wave
		private double sinAngle = 0;

		//is the mob shielded or not
		private bool isShielded = true;

		/// <summary>
		/// the pillager moves in a sine wave and has a shield that can block one instance of damage regardless of magnitude
		/// </summary>
		/// <param name="mobTexture"></param>
		/// <param name="screenWidth"></param>
		/// <param name="screenHeight"></param>
		public Pillager(Texture2D mobTexture, int screenWidth, int screenHeight) : base(mobTexture, screenWidth, screenHeight)
		{
			//give the mob the required data to fucntion
			mobID = 3;
			mobHealth = SHIELDED_HEALTH;
			mobLoc.X = -screenWidth / 10;
			mobLoc.Y = screenHeight / 10 * (float)Math.Floor((double)rng.Next(16, 40) / 8);
			hitBox.X = (int)mobLoc.X;
			hitBox.Y = (int)mobLoc.Y;
			mobSpeed /= 2;
			mobScore = 25;
		}

		//PRE: global timer
		//POST: 
		//DESC: update the mob, but account for the shield
		public override void UpdateMob(Timer timer)
		{
			//do base updates
			base.UpdateMob(timer);

			//removes the shield if hit
			if (mobHealth != SHIELDED_HEALTH && isShielded)
			{
				//health is vulnerable, no longer shielded
				mobHealth = ACTUAL_HEALTH;
				isShielded = false;
			}
		}

		//PRE: 
		//POST: 
		//DESC: move in a sine wave
		public override void Move()
		{
			//move up and down smoothly
			sinAngle += Math.PI / 150;
			mobLoc.Y += (int)(4 * Math.Sin(sinAngle));
			hitBox.Y = (int)mobLoc.Y;

			//move to the right
			base.Move();
		}
	}
}
