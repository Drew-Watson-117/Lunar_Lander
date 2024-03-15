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
    internal class ControlsView : GameStateView
    {
        private (string, Keys)[] m_menuArray;
        private int m_selectedIndex;

        public delegate GameStateEnum UpdateFunction(GameTime gameTime);
        UpdateFunction m_updateFunction;
        public delegate void DrawFunction(GameTime gameTime);
        DrawFunction m_drawFunction;

        private KeyboardInput m_keyboard;
        private GameStateEnum m_nextState;
        private Texture2D backgroundTexture;
        private Texture2D rectangleTexture;
        private SpriteFont roboto;
        private Keys m_thrustKey, m_leftKey, m_rightKey;
        public bool remap = false;
        private Timer m_delayInputTimer;
        private Timer m_flashTimer;
        private Color m_flashingColor;
        public ControlsView(GameStateEnum myState, Keys thrustKey, Keys leftKey, Keys rightKey) : base(myState)
        {
            m_thrustKey = thrustKey;
            m_leftKey = leftKey;
            m_rightKey = rightKey;
            m_menuArray = new (string, Keys)[] {
                ("Thrust: ", m_thrustKey),
                ("Counterclockwise Rotation: ", m_leftKey),
                ("Clockwise Rotation: ", m_rightKey),
            };
            m_selectedIndex = 0;
        }

        public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_updateFunction = MainUpdate;
            m_drawFunction = MainDraw;
            m_keyboard = new KeyboardInput();
            m_nextState = m_myState;
            m_flashingColor = Color.White;
            m_delayInputTimer = new Timer(500);
            
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
            m_keyboard.registerCommand(Keys.Escape, true, MenuBack);
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
            // Switch update function
            m_updateFunction = RemapUpdate;
            m_drawFunction = RemapDraw;
            m_flashTimer = new Timer(500);
        }

        private void MenuBack(GameTime gameTime, float value)
        {
            m_nextState = GameStateEnum.Menu;
        }
        #endregion

        // Getters for keys
        public Keys getThrustKey() { return m_thrustKey; }
        public Keys getLeftKey() { return m_leftKey; }
        public Keys getRightKey() { return m_rightKey; }


        public override void ProcessInput(GameTime gameTime)
        {
            m_keyboard.Update(gameTime);
        }

        public override GameStateEnum Update(GameTime gameTime)
        {
            if (m_delayInputTimer.HasExpired())
            {
                return m_updateFunction(gameTime);
            }
            else
            {
                m_delayInputTimer.Update(gameTime);
                return m_myState;
            }
        }

        private GameStateEnum MainUpdate(GameTime gameTime)
        {
            ProcessInput(gameTime);
            return m_nextState;
        }

        private GameStateEnum RemapUpdate(GameTime gameTime)
        {
            // Toggle Color of Flashing Text
            m_flashTimer.Update(gameTime);
            if (m_flashTimer.HasExpired())
            {
                if (m_flashingColor == Color.White) m_flashingColor = Color.OrangeRed;
                else if (m_flashingColor == Color.OrangeRed) m_flashingColor = Color.White;
                m_flashTimer = new Timer(500);
            }

            Keys[] keys = Keyboard.GetState().GetPressedKeys();
            if (keys.Length > 0)
            {
                Keys key = keys[keys.Length - 1];
                if (key != Keys.Escape && key != Keys.Enter)
                {
                    switch (m_selectedIndex)
                    {
                        case 0:
                            m_thrustKey = key;
                            m_menuArray[0].Item2 = m_thrustKey;
                            break;
                        case 1:
                            m_leftKey = key;
                            m_menuArray[1].Item2 = m_leftKey;
                            break;
                        case 2:
                            m_rightKey = key;
                            m_menuArray[2].Item2 = m_rightKey;
                            break;
                    }
                    remap = true;
                    m_updateFunction = MainUpdate;
                    m_drawFunction = MainDraw;
                }
            }
            return m_nextState;
        }
        public override void Draw(GameTime gameTime)
        {
            m_drawFunction(gameTime);
        }

        public void MainDraw(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            // Render background
            m_spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight), null, Color.White, 0, new Vector2(), SpriteEffects.None, 0);

            // Render backdrop for text
            m_spriteBatch.Draw(rectangleTexture, new Rectangle(m_graphics.PreferredBackBufferWidth / 4, 100, m_graphics.PreferredBackBufferWidth / 2, m_graphics.PreferredBackBufferHeight - 200), new Color(Color.Black, 0.5f));

            // Render text
            m_spriteBatch.DrawString(roboto, "Controls", new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 100), Color.Orange, 0f, new Vector2(), 2f, SpriteEffects.None, 0);
            for (int i = 0; i < m_menuArray.Length; i++)
            {
                Color textColor = Color.White;
                if (i == m_selectedIndex) textColor = Color.OrangeRed;
                Keys key = m_menuArray[i].Item2;
                m_spriteBatch.DrawString(roboto, m_menuArray[i].Item1 + key.ToString(), new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 150 + i * 50), textColor);
            }

            m_spriteBatch.End();
        }

        public void RemapDraw(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            // Render background
            m_spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight), null, Color.White, 0, new Vector2(), SpriteEffects.None, 0);

            // Render backdrop for text
            m_spriteBatch.Draw(rectangleTexture, new Rectangle(m_graphics.PreferredBackBufferWidth / 4, 100, m_graphics.PreferredBackBufferWidth / 2, m_graphics.PreferredBackBufferHeight - 200), new Color(Color.Black, 0.5f));

            // Render text
            m_spriteBatch.DrawString(roboto, "Controls", new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 100), Color.Orange, 0f, new Vector2(), 2f, SpriteEffects.None, 0);
            for (int i = 0; i < m_menuArray.Length; i++)
            {
                Color textColor = Color.White;
                if (i == m_selectedIndex) textColor = m_flashingColor;
                Keys key = m_menuArray[i].Item2;
                m_spriteBatch.DrawString(roboto, m_menuArray[i].Item1 + key.ToString(), new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 150 + i * 50), textColor);
            }

            m_spriteBatch.End();
        }
    }
}
