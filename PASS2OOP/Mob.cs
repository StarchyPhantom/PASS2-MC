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
	class Mob
	{
		//mob location, hitbox, and texture
		protected Vector2 mobLoc;
		protected Rectangle hitBox;
		protected Texture2D mobTexture;

		//default ID, health, speed, and score values
		protected int mobID = -1;
		protected int mobHealth = 1;
		protected double mobSpeed = 4;
		protected int mobScore = 10;

		protected int screenWidth;
		protected int screenHeight;

		/// <summary>
		/// default mob, parent and building blocks of all mobs
		/// </summary>
		/// <param name="mobTexture"></param>
		public Mob(Texture2D mobTexture, int screenWidth, int screenHeight)
		{
			//take in the texture and dimensions
			this.mobTexture = mobTexture;
			this.screenWidth = screenWidth;
			this.screenHeight = screenHeight;

			//default positions
			mobLoc = new Vector2(-400, -400);
			hitBox = new Rectangle((int)mobLoc.X, (int)mobLoc.Y, 80, 80);
		}

		//PRE: global timer
		//POST:
		//DESC: updates the mob
		public virtual void UpdateMob(Timer timer)
		{
			//move the mob
			Move();
		}

		//PRE: spritebatch to draw
		//POST:
		//DESC: draws the mob
		public void DrawMob(SpriteBatch spriteBatch)
		{
			//draw the mob
			spriteBatch.Draw(mobTexture, hitBox, Color.White);
		}

		//PRE: 
		//POST:
		//DESC: moves the mob
		public virtual void Move()
		{
			//updates the vector and the rectangle
			mobLoc.X += (float)mobSpeed;
			hitBox.X = (int)mobLoc.X;
		}

		//PRE: 
		//POST: health of the mob
		//DESC: returns the health of the mob
		public int GetHealth()
		{
			//return the mob's health
			return mobHealth;
		}

		//PRE: damage dealt by the player
		//POST:
		//DESC: hurts the mob
		public void BeHurt(int playerDamage)
		{
			//mob is hurt by the damage the player does
			mobHealth -= playerDamage;
		}

		//PRE: 
		//POST: boolean indicating if it is to be deleted or not
		//DESC: returns if the mob is to be deleted or not
		public virtual bool Die()
		{
			//return true if the mob has no more health, or it is offscreen
			if (mobHealth <= 0 || mobLoc.X > 800)
			{
				//return dead
				return true;
			}

			//return not dead
			return false;
		}

		//PRE: 
		//POST: the mob's rectangle
		//DESC: return the mob's rectangle
		public Rectangle GetRectangle()
		{
			//return the mob's hitbox
			return hitBox;
		}

		//PRE: 
		//POST: points gained from killing the mob
		//DESC: returns the points gained from killing the mob
		public int GetKillPoints()
		{
			//return the score gained from kill
			return mobScore;
		}

		//PRE: 
		//POST: the mob's ID
		//DESC: returns the mob's ID
		public int GetMobID()
		{
			//return the mob's unique ID number
			return mobID;
		}
	}
}
