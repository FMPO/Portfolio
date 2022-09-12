using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;

namespace Project_Fisticuffs
{
    //all the gamestates one can be in
    // TODO: Have to add rules state to explain controls/gameplay to player
    enum GameStates
    {
        MainMenu,
        CharacterSelect,
        Settings,
        GamePlay,
        HowToPlay,
        Pause,
        Results,
        Quit
    }



    public class Game1 : Game
    {
        //random object
        private Random rng;

        //graphics manager fields
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private int windowWidth;
        private int windowHeight;

        //sound effects
        List<SoundEffect> soundEffects;
        List<SoundEffect> playerSounds;

        //music
        protected Song musicTest;
        protected Song patriotism;

        //Font
        private SpriteFont debugFont;

        //Generic Gameplay system Fields
        private GameStates currentState = GameStates.MainMenu;
        private KeyboardState kbState;
        private KeyboardState prevKbState;
        private Solid[] solids;
        private GameStates[] menuItems;
        private string[] menuItemStrings;
        private int menuIndex;

        // Character Selection
        private Texture2D[] characterMenu;
        private int character1Index;
        private int character2Index;
        private bool p1Confirm;
        private bool p2Confirm;
        private Color textColor;
        private Color textSelectedColor;
        private Texture2D kairosName;
        private Texture2D kairosTooName;
        private Texture2D p1Selector;
        private Texture2D p2Selector;
        private Texture2D[] characterPortraits;
        private Texture2D kairosPortrait;
        private Texture2D kairosTooPortrait;
        private int p1Y;
        private int p2Y;

        //Player Gameplay Fields
        private InputManager p1Input;
        private InputManager p2Input;
        private Player p1;
        private Player p2;
        private Id winner;
        private Player[] p1Characters;
        private Player[] p2Characters;

        //Player Rendering Fields
        private Texture2D playerCollisionTexture;
        private Texture2D hitboxTexture;
        private Texture2D hurtboxTexture;
        private Texture2D kairosSpriteSheet;
        private Texture2D kairosTooSpriteSheet;
        private Texture2D hitEffectTexture;
        private Texture2D blockEffectTexture;

        //Generic Gameplay Rendering Fields
        private Texture2D solidTexture;
        private Texture2D backgroundTexture;

        //Background Screens
        private Texture2D optionsScreen;
        private Texture2D resultsScreen;
        private Texture2D rulesScreen;
        private Texture2D selectionScreen;
        private Texture2D pauseScreen;
        private Texture2D mainMenuScreen;
        


        //Camera Fields and player distance tracking
        private Rectangle cameraBounds;
        private Texture2D cameraBox;
        private Camera camera;

        //Timer
        private double timer;

        //Viewport
        private Viewport viewport;

        //Healthbar
        private Texture2D healthbar;
        private Texture2D healthBase;
        private Texture2D healthBorder;
        private Rectangle healthbarRectP1;
        private Rectangle healthbarRectP2;

        //Scores
        private int p1Score;
        private int p2Score;
        private Texture2D scoreTexture;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            soundEffects = new List<SoundEffect>();
            playerSounds = new List<SoundEffect>();
            IsMouseVisible = true;
        }

        //public properties for spritesheets
        //public Texture2D KairosSpriteSheet
        //{
        //    get { return kairosSpriteSheet; }
        //}

