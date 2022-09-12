using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//Project Fisticuffs Camera
/// <summary>
/// Camera updates position so it is always at the midpoint between the two characters
/// Tracks horizontal and vertical position
/// </summary>

namespace Project_Fisticuffs
{
    class Camera
    {
        //fields
        private Rectangle position;
        private Rectangle drawPosition;
        private Texture2D texture;
        private Solid[] walls;
        private Player p1;
        private Player p2;
        private Vector2 viewportPos;
        private GameStates currentState;
        private Viewport viewport;
        private float shakeOffset;
        private float shakeTimer;
        private Random rng;


        //camera takes position rectangle, texture for CameraTest (temp), solid array for the walls, and the two players
        public Camera(Rectangle position, Texture2D texture, Solid[] walls, Player p1, Player p2, Viewport viewport, GameStates currentState, Random rng)
        {
            this.position = position;
            drawPosition = position;
            this.walls = walls;
            this.p1 = p1;
            this.p2 = p2;
            this.texture = texture;
            this.viewport = viewport;
            viewportPos = new Vector2(viewport.X, viewport.Y);
            this.currentState = currentState;
            this.rng = rng;
            shakeOffset = 0;
            shakeTimer = 0;
        }

        //  PROPERTIES
        public Vector2 ViewportPos
        {
            get { return viewportPos; }
            set { viewportPos = value; }
        }

        public Rectangle Position
        {
            get { return position; }
            set { position = value; }
        }

        public Rectangle DrawPosition
        {
            get { return drawPosition; }
            set { drawPosition = value; }
        }

        public float ShakeTimer
        {
            get { return shakeTimer; }
            set { shakeTimer = value; }
        }


        //  METHODS & HELPERS

        //Left wall collision check
        public bool LeftWallCollision(Rectangle leftWall)
        {
            return position.Left <= leftWall.Right;
        }


        //Right wall collision check
        public bool RightWallCollision(Rectangle rightWall)
        {
            return position.Right >= rightWall.Left;
        }


        //Helper method to get horizontal player distance
        public int GetHDistance(Player p1, Player p2)
        {
            //returns distance between players
            return System.Math.Abs((p1.X + (p1.Position.Width / 2)) - (p2.X + (p2.Position.Width / 2)));
        }

        //Helper method to get vertical player distance
        public int GetVDistance(Player p1, Player p2)
        {
            //returns vertical distance between players
            return System.Math.Abs((p1.Y + (p1.StandHeight / 2)) - (p2.Y + (p2.StandHeight / 2)));
        }

        #region//old transform viewPort method
        //old transform Viewport method
        //public Matrix TransformViewport(Viewport viewport)
        //{
        //    if (currentState == GameStates.GamePlay)
        //    {
        //        return
        //        Matrix.CreateTranslation(new Vector3(-position.X - (position.Width / 6)/* + shakeOffset*/, -position.Y - (position.Height / 6), 0)) *
        //        Matrix.CreateRotationZ(0.0f) *
        //        Matrix.CreateScale(new Vector3(3f, 3f, 1)) *
        //        Matrix.CreateTranslation(new Vector3(viewport.Width / 2, viewport.Height / 2, 0));
        //    }
        //    else
        //    {
        //        return
        //        Matrix.CreateTranslation(new Vector3(viewport.Width / -2, viewport.Height / -2, 0)) *
        //        Matrix.CreateRotationZ(0.0f) *
        //        Matrix.CreateScale(new Vector3(1.0f, 1.0f, 1)) *
        //        Matrix.CreateTranslation(new Vector3(viewport.Width / 2, viewport.Height / 2, 0));
        //    }

        //}
        #endregion

        public Matrix TransformViewport(Viewport viewport)
        {
            if (currentState == GameStates.GamePlay)
            {
                return
                Matrix.CreateTranslation(new Vector3(-drawPosition.X - (drawPosition.Width / 6), -drawPosition.Y - (position.Height / 6), 0)) *
                Matrix.CreateRotationZ(0.0f) *
                Matrix.CreateScale(new Vector3(3f, 3f, 1)) *
                Matrix.CreateTranslation(new Vector3(viewport.Width / 2, viewport.Height / 2, 0));
            }
            else
            {
                return
                Matrix.CreateTranslation(new Vector3(viewport.Width / -2, viewport.Height / -2, 0)) *
                Matrix.CreateRotationZ(0.0f) *
                Matrix.CreateScale(new Vector3(1.0f, 1.0f, 1)) *
                Matrix.CreateTranslation(new Vector3(viewport.Width / 2, viewport.Height / 2, 0));
            }

        }




