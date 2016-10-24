using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Net.Mime.MediaTypeNames;

namespace ZombieSurvival
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private enum GameState
        {
            Menu,
            Options,
            Playing,
            HighScore,
            Exit,
            Paused
        }
         GameState _currentGameState = GameState.Menu;

        // Screen Adjustments
        readonly int screenWidth = 1920;
        readonly int screenHeight = 1200;

        private CButton _btnPlay;
        private CButton _btnOption;
        private CButton _btnExit;
        private CButton _btnMenu;
        private CButton _btnHighScore;

        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Screen stuff
            _graphics.PreferredBackBufferWidth = screenWidth;
            _graphics.PreferredBackBufferHeight = screenHeight;
            //_graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
            IsMouseVisible = true;

            _btnPlay = new CButton(Content.Load<Texture2D>("Images/Button"), _graphics.GraphicsDevice);
            _btnPlay.SetPosition(new Vector2(800, 900));
            _btnOption = new CButton(Content.Load<Texture2D>("Images/Button"), _graphics.GraphicsDevice);
            _btnOption.SetPosition(new Vector2(450, 1000));
            _btnExit = new CButton(Content.Load<Texture2D>("Images/Button"), _graphics.GraphicsDevice);
            _btnExit.SetPosition(new Vector2(800, 1100));
            _btnHighScore = new CButton(Content.Load<Texture2D>("Images/Button"), _graphics.GraphicsDevice);
            _btnHighScore.SetPosition(new Vector2(1200, 1000));
            _btnMenu = new CButton(Content.Load<Texture2D>("Images/Button"), _graphics.GraphicsDevice);
            _btnMenu.SetPosition(new Vector2(100, 1100));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }



        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            switch (_currentGameState)
            {
                case GameState.Menu:
                    if (_btnPlay.IsClicked)
                    _currentGameState = GameState.Playing;
                    _btnPlay.Update(mouse);
                    if (_btnOption.IsClicked) _currentGameState = GameState.Options;
                    _btnOption.Update(mouse);
                    if (_btnExit.IsClicked) Exit();
                    _btnExit.Update(mouse);
                    if (_btnHighScore.IsClicked) _currentGameState = GameState.HighScore;
                    _btnHighScore.Update(mouse);
                    break;

                case GameState.Playing:
                    if (_btnPlay.IsClicked) _currentGameState = GameState.Paused;
                    _btnPlay.Update(mouse);
                    if (_btnMenu.IsClicked) _currentGameState = GameState.Menu;
                    _btnMenu.Update(mouse);
                    break;

                    case GameState.Options:
                    if (_btnMenu.IsClicked) _currentGameState = GameState.Menu;
                    _btnMenu.Update(mouse);
                    break;

                case GameState.HighScore:
                    if (_btnMenu.IsClicked) _currentGameState = GameState.Menu;
                    _btnMenu.Update(mouse);
                    break;

                case GameState.Paused:
                    if (_btnPlay.IsClicked) _currentGameState = GameState.Playing;
                    _btnPlay.Update(mouse);
                    if (_btnOption.IsClicked) _currentGameState = GameState.Options;
                    _btnOption.Update(mouse);
                    if (_btnMenu.IsClicked) _currentGameState = GameState.Menu;
                    _btnMenu.Update(mouse);
                    break;

                
                    
            }
            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            switch (_currentGameState)
            {
                case GameState.Menu:
                    _spriteBatch.Draw(Content.Load<Texture2D>("Images/zombie-survival"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    _btnPlay.Draw(_spriteBatch);
                    _btnOption.Draw(_spriteBatch);
                    _btnExit.Draw(_spriteBatch);
                    _btnHighScore.Draw(_spriteBatch);

                    break;

                case GameState.Playing:
                    _spriteBatch.Draw(Content.Load<Texture2D>("Images/ThaGame"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    _btnMenu.Draw(_spriteBatch);
                    break;

                case GameState.Options:
                    _spriteBatch.Draw(Content.Load<Texture2D>("Images/Option"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                    _btnMenu.Draw(_spriteBatch);
                    break;

                case GameState.HighScore:
                    _spriteBatch.Draw(Content.Load<Texture2D>("Images/HighScore"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                   _btnMenu.Draw(_spriteBatch);
                    break;

               case GameState.Paused:

                    break;

                
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