        protected override void Initialize()
        {
            //initialize resolution and graphics stuff
            //TODO: make this resolution stuff data driven
            _graphics.PreferredBackBufferWidth = 1280;  //640
            _graphics.PreferredBackBufferHeight = 720;  //360
            _graphics.IsFullScreen = false;
            _graphics.PreferMultiSampling = false;
            _graphics.ApplyChanges();

            //random
            rng = new Random();

            //get windowWidth and windowHeight
            windowWidth = _graphics.GraphicsDevice.Viewport.Width;
            windowHeight = _graphics.GraphicsDevice.Viewport.Height;

            //initialize player input Managers
            p1Input = new InputManager(PlayerIndex.One, kbState);
            p2Input = new InputManager(PlayerIndex.Two, kbState);

            //camera and viewport initialization
            viewport = new Viewport(cameraBounds);
            camera = new Camera(cameraBounds, cameraBox, solids, null, null, viewport, currentState, rng);

            // Menuing index
            menuItems = new GameStates[] { GameStates.Settings, GameStates.HowToPlay, GameStates.Quit };
            menuItemStrings = new string[] { "SETTINGS", "PLAY", "QUIT" };
            menuIndex = 1;

            // Character index
            character1Index = 0;
            character2Index = 0;
            p1Confirm = false;
            p2Confirm = false;
            p1Y = 0;
            p2Y = 0;

            //Scores
            p1Score = 0;
            p2Score = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            debugFont = this.Content.Load<SpriteFont>("debug");
            textColor = Color.Black;
            textSelectedColor = Color.White;

            playerCollisionTexture = this.Content.Load<Texture2D>("player_collisionBox");
            hitboxTexture = this.Content.Load<Texture2D>("hitBox");
            hurtboxTexture = this.Content.Load<Texture2D>("hurtBox");
            solidTexture = this.Content.Load<Texture2D>("solid");
            backgroundTexture = this.Content.Load<Texture2D>("background");

            //Character SpriteSheets
            kairosSpriteSheet = this.Content.Load<Texture2D>("Kairos_sprites");
            kairosTooSpriteSheet = this.Content.Load<Texture2D>("KairosToo_sprites");

            //sound effects
            soundEffects.Add(Content.Load<SoundEffect>("scroll"));
            playerSounds.Add(Content.Load<SoundEffect>("Impact 1"));
            playerSounds.Add(Content.Load<SoundEffect>("Impact 2"));
            playerSounds.Add(Content.Load<SoundEffect>("Impact 3"));

            //attack effects
            hitEffectTexture = Content.Load<Texture2D>("hitEffect");
            blockEffectTexture = Content.Load<Texture2D>("blockEffect");

            //music
            musicTest = Content.Load<Song>("musicTest");
            patriotism = Content.Load<Song>("Stars and Stripes");


            //Background Screens 
            optionsScreen = this.Content.Load<Texture2D>("Options");
            resultsScreen = this.Content.Load<Texture2D>("Results");
            rulesScreen = this.Content.Load<Texture2D>("Rules");
            selectionScreen = this.Content.Load<Texture2D>("Selection");
            pauseScreen = this.Content.Load<Texture2D>("Pause");
            mainMenuScreen = this.Content.Load<Texture2D>("MainMenu");
            resultsScreen = this.Content.Load<Texture2D>("Results");

            //camera position rectangle and texture
            cameraBox = this.Content.Load<Texture2D>("cameraBounds");
            cameraBounds = new Rectangle((windowWidth / 2) - cameraBox.Width / 2, 530, 427, 240);

            //  320 x 180 - camera size
            //  1280 x 720 - total game size

            //Health bar
            healthbar = this.Content.Load<Texture2D>("healthbar");
            healthBase = this.Content.Load<Texture2D>("healthbar_base");
            healthBorder = this.Content.Load<Texture2D>("healthbar_border");
            healthbarRectP1 = new Rectangle((camera.Position.X + 30), (camera.Position.Y + 10), 64, 16);
            healthbarRectP2 = new Rectangle((camera.Position.Right - 80), (camera.Position.Y + 10), healthbar.Width, healthbar.Height);
            scoreTexture = this.Content.Load<Texture2D>("RoundIcons");

            // Character Selection
            kairosName = this.Content.Load<Texture2D>("kairosNamePlate");
            kairosTooName = this.Content.Load<Texture2D>("kairosTooNamePlate");
            characterMenu = new Texture2D[] { kairosName, kairosTooName };
            p1Selector = this.Content.Load<Texture2D>("Player1Selector");
            p2Selector = this.Content.Load<Texture2D>("Player2Selector");
            kairosPortrait = this.Content.Load<Texture2D>("KairosPortrait");
            kairosTooPortrait = this.Content.Load<Texture2D>("KairosTooPortrait");
            characterPortraits = new Texture2D[] { kairosPortrait, kairosTooPortrait };
        }

