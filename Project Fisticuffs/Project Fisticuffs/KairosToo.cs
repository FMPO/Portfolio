using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Project_Fisticuffs
{
    /// <summary>
    /// KairosToo specific child class 
    /// </summary>
    class KairosToo : Player
    {
        //-------- Fields --------


        const int KairosSpriteWidth = 225;
        const int KairosSpriteHeight = 175;
        const int KairosSpriteOriginX = 85;
        const int KairosSpriteOriginY = 167;



        //-------- Constructor --------
        public KairosToo(Texture2D colliderTexture, Texture2D hurtboxTexture, Texture2D hitboxTexture, Texture2D hitEffectTexture, Texture2D blockEffectTexture, List<SoundEffect> soundEffects, InputManager input, Id id, Solid[] bounds, int startingXPos, Texture2D spriteSheet, Camera camera, Random rng)
            : base(colliderTexture, hurtboxTexture, hitboxTexture, hitEffectTexture, blockEffectTexture, soundEffects, input, id, bounds, startingXPos, camera, rng)
        {
            this.spriteSheet = spriteSheet;
            name = "KairosToo";
            walkSpeed = 2;
            runSpeed = 5;
            backDashSpeed = 6;
            jumpStrength = 9;
            standHeight = 64;
            crouchHeight = 32;
            jumpSquat = 4;
            landingLag = 2;
            backDashDuration = 9;
            totalHealth = 200;



            #region//-----------------------------------------------------------all framedata for all attacks-------------------------------------

            attackFrameData = new int[9][];
            attackFrameData[0] = new int[] { 6, 5, 4, 3, 3, 4 }; //Stand Light
            attackFrameData[1] = new int[] { 7, 6, 5, 5, 5 }; //Stand Heavy
            attackFrameData[2] = new int[] { 40, 2, 2, 3, 3, 10, }; //Stand Favor
            attackFrameData[3] = new int[] { 5, 4, 6, 4, 4 }; //Crouch Light
            attackFrameData[4] = new int[] { 8, 6, 6, 9 }; //Crouch Heavy
            attackFrameData[5] = new int[] { 7, 6, 12, 3 }; //Crouch Favor
            attackFrameData[6] = new int[] { 5, 4, 6, 4, 6 }; //Jump Light
            attackFrameData[7] = new int[] { 7, 3, 6, 4, 6 }; //Jump Heavy
            attackFrameData[8] = new int[] { 0 }; //Jump Favor
            #endregion

            //favor costs
            favorCosts = new int[] { 10, 10, 10 };

            hurtbox = new Hurtbox(this, hurtboxTexture, 32, 89, -4, -25);
            //set health to total health
            health = totalHealth;
        }


        //-------- Properties --------



        //-------- Methods --------


        /// <summary>
        /// update the animation based on different speeds for the different states
        /// </summary>
        /// <param name="gameTime"></param>
        public override void UpdateAnimation(GameTime gameTime)
        {
            bool animated = false;
            bool attackAnimated = false;
            int stateFrameCount;

            //this is the row of the attack framedata jagged array used
            int attackType = -1;




            switch (state)
            {
                case PlayerState.Idle:
                case PlayerState.Crouch:
                case PlayerState.JumpSquat:
                case PlayerState.Jump:
                case PlayerState.Landing:
                case PlayerState.BackDash:
                case PlayerState.StandHitStun:
                case PlayerState.CrouchHitStun:
                case PlayerState.JumpHitStun:
                case PlayerState.StandBlock:
                case PlayerState.CrouchBlock:
                case PlayerState.JumpBlock:
                case PlayerState.StandBlockStun:
                case PlayerState.CrouchBlockStun:
                case PlayerState.JumpBlockStun:
                    frame = 0;
                    animated = false;
                    attackAnimated = false;
                    stateFrameCount = 1;

                    break;
                case PlayerState.WalkForward:
                    animated = true;
                    attackAnimated = false;
                    timePerFrame = 12;
                    stateFrameCount = 4;

                    break;
                case PlayerState.WalkBackward:
                    animated = true;
                    attackAnimated = false;
                    timePerFrame = 12;
                    stateFrameCount = 4;

                    break;
                case PlayerState.Run:
                    animated = true;
                    attackAnimated = false;
                    timePerFrame = 3;
                    stateFrameCount = 8;
                    break;

                case PlayerState.StandAttack:
                    animated = false;
                    attackAnimated = true;
                    switch (atkState)
                    {
                        case AttackState.Light:
                            stateFrameCount = 6;
                            attackType = 0;
                            break;
                        case AttackState.Heavy:
                            stateFrameCount = 5;
                            attackType = 1;
                            break;
                        case AttackState.Favor:
                            stateFrameCount = 6;
                            attackType = 2;
                            break;
                        default:
                            stateFrameCount = 0;
                            attackType = -1;
                            break;
                    }
                    break;
                case PlayerState.CrouchAttack:
                    animated = false;
                    attackAnimated = true;
                    stateFrameCount = 0;
                    switch (atkState)
                    {
                        case AttackState.Light:
                            stateFrameCount = 5;
                            attackType = 3;
                            break;
                        case AttackState.Heavy:
                            stateFrameCount = 4;
                            attackType = 4;
                            break;
                        case AttackState.Favor:
                            stateFrameCount = 0;
                            attackType = 5;
                            break;
                        default:
                            stateFrameCount = 0;
                            attackType = -1;
                            break;
                    }
                    break;
                case PlayerState.JumpAttack:
                    animated = false;
                    attackAnimated = true;
                    stateFrameCount = 0;
                    switch (atkState)
                    {
                        case AttackState.Light:
                            stateFrameCount = 5;
                            attackType = 6;
                            break;
                        case AttackState.Heavy:
                            stateFrameCount = 5;
                            attackType = 7;
                            break;
                        case AttackState.Favor:
                            stateFrameCount = 0;
                            attackType = 8;
                            break;
                        default:
                            stateFrameCount = 0;
                            attackType = -1;
                            break;
                    }
                    break;
                default:
                    frame = 0;
                    animated = false;
                    stateFrameCount = 1;
                    break;
            }
            if (animated)
            {
                timeCounter++;
                if (timeCounter >= timePerFrame)
                {
                    frame++;

                    if (frame > stateFrameCount - 1)
                    {
                        frame = 0;
                    }

                    timeCounter -= timePerFrame;
                }
            }
            else if (attackAnimated)
            {
                timeCounter++;
                if (timeCounter >= attackFrameData[attackType][frame])
                {
                    frame++;

                    if (frame > stateFrameCount - 1)
                    {
                        frame = stateFrameCount - 1;
                    }

                    timeCounter -= attackFrameData[attackType][frame - 1];
                }
            }
        }

        /// <summary>
        /// override of the player draw draws the sprites according to the state
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            switch (state)
            {
                case PlayerState.Idle:
                    DrawIdle(sb);
                    break;
                case PlayerState.Crouch:
                    DrawCrouch(sb);
                    break;
                case PlayerState.WalkForward:
                    DrawWalkForward(sb);
                    break;
                case PlayerState.Run:
                    DrawRun(sb);
                    break;
                case PlayerState.WalkBackward:
                    DrawWalkBackward(sb);
                    break;
                case PlayerState.BackDash:
                    DrawBackDash(sb);
                    break;
                case PlayerState.JumpSquat:
                    DrawJumpSquat(sb);
                    break;
                case PlayerState.Jump:
                    DrawJump(sb);
                    break;
                case PlayerState.Landing:
                    DrawJumpSquat(sb);
                    break;
                case PlayerState.Tech:
                    DrawTech(sb);
                    break;
                case PlayerState.StandHitStun:
                    DrawStandHitstun(sb);
                    break;
                case PlayerState.CrouchHitStun:
                    DrawCrouchHitstun(sb);
                    break;
                case PlayerState.JumpHitStun:
                    DrawJumpHitstun(sb);
                    break;
                case PlayerState.StandBlock:
                case PlayerState.StandBlockStun:
                    DrawStandBlock(sb);
                    break;
                case PlayerState.CrouchBlock:
                case PlayerState.CrouchBlockStun:
                    DrawCrouchdBlock(sb);
                    break;
                case PlayerState.JumpBlock:
                case PlayerState.JumpBlockStun:
                    DrawJumpBlock(sb);
                    break;
                case PlayerState.StandAttack:
                    switch (atkState)
                    {
                        case AttackState.Light:
                            DrawStandLight(sb);
                            break;
                        case AttackState.Heavy:
                            DrawStandHeavy(sb);
                            break;
                        case AttackState.Favor:
                            DrawStandFavor(sb);
                            break;
                        default:
                            break;
                    }
                    break;
                case PlayerState.CrouchAttack:
                    switch (atkState)
                    {
                        case AttackState.Light:
                            DrawCrouchLight(sb);
                            break;
                        case AttackState.Heavy:
                            DrawCrouchHeavy(sb);
                            break;
                        case AttackState.Favor:
                            break;
                    }
                    break;
                case PlayerState.JumpAttack:
                    switch (atkState)
                    {
                        case AttackState.Light:
                            DrawJumpLight(sb);
                            break;
                        case AttackState.Heavy:
                            DrawJumpHeavy(sb);
                            break;
                        case AttackState.Favor:
                            break;
                    }
                    break;

            }

            //do this to see the collision boxes
            base.Draw(sb);
        }

        /// <summary>
        /// update the position and size of kairos' hurtbox based on his state
        /// </summary>
        /// <param name="gameTime"></param>
        public override void UpdateHitAndHurtbox(GameTime gameTime)
        {
            switch (state)
            {
                case PlayerState.Idle:
                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-4, 32), -25, 32, 89);
                    break;

                case PlayerState.Crouch:
                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-13, 45), -14, 45, 46);

                    break;

                case PlayerState.WalkForward:
                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-4, 32), -25, 32, 89);
                    break;

                case PlayerState.WalkBackward:
                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-4, 32), -25, 32, 89);
                    break;

                case PlayerState.Run:
                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-12, 75), -3, 75, 67);

                    break;

                case PlayerState.BackDash:
                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-25, 36), -19, 36, 83);

                    break;

                case PlayerState.JumpSquat:
                case PlayerState.Landing:
                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-4, 32), -17, 32, 81);

                    break;

                case PlayerState.Jump:
                case PlayerState.Tech:
                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-6, 35), -38, 35, 69);
                    break;
                case PlayerState.StandBlock:
                case PlayerState.StandBlockStun:
                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-4, 32), -25, 32, 89);
                    break;
                case PlayerState.CrouchBlock:
                case PlayerState.CrouchBlockStun:
                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-13, 45), -14, 45, 46);
                    break;
                case PlayerState.JumpBlock:
                case PlayerState.JumpBlockStun:
                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-6, 35), -38, 35, 69);
                    break;
                case PlayerState.StandHitStun:
                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-4, 32), -25, 32, 89);
                    break;
                case PlayerState.CrouchHitStun:
                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-13, 45), -14, 45, 46);
                    break;
                case PlayerState.JumpHitStun:
                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-19, 64), -42, 64, 83);
                    break;

                case PlayerState.StandAttack:
                    switch (atkState)
                    {
                        case AttackState.Light:
                            switch (frame)
                            {
                                case 0:
                                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-24, 43), -11, 43, 75);
                                    break;
                                case 1:
                                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-10, 54), -10, 54, 73);
                                    if (hitboxes.Count < 1)
                                    {
                                        hitboxes.Add(new Hitbox(this, hitboxTexture, GetHurtboxXOffset(17, 61), 9, 61, 20, 12, false, 5, 4, 10, BlockType.Mid, 5, 4));
                                    }
                                    break;
                                case 2:
                                    ClearAllHitboxes();
                                    break;
                                case 3:
                                case 4:
                                    break;
                                case 5:
                                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-10, 39), -16, 39, 79);
                                    break;
                            }
                            break;
                        case AttackState.Heavy:
                            switch (frame)
                            {
                                case 0:
                                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-1, 59), -16, 59, 80);
                                    break;
                                case 1:
                                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(12, 69), -1, 69, 65);
                                    if (hitboxes.Count < 1)
                                    {
                                        hitboxes.Add(new Hitbox(this, hitboxTexture, GetHurtboxXOffset(11, 135), -31, 135, 95, 20, false, 8, 5, 20, BlockType.Mid, 10, 5));
                                    }
                                    break;
                                case 2:
                                    ClearAllHitboxes();
                                    break;
                                case 3:
                                    break;
                                case 4:
                                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(0, 49), -21, 49, 85);
                                    break;
                            }
                            break;
                        case AttackState.Favor:
                            switch (frame)
                            {
                                case 0:
                                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-24, 43), -11, 43, 75);
                                    break;
                                case 1:
                                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-10, 54), -10, 54, 73);
                                    if (hitboxes.Count < 1)
                                    {
                                        hitboxes.Add(new Hitbox(this, hitboxTexture, GetHurtboxXOffset(17, 61), 9, 61, 20, 30, false, 50, 10, 100, BlockType.Mid, 15, 8));
                                    }
                                    break;
                                case 2:
                                    ClearAllHitboxes();
                                    break;
                                case 3:
                                case 4:
                                    break;
                                case 5:
                                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-10, 39), -16, 39, 79);
                                    break;
                            }
                            break;
                    }
                    break;

                case PlayerState.CrouchAttack:
                    switch (atkState)
                    {
                        case AttackState.Light:
                            switch (frame)
                            {
                                case 0:
                                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-13, 45), -14, 45, 46);
                                    break;
                                case 1:
                                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-13, 61), -14, 61, 46);
                                    if (hitboxes.Count < 1)
                                    {
                                        hitboxes.Add(new Hitbox(this, hitboxTexture, GetHurtboxXOffset(10, 50), 14, 50, 18, 10, false, 4, 5, 8, BlockType.Low, 5, 4));
                                    }
                                    break;
                                case 2:
                                    ClearAllHitboxes();
                                    break;
                                case 3:
                                    break;
                                case 4:
                                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-13, 37), -14, 37, 46);
                                    break;
                            }
                            break;
                        case AttackState.Heavy:
                            switch (frame)
                            {
                                case 0:
                                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-10, 54), 9, 54, 23);
                                    break;
                                case 1:
                                    if (hitboxes.Count < 1)
                                    {
                                        hitboxes.Insert(0, new Hitbox(this, hitboxTexture, GetHurtboxXOffset(-6, 83), -59, 83, 66, 12, true, 1, 10, 20, BlockType.Mid, 12, 7));
                                    }
                                    break;
                                case 2:
                                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-10, 54), -14, 54, 46);
                                    ClearAllHitboxes();
                                    break;
                                case 3:
                                    break;
                            }
                            break;
                        case AttackState.Favor:
                            switch (frame)
                            {
                                case 0:

                                    break;
                                case 1:

                                    break;
                                case 2:

                                    break;
                                case 3:

                                    break;
                            }
                            break;
                    }
                    break;

                case PlayerState.JumpAttack:
                    switch (atkState)
                    {
                        case AttackState.Light:
                            switch (frame)
                            {
                                case 0:
                                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-4, 34), -38, 34, 65);
                                    break;
                                case 1:
                                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-6, 45), -42, 45, 83);
                                    if (hitboxes.Count < 1)
                                    {
                                        hitboxes.Add(new Hitbox(this, hitboxTexture, GetHurtboxXOffset(13, 37), -26, 37, 75, 12, false, 3, 10, 10, BlockType.High, 5, 1));
                                    }
                                    break;
                                case 2:
                                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-14, 46), -49, 46, 78);
                                    ClearAllHitboxes();
                                    break;
                                case 3:
                                    break;
                                case 4:
                                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-8, 37), -42, 37, 65);
                                    break;
                            }
                            break;
                        case AttackState.Heavy:
                            switch (frame)
                            {
                                case 0:
                                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-6, 39), -30, 39, 54);
                                    break;
                                case 1:
                                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-8, 46), -30, 46, 58);
                                    if (hitboxes.Count < 1)
                                    {
                                        hitboxes.Add(new Hitbox(this, hitboxTexture, GetHurtboxXOffset(-20, 119), -47, 119, 128, 20, true, 2, 10, 15, BlockType.High, 10, 1));
                                    }
                                    break;
                                case 2:
                                    hitboxes.Insert(0, new Hitbox(this, hitboxTexture, GetHurtboxXOffset(-52, 140), -93, 140, 140, 20, false, 5, 6, 18, BlockType.High, 10, 2));
                                    hitboxes.RemoveAt(1);
                                    break;
                                case 3:
                                    ClearAllHitboxes();
                                    break;
                                case 4:
                                    break;
                            }
                            break;
                        case AttackState.Favor:
                            switch (frame)
                            {
                                case 0:

                                    break;
                                case 1:

                                    break;
                                case 2:

                                    break;
                                case 3:

                                    break;
                            }
                            break;
                    }
                    break;

                default:
                    hurtbox.SetHurtboxPos(GetHurtboxXOffset(-4, 32), -25, 32, 89);
                    break;
            }
            hurtbox.Update(gameTime);
            foreach (Hitbox n in hitboxes)
            {
                n.Update(gameTime);
                HitboxCollision(n);
            }

        }

        public override void UpdateAttackState(GameTime gameTime)
        {
            switch (state)
            {
                case PlayerState.StandAttack:
                    switch (atkState)
                    {
                        case AttackState.Light:

                            delayCounter++;
                            if (delayCounter > GetTotalDuration(attackFrameData[0]))
                            {
                                SetState(PlayerState.Idle);
                                delayCounter = 0;
                            }
                            if (canCancel)
                            {
                                if (input.InputPressed(Inputs.Light, inputBufferArray[5]))
                                {
                                    ClearAllHitboxes();
                                    delayCounter = 0;
                                    SetState(!input.Down ? PlayerState.StandAttack : PlayerState.CrouchAttack);
                                    SetAttackState(AttackState.Light);
                                }
                                else if (input.InputPressed(Inputs.Heavy, inputBufferArray[6]))
                                {
                                    ClearAllHitboxes();
                                    delayCounter = 0;
                                    SetState(!input.Down ? PlayerState.StandAttack : PlayerState.CrouchAttack);
                                    SetAttackState(AttackState.Heavy);
                                }
                            }
                            break;
                        case AttackState.Heavy:

                            delayCounter++;
                            if (delayCounter > GetTotalDuration(attackFrameData[1]))
                            {
                                SetState(PlayerState.Idle);
                                delayCounter = 0;
                            }
                            break;
                        case AttackState.Favor:

                            delayCounter++;
                            if (delayCounter > GetTotalDuration(attackFrameData[2]))
                            {
                                SetState(PlayerState.Idle);
                                delayCounter = 0;
                            }
                            break;

                    }
                    break;
                case PlayerState.CrouchAttack:

                    switch (atkState)
                    {
                        case AttackState.Light:

                            delayCounter++;
                            if (delayCounter > GetTotalDuration(attackFrameData[3]))
                            {
                                SetState(PlayerState.Crouch);
                                delayCounter = 0;
                            }
                            if (canCancel)
                            {
                                if (input.InputPressed(Inputs.Light, inputBufferArray[5]))
                                {
                                    ClearAllHitboxes();
                                    delayCounter = 0;
                                    SetState(!input.Down ? PlayerState.StandAttack : PlayerState.CrouchAttack);
                                    SetAttackState(AttackState.Light);
                                }
                                else if (input.InputPressed(Inputs.Heavy, inputBufferArray[6]))
                                {
                                    ClearAllHitboxes();
                                    delayCounter = 0;
                                    SetState(!input.Down ? PlayerState.StandAttack : PlayerState.CrouchAttack);
                                    SetAttackState(AttackState.Heavy);
                                }
                            }
                            break;
                        case AttackState.Heavy:

                            delayCounter++;
                            if (delayCounter > GetTotalDuration(attackFrameData[4]))
                            {
                                SetState(PlayerState.Crouch);
                                delayCounter = 0;
                            }
                            break;
                        case AttackState.Favor:

                            delayCounter++;
                            if (delayCounter > GetTotalDuration(attackFrameData[5]))
                            {
                                SetState(PlayerState.Crouch);
                                delayCounter = 0;
                            }
                            break;
                    }
                    break;
                case PlayerState.JumpAttack:
                    switch (atkState)
                    {
                        case AttackState.Light:

                            delayCounter++;
                            if (delayCounter > GetTotalDuration(attackFrameData[6]))
                            {
                                SetState(PlayerState.Jump);
                                delayCounter = 0;
                            }
                            if (canCancel)
                            {
                                if (input.InputPressed(Inputs.Light, inputBufferArray[5]))
                                {
                                    ClearAllHitboxes();
                                    delayCounter = 0;
                                    SetState(PlayerState.JumpAttack);
                                    SetAttackState(AttackState.Light);
                                }
                                else if (input.InputPressed(Inputs.Heavy, inputBufferArray[6]))
                                {
                                    ClearAllHitboxes();
                                    delayCounter = 0;
                                    SetState(PlayerState.JumpAttack);
                                    SetAttackState(AttackState.Heavy);
                                }
                            }
                            break;
                        case AttackState.Heavy:

                            delayCounter++;
                            if (delayCounter > GetTotalDuration(attackFrameData[7]))
                            {
                                SetState(PlayerState.Jump);
                                delayCounter = 0;
                            }
                            break;
                        case AttackState.Favor:

                            delayCounter++;
                            if (delayCounter > GetTotalDuration(attackFrameData[8]))
                            {
                                SetState(PlayerState.Jump);
                                delayCounter = 0;
                            }
                            break;
                    }
                    break;
            }
        }

        public int GetTotalDuration(int[] attackFrameData)
        {
            int sum = 0;
            for (int i = 0; i < attackFrameData.Length; i++)
            {
                sum += attackFrameData[i];
            }
            return sum;

        }

        /// <summary>
        /// updates character from file
        /// </summary>
        public override void StatUpdate()
        {
            int[] stats = ReadStats(name);
            if (stats == null)
            {
                return;
            }
            walkSpeed = stats[0];
            runSpeed = stats[1];
            backDashSpeed = stats[2];
            jumpStrength = stats[3];
            standHeight = stats[4];
            crouchHeight = stats[5];
            jumpSquat = stats[6];
            landingLag = stats[7];
            backDashDuration = stats[8];
            totalHealth = stats[9];
        }

        #region//############################################################################# Draw Methods for each state ###################################################################################
        private void DrawIdle(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + standHeight),
                new Rectangle(
                    frame * KairosSpriteWidth,
                    0,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
        }
        private void DrawWalkForward(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + standHeight),
                new Rectangle(
                    frame * KairosSpriteWidth,
                    2 * KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
        }
        private void DrawRun(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + standHeight),
                new Rectangle(
                    frame * KairosSpriteWidth,
                    4 * KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
        }
        private void DrawWalkBackward(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + standHeight),
                new Rectangle(
                    frame * KairosSpriteWidth,
                    5 * KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
        }
        private void DrawCrouch(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + crouchHeight),
                new Rectangle(
                    frame * KairosSpriteWidth,
                    KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
        }
        private void DrawJumpSquat(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + standHeight),
                new Rectangle(
                    frame * KairosSpriteWidth,
                    3 * KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
        }
        private void DrawJump(SpriteBatch spriteBatch)
        {
            if (vspd < 0)
            {
                spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + standHeight),
                new Rectangle(
                    KairosSpriteWidth,
                    3 * KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
            }
            else
            {
                spriteBatch.Draw(
                                spriteSheet,
                                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + standHeight),
                                new Rectangle(
                                    2 * KairosSpriteWidth,
                                    3 * KairosSpriteHeight,
                                    KairosSpriteWidth,
                                    KairosSpriteHeight),
                                Color.White,
                                0,
                                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                                1.0f,
                                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                0);
            }

        }
        private void DrawBackDash(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + standHeight),
                new Rectangle(
                    frame * KairosSpriteWidth,
                    6 * KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
        }
        private void DrawStandHitstun(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + standHeight),
                new Rectangle(
                    frame * KairosSpriteWidth,
                    7 * KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
        }
        private void DrawCrouchHitstun(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + crouchHeight),
                new Rectangle(
                    frame * KairosSpriteWidth,
                    8 * KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
        }
        private void DrawJumpHitstun(SpriteBatch spriteBatch)
        {
            if (vspd < -2)
            {
                spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + standHeight),
                new Rectangle(
                    0,
                    9 * KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
            }
            else if (vspd >= -2 && vspd < 2)
            {
                spriteBatch.Draw(
                                spriteSheet,
                                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + standHeight),
                                new Rectangle(
                                    KairosSpriteWidth,
                                    9 * KairosSpriteHeight,
                                    KairosSpriteWidth,
                                    KairosSpriteHeight),
                                Color.White,
                                0,
                                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                                1.0f,
                                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                0);
            }
            else
            {
                spriteBatch.Draw(
                                spriteSheet,
                                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + standHeight),
                                new Rectangle(
                                    2 * KairosSpriteWidth,
                                    9 * KairosSpriteHeight,
                                    KairosSpriteWidth,
                                    KairosSpriteHeight),
                                Color.White,
                                0,
                                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                                1.0f,
                                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                0);
            }
        }

        private void DrawStandBlock(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + standHeight),
                new Rectangle(
                    frame * KairosSpriteWidth,
                    10 * KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
        }
        private void DrawCrouchdBlock(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + crouchHeight),
                new Rectangle(
                    frame * KairosSpriteWidth,
                    11 * KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
        }
        private void DrawJumpBlock(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + standHeight),
                new Rectangle(
                    frame * KairosSpriteWidth,
                    12 * KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
        }

        private void DrawStandLight(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + standHeight),
                new Rectangle(
                    frame * KairosSpriteWidth,
                    13 * KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
        }
        private void DrawStandHeavy(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + standHeight),
                new Rectangle(
                    frame * KairosSpriteWidth,
                    14 * KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
        }
        private void DrawStandFavor(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + standHeight),
                new Rectangle(
                    frame * KairosSpriteWidth,
                    13 * KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.Blue,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
        }
        private void DrawCrouchLight(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + crouchHeight),
                new Rectangle(
                    frame * KairosSpriteWidth,
                    15 * KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
        }
        private void DrawCrouchHeavy(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + crouchHeight),
                new Rectangle(
                    frame * KairosSpriteWidth,
                    16 * KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
        }
        private void DrawJumpLight(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + standHeight),
                new Rectangle(
                    frame * KairosSpriteWidth,
                    17 * KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
        }
        private void DrawJumpHeavy(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + standHeight),
                new Rectangle(
                    frame * KairosSpriteWidth,
                    18 * KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
        }
        //This will be the actual stand favor method
        /*private void DrawStandFavor(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + standHeight),
                new Rectangle(
                    frame * KairosSpriteWidth,
                    19 * KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
        }*/
        private void DrawCrouchFavor(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + crouchHeight),
                new Rectangle(
                    frame * KairosSpriteWidth,
                    20 * KairosSpriteHeight,
                    KairosSpriteWidth,
                    KairosSpriteHeight),
                Color.White,
                0,
                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                1.0f,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0);
        }
        private void DrawTech(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                                spriteSheet,
                                new Vector2(position.X + (colliderTexture.Width / 2), position.Y + standHeight),
                                new Rectangle(
                                    2 * KairosSpriteWidth,
                                    3 * KairosSpriteHeight,
                                    KairosSpriteWidth,
                                    KairosSpriteHeight),
                                Color.Silver,
                                0,
                                new Vector2((facingRight ? KairosSpriteOriginX : KairosSpriteWidth - KairosSpriteOriginX), KairosSpriteOriginY),
                                1.0f,
                                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                0);
        }
        #endregion
    }
}