        //Update
        public virtual void Update(GameTime GameTime, GameStates currentStateInput)
        {
            currentState = currentStateInput;
            viewportPos = new Vector2(viewport.X, viewport.Y);
            TransformViewport(viewport);

            if (p1.Y > p2.Y)
            {
                position.Y = -90 + p2.Y + 32 + (GetVDistance(p1, p2) / 2) - (position.Height / 2);
            }
            else if (p2.Y > p1.Y)
            {
                position.Y = -90 + p1.Y + 32 + (GetVDistance(p1, p2) / 2) - (position.Height / 2);
            }
            else if (p1.Y == p2.Y)
            {
                position.Y = -90 + p1.Y + 32 - (position.Height / 2);
            }
            if (position.Bottom > 720)
            {
                position.Y = 720 - position.Height;
            }


            if (GetHDistance(p1, p2) < (427 - (p1.Position.Width * 2)))
            {
                //as long as camera doesn't collide with any walls, can move right or left
                if (LeftWallCollision(walls[0].Position) == false && RightWallCollision(walls[1].Position) == false)
                {
                    if ((p1.X + p1.Position.Width / 2) > (p2.X + p1.Position.Width / 2))    //if p1 is to the right of p2
                    {
                        //midpoint is calculated going up from p2
                        position.X = p2.X + (GetHDistance(p1, p2) / 2) - (position.Width / 2);
                    }
                    else if ((p2.X + p1.Position.Width / 2) > (p1.X + p1.Position.Width / 2))
                    {
                        //midpoint is calculated going up from p1
                        position.X = p1.X + (GetHDistance(p1, p2) / 2) - (position.Width / 2);
                    }
                }
                //if at left wall, camera can only move right
                else if (LeftWallCollision(walls[0].Position) == true && RightWallCollision(walls[1].Position) == false)
                {
                    //left collision true, right collision false so can only move right
                    if ((p1.X + p1.Position.Width / 2) > (p2.X + p1.Position.Width / 2))
                    {
                        if (p2.X + (p1.Position.Width / 2) + (GetHDistance(p1, p2) / 2) - (position.Width / 2) > 20)
                        {
                            //midpoint is calculated going up from p2
                            position.X = p2.X + (GetHDistance(p1, p2) / 2) - (position.Width / 2);
                        }
                    }
                    else if ((p2.X + p1.Position.Width / 2) > (p1.X + p1.Position.Width / 2))
                    {
                        if (p1.X + (p1.Position.Width / 2) + (GetHDistance(p1, p2) / 2) - (position.Width / 2) > 20)
                        {
                            //midpoint is calculated going up from p1
                            position.X = p1.X + (GetHDistance(p1, p2) / 2) - (position.Width / 2);
                        }
                    }
                }
                //if at right wall, camera can only move left
                else if (LeftWallCollision(walls[0].Position) == false && RightWallCollision(walls[1].Position) == true)
                {
                    //right collision true, left collision false so can only move left
                    if ((p1.X + p1.Position.Width / 2) > (p2.X + p1.Position.Width / 2))
                    {
                        if (p1.X + (p1.Position.Width / 2) - (GetHDistance(p1, p2) / 2) + (position.Width / 2) < 1290)
                        {
                            //midpoint is calculated going up from p2
                            position.X = p2.X + (GetHDistance(p1, p2) / 2) - (position.Width / 2);
                        }
                    }
                    else if ((p2.X + p1.Position.Width / 2) > (p1.X + p1.Position.Width / 2))
                    {
                        if (p2.X + (p1.Position.Width / 2) - (GetHDistance(p1, p2) / 2) + (position.Width / 2) < 1290)
                        {
                            //midpoint is calculated going up from p1
                            position.X = p1.X + (GetHDistance(p1, p2) / 2) - (position.Width / 2);
                        }
                    }
                }
            }
            drawPosition = position;

        }

        //Camera shake method
        //private void cameraShake(int direction)
        //{
        //    position.X += 2 * direction;
        //    position.Y += 2 * direction;
        //}

        //Timed method to run camerashake
        public void startShake(int hitstop)
        {
            #region //old code
            //int shakeTimer = 0;

            //while (shakeTimer <= 10)
            //{
            //    cameraShake(1);
            //    shakeTimer += 1;
            //}
            //while (shakeTimer <= 30)
            //{
            //    cameraShake(-1);
            //    shakeTimer += 1;
            //}
            //while (shakeTimer <= 50)
            //{
            //    cameraShake(1);
            //    shakeTimer += 1;
            //}
            //while (shakeTimer <= 70)
            //{
            //    cameraShake(-1);
            //    shakeTimer += 1;
            //}
            //while (shakeTimer <= 80)
            //{
            //    cameraShake(1);
            //    shakeTimer += 1;
            //}
            #endregion
            if(shakeTimer < hitstop)
            {
                drawPosition.X += shakeTimer % 2 == 0 ? 2 : -2;
                shakeTimer++;
            }
            else
            {
                shakeTimer = 0;
            }

            //shakeTimer = 0;
        }

        //Draw
        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, Color.Green);

        }

    }
}
