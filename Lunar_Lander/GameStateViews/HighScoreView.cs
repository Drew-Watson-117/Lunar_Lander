using Lunar_Lander;
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
    public class HighScoreView : GameStateView
    {

        private List<Score> m_highScores;
        private ScorePersister m_persister;

        private KeyboardInput m_keyboard;
        private GameStateEnum m_nextState;
        private Texture2D backgroundTexture;
        private Texture2D rectangleTexture;
        private SpriteFont roboto;
        public HighScoreView(GameStateEnum myState) : base(myState)
        {
            m_persister = new ScorePersister("HighScores.json");
        }

        public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_keyboard = new KeyboardInput();
            m_nextState = m_myState;
            RegisterCommands();
            m_highScores = null;
            m_persister.Load();
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
            m_highScores = m_persister.getHighScores();

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
            m_spriteBatch.DrawString(roboto, "High Scores", new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 100), Color.Orange, 0f, new Vector2(), 2f, SpriteEffects.None, 0);
            if (m_highScores == null)
            {
                m_spriteBatch.DrawString(roboto, "No High Scores to Display", new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 150), Color.White);
            }
            else
            {
                for (int i = m_highScores.Count - 1; i >= 0; i--)
                {
                    string displayString = " Rank: " + (m_highScores.Count - i).ToString() + "\n   Level: " + m_highScores[i].Level + "\n   Fuel: " + m_highScores[i].Fuel;
                    m_spriteBatch.DrawString(roboto, displayString, new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 160 + (m_highScores.Count - 1 - i) * 60), Color.White);
                }
            }

            m_spriteBatch.End();
        }
    }
}