        protected override void Update(GameTime gameTime)
        {
            //Basic hotkeys always available to be pressed
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.F11))
            {
                _graphics.IsFullScreen = !_graphics.IsFullScreen;
                _graphics.ApplyChanges();
            }

            //Update player inputs
            kbState = Keyboard.GetState();
            p1Input.Update(kbState);
            p2Input.Update(kbState);
            // Menu FSM
            switch (currentState)
            {
                case GameStates.MainMenu:

                    // Highlights different options based on input presses
                    if (p1Input.InputPressed(Inputs.Right, false) || p2Input.InputPressed(Inputs.Right, false))
                    {
                        menuIndex++;
                        soundEffects[0].Play();
                    }
                    if (p1Input.InputPressed(Inputs.Left, false) || p2Input.InputPressed(Inputs.Left, false))
                    {
                        menuIndex--;
                        soundEffects[0].Play();
                    }
                    // Cuts off index
                    if (menuIndex < 0)
                    {
                        menuIndex = 0;
                    }
                    if (menuIndex >= menuItems.Length)
                    {
                        menuIndex = menuItems.Length - 1;
                    }
                    // When selection is confirmed
                    if (p1Input.InputPressed(Inputs.Light, false) || p2Input.InputPressed(Inputs.Light, false) 
                        || (kbState.IsKeyDown(Keys.Enter) && prevKbState.IsKeyUp(Keys.Enter)))
                    {
                        currentState = menuItems[menuIndex];

                        // Move to next state
                        switch (currentState)
                        {
                            case GameStates.Settings:
                                menuItems = new GameStates[] { GameStates.MainMenu };
                                menuIndex = 0;
                                break;

                            case GameStates.HowToPlay:
                                menuItems = new GameStates[] { GameStates.MainMenu, GameStates.CharacterSelect };
                                menuIndex = 1;
                                break;

                            case GameStates.Quit:
                                Exit();
                                break;
                        }
                    }

                    break;

                case GameStates.HowToPlay:

                    // Highlights different options based on input presses
                    if (p1Input.InputPressed(Inputs.Right, false) || p2Input.InputPressed(Inputs.Right, false))
                    {
                        menuIndex++;
                    }
                    if (p1Input.InputPressed(Inputs.Left, false) || p2Input.InputPressed(Inputs.Left, false))
                    {
                        menuIndex--;
                    }
                    // Cuts off index
                    if (menuIndex < 0)
                    {
                        menuIndex = 0;
                    }
                    if (menuIndex >= menuItems.Length)
                    {
                        menuIndex = menuItems.Length - 1;
                    }
                    // When selection is confirmed
                    if (p1Input.InputPressed(Inputs.Light, false) || p2Input.InputPressed(Inputs.Light, false)
                        || (kbState.IsKeyDown(Keys.Enter) && prevKbState.IsKeyUp(Keys.Enter)))
                    {
                        currentState = menuItems[menuIndex];

                        // Move to next state
                        switch (currentState)
                        {
                            case GameStates.CharacterSelect:
                                menuItems = new GameStates[] { GameStates.GamePlay, GameStates.MainMenu };
                                menuIndex = 0;
                                character1Index = 0;
                                character2Index = 0;
                                p1Confirm = false;
                                p2Confirm = false;
                                break;

                            case GameStates.MainMenu:
                                menuItems = new GameStates[] { GameStates.Settings, GameStates.HowToPlay, GameStates.Quit };
                                menuItemStrings = new string[] { "SETTINGS", "PLAY", "QUIT" };
                                menuIndex = 1;
                                break;
                        }
                    }

                    break;

                case GameStates.CharacterSelect:

                    // Character index
                    if (p1Input.InputPressed(Inputs.Down, false) && !p1Confirm)
                    {
                        character1Index++;
                        p1Y = 120;
                    }
                    if (p2Input.InputPressed(Inputs.Down, false) && !p2Confirm)
                    {
                        character2Index++;
                        p2Y = 120;
                    }
                    if (p1Input.InputPressed(Inputs.Up, false) && !p1Confirm)
                    {
                        character1Index--;
                        p1Y = 0;
                    }
                    if (p2Input.InputPressed(Inputs.Up, false) && !p2Confirm)
                    {
                        character2Index--;
                        p2Y = 0;
                    }
                    // Bounds of index
                    if (character1Index < 0)
                    {
                        character1Index = 0;
                    }
                    if (character2Index < 0)
                    {
                        character2Index = 0;
                    }
                    if (character1Index >= characterMenu.Length)
                    {
                        character1Index = characterMenu.Length - 1;
                    }
                    if (character2Index >= characterMenu.Length)
                    {
                        character2Index = characterMenu.Length - 1;
                    }
                    // Confirm selection
                    if (p1Input.InputPressed(Inputs.Light, false))
                    {
                        p1Confirm = true;
                    }
                    if (p2Input.InputPressed(Inputs.Light, false))
                    {
                        p2Confirm = true;
                    }
                    // Once both have been confirmed, move to gameplay
                    if (p1Confirm && p2Confirm)
                    {
                        currentState = GameStates.GamePlay;

                        // Move to next state
                        switch (currentState)
                        {
                            case GameStates.GamePlay:
                                LoadGameplay();
                                menuItems = new GameStates[] { GameStates.Pause };
                                menuIndex = 0;
                                break;

                                //case GameStates.MainMenu:
                                //    menuItems = new GameStates[] { GameStates.Settings, GameStates.HowToPlay, GameStates.Quit };
                                //    menuItemStrings = new string[] { "SETTINGS", "PLAY", "QUIT" };
                                //    menuIndex = 1;
                                //    break;
                        }
                    }

                    break;

                case GameStates.Settings:

                    if (p1Input.InputPressed(Inputs.Heavy, false) || p2Input.InputPressed(Inputs.Heavy, false) 
                        || (kbState.IsKeyDown(Keys.Back) && prevKbState.IsKeyUp(Keys.Back)))
                    {
                        menuItems = new GameStates[] { GameStates.Settings, GameStates.HowToPlay, GameStates.Quit };
                        menuItemStrings = new string[] { "SETTINGS", "PLAY", "QUIT" };
                        menuIndex = 1;
                        currentState = GameStates.MainMenu;
                    }

                    break;

                case GameStates.GamePlay:
                    p1.Update(gameTime);
                    p2.Update(gameTime);

                    //Camera
                    camera.Update(gameTime, currentState);

                    //Camera shake check
                    if (p1.HitStop > 0 || p2.HitStop > 0)
                    {
                        //camera.ShakeTimer = 0;
                        camera.startShake(p1.HitStop > p2.HitStop ? p1.HitStop : p2.HitStop);
                    }

                    viewport = new Viewport(camera.Position.X, camera.Position.Y, camera.Position.Width, camera.Position.Height);



                    timer -= gameTime.ElapsedGameTime.TotalSeconds;

                    if (p1Input.InputPressed(Inputs.Start, false) || p2Input.InputPressed(Inputs.Start, false))
                    {
                        currentState = GameStates.Pause;
                    }

                    //Go to the results screen
                    if (p1.Health <= 0 || p2.Health <= 0 || timer <= 0)
                    {
                        //p1 win
                        if (p1.Health > p2.Health)
                        {
                            p1Score += 1;
                            if (p1Score + p2Score < 3 && p1Score != 2)
                            {
                                LoadGameplay();
                            }
                            else
                            {
                                winner = p1.Id;
                                currentState = GameStates.Results;
                                menuItems = new GameStates[] { GameStates.GamePlay, GameStates.CharacterSelect, GameStates.MainMenu };
                                menuItemStrings = new string[] { "REMATCH", "CHARACTER SELECT", "MAIN MENU" };
                                menuIndex = 0;
                                MediaPlayer.Stop();
                            }
                        }
                        //p2 win
                        else if (p2.Health > p1.Health)
                        {
                            p2Score += 1;
                            if (p1Score + p2Score < 3 && p2Score != 2)
                            {
                                LoadGameplay();
                            }
                            else
                            {
                                winner = p2.Id;
                                currentState = GameStates.Results;
                                menuItems = new GameStates[] { GameStates.GamePlay, GameStates.CharacterSelect, GameStates.MainMenu };
                                menuItemStrings = new string[] { "REMATCH", "CHARACTER SELECT", "MAIN MENU" };
                                menuIndex = 0;
                                MediaPlayer.Stop();
                            }
                        }
                        //timer run-out
                        else
                        {
                            if (p1Score + p2Score < 3)
                            {
                                LoadGameplay();
                            }
                            else
                            {
                                winner = Id.None;
                                currentState = GameStates.Results;
                                menuItems = new GameStates[] { GameStates.GamePlay, GameStates.CharacterSelect, GameStates.MainMenu };
                                menuItemStrings = new string[] { "REMATCH", "CHARACTER SELECT", "MAIN MENU" };
                                menuIndex = 0;
                                MediaPlayer.Stop();
                            }
                        }
                    }

                    //Check for hitstop


                    break;

                case GameStates.Pause:
                    viewport = new Viewport(0, 0, 512, 288);
                    camera.Update(gameTime, GameStates.Pause);


                    if (p1Input.InputPressed(Inputs.Start, false) || p2Input.InputPressed(Inputs.Start, false))
                    {
                        currentState = GameStates.GamePlay;
                    }
                    if (p1Input.InputPressed(Inputs.Favor, false) || p2Input.InputPressed(Inputs.Favor, false))
                    {
                        currentState = GameStates.MainMenu;
                        //stop music and reset score
                        MediaPlayer.Stop();
                        p1Score = 0;
                        p2Score = 0;
                        menuItems = new GameStates[] { GameStates.Settings, GameStates.HowToPlay, GameStates.Quit };
                        menuItemStrings = new string[] { "SETTINGS", "PLAY", "QUIT" };
                        menuIndex = 1;
                    }
                    break;

                case GameStates.Results:
                    viewport = new Viewport(0, 0, 512, 288);
                    camera.Update(gameTime, GameStates.Results);

                    //reset score
                    p1Score = 0;
                    p2Score = 0;

                    // Highlights different options based on input presses
                    if (p1Input.InputPressed(Inputs.Down, false) || p2Input.InputPressed(Inputs.Down, false))
                    {
                        menuIndex++;
                    }
                    if (p1Input.InputPressed(Inputs.Up, false) || p2Input.InputPressed(Inputs.Up, false))
                    {
                        menuIndex--;
                    }
                    // Cuts off index
                    if (menuIndex < 0)
                    {
                        menuIndex = 0;
                    }
                    if (menuIndex >= menuItems.Length)
                    {
                        menuIndex = menuItems.Length - 1;
                    }
                    // When selection is confirmed
                    if (p1Input.InputPressed(Inputs.Light, false) || p2Input.InputPressed(Inputs.Light, false) 
                        || (kbState.IsKeyDown(Keys.Enter) && prevKbState.IsKeyUp(Keys.Enter)))
                    {
                        currentState = menuItems[menuIndex];


                        // Move to next state
                        switch (currentState)
                        {
                            case GameStates.GamePlay:
                                LoadGameplay();
                                menuItems = new GameStates[] { GameStates.Pause };
                                menuIndex = 0;
                                break;

                            case GameStates.MainMenu:
                                menuItems = new GameStates[] { GameStates.Settings, GameStates.HowToPlay, GameStates.Quit };
                                menuItemStrings = new string[] { "SETTINGS", "PLAY", "QUIT" };
                                menuIndex = 1;
                                break;

                            case GameStates.CharacterSelect:
                                p1Confirm = false;
                                p2Confirm = false;
                                menuItems = new GameStates[] { GameStates.GamePlay, GameStates.MainMenu };
                                menuIndex = 0;
                                break;
                        }
                    }

                    //if (p1Input.InputPressed(Inputs.Light) || p2Input.InputPressed(Inputs.Light))
                    //    if (p1Input.InputPressed(Inputs.Light) || p2Input.InputPressed(Inputs.Light))
                    //    {
                    //        currentState = GameStates.GamePlay;
                    //        LoadGameplay();
                    //    }
                    //if (p1Input.InputPressed(Inputs.Heavy) || p2Input.InputPressed(Inputs.Heavy))
                    //{
                    //    currentState = GameStates.CharacterSelect;
                    //}
                    //if (p1Input.Favor || p2Input.Favor)
                    //{
                    //    currentState = GameStates.MainMenu;
                    //}
                    break;
            }
            // Update our saved device state
            prevKbState = kbState;
            p1Input.SetPrevInputs();
            p2Input.SetPrevInputs();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            Matrix viewMatrix = camera.TransformViewport(viewport);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: viewMatrix);

            switch (currentState)
            {
                case GameStates.MainMenu:

                    _spriteBatch.Draw(mainMenuScreen, new Rectangle(0, 0, windowWidth, windowHeight), Color.White);

                    for (int i = 0; i < menuItems.Length; i++)
                    {
                        _spriteBatch.DrawString(debugFont, menuItemStrings[i], new Vector2(45 + 550 * i, i % 2 == 0 ? 620 : 550), menuIndex == i ? textSelectedColor : textColor);
                    }

                    break;

                case GameStates.HowToPlay:

                    _spriteBatch.Draw(rulesScreen, new Rectangle(0, 0, windowWidth, windowHeight), Color.White);
                    _spriteBatch.DrawString(debugFont, "Rules", new Vector2(550, 100), textColor);
                    //Drawing player 1's rules
                    _spriteBatch.DrawString(debugFont, "Player 1\n\nR - Light\nT - Heavy\nY - Favor (NT)\nWASD - Movement\nEnter - Start", new Vector2(300, 300), textColor);
                    //Drawing Player 2's rules
                    _spriteBatch.DrawString(debugFont, "Player 2\n\n1 on numpad - Light\n2 on numpad - Heavy\n3 on numpad - Favor (NT)" +
                        "\nArrow Keys - Movement\n9 on numpad - Start", new Vector2(700, 300), textColor);
                    //Drawing rules that apply to both players
                    _spriteBatch.DrawString(debugFont, "Light = X, Heavy = Y, Favor = RB or B", new Vector2(350, 580), textColor);
                    _spriteBatch.DrawString(debugFont, "Block by holding in the opposite direction from which you are facing", new Vector2(180, 640), textColor);

                    break;

                case GameStates.CharacterSelect:

                    // Background
                    _spriteBatch.Draw(selectionScreen, new Rectangle(0, 0, windowWidth, windowHeight), Color.White);

                    // Player titles
                    _spriteBatch.DrawString(debugFont, "Candidate 1", new Vector2(120, 90), Color.Black);
                    _spriteBatch.DrawString(debugFont, "Candidate 2", new Vector2(920, 90), Color.Black);

                    // Draw name plates
                    for (int i = 0; i < characterMenu.Length; i++)
                    {
                        _spriteBatch.Draw(characterMenu[i], new Rectangle((windowWidth / 3) + 100, 210 + (i * 120), 225, 93), Color.White);
                    }

                    // Player 1
                    if (!p1Confirm)
                    {
                        // Selector
                        _spriteBatch.Draw(p1Selector, new Rectangle((windowWidth / 3) + 100, 210 + p1Y, 225, 93),
                            new Rectangle(0, 0, p1Selector.Width / 2, p1Selector.Height), Color.White);

                        // Character sprite
                        _spriteBatch.Draw(characterPortraits[character1Index], 
                            new Rectangle(118, 100, characterPortraits[character1Index].Width * 4, characterPortraits[character1Index].Height * 4), 
                            Color.White);
                    }
                    else
                    {
                        // Selector
                        _spriteBatch.Draw(p1Selector, new Rectangle((windowWidth / 3) + 100, 210 + p1Y, 225, 93),
                            new Rectangle(p1Selector.Width / 2, 0, p1Selector.Width / 2, p1Selector.Height), Color.White);

                        // Character sprite
                        _spriteBatch.Draw(characterPortraits[character1Index],
                            new Rectangle(118, 100, characterPortraits[character1Index].Width * 4, characterPortraits[character1Index].Height * 4),
                            Color.White);
                    }

                    // Player 2
                    if (!p2Confirm)
                    {
                        // Selector
                        _spriteBatch.Draw(p2Selector, new Rectangle((windowWidth / 3) + 100, 210 + p2Y, 225, 93),
                            new Rectangle(0, 0, p2Selector.Width / 2, p2Selector.Height), Color.White);

                        // Character sprite
                        _spriteBatch.Draw(characterPortraits[character2Index], 
                            new Rectangle(860, 100, characterPortraits[character2Index].Width * 4, characterPortraits[character2Index].Height * 4), 
                            new Rectangle(0,0, characterPortraits[character2Index].Width, characterPortraits[character2Index].Height), 
                            Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
                    }
                    else
                    {
                        // Selector
                        _spriteBatch.Draw(p2Selector, new Rectangle((windowWidth / 3) + 100, 210 + p2Y, 225, 93),
                            new Rectangle(p2Selector.Width / 2, 0, p2Selector.Width / 2, p2Selector.Height), Color.White);

                        // Character sprite
                        _spriteBatch.Draw(characterPortraits[character2Index],
                            new Rectangle(860, 100, characterPortraits[character2Index].Width * 4, characterPortraits[character2Index].Height * 4),
                            new Rectangle(0, 0, characterPortraits[character2Index].Width, characterPortraits[character2Index].Height),
                            Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
                    }

                    // Player confirmations
                    if (p1Confirm)
                    {
                        _spriteBatch.DrawString(debugFont, "CANDIDATE 1 READY!", new Vector2((windowWidth / 4) - 200, 630), Color.Yellow);
                    }
                    if (p2Confirm)
                    {
                        _spriteBatch.DrawString(debugFont, "CANDIDATE 2 READY!", new Vector2(windowWidth - 400, 630), Color.Yellow);
                    }

                    break;

                case GameStates.Settings:

                    _spriteBatch.Draw(optionsScreen, new Rectangle(0, 0, windowWidth, windowHeight), Color.White);

                    _spriteBatch.DrawString(debugFont, "CREDITS", new Vector2((windowWidth / 2) - 100, 200), Color.Black);

                    _spriteBatch.DrawString(debugFont, "Patrick Emmons", new Vector2((windowWidth / 2) - 130, 300), Color.Black);
                    _spriteBatch.DrawString(debugFont, "Corinne Dushay", new Vector2((windowWidth / 2) - 130, 350), Color.Black);
                    _spriteBatch.DrawString(debugFont, "Rebecca Greene", new Vector2((windowWidth / 2) - 130, 400), Color.Black);
                    _spriteBatch.DrawString(debugFont, "Sean Shapiro", new Vector2((windowWidth / 2) - 130, 450), Color.Black);
                    _spriteBatch.DrawString(debugFont, "Beau Wacker", new Vector2((windowWidth / 2) - 130, 500), Color.Black);

                    _spriteBatch.DrawString(debugFont, "Press BACKSPACE \nto go back", new Vector2(1000, 500), Color.White);


                    break;

                case GameStates.GamePlay:

                    //draw background
                    _spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, windowWidth, windowHeight), Color.White);

                    //draw solids if need be for debugging
                    /*foreach (Solid n in solids)
                    {
                        n.Draw(_spriteBatch);
                    }*/

                    //draw players
                    p1.Draw(_spriteBatch);
                    p2.Draw(_spriteBatch);

                    foreach (AttackEffect n in p1.AttackEffects)
                    {
                        n.Draw(_spriteBatch);
                    }
                    foreach (AttackEffect n in p2.AttackEffects)
                    {
                        n.Draw(_spriteBatch);
                    }

                    //Healthbar base
                    _spriteBatch.Draw(healthBase, new Vector2((camera.DrawPosition.X + 30), (camera.DrawPosition.Y + 10)), Color.Blue);
                    _spriteBatch.Draw(healthBase, new Vector2((camera.DrawPosition.Right - healthBase.Width - 30), (camera.DrawPosition.Y + 10)), Color.Blue);
                    //Healthbar
                    _spriteBatch.Draw(healthbar, new Rectangle((camera.DrawPosition.X + 30 + (healthbar.Width - (int)(healthbar.Width * (p1.Health / p1.TotalHealth)))), (camera.DrawPosition.Y + 10), (int)(healthbar.Width * (p1.Health / p1.TotalHealth)), 16), Color.Yellow);
                    _spriteBatch.Draw(healthbar, new Rectangle((camera.DrawPosition.Right - healthBase.Width - 30), (camera.DrawPosition.Y + 10), (int)(healthbar.Width * (p2.Health / p2.TotalHealth)), 16), Color.Yellow);
                    //Healthbar border
                    _spriteBatch.Draw(healthBorder, new Vector2((camera.DrawPosition.X + 30), (camera.DrawPosition.Y + 10)), Color.Blue);
                    _spriteBatch.Draw(healthBorder, new Vector2((camera.DrawPosition.Right - healthBase.Width - 30), (camera.DrawPosition.Y + 10)), Color.Blue);

                    //draw UI (combo count, timer, meter)

                    _spriteBatch.DrawString(debugFont, $"{p1.Favor}", new Vector2((camera.DrawPosition.X + 30), (camera.DrawPosition.Y + 205)), Color.Blue);
                    _spriteBatch.DrawString(debugFont, $"{p2.Favor}", new Vector2((camera.DrawPosition.Right - 80), (camera.DrawPosition.Y + 205)), Color.Blue);
                    _spriteBatch.DrawString(debugFont, $"{(int)timer}", new Vector2((camera.DrawPosition.X + (camera.DrawPosition.Width / 2) - 20), (camera.DrawPosition.Y + 10)), Color.White);

                    //draw score checkboxes

                    //p1Score
                    if (p1Score == 0)
                    {
                        _spriteBatch.Draw(scoreTexture, new Vector2((camera.DrawPosition.X + 160), (camera.DrawPosition.Y + 40)), new Rectangle(0, 0, scoreTexture.Width, 8), Color.White);
                    }
                    else if (p1Score == 1)
                    {
                        _spriteBatch.Draw(scoreTexture, new Vector2((camera.DrawPosition.X + 160), (camera.DrawPosition.Y + 40)), new Rectangle(0, 8, scoreTexture.Width, 8), Color.White);
                    }
                    else
                    {
                        _spriteBatch.Draw(scoreTexture, new Vector2((camera.DrawPosition.X + 160), (camera.DrawPosition.Y + 40)), new Rectangle(0, 16, scoreTexture.Width, 8), Color.White);
                    }

                    //p2Score
                    if (p2Score == 0)
                    {
                        _spriteBatch.Draw(scoreTexture, new Vector2((camera.DrawPosition.X + 240), (camera.DrawPosition.Y + 40)), new Rectangle(0, 0, scoreTexture.Width, 8), Color.White);
                    }
                    else if (p2Score == 1)
                    {
                        _spriteBatch.Draw(scoreTexture, new Vector2((camera.DrawPosition.X + 240), (camera.DrawPosition.Y + 40)), new Rectangle(0, 8, scoreTexture.Width, 8), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f);
                    }
                    else
                    {
                        _spriteBatch.Draw(scoreTexture, new Vector2((camera.DrawPosition.X + 240), (camera.DrawPosition.Y + 40)), new Rectangle(0, 16, scoreTexture.Width, 8), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f);
                    }


                    if (p1.ComboCount > 0)
                    {
                        _spriteBatch.DrawString(debugFont, $"{p1.ComboCount} Hits", new Vector2((camera.DrawPosition.Right - 110), (camera.DrawPosition.Y + 40)), Color.DarkRed);
                    }
                    if (p2.ComboCount > 0)
                    {
                        _spriteBatch.DrawString(debugFont, $"{p2.ComboCount} Hits", new Vector2((camera.DrawPosition.X + 30), (camera.DrawPosition.Y + 40)), Color.DarkRed);
                    }
                    break;

                case GameStates.Pause:

                    _spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, windowWidth, windowHeight), Color.White);
                    _spriteBatch.Draw(pauseScreen, new Rectangle(0, 0, windowWidth, windowHeight), Color.White);
                    _spriteBatch.DrawString(debugFont, "PAUSE", new Vector2(windowWidth / 2 - 40, 160), Color.White);
                    _spriteBatch.DrawString(debugFont, "To go back to gameplay, Press ENTER", new Vector2(windowWidth - 970, 200), Color.White);
                    _spriteBatch.DrawString(debugFont, "To go back to the main menu, Press P of Y", new Vector2(windowWidth - 970, 250), Color.White);
                    break;

                case GameStates.Results:

                    _spriteBatch.Draw(resultsScreen,
                        new Rectangle(0, 0, windowWidth, windowHeight),
                        Color.White);

                    // Placeholder Text
                    if (winner == p1.Id)
                    {
                        _spriteBatch.DrawString(debugFont, p1.Name.ToUpper() + " WINS DEBATE!", 
                            new Vector2((_graphics.PreferredBackBufferWidth / 3) + 200, _graphics.PreferredBackBufferHeight / 5), Color.Black);
                        _spriteBatch.Draw(characterPortraits[character1Index],
                            new Rectangle(118, 100, characterPortraits[character1Index].Width * 4, characterPortraits[character1Index].Height * 4),
                            Color.White);
                    }
                    else if (winner == p2.Id)
                    {
                        _spriteBatch.DrawString(debugFont, p2.Name.ToUpper() + " WINS DEBATE!", 
                            new Vector2((_graphics.PreferredBackBufferWidth / 3) + 200, _graphics.PreferredBackBufferHeight / 5), Color.Black);
                        _spriteBatch.Draw(characterPortraits[character1Index],
                            new Rectangle(118, 100, characterPortraits[character2Index].Width * 4, characterPortraits[character1Index].Height * 4),
                            Color.White);

                    }
                    else
                    {
                        _spriteBatch.DrawString(debugFont, "LOOKS LIKE WE HAVE OURSELVES A STALEMATE!", 
                            new Vector2((_graphics.PreferredBackBufferWidth / 3) + 50, _graphics.PreferredBackBufferHeight / 5), Color.Black);
                    }


                    for (int i = 0; i < menuItems.Length; i++)
                    {
                        _spriteBatch.DrawString(debugFont, menuItemStrings[i], new Vector2(windowWidth - 500, 320 + 80 * i), menuIndex == i ? textSelectedColor : textColor);
                    }
                    break;

            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// load the things needed for the gameplay part of the game
        /// </summary>
        public void LoadGameplay()
        {
            //play music
            if (p1Score == 0 && p2Score == 0)
            {
                MediaPlayer.Play(musicTest);
                MediaPlayer.IsRepeating = true;
            }



            //load game bounds
            solids = new Solid[3] {
                new Solid(new Rectangle(0, 0, solidTexture.Width, windowHeight), solidTexture),
                new Solid(new Rectangle(windowWidth-solidTexture.Width, 0, solidTexture.Width, windowHeight), solidTexture),
                new Solid(new Rectangle(0 ,windowHeight-solidTexture.Height, windowWidth, solidTexture.Height), solidTexture)
            };

            //load character lists
            p1Characters = new Player[]
            {
                new Kairos(playerCollisionTexture, hurtboxTexture, hitboxTexture, hitEffectTexture, blockEffectTexture, playerSounds, p1Input, Id.P1, solids, (windowWidth / 2 - 100), kairosSpriteSheet, camera, rng),
                new KairosToo(playerCollisionTexture, hurtboxTexture, hitboxTexture, hitEffectTexture, blockEffectTexture, playerSounds, p1Input, Id.P1, solids, (windowWidth / 2 - 100), kairosTooSpriteSheet, camera, rng)
            };

            p2Characters = new Player[]
                {
                new Kairos(playerCollisionTexture, hurtboxTexture, hitboxTexture, hitEffectTexture, blockEffectTexture, playerSounds, p2Input, Id.P2, solids, (windowWidth / 2 + 100), kairosSpriteSheet, camera, rng),
                new KairosToo(playerCollisionTexture, hurtboxTexture, hitboxTexture, hitEffectTexture, blockEffectTexture, playerSounds, p2Input, Id.P2, solids, (windowWidth / 2 + 100), kairosTooSpriteSheet, camera, rng)
            };

            //load players
            p1 = p1Characters[character1Index];
            p2 = p2Characters[character2Index];
            p1.Opponent = p2;
            p2.Opponent = p1;

            //load camera
            camera = new Camera(cameraBounds, cameraBox, solids, p1, p2, viewport, currentState, rng);
            p1.Camera = camera;
            p2.Camera = camera;

            //Set Timer
            timer = 100;

            //load stats from file
            p1.StatUpdate();
            p2.StatUpdate();
        }

        public bool KeyPressed(Keys key, KeyboardState kbState)
        {
            return kbState.IsKeyDown(key) && prevKbState.IsKeyUp(key);
        }
    }
}
