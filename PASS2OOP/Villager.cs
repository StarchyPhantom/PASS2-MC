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
	class Villager : Mob
	{
		//random number generator
		private Random rng = new Random();

		/// <summary>
		/// villager mob, basically the same as the default mob
		/// </summary>
		/// <param name="mobTexture"></param>
		public Villager(Texture2D mobTexture, int screenWidth, int screenHeight) : base(mobTexture, screenWidth, screenHeight)
		{
			//spawn the mob offscreen with ID 0
			mobID = 0;
			mobLoc.X = -screenWidth / 10;
			mobLoc.Y = screenHeight / 10 * (float)Math.Floor((double)rng.Next(16, 64) / 8);
			hitBox.X = (int)mobLoc.X;
			hitBox.Y = (int)mobLoc.Y;
		}
	}
}
