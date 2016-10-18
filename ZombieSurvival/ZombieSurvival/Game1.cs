using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
            Paused
        }
         GameState _currentGameState = GameState.Menu;

        // Screen Adjustments
        readonly int _screenWidth = 1920;
        readonly int _screenHeight = 1200;

        private CButton _btnPlay;

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
            _graphics.PreferredBackBufferWidth = _screenWidth;
            _graphics.PreferredBackBufferHeight = _screenHeight;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
            IsMouseVisible = true;

            _btnPlay = new CButton(Content.Load<Texture2D>("Images/Button"), _graphics.GraphicsDevice);
            _btnPlay.SetPosition(new Vector2(750, 600));
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


      

            
            switch (_currentGameState)
            {
                case GameState.Menu:
                    if(_btnPlay.IsClicked != true) _currentGameState = GameState.Menu;
                    _btnPlay.Update(mouse);
                    break;

                case GameState.Playing:
                    if (_btnPlay.IsClicked == true) _currentGameState = GameState.Playing;
                    _btnPlay.Update(mouse);
                    break;

                case GameState.Options:

                    break;

                case GameState.Paused:

                    break;

                default:

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
                    _spriteBatch.Draw(Content.Load<Texture2D>("Images/background"), new Rectangle(0, 0, _screenWidth, _screenHeight), Color.White);
                    _btnPlay.Draw(_spriteBatch);
                    break;

                case GameState.Playing:

                    break;

                case GameState.Options:

                    break;

                case GameState.Paused:

                    break;

                default:

                    break;
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
