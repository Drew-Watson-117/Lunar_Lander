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
    public class CreditsView : GameStateView
    {

        private string[] m_creditsArray;

        private KeyboardInput m_keyboard;
        private GameStateEnum m_nextState;
        private Texture2D backgroundTexture;
        private Texture2D rectangleTexture;
        private SpriteFont roboto;
        public CreditsView(GameStateEnum myState) : base(myState)
        {

            m_creditsArray = new string[]{
                "Programming: Drew Watson",
                "Assets: Drew Watson",
                "Background Image: Hubble Space Telescope",
            };
        }

        public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_keyboard = new KeyboardInput();
            m_nextState = m_myState;
            RegisterCommands();
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
            m_keyboard.registerCommand(Keys.Escape, true, (gameTime, value) => { m_nextState = GameStateEnum.Menu; });
        }

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
            m_spriteBatch.DrawString(roboto, "Credits", new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 100), Color.Orange, 0f, new Vector2(), 2f, SpriteEffects.None, 0);
            for (int i = 0; i < m_creditsArray.Length; i++)
            {
                m_spriteBatch.DrawString(roboto, m_creditsArray[i], new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 150 + i * 50), Color.White);
            }

            m_spriteBatch.End();
        }
    }
}
