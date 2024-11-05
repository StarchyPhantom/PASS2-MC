using Animation2D;
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
	class Arrow
	{
		//dimensions of the arrow
		private const int ARROW_WIDTH = 20;
		private const int ARROW_HEIGHT = 40;

		//speed of the arrow
		private const int ARROW_SPEED = 6;
		
		//arrow pos/rec/speed
		private Vector2 arrowLoc;
		private Rectangle hitBox;
		private float arrowSpeed;

		/// <summary>
		/// arrow that can be owned by the player or a skeleton, moves and collides with things
		/// </summary>
		/// <param name="isPlayer"></param>
		/// <param name="posX"></param>
		/// <param name="posY"></param>
		public Arrow(bool isPlayer, float posX, float posY)
		{
			//arrow location and size
			arrowLoc = new Vector2(posX - ARROW_WIDTH / 2, posY - ARROW_HEIGHT / 2);
			hitBox = new Rectangle((int)arrowLoc.X, (int)arrowLoc.Y, ARROW_WIDTH, ARROW_HEIGHT);

			//choose direction based on who shot it
			if (isPlayer)
			{
				//arrow goes up
				arrowSpeed = -ARROW_SPEED;
			}
			else
			{
				//arrows go down
				arrowSpeed = ARROW_SPEED;
			}
		}

		//PRE: 
		//POST: 
		//DESC: move the arrow
		public void Move()
		{
			//update the arrow's position
			arrowLoc.Y += arrowSpeed;
			hitBox.Y = (int)arrowLoc.Y;
		}

		//PRE: 
		//POST: the arrow's rectangle
		//DESC: returns the arrow's rectangle
		public Rectangle GetRectangle()
		{
			//return the rectangle of the arrow
			return hitBox;
		}
	}
}
