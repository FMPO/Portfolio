using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Fisticuffs
{
    abstract class Projectile
    {
        //-------- Fields --------
        protected Rectangle position;
        protected Texture2D spriteSheet;
        protected Player owner;
        protected Player opponent;
        protected int vspd;
        protected int hspd;
        protected int spawnX;
        protected int spawnY;
        protected int width;
        protected int height;


        //-------- Constructor --------
        public Projectile(Player owner, Player opponent, Texture2D spriteSheet, int spawnX, int spawnY, int width, int height)
        {
            this.owner = owner;
            this.opponent = opponent;
            this.spriteSheet = spriteSheet;
            this.spawnX = spawnX;
            this.spawnY = spawnY;
            this.width = width;
            this.height = height;


            position = new Rectangle(spawnX, spawnY, width, height);
        }
        //-------- Properties --------

        public Rectangle Position
        {
            get { return position; }
        }
        public Player Owner
        {
            get { return owner; }
            //no set
        }

        //-------- Methods --------

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch sb);


    }
}
