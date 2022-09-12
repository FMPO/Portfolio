using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Fisticuffs
{
    class Solid
    {

        //-------- Fields --------
        private Rectangle position;
        private Texture2D texture;



        //-------- Constructor --------
        public Solid(Rectangle position, Texture2D texture)
        {
            this.position = position;
            this.texture = texture;
        }


        //-------- Properties --------
        public Rectangle Position
        {
            get { return position; }
            //no set statement
        }

        //-------- Methods --------

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, Color.White);
        }

    }
}
