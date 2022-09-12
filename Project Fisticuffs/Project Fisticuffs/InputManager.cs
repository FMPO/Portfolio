using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;


namespace Project_Fisticuffs
{
    /// <summary>
    /// the possible inputs that can be made by the controller
    /// </summary>
    enum Inputs
    {
        Up,
        Down,
        Left,
        Right,
        Start,
        Light,
        Heavy,
        Favor,

    }
    class InputManager
    {
        //-------- Fields --------
        private PlayerIndex playerNum;
        private KeyboardState kbState;

        //possible controller inputs
        private bool left;
        private bool right;
        private bool up;
        private bool down;
        private bool start;
        private bool light;
        private bool heavy;
        private bool favor;

        //checking each controller input's previous state
        private bool leftPrev;
        private bool rightPrev;
        private bool upPrev;
        private bool downPrev;
        private bool startPrev;
        private bool lightPrev;
        private bool heavyPrev;
        private bool favorPrev;


        //-------- Constructor --------
        public InputManager(PlayerIndex playerNum, KeyboardState kbState)
        {
            this.playerNum = playerNum;
        }


        //-------- Properties --------
        public bool Left
        {
            get { return left; }
            set { left = value; }
        }

        public bool Right
        {
            get { return right; }
            set { right = value; }
        }

        public bool Up
        {
            get { return up; }
            set { up = value; }
        }

        public bool Down
        {
            get { return down; }
            set { down = value; }
        }

        public bool Start
        {
            get { return start; }
            set { start = value; }
        }

        public bool Light
        {
            get { return light; }
            set { light = value; }
        }


        public bool Heavy
        {
            get { return heavy; }
            set { heavy = value; }
        }

        public bool Favor
        {
            get { return favor; }
            set { favor = value; }
        }


        //-------- Methods --------

        /// <summary>
        /// this processes all of the inputs of this given input manager
        /// </summary>
        /// <returns></returns>
        public void Update(KeyboardState kbState)
        {
         

            //Update the controller state of the current player
            up = (GamePad.GetState(playerNum).DPad.Up == ButtonState.Pressed || (playerNum == PlayerIndex.One? kbState.IsKeyDown(Keys.W): kbState.IsKeyDown(Keys.Up)));
            down = (GamePad.GetState(playerNum).DPad.Down == ButtonState.Pressed || (playerNum == PlayerIndex.One ? kbState.IsKeyDown(Keys.S) : kbState.IsKeyDown(Keys.Down)));
            left = (GamePad.GetState(playerNum).DPad.Left == ButtonState.Pressed || (playerNum == PlayerIndex.One ? kbState.IsKeyDown(Keys.A) : kbState.IsKeyDown(Keys.Left)));
            right = (GamePad.GetState(playerNum).DPad.Right == ButtonState.Pressed || (playerNum == PlayerIndex.One ? kbState.IsKeyDown(Keys.D) : kbState.IsKeyDown(Keys.Right)));
            start = (GamePad.GetState(playerNum).Buttons.Start == ButtonState.Pressed || (playerNum == PlayerIndex.One ? kbState.IsKeyDown(Keys.Enter) : kbState.IsKeyDown(Keys.NumPad9)));
            light = (GamePad.GetState(playerNum).Buttons.X == ButtonState.Pressed|| (playerNum == PlayerIndex.One ? kbState.IsKeyDown(Keys.R) : (kbState.IsKeyDown(Keys.NumPad1)|| (kbState.IsKeyDown(Keys.I)))));
            heavy = (GamePad.GetState(playerNum).Buttons.Y == ButtonState.Pressed || (playerNum == PlayerIndex.One ? kbState.IsKeyDown(Keys.T) : (kbState.IsKeyDown(Keys.NumPad2) || (kbState.IsKeyDown(Keys.O)))));
            favor = (GamePad.GetState(playerNum).Buttons.B == ButtonState.Pressed || GamePad.GetState(playerNum).Buttons.RightShoulder == ButtonState.Pressed || (playerNum == PlayerIndex.One ? kbState.IsKeyDown(Keys.Y) : (kbState.IsKeyDown(Keys.NumPad3) || (kbState.IsKeyDown(Keys.P)))));



        }

        /// <summary>
        /// set variables for the previous state of the controller
        /// </summary>
        public void SetPrevInputs()
        {
            upPrev = up;
            downPrev = down;
            leftPrev = left;
            rightPrev = right;
            startPrev = start;
            lightPrev = light;
            heavyPrev = heavy;
            favorPrev = favor;
        }

        public bool InputPressed(Inputs input, bool overrideBool)
        {
            if (overrideBool)
            {
                return true;
            }
            switch (input)
            {
                case Inputs.Up:
                    return (up && !upPrev);

                case Inputs.Down:
                    return (down && !downPrev);

                case Inputs.Left:
                    return (left && !leftPrev);

                case Inputs.Right:
                    return (right && !rightPrev);

                case Inputs.Start:
                    return (start && !startPrev);

                case Inputs.Light:
                    return (light && !lightPrev);

                case Inputs.Heavy:
                    return (heavy && !heavyPrev);

                case Inputs.Favor:
                    return (favor && !favorPrev);

                default:
                    return false;
            }
        }



    }
}
