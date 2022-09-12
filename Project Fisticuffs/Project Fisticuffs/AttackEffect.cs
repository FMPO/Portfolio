using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Fisticuffs
{
    class AttackEffect
    {
        //-------- Fields --------
        private Texture2D attackEffect;
        private int hitEffectWidth;
        private int blockEffectWidth;
        private int hitEffectHeight;
        private int blockEffectHeight;
        private int timeCounter;
        private int effectFrameCount;
        private int effectTimePerFrame;
        private int frame;
        private Vector2 position;
        private bool isHitEffect;
        private bool facingRight;
        private bool effectFinished;

        //-------- Constructor --------
        public AttackEffect(bool isHitEffect, Texture2D attackEffect, int spawnX, int spawnY, bool facingRight)
        {
            this.attackEffect = attackEffect;
            this.isHitEffect = isHitEffect;
            this.facingRight = facingRight;
            timeCounter = 0;
            frame = 0;
            effectFinished = false;

            // effect bounds
            hitEffectHeight = 32;
            hitEffectWidth = 32;
            blockEffectHeight = 32;
            blockEffectWidth = 30 ;


            position = new Vector2(spawnX, spawnY);
            //position = new Rectangle(spawnX, spawnY, isHitEffect ? hitEffect.Width : blockEffect.Width, isHitEffect ? hitEffect.Height : blockEffect.Height);
        }
        //-------- Properties --------
        public bool EffectFinished
        {
            get { return effectFinished; }
            //no set
        }
        //-------- Methods --------

        public void Update()
        {
            //check whether the effect is on hit or on block, and assign the effect frame values accordingly
            if (isHitEffect)
            {
                effectFrameCount = 6;
                effectTimePerFrame = 3;
            }
            else
            {
                effectFrameCount = 7;
                effectTimePerFrame = 2;
            }

            timeCounter++;
            if (timeCounter >= effectTimePerFrame)
            {
                frame++;

                if (frame > effectFrameCount - 1)
                {
                    frame = effectFrameCount - 1;
                    effectFinished = true;
                }

                timeCounter -= effectTimePerFrame;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (isHitEffect)
            {
                sb.Draw(
                attackEffect,
                position,
                new Rectangle(
                    frame * hitEffectWidth,
                    0,
                    hitEffectWidth,
                    hitEffectHeight),
                Color.White,
                0,
                new Vector2(hitEffectWidth/2, hitEffectHeight/2),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
            }
            else
            {
                sb.Draw(
                 attackEffect,
                 position,
                 new Rectangle(
                     frame * blockEffectWidth,
                     0,
                     blockEffectWidth,
                     blockEffectHeight),
                 Color.White,
                 0,
                 new Vector2(blockEffectWidth / 2, blockEffectHeight / 2),
                 1.0f,
                 facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                 0);
            }
        }

    }
}
