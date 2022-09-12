using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Fisticuffs
{
    class Hurtbox
    {
        //-------- Fields --------
        private Texture2D texture;
        private int width;
        private int height;
        private Player owner;
        private int xOffset;
        private int yOffset;
        private Rectangle position;



        //-------- Constructor --------
        public Hurtbox(Player owner, Texture2D texture, int width,int height, int xOffset, int yOffset)
        {
            this.owner = owner;
            this.texture = texture;
            this.width = width;
            this.height = height;
            this.xOffset = xOffset;
            this.yOffset = yOffset;
            position = new Rectangle(owner.Position.X + xOffset, owner.Position.Y + yOffset, width, height);
        }


        //-------- Properties --------
        public Rectangle Position
        {
            get { return position; }
            //no set
        }

        //-------- Methods --------

        /// <summary>
        /// update the state of the hurtbox
        /// </summary>
        public void Update(GameTime gameTime)
        {
            position.X = owner.Position.X + xOffset;
            position.Y = owner.Position.Y + yOffset;
            position.Width = width;
            position.Height = height;

        }

        /// <summary>
        /// draw the hurtbox
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, Color.White);
        }

        /// <summary>
        /// set the position Rectangle of the hurtbox
        /// </summary>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetHurtboxPos(int xOffset, int yOffset, int width, int height)
        {
            this.xOffset = xOffset;
            this.yOffset = yOffset;
            this.width = width;
            this.height = height;
        }

    }
}
