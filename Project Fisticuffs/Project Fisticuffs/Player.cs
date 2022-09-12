using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace Project_Fisticuffs
{
    //enum for player IDs
    enum Id
    {
        P1,
        P2,
        None
    }

    //enum for player states
    enum PlayerState
    {
        Idle,
        WalkForward,
        WalkBackward,
        Crouch,
        Run,
        BackDash,
        JumpSquat,
        Jump,
        Landing,
        StandAttack,
        CrouchAttack,
        JumpAttack,
        StandBlock,
        CrouchBlock,
        JumpBlock,
        StandBlockStun,
        CrouchBlockStun,
        JumpBlockStun,
        StandHitStun,
        CrouchHitStun,
        JumpHitStun,
        Tech,
        Knockdown
    }

    enum AttackState
    {
        None,
        Light,
        Heavy,
        Favor
    }


    /// <summary>
    /// Generic Player Class that all character classes are based on
    /// </summary>
    abstract class Player
    {
        //-------- Fields --------

        //animation fields
        protected int frame;
        protected int timePerFrame;
        protected int timeCounter;

        //random
        protected Random rng;

        //generic gameplay fields
        protected int gravity;
        protected int gravityModifierCount;
        protected Rectangle position;
        protected bool cornered;
        protected int hspd;
        protected int vspd;
        protected Texture2D colliderTexture;
        protected Texture2D hurtboxTexture;
        protected Texture2D hitboxTexture;
        protected Texture2D hitEffectTexture;
        protected Texture2D blockEffectTexture;
        protected List<SoundEffect> soundEffects;
        protected int attackEffectSpawnX;
        protected int attackEffectSpawnY;
        protected InputManager input;
        protected Id id;
        protected Player opponent;
        protected Solid[] bounds;
        protected PlayerState state;
        protected PlayerState prevState;
        protected AttackState atkState;
        protected AttackState prevAtkState;
        protected bool facingRight;
        protected bool forward;
        protected bool backward;
        protected bool debugDisplay;
        protected bool isHit;
        protected Hitbox hitHitbox;
        protected bool isInvincible;
        protected bool ignoreHit;
        protected bool canCancel;
        protected int comboCount;
        protected double comboScaler;
        protected bool isActionable;
        protected int hitStop;
        protected int hitStopValue;
        protected bool[] inputBufferArray;
        protected bool[] inputBufferInputs;

        //attack effect logic fields
        protected int hitRectLeft;
        protected int hitRectRight;
        protected int hitRectTop;
        protected int hitRectBottom;
        protected Rectangle hitRect;



        //This is a kind of frame wait counter for states like jumpSquat and Landing
        protected int delayCounter;
        protected int inputBufferDelayCounter;

        //these are fields for executing the dash code
        protected const int doubleTapBuffer = 30;
        protected int runInputCounter;
        protected bool startRunCounter;
        protected Inputs startDTInput;

        //Character Specific Fields
        protected int walkSpeed;
        protected int runSpeed;
        protected int backDashSpeed;
        protected int jumpStrength;
        protected int standHeight;
        protected int crouchHeight;
        protected int jumpSquat;
        protected int landingLag;
        protected Texture2D spriteSheet;
        protected int backDashDuration;
        protected Hurtbox hurtbox;
        protected int[][] attackFrameData;
        protected List<Hitbox> hitboxes;
        protected string name;
        protected double totalHealth;
        protected double health;
        protected int favor;
        protected int[] favorCosts;
        protected List<AttackEffect> attackEffects;

        //field for camera
        protected Camera camera;


        //-------- Constructor --------

        public Player(Texture2D colliderTexture, Texture2D hurtboxTexture, Texture2D hitboxTexture, Texture2D hitEffectTexture, Texture2D blockEffectTexture, List<SoundEffect> soundEffects, InputManager input, Id id, Solid[] bounds, int startingXPos, Camera camera, Random rng)
        {
            //Assigning constructor based fields
            this.colliderTexture = colliderTexture;
            this.hurtboxTexture = hurtboxTexture;
            this.hitboxTexture = hitboxTexture;
            this.hitEffectTexture = hitEffectTexture;
            this.blockEffectTexture = blockEffectTexture;
            this.soundEffects = soundEffects;
            this.input = input;
            this.id = id;

            //the solid collider this player needs to collide with
            this.bounds = bounds;

            //camera
            this.camera = camera;

            //random
            this.rng = rng;

            //the rectangle that represents the collision box of the player
            position = new Rectangle
                (
                startingXPos - (colliderTexture.Width / 2),
                bounds[2].Position.Top - colliderTexture.Height,
                colliderTexture.Width,
                colliderTexture.Height
                );

            //fields based on movement
            hspd = 0;
            vspd = 0;
            gravity = 1;
            gravityModifierCount = 0;
            cornered = false;
            hitStopValue = 8;


            //determines whether to flip the sprite
            facingRight = (id == Id.P1 ? true : false);

            //used for player FSM
            state = PlayerState.Idle;
            atkState = AttackState.None;
            isHit = false;
            isActionable = true;
            inputBufferArray = new bool[8];
            inputBufferInputs = new bool[] { input.Up, input.Down, input.Left, input.Right, input.Start, input.Light, input.Heavy, input.Favor };


            //used as a relative input in place of left and right
            forward = false;
            backward = false;

            //player hitboxes
            hitboxes = new List<Hitbox>();
            ignoreHit = false;
            hitHitbox = null;
            canCancel = false;


            //misc. counters used for various features
            delayCounter = 0;
            inputBufferDelayCounter = 0;
            runInputCounter = 0;
            startRunCounter = false;
            hitStop = 0;
            comboCount = 0;
            comboScaler = 1.0;
            favor = 50;

            //attack effect list
            attackEffects = new List<AttackEffect>();

        }

        #region //-------- Properties --------
        public Rectangle Position
        {
            get { return position; }
            set { position = value; }
        }

        public Player Opponent
        {
            get { return opponent; }
            set { opponent = value; }
        }

        public bool IsHit
        {
            get { return isHit; }
            set { isHit = value; }
        }

        public bool IgnoreHit
        {
            get { return ignoreHit; }
            set { ignoreHit = value; }
        }

        public Hitbox HitHitbox
        {
            get { return hitHitbox; }
            set { hitHitbox = value; }
        }
        public Hurtbox Hurtbox
        {
            get { return hurtbox; }
            //no set
        }

        public Camera Camera
        {
            get { return camera; }
            set { camera = value; }
        }

        public int StandHeight
        {
            get { return standHeight; }
            //no set
        }

        public double TotalHealth
        {
            get { return totalHealth; }
            //no set
        }
        
        public int X
        {
            get { return position.X + (position.Width / 2); }
            set { position.X = value - (position.Width / 2); }
        }

        public int Y
        {
            get { return position.Bottom; }
            set { position.Y = value - position.Height; }
        }

        public double Health
        {
            get { return health; }
            // no set
        }

        public int Favor
        {
            get { return favor; }
            
        }
        public Id Id
        {
            get { return id; }
            //no set
        }

        public string Name
        {
            get { return name; }
        }

        public int ComboCount
        {
            get { return comboCount; }
            //no set
        }
        public double ComboScaler
        {
            get { return comboScaler; }
        }

        public int HitStop
        { 
            get { return hitStop; } 
        }

        public List<AttackEffect> AttackEffects
        {
            get { return attackEffects; }
            //no set
        }

        public int HitRectLeft
        {
            get { return hitRectLeft; }
            set { hitRectLeft = value; }
        }

        public int HitRectRight
        {
            get { return hitRectRight; }
            set { hitRectRight = value; }
        }

        public int HitRectTop
        {
            get { return hitRectTop; }
            set { hitRectTop = value; }
        }

        public int HitRectBottom
        {
            get { return hitRectBottom; }
            set { hitRectBottom = value; }
        }

        public Rectangle HitRect
        {
            get { return hitRect; }
            set { hitRect = value; }
        }

        public int AttackEffectSpawnX
        {
            get { return attackEffectSpawnX; }
            set { attackEffectSpawnX = value; }
        }
        public int AttackEffectSpawnY
        {
            get { return attackEffectSpawnY; }
            set { attackEffectSpawnY = value; }
        }
        #endregion


        //-------- Methods --------

        public virtual void Update(GameTime GameTime)
        {
            //Queue up inputs for whe you become actionable again
            if (!isActionable)
            {
                //array of all inputs makeable by input device
                inputBufferInputs = new bool[] { input.Up, input.Down, input.Left, input.Right, input.InputPressed(Inputs.Start, false), input.InputPressed(Inputs.Light, false), input.InputPressed(Inputs.Heavy, false), input.InputPressed(Inputs.Favor, false) };

                //assign them to another array to act as an input buffer during hitstop
                for (int i = 0; i < inputBufferArray.Length; i++)
                {
                    if (!inputBufferArray[i] && inputBufferInputs[i])
                    {
                        inputBufferArray[i] = true;
                    }
                }
            }
            else
            {

                if (inputBufferDelayCounter <= 0)
                {
                    for (int i = 0; i < inputBufferArray.Length; i++)
                    {
                        inputBufferArray[i] = false;
                    }
                }
                inputBufferDelayCounter -= inputBufferDelayCounter <= 0? 0:1;

            }

            //update attack effects (before hitstop logc bcs we wanna animate the effects even when in hitstop
            foreach (AttackEffect n in attackEffects)
            {
                n.Update();
            }

            // handle hitstop
            if (hitStop > 0)
            {
                isActionable = false;
                
                //decrement hitstop
                hitStop--;

                if(hitStop <= 0)
                {

                    inputBufferDelayCounter = 1;
                }


                return;
            }
            else if (!(state == PlayerState.JumpSquat || state == PlayerState.Landing || state == PlayerState.Tech || state == PlayerState.BackDash))
            {
                isActionable = true;
            }

            

            

            //determine forward and backward directions
            forward = facingRight ? input.Right : input.Left;
            backward = facingRight ? input.Left : input.Right;

            //player player collision
            PlayerCollision(opponent);

            //horizontal collision with walls (verticle collision only happens in the air)
            WallCollision(bounds[0].Position, bounds[1].Position);
            cornered = (position.Right >= bounds[1].Position.Left || position.Left <= bounds[0].Position.Right);

            //camera edge collision
            CameraCollision(camera.Position);

            //display debug textures like collision boxes, hurtboxes, and hitboxes
            if (input.InputPressed(Inputs.Start, false) && input.Down && input.Favor)
            {
                debugDisplay = !debugDisplay;
            }



            //determine if inputs are double-tapped
            bool forwardDash = facingRight ? DoubleTap(Inputs.Right) : DoubleTap(Inputs.Left);
            bool backwardDash = facingRight ? DoubleTap(Inputs.Left) : DoubleTap(Inputs.Right);

            //Player FSM
            switch (state)
            {
                case PlayerState.Idle:
                    //Which way is the player facing
                    facingRight = position.X < opponent.Position.X ? true : false;

                    //set speeds
                    hspd = 0;
                    vspd = 0;


                    //transition to other states based on input+
                    if (input.InputPressed(Inputs.Light, inputBufferArray[5]))
                    {

                        SetState(!input.Down ? PlayerState.StandAttack : PlayerState.CrouchAttack);
                        SetAttackState(AttackState.Light);
                    }
                    else if (input.InputPressed(Inputs.Heavy, inputBufferArray[6]))
                    {
                        SetState(!input.Down ? PlayerState.StandAttack : PlayerState.CrouchAttack);
                        SetAttackState(AttackState.Heavy);
                    }
                    if (input.InputPressed(Inputs.Favor, inputBufferArray[7]))
                    {

                        SetState(/*!input.Down ? */PlayerState.StandAttack/* : PlayerState.CrouchAttack*/);
                        SetAttackState(AttackState.Favor);
                    }
                    else if (forward)
                    {
                        SetState(PlayerState.WalkForward);
                    }
                    else if (backward)
                    {
                        SetState(PlayerState.WalkBackward);
                    }
                    else if (input.Up)
                    {
                        SetState(PlayerState.JumpSquat);
                    }
                    else if (input.Down)
                    {
                        SetState(PlayerState.Crouch);
                    }


                    if (forwardDash)
                    {
                        SetState(PlayerState.Run);
                    }
                    if (backwardDash)
                    {
                        delayCounter = 0;
                        SetState(PlayerState.BackDash);
                    }
                    //if hit, go into appropriate hitstun
                    CheckHitStanding();
                    break;

                case PlayerState.WalkForward:

                    //set speeds
                    vspd = 0;

                    //transition to other states based on input
                    if (forward)
                    {
                        hspd = walkSpeed * (facingRight ? 1 : -1);
                    }
                    else
                    {
                        SetState(PlayerState.Idle);
                    }
                    if (input.InputPressed(Inputs.Light, inputBufferArray[5]))
                    {
                        SetState(!input.Down ? PlayerState.StandAttack : PlayerState.CrouchAttack);
                        SetAttackState(AttackState.Light);
                    }
                    else if (input.InputPressed(Inputs.Heavy, inputBufferArray[6]))
                    {
                        SetState(!input.Down ? PlayerState.StandAttack : PlayerState.CrouchAttack);
                        SetAttackState(AttackState.Heavy);
                    }
                    else if (input.InputPressed(Inputs.Favor, inputBufferArray[7]))
                    {

                        SetState(!input.Down ? PlayerState.StandAttack : PlayerState.CrouchAttack);
                        SetAttackState(AttackState.Favor);
                    }
                    else if (input.Up)
                    {
                        SetState(PlayerState.JumpSquat);
                    }
                    else if (input.Down)
                    {
                        SetState(PlayerState.Crouch);

                    }


                    //if hit, go into appropriate hitstun
                    CheckHitStanding();
                    break;

                case PlayerState.WalkBackward:


                    //transition to other states based on input
                    if (backward)
                    {
                        hspd = walkSpeed * (facingRight ? -1 : 1);
                        if (opponent.atkState != AttackState.None || opponent.hitboxes.Count > 0)
                        {
                            SetState(PlayerState.StandBlock);
                            hspd = 0;
                        }
                    }
                    else
                    {
                        SetState(PlayerState.Idle);
                    }
                    if (input.InputPressed(Inputs.Light, inputBufferArray[5]))
                    {
                        SetState(!input.Down ? PlayerState.StandAttack : PlayerState.CrouchAttack);
                        SetAttackState(AttackState.Light);
                    }
                    else if (input.InputPressed(Inputs.Heavy, inputBufferArray[6]))
                    {
                        SetState(!input.Down ? PlayerState.StandAttack : PlayerState.CrouchAttack);
                        SetAttackState(AttackState.Heavy);
                    }
                    else if (input.InputPressed(Inputs.Favor, inputBufferArray[7]))
                    {

                        SetState(!input.Down ? PlayerState.StandAttack : PlayerState.CrouchAttack);
                        SetAttackState(AttackState.Favor);
                    }
                    if (input.Up)
                    {
                        SetState(PlayerState.JumpSquat);
                    }
                    else if (input.Down)
                    {
                        SetState(PlayerState.Crouch);
                    }

                    //if hit, go into appropriate hitstun
                    CheckHitStanding();
                    break;

                case PlayerState.Run:

                    if (forward)
                    {
                        hspd = runSpeed * (facingRight ? 1 : -1);
                    }
                    else
                    {
                        SetState(PlayerState.Idle);
                    }
                    if (input.InputPressed(Inputs.Light, inputBufferArray[5]))
                    {
                        SetState(!input.Down ? PlayerState.StandAttack : PlayerState.CrouchAttack);
                        SetAttackState(AttackState.Light);
                    }
                    else if (input.InputPressed(Inputs.Heavy, inputBufferArray[6]))
                    {
                        SetState(!input.Down ? PlayerState.StandAttack : PlayerState.CrouchAttack);
                        SetAttackState(AttackState.Heavy);
                    }
                    else if (input.InputPressed(Inputs.Favor, inputBufferArray[7]))
                    {

                        SetState(!input.Down ? PlayerState.StandAttack : PlayerState.CrouchAttack);
                        SetAttackState(AttackState.Favor);
                    }
                    else if (input.Up)
                    {
                        SetState(PlayerState.JumpSquat);
                    }
                    else if (input.Down)
                    {
                        SetState(PlayerState.Crouch);
                    }
                    //if hit, go into appropriate hitstun
                    CheckHitStanding();
                    break;

                case PlayerState.BackDash:

                    //if hit, go into appropriate hitstun
                    CheckHitStanding();

                    //Set speeds
                    hspd = backDashSpeed * (facingRight ? -1 : 1);

                    delayCounter++;
                    if (delayCounter > backDashDuration)
                    {
                        delayCounter = 0;


                        SetState(PlayerState.Idle);
                    }
                    break;

                case PlayerState.Crouch:
                    //Which way is the player facing
                    facingRight = position.X < opponent.Position.X ? true : false;
                    hspd = 0;

                    //if hit, go into appropriate hitstun
                    CheckHitCrouching();

                    //transition to other states based on input
                    if (backward && (opponent.atkState != AttackState.None || opponent.hitboxes.Count > 0))
                    {
                        SetState(PlayerState.CrouchBlock);
                    }
                    if (input.InputPressed(Inputs.Light, inputBufferArray[5]))
                    {
                        SetState(!input.Down ? PlayerState.StandAttack : PlayerState.CrouchAttack);
                        SetAttackState(AttackState.Light);
                    }
                    if (input.InputPressed(Inputs.Heavy, inputBufferArray[6]))
                    {
                        SetState(PlayerState.CrouchAttack);
                        SetAttackState(AttackState.Heavy);
                    }
                    if (!input.Down)
                    {
                        SetState(PlayerState.Idle);
                    }
                    break;

                case PlayerState.JumpSquat:

                    //Set speeds
                    hspd = 0;
                    vspd = 0;

                    //if hit, go into appropriate hitstun
                    CheckHitStanding();

                    delayCounter++;
                    if (delayCounter > jumpSquat)
                    {
                        vspd -= jumpStrength;
                        delayCounter = 0;

                        switch (prevState)
                        {
                            case PlayerState.WalkForward:
                                hspd = walkSpeed * (facingRight ? 1 : -1);
                                break;
                            case PlayerState.WalkBackward:
                                hspd = walkSpeed * (facingRight ? -1 : 1);
                                break;
                            case PlayerState.Run:
                                hspd = runSpeed * (facingRight ? 1 : -1);
                                break;
                        }
                        SetState(PlayerState.Jump);
                    }
                    break;

                case PlayerState.Jump:

                    //blocking
                    if (backward && (opponent.atkState != AttackState.None || opponent.hitboxes.Count > 0))
                    {
                        SetState(PlayerState.JumpBlock);
                    }

                    if (input.InputPressed(Inputs.Light, inputBufferArray[5]))
                    {
                        SetState(PlayerState.JumpAttack);
                        SetAttackState(AttackState.Light);
                    }
                    if (input.InputPressed(Inputs.Heavy, inputBufferArray[6]))
                    {
                        SetState(PlayerState.JumpAttack);
                        SetAttackState(AttackState.Heavy);
                    }
                    GroundCollision(bounds[2].Position);

                    CheckHitJumping();
                    break;

                case PlayerState.Landing:

                    ClearAllHitboxes();
                    //Set speeds
                    hspd = 0;
                    vspd = 0;

                    CheckHitStanding();

                    delayCounter++;
                    if (delayCounter > landingLag)
                    {
                        SetState(PlayerState.Idle);
                        delayCounter = 0;
                    }
                    break;

                case PlayerState.StandBlock:
                    //Which way is the player facing
                    facingRight = position.X < opponent.Position.X ? true : false;

                    CheckHitStanding();
                    if (!backward || opponent.atkState == AttackState.None)
                    {
                        SetState(input.Down ? PlayerState.Crouch : PlayerState.Idle);
                    }
                    if (input.InputPressed(Inputs.Light, inputBufferArray[5]))
                    {
                        SetState(!input.Down ? PlayerState.StandAttack : PlayerState.CrouchAttack);
                        SetAttackState(AttackState.Light);
                    }
                    else if (input.InputPressed(Inputs.Heavy, inputBufferArray[6]))
                    {
                        SetState(!input.Down ? PlayerState.StandAttack : PlayerState.CrouchAttack);
                        SetAttackState(AttackState.Heavy);
                    }
                    else if (input.InputPressed(Inputs.Favor, inputBufferArray[7]))
                    {

                        SetState(!input.Down ? PlayerState.StandAttack : PlayerState.CrouchAttack);
                        SetAttackState(AttackState.Favor);
                    }
                    if (input.Up)
                    {
                        SetState(PlayerState.JumpSquat);
                    }
                    else if (input.Down)
                    {
                        SetState(backward?PlayerState.CrouchBlock : PlayerState.Crouch);
                    }
                    break;

                case PlayerState.CrouchBlock:
                    //Which way is the player facing
                    facingRight = position.X < opponent.Position.X ? true : false;

                    CheckHitCrouching();
                    if (!backward || opponent.atkState == AttackState.None)
                    {
                        SetState(input.Down ? PlayerState.Crouch : PlayerState.Idle);
                    }

                    //inputs outside of crouch preblock
                    if (input.InputPressed(Inputs.Light, inputBufferArray[5]))
                    {
                        SetState(!input.Down ? PlayerState.StandAttack : PlayerState.CrouchAttack);
                        SetAttackState(AttackState.Light);
                    }
                    if (input.InputPressed(Inputs.Heavy, inputBufferArray[6]))
                    {
                        SetState(PlayerState.CrouchAttack);
                        SetAttackState(AttackState.Heavy);
                    }
                    if (!input.Down)
                    {
                        SetState(backward ? PlayerState.StandBlock : PlayerState.Idle);
                    }
                    break;

                case PlayerState.JumpBlock:
                    CheckHitJumping();
                    if (!backward || opponent.atkState == AttackState.None)
                    {
                        SetState(PlayerState.Jump);
                    }
                    GroundCollision(bounds[2].Position);
                    break;

                case PlayerState.StandBlockStun:
                    //Which way is the player facing
                    facingRight = position.X < opponent.Position.X ? true : false;

                    ClearAllHitboxes();
                    delayCounter++;
                    if(cornered)
                    {
                        if (opponent.hspd > 0)
                        {
                            opponent.hspd = delayCounter % 2 == 0 ? opponent.hspd - 1 : opponent.hspd;
                        }
                        else if (opponent.hspd < 0)
                        {
                            opponent.hspd = delayCounter % 2 == 0 ? opponent.hspd + 1 : opponent.hspd;
                        }
                    }
                    else
                    {
                        if (hspd > 0)
                        {
                            hspd = delayCounter % 2 == 0 ? hspd - 1 : hspd;
                        }
                        else if (hspd < 0)
                        {
                            hspd = delayCounter % 2 == 0 ? hspd + 1 : hspd;
                        }
                    }
                    

                    if (delayCounter > hitHitbox.BlockStun)
                    {
                        SetState(PlayerState.StandBlock);
                        delayCounter = 0;
                    }

                    CheckHitStanding();
                    break;
                case PlayerState.CrouchBlockStun:
                    //Which way is the player facing
                    facingRight = position.X < opponent.Position.X ? true : false;

                    ClearAllHitboxes();
                    delayCounter++;
                    if (cornered)
                    {
                        if (opponent.hspd > 0)
                        {
                            opponent.hspd --;
                        }
                        else if (opponent.hspd < 0)
                        {
                            opponent.hspd ++;
                        }
                    }
                    else
                    {
                        if (hspd > 0)
                        {
                            hspd --;
                        }
                        else if (hspd < 0)
                        {
                            hspd ++;
                        }
                    }
                    if (delayCounter > hitHitbox.BlockStun + 1)
                    {
                        SetState(PlayerState.CrouchBlock);
                        delayCounter = 0;
                    }
                    CheckHitCrouching();
                    break;
                case PlayerState.JumpBlockStun:
                    ClearAllHitboxes();
                    delayCounter++;
                    if (cornered)
                    {
                        if (opponent.hspd > 0)
                        {
                            opponent.hspd--;
                        }
                        else if (opponent.hspd < 0)
                        {
                            opponent.hspd++;
                        }
                    }
                    else
                    {
                        if (hspd > 0)
                        {
                            hspd--;
                        }
                        else if (hspd < 0)
                        {
                            hspd++;
                        }
                    }
                    if (delayCounter > hitHitbox.BlockStun)
                    {
                        SetState(PlayerState.JumpBlock);
                        delayCounter = 0;
                    }
                    CheckHitJumping();
                    GroundCollision(bounds[2].Position);
                    break;

                case PlayerState.StandAttack:
                    UpdateAttackState(GameTime);
                    CheckHitStanding();
                    break;
                case PlayerState.CrouchAttack:
                    UpdateAttackState(GameTime);
                    CheckHitCrouching();
                    break;

                case PlayerState.JumpAttack:
                    UpdateAttackState(GameTime);
                    GroundCollision(bounds[2].Position);

                    CheckHitJumping();
                    break;

                case PlayerState.StandHitStun:
                    //Which way is the player facing
                    facingRight = position.X < opponent.Position.X ? true : false;

                    ClearAllHitboxes();
                    delayCounter++;
                    if (cornered)
                    {
                        if (opponent.hspd > 0)
                        {
                            opponent.hspd = delayCounter % 2 == 0 ? opponent.hspd - 1 : opponent.hspd;
                        }
                        else if (opponent.hspd < 0)
                        {
                            opponent.hspd = delayCounter % 2 == 0 ? opponent.hspd + 1 : opponent.hspd;
                        }
                    }
                    else
                    {
                        if (hspd > 0)
                        {
                            hspd = delayCounter % 2 == 0 ? hspd - 1 : hspd;
                        }
                        else if (hspd < 0)
                        {
                            hspd = delayCounter % 2 == 0 ? hspd + 1 : hspd;
                        }
                    }
                    if (delayCounter > hitHitbox.Hitstun)
                    {
                        SetState(PlayerState.Idle);
                        delayCounter = 0;
                    }

                    CheckHitStanding();
                    break;
                case PlayerState.CrouchHitStun:
                    //Which way is the player facing
                    facingRight = position.X < opponent.Position.X ? true : false;

                    ClearAllHitboxes();
                    delayCounter++;
                    if (cornered)
                    {
                        if (opponent.hspd > 0)
                        {
                            opponent.hspd = delayCounter % 2 == 0 ? opponent.hspd - 1 : opponent.hspd;
                        }
                        else if (opponent.hspd < 0)
                        {
                            opponent.hspd = delayCounter % 2 == 0 ? opponent.hspd + 1 : opponent.hspd;
                        }
                    }
                    else
                    {
                        if (hspd > 0)
                        {
                            hspd = delayCounter % 2 == 0 ? hspd - 1 : hspd;
                        }
                        else if (hspd < 0)
                        {
                            hspd = delayCounter % 2 == 0 ? hspd + 1 : hspd;
                        }
                    }

                    if (delayCounter > hitHitbox.Hitstun + 1)
                    {
                        SetState(PlayerState.Crouch);
                        delayCounter = 0;
                    }
                    CheckHitCrouching();
                    break;
                case PlayerState.JumpHitStun:
                    ClearAllHitboxes();
                    if (cornered)
                    {
                        if (opponent.hspd > 0)
                        {
                            opponent.hspd--;
                        }
                        else if (opponent.hspd < 0)
                        {
                            opponent.hspd++;
                        }
                    }
                    CheckHitJumping();
                    GroundCollision(bounds[2].Position);
                    break;
                case PlayerState.Tech:
                    isInvincible = true;
                    GroundCollision(bounds[2].Position);
                    break;
            }

            


            X += hspd;
            Y += vspd;
            UpdateAnimation(GameTime);
            //Update the player hurtbox
            UpdateHitAndHurtbox(GameTime);

            for (int i = 0; i < attackEffects.Count; i++)
            {
                if (attackEffects[i].EffectFinished)
                {
                    attackEffects.RemoveAt(i);
                    i--;
                }
            }

        }

        /// <summary>
        /// Draw function for generic Player
        /// </summary>
        /// <param name="sb"></param>
        public virtual void Draw(SpriteBatch sb)
        {
            if (debugDisplay)
            {
                //only draw this if you want to see the collision box
                sb.Draw(colliderTexture, position, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                hurtbox.Draw(sb);
                foreach (Hitbox n in hitboxes)
                {
                    n.Draw(sb);
                }
            }

        }

        #region // ------------------------------------------------------------ Collision -----------------------------------------------------------------
        /// <summary>
        /// Check For Collision With the Ground
        /// </summary>
        /// <param name="ground"></param>
        public void GroundCollision(Rectangle ground)
        {
            //apply gravity to the player always

            if (gravityModifierCount >= 2)
            {
                vspd += gravity;
                gravityModifierCount = 0;
            }
            gravityModifierCount++;

            //Check for ground
            if (Y + vspd >= ground.Top)
            {
                Y = ground.Top;
                vspd = 0;
                delayCounter = 0;

                //go into tech if coming out of a combo
                if(state == PlayerState.JumpHitStun)
                {
                    vspd = -4;
                    hspd = facingRight ? -3 : 3;
                    SetState(PlayerState.Tech);
                }
                else if(state == PlayerState.Tech)
                {
                    isInvincible = false;
                    SetState(backward ? (!input.Down? PlayerState.StandBlock : PlayerState.CrouchBlock) : (!input.Down ? PlayerState.Idle : PlayerState.Crouch));
                }
                else
                {
                    
                    SetState(PlayerState.Landing);
                }
            }

            

        }

        /// <summary>
        /// check for collision with the walls
        /// </summary>
        /// <param name="leftWall"></param>
        /// <param name="rightWall"></param> 
        public void WallCollision(Rectangle leftWall, Rectangle rightWall)
        {
            if (position.Left - hspd <= leftWall.Right && hspd <= 0)
            {
                hspd = 0;
                X = leftWall.Right + position.Width / 2;

            }

            if (position.Right + hspd > rightWall.Left)
            {
                hspd = 0;
                X = rightWall.Left - position.Width / 2;
            }

        }

        /// <summary>
        /// check for collision with the opponent
        /// </summary>
        /// <param name="opponentCollider"></param>
        public void PlayerCollision(Player opponent)
        {

            //TODO: Finish Fixing this. Im not done and coming back to this later
            if (position.Intersects(opponent.Position))
            {
                if(cornered && Y > opponent.Y)
                {
                    X = X <= opponent.X ? X + 1 : X - 1;
                }
                /*if (facingRight)
                {
                    X = X < opponent.X ? X - (state != PlayerState.Run ? walkSpeed : runSpeed) : X + (state != PlayerState.Run ? walkSpeed : runSpeed);
                }
                else
                {
                    X = X >= opponent.X ? X + (state != PlayerState.Run ? walkSpeed : runSpeed) : X - (state != PlayerState.Run ? walkSpeed : runSpeed);
                }*/
                X = X <= opponent.X ? X - (state != PlayerState.Run ? walkSpeed : runSpeed) : X + (state != PlayerState.Run ? walkSpeed : runSpeed);

            }

        }

        /// <summary>
        /// check for collision with the camera edges
        /// </summary>
        /// <param name="camera"></param>
        public void CameraCollision(Rectangle camera)
        {
            if (((X - position.Width / 2) + hspd) < camera.Left + position.Width / 2)
            {
                hspd = 0;
                X = camera.Left + position.Width;
            }

            if (((X + position.Width / 2) + hspd) > camera.Right - position.Width / 2)
            {
                hspd = 0;
                X = camera.Right - position.Width;
            }
        }

        /// <summary>
        /// checks if the player's hurtbox came into contact with an opponents hitbox
        /// </summary>
        /// <param name="hitbox"></param>
        /// <returns></returns>
        public void HitboxCollision(Hitbox hitbox)
        {
            if (hitbox.Position.Intersects(opponent.Hurtbox.Position) && !opponent.ignoreHit && !opponent.isInvincible)
            {
                opponent.IsHit = true;
                opponent.HitHitbox = hitbox;
                opponent.ignoreHit = true;
                canCancel = true;

                //get the centerpoint of the hitbox collision (used for spawn location of the hit effect)
                opponent.HitRectLeft = hitbox.Position.Left > opponent.Hurtbox.Position.Left? hitbox.Position.Left: opponent.Hurtbox.Position.Left;
                opponent.HitRectRight = hitbox.Position.Right > opponent.Hurtbox.Position.Right ? opponent.Hurtbox.Position.Right : hitbox.Position.Right;
                opponent.HitRectTop = hitbox.Position.Top > opponent.Hurtbox.Position.Top ? hitbox.Position.Top : opponent.Hurtbox.Position.Top;
                opponent.HitRectBottom = hitbox.Position.Bottom > opponent.Hurtbox.Position.Bottom ? opponent.Hurtbox.Position.Bottom : hitbox.Position.Bottom;

                opponent.HitRect = new Rectangle(opponent.HitRectLeft, opponent.HitRectTop, opponent.HitRectRight - opponent.HitRectLeft, opponent.HitRectBottom - opponent.HitRectTop);
                opponent.AttackEffectSpawnX = opponent.HitRect.Center.X;
                opponent.AttackEffectSpawnY = opponent.HitRect.Center.Y;

            }
            else
            {
                opponent.IsHit = false;
            }



        }
        #endregion

        /// <summary>
        /// Set the state and previous state of the player
        /// </summary>
        /// <param name="targetState"></param>
        public void SetState(PlayerState targetState)
        {
            timeCounter = 0;
            frame = 0;
            prevState = state;
            state = targetState;
            if (targetState == PlayerState.Crouch || targetState == PlayerState.CrouchAttack || targetState == PlayerState.CrouchBlock || targetState == PlayerState.CrouchHitStun || targetState == PlayerState.CrouchBlockStun)
            {
                position.Height = crouchHeight;
            }
            else
            {
                position.Height = standHeight;
            }
            if (targetState != PlayerState.Jump && targetState != PlayerState.JumpAttack && targetState != PlayerState.JumpBlock && targetState != PlayerState.JumpHitStun && targetState != PlayerState.JumpBlockStun)
            {
                Y = bounds[2].Position.Top;
            }
            if (state != PlayerState.StandAttack && state != PlayerState.CrouchAttack && state != PlayerState.JumpAttack)
            {
                SetAttackState(AttackState.None);
            }
            if(targetState != PlayerState.StandHitStun && targetState != PlayerState.CrouchHitStun && targetState != PlayerState.JumpHitStun)
            {
                comboCount = 0;
                comboScaler = 1;
            }
            if(targetState == PlayerState.StandHitStun || targetState == PlayerState.CrouchHitStun || targetState == PlayerState.JumpHitStun || targetState == PlayerState.StandBlockStun || targetState == PlayerState.CrouchBlockStun || targetState == PlayerState.JumpBlockStun)
            {
                hitStop = hitStopValue;
                opponent.hitStop = hitStopValue;
                //your opponent gets 1 free unscaled hit
                if(comboCount >= 2)
                {
                    comboScaler -= .1;
                }
            }

            if (targetState == PlayerState.JumpSquat || targetState == PlayerState.Landing || targetState == PlayerState.Tech || targetState == PlayerState.BackDash)
            {
                isActionable = false;
            }
            else
            {
                isActionable = true;
                inputBufferDelayCounter = 1;
            }

        }

        /// <summary>
        /// Set the state and previous state of the attackState;
        /// </summary>
        /// <param name="targetState"></param>
        public void SetAttackState(AttackState targetState)
        {
            //if the move is a Favor, see if we want to proceed
            if (targetState == AttackState.Favor)
            {
                switch (state)
                {
                    case PlayerState.StandAttack:
                        if (!SpendFavor(favorCosts[0]))
                        {
                            timeCounter = 0;
                            frame = 0;
                            prevState = state;
                            state = PlayerState.Idle;
                            return;
                        }
                        break;
                    case PlayerState.CrouchAttack:
                        if (!SpendFavor(favorCosts[1]))
                        {
                            timeCounter = 0;
                            frame = 0;
                            prevState = state;
                            state = PlayerState.Crouch;
                            return;
                        }
                        break;
                    case PlayerState.JumpAttack:
                        if (!SpendFavor(favorCosts[2]))
                        {
                            timeCounter = 0;
                            frame = 0;
                            prevState = state;
                            state = PlayerState.Jump;
                            return;
                        }
                        break;
                }
            }
            canCancel = false;
            prevAtkState = atkState;
            atkState = targetState;
            if((state == PlayerState.StandAttack || state == PlayerState.CrouchAttack) && (prevState != PlayerState.StandAttack && prevState != PlayerState.CrouchAttack))
            {
                hspd = 0;
                vspd = 0;
            }
            
            
            
        }



        /// <summary>
        /// update the different aspects of the players animation based on their state
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void UpdateAnimation(GameTime gameTime);

        /// <summary>
        /// checks to see if player can use favor move, and if so, pays for it.
        /// </summary>
        /// <param name="favorCost"></param>
        public bool SpendFavor(int favorCost)
        {
            if (favor >= favorCost)
            {
                favor -= favorCost;
                opponent.favor += favorCost;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check whether a target input is double tapped
        /// </summary>
        /// <param name="targetInput"></param>
        /// <returns></returns>
        public bool DoubleTap(Inputs targetInput)
        {

            bool tapInput = input.InputPressed(targetInput, false);
            if (tapInput && !startRunCounter)
            {
                startDTInput = targetInput;
                startRunCounter = true;
                tapInput = false;
            }

            if (startRunCounter)
            {
                runInputCounter++;
            }

            if (tapInput && runInputCounter < doubleTapBuffer && runInputCounter > 0 && targetInput == startDTInput)
            {
                startRunCounter = false;
                runInputCounter = 0;

                //should be null but enums are non nullable, so this shouldn't break anything
                startDTInput = Inputs.Start;

                return true;
            }

            if (runInputCounter >= doubleTapBuffer)
            {
                startRunCounter = false;
                runInputCounter = 0;
            }
            return false;
        }

        /// <summary>
        /// update the hurtbox according to the player's state (character dependant)
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void UpdateHitAndHurtbox(GameTime gameTime);

        /// <summary>
        /// update the character specific actions that take place in the various attack states
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void UpdateAttackState(GameTime gameTime);

        

        /// <summary>
        /// helper method that returns the proper number for the xOffset based on whether the player is facing left or right
        /// </summary>
        /// <param name="baseXOffset"></param>
        /// <param name="hurtBoxWidth"></param>
        /// <returns></returns>
        public int GetHurtboxXOffset(int baseXOffset, int hurtBoxWidth)
        {
            return facingRight ? baseXOffset : -(hurtBoxWidth - (position.Width - baseXOffset));
        }

        //helper methods to properly transition a player into the correct hitstun state
        public void CheckHitStanding()
        {
            if (isHit)
            {
                hspd = 0;
                vspd = 0;
                if ((state == PlayerState.StandBlock && (hitHitbox.BlockType == BlockType.Mid || hitHitbox.BlockType == BlockType.High)) || state == PlayerState.StandBlockStun)
                {
                    SetState(PlayerState.StandBlockStun);
                    if (cornered)
                    {
                        opponent.hspd = opponent.facingRight ? -hitHitbox.BlockXPushback : hitHitbox.BlockXPushback;
                    }
                    else
                    {
                        hspd = facingRight ? -hitHitbox.BlockXPushback : hitHitbox.BlockXPushback;
                    }
                    attackEffects.Add(new AttackEffect(false, blockEffectTexture, attackEffectSpawnX, attackEffectSpawnY, facingRight));
                    PlayBlockSound();
                    
                    
                }
                else
                {
                    if (hitHitbox.Launcher)
                    {
                        SetState(PlayerState.JumpHitStun);
                        hspd = facingRight ? -hitHitbox.XKnockback : hitHitbox.XKnockback;
                        vspd = (int)(-hitHitbox.Yknockback * comboScaler);
                    }
                    else
                    {
                        SetState(PlayerState.StandHitStun);
                        if (cornered)
                        {
                            opponent.hspd = opponent.facingRight ? -hitHitbox.XKnockback : hitHitbox.XKnockback;
                        }
                        else
                        {
                            hspd = facingRight ? -hitHitbox.XKnockback : hitHitbox.XKnockback;
                        }
                    }
                    health -= (hitHitbox.Damage * comboScaler);
                    comboCount++;
                    PlayHitSound();
                    attackEffects.Add(new AttackEffect(true, hitEffectTexture, attackEffectSpawnX, attackEffectSpawnY, facingRight));
                }
                delayCounter = 0;
                isHit = false;


            }
        }
        public void CheckHitCrouching()
        {
            if (isHit)
            {
                //set speeds initially
                hspd = 0;
                vspd = 0;
                //check if you are blocking when you get hit
                if ((state == PlayerState.CrouchBlock && (hitHitbox.BlockType == BlockType.Mid || hitHitbox.BlockType == BlockType.Low)) || state == PlayerState.CrouchBlockStun)
                {
                    //if you are cornered, the opponent moves back, otherwise, you move back based on the hit hitbox knockback values
                    SetState(PlayerState.CrouchBlockStun);
                    if (cornered)
                    {
                        opponent.hspd = opponent.facingRight ? -hitHitbox.BlockXPushback : hitHitbox.BlockXPushback;
                    }
                    else
                    {
                        hspd = facingRight ? -hitHitbox.BlockXPushback : hitHitbox.BlockXPushback;
                    }
                    
                    attackEffects.Add(new AttackEffect(false, blockEffectTexture, attackEffectSpawnX, attackEffectSpawnY, facingRight));
                    PlayBlockSound();


                }
                //otherwise you are hit
                else
                {
                    if (hitHitbox.Launcher)
                    {
                        SetState(PlayerState.JumpHitStun);
                        //if you are cornered, the opponent moves back, otherwise, you move back based on the hit hitbox knockback values
                        if (cornered)
                        {
                            opponent.hspd = opponent.facingRight ? -hitHitbox.XKnockback : hitHitbox.XKnockback;
                        }
                        else
                        {
                            hspd = facingRight ? -hitHitbox.XKnockback : hitHitbox.XKnockback;
                        }
                        
                        vspd = (int)(-hitHitbox.Yknockback * comboScaler);
                        Y = bounds[2].Position.Top;
                    }
                    else
                    {
                        SetState(PlayerState.CrouchHitStun);
                        //if you are cornered, the opponent moves back, otherwise, you move back based on the hit hitbox knockback values
                        if (cornered)
                        {
                            opponent.hspd = opponent.facingRight ? -hitHitbox.XKnockback : hitHitbox.XKnockback;
                        }
                        else
                        {
                            hspd = facingRight ? -hitHitbox.XKnockback : hitHitbox.XKnockback;
                        }
                        
                    }
                    health -= hitHitbox.Damage;
                    PlayHitSound();
                    comboCount++;
                    attackEffects.Add(new AttackEffect(true, hitEffectTexture, attackEffectSpawnX, attackEffectSpawnY, facingRight));

                }
                delayCounter = 0;
                isHit = false;
            }
        }
        public void CheckHitJumping()
        {
            if (isHit)
            {
                hspd = 0;
                vspd = 0;
                if ((state == PlayerState.JumpBlock && (hitHitbox.BlockType != BlockType.Unblockable)) || state == PlayerState.JumpBlockStun)
                {
                    SetState(PlayerState.JumpBlockStun);
                    if (cornered)
                    {
                        opponent.hspd = opponent.facingRight ? -hitHitbox.BlockXPushback : hitHitbox.BlockXPushback;
                    }
                    else
                    {
                        hspd = facingRight ? -hitHitbox.BlockXPushback : hitHitbox.BlockXPushback;
                    }
                    vspd = -1;
                    attackEffects.Add(new AttackEffect(false, blockEffectTexture, attackEffectSpawnX, attackEffectSpawnY, facingRight));
                    PlayBlockSound();

                }
                else
                {
                    SetState(PlayerState.JumpHitStun);
                    if (cornered)
                    {
                        opponent.hspd = opponent.facingRight ? -hitHitbox.XKnockback : hitHitbox.XKnockback;
                    }
                    else
                    {
                        hspd = facingRight ? -hitHitbox.XKnockback : hitHitbox.XKnockback;
                    }
                    vspd = (int)(-hitHitbox.Yknockback * comboScaler);
                    health -= (hitHitbox.Damage * comboScaler);
                    PlayHitSound();
                    comboCount++;
                    attackEffects.Add(new AttackEffect(true, hitEffectTexture, attackEffectSpawnX, attackEffectSpawnY, facingRight));
                }
                delayCounter = 0;
                isHit = false;
            }
        }


        public void ClearAllHitboxes()
        {
            for (int i = 0; i < hitboxes.Count; i++)
            {
                if (!hitboxes[i].IsProjectile)
                {
                    hitboxes.RemoveAt(i);
                    i--;
                }
            }
            opponent.ignoreHit = false;

        }

        /// <summary>
        /// Load character stats from file, starting at the line that says their name and stopping when a ### is reached.
        /// </summary>
        public int[] ReadStats(string name)
        {
            // initialize variables
            string path = "../../../statManager.txt";
            StreamReader input = null;
            int numOfStats = 0;
            int[] frameData = null;

            try
            {
                string line = null;
                input = new StreamReader(path);

                //find the character's name in the file
                while ((line = input.ReadLine()) != null && line != name) { }

                //figure out how big the array needs to be by getting the size of the character's data, then make array
                while ((line = input.ReadLine()) != null && line != "###")
                {
                    numOfStats += 1;
                }
                frameData = new int[numOfStats];

                //Close and reopen file so we can get back to the beginning of the character's data (I can't find a more efficient way to rewind a StreamReader, which seems really strange to me)
                input.Close();
                input = new StreamReader(path);
                while ((line = input.ReadLine()) != null && line != name) { }

                //read through pertinent data and build an array for character stats to read from
                for (int i = 0; i < numOfStats; i++)
                {
                    line = input.ReadLine();
                    string[] statistics = line.Split(',');
                    frameData[i] = int.Parse(statistics[1]);
                }

            }
            catch (Exception)
            {
                //TODO: throw visible exception (FAIL LOUDLY)
            }

            //close data file if it's open
            if (input != null)
            {
                input.Close();
            }
            return frameData;
        }

        /// <summary>
        /// sounds to play on hit
        /// </summary>
        protected void PlayHitSound()
        {
            soundEffects[rng.Next(1, 3)].Play();
        }

        /// <summary>
        /// sounds to play on block
        /// </summary>
        protected void PlayBlockSound()
        {
            soundEffects[0].Play();
        }

        /// <summary>
        /// updates character to match ReadStats
        /// </summary>
        /// <param name="stats"></param>
        public abstract void StatUpdate();
    }
}
