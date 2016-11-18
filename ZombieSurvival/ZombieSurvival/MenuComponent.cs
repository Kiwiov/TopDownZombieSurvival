using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ZombieSurvival
{
    public class MenuComponent : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        SpriteFont normalFont;
        SpriteFont selectedFont;
        Color backgroundColor;
        MouseState previousMouseState;
        Menu menu;
        Menu activeMenu;

        public MenuComponent(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            menu = new Menu();
            activeMenu = menu;

            var optionsMenu = new Menu();
            var graphicsMenu = new Menu();
            var audioMenu = new Menu();
            var otherOptionMenu = new Menu();
            
           
            menu.Items = new List<MenuChoice>
            {
                new MenuChoice(menu) {Text = "Zombie Survival", IsEnabled = false , IsVisible = () => Game1.GameState != GameState.Paused && Game1.GameState != GameState.Playing},
                new MenuChoice(menu) {Text = "START", Selected = true, ClickAction = MenuStartClicked, IsVisible = () => Game1.GameState != GameState.Paused && Game1.GameState != GameState.Playing},
                new MenuChoice(menu) {Text = "PAUSED", ClickAction = MenuStartClicked, IsVisible = () => Game1.GameState != GameState.Startup && Game1.GameState != GameState.Playing, IsEnabled = false},
                new MenuChoice(menu) {Text = "OPTIONS", ClickAction = MenuOptionsClicked, SubMenu = optionsMenu, IsVisible = () => Game1.GameState != GameState.Paused && Game1.GameState != GameState.Playing},
                new MenuChoice(menu) {Text = "QUIT", ClickAction = MenuQuitClicked, IsVisible = () => Game1.GameState != GameState.Paused && Game1.GameState != GameState.Playing}
            };

            optionsMenu.Items = new List<MenuChoice>
            {
                new MenuChoice(menu) { Text = "Graphics Settings", ClickAction = GraphicsOptionsClicked, SubMenu = graphicsMenu},
                new MenuChoice(menu) { Text = "Audio Settings", ClickAction = AudioOptionsClicked, SubMenu = audioMenu},
                new MenuChoice(menu) { Text = "Other Settings", ClickAction = OtherOptionsClicked, SubMenu = otherOptionMenu}
                };

            graphicsMenu.Items = new List<MenuChoice>
            {
                new MenuChoice(menu) { Text = "Back", ClickAction = MenuOptionsClicked, SubMenu = optionsMenu}
            };

            audioMenu.Items = new List<MenuChoice>
            {
                new MenuChoice(menu) { Text = "Back", ClickAction = MenuOptionsClicked, SubMenu = optionsMenu}
            };

            otherOptionMenu.Items = new List<MenuChoice>
            {
                new MenuChoice(menu) { Text = "Back", ClickAction = MenuOptionsClicked, SubMenu = optionsMenu}
            };
            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            normalFont = Game.Content.Load<SpriteFont>("menuFontNormal");
            selectedFont = Game.Content.Load<SpriteFont>("menuFontSelected");
            backgroundColor = Color.CornflowerBlue;

            previousMouseState = Mouse.GetState();

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (KeyboardComponent.KeyPressed(Keys.Escape))
            {
                    var selectedChoice = activeMenu.Items.First(c => c.Selected);
                if (selectedChoice.ParentMenu != null)
                    activeMenu = selectedChoice.ParentMenu;
            }
            if (KeyboardComponent.KeyPressed(Keys.Down))
                NextMenuChoice();
            if (KeyboardComponent.KeyPressed(Keys.Up))
                PreviousMenuChoice();
            if (KeyboardComponent.KeyPressed(Keys.Enter))
            {
                var selectedChoice = activeMenu.Items.First(c => c.Selected);
                selectedChoice.ClickAction.Invoke();
                if (selectedChoice.SubMenu != null)
                    activeMenu = selectedChoice.SubMenu;
            }

            var mouseState = Mouse.GetState();

            foreach (var choice in activeMenu.Items)
            {
                if (choice.HitBox.Contains(mouseState.X, mouseState.Y) && choice.IsEnabled)
                {
                    activeMenu.Items.ForEach(c => c.Selected = false);
                    choice.Selected = true;

                    if (previousMouseState.LeftButton == ButtonState.Released
                        && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        choice.ClickAction.Invoke();
                        if (choice.SubMenu != null)
                            activeMenu = choice.SubMenu;
                    }
                }
            }

            previousMouseState = mouseState;

            float startY = 0.2f * GraphicsDevice.Viewport.Height;
            foreach (var choice in activeMenu.Items)
            {
                if (!choice.IsVisible())
                    continue;

                Vector2 size = normalFont.MeasureString(choice.Text);
                choice.Y = startY;
                choice.X = GraphicsDevice.Viewport.Width / 2.0f - size.X / 2;
                choice.HitBox = new Rectangle((int)choice.X, (int)choice.Y, (int)size.X, (int)size.Y);
                startY += 70;
            }

            base.Update(gameTime);
        }

        private void PreviousMenuChoice()
        {
            int selectedIndex = activeMenu.Items.IndexOf(activeMenu.Items.First(c => c.Selected));
            activeMenu.Items[selectedIndex].Selected = false;

            for (int i = 0; i < activeMenu.Items.Count; i++)
            {
                selectedIndex--;
                if (selectedIndex < 0)
                    selectedIndex = activeMenu.Items.Count - 1;
                if (activeMenu.Items[selectedIndex].IsVisible() && activeMenu.Items[selectedIndex].IsEnabled)
                {
                    activeMenu.Items[selectedIndex].Selected = true;
                    break;
                }
            }
        }

        private void NextMenuChoice()
        {
            int selectedIndex = activeMenu.Items.IndexOf(activeMenu.Items.First(c => c.Selected));
            activeMenu.Items[selectedIndex].Selected = false;

            for (int i = 0; i < activeMenu.Items.Count; i++)
            {
                selectedIndex++;
                if (selectedIndex >= activeMenu.Items.Count)
                    selectedIndex = 0;
                if (activeMenu.Items[selectedIndex].IsVisible() && activeMenu.Items[selectedIndex].IsEnabled)
                {
                    activeMenu.Items[selectedIndex].Selected = true;
                    break;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (Game1.GameState == GameState.Startup)
            {
                GraphicsDevice.Clear(backgroundColor);
            }
            spriteBatch.Begin();

            foreach (var choice in activeMenu.Items)
            {
                if (!choice.IsVisible())
                    continue;
                var blendColor = (choice.IsEnabled) ? Color.White : Color.Black;
                spriteBatch.DrawString(choice.Selected ? selectedFont : normalFont, choice.Text, new Vector2(choice.X, choice.Y), blendColor);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        #region Menu Clicks

        private void MenuStartClicked()
        {
            Game1.GameState = GameState.Playing;
        }

        private void MenuSelectClicked()
        {
            backgroundColor = Color.White;
        }

        private void MenuOptionsClicked()
        {
            backgroundColor = Color.Silver;
        }

        private void GraphicsOptionsClicked()
        {
            backgroundColor = Color.Bisque;
        }

        private void AudioOptionsClicked()
        {
            backgroundColor = Color.Red;
        }

        private void OtherOptionsClicked()
        {
            backgroundColor = Color.SeaGreen;
        }

        private void MenuQuitClicked()
        {
            this.Game.Exit();
        }

        #endregion
    }
}
