using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar_Lander
{
    internal class MenuView : GameStateView
    {

        private (string, GameStateEnum)[] m_menuArray;
        private int m_selectedIndex;

        private KeyboardInput m_keyboard;
        private GameStateEnum m_nextState;
        private Texture2D backgroundTexture;
        private Texture2D rectangleTexture;
        private SpriteFont roboto;
        public MenuView(GameStateEnum myState) : base(myState)
        {

            m_menuArray = new (string,GameStateEnum)[]{ 
                ("New Game", GameStateEnum.Level1),
                ("High Scores", GameStateEnum.HighScores),
                ("Controls", GameStateEnum.Controls),
                ("Credits", GameStateEnum.Credits)
            };
            m_selectedIndex = 0;
        }

        public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_keyboard = new KeyboardInput();
            m_nextState = m_myState;
            this.RegisterCommands();
            base.Initialize(graphicsDevice, graphics);
        }

        public override void LoadContent(ContentManager contentManager)
        {
            roboto = contentManager.Load<SpriteFont>("roboto");
            backgroundTexture = contentManager.Load<Texture2D>("space");
            rectangleTexture = contentManager.Load<Texture2D>("whiteRectangle");
        }

        public override void RegisterCommands()
        {
            m_keyboard.registerCommand(Keys.Up, true, MenuUp);
            m_keyboard.registerCommand(Keys.Down, true, MenuDown);
            m_keyboard.registerCommand(Keys.Enter, true, MenuSelect);

        }

        #region Input Handler Functions
        private void MenuUp(GameTime gameTime, float value)
        {
            m_selectedIndex--;
            if (m_selectedIndex < 0) m_selectedIndex = m_menuArray.Length - 1;
        }

        private void MenuDown(GameTime gameTime, float value)
        {
            m_selectedIndex++;
            if (m_selectedIndex >= m_menuArray.Length) m_selectedIndex = 0;
        }

        private void MenuSelect(GameTime gameTime, float value)
        {
            m_nextState = m_menuArray[m_selectedIndex].Item2;
        }
        #endregion

        public override void ProcessInput(GameTime gameTime)
        {
            m_keyboard.Update(gameTime);
        }

        public override GameStateEnum Update(GameTime gameTime)
        {
            ProcessInput(gameTime);
            return m_nextState;
        }
        public override void Draw(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            // Render background
            m_spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight), null, Color.White, 0, new Vector2(), SpriteEffects.None, 0);

            // Render backdrop for text
            m_spriteBatch.Draw(rectangleTexture, new Rectangle(m_graphics.PreferredBackBufferWidth / 4, 100, m_graphics.PreferredBackBufferWidth / 2, m_graphics.PreferredBackBufferHeight - 200), new Color(Color.Black, 0.5f));

            // Render text
            m_spriteBatch.DrawString(roboto, "Mars Lander", new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 100), Color.Orange, 0f, new Vector2(), 2f, SpriteEffects.None, 0);
            for (int i = 0; i < m_menuArray.Length; i++)
            {
                Color textColor = Color.White;
                if (i == m_selectedIndex) textColor = Color.OrangeRed;
                m_spriteBatch.DrawString(roboto, m_menuArray[i].Item1, new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 150 + i * 50), textColor);
            }

            m_spriteBatch.End();
        }
    }
}
