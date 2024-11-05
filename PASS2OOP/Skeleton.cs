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
	class Skeleton : Mob
	{
		//speeds in regards to spinning
		private const int INITIAL_SPIN_SPEED = 6;
		private const double SPIN_DECREASE_SPEED = 0.0085;

		//defining velocities, angles, "timers", to spiral or not to spiral
		private double velocityX = 0;
		private double velocityY = 0;
		private double spiralRate = 0;
		private double shootNextAt = 0;
		private bool startSpiral = false;

		//the level's arrows
		private List<Arrow> arrows;

		//raondom number generator
		private Random rng = new Random();

		/// <summary>
		/// skeleton, it shoots arrows which remove the player's buffs and take points away, but also spins in a fun spiral
		/// </summary>
		/// <param name="mobTexture"></param>
		/// <param name="arrows"></param>
		/// <param name="screenWidth"></param>
		/// <param name="screenHeight"></param>
		public Skeleton(Texture2D mobTexture, List<Arrow> arrows, int screenWidth, int screenHeight) : base (mobTexture, screenWidth, screenHeight)
		{
			//take in and share(?) the level arrows, and take in the screen dimensions
			this.arrows = arrows;
			this.screenWidth = screenWidth;
			this.screenHeight = screenHeight;

			//give the mob the needed data
			mobID = 2;
			mobHealth = 3;
			mobLoc = new Vector2(screenWidth / 10 * 7, 0);
			hitBox.X = (int)mobLoc.X;
			hitBox.Y = (int)mobLoc.Y;
			mobSpeed = 4;
			velocityY = mobSpeed;
			mobScore = 25;
		}

		//PRE: global timer
		//POST: 
		//DESC: base update mob, but with arrow shooting
		public override void UpdateMob(Timer timer)
		{
			//do base updates and shoot arrows
			base.UpdateMob(timer);
			ShootArrow(timer);
		}

		//PRE: 
		//POST: 
		//DESC: make the mob go down, then spiral, then exit right
		public override void Move()
		{
			//start spinning near half the screen
			if (!startSpiral && mobLoc.Y + hitBox.Height > screenHeight / 2)
			{
				//indicate to start spinning and change the speed
				startSpiral = true;
				mobSpeed = INITIAL_SPIN_SPEED;
			}

			//spin until speed has reached an exit threshold, where it will exit right
			if (startSpiral && mobSpeed > 3)
			{
				//make a funky angle, decrease the speed, update the velocity
				spiralRate += (Math.PI + spiralRate) / 120;
				mobSpeed -= SPIN_DECREASE_SPEED;
				velocityX = -mobSpeed * Math.Sin(spiralRate);
				velocityY = mobSpeed * Math.Cos(spiralRate);
			}
			else if (mobSpeed <= 3)
			{
				//make the mob exit right
				velocityX = 2.5;
				velocityY = 0;
			}

			//update the position of the mob
			mobLoc.X += (float)velocityX;
			mobLoc.Y += (float)velocityY;
			hitBox.X = (int)mobLoc.X;
			hitBox.Y = (int)mobLoc.Y;
		}

		//PRE: global timer
		//POST: 
		//DESC: shoots an arrow from the skeleton
		private void ShootArrow(Timer timer)
		{
			//if the "timer" is up, and the skeleton is onscreen, fire an arrow
			if (timer.GetTimePassed() > shootNextAt && mobLoc.Y > 0 && mobLoc.X < screenWidth)
			{
				//add a new arrow to the level arrow list, reset the "timer"
				arrows.Add(new Arrow(false, mobLoc.X + hitBox.Width / 2, mobLoc.Y));
				shootNextAt = timer.GetTimePassed() + rng.Next(1250, 2000);
			}
		}
	}
}
