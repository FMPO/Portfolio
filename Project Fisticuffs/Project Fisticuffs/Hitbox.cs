using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Fisticuffs
{
    enum BlockType
    {
        Low,
        Mid,
        High,
        Unblockable
    }
    class Hitbox
    {
        //-------- Fields --------
        private Texture2D texture;
        private int width;
        private int height;
        private Player owner;
        private bool isProjectile;
        private Projectile ownerProjectile;
        private int xOffset;
        private int yOffset;
        private Rectangle position;

        //effect on hit fields
        private int hitstun;
        private bool launcher;
        private int xKnockback;
        private int yKnockback;
        private int damage;
        private BlockType blockType;
        private int blockStun;
        private int blockXPushback;



        //-------- Constructor --------
        public Hitbox(
            Player owner,
            Texture2D texture,
            int xOffset,
            int yOffset,
            int width,
            int height,
            int hitstun,
            bool launcher,
            int xKnockback,
            int yKnockback,
            int damage,
            BlockType blockType,
            int blockStun,
            int blockXPushback)
        {
            isProjectile = false;
            this.owner = owner;
            this.texture = texture;
            this.width = width;
            this.height = height;
            this.xOffset = xOffset;
            this.yOffset = yOffset;

            //hitbox effect on hit fields
            this.hitstun = hitstun;
            this.launcher = launcher;
            this.xKnockback = xKnockback;
            this.yKnockback = yKnockback;
            this.damage = damage;
            this.blockType = blockType;
            this.blockStun = blockStun;
            this.blockXPushback = blockXPushback;

            position = new Rectangle(owner.Position.X + xOffset, owner.Position.Y + yOffset, width, height);
        }
        public Hitbox(
            Projectile ownerProjectile,
            Texture2D texture,
            int xOffset,
            int yOffset,
            int width,
            int height,
            int hitstun,
            bool launcher,
            int xKnockback,
            int yKnockback,
            int damage,
            BlockType blockType,
            int blockStun,
            int blockXPushback)
        {
            isProjectile = true;
            this.ownerProjectile = ownerProjectile;
            owner = ownerProjectile.Owner;
            this.texture = texture;
            this.width = width;
            this.height = height;
            this.xOffset = xOffset;
            this.yOffset = yOffset;

            //hitbox effect on hit fields
            this.hitstun = hitstun;
            this.launcher = launcher;
            this.xKnockback = xKnockback;
            this.yKnockback = yKnockback;
            this.damage = damage;
            this.blockType = blockType;
            this.blockStun = blockStun;
            this.blockXPushback = blockXPushback;

            position = new Rectangle(ownerProjectile.Position.X + xOffset, owner.Position.Y + yOffset, width, height);
        }


        //-------- Properties --------
        public Rectangle Position
        {
            get { return position; }
            set { position = value; }
        }
        public Player Owner
        {
            get { return owner; }
            //no set
        }

        public Projectile OwnerProjectile
        {
            get { return ownerProjectile; }
            //no set
        }
        public bool IsProjectile
        {
            get { return isProjectile; }
            //no set
        }


        //effect on hit properties
        public int Hitstun
        {
            get { return hitstun; }
            //no set
        }
        public bool Launcher
        {
            get { return launcher; }
        }
        public int XKnockback
        {
            get { return xKnockback; }
        }
        public int Yknockback
        {
            get { return yKnockback; }
        }
        public int Damage
        {
            get { return damage; }
        }
        public BlockType BlockType
        {
            get { return blockType; }
            //no set
        }
        public int BlockStun
        {
            get { return blockStun; }
        }
        public int BlockXPushback
        {
            get { return blockXPushback; }
        }

        //-------- Methods --------

        /// <summary>
        /// update the state of the hitbox
        /// </summary>
        public void Update(GameTime gameTime)
        {
            position.X = !isProjectile ? owner.Position.X + xOffset : ownerProjectile.Position.X + xOffset;
            position.Y = !isProjectile ? owner.Position.Y + yOffset : ownerProjectile.Position.Y + yOffset;
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
        public void SetHitboxPos(int xOffset, int yOffset, int width, int height)
        {
            this.xOffset = xOffset;
            this.yOffset = yOffset;
            this.width = width;
            this.height = height;
        }
    }
}
